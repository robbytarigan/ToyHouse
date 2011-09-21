//------------------------------------------------------------------------------
// The contents of this file are subject to the nopCommerce Public License Version 1.0 ("License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  http://www.nopCommerce.com/License.aspx. 
// 
// Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// See the License for the specific language governing rights and limitations under the License.
// 
// The Original Code is nopCommerce.
// The Initial Developer of the Original Code is NopSolutions.
// All Rights Reserved.
// 
// Contributor(s): _______. 
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products.Attributes;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.Tax;

namespace NopSolutions.NopCommerce.BusinessLogic.Products
{
    /// <summary>
    /// Price helper
    /// </summary>
    public partial class PriceHelper
    {
        #region Utilities

        /// <summary>
        /// Gets allowed discounts
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="customer">Customer</param>
        /// <returns>Discounts</returns>
        protected static List<Discount> GetAllowedDiscounts(ProductVariant productVariant, Customer customer)
        {
            var allowedDiscounts = new List<Discount>();

            string customerCouponCode = string.Empty;
            if (customer != null)
                customerCouponCode = customer.LastAppliedCouponCode;

            foreach (var discount in productVariant.AllDiscounts)
            {
                if (discount.IsActive(customerCouponCode) &&
                    discount.DiscountType == DiscountTypeEnum.AssignedToSKUs &&
                    !allowedDiscounts.ContainsDiscount(discount.Name))
                {
                    //discount requirements
                    if (discount.CheckDiscountRequirements(customer)
                        && discount.CheckDiscountLimitations(customer))
                    {
                        allowedDiscounts.Add(discount);
                    }
                }
            }

            var productCategories = IoC.Resolve<ICategoryService>().GetProductCategoriesByProductId(productVariant.ProductId);
            foreach (var productCategory in productCategories)
            {
                //UNDONE should we filter categories by ACL here?
                var categoryDiscounts = IoC.Resolve<IDiscountService>().GetDiscountsByCategoryId(productCategory.CategoryId);
                foreach (var discount in categoryDiscounts)
                {
                    if (discount.IsActive(customerCouponCode) &&
                        discount.DiscountType == DiscountTypeEnum.AssignedToCategories &&
                        !allowedDiscounts.ContainsDiscount(discount.Name))
                    {
                        //discount requirements
                        if (discount.CheckDiscountRequirements(customer)
                            && discount.CheckDiscountLimitations(customer))
                        {
                            allowedDiscounts.Add(discount);
                        }
                    }
                }
            }
            return allowedDiscounts;
        }

        /// <summary>
        /// Gets a preferred discount
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="customer">Customer</param>
        /// <returns>Preferred discount</returns>
        protected static Discount GetPreferredDiscount(ProductVariant productVariant, 
            Customer customer)
        {
            return GetPreferredDiscount(productVariant, customer, decimal.Zero);
        }

        /// <summary>
        /// Gets a preferred discount
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="customer">Customer</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <returns>Preferred discount</returns>
        protected static Discount GetPreferredDiscount(ProductVariant productVariant, 
            Customer customer, decimal additionalCharge)
        {
            return GetPreferredDiscount(productVariant, customer, additionalCharge, 1);
        }

        /// <summary>
        /// Gets a preferred discount
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="customer">Customer</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="quantity">Product quantity</param>
        /// <returns>Preferred discount</returns>
        protected static Discount GetPreferredDiscount(ProductVariant productVariant,
            Customer customer, decimal additionalCharge, int quantity)
        {
            var allowedDiscounts = GetAllowedDiscounts(productVariant, customer);
            decimal finalPriceWithoutDiscount = GetFinalPrice(productVariant, customer, additionalCharge, false, quantity);
            var preferredDiscount = IoC.Resolve<IDiscountService>().GetPreferredDiscount(allowedDiscounts, finalPriceWithoutDiscount);
            return preferredDiscount;
        }
      
        /// <summary>
        /// Gets a tier price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="quantity">Quantity</param>
        /// <returns>Price</returns>
        protected static decimal GetTierPrice(ProductVariant productVariant, int quantity)
        {
            var tierPrices = productVariant.TierPrices;

            int previousQty = 1;
            decimal previousPrice = productVariant.Price;            
            foreach (TierPrice tierPrice in tierPrices)
            {
                if (quantity < tierPrice.Quantity)
                    continue;

                if (tierPrice.Quantity < previousQty)
                    continue;

                previousPrice = tierPrice.Price; 
                previousQty = tierPrice.Quantity;
            }

            return  previousPrice;
        }

