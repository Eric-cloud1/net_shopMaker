using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.Stores
{
    /// <summary>
    /// Base class for forex provider implementations
    /// </summary>
    public abstract class ForexProviderBase : IForexProvider
    {
        #region IForexProvider Members

        /// <summary>
        /// Class Id of the implementation
        /// </summary>
        public string ClassId
        {
            get { return Utility.Misc.GetClassId(this.GetType()); }
        }

        /// <summary>
        /// Name of the implementation
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Version of the implementation
        /// </summary>
        public string Version
        {
            get { return this.GetType().Assembly.GetName().Version.ToString(); }
        }

        /// <summary>
        /// Gets the exchange rate when converting from source currency to target currency
        /// </summary>
        /// <param name="sourceCurrency">The source currency to convert from</param>
        /// <param name="targetCurrency">The target currency to convert to</param>
        /// <returns>The exchange rate for conversion</returns>
        public abstract MakerShop.Common.LSDecimal GetExchangeRate(string sourceCurrency, string targetCurrency);

        #endregion
    }
}
