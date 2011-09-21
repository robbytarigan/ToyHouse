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
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.SEO;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web.Modules
{
    public partial class ForumControl: BaseNopFrontendUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Visible = this.ForumService.ForumsEnabled;

            if (!Page.IsPostBack)
            {
                BindData();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ctrlForumBreadcrumb.ForumId = this.ForumId;
        }

        protected void BindData()
        {
            var forum = this.ForumService.GetForumById(this.ForumId);
            if (forum != null)
            {
                //hlNewTopic.Visible = this.ForumService.IsUserAllowedToCreateTopic(NopContext.Current.User, forum);

                lblForumName.Text = Server.HtmlEncode(forum.Name);
                lblForumDescription.Text = Server.HtmlEncode(forum.Description);

                hlNewTopic.NavigateUrl = SEOHelper.GetNewForumTopicUrl(forum.ForumId);

                int pageSize = 10;
                if (this.ForumService.TopicsPageSize > 0)
                {
                    pageSize = this.ForumService.TopicsPageSize;
                }

                var forumTopics = this.ForumService.GetAllTopics(forum.ForumId, 0, string.Empty,
                     ForumSearchTypeEnum.All, 0, this.CurrentPageIndex, pageSize);
                if (forumTopics.Count > 0)
                {
                    this.topicsPager1.PageSize = pageSize;
                    this.topicsPager1.TotalRecords = forumTopics.TotalCount;
                    this.topicsPager1.PageIndex = this.CurrentPageIndex;

                    this.topicsPager2.PageSize = pageSize;
                    this.topicsPager2.TotalRecords = forumTopics.TotalCount;
                    this.topicsPager2.PageIndex = this.CurrentPageIndex;

                    rptrTopics.DataSource = forumTopics;
                    rptrTopics.DataBind();
                }

                //subsciption
                if (this.ForumService.IsUserAllowedToSubscribe(NopContext.Current.User))
                {
                    var forumSubscription = this.ForumService.GetAllSubscriptions(NopContext.Current.User.CustomerId,
                        forum.ForumId, 0, 0, 1).FirstOrDefault();

                    if (forumSubscription == null)
                    {
                        btnWatchForum.Text = GetLocaleResourceString("Forum.WatchForum");
                    }
                    else
                    {
                        btnWatchForum.Text = GetLocaleResourceString("Forum.UnwatchForum");
                    }
                }
                else
                {
                    btnWatchForum.Visible = false;
                }
            }
            else
            {
                Response.Redirect(SEOHelper.GetForumMainUrl());
            }
        }
        
        protected void btnWatchForum_Click(object sender, EventArgs e)
        {
            var forum = this.ForumService.GetForumById(this.ForumId);
            if (forum == null)
                return;

            if (!this.ForumService.IsUserAllowedToSubscribe(NopContext.Current.User))
            {
                string loginURL = SEOHelper.GetLoginPageUrl(true);
                Response.Redirect(loginURL);
            }

            var forumSubscription = this.ForumService.GetAllSubscriptions(NopContext.Current.User.CustomerId,
                   forum.ForumId, 0, 0, 1).FirstOrDefault();

            if (forumSubscription == null)
            {
                forumSubscription = new ForumSubscription()
                {
                    SubscriptionGuid = Guid.NewGuid(),
                    UserId = NopContext.Current.User.CustomerId,
                    ForumId = forum.ForumId,
                    CreatedOn = DateTime.UtcNow
                };
                this.ForumService.InsertSubscription(forumSubscription);
            }
            else
            {
                this.ForumService.DeleteSubscription(forumSubscription.ForumSubscriptionId);
            }

            CommonHelper.ReloadCurrentPage();
        }

        protected void rptrTopics_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var forumTopic = e.Item.DataItem as ForumTopic;
                var customer = forumTopic.User;

                var pnlTopicImage = e.Item.FindControl("pnlTopicImage") as Panel;
                if (pnlTopicImage != null)
                {
                    switch (forumTopic.TopicType)
                    {
                        case ForumTopicTypeEnum.Normal:
                            pnlTopicImage.CssClass = "post";
                            break;
                        case ForumTopicTypeEnum.Sticky:
                            pnlTopicImage.CssClass = "poststicky";
                            break;
                        case ForumTopicTypeEnum.Announcement:
                            pnlTopicImage.CssClass = "postannoucement";
                            break;
                        default:
                            pnlTopicImage.CssClass = "post";
                            break;
                    }
                }

                var lblTopicType = e.Item.FindControl("lblTopicType") as Label;
                if (lblTopicType != null)
                {
                    switch (forumTopic.TopicType)
                    {
                        case ForumTopicTypeEnum.Sticky:
                            lblTopicType.Text = string.Format("[{0}]", GetLocaleResourceString("Forum.Sticky"));
                            break;
                        case ForumTopicTypeEnum.Announcement:
                            lblTopicType.Text = string.Format("[{0}]", GetLocaleResourceString("Forum.Announcement"));
                            break;
                        default:
                            lblTopicType.Visible = false;
                            break;
                    }
                }

                var hlTopic = e.Item.FindControl("hlTopic") as HyperLink;
                if (hlTopic != null)
                {
                    hlTopic.NavigateUrl = SEOHelper.GetForumTopicUrl(forumTopic);
                    hlTopic.Text = Server.HtmlEncode(forumTopic.Subject);
                }

                var lPager = e.Item.FindControl("lPager") as Literal;
                if (lPager != null)
                {
                    string postPager = CreatePostPager(forumTopic);
                    if (!String.IsNullOrEmpty(postPager))
                    {
                        string postPagerFinal = string.Format("<br /><span class=\"topicspager\">{0}</span>", string.Format(GetLocaleResourceString("Forum.Topics.GotoPostPager"), postPager));
                        lPager.Text = postPagerFinal;
                    }
                    else
                        lPager.Visible = false;
                }

                var hlTopicStarter = e.Item.FindControl("hlTopicStarter") as HyperLink;
                if(hlTopicStarter != null)
                {
                    if(customer != null && this.CustomerService.AllowViewingProfiles && !customer.IsGuest)
                    {
                        hlTopicStarter.Text = Server.HtmlEncode(customer.FormatUserName(true));
                        hlTopicStarter.NavigateUrl = SEOHelper.GetUserProfileUrl(customer.CustomerId);
                    }
                    else
                    {
                        hlTopicStarter.Visible = false;
                    }
                }

                var lblTopicStarter = e.Item.FindControl("lblTopicStarter") as Label;
                if(lblTopicStarter != null)
                {
                    if(customer != null && (!this.CustomerService.AllowViewingProfiles || customer.IsGuest))
                    {
                        lblTopicStarter.Text = Server.HtmlEncode(customer.FormatUserName(true));
                    }
                    else
                    {
                        lblTopicStarter.Visible = false;
                    }
                }
            }
        }

        protected string CreatePostPager(ForumTopic forumTopic)
        {
            int pageSize = 10;
            if (this.ForumService.PostsPageSize > 0)
            {
                pageSize = this.ForumService.PostsPageSize;
            }
            string queryStringParam = "p";
            string result = string.Empty;

            int NumToDisplay = 4;
            int PageCount = (int)Math.Ceiling((double)forumTopic.NumPosts / pageSize);

            if (PageCount > 1)
            {
                if (PageCount > NumToDisplay)
                {
                    result += createLink(SEOHelper.GetForumTopicUrl(forumTopic), "1");
                    result += " ... ";
                    bool first = true;

                    for (int i = (PageCount - (NumToDisplay - 1)); i < PageCount; i++)
                    {
                        int iPost = i + 1;

                        if (first)
                            first = false;
                        else
                            result += ", ";

                        result += createLink(SEOHelper.GetForumTopicUrl(forumTopic, queryStringParam, iPost), iPost.ToString());
                    }
                }
                else
                {
                    bool first = true;
                    for (int i = 0; i < PageCount; i++)
                    {
                        int iPost = i + 1;

                        if (first)
                            first = false;
                        else
                            result += ", ";

                        result += createLink(SEOHelper.GetForumTopicUrl(forumTopic, queryStringParam, iPost), iPost.ToString());
                    }
                }
            }
            return result;
        }

        protected string createLink (string link, string text)
        {
            return String.Format("<a href=\"{0}\">{1}</a>", link, text);
        }

        public int CurrentPageIndex
        {
            get
            {
                int _pageIndex = CommonHelper.QueryStringInt(topicsPager1.QueryStringProperty);
                _pageIndex--;
                if (_pageIndex < 0)
                    _pageIndex = 0;
                return _pageIndex;
            }
        }

        public int ForumId
        {
            get
            {
                return CommonHelper.QueryStringInt("ForumId");
            }
        }
    }
}
