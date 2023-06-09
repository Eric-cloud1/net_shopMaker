using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Permissions;
using System.Web.Caching;

namespace MakerShop.Utility
{
    /// <summary>
    /// Class for misc utility functions
    /// </summary>
    public static class Misc
    {
        /// <summary>
        /// Returns the class ID of a type without version or culture specification
        /// </summary>
        /// <param name="type">The type to return class ID for</param>
        /// <returns>The class ID without version or culture specification</returns>
        public static string GetClassId(Type type)
        {
            string classId = type.AssemblyQualifiedName;
            int index = classId.IndexOf(", Version");
            if (index >= 0)
            {
                classId = classId.Substring(0, index);
            }
            return classId;
        }

        /// <summary>
        /// Determines whether the process has permission to run unmanaged code
        /// </summary>
        /// <returns>True if permission to run unmanaged code exists, false otherwise.</returns>
        public static bool HasUnmanagedCodePermission()
        {
            try
            {

                SecurityPermission sp = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
                sp.Demand();
            }
            catch (System.Security.SecurityException)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Attempts to determine the current process identity.
        /// </summary>
        /// <returns>The current process identity.</returns>
        public static string GetProcessIdentity()
        {
            string processIdentity;
            try
            {
                processIdentity = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            }
            catch
            {
                processIdentity = "Unable to determine. Generally NETWORK SERVICE (Windows 2003/XP) or ASPNET (Windows 2000)";
            }
            return processIdentity;
        }

        /// <summary>
        /// Performs an automated lookup at whatismyip.com to determine the external IP of the calling server
        /// </summary>
        /// <returns>A string with the external IP of the calling server</returns>
        public static string WhatIsMyIP()
        {
            // LOOK FOR CACHED IP INFORMATION
            Cache cache = HttpContextHelper.SafeGetCache();
            if (cache != null && cache["WhatIsMyIP"] != null) return (string)cache["WhatIsMyIP"];

            // TRY TO GET IP FROM WEBSERVICE
            string response = string.Empty;
            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.whatismyip.com/automation/n09230945.asp");
                httpWebRequest.Method = "GET";
                using (StreamReader responseStream = new StreamReader(httpWebRequest.GetResponse().GetResponseStream(), System.Text.Encoding.UTF8))
                {
                    response = responseStream.ReadToEnd();
                    responseStream.Close();
                }
                if (!Regex.IsMatch(response, @"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$")) response = string.Empty;
            }
            catch { }

            // CACHE AND RETURN RESPONSE
            if (cache != null) cache.Add("WhatIsMyIP", response, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 10, 0), CacheItemPriority.Normal, null);
            return response;
        }
    }
}