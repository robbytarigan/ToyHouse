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
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Content.Forums;
using NopSolutions.NopCommerce.BusinessLogic.CustomerManagement;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Utils.Html;
using NopSolutions.NopCommerce.Common.Xml;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class ProfileInfoControl: BaseNopFrontendUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                BindData();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            gvLP.PageSize = this.ForumService.LatestUserPostsPageSize;
            base.OnInit(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            pnlLatestPosts.Visible = this.ForumService.ForumsEnabled;

            base.OnPreRender(e);
        }

        private void BindData()
        {
            //general info
            var customer = this.CustomerService.GetCustomerById(this.CustomerId);
            if (customer == null)
            {
                this.Visible = false;
                return;
            }

            //avatar
            if (this.CustomerService.AllowCustomersToUploadAvatars)
            {
                phAvatar.Visible = true;
                var customerAvatar = customer.Avatar;
                int avatarSize = this.SettingManager.GetSettingValueInteger("Media.Customer.AvatarSize", 85);
                if (customerAvatar != null)
                {
                    string pictureUrl = this.PictureService.GetPictureUrl(customerAvatar, avatarSize, false);
                    this.imgAvatar.ImageUrl = pictureUrl;
                }
                else
                {
                    if (this.CustomerService.DefaultAvatarEnabled)
                    {
                        string pictureUrl = this.PictureService.GetDefaultPictureUrl(PictureTypeEnum.Avatar, avatarSize);
                        this.imgAvatar.ImageUrl = pictureUrl;
                    }
                    else
                    {
                        phAvatar.Visible = false;
                    }
                }
            }
            else
            {
                phAvatar.Visible = false;
            }

            //name
            phFullName.Visible = false;

            //location
            if (this.CustomerService.ShowCustomersLocation)
            {
                phLocation.Visible = true;
                var country = this.CountryService.GetCountryById(customer.CountryId);
                if (country != null)
                {
                    lblCountry.Text = Server.HtmlEncode(country.Name);
                }
                else
                {
                    phLocation.Visible = false;
                }
            }
            else
            {
                phLocation.Visible = false;
            }

            //private message
            if (this.ForumService.AllowPrivateMessages)
            {
                if (customer != null && !customer.IsGuest)
                {
                    btnSendPM.CustomerId = customer.CustomerId;
                    phPM.Visible = true;
                }
                else
                {
                    phPM.Visible = false;
                }
            }
            else
            {
                phPM.Visible = false;
            }

            //total forum posts
            if (this.ForumService.ForumsEnabled && this.ForumService.ShowCustomersPostCount)
            {
                phTotalPosts.Visible = true;
                lblTotalPosts.Text = customer.TotalForumPosts.ToString();
            }
            else
            {
                phTotalPosts.Visible = false;
            }

            //registration date
            if (this.CustomerService.ShowCustomersJoinDate)
            {
                phJoinDate.Visible = true;
                lblJoinDate.Text = DateTimeHelper.ConvertToUserTime(customer.RegistrationDate, DateTimeKind.Utc).ToString("f");
            }
            else
            {
                phJoinDate.Visible = false;
            }

            //birth date
            if (customer.DateOfBirth.HasValue)
            {
                lblDateOfBirth.Text = customer.DateOfBirth.Value.ToString("D");
            }
            else
            {
                phDateOfBirth.Visible = false;
            }
        }

        protected void gvLP_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ForumPost forumPost = (ForumPost)e.Row.DataItem;

                //topic info
                HyperLink hlTopic = e.Row.FindControl("hlTopic") as HyperLink;
                if (hlTopic != null)
                {
                    ForumTopic forumTopic = forumPost.Topic;
                    if (forumTopic != null)
                    {
                        hlTopic.Text = Server.HtmlEncode(forumTopic.Subject);
                        hlTopic.NavigateUrl = SEOHelper.GetForumTopicUrl(forumPost.TopicId);
                    }
                }

                //post date
                Label lblPosted = e.Row.FindControl("lblPosted") as Label;
                if (lblPosted != null)
                {
                    string dateStr = string.Empty;
                    if (this.ForumService.RelativeDateTimeFormattingEnabled)
                        dateStr = forumPost.CreatedOn.RelativeFormat(true, "f");
                    else
                        dateStr = DateTimeHelper.ConvertToUserTime(forumPost.CreatedOn, DateTimeKind.Utc).ToString("f");

                    lblPosted.Text = dateStr;
                }

                //forum post
                Label lblPost = e.Row.FindControl("lblPost") as Label;
                if (lblPost != null)
                {
                    lblPost.Text = forumPost.FormatPostText();
                }
            }
        }

        public int CustomerId
        {
            get
            {
                return CommonHelper.QueryStringInt("UserId");
            }
        }
    }
}