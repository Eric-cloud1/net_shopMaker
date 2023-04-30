using System;
using System.Collections.Generic;
using System.Text;
using MakerShop.Marketing;

namespace MakerShop.Reporting
{
    /// <summary>
    /// Class that holds summary data about a coupon usage
    /// </summary>
    public class CouponSummary
    {
        private string _CouponCode;
        private DateTime _StartDate;
        private DateTime _EndDate;
        private int _OrderCount;
        private Decimal _OrderTotal;
        private Coupon _Coupon;

        /// <summary>
        /// The coupon code
        /// </summary>
        public string CouponCode
        {
            get { return _CouponCode; }
            set { _CouponCode = value; }
        }

        /// <summary>
        /// Start date of this summary data
        /// </summary>
        public DateTime StartDate
        {
            get { return _StartDate; }
            set { _StartDate = value; }
        }

        /// <summary>
        /// End date of this summary data
        /// </summary>
        public DateTime EndDate
        {
            get { return _EndDate; }
            set { _EndDate = value; }
        }

        /// <summary>
        /// Number of orders
        /// </summary>
        public int OrderCount
        {
            get { return _OrderCount; }
            set { _OrderCount = value; }
        }

        /// <summary>
        /// Order total
        /// </summary>
        public Decimal OrderTotal
        {
            get { return _OrderTotal; }
            set { _OrderTotal = value; }
        }
        
        /// <summary>
        /// The coupon object
        /// </summary>
        public Coupon Coupon
        {
            get
            {
                if (this._Coupon == null)
                {
                    this._Coupon = CouponDataSource.LoadForCouponCode(this.CouponCode);
                }
                return this._Coupon;
            }
        }
    }
}
