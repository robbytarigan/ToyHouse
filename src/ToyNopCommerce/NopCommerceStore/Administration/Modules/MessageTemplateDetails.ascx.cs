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
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using FredCK.FCKeditorV2;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class MessageTemplateDetailsControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            if (this.MessageTemplate != null)
            {
                StringBuilder allowedTokensString = new StringBuilder();
                string[] allowedTokens = this.MessageService.GetListOfAllowedTokens();
                for (int i = 0; i < allowedTokens.Length; i++)
                {
                    string token = allowedTokens[i];
                    allowedTokensString.Append(token);
                    if (i != allowedTokens.Length - 1)
                        allowedTokensString.Append(", ");
                }
                this.lblAllowedTokens.Text = allowedTokensString.ToString();

                this.lblTemplate.Text = this.MessageTemplate.Name;

                var languages = this.GetLocalizableLanguagesSupported();
                rptrLanguageTabs.DataSource = languages;
                rptrLanguageTabs.DataBind();
                rptrLanguageDivs.DataSource = languages;
                rptrLanguageDivs.DataBind();
            }
            else
                Response.Redirect("MessageTemplates.aspx");
        }

        protected override void OnPreRender(EventArgs e)
        {
            BindJQuery();
            BindJQueryIdTabs();

            base.OnPreRender(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.BindData();
            }
        }

        protected void rptrLanguageDivs_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var ddlEmailAccount = (DropDownList)e.Item.FindControl("ddlEmailAccount");
                var txtBCCEmailAddresses = (TextBox)e.Item.FindControl("txtBCCEmailAddresses");
                var txtSubject = (TextBox)e.Item.FindControl("txtSubject");
                var txtBody = (FCKeditor)e.Item.FindControl("txtBody");
                var cbActive = (CheckBox)e.Item.FindControl("cbActive");
                var lblLanguageId = (Label)e.Item.FindControl("lblLanguageId");
                
                int languageId = int.Parse(lblLanguageId.Text);

                var emailAccounts= this.MessageService.GetAllEmailAccounts();
                ddlEmailAccount.Items.Clear();
                foreach(var emailAccount in emailAccounts)
                {
                    ListItem item = new ListItem(emailAccount.FriendlyName, emailAccount.EmailAccountId.ToString());
                    ddlEmailAccount.Items.Add(item);
                }

                var content = this.MessageService.GetLocalizedMessageTemplate(this.MessageTemplate.Name, languageId);
                if (content != null)
                {
                    CommonHelper.SelectListItem(ddlEmailAccount, content.EmailAccount.EmailAccountId);
                    txtBCCEmailAddresses.Text = content.BccEmailAddresses;
                    txtSubject.Text = content.Subject;
                    txtBody.Value = content.Body;
                    cbActive.Checked = content.IsActive;
                }
                else
                {
                    CommonHelper.SelectListItem(ddlEmailAccount, this.MessageService.DefaultEmailAccount.EmailAccountId);
                }
            }
        }

        protected MessageTemplate Save()
        {
            MessageTemplate messageTemplate = this.MessageService.GetMessageTemplateById(this.MessageTemplateId);

            foreach (RepeaterItem item in rptrLanguageDivs.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    var ddlEmailAccount = (DropDownList)item.FindControl("ddlEmailAccount");
                    var txtBCCEmailAddresses = (TextBox)item.FindControl("txtBCCEmailAddresses");
                    var txtSubject = (TextBox)item.FindControl("txtSubject");
                    var txtBody = (FCKeditor)item.FindControl("txtBody");
                    var cbActive = (CheckBox)item.FindControl("cbActive");
                    var lblLanguageId = (Label)item.FindControl("lblLanguageId");

                    int emailAccountId = int.Parse(ddlEmailAccount.SelectedValue);
                    int languageId = int.Parse(lblLanguageId.Text);
                    string BCCEmailAddresses = txtBCCEmailAddresses.Text;
                    string subject = txtSubject.Text;
                    string body = txtBody.Value;
                    bool active = cbActive.Checked;

                    var content = this.MessageService.GetLocalizedMessageTemplate(this.MessageTemplate.Name, languageId);
                    if (content == null)
                    {
                        content = new LocalizedMessageTemplate()
                        {
                            MessageTemplateId = this.MessageTemplateId,
                            LanguageId = languageId,
                            EmailAccountId = emailAccountId,
                            BccEmailAddresses = BCCEmailAddresses,
                            Subject = subject,
                            Body = body,
                            IsActive = active
                        };
                        this.MessageService.InsertLocalizedMessageTemplate(content);
                    }
                    else
                    {
                        content.EmailAccountId = emailAccountId;
                        content.BccEmailAddresses = BCCEmailAddresses;
                        content.Subject = subject;
                        content.Body = body;
                        content.IsActive = active;
                        this.MessageService.UpdateLocalizedMessageTemplate(content);
                    }
                }
            }

            return messageTemplate;
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    MessageTemplate messageTemplate = Save();
                    Response.Redirect("MessageTemplates.aspx");
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }

        protected void SaveAndStayButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    MessageTemplate messageTemplate = Save();
                    Response.Redirect(string.Format("MessageTemplateDetails.aspx?MessageTemplateID={0}", messageTemplate.MessageTemplateId));
                }
                catch (Exception exc)
                {
                    ProcessException(exc);
                }
            }
        }

        private MessageTemplate _messageTemplate;
        public MessageTemplate MessageTemplate
        {
            get
            {
                if (_messageTemplate == null)
                {
                    _messageTemplate = this.MessageService.GetMessageTemplateById(this.MessageTemplateId);
                }
                return _messageTemplate;
            }
        }

        public int MessageTemplateId
        {
            get
            {
                return CommonHelper.QueryStringInt("MessageTemplateId");
            }
        }
    }
}