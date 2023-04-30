using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Orders;

namespace MakerShop.Shipping
{
    /// <summary>
    /// DataSource class for ShipZone objects
    /// </summary>
    [DataObject(true)]
    public partial class ShipZoneDataSource
    {
        /// <summary>
        /// Loads a collection of ShipZone objects for the given basket shipment
        /// </summary>
        /// <param name="shipment">BasketShipment to load the ship zones for</param>
        /// <returns>A collection of ShipZone objects for the given basket shipment</returns>
        public static ShipZoneCollection LoadForShipment(BasketShipment shipment)
        {
            return LoadForAddress(shipment.Address);
        }

        /// <summary>
        /// Loads a collection of ShipZone objects for the given address
        /// </summary>
        /// <param name="address">Address to load the ship zones for</param>
        /// <returns>A collection of ShipZone objects for the given address</returns>
        public static ShipZoneCollection LoadForAddress(Users.Address address)
        {
            //LOAD ALL ZONES
            ShipZoneCollection allZones = ShipZoneDataSource.LoadForStore();
            //BUILD LIST OF ZONES THAT MEET CRITERIA
            ShipZoneCollection applicableZones = new ShipZoneCollection();
            foreach (ShipZone zone in allZones)
            {
                if (zone.IncludesAddress(address)) applicableZones.Add(zone);
            }
            return applicableZones;
        }

        /// <summary>
        /// Loads a collection of ShipZone objects for the given address
        /// </summary>
        /// <param name="address">Address to load the ship zones for</param>
        /// <returns>A collection of ShipZone objects for the given address</returns>
        public static ShipZoneCollection LoadForAddress(Taxes.TaxAddress address)
        {
            //LOAD ALL ZONES
            ShipZoneCollection allZones = ShipZoneDataSource.LoadForStore();
            //BUILD LIST OF ZONES THAT MEET CRITERIA
            ShipZoneCollection applicableZones = new ShipZoneCollection();
            foreach (ShipZone zone in allZones)
            {
                if (zone.IncludesAddress(address)) applicableZones.Add(zone);
            }
            return applicableZones;
        }

        /// <summary>
        /// Loads a collection of ShipZone objects for the given country
        /// </summary>
        /// <param name="countryCode">The country code of the country to load the ship zones for</param>
        /// <returns>A collection of ShipZone objects for the given country</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ShipZoneCollection LoadForCountry2(String countryCode)
        {
            ShipZoneCollection ShipZones = new ShipZoneCollection();
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT " + ShipZone.GetColumnNames("ac_ShipZones"));
            selectQuery.Append(" FROM ac_ShipZones LEFT JOIN ac_ShipZoneCountries ON ac_ShipZones.ShipZoneId = ac_ShipZoneCountries.ShipZoneId");
            selectQuery.Append(" WHERE StoreId = @storeId");
            selectQuery.Append(" AND (ac_ShipZoneCountries.CountryCode IS NULL OR ac_ShipZoneCountries.CountryCode = @CountryCode)");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@countryCode", System.Data.DbType.String, countryCode);
            database.AddInParameter(selectCommand, "@storeId", System.Data.DbType.Int32, Token.Instance.StoreId);
            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    ShipZone shipZone = new ShipZone();
                    ShipZone.LoadDataReader(shipZone, dr);
                    ShipZones.Add(shipZone);
                }
                dr.Close();
            }
            return ShipZones;
        }

    }
}
