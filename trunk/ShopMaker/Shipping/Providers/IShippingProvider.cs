using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using MakerShop.Orders;
using MakerShop.Users;

namespace MakerShop.Shipping.Providers
{
    /// <summary>
    /// Interface to be implemented by Shipping Providers
    /// </summary>
    public interface IShippingProvider
    {
        /// <summary>
        /// Id of the shipping gateway in database. Id is passed at the time of initialization.
        /// </summary>
        int ShipGatewayId { get; }

        /// <summary>
        /// Name of the shipping provider implementation
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Version of the shipping provider implementation
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Description of the shipping provider implementation
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets a Url for the logo of the shipping provider implementation
        /// </summary>
        /// <param name="cs">ClientScriptManager</param>
        /// <returns>A Url for the logo of the shipping provider implementation</returns>
        string GetLogoUrl(System.Web.UI.ClientScriptManager cs);

        /// <summary>
        /// Gets a Url for the configuration of the shipping provider implementation
        /// </summary>
        /// <param name="cs">ClientScriptManager</param>
        /// <returns>A Url for the configuration of the shipping provider implementation</returns>
        string GetConfigUrl(System.Web.UI.ClientScriptManager cs);

        /// <summary>
        /// Whether to use debugging mode or not?
        /// </summary>
        bool UseDebugMode { get; set; }

        /// <summary>
        /// Initializes the shipping provider implementation. Called by AC at the time of initialization.
        /// </summary>
        /// <param name="shipGatewayId">Id of the shipping gateway in database</param>
        /// <param name="configurationData">Configuration data as name-value pairs</param>
        void Initialize(int shipGatewayId, Dictionary<String, String> configurationData);

        /// <summary>
        /// Gets the configuration data as name-value paris
        /// </summary>
        /// <returns>Configuration data as name-value paris</returns>
        Dictionary<string, string> GetConfigData();

        /// <summary>
        /// Gets a list of services provided by the shipping provider implementation
        /// </summary>
        /// <returns>An array of ListItem objects containing services provided by the shipping provider implementation</returns>
        ListItem[] GetServiceListItems();

        /// <summary>
        /// Gets a shipping rate quote for shipping the given basket items from given warehouse to the given destination address
        /// </summary>
        /// <param name="origin">Warehouse from where shipment initiates</param>
        /// <param name="destination">The destination address for the shipment</param>
        /// <param name="contents">Contents of the shipment</param>
        /// <param name="serviceCode">Service code to get rate quote for</param>
        /// <returns>ShipRateQuote for the given shipping requirements</returns>
        ShipRateQuote GetShipRateQuote(Warehouse origin, Address destination, BasketItemCollection contents, string serviceCode);

        /// <summary>
        /// Gets a shipping rate quote for shipping the given shipment
        /// </summary>
        /// <param name="shipment">The shipment to get rate quote for</param>
        /// <param name="serviceCode">Service code to get rate quote for</param>
        /// <returns>ShipRateQuote for the given shipment</returns>
        ShipRateQuote GetShipRateQuote(IShipment shipment, string serviceCode);

        //Get all quotes from a shipping gateway in one operation
        //List<ShipRateQuote> GetShipRateQuotes(BasketShipment shipment, List<ShipMethod> shipMethods);
        //List<ShipRateQuote> GetShipRateQuotes(Warehouse origin, Address destination, BasketItemCollection contents, List<ShipMethod> shipMethods);

        /// <summary>
        /// Gets tracking summary for a given tracking number
        /// </summary>
        /// <param name="trackingNumber"></param>
        /// <returns></returns>
        TrackingSummary GetTrackingSummary(TrackingNumber trackingNumber);

    }
}
