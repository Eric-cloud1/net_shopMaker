using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.Utility
{
    /// <summary>
    /// Utility class for wrapping caching date and cached value
    /// </summary>
    public class CacheWrapper
    {
        private DateTime _CacheDate;
        private object _CacheValue;

        /// <summary>
        /// The cache date
        /// </summary>
        public DateTime CacheDate
        {
            get { return _CacheDate; }
            set { _CacheDate = value; }
        }

        /// <summary>
        /// The cache value
        /// </summary>
        public object CacheValue
        {
            get { return _CacheValue; }
            set { _CacheValue = value; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public CacheWrapper() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="cacheValue">Cache value</param>
        public CacheWrapper(object cacheValue)
        {
            _CacheDate = LocaleHelper.LocalNow;
            _CacheValue = cacheValue;
        }
    }
}
