using System;
using System.Collections.Generic;
using System.Text;
using MakerShop.Orders;

namespace MakerShop.DigitalDelivery
{
    /// <summary>
    /// Interface that must be implemented by digital delivery serial key providers
    /// </summary>
    public interface ISerialKeyProvider
    {
        /// <summary>
        /// Name of the provider
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Implementation version
        /// </summary>
        string Version { get; }

        /// <summary>
        /// A description of the provider implementation
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The configuration URL where the provider can be configured
        /// </summary>
        /// <param name="cs">ClientScriptManager</param>
        /// <returns>The configuration URL where the provider can be configured</returns>
        string GetConfigUrl(System.Web.UI.ClientScriptManager cs);

        /// <summary>
        /// Initializes the provider/module with the configuration data.
        /// </summary>
        /// <param name="digitalGood">The digital good</param>
        /// <param name="configurationData">The configuration data</param>
        void Initialize(DigitalGood digitalGood, Dictionary<String, String> configurationData);

        /// <summary>
        /// The digital good associated. Passed during initialization.
        /// </summary>
        /// <returns>The DigitalGood associated</returns>
        DigitalGood GetDigitalGood();

        /// <summary>
        /// The configuration data
        /// </summary>
        /// <returns>The configuration data</returns>
        Dictionary<string, string> GetConfigData();

        /// <summary>
        /// Acquires a new serial key from provider
        /// </summary>        
        /// <returns>Serial key acquisition response</returns>
        AcquireSerialKeyResponse AcquireSerialKey();

        /// <summary>
        /// Acquires a new serial key from provider
        /// </summary>
        /// <param name="oidg">OrderItemDigitalGood for which to acquire the key</param>
        /// <returns>Serial key acquisition response</returns>
        AcquireSerialKeyResponse AcquireSerialKey(OrderItemDigitalGood oidg);

        /// <summary>
        /// Returns a serial key back to the provider store
        /// </summary>
        /// <param name="keyData">The key to return</param>
        void ReturnSerialKey(string keyData);

        /// <summary>
        /// Returns a serial key back to the provider store
        /// </summary>
        /// <param name="keyData">The key to return</param>
        /// <param name="oidg">OrderItemDigitalGood the returned key is associated to</param>
        void ReturnSerialKey(string keyData, OrderItemDigitalGood oidg);

        /// <summary>
        /// Upgrages a key to the new Level
        /// </summary>
        /// <param name="oidg"></param>
        /// <param name="newLevel"></param>
        /// <returns>Serial key upgrade response</returns>
        AcquireSerialKeyResponse UpgradeSerialKey(OrderItemDigitalGood oidg, string newLevel );

        /// <summary>
        /// Login for a Digital Good.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="URL"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        bool Login(string username, string password, out string URL, out List<string> messages);

    }
}
