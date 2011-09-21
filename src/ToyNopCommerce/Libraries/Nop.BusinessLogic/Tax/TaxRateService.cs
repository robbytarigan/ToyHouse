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
using NopSolutions.NopCommerce.BusinessLogic.Configuration.Settings;
using NopSolutions.NopCommerce.BusinessLogic.Data;
using NopSolutions.NopCommerce.BusinessLogic.Infrastructure;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.BusinessLogic.Tax
{
    /// <summary>
    /// Tax rate service
    /// </summary>
    public partial class TaxRateService : ITaxRateService
    {
        #region Constants
        private const string TAXRATE_ALL_KEY = "Nop.taxrate.all";
        private const string TAXRATE_BY_ID_KEY = "Nop.taxrate.id-{0}";
        private const string TAXRATE_PATTERN_KEY = "Nop.taxrate.";
        #endregion

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
        public TaxRateService(NopObjectContext context)
        {
            this._context = context;
            this._cacheManager = new NopRequestCache();
        }

        #endregion

        #region Methods
        /// <summary>
        /// Deletes a tax rate
        /// </summary>
        /// <param name="taxRateId">Tax rate identifier</param>
        public void DeleteTaxRate(int taxRateId)
        {
            var taxRate = GetTaxRateById(taxRateId);
            if (taxRate == null)
                return;

            
            if (!_context.IsAttached(taxRate))
                _context.TaxRates.Attach(taxRate);
            _context.DeleteObject(taxRate);
            _context.SaveChanges();
            
            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(TAXRATE_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets a tax rate
        /// </summary>
        /// <param name="taxRateId">Tax rate identifier</param>
        /// <returns>Tax rate</returns>
        public TaxRate GetTaxRateById(int taxRateId)
        {
            if (taxRateId == 0)
                return null;

            string key = string.Format(TAXRATE_BY_ID_KEY, taxRateId);
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (TaxRate)obj2;
            }

            
            var query = from tr in _context.TaxRates
                        where tr.TaxRateId == taxRateId
                        select tr;
            var taxRate = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, taxRate);
            }
            return taxRate;
        }

        /// <summary>
        /// Gets all tax rates
        /// </summary>
        /// <returns>Tax rate collection</returns>
        public List<TaxRate> GetAllTaxRates()
        {
            string key = TAXRATE_ALL_KEY;
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<TaxRate>)obj2;
            }

            //var query = from tr in _context.TaxRates
            //            from c in _context.Countries
            //            .Where(c => c.CountryId == tr.CountryId).DefaultIfEmpty()
            //            from sp in _context.StateProvinces
            //            .Where(sp => sp.StateProvinceId == tr.StateProvinceId).DefaultIfEmpty()
            //            orderby c.DisplayOrder, c.Name, sp.DisplayOrder, sp.Name, sp.StateProvinceId, tr.Zip, tr.TaxCategoryId
            //            select tr;

            var query = from tr in _context.TaxRates
                        join c1 in _context.Countries on tr.CountryId equals c1.CountryId into c2
                        from c in c2.DefaultIfEmpty()
                        join sp1 in _context.StateProvinces on tr.StateProvinceId equals sp1.StateProvinceId into sp2
                        from sp in sp2.DefaultIfEmpty()
                        orderby c.DisplayOrder, c.Name, sp.DisplayOrder, sp.Name, sp.StateProvinceId, tr.Zip, tr.TaxCategoryId
                        select tr;

            var taxRates = query.ToList();

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, taxRates);
            }

            return taxRates;
        }

        /// <summary>
        /// Gets all tax rates by params
        /// </summary>
        /// <param name="taxCategoryId">The tax category identifier</param>
        /// <param name="countryId">The country identifier</param>
        /// <param name="stateProvinceId">The state/province identifier</param>
        /// <param name="zip">The zip</param>
        /// <returns>Tax rate collection</returns>
        public List<TaxRate> GetAllTaxRates(int taxCategoryId, int countryId,
            int stateProvinceId, string zip)
        {
            if (zip == null)
                zip = string.Empty;
            if (!String.IsNullOrEmpty(zip))
                zip = zip.Trim();

            var existingRates = GetAllTaxRates().FindTaxRates(countryId, taxCategoryId);

            //filter by state/province
            var matchedByStateProvince = new List<TaxRate>();
            foreach (var taxRate in existingRates)
            {
                if (stateProvinceId == taxRate.StateProvinceId)
                    matchedByStateProvince.Add(taxRate);
            }
            if (matchedByStateProvince.Count == 0)
            {
                foreach (var taxRate in existingRates)
                {
                    if (taxRate.StateProvinceId == 0)
                        matchedByStateProvince.Add(taxRate);
                }
            }

            //filter by zip
            var matchedByZip = new List<TaxRate>();
            foreach (var taxRate in matchedByStateProvince)
            {
                if (zip.ToLower() == taxRate.Zip.ToLower())
                    matchedByZip.Add(taxRate);
            }
            if (matchedByZip.Count == 0)
            {
                foreach (var taxRate in matchedByStateProvince)
                {
                    if (taxRate.Zip.Trim() == string.Empty)
                        matchedByZip.Add(taxRate);
                }
            }

            return matchedByZip;
        }

        /// <summary>
        /// Inserts a tax rate
        /// </summary>
        /// <param name="taxRate">Tax rate</param>
        public void InsertTaxRate(TaxRate taxRate)
        {
            if (taxRate == null)
                throw new ArgumentNullException("taxRate");

            taxRate.Zip = CommonHelper.EnsureNotNull(taxRate.Zip);
            taxRate.Zip = taxRate.Zip.Trim();
            taxRate.Zip = CommonHelper.EnsureMaximumLength(taxRate.Zip, 50);

            
            
            _context.TaxRates.AddObject(taxRate);
            _context.SaveChanges();
            
            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(TAXRATE_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the tax rate
        /// </summary>
        /// <param name="taxRate">Tax rate</param>
        public void UpdateTaxRate(TaxRate taxRate)
        {
            if (taxRate == null)
                throw new ArgumentNullException("taxRate");

            taxRate.Zip = CommonHelper.EnsureNotNull(taxRate.Zip);
            taxRate.Zip = taxRate.Zip.Trim();
            taxRate.Zip = CommonHelper.EnsureMaximumLength(taxRate.Zip, 50);

            
            if (!_context.IsAttached(taxRate))
                _context.TaxRates.Attach(taxRate);

            _context.SaveChanges();
            
            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(TAXRATE_PATTERN_KEY);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public bool CacheEnabled
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Cache.TaxRateManager.CacheEnabled");
            }
        }
        #endregion
    }
}
