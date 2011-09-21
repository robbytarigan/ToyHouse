﻿using System;
using System.Text.RegularExpressions;
using System.Web.UI;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Payment.Methods.Svea;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web
{
    public partial class SveaHostedPaymentReturn : BaseNopFrontendPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if((NopContext.Current.User == null) || NopContext.Current.User.IsGuest)
            {
                Response.Redirect(SEOHelper.GetLoginPageUrl(true));
            }

            CommonHelper.SetResponseNoCache(Response);

            if(!Page.IsPostBack)
            {
                Order order = this.OrderService.GetOrderById(CommonHelper.QueryStringInt("OrderId"));
                if(order == null)
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }
                if(NopContext.Current.User.CustomerId != order.CustomerId)
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }

                string md5 = CommonHelper.QueryString("MD5");

                if(String.IsNullOrEmpty(md5) || !md5.Equals(HostedPaymentHelper.CalcMd5Hash(String.Format("{0}SveaHostedPaymentReturn.aspx{1}{2}", CommonHelper.GetStoreHost(false), Regex.Replace(Request.Url.Query, "&MD5=.*", String.Empty), HostedPaymentSettings.Password))))
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }



                bool success = CommonHelper.QueryStringBool("Success");
                if (!success)
                {
                    string error = "Return page. Success parameter is false. ";
                    string errorCode = CommonHelper.QueryString("ErrorCode");
                    if (!String.IsNullOrEmpty(errorCode))
                    {
                        error += string.Format("Svea error code: {0}.", errorCode);
                    }

                    this.OrderService.InsertOrderNote(order.OrderId, error, DateTime.UtcNow);

                    Response.Redirect(CommonHelper.GetStoreLocation());
                }

                if (this.OrderService.CanMarkOrderAsPaid(order))
                {
                    this.OrderService.MarkOrderAsPaid(order.OrderId);
                }

                Response.Redirect("~/checkoutcompleted.aspx");
            }
        }

        public override bool AllowGuestNavigation
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this page is tracked by 'Online Customers' module
        /// </summary>
        public override bool TrackedByOnlineCustomersModule
        {
            get
            {
                return false;
            }
        }
    }
}
