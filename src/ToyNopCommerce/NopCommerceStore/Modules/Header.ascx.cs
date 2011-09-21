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
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Content.Forums;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class HeaderControl: BaseNopFrontendUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        
        protected void lFinishImpersonate_Click(object sender, EventArgs e)
        {
            if (NopContext.Current.IsCurrentCustomerImpersonated &&
                NopContext.Current.OriginalUser != null)
            {
                NopContext.Current.OriginalUser.ImpersonatedCustomerGuid = Guid.Empty;
                string returnUrl = CommonHelper.GetStoreAdminLocation();
                if (NopContext.Current.User != null)
                {
                    returnUrl = string.Format("{0}CustomerDetails.aspx?CustomerID={1}&TabID={2}", returnUrl, NopContext.Current.User.CustomerId, "pnlCustomerPlaceOrder");
                }
                Response.Redirect(returnUrl);
            }
        }
        
        protected override void OnPreRender(EventArgs e)
        {
            Literal lUnreadPrivateMessages = topLoginView.FindControl("lUnreadPrivateMessages") as Literal;
            if (lUnreadPrivateMessages != null)
            {
                lUnreadPrivateMessages.Text = GetUnreadPrivateMessages();
            }
            base.OnPreRender(e);
        }

        protected string GetUnreadPrivateMessages()
        {
            string result = string.Empty;
            if (this.ForumService.AllowPrivateMessages &&
                NopContext.Current.User != null && !NopContext.Current.User.IsGuest)
            {
                var privateMessages = this.ForumService.GetAllPrivateMessages(0,
                    NopContext.Current.User.CustomerId, false, null, false, string.Empty, 0, 1);

                if (privateMessages.TotalCount > 0)
                {
                    result = string.Format(GetLocaleResourceString("PrivateMessages.TotalUnread"), privateMessages.TotalCount);

                    //notifications here
                    if (this.SettingManager.GetSettingValueBoolean("Common.ShowAlertForPM") &&
                        !NopContext.Current.User.NotifiedAboutNewPrivateMessages)
                    {
                        this.DisplayAlertMessage(string.Format(GetLocaleResourceString("PrivateMessages.YouHaveUnreadPM", privateMessages.TotalCount)));
                        NopContext.Current.User.NotifiedAboutNewPrivateMessages = true;
                    }
                }
            }
            return result;
        }
    }
}