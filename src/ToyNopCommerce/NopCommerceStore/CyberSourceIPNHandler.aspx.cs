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
using NopSolutions.NopCommerce.Payment.Methods.CyberSource;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web
{
    public partial class CyberSourceIPNHandler : BaseNopFrontendPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CommonHelper.SetResponseNoCache(Response);

            if(!Page.IsPostBack)
            {
                if(HostedPaymentHelper.ValidateResponseSign(Request.Form))
                {
                    string reasonCode = Request.Form["reasonCode"];
                    if(!String.IsNullOrEmpty(reasonCode) && reasonCode.Equals("100"))
                    {
                        int orderId = 0;
                        if(Int32.TryParse(Request.Form["orderNumber"], out orderId))
                        {
                            Order order = this.OrderService.GetOrderById(orderId);
                            if(order != null && this.OrderService.CanMarkOrderAsAuthorized(order))
                            {
                                this.OrderService.MarkAsAuthorized(order.OrderId);
                            }
                        }
                    }
                }
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
