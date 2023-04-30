using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace MakerShop.Taxes.Providers
{
    /// <summary>
    /// DataSource class for TaxProvider objects
    /// </summary>
    public class TaxProviderDataSource
    {
        /// <summary>
        /// Gets instances of all classes available that implement the ITaxProvider interface
        /// </summary>
        /// <returns>List of instances of classes implementing ITaxProvider</returns>
        public static List<ITaxProvider> GetTaxProviders()
        {
            return GetTaxProviders(true);
        }

        /// <summary>
        /// Gets instances of all classes available that implement the ITaxProvider interface
        /// </summary>
        /// <param name="includeConfigured">Whether to include configured providers or not</param>
        /// <returns>List of instances of classes implementing ITaxProvider</returns>
        public static List<ITaxProvider> GetTaxProviders(bool includeConfigured)
        {
            List<ITaxProvider> providers = new List<ITaxProvider>();
            TaxGatewayCollection configuredGateways = null;
            if (!includeConfigured) configuredGateways = TaxGatewayDataSource.LoadForStore();
            
            if (HttpContext.Current != null)
            {
                HttpServerUtility server = HttpContext.Current.Server;
                string[] files = System.IO.Directory.GetFiles(server.MapPath("~/bin"), "*.DLL");
                List<string> providerNames = new List<string>();
                foreach (System.Reflection.Assembly assemblyInstance in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        foreach (Type thisType in assemblyInstance.GetTypes())
                        {
                            if ((thisType.IsClass && !thisType.IsAbstract))
                            {
                                foreach (Type thisInterface in thisType.GetInterfaces())
                                {
                                    ITaxProvider instance = null;
                                    if ((!string.IsNullOrEmpty(thisInterface.FullName) && thisInterface.FullName.Equals("MakerShop.Taxes.Providers.ITaxProvider")))
                                    {
                                        string classId = Utility.Misc.GetClassId(thisType);
                                        string loweredClassId = classId.ToLowerInvariant();
                                        if (!providerNames.Contains(loweredClassId))
                                        {
                                            if (includeConfigured || !IsConfigured(configuredGateways, classId))
                                            {
                                                instance = Activator.CreateInstance(Type.GetType(classId)) as ITaxProvider;
                                                if (instance != null)
                                                {
                                                    providers.Add(instance);
                                                }
                                                providerNames.Add(loweredClassId);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                    catch
                    {
                        //ignore error
                    }
                }
            }
            return providers;
        }

        private static bool IsConfigured(TaxGatewayCollection gateways, string classId)
        {
            foreach (TaxGateway gateway in gateways)
            {
                if (gateway.ClassId.Equals(classId)) return true;
            }
            return false;
        }

    }
}
