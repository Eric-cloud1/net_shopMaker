//-----------------------------------------------------------------------
// <copyright file="TaxReportSummaryItem.cs" company="Able Solutions Corporation">
//     Copyright (c) 2009 Able Solutions Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace MakerShop.Reporting
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using MakerShop.Common;
    using MakerShop.Taxes;

    /// <summary>
    /// Contains summary data for a tax report
    /// </summary>
    public class TaxReportSummaryItem
    {
        private int _TaxRuleId;
        private string _TaxName;
        private LSDecimal _TaxAmount;

        /// <summary>
        /// Gets or sets the tax rule id for this summary record
        /// </summary>
        public int TaxRuleId
        {
            get { return this._TaxRuleId; }
            set { this._TaxRuleId = value; }
        }

        /// <summary>
        /// Gets the associated tax rule for this summary record
        /// </summary>
        public TaxRule TaxRule
        {
            get { return TaxRuleDataSource.Load(this._TaxRuleId); }
        }

        /// <summary>
        /// Gets or sets the tax name for this summary record
        /// </summary>
        public string TaxName
        {
            get { return this._TaxName; }
            set { this._TaxName = value; }
        }

        /// <summary>
        /// Gets or sets the tax amount for this summary record
        /// </summary>
        public LSDecimal TaxAmount
        {
            get { return this._TaxAmount; }
            set { this._TaxAmount = value; }
        }
    }
}
