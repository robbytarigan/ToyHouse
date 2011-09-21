﻿using System;
using System.Globalization;
using System.IO;
using System.Text;
using NopSolutions.NopCommerce.BusinessLogic.Categories;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils.Html;

namespace NopSolutions.NopCommerce.PriceGrabber
{
    /// <summary>
    /// PriceGrabber service
    /// </summary>
    public static partial class PriceGrabberService
    {
        /// <summary>
        /// Generate PriceGrabber feed
        /// </summary>
        /// <param name="stream">Stream</param>
        public static void GenerateFeed(Stream stream)
        {
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.WriteLine("Unique Retailer SKU;Manufacturer Name;Manufacturer Part Number;Product Title;Categorization;Product URL;Image URL;Detailed Description;Selling Price;Condition;Availability");

                foreach(Product p in IoC.Resolve<IProductService>().GetAllProducts(false))
                {
                    foreach (ProductVariant pv in IoC.Resolve<IProductService>().GetProductVariantsByProductId(p.ProductId, false))
                    {
                        string sku = pv.ProductVariantId.ToString();
                        string manufacturerName = p.ProductManufacturers.Count > 0 ? p.ProductManufacturers[0].Manufacturer.Name : String.Empty;
                        string manufacturerPartNumber = pv.ManufacturerPartNumber;
                        string productTitle = pv.FullProductName;
                        string productUrl = SEOHelper.GetProductUrl(p);

                        string imageUrl = string.Empty;
                        var pictures = IoC.Resolve<IPictureService>().GetPicturesByProductId(p.ProductId, 1);
                        if (pictures.Count > 0)
                            imageUrl = IoC.Resolve<IPictureService>().GetPictureUrl(pictures[0], IoC.Resolve<ISettingManager>().GetSettingValueInteger("PromotionProvider.PriceGrabber.ProductThumbnailImageSize"), true);
                        else
                            imageUrl = IoC.Resolve<IPictureService>().GetDefaultPictureUrl(PictureTypeEnum.Entity, IoC.Resolve<ISettingManager>().GetSettingValueInteger("PromotionProvider.PriceGrabber.ProductThumbnailImageSize"));

                        string description = pv.Description;
                        string price = IoC.Resolve<ICurrencyService>().ConvertCurrency(pv.Price, IoC.Resolve<ICurrencyService>().PrimaryStoreCurrency, PriceGrabberService.UsedCurrency).ToString(new CultureInfo("en-US", false).NumberFormat);
                        string availability = pv.StockQuantity > 0 ? "Yes" : "No";
                        string categorization = "no category";

                        if (String.IsNullOrEmpty(description))
                        {
                            description = p.FullDescription;
                        }
                        if (String.IsNullOrEmpty(description))
                        {
                            description = p.ShortDescription;
                        }
                        if (String.IsNullOrEmpty(description))
                        {
                            description = p.Name;
                        }

                        var productCategories = p.ProductCategories;
                        if (productCategories.Count > 0)
                        {
                            StringBuilder sb = new StringBuilder();
                            foreach (Category cat in IoC.Resolve<ICategoryService>().GetBreadCrumb(productCategories[0].CategoryId))
                            {
                                sb.AppendFormat("{0}>", cat.Name);
                            }
                            sb.Length -= 1;
                            categorization = sb.ToString();
                        }

                        productTitle = RemoveSpecChars(productTitle);
                        manufacturerPartNumber = RemoveSpecChars(manufacturerPartNumber);
                        manufacturerName = RemoveSpecChars(manufacturerName);
                        description = HtmlHelper.StripTags(description);
                        description = RemoveSpecChars(description);
                        categorization = RemoveSpecChars(categorization);

                        writer.WriteLine("{0};{1};{2};{3};{4};{5};{6};{7};{8};New;{9}", 
                            sku, 
                            manufacturerName, 
                            manufacturerPartNumber, 
                            productTitle, 
                            categorization, 
                            productUrl, 
                            imageUrl, 
                            description, 
                            price, 
                            availability);
                    }
                }
            }
        }

        private static string RemoveSpecChars(string s)
        {
            s = s.Replace(';', ',');
            s = s.Replace('\r', ' ');
            s = s.Replace('\n', ' ');
            return s;
        }

        /// <summary>
        /// Gets or sets the currency that is used to generate the feed
        /// </summary>
        public static Currency UsedCurrency
        {
            get
            {
                int currencyId = IoC.Resolve<ISettingManager>().GetSettingValueInteger("PromotionProvider.PriceGrabber.Currency");
                var currency = IoC.Resolve<ICurrencyService>().GetCurrencyById(currencyId);
                if (currency == null || !currency.Published)
                    currency = IoC.Resolve<ICurrencyService>().PrimaryStoreCurrency;
                return currency;
            }
            set
            {
                int id = 0;
                if (value != null)
                    id = value.CurrencyId;
                IoC.Resolve<ISettingManager>().SetParam("PromotionProvider.PriceGrabber.Currency", id.ToString());
            }
        }
    }
}
