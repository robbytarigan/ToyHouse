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
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using NopSolutions.NopCommerce.BusinessLogic;
using NopSolutions.NopCommerce.BusinessLogic.Content.Blog;
using NopSolutions.NopCommerce.BusinessLogic.Directory;
using NopSolutions.NopCommerce.BusinessLogic.Profile;
using NopSolutions.NopCommerce.Common.Utils;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;

namespace NopSolutions.NopCommerce.Web.Administration.Modules
{
    public partial class BlogPostInfoControl : BaseNopAdministrationUserControl
    {
        private void FillDropDowns()
        {
            this.ddlLanguage.Items.Clear();
            var languages = this.LanguageService.GetAllLanguages();
            foreach (Language language in languages)
            {
                ListItem item2 = new ListItem(language.Name, language.LanguageId.ToString());
                this.ddlLanguage.Items.Add(item2);
            }
        }

        private void BindData()
        {
            BlogPost blogPost = this.BlogService.GetBlogPostById(this.BlogPostId);
            if (blogPost != null)
            {
                CommonHelper.SelectListItem(this.ddlLanguage, blogPost.LanguageId);
                this.txtBlogPostTitle.Text = blogPost.BlogPostTitle;
                this.txtBlogPostBody.Value = blogPost.BlogPostBody;
                this.cbBlogPostAllowComments.Checked = blogPost.BlogPostAllowComments;
                this.txtTags.Text = blogPost.Tags;

                this.pnlCreatedOn.Visible = true;
                this.lblCreatedOn.Text = DateTimeHelper.ConvertToUserTime(blogPost.CreatedOn, DateTimeKind.Utc).ToString();

                var blogComments = blogPost.BlogComments;
                if (blogComments.Count > 0)
                {
                    this.hlViewComments.Visible = true;
                    this.hlViewComments.Text = string.Format(GetLocaleResourceString("Admin.BlogPostInfo.ViewComments"), blogComments.Count);
                    this.hlViewComments.NavigateUrl = CommonHelper.GetStoreAdminLocation() + "BlogComments.aspx?BlogPostID=" + blogPost.BlogPostId;
                }
                else
                    this.hlViewComments.Visible = false;
            }
            else
            {
                this.pnlCreatedOn.Visible = false;
                hlViewComments.Visible = false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.FillDropDowns();
                this.BindData();
            }
        }

        public BlogPost SaveInfo()
        {
            BlogPost blogPost = this.BlogService.GetBlogPostById(this.BlogPostId);
            if (blogPost != null)
            {
                blogPost.LanguageId = int.Parse(this.ddlLanguage.SelectedItem.Value);
                blogPost.BlogPostTitle = txtBlogPostTitle.Text;
                blogPost.BlogPostBody = txtBlogPostBody.Value;
                blogPost.BlogPostAllowComments = cbBlogPostAllowComments.Checked;
                blogPost.Tags = txtTags.Text.Trim();

                this.BlogService.UpdateBlogPost(blogPost);
            }
            else
            {
                blogPost = new BlogPost()
                {
                    LanguageId = int.Parse(this.ddlLanguage.SelectedItem.Value),
                    BlogPostTitle = txtBlogPostTitle.Text,
                    BlogPostBody = txtBlogPostBody.Value,
                    BlogPostAllowComments = cbBlogPostAllowComments.Checked,
                    Tags = txtTags.Text.Trim(),
                    CreatedById = NopContext.Current.User.CustomerId,
                    CreatedOn = DateTime.UtcNow
                };
                this.BlogService.InsertBlogPost(blogPost);
            }
            return blogPost;
        }

        public int BlogPostId
        {
            get
            {
                return CommonHelper.QueryStringInt("BlogPostId");
            }
        }
    }
}