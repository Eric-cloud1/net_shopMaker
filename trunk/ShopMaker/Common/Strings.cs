using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;
using System.Reflection;

namespace MakerShop.Common
{
    /// <summary>
    /// Utility class for accessing resource strings
    /// </summary>
    public static class Strings
    {
        private static ResourceManager resourceManager;

        /// <summary>
        /// static constructor
        /// </summary>
        static Strings()
        {
            Assembly resourceAssembly = Assembly.GetExecutingAssembly();
            resourceManager = new ResourceManager("MakerShop.Resources.StringResources", resourceAssembly);
            resourceManager.IgnoreCase = true;
        }

        /// <summary>
        /// Gets the resource string for the given key
        /// </summary>
        /// <param name="key">Key to get the resource string for</param>
        /// <returns>The resource string for the given key</returns>
        public static string GetString(string key)
        {
            try
            {
                string s = resourceManager.GetString(key);
                if (null == s) throw (new Exception());
                return s;
            }
            catch
            {
                return String.Format("[?:{0}]", key);
            }
        }
    }
}
