﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Orders;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Payment.Methods.PayPoint;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web
{
    public partial class PayPointHostedPaymentReturn : BaseNopFrontendPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(NopContext.Current.User == null)
            {
                string loginURL = SEOHelper.GetLoginPageUrl(true);
                Response.Redirect(loginURL);
            }

            CommonHelper.SetResponseNoCache(Response);

            if(!Page.IsPostBack)
            {

                if(!PayPointHelper.ValidateResponseSign(Request.Url))
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }
                if(!CommonHelper.QueryStringBool("valid"))
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }

                int orderId = CommonHelper.QueryStringInt("trans_id");
                Order order = this.OrderService.GetOrderById(orderId);
                if(order == null)
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }
                if(NopContext.Current.User.CustomerId != order.CustomerId)
                {
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
