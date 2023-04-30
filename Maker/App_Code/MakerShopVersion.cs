using System;
using System.IO;
using System.Web;
using System.Web.Caching;
using System.Xml;
using MakerShop.Utility;

/// <summary>
/// Singleton class that holds data regarding the version of MakerShop
/// </summary>
public class MakerShopVersion
{
    private static object syncLock = new object();
    private string _Version;
    private string _Build;
    public string Version { get { return _Version; } }
    public string Build { get { return _Build; } }
    private MakerShopVersion(string version, string build)
    {
        _Version = version;
        _Build = build;
    }
    public static MakerShopVersion Instance
    {
        get
        {
            HttpContext context = HttpContext.Current;
            MakerShopVersion tempInstance = context.Cache["MakerShopVersion"] as MakerShopVersion;
            if (tempInstance != null) return tempInstance;
            //MUST INITIALIZE NEW INSTANCE
            string version, build;
            string versionFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/VersionInfo.xml");
            try
            {
                XmlDocument versionInfo = new XmlDocument();
                versionInfo.Load(versionFile);
                version = XmlUtility.GetElementValue(versionInfo.DocumentElement, "Version");
                build = XmlUtility.GetElementValue(versionInfo.DocumentElement, "BuildNumber");
            }
            catch
            {
                version = "7.0.4";
                build = "unknown";
            }
            tempInstance = new MakerShopVersion(version, build);
            CacheDependency fileDep = new CacheDependency(versionFile);
            lock (syncLock)
            {
                context.Cache.Remove("MakerShopVersion");
                context.Cache.Add("MakerShopVersion", tempInstance, fileDep, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);
            }
            return tempInstance;
        }
    }
}