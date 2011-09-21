﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class MiniShoppingCartBoxControl: BaseNopFrontendUserControl
    {
        protected void lvCart_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                var dataItem = e.Item as ListViewDataItem;
                if (dataItem != null)
                {
                    var sci = dataItem.DataItem as ShoppingCartItem;
                    if (sci != null)
                    {
                        var hlProduct = dataItem.FindControl("hlProduct") as HyperLink;
                        if (hlProduct != null)
                        {
                            hlProduct.NavigateUrl = SEOHelper.GetProductUrl(sci.ProductVariant.Product);
                            hlProduct.Text = Server.HtmlEncode(sci.ProductVariant.LocalizedFullProductName);
                        }

                        var lblQty = dataItem.FindControl("lblQty") as Label;
                        if (lblQty != null)
                        {
                            lblQty.Text = string.Format("{0} x ", sci.Quantity);
                        }                        
                    }
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (this.SettingManager.GetSettingValueBoolean("Common.ShowMiniShoppingCart"))
            {
                var shoppingCart = this.ShoppingCartService.GetCurrentShoppingCart(ShoppingCartTypeEnum.ShoppingCart);
                if (shoppingCart.TotalProducts == 0)
                {
                    phCheckoutInfo.Visible = false;
                    lShoppingCart.Text = GetLocaleResourceString("MiniShoppingCartBox.NoItems");

                    lvCart.Visible = false;
                }
                else
                {
                    phCheckoutInfo.Visible = true;
                    if (shoppingCart.TotalProducts == 1)
                    {
                        lShoppingCart.Text = string.Format(GetLocaleResourceString("MiniShoppingCartBox.OneItemText"), string.Format("<a href=\"{0}\" class=\"items\">{1}</a>", SEOHelper.GetShoppingCartUrl(), GetLocaleResourceString("MiniShoppingCartBox.OneItem")));
                    }
                    else
                    {
                        lShoppingCart.Text = string.Format(GetLocaleResourceString("MiniShoppingCartBox.SeveralItemsText"), string.Format("<a href=\"{0}\" class=\"items\">{1}</a>", SEOHelper.GetShoppingCartUrl(), string.Format(GetLocaleResourceString("MiniShoppingCartBox.SeveralItems"), shoppingCart.TotalProducts)));
                    }

                    lblOrderSubtotal.Text = GetLocaleResourceString("MiniShoppingCartBox.OrderSubtotal", GetOrderSubtotal(shoppingCart));

                    if (this.SettingManager.GetSettingValueBoolean("Display.ItemsInMiniShoppingCart", false))
                    {
                        lvCart.Visible = true;
                        lvCart.DataSource = shoppingCart;
                        lvCart.DataBind();
                    }
                    else
                    {
                        lvCart.Visible = false;
                    }

                }
            }
            else
            {
                this.Visible = false;
            }
            base.OnPreRender(e);
        }

        protected void BtnCheckout_OnClick(object sender, EventArgs e)
        {
            Response.Redirect(SEOHelper.GetShoppingCartUrl());
        }

        protected string GetOrderSubtotal(ShoppingCart shoppingCart)
        {
            decimal subtotalBase = decimal.Zero;
            decimal orderSubTotalDiscountAmount = decimal.Zero;
            Discount orderSubTotalAppliedDiscount = null;
            decimal subTotalWithoutDiscountBase = decimal.Zero;
            decimal subTotalWithDiscountBase = decimal.Zero;
            string SubTotalError = this.ShoppingCartService.GetShoppingCartSubTotal(shoppingCart,
                NopContext.Current.User, out orderSubTotalDiscountAmount, out orderSubTotalAppliedDiscount,
                out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
            subtotalBase = subTotalWithoutDiscountBase;
            if (String.IsNullOrEmpty(SubTotalError))
            {
                decimal subTotal = this.CurrencyService.ConvertCurrency(subtotalBase, this.CurrencyService.PrimaryStoreCurrency, NopContext.Current.WorkingCurrency);
                return PriceHelper.FormatPrice(subTotal);
            }
            else
            {
                return GetLocaleResourceString("MiniShoppingCartBox.OrderSubtotal.CalculatedDuringCheckout");
            }
        }
    }
}