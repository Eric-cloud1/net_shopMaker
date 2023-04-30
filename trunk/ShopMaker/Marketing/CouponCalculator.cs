using System;
using System.Collections.Generic;
using System.Text;
using MakerShop.Common;
using MakerShop.Orders;
using MakerShop.Users;
using MakerShop.Utility;

namespace MakerShop.Marketing
{
    /// <summary>
    /// Utility class for supporting coupon calculations
    /// </summary>
    public static class CouponCalculator
    {
        /// <summary>
        /// Checks whether the given user is in any of the coupon groups specified
        /// </summary>
        /// <param name="user">The user to check for</param>
        /// <param name="couponGroups">The coupon groups to look into</param>
        /// <returns><b>true</b> if user is member of any of the specified coupon groups, <b>false</b> otherwise</returns>
        private static bool IsUserInGroup(User user, CouponGroupCollection couponGroups)
        {
            if (couponGroups.Count == 0) return true;
            foreach (CouponGroup couponGroup in couponGroups)
            {
                if (user.IsInGroup(couponGroup.GroupId)) return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if the given coupon is already used in a basket
        /// </summary>
        /// <param name="basket">The basket to check</param>
        /// <param name="coupon">The coupon to check</param>
        /// <returns><b>true</b> if coupon is already used in basket, <b>false</b> otherwise</returns>
        public static bool IsCouponAlreadyUsed(Basket basket, Coupon coupon)
        {
            //MAKE SURE COUPON IS NOT ALREADY USED
            foreach (BasketCoupon bc in basket.BasketCoupons)
            {
                if (bc.CouponId.Equals(coupon.CouponId)) return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if the given coupon is valid for the specified basket
        /// </summary>
        /// <param name="basket">The basket to check</param>
        /// <param name="coupon">The coupon to check</param>
        /// <returns><b>true</b> if coupon is valid for the specified basket, <b>false</b> otherwise</returns>
        public static bool IsCouponValid(Basket basket, Coupon coupon)
        {
            String temp = "";
            return IsCouponValid(basket, coupon,out temp);
        }

        /// <summary>
        /// Checks if the given coupon is valid for the specified basket
        /// </summary>
        /// <param name="basket">The basket to check</param>
        /// <param name="coupon">The coupon to check</param>
        /// <param name="message">Warning or error message is returned in this variable if any</param>
        /// <returns><b>true</b> if coupon is valid for the specified basket, <b>false</b> otherwise</returns>
        public static bool IsCouponValid(Basket basket, Coupon coupon, out String message)
        {
            //CHECK IF STARTDATE HAS NOT REACHED
            if (coupon.StartDate > LocaleHelper.LocalNow)
            {
                message = "The coupon code you've entered is not yet activated.";
                return false;
            }
            //CHECK IF ENDDATE HAS PASSED
            if ((coupon.EndDate > System.DateTime.MinValue) && (coupon.EndDate < LocaleHelper.LocalNow))
            {
                message = "The coupon code you've entered has expired.";
                return false;
            }
            //VALIDATE ROLE RESTRICTION
            if (!CouponCalculator.IsUserInGroup(basket.User, coupon.CouponGroups))
            {
                message = "The coupon code you've entered is not valid.";
                return false;
            }
            //VALIDATE USAGE RESTRICTIONS
            if (coupon.MaxUses > 0)
            {
                int useCount = CouponDataSource.CountUses(coupon.CouponCode);
                if (useCount >= coupon.MaxUses)
                {
                    message = "You are no longer allowed to use this coupon.";
                    return false;
                }
            }
            if (coupon.MaxUsesPerCustomer > 0)
            {
                int useCount = CouponDataSource.CountUsesForUser(coupon.CouponCode, basket.UserId);
                if (useCount >= coupon.MaxUsesPerCustomer)
                {
                    message = "You are no longer allowed to use this coupon.";                    
                    return false;
                }
            }
            OrderItemType[] productTypes = { OrderItemType.Product, OrderItemType.Discount, OrderItemType.Coupon };
            if (coupon.CouponType != CouponType.Shipping)
            {
                //GET TOTAL VALUE OF ITEMS TO BE DISCOUNTED BY COUPON
                List<int> productIds = new List<int>();
                CouponProductCollection cpc = coupon.CouponProducts;
                foreach (CouponProduct cp in cpc)
                {
                    productIds.Add(cp.ProductId);
                }
                LSDecimal valueOfItemsPurchased = CouponCalculator.GetItemTotal(basket.Items, productTypes, coupon.ProductRule, productIds.ToArray(), coupon.MinQuantity);
                //IF THERE IS NO VALUE, NOTHING CAN BE DISCOUNTED BY COUPON
                if (valueOfItemsPurchased == 0)
                {
                    message = "The coupon code you've entered does not apply to any of the products in your basket.";
                    return false;
                }
                else if (valueOfItemsPurchased < 0) //VALIDATE MIN QUANTITY
                {
                    message = "The coupon code you've entered requires a minimum item quantity of {0}.";
                    message = String.Format(message, coupon.MinQuantity);
                    return false;
                }
                //VALIDATE MIN PURCHASE
                if ((coupon.MinPurchase > 0) && (valueOfItemsPurchased < coupon.MinPurchase))
                {
                    message = "The coupon code you've entered requires a minimum purchase of {0:ulc}.";
                    message = String.Format(message, coupon.MinPurchase);
                    return false;
                }
            }
            else
            {
                bool foundValidShipment = false;
                bool minPurchaseRequired = false;
                foreach (BasketShipment shipment in basket.Shipments)
                {
                    BasketItemCollection shipmentItems = shipment.GetItems(basket);
                    //THIS COUPON MEETS THE MINIMUM PURCHASE REQUIMENTS FOR THIS SHIPMENT
                    //GET THE TOTAL OF SHIPPING CHARGES
                    List<int> shipMethodIds = new List<int>();
                    foreach (CouponShipMethod csm in coupon.CouponShipMethods)
                    {
                        shipMethodIds.Add(csm.ShipMethodId);
                    }
                    LSDecimal shippingTotal = CouponCalculator.GetShippingTotal(shipmentItems, coupon.ProductRule, shipMethodIds.ToArray());
                    if (shippingTotal > 0)
                    {
                        if (coupon.MinPurchase == 0)
                        {
                            foundValidShipment = true;
                        }
                        else
                        {
                            minPurchaseRequired = true;
                            LSDecimal valueOfItemsPurchased = CouponCalculator.GetItemTotal(shipmentItems, productTypes, CouponRule.All, null);
                            if (valueOfItemsPurchased < 0) valueOfItemsPurchased = 0;
                            foundValidShipment = (coupon.MinPurchase <= valueOfItemsPurchased);                            
                        }
                    }
                    if (foundValidShipment) break;
                }
                if (!foundValidShipment)
                {
                    if (minPurchaseRequired)
                    {
                        message = "The coupon code you've entered requires a shipment with value of items to be {0:ulc} or more.";
                        message = String.Format(message, coupon.MinPurchase);
                    }
                    else
                    {
                        message = "The coupon code you've entered does not apply to any shipment.";
                    }
                    //message = String.Format(message, coupon.MinPurchase);
                    return false;
                }
            }
            message = "";
            return true;
        }

        /// <summary>
        /// Processes coupons in the given basket
        /// </summary>
        /// <param name="basket">The basket to process coupons for</param>
        /// <param name="removeZeroValueCoupons">If <b>true</b> zero valued coupons are removed</param>
        public static void ProcessBasket(Basket basket, bool removeZeroValueCoupons)
        {
            //REMOVE ANY EXISTING COUPON ITEMS FROM BASKET
            for (int i = basket.Items.Count - 1; i >= 0; i--)
            {
                BasketItem item = basket.Items[i];
                if (item.OrderItemType == OrderItemType.Coupon)
                {
                    basket.Items.DeleteAt(i);
                }
            }

            List<BasketCoupon> orderCoupons = new List<BasketCoupon>();            
            //NOW LOOP ALL COUPONS USED FOR ORDER
            for (int i = basket.BasketCoupons.Count - 1; i >= 0; i--)
            {
                BasketCoupon basketCoupon = basket.BasketCoupons[i];
                if (CouponCalculator.IsCouponValid(basket, basketCoupon.Coupon))
                {
                    if (basketCoupon.Coupon.CouponType == CouponType.Order)
                    {
                        //keep order coupons for later processing
                        orderCoupons.Add(basketCoupon);
                    }
                    else
                    {
                        LSDecimal couponValue = ApplyCoupon(basket, basketCoupon.Coupon);
                        if ((couponValue == 0) && removeZeroValueCoupons) basket.BasketCoupons.DeleteAt(i);
                    }
                }
                else
                {
                    basket.BasketCoupons.DeleteAt(i);
                }
            }

            foreach (BasketCoupon cpn in orderCoupons)
            {
                LSDecimal couponValue = ApplyCoupon(basket, cpn.Coupon);
                if ((couponValue == 0) && removeZeroValueCoupons) basket.BasketCoupons.Remove(cpn);
            }

        }

        /// <summary>
        /// Applies a coupon to the given basket
        /// </summary>
        /// <param name="basket">The basket object</param>
        /// <param name="coupon">The coupon to apply</param>
        /// <returns>Applied value of coupon</returns>
        public static LSDecimal ApplyCoupon(Basket basket, Coupon coupon)
        {
            switch (coupon.CouponType)
            {
                case CouponType.Order:
                    return ApplyOrderCoupon(basket, coupon);
                case CouponType.Product:
                    return ApplyProductCoupon(basket, coupon);
                default:
                    return ApplyShippingCoupon(basket, coupon);
            }
        }

        /// <summary>
        /// Returns the value of the products in the basket.
        /// </summary>
        /// <param name="basket">Basket to total items for.</param>
        /// <param name="rule">The rule that specifies how coupon products should be treated.</param>
        /// <param name="couponProducts">Any products associated with the coupon for inclusion or exclusion, depending on the rule.</param>
        /// <param name="taxShipmentLookup">A dictionary that contains the value of items grouped by tax code and shipment</param>
        /// <returns>The value of items in the basket.</returns>
        private static LSDecimal GetOrderValue(Basket basket, CouponRule rule, CouponProductCollection couponProducts, out Dictionary<string, LSDecimal> taxShipmentLookup)
        {
            LSDecimal basketValue = 0;
            taxShipmentLookup = new Dictionary<string, LSDecimal>();
            //GET ARRAY OF PRODUCT IDS
            List<int> productIdList = new List<int>();
            foreach (CouponProduct cp in couponProducts)
                productIdList.Add(cp.ProductId);
            if (productIdList.Count == 0)
            {
                if (rule == CouponRule.AllowSelected) return 0;
                if (rule == CouponRule.ExcludeSelected) rule = CouponRule.All;
            }
            //KEEP TRACK OF BASKET ITEMS THAT ARE ASSOCIATED WITH PRODUCTS
            string taxShipmentKey;
            List<int> productBasketItems = new List<int>();
            switch (rule)
            {
                case CouponRule.AllowSelected:
                    //INCLUDE SELECTED PRODUCT ITEMS
                    foreach (BasketItem bi in basket.Items)
                    {
                        if ((bi.OrderItemType == OrderItemType.Product) && productIdList.Contains(bi.ProductId))
                        {
                            productBasketItems.Add(bi.BasketItemId);
                            basketValue += bi.ExtendedPrice;
                            taxShipmentKey = bi.TaxCodeId.ToString() + "_" + bi.BasketShipmentId.ToString();
                            if (taxShipmentLookup.ContainsKey(taxShipmentKey))
                                taxShipmentLookup[taxShipmentKey] += bi.ExtendedPrice;
                            else taxShipmentLookup[taxShipmentKey] = bi.ExtendedPrice;
                        }
                    }
                    break;
                case CouponRule.ExcludeSelected:
                    //EXCLUDE SELECTED PRODUCT ITEMS
                    foreach (BasketItem bi in basket.Items)
                    {
                        if ((bi.OrderItemType == OrderItemType.Product) && !productIdList.Contains(bi.ProductId))
                        {
                            productBasketItems.Add(bi.BasketItemId);
                            basketValue += bi.ExtendedPrice;
                            taxShipmentKey = bi.TaxCodeId.ToString() + "_" + bi.BasketShipmentId.ToString();
                            if (taxShipmentLookup.ContainsKey(taxShipmentKey))
                                taxShipmentLookup[taxShipmentKey] += bi.ExtendedPrice;
                            else taxShipmentLookup[taxShipmentKey] = bi.ExtendedPrice;
                        }
                    }
                    break;
                default:
                    //TOTAL ALL PRODUCT ITEMS
                    foreach (BasketItem bi in basket.Items)
                    {
                        if (bi.OrderItemType == OrderItemType.Product)
                        {
                            productBasketItems.Add(bi.BasketItemId);
                            basketValue += bi.ExtendedPrice;
                            taxShipmentKey = bi.TaxCodeId.ToString() + "_" + bi.BasketShipmentId.ToString();
                            if (taxShipmentLookup.ContainsKey(taxShipmentKey))
                                taxShipmentLookup[taxShipmentKey] += bi.ExtendedPrice;
                            else taxShipmentLookup[taxShipmentKey] = bi.ExtendedPrice;
                        }
                    }
                    break;
            }
            //INCLUDE ANY DISCOUNTS OR COUPONS FOR INCLUDED ITEMS
            foreach (BasketItem bi in basket.Items)
            {
                if (bi.OrderItemType == OrderItemType.Discount || bi.OrderItemType == OrderItemType.Coupon)
                {
                    //IF THE COUPON OR DISCOUNT HAS NO PARENT IT SHOULD BE INCLUDED IN TOTAL
                    //OTHERWISE INCLUDE IF IT IS A CHILD OF A BASKET ITEM ALREADY ADDED
                    //Shipping and order coupons should be ignored : bug 6774
                    if ( (bi.ParentItemId == 0 && bi.OrderItemType!= OrderItemType.Coupon) 
                         || productBasketItems.Contains(bi.ParentItemId))
                    {
                        basketValue += bi.ExtendedPrice;
                        taxShipmentKey = bi.TaxCodeId.ToString() + "_" + bi.BasketShipmentId.ToString();
                        if (taxShipmentLookup.ContainsKey(taxShipmentKey))
                            taxShipmentLookup[taxShipmentKey] += bi.ExtendedPrice;
                        else taxShipmentLookup[taxShipmentKey] = bi.ExtendedPrice;
                    }
                }
            }
            return basketValue;
        }

        private static LSDecimal ApplyOrderCoupon(Basket basket, Coupon coupon)
        {
            //ORDER COUPONS ALWAYS HAVE A QUANTITY OF 1
            //SO UNIT PRICE IS THE EXTENDED PRICE
            LSDecimal couponExtendedPrice = 0;
            //WE NEED TO KNOW THE TOTAL OF ALL APPLICABLE PRODUCTS
            Dictionary<string, LSDecimal> taxShipmentLookup;
            LSDecimal orderValue = CouponCalculator.GetOrderValue(basket, coupon.ProductRule, coupon.CouponProducts, out taxShipmentLookup);
            if (orderValue < 0) orderValue = 0;
            if (coupon.IsPercent)
            {
                couponExtendedPrice = Math.Round(((Decimal)orderValue * (Decimal)coupon.DiscountAmount) / 100, 2);
            }
            else
            {
                couponExtendedPrice = coupon.DiscountAmount;
            }
            //COUPON CANNOT EXCEED VALUE OF ITEMS
            if (couponExtendedPrice > orderValue) couponExtendedPrice = orderValue;
            if ((coupon.MaxValue > 0) && (couponExtendedPrice > coupon.MaxValue)) couponExtendedPrice = coupon.MaxValue;
            if (couponExtendedPrice > 0)
            {
                int keyIndex = 1;
                LSDecimal totalDiscountApplied = 0;
                foreach (string taxShipmentKey in taxShipmentLookup.Keys)
                {
                    string[] keyParts = taxShipmentKey.Split("_".ToCharArray());
                    int taxCodeId = AlwaysConvert.ToInt(keyParts[0]);
                    int shipmentId = AlwaysConvert.ToInt(keyParts[1]);
                    LSDecimal thisDiscount = 0;
                    //IS THE LAST ITEM IN THE DICTIONARY?
                    if (keyIndex == taxShipmentLookup.Keys.Count)
                    {
                        //YES, THE DISCOUNT IS THE ENTIRE REMAINING AMOUNT
                        thisDiscount = couponExtendedPrice - totalDiscountApplied;
                    }
                    else
                    {
                        //NO, WE NEED TO DETERMINE THE PROPORTIONAL DISCOUNT AMOUNT
                        LSDecimal ratioToTotal = taxShipmentLookup[taxShipmentKey] / orderValue;
                        thisDiscount = Math.Round((decimal)(couponExtendedPrice * ratioToTotal), 2);
                        keyIndex++;
                    }
                    //CREATE THE COUPON LINE ITEM
                    BasketItem couponItem = new BasketItem();
                    couponItem.BasketId = basket.BasketId;
                    couponItem.TaxCodeId = taxCodeId;
                    couponItem.ParentItemId = 0;
                    couponItem.BasketShipmentId = shipmentId;
                    couponItem.OrderItemType = OrderItemType.Coupon;
                    couponItem.Name = coupon.Name;
                    couponItem.Sku = coupon.CouponCode;
                    couponItem.Price = (-1 * thisDiscount);
                    couponItem.Quantity = 1;
                    couponItem.Shippable = MakerShop.Shipping.Shippable.No;
                    couponItem.Save();
                    basket.Items.Add(couponItem);
                    //TRACK THE TOTAL DISCOUNT APPLIED
                    totalDiscountApplied += thisDiscount;
                }
            }
            return couponExtendedPrice;
        }

        private static bool CouponAppliesToItem(Coupon coupon, int productId, int quantity)
        {
            if ((coupon.MinQuantity > 1) && (coupon.MinQuantity > quantity)) return false;
            if (coupon.ProductRule == CouponRule.All) return true;
            bool matchReturn = (coupon.ProductRule == CouponRule.AllowSelected);
            CouponProductCollection cpc = coupon.CouponProducts;
            foreach (CouponProduct cp in cpc)
            {
                if (cp.ProductId.Equals(productId)) return matchReturn;
            }
            return !matchReturn;
        }

        private static IntervalQuantity CalculateIntervalQuantity(int quantity, int minQuantity, int maxQuantity, int interval)
        {
            if (minQuantity < 1) minQuantity = 1;
            if (quantity < minQuantity) return new IntervalQuantity(0, 0);
            if (quantity == minQuantity) return new IntervalQuantity(minQuantity, 1);
            if ((maxQuantity > 0) && (quantity >= maxQuantity)) quantity = maxQuantity;
            if (interval < 1) interval = 1;
            int span = quantity - minQuantity;
            int steps = (int)Math.Floor((Decimal)(span / interval));
            int tempQuantity = minQuantity + (steps * interval);
            int tempMultiplier = (steps + 1);
            return new IntervalQuantity((int)(tempQuantity / tempMultiplier), tempMultiplier);
        }

        private class IntervalQuantity
        {
            public int Quantity;
            public int Mulitplier;
            public IntervalQuantity(int quantity, int multiplier)
            {
                this.Quantity = quantity;
                this.Mulitplier = multiplier;
            }
        }

        private static LSDecimal ApplyProductCoupon(Basket basket, Coupon coupon)
        {
            LSDecimal couponUnitPrice = 0;
            LSDecimal couponExtendedPrice = 0;
            LSDecimal itemUnitDiscount = 0;
            LSDecimal itemUnitPrice = 0;
            LSDecimal itemExtendedPrice;
            List<BasketItem> newItems = new List<BasketItem>();
            foreach (BasketItem item in basket.Items)
            {
                if (item.OrderItemType == OrderItemType.Product && !item.IsChildItem && item.Product!=null && !item.Product.IsGiftCertificate)
                {
                    if (CouponCalculator.CouponAppliesToItem(coupon, item.ProductId, item.Quantity))
                    {
                        //GET THE QUANTITY TO APPLY ON
                        IntervalQuantity iq = CouponCalculator.CalculateIntervalQuantity(item.Quantity, coupon.MinQuantity, coupon.MaxQuantity, coupon.QuantityInterval);
                        //ADD THE COUPON ITEM
                        if (iq.Quantity > 0)
                        {
                            //CALCULATE DISCOUNT FOR EACH UNIT
                            itemUnitDiscount = GetUnitDiscount(item, basket);
                            itemUnitPrice = (item.Price + itemUnitDiscount);
                            //CALCULATE TOTAL VALUE: PRICE OF ITEM (AFTER DISCOUNT) TIMES INTERVAL QUANTITY
                            itemExtendedPrice = itemUnitPrice * iq.Quantity;
                            if (coupon.IsPercent)
                            {
                                //CALCULATE COUPON UNIT PRICE BASED ON ITEM VALUE
                                couponUnitPrice = Math.Round(((Decimal)(itemExtendedPrice) * (Decimal)coupon.DiscountAmount) / 100, 2);
                            }
                            else
                            {
                                //USE FIXED VALUE
                                couponUnitPrice = coupon.DiscountAmount;
                            }
                            //COUPON CANNOT EXCEED VALUE OF LINE ITEM
                            couponExtendedPrice = (couponUnitPrice * iq.Quantity);
                            if (couponExtendedPrice > itemExtendedPrice) couponUnitPrice = itemUnitPrice;
                            //TOTAL VALUE OF COUPON CANNOT EXCEED MAX VALUE
                            if ((coupon.MaxValue > 0) && (couponExtendedPrice > coupon.MaxValue)) couponUnitPrice = coupon.MaxValue / iq.Quantity;
                            //CREATE THE COUPON LINE ITEM
                            BasketItem couponItem = new BasketItem();
                            couponItem.BasketId = basket.BasketId;
                            couponItem.OrderItemType = OrderItemType.Coupon;
                            couponItem.ParentItemId = item.BasketItemId;
                            couponItem.BasketShipmentId = item.BasketShipmentId;
                            couponItem.Name = coupon.Name;
                            couponItem.Sku = coupon.CouponCode;
                            couponItem.Price = (-1 * couponUnitPrice);
                            couponItem.Quantity = (short)iq.Mulitplier;
                            couponItem.TaxCodeId = item.TaxCodeId;
                            newItems.Add(couponItem);
                        }
                    }
                }
            }
            foreach (BasketItem item in newItems)
            {
                item.Save();
                basket.Items.Add(item);
            }
            return couponExtendedPrice;
        }

        /// <summary>
        /// Get the discount amount for a single quantity of the item.
        /// </summary>
        /// <param name="item">Item to find unit discount for</param>
        /// <param name="basket">Basket containing the item</param>
        /// <returns>The discount amount applied to a single quantity of the item.</returns>
        private static LSDecimal GetUnitDiscount(BasketItem item, Basket basket)
        {
            //MAKE SURE QUANTITY OF ITEM IS VALID
            if (item.Quantity < 1) return 0;
            //GET THE DISCOUNTS ASSOCIATED WITH THIS BASKET ITEM
            BasketItemCollection discountItems = basket.Items.GetChildItems(item, OrderItemType.Discount);
            //GET TOTAL OF ALL DISCOUNTS FOR THE BASKET ITEM
            LSDecimal totalDiscount = 0;
            foreach (BasketItem bitem in discountItems)
            {
                totalDiscount += bitem.ExtendedPrice;
            }
            //CALCULATE AND RETURN UNIT DISCOUNT
            return totalDiscount / item.Quantity;
        }

        private static LSDecimal ApplyShippingCoupon(Basket basket, Coupon coupon)
        {
            LSDecimal totalCouponValue = 0;
            //GET A LIST OF PRODUCTS ASSOCIATED WITH COUPON
            List<int> productIds = new List<int>();
            CouponProductCollection cpc = coupon.CouponProducts;
            foreach (CouponProduct cp in cpc)
            {
                productIds.Add(cp.ProductId);
            }
            OrderItemType[] productTypes = { OrderItemType.Product, OrderItemType.Discount, OrderItemType.Coupon };
            //SHIPPING COUPONS MUST BE APPLIED ON A PER-SHIPMENT BASIS
            foreach (BasketShipment shipment in basket.Shipments)
            {
                BasketItemCollection shipmentItems = shipment.GetItems(basket);
                LSDecimal shipmentItemsTotal = CouponCalculator.GetItemTotal(shipmentItems, productTypes, CouponRule.All, null);
                if (shipmentItemsTotal < 0) shipmentItemsTotal = 0;
                if ((coupon.MinPurchase == 0) || (coupon.MinPurchase <= shipmentItemsTotal))
                {
                    //THIS COUPON MEETS THE MINIMUM PURCHASE REQUIMENTS FOR THIS SHIPMENT
                    //GET THE TOTAL OF SHIPPING CHARGES
                    List<int> shipMethodIds = new List<int>();
                    foreach (CouponShipMethod csm in coupon.CouponShipMethods)
                    {
                        shipMethodIds.Add(csm.ShipMethodId);
                    }
                    LSDecimal shippingTotal = CouponCalculator.GetShippingTotal(shipmentItems, coupon.ProductRule, shipMethodIds.ToArray());
                    if (shippingTotal > 0)
                    {
                        LSDecimal couponValue;
                        if (coupon.IsPercent) couponValue = Math.Round(((Decimal)shippingTotal * (Decimal)coupon.DiscountAmount) / 100, 2);
                        else couponValue = coupon.DiscountAmount;
                        if (couponValue > 0)
                        {
                            if (couponValue > shippingTotal) couponValue = shippingTotal;
                            if ((coupon.MaxValue > 0) && (couponValue > coupon.MaxValue)) couponValue = coupon.MaxValue;
                            BasketItem couponItem = new BasketItem();
                            couponItem.BasketId = basket.BasketId;
                            couponItem.BasketShipmentId = shipment.BasketShipmentId;
                            couponItem.OrderItemType = OrderItemType.Coupon;
                            couponItem.Name = coupon.Name;
                            couponItem.Sku = coupon.CouponCode;
                            couponItem.Price = (-1 * couponValue);
                            couponItem.Quantity = 1;
                            if(shipment.ShipMethod!=null) couponItem.TaxCodeId = shipment.ShipMethod.TaxCodeId;
                            couponItem.Save();
                            basket.Items.Add(couponItem);
                            totalCouponValue += couponValue;
                        }
                    }
                }
            }
            return totalCouponValue;
        }

        private static LSDecimal GetItemTotal(BasketItemCollection items, OrderItemType[] itemTypes, CouponRule rule, int[] productIds)
        {
            return GetItemTotal(items, itemTypes, rule, productIds, 1);
        }

        private static LSDecimal GetItemTotal(BasketItemCollection items, OrderItemType[] itemTypes, CouponRule rule, int[] productIds, int minQuantity)
        {
            LSDecimal total = 0;
            bool minQRequired = false;
            foreach (BasketItem item in items)
            {
                //CHECK THAT ITEM TYPE SHOULD BE INCLUDED
                if ((itemTypes == null) || (Array.IndexOf(itemTypes, item.OrderItemType) > -1))
                {
                    if (item.OrderItemType == OrderItemType.Product)
                    {
                        if (item.Product == null || item.Product.IsGiftCertificate)
                        {
                            continue;
                        }
                    }

                    if (item.Quantity < minQuantity)
                    {
                        minQRequired = true;
                        continue;
                    }

                    //CHECK RULE
                    if (rule == CouponRule.All)
                    {
                        total += item.ExtendedPrice;
                    }
                    else if (rule == CouponRule.AllowSelected)
                    {
                        //ALLOW SELECTED
                        if (Array.IndexOf(productIds, item.ProductId) > -1) total += item.ExtendedPrice;
                    }
                    else
                    {
                        //EXCLUDE SELECTED
                        if (Array.IndexOf(productIds, item.ProductId) < 0) total += item.ExtendedPrice;
                    }                
                }
            }
            if (total == 0 && minQRequired)
            {
                total = -1;
            }
            return total;
        }

        private static LSDecimal GetShippingTotal(BasketItemCollection items, CouponRule shipMethodRule, int[] shipMethodIds)
        {
            LSDecimal total = 0;
            foreach (BasketItem item in items)
            {
                //CHECK THAT ITEM TYPE SHOULD BE INCLUDED
                if (item.OrderItemType == OrderItemType.Shipping || item.OrderItemType == OrderItemType.Handling)
                {
                    if (item.OrderItemType == OrderItemType.Product)
                    {
                        if (item.Product == null || item.Product.IsGiftCertificate)
                        {
                            continue;
                        }
                    }
                    //CHECK RULE
                    if (shipMethodRule == CouponRule.All)
                    {
                        total += item.ExtendedPrice;
                    }
                    else if (shipMethodRule == CouponRule.AllowSelected)
                    {
                        //ALLOW SELECTED
                        if (Array.IndexOf(shipMethodIds, item.BasketShipment.ShipMethodId) > -1) total += item.ExtendedPrice;
                    }
                    else
                    {                            
                        //EXCLUDE SELECTED
                        if (Array.IndexOf(shipMethodIds, item.BasketShipment.ShipMethodId) < 0) total += item.ExtendedPrice;                            
                    }
                }
            }
            return total;
        }
    }
}
