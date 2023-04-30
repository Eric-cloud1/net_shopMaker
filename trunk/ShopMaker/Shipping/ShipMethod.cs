using System;
using MakerShop.Orders;
using MakerShop.Shipping.Providers;
using MakerShop.Common;
using System.Data.Common;
using MakerShop.Data;

namespace MakerShop.Shipping
{
    public partial class ShipMethod
    {
        /// <summary>
        /// The type of this ship method
        /// </summary>
        public ShipMethodType ShipMethodType
        {
            get
            {
                return (ShipMethodType)this.ShipMethodTypeId;
            }
            set
            {
                this.ShipMethodTypeId = (short)value;
            }
        }

        private LSDecimal GetTotalShipmentCost(IShipment shipment)
        {
            LSDecimal totalCost = 0;
            BasketItemCollection shipmentItems = shipment.GetItems();
            OrderItemType[] includedItemTypes = { OrderItemType.Product, OrderItemType.Discount, OrderItemType.Coupon };
            foreach (BasketItem item in shipmentItems)
            {
                if (Array.IndexOf(includedItemTypes, item.OrderItemType) > -1)
                {
                    totalCost += item.ExtendedPrice;
                }
            }
            return totalCost;
        }

        private LSDecimal GetTotalShipmentWeight(IShipment shipment)
        {
            LSDecimal totalWeight = 0;
            BasketItemCollection shipmentItems = shipment.GetItems();
            foreach (BasketItem item in shipmentItems)
            {
                if (item.OrderItemType == OrderItemType.Product)
                {
                    totalWeight += item.ExtendedWeight;
                }
            }
            return totalWeight;
        }

        private int GetTotalShipmentQuantity(IShipment shipment)
        {
            int totalQuantity = 0;
            BasketItemCollection shipmentItems = shipment.GetItems();
            foreach (BasketItem item in shipmentItems)
            {
                if (item.OrderItemType == OrderItemType.Product)
                {
                    totalQuantity += item.Quantity;
                }
            }
            return totalQuantity;
        }