        /// <summary>
        /// Gets a price by customer role (if defined)
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="customer">Customer</param>
        /// <returns>Price</returns>
        protected static decimal? GetCustomPriceByCustomerRole(ProductVariant productVariant, Customer customer)
        {
            if (productVariant == null)
                return null;
            if (customer == null)
                return null;

            decimal? result = null;
            var customerRoles = customer.CustomerRoles;
            var crppCollection = productVariant.CustomerRoleProductPrices;
            foreach (var crpp in crppCollection)
            {
                foreach (var cr in customerRoles)
                {
                    if (cr.CustomerRoleId == crpp.CustomerRoleId)
                    {
                        if (result.HasValue)
                        {
                            if (result.Value > crpp.Price)
                            {
                                result = crpp.Price;
                            }
                        }
                        else
                        {
                            result = crpp.Price;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets currency string
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <returns>Currency string without exchange rate</returns>
        protected static string GetCurrencyString(decimal amount)
        {
            bool showCurrency = true;
            var targetCurrency = NopContext.Current.WorkingCurrency;
            return GetCurrencyString(amount, showCurrency, targetCurrency);
        }

        /// <summary>
        /// Gets currency string
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="targetCurrency">Target currency</param>
        /// <returns>Currency string without exchange rate</returns>
        protected static string GetCurrencyString(decimal amount,
            bool showCurrency, Currency targetCurrency)
        {
            string result = string.Empty;
            if (!String.IsNullOrEmpty(targetCurrency.CustomFormatting))
            {
                result = amount.ToString(targetCurrency.CustomFormatting);
            }
            else
            {
                if (!String.IsNullOrEmpty(targetCurrency.DisplayLocale))
                {
                    result = amount.ToString("C", new CultureInfo(targetCurrency.DisplayLocale));
                }
                else
                {
                    result = String.Format("{0} ({1})", amount.ToString("N"), targetCurrency.CurrencyCode);
                    return result;
                }
            }

            if (showCurrency && IoC.Resolve<ICurrencyService>().GetAllCurrencies().Count > 1)
                result = String.Format("{0} ({1})", result, targetCurrency.CurrencyCode);
            return result;
        }

        #endregion

        #region Methods

        #region Calculation methods

        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <returns>Final price</returns>
        public static decimal GetFinalPrice(ProductVariant productVariant, 
            bool includeDiscounts)
        {
            var customer = NopContext.Current.User;
            return GetFinalPrice(productVariant, customer, includeDiscounts);
        }

        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="customer">The customer</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <returns>Final price</returns>
        public static decimal GetFinalPrice(ProductVariant productVariant, Customer customer, 
            bool includeDiscounts)
        {
            return GetFinalPrice(productVariant, customer, decimal.Zero, includeDiscounts);
        }

        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="customer">The customer</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <returns>Final price</returns>
        public static decimal GetFinalPrice(ProductVariant productVariant, Customer customer, 
            decimal additionalCharge, bool includeDiscounts)
        {
            return GetFinalPrice(productVariant, customer, additionalCharge, 
                includeDiscounts, 1);
        }

        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="customer">The customer</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <param name="quantity">Shopping cart item quantity</param>
        /// <returns>Final price</returns>
        public static decimal GetFinalPrice(ProductVariant productVariant, Customer customer,
            decimal additionalCharge, bool includeDiscounts, int quantity)
        {
            decimal result = decimal.Zero;

            //initial price
            decimal initialPrice = productVariant.Price;

            //price by customer role
            decimal? cpcc = GetCustomPriceByCustomerRole(productVariant, customer);
            if (cpcc.HasValue)
            {
                initialPrice = cpcc.Value;
            }
            
            //tier prices
            if (productVariant.TierPrices.Count > 0)
            {
                decimal tierPrice = GetTierPrice(productVariant, quantity);
                initialPrice = Math.Min(initialPrice, tierPrice);
            }

            //discount + additional charge
            if (includeDiscounts)
            {
                Discount appliedDiscount = null;
                decimal discountAmount = GetDiscountAmount(productVariant, customer, additionalCharge, quantity, out appliedDiscount);
                result = initialPrice + additionalCharge - discountAmount;
            }
            else
            {
                result = initialPrice + additionalCharge;
            }
            if (result < decimal.Zero)
                result = decimal.Zero;
            return result;
        }

        /// <summary>
        /// Gets the shopping cart item sub total
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
        /// <returns>Shopping cart item sub total</returns>
        public static decimal GetSubTotal(ShoppingCartItem shoppingCartItem, bool includeDiscounts)
        {
            var customer = NopContext.Current.User;
            return GetSubTotal(shoppingCartItem, customer, includeDiscounts);
        }

        /// <summary>
        /// Gets the shopping cart item sub total
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <param name="customer">The customer</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
        /// <returns>Shopping cart item sub total</returns>
        public static decimal GetSubTotal(ShoppingCartItem shoppingCartItem, Customer customer, 
            bool includeDiscounts)
        {
            return GetUnitPrice(shoppingCartItem, customer, includeDiscounts) * shoppingCartItem.Quantity;
        }

        /// <summary>
        /// Gets the shopping cart unit price (one item)
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
        /// <returns>Shopping cart unit price (one item)</returns>
        public static decimal GetUnitPrice(ShoppingCartItem shoppingCartItem, bool includeDiscounts)
        {
            var customer = NopContext.Current.User;
            return GetUnitPrice(shoppingCartItem, customer, includeDiscounts);
        }

        /// <summary>
        /// Gets the shopping cart unit price (one item)
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <param name="customer">The customer</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
        /// <returns>Shopping cart unit price (one item)</returns>
        public static decimal GetUnitPrice(ShoppingCartItem shoppingCartItem, Customer customer,
            bool includeDiscounts)
        {
            decimal finalPrice = decimal.Zero;
            var productVariant = shoppingCartItem.ProductVariant;
            if (productVariant != null)
            {
                decimal attributesTotalPrice = decimal.Zero;

                var pvaValues = ProductAttributeHelper.ParseProductVariantAttributeValues(shoppingCartItem.AttributesXml);
                foreach (var pvaValue in pvaValues)
                {
                    attributesTotalPrice += pvaValue.PriceAdjustment;
                }

                if (productVariant.CustomerEntersPrice)
                {
                    finalPrice = shoppingCartItem.CustomerEnteredPrice;
                }
                else
                {
                    finalPrice = GetFinalPrice(productVariant, 
                        customer, 
                        attributesTotalPrice, 
                        includeDiscounts, 
                        shoppingCartItem.Quantity);
                }
            }

            finalPrice = Math.Round(finalPrice, 2);

            return finalPrice;
        }



        /// <summary>
        /// Gets discount amount
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <returns>Discount amount</returns>
        public static decimal GetDiscountAmount(ProductVariant productVariant)
        {
            var customer = NopContext.Current.User;
            return GetDiscountAmount(productVariant, customer, decimal.Zero);
        }

        /// <summary>
        /// Gets discount amount
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="customer">The customer</param>
        /// <returns>Discount amount</returns>
        public static decimal GetDiscountAmount(ProductVariant productVariant, Customer customer)
        {
            return GetDiscountAmount(productVariant, customer, decimal.Zero);
        }

        /// <summary>
        /// Gets discount amount
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="customer">The customer</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <returns>Discount amount</returns>
        public static decimal GetDiscountAmount(ProductVariant productVariant, Customer customer, 
            decimal additionalCharge)
        {
            Discount appliedDiscount = null;
            return GetDiscountAmount(productVariant, customer, additionalCharge, out appliedDiscount);
        }

        /// <summary>
        /// Gets discount amount
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="customer">The customer</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="appliedDiscount">Applied discount</param>
        /// <returns>Discount amount</returns>
        public static decimal GetDiscountAmount(ProductVariant productVariant, Customer customer,
            decimal additionalCharge, out Discount appliedDiscount)
        {
            return GetDiscountAmount(productVariant, customer, additionalCharge, 1, out appliedDiscount);
        }

        /// <summary>
        /// Gets discount amount
        /// </summary>
        /// <param name="productVariant">Product variant</param>
        /// <param name="customer">The customer</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="quantity">Product quantity</param>
        /// <param name="appliedDiscount">Applied discount</param>
        /// <returns>Discount amount</returns>
        public static decimal GetDiscountAmount(ProductVariant productVariant, Customer customer,
            decimal additionalCharge, int quantity, out Discount appliedDiscount)
        {
            decimal appliedDiscountAmount = decimal.Zero;

            //we don't apply discounts to products with price entered by a customer
            if (productVariant.CustomerEntersPrice)
            {
                appliedDiscount = null;
                return appliedDiscountAmount;
            }

            appliedDiscount = GetPreferredDiscount(productVariant, customer, additionalCharge, quantity);
            if (appliedDiscount != null)
            {
                decimal finalPriceWithoutDiscount = GetFinalPrice(productVariant, customer, additionalCharge, false, quantity);
                appliedDiscountAmount = appliedDiscount.GetDiscountAmount(finalPriceWithoutDiscount);
            }

            return appliedDiscountAmount;
        }



        /// <summary>
        /// Gets discount amount
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <returns>Discount amount</returns>
        public static decimal GetDiscountAmount(ShoppingCartItem shoppingCartItem)
        {
            var customer = NopContext.Current.User;
            return GetDiscountAmount(shoppingCartItem, customer);
        }

        /// <summary>
        /// Gets discount amount
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <param name="customer">The customer</param>
        /// <returns>Discount amount</returns>
        public static decimal GetDiscountAmount(ShoppingCartItem shoppingCartItem, Customer customer)
        {
            Discount appliedDiscount = null;
            return GetDiscountAmount(shoppingCartItem, customer, out appliedDiscount);
        }

        /// <summary>
        /// Gets discount amount
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <param name="customer">The customer</param>
        /// <param name="appliedDiscount">Applied discount</param>
        /// <returns>Discount amount</returns>
        public static decimal GetDiscountAmount(ShoppingCartItem shoppingCartItem, Customer customer,
            out Discount appliedDiscount)
        {
            appliedDiscount = null;
            decimal discountAmount = decimal.Zero;
            var productVariant = shoppingCartItem.ProductVariant;
            if (productVariant != null)
            {
                decimal attributesTotalPrice = decimal.Zero;

                var pvaValues = ProductAttributeHelper.ParseProductVariantAttributeValues(shoppingCartItem.AttributesXml);
                foreach (var pvaValue in pvaValues)
                {
                    attributesTotalPrice += pvaValue.PriceAdjustment;
                }

                decimal productVariantDiscountAmount = GetDiscountAmount(productVariant, customer, attributesTotalPrice, shoppingCartItem.Quantity, out appliedDiscount);
                discountAmount = productVariantDiscountAmount * shoppingCartItem.Quantity;
            }

            discountAmount = Math.Round(discountAmount, 2);
            return discountAmount;
        }
        
        #endregion
        
        #region Formatting

        /// <summary>
        /// Formats the price
        /// </summary>
        /// <param name="price">Price</param>
        /// <returns>Price</returns>
        public static string FormatPrice(decimal price)
        {
            bool showCurrency = true;
            var targetCurrency = NopContext.Current.WorkingCurrency;
            return FormatPrice(price, showCurrency, targetCurrency);
        }

        /// <summary>
        /// Formats the price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="targetCurrency">Target currency</param>
        /// <returns>Price</returns>
        public static string FormatPrice(decimal price, bool showCurrency, Currency targetCurrency)
        {
            var language = NopContext.Current.WorkingLanguage;
            bool priceIncludesTax = false;
            switch (NopContext.Current.TaxDisplayType)
            {
                case TaxDisplayTypeEnum.ExcludingTax:
                    priceIncludesTax = false;
                    break;
                case TaxDisplayTypeEnum.IncludingTax:
                    priceIncludesTax = true;
                    break;
            }
            return FormatPrice(price, showCurrency, targetCurrency, language, priceIncludesTax);
        }

        /// <summary>
        /// Formats the price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="showTax">A value indicating whether to show tax suffix</param>
        /// <returns>Price</returns>
        public static string FormatPrice(decimal price, bool showCurrency, bool showTax)
        {
            var targetCurrency = NopContext.Current.WorkingCurrency;
            var language = NopContext.Current.WorkingLanguage;
            bool priceIncludesTax = false;
            switch (NopContext.Current.TaxDisplayType)
            {
                case TaxDisplayTypeEnum.ExcludingTax:
                    priceIncludesTax = false;
                    break;
                case TaxDisplayTypeEnum.IncludingTax:
                    priceIncludesTax = true;
                    break;
            }
            return FormatPrice(price, showCurrency, targetCurrency, language, priceIncludesTax, showTax);
        }

        /// <summary>
        /// Formats the price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="currencyCode">Currency code</param>
        /// <param name="showTax">A value indicating whether to show tax suffix</param>
        /// <returns>Price</returns>
        public static string FormatPrice(decimal price, bool showCurrency, 
            string currencyCode, bool showTax)
        {
            var currency = IoC.Resolve<ICurrencyService>().GetCurrencyByCode(currencyCode);
            if (currency == null)
            {
                currency = new Currency();
                currency.CurrencyCode = currencyCode;
            }
            var language = NopContext.Current.WorkingLanguage;
            bool priceIncludesTax = false;
            switch (NopContext.Current.TaxDisplayType)
            {
                case TaxDisplayTypeEnum.ExcludingTax:
                    priceIncludesTax = false;
                    break;
                case TaxDisplayTypeEnum.IncludingTax:
                    priceIncludesTax = true;
                    break;
            }

            return FormatPrice(price, showCurrency, currency, 
                language, priceIncludesTax, showTax);
        }

        /// <summary>
        /// Formats the price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="currencyCode">Currency code</param>
        /// <param name="language">Language</param>
        /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
        /// <returns>Price</returns>
        public static string FormatPrice(decimal price, bool showCurrency,
            string currencyCode, Language language, bool priceIncludesTax)
        {
            var currency = IoC.Resolve<ICurrencyService>().GetCurrencyByCode(currencyCode);
            if (currency == null)
            {
                currency = new Currency();
                currency.CurrencyCode = currencyCode;
            }
            return FormatPrice(price, showCurrency, currency, language, priceIncludesTax);
        }

        /// <summary>
        /// Formats the price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="targetCurrency">Target currency</param>
        /// <param name="language">Language</param>
        /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
        /// <returns>Price</returns>
        public static string FormatPrice(decimal price, bool showCurrency, 
            Currency targetCurrency, Language language, bool priceIncludesTax)
        {
            bool showTax = IoC.Resolve<ITaxService>().DisplayTaxSuffix;
            return FormatPrice(price, showCurrency, targetCurrency, language, 
                priceIncludesTax, showTax);
        }

        /// <summary>
        /// Formats the price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="targetCurrency">Target currency</param>
        /// <param name="language">Language</param>
        /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
        /// <param name="showTax">A value indicating whether to show tax suffix</param>
        /// <returns>Price</returns>
        public static string FormatPrice(decimal price, bool showCurrency, 
            Currency targetCurrency, Language language, bool priceIncludesTax, bool showTax)
        {
            string currencyString = GetCurrencyString(price, showCurrency, targetCurrency);

            if (showTax)
            {
                var localizationManager = IoC.Resolve<ILocalizationManager>();

                string formatStr = string.Empty;
                if (priceIncludesTax)
                {
                    formatStr = localizationManager.GetLocaleResourceString("Products.InclTaxSuffix", language.LanguageId, false);
                    if (String.IsNullOrEmpty(formatStr))
                    {
                        formatStr = "{0} incl tax";
                    }
                }
                else
                {
                    formatStr = localizationManager.GetLocaleResourceString("Products.ExclTaxSuffix", language.LanguageId, false);
                    if (String.IsNullOrEmpty(formatStr))
                    {
                        formatStr = "{0} excl tax";
                    }
                }
                string taxString = string.Format(formatStr, currencyString);
                return taxString;
            }
            else
            {
                return currencyString;
            }
        }


        /// <summary>
        /// Formats the shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <returns>Price</returns>
        public static string FormatShippingPrice(decimal price, bool showCurrency)
        {
            var targetCurrency = NopContext.Current.WorkingCurrency;
            var language = NopContext.Current.WorkingLanguage;
            bool priceIncludesTax = false;
            switch (NopContext.Current.TaxDisplayType)
            {
                case TaxDisplayTypeEnum.ExcludingTax:
                    priceIncludesTax = false;
                    break;
                case TaxDisplayTypeEnum.IncludingTax:
                    priceIncludesTax = true;
                    break;
            }
            return FormatShippingPrice(price, showCurrency, targetCurrency, language, priceIncludesTax);
        }

        /// <summary>
        /// Formats the shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="targetCurrency">Target currency</param>
        /// <param name="language">Language</param>
        /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
        /// <returns>Price</returns>
        public static string FormatShippingPrice(decimal price, bool showCurrency, 
            Currency targetCurrency, Language language, bool priceIncludesTax)
        {
            bool showTax = IoC.Resolve<ITaxService>().ShippingIsTaxable && IoC.Resolve<ITaxService>().DisplayTaxSuffix;
            return FormatShippingPrice(price, showCurrency, targetCurrency, language, priceIncludesTax, showTax);
        }

        /// <summary>
        /// Formats the shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="targetCurrency">Target currency</param>
        /// <param name="language">Language</param>
        /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
        /// <param name="showTax">A value indicating whether to show tax suffix</param>
        /// <returns>Price</returns>
        public static string FormatShippingPrice(decimal price, bool showCurrency, 
            Currency targetCurrency, Language language, bool priceIncludesTax, bool showTax)
        {
            return FormatPrice(price, showCurrency, targetCurrency, language, priceIncludesTax, showTax);
        }
        
        /// <summary>
        /// Formats the shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="currencyCode">Currency code</param>
        /// <param name="language">Language</param>
        /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
        /// <returns>Price</returns>
        public static string FormatShippingPrice(decimal price, bool showCurrency, 
            string currencyCode, Language language, bool priceIncludesTax)
        {
            var currency = IoC.Resolve<ICurrencyService>().GetCurrencyByCode(currencyCode);
            if (currency == null)
            {
                currency = new Currency();
                currency.CurrencyCode = currencyCode;
            }
            return FormatShippingPrice(price, showCurrency, currency, language, priceIncludesTax);
        }



        /// <summary>
        /// Formats the payment method additional fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <returns>Price</returns>
        public static string FormatPaymentMethodAdditionalFee(decimal price, bool showCurrency)
        {
            var targetCurrency = NopContext.Current.WorkingCurrency;
            var language = NopContext.Current.WorkingLanguage;
            bool priceIncludesTax = false;
            switch (NopContext.Current.TaxDisplayType)
            {
                case TaxDisplayTypeEnum.ExcludingTax:
                    priceIncludesTax = false;
                    break;
                case TaxDisplayTypeEnum.IncludingTax:
                    priceIncludesTax = true;
                    break;
            }
            return FormatPaymentMethodAdditionalFee(price, showCurrency, targetCurrency, 
                language, priceIncludesTax);
        }

        /// <summary>
        /// Formats the payment method additional fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="targetCurrency">Target currency</param>
        /// <param name="language">Language</param>
        /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
        /// <returns>Price</returns>
        public static string FormatPaymentMethodAdditionalFee(decimal price, bool showCurrency,
            Currency targetCurrency, Language language, bool priceIncludesTax)
        {
            bool showTax = IoC.Resolve<ITaxService>().PaymentMethodAdditionalFeeIsTaxable && IoC.Resolve<ITaxService>().DisplayTaxSuffix;
            return FormatPaymentMethodAdditionalFee(price, showCurrency, targetCurrency, language, priceIncludesTax, showTax);
        }

        /// <summary>
        /// Formats the payment method additional fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="targetCurrency">Target currency</param>
        /// <param name="language">Language</param>
        /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
        /// <param name="showTax">A value indicating whether to show tax suffix</param>
        /// <returns>Price</returns>
        public static string FormatPaymentMethodAdditionalFee(decimal price, bool showCurrency, 
            Currency targetCurrency, Language language, bool priceIncludesTax, bool showTax)
        {
            return FormatPrice(price, showCurrency, targetCurrency, language, 
                priceIncludesTax, showTax);
        }

        /// <summary>
        /// Formats the payment method additional fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="showCurrency">A value indicating whether to show a currency</param>
        /// <param name="currencyCode">Currency code</param>
        /// <param name="language">Language</param>
        /// <param name="priceIncludesTax">A value indicating whether price includes tax</param>
        /// <returns>Price</returns>
        public static string FormatPaymentMethodAdditionalFee(decimal price, bool showCurrency, 
            string currencyCode, Language language, bool priceIncludesTax)
        {
            var currency = IoC.Resolve<ICurrencyService>().GetCurrencyByCode(currencyCode);
            if (currency == null)
            {
                currency = new Currency();
                currency.CurrencyCode = currencyCode;
            }
            return FormatPaymentMethodAdditionalFee(price, showCurrency, currency, 
                language, priceIncludesTax);
        }



        /// <summary>
        /// Formats a tax rate
        /// </summary>
        /// <param name="taxRate">Tax rate</param>
        /// <returns>Formatted tax rate</returns>
        public static string FormatTaxRate(decimal taxRate)
        {
            return taxRate.ToString("G29");
        }

        #endregion

        #endregion
    }
}
