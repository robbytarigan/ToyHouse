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
using System.Collections.Generic;
using System.Linq;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
using NopSolutions.NopCommerce.BusinessLogic.Media;
using NopSolutions.NopCommerce.BusinessLogic.Products;
using NopSolutions.NopCommerce.BusinessLogic.Promo.Discounts;
using NopSolutions.NopCommerce.BusinessLogic.Templates;

namespace NopSolutions.NopCommerce.BusinessLogic.Categories
{
    /// <summary>
    /// Represents a category
    /// </summary>
    public partial class Category : BaseEntity
    {
        #region Fields
        private List<CategoryLocalized> _categoryLocalized;
        #endregion
        
        #region Properties
        /// <summary>
        /// Gets or sets the category identifier
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the template identifier
        /// </summary>
        public int TemplateId { get; set; }

        /// <summary>
        /// Gets or sets the meta keywords
        /// </summary>
        public string MetaKeywords { get; set; }

        /// <summary>
        /// Gets or sets the meta description
        /// </summary>
        public string MetaDescription { get; set; }

        /// <summary>
        /// Gets or sets the meta title
        /// </summary>
        public string MetaTitle { get; set; }

        /// <summary>
        /// Gets or sets the search-engine name
        /// </summary>
        public string SEName { get; set; }

        /// <summary>
        /// Gets or sets the parent category identifier
        /// </summary>
        public int ParentCategoryId { get; set; }

        /// <summary>
        /// Gets or sets the picture identifier
        /// </summary>
        public int PictureId { get; set; }

