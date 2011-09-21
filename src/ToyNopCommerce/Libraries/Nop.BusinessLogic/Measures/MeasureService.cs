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
using NopSolutions.NopCommerce.Common;
using NopSolutions.NopCommerce.Common.Utils;

namespace NopSolutions.NopCommerce.BusinessLogic.Measures
{
    /// <summary>
    /// Measure dimension service
    /// </summary>
    public partial class MeasureService : IMeasureService
    {
        #region Constants
        private const string MEASUREDIMENSIONS_ALL_KEY = "Nop.measuredimension.all";
        private const string MEASUREDIMENSIONS_BY_ID_KEY = "Nop.measuredimension.id-{0}";
        private const string MEASUREWEIGHTS_ALL_KEY = "Nop.measureweight.all";
        private const string MEASUREWEIGHTS_BY_ID_KEY = "Nop.measureweight.id-{0}";
        private const string MEASUREDIMENSIONS_PATTERN_KEY = "Nop.measuredimension.";
        private const string MEASUREWEIGHTS_PATTERN_KEY = "Nop.measureweight.";
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
        public MeasureService(NopObjectContext context)
        {
            this._context = context;
            this._cacheManager = new NopRequestCache();
        }

        #endregion

        #region Methods

        #region Dimensions
        /// <summary>
        /// Deletes measure dimension
        /// </summary>
        /// <param name="measureDimensionId">Measure dimension identifier</param>
        public void DeleteMeasureDimension(int measureDimensionId)
        {
            var measureDimension = GetMeasureDimensionById(measureDimensionId);
            if (measureDimension == null)
                return;

            
            if (!_context.IsAttached(measureDimension))
                _context.MeasureDimensions.Attach(measureDimension);
            _context.DeleteObject(measureDimension);
            _context.SaveChanges();
            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(MEASUREDIMENSIONS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets a measure dimension by identifier
        /// </summary>
        /// <param name="measureDimensionId">Measure dimension identifier</param>
        /// <returns>Measure dimension</returns>
        public MeasureDimension GetMeasureDimensionById(int measureDimensionId)
        {
            if (measureDimensionId == 0)
                return null;

            string key = string.Format(MEASUREDIMENSIONS_BY_ID_KEY, measureDimensionId);
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (MeasureDimension)obj2;
            }

            
            var query = from md in _context.MeasureDimensions
                        where md.MeasureDimensionId == measureDimensionId
                        select md;
            var measureDimension = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, measureDimension);
            }
            return measureDimension;
        }

        /// <summary>
        /// Gets a measure dimension by system keyword
        /// </summary>
        /// <param name="systemKeyword">The system keyword</param>
        /// <returns>Measure dimension</returns>
        public MeasureDimension GetMeasureDimensionBySystemKeyword(string systemKeyword)
        {
            if (String.IsNullOrEmpty(systemKeyword))
                return null;

            var measureDimensions = GetAllMeasureDimensions();
            foreach (var measureDimension in measureDimensions)
                if (measureDimension.SystemKeyword.ToLowerInvariant() == systemKeyword.ToLowerInvariant())
                    return measureDimension;
            return null;
        }

