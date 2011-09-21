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
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Localization;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class LocaleStringResourceInfoControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            var localeStringResource = this.LocalizationManager.GetLocaleStringResourceById(this.LocaleStringResourceId);
            if (localeStringResource != null)
            {
                Language language = this.LanguageService.GetLanguageById(localeStringResource.LanguageId);
                if (language != null)
                    lblLanguage.Text = Server.HtmlEncode(language.Name);
                else
                    Response.Redirect("LocaleStringResources.aspx");

                this.txtResourceName.Text = localeStringResource.ResourceName;
                this.txtResourceValue.Text = localeStringResource.ResourceValue;
            }
            else
            {
                Language language = this.LanguageService.GetLanguageById(this.LanguageId);
                if (language != null)
                    lblLanguage.Text = Server.HtmlEncode(language.Name);
                else
                    Response.Redirect("LocaleStringResources.aspx");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.BindData();
            }
        }

        public LocaleStringResource SaveInfo()
        {
            LocaleStringResource localeStringResource = this.LocalizationManager.GetLocaleStringResourceById(this.LocaleStringResourceId);

            if (localeStringResource != null)
            {
                localeStringResource.ResourceName = txtResourceName.Text;
                localeStringResource.ResourceValue = txtResourceValue.Text;
                this.LocalizationManager.UpdateLocaleStringResource(localeStringResource);
            }
            else
            {
                localeStringResource = new LocaleStringResource()
                {
                    LanguageId = this.LanguageId,
                    ResourceName = txtResourceName.Text,
                    ResourceValue = txtResourceValue.Text
                };
                this.LocalizationManager.InsertLocaleStringResource(localeStringResource);
            }

            return localeStringResource;
        }

        public int LanguageId
        {
            get
            {
                return CommonHelper.QueryStringInt("LanguageId");
            }
        }

        public int LocaleStringResourceId
        {
            get
            {
                return CommonHelper.QueryStringInt("LocaleStringResourceId");
            }
        }
    }
}