        private ShipRateMatrix GetShipRateMatrixItem(LSDecimal criteria)
        {
            foreach (ShipRateMatrix rateItem in this.ShipRateMatrices)
            {
                if (((rateItem.RangeStart == 0) || (rateItem.RangeStart <= criteria)) && ((rateItem.RangeEnd == 0) || (rateItem.RangeEnd >= criteria)))
                {
                    return rateItem;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets a ship rate quote for the given shippment using this ship method
        /// </summary>
        /// <param name="shipment">The shipment to get rate quote for</param>
        /// <returns>ShipRateQuote object containing the ship rate quote or null if ship rate quote could not be obtained</returns>
        public ShipRateQuote GetShipRateQuote(IShipment shipment)
        {
            ShipRateQuote quote;
            if (this.ShipGateway != null)
            {
                //Proceed only if ship gateway is enabled
                if (!this.ShipGateway.Enabled) return null;

                //RATE CALCULATION MADE BY PROVIDER
                try
                {
                    IShippingProvider provider = this.ShipGateway.GetProviderInstance();
                    if (provider != null)
                    {
                        ShipRateQuote providerQuote = provider.GetShipRateQuote(shipment, this.ServiceCode);
                        if (providerQuote == null) return null;
                        //WE WANT TO USE A CLONE OF THE PROVIDER QUOTE BECAUSE THE PROVIDER MAY CACHE THE OBJECT
                        //AND WE MAY MODIFY THE RATE WITH A SURCHARGE IN OUR PROCESSING (SEE BUG 6353)
                        quote = providerQuote.Clone();
                        quote.ShipMethod = this;
                    }
                    else
                    {
                        Utility.Logger.Warn("Could not obtain provider instance for '" + this.Name + "'. Instance is null.");
                        return null;
                    }
                }
                catch (Exception e)
                {
                    Utility.Logger.Warn("Error obtaining rate quote for shipping method '" + this.Name + "'.", e);
                    return null;
                }
            }
            else
            {
                //RATE CALCULATION MADE BY AC
                ShipRateMatrix rateItem;
                quote = new ShipRateQuote();
                quote.ShipMethod = this;
                switch (this.ShipMethodType)
                {
                    case ShipMethodType.FlatRate:
                        if (this.ShipRateMatrices.Count > 0)
                        {
                            quote.Rate = this.ShipRateMatrices[0].Rate;
                        }
                        break;
                    case ShipMethodType.CostBased:
                        //NEED TOTAL COST OF SHIPMENT
                        LSDecimal totalCost = GetTotalShipmentCost(shipment);
                        rateItem = GetShipRateMatrixItem(totalCost);
                        //CHECK FOR APPLIED RATE
                        if (rateItem == null) return null;
                        if (!rateItem.IsPercent) quote.Rate = rateItem.Rate;
                        else quote.Rate = (decimal)Math.Round((double)((rateItem.Rate / 100) * totalCost), 2);
                        break;
                    case ShipMethodType.WeightBased:
                        //NEED TOTAL WEIGHT OF SHIPMENT
                        LSDecimal totalWeight = GetTotalShipmentWeight(shipment);
                        rateItem = GetShipRateMatrixItem(totalWeight);
                        //CHECK FOR APPLIED RATE
                        if (rateItem == null) return null;
                        if (!rateItem.IsPercent) quote.Rate = rateItem.Rate;
                        else quote.Rate = (decimal)Math.Round((double)((rateItem.Rate / 100) * totalWeight), 2);
                        break;
                    case ShipMethodType.QuantityBased:
                        //NEED TOTAL QUANTITY OF SHIPMENT
                        LSDecimal totalQuantity = GetTotalShipmentQuantity(shipment);
                        rateItem = GetShipRateMatrixItem(totalQuantity);
                        //CHECK FOR APPLIED RATE
                        if (rateItem == null) return null;
                        if (!rateItem.IsPercent) quote.Rate = rateItem.Rate;
                        // FIXED BUG: 5132 (I believe it should be calculating the percent of the cost of each shipment)
                        else quote.Rate = (decimal)Math.Round((double)((rateItem.Rate / 100) * GetTotalShipmentCost(shipment)), 2);
                        break;
                }
            }

            //ADD ADDITIONAL SURCHARGE DEFINED FOR THIS SHIP METHOD
            LSDecimal surchargeAmount;
            if (this.SurchargeIsPercent)
                surchargeAmount = Math.Round(((Decimal)quote.Rate / 100) * (Decimal)this.Surcharge, 2);
            else surchargeAmount = this.Surcharge;
            if (this.SurchargeIsVisible)
                quote.Surcharge = surchargeAmount;
            else quote.Rate += surchargeAmount;
            return quote;
        }

        /// <summary>
        /// Indicates whether this ship method is applicable to given address
        /// </summary>
        /// <param name="address">The address for which to check applicability</param>
        /// <returns>true if applicable, false otherwise</returns>
        public bool IsApplicableTo(Users.Address address)
        {
            if (this.ShipMethodShipZones == null || this.ShipMethodShipZones.Count == 0)
            {
                //No ship zones defined. Applies to all.
                return true;
            }

            //ship zones defined. determine applicability
            bool applicable = false;
            ShipZoneCountryCollection countryCol;
            ShipZoneProvinceCollection provinceCol;
            ShipZoneCountry country;
            ShipZoneProvince province;

            foreach (ShipMethodShipZone mShipZone in this.ShipMethodShipZones)
            {
                countryCol = mShipZone.ShipZone.ShipZoneCountries;
                provinceCol = mShipZone.ShipZone.ShipZoneProvinces;

                if (countryCol != null)
                {
                    country = FindMatchingCountry(countryCol, address);
                    if (country != null)
                    {
                        //country found in zone
                        if (provinceCol == null || provinceCol.Count == 0)
                        {
                            applicable = true;
                            break;
                        }
                        else
                        {
                            province = FindMatchingProvince(provinceCol, address);
                            if (province != null)
                            {
                                //province found in zone
                                applicable = true;
                                break;
                            }
                        }
                    }
                }
            }

            return applicable;
        }

        private ShipZoneCountry FindMatchingCountry(ShipZoneCountryCollection col, Users.Address address)
        {
            foreach (ShipZoneCountry country in col)
            {
                if (address.CountryCode.Equals(country.CountryCode, StringComparison.OrdinalIgnoreCase))
                {
                    return country;
                }
            }
            return null;
        }

        private ShipZoneProvince FindMatchingProvince(ShipZoneProvinceCollection col, Users.Address address)
        {
            foreach (ShipZoneProvince province in col)
            {
                if (address.CountryCode.Equals(province.Province.CountryCode, StringComparison.OrdinalIgnoreCase)
                    && (address.Province.Equals(province.Province.ProvinceCode, StringComparison.OrdinalIgnoreCase)))
                {
                    return province;
                }
            }
            return null;
        }

        /// <summary>
        /// Deletes this ShipMethod object from the database.
        /// </summary>
        /// <returns><b>true</b> if delete successful, <b>false</b> otherwise.</returns>
        public bool Delete()
        {
            // REMOVE SHIPMETHOD GROUP ASSOCIATION
            Database database = Token.Instance.Database;
            DbCommand deleteCommand = database.GetSqlStringCommand("DELETE FROM ac_ShipMethodGroups WHERE ShipMethodId = " + this.ShipMethodId);

            MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null); 
            database.ExecuteNonQuery(deleteCommand);
            MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();
            return this.BaseDelete();
        }
    }
}