        /// <summary>
        /// Gets all measure dimensions
        /// </summary>
        /// <returns>Measure dimension collection</returns>
        public List<MeasureDimension> GetAllMeasureDimensions()
        {
            string key = MEASUREDIMENSIONS_ALL_KEY;
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<MeasureDimension>)obj2;
            }

            
            var query = from md in _context.MeasureDimensions
                        orderby md.DisplayOrder
                        select md;
            var measureDimensionCollection = query.ToList();

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, measureDimensionCollection);
            }
            return measureDimensionCollection;
        }

        /// <summary>
        /// Inserts a measure dimension
        /// </summary>
        /// <param name="measure">Measure dimension</param>
        public void InsertMeasureDimension(MeasureDimension measure)
        {
            if (measure == null)
                throw new ArgumentNullException("measure");

            measure.Name = CommonHelper.EnsureNotNull(measure.Name);
            measure.Name = CommonHelper.EnsureMaximumLength(measure.Name, 100);
            measure.SystemKeyword = CommonHelper.EnsureNotNull(measure.SystemKeyword);
            measure.SystemKeyword = CommonHelper.EnsureMaximumLength(measure.SystemKeyword, 100);

            
            
            _context.MeasureDimensions.AddObject(measure);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(MEASUREDIMENSIONS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the measure dimension
        /// </summary>
        /// <param name="measure">Measure dimension</param>
        public void UpdateMeasureDimension(MeasureDimension measure)
        {
            if (measure == null)
                throw new ArgumentNullException("measure");

            measure.Name = CommonHelper.EnsureNotNull(measure.Name);
            measure.Name = CommonHelper.EnsureMaximumLength(measure.Name, 100);
            measure.SystemKeyword = CommonHelper.EnsureNotNull(measure.SystemKeyword);
            measure.SystemKeyword = CommonHelper.EnsureMaximumLength(measure.SystemKeyword, 100);

            
            if (!_context.IsAttached(measure))
                _context.MeasureDimensions.Attach(measure);

            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(MEASUREDIMENSIONS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Converts dimension
        /// </summary>
        /// <param name="quantity">Quantity</param>
        /// <param name="sourceMeasureDimension">Source dimension</param>
        /// <param name="targetMeasureDimension">Target dimension</param>
        /// <returns>Converted value</returns>
        public decimal ConvertDimension(decimal quantity,
            MeasureDimension sourceMeasureDimension, MeasureDimension targetMeasureDimension)
        {
            decimal result = quantity;
            if (sourceMeasureDimension.MeasureDimensionId == targetMeasureDimension.MeasureDimensionId)
                return result;
            if (result != decimal.Zero && sourceMeasureDimension.MeasureDimensionId != targetMeasureDimension.MeasureDimensionId)
            {
                result = ConvertToPrimaryMeasureDimension(result, sourceMeasureDimension);
                result = ConvertFromPrimaryMeasureDimension(result, targetMeasureDimension);
            }
            result = Math.Round(result, 2);
            return result;
        }

        /// <summary>
        /// Converts to primary measure dimension
        /// </summary>
        /// <param name="quantity">Quantity</param>
        /// <param name="sourceMeasureDimension">Source dimension</param>
        /// <returns>Converted value</returns>
        public decimal ConvertToPrimaryMeasureDimension(decimal quantity,
            MeasureDimension sourceMeasureDimension)
        {
            decimal result = quantity;
            if (result != decimal.Zero && sourceMeasureDimension.MeasureDimensionId != BaseDimensionIn.MeasureDimensionId)
            {
                decimal exchangeRatio = sourceMeasureDimension.Ratio;
                if (exchangeRatio == decimal.Zero)
                    throw new NopException(string.Format("Exchange ratio not set for dimension [{0}]", sourceMeasureDimension.Name));
                result = result / exchangeRatio;
            }
            return result;
        }

        /// <summary>
        /// Converts from primary dimension
        /// </summary>
        /// <param name="quantity">Quantity</param>
        /// <param name="targetMeasureDimension">Target dimension</param>
        /// <returns>Converted value</returns>
        public decimal ConvertFromPrimaryMeasureDimension(decimal quantity,
            MeasureDimension targetMeasureDimension)
        {
            decimal result = quantity;
            if (result != decimal.Zero && targetMeasureDimension.MeasureDimensionId != BaseDimensionIn.MeasureDimensionId)
            {
                decimal exchangeRatio = targetMeasureDimension.Ratio;
                if (exchangeRatio == decimal.Zero)
                    throw new NopException(string.Format("Exchange ratio not set for dimension [{0}]", targetMeasureDimension.Name));
                result = result * exchangeRatio;
            }
            return result;
        }

        #endregion

        #region Weights

        /// <summary>
        /// Deletes measure weight
        /// </summary>
        /// <param name="measureWeightId">Measure weight identifier</param>
        public void DeleteMeasureWeight(int measureWeightId)
        {
            var measureWeight = GetMeasureWeightById(measureWeightId);
            if (measureWeight == null)
                return;

            
            if (!_context.IsAttached(measureWeight))
                _context.MeasureWeights.Attach(measureWeight);
            _context.DeleteObject(measureWeight);
            _context.SaveChanges();
            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(MEASUREWEIGHTS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Gets a measure weight by identifier
        /// </summary>
        /// <param name="measureWeightId">Measure weight identifier</param>
        /// <returns>Measure weight</returns>
        public MeasureWeight GetMeasureWeightById(int measureWeightId)
        {
            if (measureWeightId == 0)
                return null;

            string key = string.Format(MEASUREWEIGHTS_BY_ID_KEY, measureWeightId);
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (MeasureWeight)obj2;
            }

            
            var query = from mw in _context.MeasureWeights
                        where mw.MeasureWeightId == measureWeightId
                        select mw;
            var measureWeight = query.SingleOrDefault();

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, measureWeight);
            }
            return measureWeight;
        }

        /// <summary>
        /// Gets a measure weight by system keyword
        /// </summary>
        /// <param name="systemKeyword">The system keyword</param>
        /// <returns>Measure weight</returns>
        public MeasureWeight GetMeasureWeightBySystemKeyword(string systemKeyword)
        {
            if (String.IsNullOrEmpty(systemKeyword))
                return null;

            var measureWeights = GetAllMeasureWeights();
            foreach (var measureWeight in measureWeights)
                if (measureWeight.SystemKeyword.ToLowerInvariant() == systemKeyword.ToLowerInvariant())
                    return measureWeight;
            return null;
        }

        /// <summary>
        /// Gets all measure weights
        /// </summary>
        /// <returns>Measure weight collection</returns>
        public List<MeasureWeight> GetAllMeasureWeights()
        {
            string key = MEASUREWEIGHTS_ALL_KEY;
            object obj2 = _cacheManager.Get(key);
            if (this.CacheEnabled && (obj2 != null))
            {
                return (List<MeasureWeight>)obj2;
            }

            
            var query = from mw in _context.MeasureWeights
                        orderby mw.DisplayOrder
                        select mw;
            var measureWeightCollection = query.ToList();

            if (this.CacheEnabled)
            {
                _cacheManager.Add(key, measureWeightCollection);
            }
            return measureWeightCollection;
        }

        /// <summary>
        /// Inserts a measure weight
        /// </summary>
        /// <param name="measure">Measure weight</param>
        public void InsertMeasureWeight(MeasureWeight measure)
        {
            if (measure == null)
                throw new ArgumentNullException("measure");

            measure.Name = CommonHelper.EnsureNotNull(measure.Name);
            measure.Name = CommonHelper.EnsureMaximumLength(measure.Name, 100);
            measure.SystemKeyword = CommonHelper.EnsureNotNull(measure.SystemKeyword);
            measure.SystemKeyword = CommonHelper.EnsureMaximumLength(measure.SystemKeyword, 100);

            

            _context.MeasureWeights.AddObject(measure);
            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(MEASUREWEIGHTS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Updates the measure weight
        /// </summary>
        /// <param name="measure">Measure weight</param>
        public void UpdateMeasureWeight(MeasureWeight measure)
        {
            if (measure == null)
                throw new ArgumentNullException("measure");

            measure.Name = CommonHelper.EnsureNotNull(measure.Name);
            measure.Name = CommonHelper.EnsureMaximumLength(measure.Name, 100);
            measure.SystemKeyword = CommonHelper.EnsureNotNull(measure.SystemKeyword);
            measure.SystemKeyword = CommonHelper.EnsureMaximumLength(measure.SystemKeyword, 100);

            
            if (!_context.IsAttached(measure))
                _context.MeasureWeights.Attach(measure);

            _context.SaveChanges();

            if (this.CacheEnabled)
            {
                _cacheManager.RemoveByPattern(MEASUREWEIGHTS_PATTERN_KEY);
            }
        }

        /// <summary>
        /// Converts weight
        /// </summary>
        /// <param name="quantity">Quantity</param>
        /// <param name="sourceMeasureWeight">Source weight</param>
        /// <param name="targetMeasureWeight">Target weight</param>
        /// <returns>Converted value</returns>
        public decimal ConvertWeight(decimal quantity,
            MeasureWeight sourceMeasureWeight, MeasureWeight targetMeasureWeight)
        {
            decimal result = quantity;
            if (sourceMeasureWeight.MeasureWeightId == targetMeasureWeight.MeasureWeightId)
                return result;
            if (result != decimal.Zero && sourceMeasureWeight.MeasureWeightId != targetMeasureWeight.MeasureWeightId)
            {
                result = ConvertToPrimaryMeasureWeight(result, sourceMeasureWeight);
                result = ConvertFromPrimaryMeasureWeight(result, targetMeasureWeight);
            }
            result = Math.Round(result, 2);
            return result;
        }

        /// <summary>
        /// Converts to primary measure weight
        /// </summary>
        /// <param name="quantity">Quantity</param>
        /// <param name="sourceMeasureWeight">Source weight</param>
        /// <returns>Converted value</returns>
        public decimal ConvertToPrimaryMeasureWeight(decimal quantity, MeasureWeight sourceMeasureWeight)
        {
            decimal result = quantity;
            if (result != decimal.Zero && sourceMeasureWeight.MeasureWeightId != BaseWeightIn.MeasureWeightId)
            {
                decimal exchangeRatio = sourceMeasureWeight.Ratio;
                if (exchangeRatio == decimal.Zero)
                    throw new NopException(string.Format("Exchange ratio not set for weight [{0}]", sourceMeasureWeight.Name));
                result = result / exchangeRatio;
            }
            return result;
        }

        /// <summary>
        /// Converts from primary weight
        /// </summary>
        /// <param name="quantity">Quantity</param>
        /// <param name="targetMeasureWeight">Target weight</param>
        /// <returns>Converted value</returns>
        public decimal ConvertFromPrimaryMeasureWeight(decimal quantity,
            MeasureWeight targetMeasureWeight)
        {
            decimal result = quantity;
            if (result != decimal.Zero && targetMeasureWeight.MeasureWeightId != BaseWeightIn.MeasureWeightId)
            {
                decimal exchangeRatio = targetMeasureWeight.Ratio;
                if (exchangeRatio == decimal.Zero)
                    throw new NopException(string.Format("Exchange ratio not set for weight [{0}]", targetMeasureWeight.Name));
                result = result * exchangeRatio;
            }
            return result;
        }

        #endregion

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the dimension that will be used as a default
        /// </summary>
        public MeasureDimension BaseDimensionIn
        {
            get
            {
                int baseDimensionIn = IoC.Resolve<ISettingManager>().GetSettingValueInteger("Common.BaseDimensionIn");
                return this.GetMeasureDimensionById(baseDimensionIn);
            }
            set
            {
                if (value != null)
                    IoC.Resolve<ISettingManager>().SetParam("Common.BaseDimensionIn", value.MeasureDimensionId.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the weight that will be used as a default
        /// </summary>
        public MeasureWeight BaseWeightIn
        {
            get
            {
                int baseWeightIn = IoC.Resolve<ISettingManager>().GetSettingValueInteger("Common.BaseWeightIn");
                return this.GetMeasureWeightById(baseWeightIn);
            }
            set
            {
                if (value != null)
                    IoC.Resolve<ISettingManager>().SetParam("Common.BaseWeightIn", value.MeasureWeightId.ToString());
            }
        }

        /// <summary>
        /// Gets a value indicating whether cache is enabled
        /// </summary>
        public bool CacheEnabled
        {
            get
            {
                return IoC.Resolve<ISettingManager>().GetSettingValueBoolean("Cache.MeasureManager.CacheEnabled");
            }
        }
        #endregion
    }
}