using MakerShop.Common;

namespace MakerShop.Shipping
{
    public partial class ShipZone
    {
        /// <summary>
        /// Gets or sets the filter rule for the associated country list
        /// </summary>
        public FilterRule CountryRule
        {
            get { return (FilterRule)this.CountryRuleId; }
            set { this.CountryRuleId = (byte)value; }
        }

        /// <summary>
        /// Gets or sets the filter rule for the associated country list
        /// </summary>
        public FilterRule ProvinceRule
        {
            get { return (FilterRule)this.ProvinceRuleId; }
            set { this.ProvinceRuleId = (byte)value; }
        }

        /// <summary>
        /// Creates a copy of the given ShipZone 
        /// </summary>
        /// <param name="shipZoneId">Id of the ShipZone to create copy of</param>
        /// <param name="deepCopy">if <b>true</b> chiled objects are also copied</param>
        /// <returns></returns>
        public static ShipZone Copy(int shipZoneId, bool deepCopy)
        {
            ShipZone copy = ShipZoneDataSource.Load(shipZoneId);
            if (copy != null)
            {
                if (deepCopy)
                {
                    //LOAD THE CHILD COLLECTIONS AND RESET
                    foreach (ShipZoneCountry sc in copy.ShipZoneCountries)
                    {
                        sc.ShipZoneId = 0;
                    }
                    foreach (ShipZoneProvince sp in copy.ShipZoneProvinces)
                    {
                        sp.ShipZoneId = 0;
                    }
                }
                copy.ShipZoneId = 0;
                return copy;
            }
            return null;
        }

        /// <summary>
        /// Delete this ShipZone object from database
        /// </summary>
        /// <returns></returns>
        public bool Delete()
        {
            //DELETE ANY ASSOCIATED SHIPPING METHODS
            this.ShipMethodShipZones.DeleteAll();
            return this.BaseDelete();
        }

        /// <summary>
        /// Indicates whether the given address is included in the zone.
        /// </summary>
        /// <param name="address">The address to test against the zone criteria</param>
        /// <returns>True if the address is included in the zone, false otherwise.</returns>
        public bool IncludesAddress(Users.Address address)
        {
            return this.IncludesAddress(address.CountryCode, address.ProvinceId, address.PostalCode);
        }

        /// <summary>
        /// Indicates whether the given address is included in the zone.
        /// </summary>
        /// <param name="address">The address to test against the zone criteria</param>
        /// <returns>True if the address is included in the zone, false otherwise.</returns>
        public bool IncludesAddress(Taxes.TaxAddress address)
        {
            return this.IncludesAddress(address.CountryCode, address.ProvinceId, address.PostalCode);
        }

        public bool IncludesAddress(string countryCode, int provinceId, string postalCode)
        {
            if (!this.IncludesCountry(countryCode)) return false;
            if (!this.IncludesProvince(provinceId)) return false;
            if (!PostalCodeValidator.IsMatch(this.PostalCodeFilter, postalCode)) return false;
            // IF THERE ARE NO EXCLUSIONS THIS IS VALID
            if (string.IsNullOrEmpty(this.ExcludePostalCodeFilter)) return true;
            // ZONE INCLUDES ADDRESS IF POSTAL CODE IS NOT IN EXCLUSION LIST
            return !PostalCodeValidator.IsMatch(this.ExcludePostalCodeFilter, postalCode);
        }

        private bool IncludesCountry(string countryCode)
        {
            if (this.CountryRule == FilterRule.All) return true;
            bool countryAssociationExists = ShipZoneCountryDataSource.Exists(this.ShipZoneId, countryCode);
            //FOR INCLUDE RULE, COUNTRY NEEDS TO BE IN LIST
            if (this.CountryRule == FilterRule.IncludeSelected) return countryAssociationExists;
            //FOR EXCLUDE RULE, COUNTRY NEEDS TO NOT BE IN LIST
            return !countryAssociationExists;
        }

        private bool IncludesProvince(int provinceId)
        {
            if (this.ProvinceRule == FilterRule.All) return true;
            bool provinceAssociationExists = ShipZoneProvinceDataSource.Exists(this.ShipZoneId, provinceId);
            //FOR INCLUDE RULE, PROVINCE NEEDS TO BE IN LIST
            if (this.ProvinceRule == FilterRule.IncludeSelected) return provinceAssociationExists;
            //FOR EXCLUDE RULE, PROVINCE NEEDS TO NOT BE IN LIST
            return !provinceAssociationExists;
        }
    }
}