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
using NopSolutions.NopCommerce.BusinessLogic.Caching;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.BusinessLogic.Promo.Affiliates
{
    /// <summary>
    /// Affiliate service
    /// </summary>
    public partial class AffiliateService : IAffiliateService
    {
        #region Fields

        /// <summary>
        /// Object context
        /// </summary>
        private readonly NopObjectContext _context;

        /// <summary>
        /// Cache manager
        /// </summary>
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public AffiliateService(NopObjectContext context)
        {
            this._context = context;
            this._cacheManager = new NopRequestCache();
        }

        #endregion

        #region Methods
        /// <summary>
        /// Gets an affiliate by affiliate identifier
        /// </summary>
        /// <param name="affiliateId">Affiliate identifier</param>
        /// <returns>Affiliate</returns>
        public Affiliate GetAffiliateById(int affiliateId)
        {
            if (affiliateId == 0)
                return null;

            
            var query = from a in _context.Affiliates
                        where a.AffiliateId == affiliateId
                        select a;
            var affiliate = query.SingleOrDefault();

            return affiliate;
        }

        /// <summary>
        /// Marks affiliate as deleted 
        /// </summary>
        /// <param name="affiliateId">Affiliate identifier</param>
        public void MarkAffiliateAsDeleted(int affiliateId)
        {
            var affiliate = GetAffiliateById(affiliateId);
            if (affiliate != null)
            {
                affiliate.Deleted = true;
                UpdateAffiliate(affiliate);
            }
        }

        /// <summary>
        /// Gets all affiliates
        /// </summary>
        /// <returns>Affiliate collection</returns>
        public List<Affiliate> GetAllAffiliates()
        {
            
            var query = from a in _context.Affiliates
                        orderby a.LastName
                        where !a.Deleted
                        select a;
            var affiliates = query.ToList();

            return affiliates;
        }

        /// <summary>
        /// Inserts an affiliate
        /// </summary>
        /// <param name="affiliate">Affiliate</param>
        public void InsertAffiliate(Affiliate affiliate)
        {
            if (affiliate == null)
                throw new ArgumentNullException("affiliate");
            
            affiliate.FirstName = CommonHelper.EnsureNotNull(affiliate.FirstName);
            affiliate.LastName = CommonHelper.EnsureNotNull(affiliate.LastName);
            affiliate.FirstName = CommonHelper.EnsureMaximumLength(affiliate.FirstName, 100);
            affiliate.LastName = CommonHelper.EnsureMaximumLength(affiliate.LastName, 100);
            affiliate.MiddleName = CommonHelper.EnsureNotNull(affiliate.MiddleName);
            affiliate.MiddleName = CommonHelper.EnsureMaximumLength(affiliate.MiddleName, 100);
            affiliate.PhoneNumber = CommonHelper.EnsureNotNull(affiliate.PhoneNumber);
            affiliate.PhoneNumber = CommonHelper.EnsureMaximumLength(affiliate.PhoneNumber, 50);
            affiliate.Email = CommonHelper.EnsureNotNull(affiliate.Email);
            affiliate.Email = CommonHelper.EnsureMaximumLength(affiliate.Email, 255);
            affiliate.FaxNumber = CommonHelper.EnsureNotNull(affiliate.FaxNumber);
            affiliate.FaxNumber = CommonHelper.EnsureMaximumLength(affiliate.FaxNumber, 50);
            affiliate.Company = CommonHelper.EnsureNotNull(affiliate.Company);
            affiliate.Company = CommonHelper.EnsureMaximumLength(affiliate.Company, 100);
            affiliate.Address1 = CommonHelper.EnsureNotNull(affiliate.Address1);
            affiliate.Address1 = CommonHelper.EnsureMaximumLength(affiliate.Address1, 100);
            affiliate.Address2 = CommonHelper.EnsureNotNull(affiliate.Address2);
            affiliate.Address2 = CommonHelper.EnsureMaximumLength(affiliate.Address2, 100);
            affiliate.City = CommonHelper.EnsureNotNull(affiliate.City);
            affiliate.City = CommonHelper.EnsureMaximumLength(affiliate.City, 100);
            affiliate.StateProvince = CommonHelper.EnsureNotNull(affiliate.StateProvince);
            affiliate.StateProvince = CommonHelper.EnsureMaximumLength(affiliate.StateProvince, 100);
            affiliate.ZipPostalCode = CommonHelper.EnsureNotNull(affiliate.ZipPostalCode);
            affiliate.ZipPostalCode = CommonHelper.EnsureMaximumLength(affiliate.ZipPostalCode, 30);

            

            _context.Affiliates.AddObject(affiliate);
            _context.SaveChanges();
        }

        /// <summary>
        /// Updates the affiliate
        /// </summary>
        /// <param name="affiliate">Affiliate</param>
        public void UpdateAffiliate(Affiliate affiliate)
        {
            if (affiliate == null)
                throw new ArgumentNullException("affiliate");

            affiliate.FirstName = CommonHelper.EnsureNotNull(affiliate.FirstName);
            affiliate.LastName = CommonHelper.EnsureNotNull(affiliate.LastName);
            affiliate.FirstName = CommonHelper.EnsureMaximumLength(affiliate.FirstName, 100);
            affiliate.LastName = CommonHelper.EnsureMaximumLength(affiliate.LastName, 100);
            affiliate.MiddleName = CommonHelper.EnsureNotNull(affiliate.MiddleName);
            affiliate.MiddleName = CommonHelper.EnsureMaximumLength(affiliate.MiddleName, 100);
            affiliate.PhoneNumber = CommonHelper.EnsureNotNull(affiliate.PhoneNumber);
            affiliate.PhoneNumber = CommonHelper.EnsureMaximumLength(affiliate.PhoneNumber, 50);
            affiliate.Email = CommonHelper.EnsureNotNull(affiliate.Email);
            affiliate.Email = CommonHelper.EnsureMaximumLength(affiliate.Email, 255);
            affiliate.FaxNumber = CommonHelper.EnsureNotNull(affiliate.FaxNumber);
            affiliate.FaxNumber = CommonHelper.EnsureMaximumLength(affiliate.FaxNumber, 50);
            affiliate.Company = CommonHelper.EnsureNotNull(affiliate.Company);
            affiliate.Company = CommonHelper.EnsureMaximumLength(affiliate.Company, 100);
            affiliate.Address1 = CommonHelper.EnsureNotNull(affiliate.Address1);
            affiliate.Address1 = CommonHelper.EnsureMaximumLength(affiliate.Address1, 100);
            affiliate.Address2 = CommonHelper.EnsureNotNull(affiliate.Address2);
            affiliate.Address2 = CommonHelper.EnsureMaximumLength(affiliate.Address2, 100);
            affiliate.City = CommonHelper.EnsureNotNull(affiliate.City);
            affiliate.City = CommonHelper.EnsureMaximumLength(affiliate.City, 100);
            affiliate.StateProvince = CommonHelper.EnsureNotNull(affiliate.StateProvince);
            affiliate.StateProvince = CommonHelper.EnsureMaximumLength(affiliate.StateProvince, 100);
            affiliate.ZipPostalCode = CommonHelper.EnsureNotNull(affiliate.ZipPostalCode);
            affiliate.ZipPostalCode = CommonHelper.EnsureMaximumLength(affiliate.ZipPostalCode, 30);

            
            if (!_context.IsAttached(affiliate))
                _context.Affiliates.Attach(affiliate);

            _context.SaveChanges();
        }
        #endregion
    }
}
