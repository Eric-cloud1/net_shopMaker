using System.ComponentModel;
using MakerShop.Common;
namespace MakerShop.Marketing
{
    /// <summary>
    /// This class represents a Coupon object in the database.
    /// </summary>
    public partial class Coupon
    {

        //private CouponProductCollection _NoGcCouponProducts;
        //[EditorBrowsable(EditorBrowsableState.Advanced)]
        //internal bool CouponNoGcProductsLoaded { get { return (this._NoGcCouponProducts != null); } }

        /// <summary>
        /// The type of this coupon
        /// </summary>
        public CouponType CouponType
        {
            get
            {
                return (CouponType)this.CouponTypeId;
            }
            set
            {
                this.CouponTypeId = (byte)value;
            }
        }

        /// <summary>
        /// Product rule for this coupon
        /// </summary>
        public CouponRule ProductRule
        {
            get
            {
                return (CouponRule)this.ProductRuleId;
            }
            set
            {
                this.ProductRuleId = (byte)value;
            }
        }

        /// <summary>
        /// Creates a clone of this Coupon object
        /// </summary>
        /// <param name="deepClone">If <b>true</b> all the child elements are also copied. (TODO)</param>
        /// <returns>A clone of this coupon object</returns>
        public Coupon Clone(bool deepClone)
        {
            Coupon cloneItem = new Coupon();
            cloneItem.CouponTypeId = this.CouponTypeId;
            cloneItem.Name = this.Name;
            cloneItem.CouponCode = this.CouponCode;
            cloneItem.DiscountAmount = this.DiscountAmount;
            cloneItem.IsPercent = this.IsPercent;
            cloneItem.MaxValue = this.MaxValue;
            cloneItem.MinPurchase = this.MinPurchase;
            cloneItem.MinQuantity = this.MinQuantity;
            cloneItem.MaxQuantity = this.MaxQuantity;
            cloneItem.QuantityInterval = this.QuantityInterval;
            cloneItem.MaxUses = this.MaxUses;
            cloneItem.MaxUsesPerCustomer = this.MaxUsesPerCustomer;
            cloneItem.StartDate = this.StartDate;
            cloneItem.EndDate = this.EndDate;
            cloneItem.ProductRule = this.ProductRule;
            cloneItem.AllowCombine = this.AllowCombine;
            if (deepClone)
            {   
                // COPY CHILD OBJECTS
                // WE HAVE TO SAVE THE COUPON
                if(cloneItem.Save() != SaveResult.Failed)
                {                
                    // COPY GROUPS
                    foreach (CouponGroup grop in this.CouponGroups)
                    {
                        cloneItem.CouponGroups.Add(new CouponGroup(cloneItem.CouponId, grop.GroupId));
                    }
                    cloneItem.CouponGroups.Save();

                    // COPY PRODUCTS
                    foreach (CouponProduct product in this.CouponProducts)
                    {
                        cloneItem.CouponProducts.Add(new CouponProduct(cloneItem.CouponId,product.ProductId));
                    }
                    cloneItem.CouponProducts.Save();

                    // COPY SHIP METHODS
                    foreach (CouponShipMethod couponShipMethod in this.CouponShipMethods)
                    {
                        cloneItem.CouponShipMethods.Add(new CouponShipMethod(cloneItem.CouponId, couponShipMethod.ShipMethodId));
                    }
                    cloneItem.CouponShipMethods.Save();
                }
            }
            return cloneItem;
        }

        /*public CouponProductCollection GetCouponProducts(bool ignoreGiftCertificates)
        {
            if (ignoreGiftCertificates)
            {
                if (!this.CouponNoGcProductsLoaded)
                {
                    this._NoGcCouponProducts = CouponProductDataSource.LoadForCoupon(this.CouponId,true);
                }
                return this._NoGcCouponProducts;                
            }
            else
            {
                return this.CouponProducts;
            }
        }*/
    }
}
