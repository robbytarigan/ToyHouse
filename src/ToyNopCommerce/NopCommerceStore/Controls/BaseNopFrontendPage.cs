﻿//------------------------------------------------------------------------------
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
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Installation;
using NopSolutions.NopCommerce.BusinessLogic.Audit.UsersOnline;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web
{
    public partial class BaseNopFrontendPage : BaseNopPage
    {
        #region Fields

        protected Stopwatch executionTimer;
        protected bool showExecutionTimer = false;

        #endregion

        #region Ctor

        public BaseNopFrontendPage()
        {
            showExecutionTimer = this.SettingManager.GetSettingValueBoolean("Display.PageExecutionTimeInfoEnabled");
            if (showExecutionTimer)
            {
                executionTimer = new Stopwatch();
            }
        }

        #endregion

        #region Overrides

        protected override void OnPreInit(EventArgs e)
        {
            //store is closed
            if (this.SettingManager.GetSettingValueBoolean("Common.StoreClosed"))
            {
                if (NopContext.Current.User != null &&
                    NopContext.Current.User.IsAdmin &&
                    this.SettingManager.GetSettingValueBoolean("Common.StoreClosed.AllowAdminAccess"))
                {
                    //do nothing - allow admin access
                }
                else
                {
                    Response.Redirect("~/StoreClosed.htm");
                }
            }

            //SSL
            switch (this.SslProtected)
            {
                case PageSslProtectionEnum.Yes:
                    {
                        CommonHelper.EnsureSsl();
                    }
                    break;
                case PageSslProtectionEnum.No:
                    {
                        CommonHelper.EnsureNonSsl();
                    }
                    break;
                case PageSslProtectionEnum.DoesntMatter:
                    {
                        //do nothing in this case
                    }
                    break;
            }

            //allow navigation only for registered customers
            if (this.CustomerService.AllowNavigationOnlyRegisteredCustomers)
            {
                if (NopContext.Current.User == null || NopContext.Current.User.IsGuest)
                {
                    if (!this.AllowGuestNavigation)
                    {
                        //it's not login/logout/passwordrecovery/captchaimage/register/accountactivation page (be default)
                        string loginURL = SEOHelper.GetLoginPageUrl(false);
                        Response.Redirect(loginURL);
                    }
                }
            }

            //theme
            if (!String.IsNullOrEmpty(NopContext.Current.WorkingTheme))
            {
                this.Theme = NopContext.Current.WorkingTheme;
            }
            base.OnPreInit(e);
        }

        protected override void OnInit(EventArgs e)
        {
            if (showExecutionTimer)
            {
                executionTimer.Start();
            }

            base.OnInit(e);

            if (showExecutionTimer)
            {
                executionTimer.Stop();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            if (showExecutionTimer)
            {
                executionTimer.Start();
            }

            base.OnLoad(e);

            if (showExecutionTimer)
            {
                executionTimer.Stop();
            }
        }

        protected override void CreateChildControls()
        {
            if (showExecutionTimer)
            {
                executionTimer.Start();
            }

            base.CreateChildControls();

            if (showExecutionTimer)
            {
                executionTimer.Stop();
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (showExecutionTimer)
            {
                executionTimer.Start();
            }

            base.Render(writer);

            if (showExecutionTimer)
            {
                executionTimer.Stop();
                RenderExecutionTimerValue(writer);
            }
        }
        
        protected override void OnPreRender(EventArgs e)
        {
            //java-script
            string publicJS = CommonHelper.GetStoreLocation() + "Scripts/public.js";
            Page.ClientScript.RegisterClientScriptInclude(publicJS, publicJS);

            base.OnPreRender(e);
        }
        #endregion

        #region Utiilities

        protected virtual void RenderExecutionTimerValue(HtmlTextWriter writer)
        {
            if (showExecutionTimer)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(@"<div style=""color:#ffffff;background:#000000;font-weight:bold,padding:5px"">");
                sb.Append(String.Format("Page execution time is {0:F10}.<br />", executionTimer.Elapsed.TotalSeconds));
                sb.Append(@"</div>");
                writer.Write(sb.ToString());
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this page is SSL protected
        /// </summary>
        public virtual PageSslProtectionEnum SslProtected
        {
            get
            {
                return PageSslProtectionEnum.DoesntMatter;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this page can be visited by anonymous customer when "Allow navigation only for registered customers" settings is set to true
        /// </summary>
        public virtual bool AllowGuestNavigation
        {
            get
            {
                return false;
            }
        }
        #endregion
    }
}