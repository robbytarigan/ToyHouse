﻿using System;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class NewsLetterSubscriptionActivationControl: BaseNopFrontendUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                var NewsLetterSubscriptionGuid = CommonHelper.QueryStringGuid("T");
                bool IsActive = CommonHelper.QueryStringBool("Active");
                if(!NewsLetterSubscriptionGuid.HasValue)
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }

                var subscription = this.MessageService.GetNewsLetterSubscriptionByGuid(NewsLetterSubscriptionGuid.Value);
                if(subscription == null)
                {
                    Response.Redirect(CommonHelper.GetStoreLocation());
                }

                subscription.Active = IsActive;
               this.MessageService.UpdateNewsLetterSubscription(subscription);

                if(subscription.Active)
                {
                    lblActivationResult.Text = GetLocaleResourceString("NewsLetterSubscriptionActivation.ResultActivated");
                }
                else
                {
                    lblActivationResult.Text = GetLocaleResourceString("NewsLetterSubscriptionActivation.ResultDectivated");
                }
            }
        }
    }
}