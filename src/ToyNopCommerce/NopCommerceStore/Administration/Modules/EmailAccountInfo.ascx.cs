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
using System.Net.Mail;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Messages;
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class EmailAccountInfoControl : BaseNopAdministrationUserControl
    {
        private void BindData()
        {
            var emailAccount = this.MessageService.GetEmailAccountById(this.EmailAccountId);
            if (emailAccount != null)
            {
                this.txtEmailAddress.Text = emailAccount.Email;
                this.txtDisplayName.Text = emailAccount.DisplayName;
                this.txtHost.Text = emailAccount.Host;
                this.txtPort.Text = emailAccount.Port.ToString();
                this.txtUser.Text = emailAccount.Username;
                this.txtPassword.Text = emailAccount.Password;
                this.cbEnableSsl.Checked = emailAccount.EnableSSL;
                this.cbUseDefaultCredentials.Checked = emailAccount.UseDefaultCredentials;

                phTestEmail.Visible = true;
            }
            else
            {
                phTestEmail.Visible = false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.BindData();
            }
        }

        public EmailAccount SaveInfo()
        {
            string email = txtEmailAddress.Text;
            string displayName = txtDisplayName.Text;
            string host = txtHost.Text;
            int port = 0;
            if (!String.IsNullOrEmpty(txtPort.Text))
                port = Convert.ToInt32(txtPort.Text);
            string username = txtUser.Text;
            string password = txtPassword.Text;
            bool enableSSL = cbEnableSsl.Checked;
            bool useDefaultCredentials = cbUseDefaultCredentials.Checked;

            var emailAccount = this.MessageService.GetEmailAccountById(this.EmailAccountId);
            if (emailAccount != null)
            {
                emailAccount.Email = email;
                emailAccount.DisplayName = displayName;
                emailAccount.Host = host;
                emailAccount.Port = port;
                emailAccount.Username = username;
                emailAccount.Password = password;
                emailAccount.EnableSSL = enableSSL;
                emailAccount.UseDefaultCredentials = useDefaultCredentials;

                this.MessageService.UpdateEmailAccount(emailAccount);
            }
            else
            {
                emailAccount = new EmailAccount()
                {
                    Email = email,
                    DisplayName = displayName,
                    Host = host,
                    Port = port,
                    Username = username,
                    Password = password,
                    EnableSSL = enableSSL,
                    UseDefaultCredentials = useDefaultCredentials
                };
                this.MessageService.InsertEmailAccount(emailAccount);
            }

            return emailAccount;
        }

        protected void btnSendTestEmail_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    var emailAccount = this.MessageService.GetEmailAccountById(this.EmailAccountId);
                    if (emailAccount == null)
                        throw new NopException("Email account could not be loaded");

                    MailAddress from = new MailAddress(emailAccount.Email, emailAccount.DisplayName);
                    MailAddress to = new MailAddress(txtSendEmailTo.Text);
                    string subject = this.SettingManager.StoreName + ". Testing email functionaly.";
                    string body = "Email works fine.";
                    this.MessageService.SendEmail(subject, body, from, to, emailAccount);
                    lblSendTestEmailResult.Text = GetLocaleResourceString("Admin.EmailAccountInfo.SendTestEmailSuccess");
                }
                catch (Exception exc)
                {
                    lblSendTestEmailResult.Text = exc.Message;
                }
            }
        }

        public int EmailAccountId
        {
            get
            {
                return CommonHelper.QueryStringInt("EmailAccountId");
            }
        }
    }
}