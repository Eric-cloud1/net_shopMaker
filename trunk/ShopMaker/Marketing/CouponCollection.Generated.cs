namespace MakerShop.Marketing
{
    using System;
    using MakerShop.Common;
    /// <summary>
    /// This class implements a PersistentCollection of Coupon objects.
    /// </summary>
    public partial class CouponCollection : PersistentCollection<Coupon>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="couponId">Value of CouponId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 couponId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (couponId == this[i].CouponId) return i;
            }
            return -1;
        }
    }
}