        /// <summary>
        /// Gets or sets the page size
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the available price ranges
        /// </summary>
        public string PriceRanges { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the category on home page
        /// </summary>
        public bool ShowOnHomePage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is published
        /// </summary>
        public bool Published { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance update
        /// </summary>
        public DateTime UpdatedOn { get; set; }
        #endregion

        #region Localizable methods/properties
        
        /// <summary>
        /// Gets the localized name
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Localized name</returns>
        public string GetLocalizedName(int languageId)
        {
            if (NopContext.Current.LocalizedEntityPropertiesEnabled)
            {
                if (languageId > 0)
                {
                    if (_categoryLocalized == null)
                        _categoryLocalized = IoC.Resolve<ICategoryService>().GetCategoryLocalizedByCategoryId(this.CategoryId);

                    var temp1 = _categoryLocalized.FirstOrDefault(cl => cl.LanguageId == languageId);
                    if (temp1 != null && !String.IsNullOrWhiteSpace(temp1.Name))
                        return temp1.Name;
                }
            }

            return this.Name;
        }

        /// <summary>
        /// Gets the localized name 
        /// </summary>
        public string LocalizedName
        {
            get
            {
                return GetLocalizedName(NopContext.Current.WorkingLanguage.LanguageId);
            }
        }

        /// <summary>
        /// Gets the localized description
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Localized description</returns>
        public string GetLocalizedDescription(int languageId)
        {
            if (NopContext.Current.LocalizedEntityPropertiesEnabled)
            {
                if (languageId > 0)
                {
                    if (_categoryLocalized == null)
                        _categoryLocalized = IoC.Resolve<ICategoryService>().GetCategoryLocalizedByCategoryId(this.CategoryId);

                    var temp1 = _categoryLocalized.FirstOrDefault(cl => cl.LanguageId == languageId);
                    if (temp1 != null && !String.IsNullOrWhiteSpace(temp1.Description))
                        return temp1.Description;
                }
            }

            return this.Description;
        }

        /// <summary>
        /// Gets the localized description 
        /// </summary>
        public string LocalizedDescription
        {
            get
            {
                return GetLocalizedDescription(NopContext.Current.WorkingLanguage.LanguageId);
            }
        }

        /// <summary>
        /// Gets the localized meta keywords
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Localized meta keywords</returns>
        public string GetLocalizedMetaKeywords(int languageId)
        {
            if (NopContext.Current.LocalizedEntityPropertiesEnabled)
            {
                if (languageId > 0)
                {
                    if (_categoryLocalized == null)
                        _categoryLocalized = IoC.Resolve<ICategoryService>().GetCategoryLocalizedByCategoryId(this.CategoryId);

                    var temp1 = _categoryLocalized.FirstOrDefault(cl => cl.LanguageId == languageId);
                    if (temp1 != null && !String.IsNullOrWhiteSpace(temp1.MetaKeywords))
                        return temp1.MetaKeywords;
                }
            }

            return this.MetaKeywords;
        }

        /// <summary>
        /// Gets the localized meta keywords 
        /// </summary>
        public string LocalizedMetaKeywords
        {
            get
            {
                return GetLocalizedMetaKeywords(NopContext.Current.WorkingLanguage.LanguageId);
            }
        }

        /// <summary>
        /// Gets the localized meta description
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Localized meta description</returns>
        public string GetLocalizedMetaDescription(int languageId)
        {
            if (NopContext.Current.LocalizedEntityPropertiesEnabled)
            {
                if (languageId > 0)
                {
                    if (_categoryLocalized == null)
                        _categoryLocalized = IoC.Resolve<ICategoryService>().GetCategoryLocalizedByCategoryId(this.CategoryId);

                    var temp1 = _categoryLocalized.FirstOrDefault(cl => cl.LanguageId == languageId);
                    if (temp1 != null && !String.IsNullOrWhiteSpace(temp1.MetaDescription))
                        return temp1.MetaDescription;
                }
            }

            return this.MetaDescription;
        }

        /// <summary>
        /// Gets the localized meta description
        /// </summary>
        public string LocalizedMetaDescription
        {
            get
            {
                return GetLocalizedMetaDescription(NopContext.Current.WorkingLanguage.LanguageId);
            }
        }

        /// <summary>
        /// Gets the localized meta title 
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Localized meta title </returns>
        public string GetLocalizedMetaTitle(int languageId)
        {
            if (NopContext.Current.LocalizedEntityPropertiesEnabled)
            {
                if (languageId > 0)
                {
                    if (_categoryLocalized == null)
                        _categoryLocalized = IoC.Resolve<ICategoryService>().GetCategoryLocalizedByCategoryId(this.CategoryId);

                    var temp1 = _categoryLocalized.FirstOrDefault(cl => cl.LanguageId == languageId);
                    if (temp1 != null && !String.IsNullOrWhiteSpace(temp1.MetaTitle))
                        return temp1.MetaTitle;
                }
            }

            return this.MetaTitle;
        }

        /// <summary>
        /// Gets the localized meta title 
        /// </summary>
        public string LocalizedMetaTitle
        {
            get
            {
                return GetLocalizedMetaTitle(NopContext.Current.WorkingLanguage.LanguageId);
            }
        }

        /// <summary>
        /// Gets the localized search-engine name
        /// </summary>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Localized search-engine name</returns>
        public string GetLocalizedSEName(int languageId)
        {
            if (NopContext.Current.LocalizedEntityPropertiesEnabled)
            {
                if (languageId > 0)
                {
                    if (_categoryLocalized == null)
                        _categoryLocalized = IoC.Resolve<ICategoryService>().GetCategoryLocalizedByCategoryId(this.CategoryId);

                    var temp1 = _categoryLocalized.FirstOrDefault(cl => cl.LanguageId == languageId);
                    if (temp1 != null && !String.IsNullOrWhiteSpace(temp1.SEName))
                        return temp1.SEName;
                }
            }

            return this.SEName;
        }

        /// <summary>
        /// Gets the localized search-engine name 
        /// </summary>
        public string LocalizedSEName
        {
            get
            {
                return GetLocalizedSEName(NopContext.Current.WorkingLanguage.LanguageId);
            }
        }

        #endregion

        #region Custom Properties

        /// <summary>
        /// Gets the parent category
        /// </summary>
        public Category ParentCategory
        {
            get
            {
                return IoC.Resolve<ICategoryService>().GetCategoryById(this.ParentCategoryId);
            }
        }

        /// <summary>
        /// Gets the category template
        /// </summary>
        public CategoryTemplate CategoryTemplate
        {
            get
            {
                return IoC.Resolve<ITemplateService>().GetCategoryTemplateById(this.TemplateId);
            }
        }

        /// <summary>
        /// Gets the products
        /// </summary>
        public List<ProductCategory> ProductCategories
        {
            get
            {
                return IoC.Resolve<ICategoryService>().GetProductCategoriesByCategoryId(this.CategoryId);
            }
        }

        /// <summary>
        /// Gets the picture
        /// </summary>
        public Picture Picture
        {
            get
            {
                return IoC.Resolve<IPictureService>().GetPictureById(this.PictureId);
            }
        }

        /// <summary>
        /// Gets the discounts of the category
        /// </summary>
        public List<Discount> Discounts
        {
            get
            {
                return IoC.Resolve<IDiscountService>().GetDiscountsByCategoryId(this.CategoryId);
            }
        }

        /// <summary>
        /// Gets the featured products of the category
        /// </summary>
        public List<Product> FeaturedProducts
        {
            get
            {
                int totalFeaturedRecords;
                var featuredProducts = IoC.Resolve<IProductService>().GetAllProducts(this.CategoryId,
                    0, 0, true, int.MaxValue - 1, 0, out totalFeaturedRecords);
                return featuredProducts;
            }
        }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the localized category
        /// </summary>
        public virtual ICollection<CategoryLocalized> NpCategoryLocalized { get; set; }

        /// <summary>
        /// Gets the discount
        /// </summary>
        public virtual ICollection<Discount> NpDiscounts { get; set; }

        /// <summary>
        /// Gets the product categories
        /// </summary>
        public virtual ICollection<ProductCategory> NpProductCategories { get; set; }

        #endregion
    }
}
