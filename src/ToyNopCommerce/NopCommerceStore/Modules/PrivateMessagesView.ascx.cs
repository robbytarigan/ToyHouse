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
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Content.Forums;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Utils.Html;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class PrivateMessagesViewControl: BaseNopFrontendUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindData();
            }
        }

        private void BindData()
        {
            var pm = this.ForumService.GetPrivateMessageById(this.PrivateMessageId);
            if (pm != null)
            {
                if (pm.ToUserId != NopContext.Current.User.CustomerId && pm.FromUserId != NopContext.Current.User.CustomerId)
                {
                    Response.Redirect(CommonHelper.GetStoreLocation() + "privatemessages.aspx");
                }

                if (!pm.IsRead && pm.ToUserId == NopContext.Current.User.CustomerId)
                {
                    pm.IsRead = true;
                    this.ForumService.UpdatePrivateMessage(pm);
                }
            }
            else
            {
                Response.Redirect(CommonHelper.GetStoreLocation() + "privatemessages.aspx");
            }

            lblFrom.Text = Server.HtmlEncode(pm.FromUser.FormatUserName());
            lblTo.Text = Server.HtmlEncode(pm.ToUser.FormatUserName());
            lblSubject.Text = Server.HtmlEncode(pm.Subject);
            lblMessage.Text = pm.FormatPrivateMessageText();
        }

        protected void btnReply_Click(object sender, EventArgs e)
        {
            var pm = this.ForumService.GetPrivateMessageById(this.PrivateMessageId);
            if (pm != null)
            {
                string replyURL = string.Format("{0}sendpm.aspx?r={1}", CommonHelper.GetStoreLocation(), pm.PrivateMessageId);
                Response.Redirect(replyURL);
            }
            else
            {
                Response.Redirect(CommonHelper.GetStoreLocation() + "privatemessages.aspx");
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            var pm = this.ForumService.GetPrivateMessageById(this.PrivateMessageId);
            if (pm != null)
            {
                if (pm.FromUserId == NopContext.Current.User.CustomerId)
                {
                    pm.IsDeletedByAuthor = true;
                    this.ForumService.UpdatePrivateMessage(pm);
                }

                if (pm != null)
                {
                    if (pm.ToUserId == NopContext.Current.User.CustomerId)
                    {
                        pm.IsDeletedByRecipient = true;
                        this.ForumService.UpdatePrivateMessage(pm);
                    }
                }
            }
            Response.Redirect(CommonHelper.GetStoreLocation() + "privatemessages.aspx");
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(CommonHelper.GetStoreLocation() + "privatemessages.aspx");
        }

        public int PrivateMessageId
        {
            get
            {
                return CommonHelper.QueryStringInt("PM");
            }
        }
    }
}
