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
using System.Collections.ObjectModel;
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
using NopSolutions.NopCommerce.BusinessLogic.Audit;
using NopSolutions.NopCommerce.BusinessLogic.Content.Forums;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.Common.Xml;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class PrivateMessagesInboxControl: BaseNopFrontendUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            BindJQuery();

            if (gvInbox.Rows.Count > 0)
            {
                btnDeleteSelected.Visible = true;
                btnMarkAsUnread.Visible = true;
            }
            else
            {
                btnDeleteSelected.Visible = false;
                btnMarkAsUnread.Visible = false;
            }
            base.OnPreRender(e);
        }

        protected string GetFromInfo(int customerId)
        {
            string customerInfo = string.Empty;
            var customer = this.CustomerService.GetCustomerById(customerId);
            if (customer != null && !customer.IsGuest)
            {
                if (this.CustomerService.AllowViewingProfiles)
                {
                    customerInfo = string.Format("<a href=\"{0}\">{1}</a>", SEOHelper.GetUserProfileUrl(customer.CustomerId), Server.HtmlEncode(customer.FormatUserName()));
                }
                else
                {
                    customerInfo = Server.HtmlEncode(customer.FormatUserName());
                }           
            }
            return customerInfo; 
        }

        protected string GetSubjectInfo(PrivateMessage pm)
        {
            string result = string.Empty;
            string subjectInfo = string.Empty;
            if (pm.IsRead)
            {
                subjectInfo = Server.HtmlEncode(pm.Subject);
            }
            else
            {
                subjectInfo = string.Format("<b>{0}</b>", Server.HtmlEncode(pm.Subject));
            }

            result = string.Format("<a href=\"{0}viewpm.aspx?pm={1}\">{2}</a>", CommonHelper.GetStoreLocation(), pm.PrivateMessageId, subjectInfo);
            return result;
        }

        protected void btnDeleteSelected_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    foreach (GridViewRow row in gvInbox.Rows)
                    {
                        var cbSelect = row.FindControl("cbSelect") as CheckBox;
                        var hfPrivateMessageId = row.FindControl("hfPrivateMessageId") as HiddenField;
                        if (cbSelect != null && hfPrivateMessageId != null)
                        {
                            bool selected = cbSelect.Checked;
                            int pmId = int.Parse(hfPrivateMessageId.Value);
                            if (selected)
                            {
                                PrivateMessage pm = this.ForumService.GetPrivateMessageById(pmId);
                                if (pm != null)
                                {
                                    if (pm.ToUserId == NopContext.Current.User.CustomerId)
                                    {
                                        pm.IsDeletedByRecipient = true;
                                        this.ForumService.UpdatePrivateMessage(pm);
                                    }
                                }
                            }
                        }
                    }

                    Response.Redirect(CommonHelper.GetStoreLocation() + "privatemessages.aspx");
                }
                catch (Exception exc)
                {
                    this.LogService.InsertLog(LogTypeEnum.CustomerError, exc.Message, exc);
                }
            }
        }

        protected void btnMarkAsUnread_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    foreach (GridViewRow row in gvInbox.Rows)
                    {
                        var cbSelect = row.FindControl("cbSelect") as CheckBox;
                        var hfPrivateMessageId = row.FindControl("hfPrivateMessageId") as HiddenField;
                        if (cbSelect != null && hfPrivateMessageId != null)
                        {
                            bool selected = cbSelect.Checked;
                            int pmId = int.Parse(hfPrivateMessageId.Value);
                            if (selected)
                            {
                                PrivateMessage pm = this.ForumService.GetPrivateMessageById(pmId);
                                if (pm != null && pm.IsRead)
                                {
                                    if (pm.ToUserId == NopContext.Current.User.CustomerId)
                                    {
                                        pm.IsRead = false;
                                        this.ForumService.UpdatePrivateMessage(pm);
                                    }
                                }
                            }
                        }
                    }

                    Response.Redirect(CommonHelper.GetStoreLocation() + "privatemessages.aspx");
                }
                catch (Exception exc)
                {
                    this.LogService.InsertLog(LogTypeEnum.CustomerError, exc.Message, exc);
                }
            }
        }
    }
}