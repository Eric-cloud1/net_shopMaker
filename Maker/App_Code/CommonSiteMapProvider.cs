using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;

using MakerShop.Common;
using MakerShop.Web.SiteMap;
using MakerShop.Catalog;
using MakerShop.Products;
using MakerShop.Stores;
using MakerShop.Utility;
using System.Web;

using System.IO.Compression;


public class CommonSiteMapProvider : ISiteMapProvider
{
        public bool CreateSiteMap(SiteMapOptions options, ref List<string> messages)
        {
			return DoSiteMap(options, false, ref messages);
        }

        public bool CreateAndCompressSiteMap(SiteMapOptions options, ref List<string> messages)
        {
            return DoSiteMap(options, true, ref messages);
        }

		private bool DoSiteMap(SiteMapOptions options, bool compress, ref List<string> messages)
		{
			string SiteMapFile = Path.Combine(options.SiteMapDataPath, options.SiteMapFileName);
			string SiteMapData;
			StreamWriter SiteMapWriter;

            //get the data items to generate SiteMap for
            List<SiteMapUrl> siteMapItems = GetSiteMapItems(options);

			int maxUrls = 50000;

			if(siteMapItems.Count > maxUrls) 
			{
				//multiple sitemap files needed. It is best to devide them in equally sized files
                int nFiles = (int)Math.Ceiling((double)siteMapItems.Count / maxUrls);
                int nItems = (int)Math.Ceiling((double)siteMapItems.Count / nFiles);
				int nLastFileItems = siteMapItems.Count - ((nFiles -1) * nItems);

				String fName, cfName;
				int startIndex;

				for(int i=0; i<nFiles; i++) 
				{
					startIndex = i * nItems;
					fName = Path.Combine(options.SiteMapDataPath,"SiteMap" + (i+1).ToString() + ".xml");
					cfName = Path.Combine(options.SiteMapDataPath,fName + ".gz");

					if(i==nFiles-1)
						//this is the last file
						SiteMapData = GenerateSiteMap(siteMapItems, startIndex, nLastFileItems);
					else
						SiteMapData = GenerateSiteMap(siteMapItems, startIndex, nItems);					
					
					SiteMapWriter = File.CreateText(fName);
					using (SiteMapWriter)
					{
						SiteMapWriter.Write(SiteMapData);
						SiteMapWriter.Flush();
						SiteMapWriter.Close();
					}

					if(compress)
					{
						CompressFile(fName, cfName);
					}
				}

				messages.Add(string.Format("{0} SiteMap Files Created.", nFiles));
				
				GenerateSiteMapIndexFile(options, nFiles, compress);

                messages.Add("SiteMap Index File 'SiteMapIndex.xml' Created.");
			}
            else
            {				
				if (File.Exists(SiteMapFile) && !options.OverwriteSiteMapFile)
				{
					messages.Add("SiteMap File Already Exists. You should either chose to overwrite the SiteMap file or provide a different name.");
					return false;
				}
				//generate the site map
				SiteMapData = GenerateSiteMap(siteMapItems,0, siteMapItems.Count);

				SiteMapWriter = File.CreateText(SiteMapFile);
				using (SiteMapWriter)
				{
					SiteMapWriter.Write(SiteMapData);
					SiteMapWriter.Flush();
					SiteMapWriter.Close();
				}
                				
				messages.Add(string.Format("SiteMap File '{0}' Created.", options.SiteMapFileName));

				if(compress) 
				{
					string compressedFile = Path.Combine(options.SiteMapDataPath, options.CompressedSiteMapFileName);
					if (File.Exists(compressedFile) && !options.OverwriteCompressedFile)
					{
						messages.Add(string.Format("Compressed SiteMap File '{0}' Already Exists. You should either chose to overwrite the compressed SiteMap file or provide a different name.", compressedFile));
						return false;
					}

					CompressFile(SiteMapFile, compressedFile);
					messages.Add(string.Format("Compressed SiteMap File '{0}' Created.", options.CompressedSiteMapFileName));
				}
			}
			
            return true;
		}

		private void CompressFile(string fileToCompress, string compressedFileName)
		{
			if (File.Exists(compressedFileName)) File.Delete(compressedFileName);
			// Compress text data into a GZIP file...
			FileStream inputStream = File.OpenRead(fileToCompress);
			FileStream fsOutputFile = new FileStream(compressedFileName, FileMode.OpenOrCreate, FileAccess.Write);            
			GZipStream gzZipStream = new GZipStream(fsOutputFile, CompressionMode.Compress);

			using (gzZipStream)
			{
				using (inputStream)
				{
					byte[] fdata = new byte[inputStream.Length];
					inputStream.Read(fdata, 0, fdata.Length);
					gzZipStream.Write(fdata, 0, fdata.Length);
					gzZipStream.Flush();
					inputStream.Close();
				}
				gzZipStream.Close();
			}
		}

        private List<SiteMapUrl> GetSiteMapItems(SiteMapOptions options)
        {
            List<SiteMapUrl> siteMapItems = new List<SiteMapUrl>();
            //first get the ASP.NET SiteMap nodes
            System.Web.SiteMapNode rootNode = System.Web.SiteMap.RootNode;

            //PopulateAspNetSiteMapUrls(siteMapItems, rootNode, options);

			PopulateCommonUrls(siteMapItems, options);

            if (options.IncludeCategories)
            {
                PopulateCategoryUrls(siteMapItems, options);
            }

            if (options.IncludeProducts)
            {
                PopulateProductUrls(siteMapItems, options);
            }

            if (options.IncludeWebpages)
            {
                PopulateWebpageUrls(siteMapItems, options);
            }
	    
            return siteMapItems;
        }


		private void PopulateCommonUrls(List<SiteMapUrl> siteMapItems, SiteMapOptions options) 
		{
			siteMapItems.Add(new SiteMapUrl(GetAbsoluteUrl("~/Default.aspx"), options.DefaultChangeFrequency, options.DefaultUrlPriority));
			siteMapItems.Add(new SiteMapUrl(GetAbsoluteUrl("~/ContactUs.aspx"), options.DefaultChangeFrequency, options.DefaultUrlPriority));
			siteMapItems.Add(new SiteMapUrl(GetAbsoluteUrl("~/AboutUs.aspx"), options.DefaultChangeFrequency, options.DefaultUrlPriority));
		}

        private void PopulateAspNetSiteMapUrls(List<SiteMapUrl> siteMapItems, System.Web.SiteMapNode siteMapNode, SiteMapOptions options)
        {
            if (siteMapNode == null) return;
            string nodeUrl = GetAbsoluteUrl(siteMapNode.Url);
            //todo set other properties : last modified, change frequency, priority
            siteMapItems.Add(new SiteMapUrl(nodeUrl, options.DefaultChangeFrequency, options.DefaultUrlPriority));
            if (siteMapNode.HasChildNodes)
            {
                for (int i = 0; i < siteMapNode.ChildNodes.Count; i++)
                {
                    PopulateAspNetSiteMapUrls(siteMapItems, siteMapNode.ChildNodes[i],options);
                }
            }
        }

        private void PopulateCategoryUrls(List<SiteMapUrl> siteMapItems, SiteMapOptions options)
        {
            Store store = Token.Instance.Store;
            if (store == null) return;
            string url;
            foreach (Category cat in store.Categories)
            {
                if (cat.Visibility == CatalogVisibility.Public)
                {
                    url = UrlGenerator.GetBrowseUrl(cat.CategoryId, CatalogNodeType.Category, cat.Name);
                    siteMapItems.Add(new SiteMapUrl(GetAbsoluteUrl(url), options.DefaultChangeFrequency, options.DefaultUrlPriority));
                }
            }
        }

        private void PopulateProductUrls(List<SiteMapUrl> siteMapItems, SiteMapOptions options)
        {
            Store store = Token.Instance.Store;
            if (store == null) return;
            string url;
            foreach (Product prod in store.Products)
            {
                if (prod.Visibility == CatalogVisibility.Public)
                {
                    url = UrlGenerator.GetBrowseUrl(prod.ProductId, CatalogNodeType.Product, prod.Name);
                    siteMapItems.Add(new SiteMapUrl(GetAbsoluteUrl(url), options.DefaultChangeFrequency, options.DefaultUrlPriority));
                }
            }
        }

        private void PopulateWebpageUrls(List<SiteMapUrl> siteMapItems, SiteMapOptions options)
        {
            Store store = Token.Instance.Store;
            if (store == null) return;
            string url;
            foreach (Webpage wp in store.Webpages)
            {
                if (wp.Visibility == CatalogVisibility.Public)
                {
                    url = UrlGenerator.GetBrowseUrl(wp.WebpageId, CatalogNodeType.Webpage, wp.Name);
                    siteMapItems.Add(new SiteMapUrl(GetAbsoluteUrl(url), options.DefaultChangeFrequency, options.DefaultUrlPriority));
                }
            }
        }

        protected string GenerateSiteMap(List<SiteMapUrl> siteMapItems, int startIndex, int count) 
        {
            urlset siteMap = new urlset();
            url url;

            List<url> urlList = new List<url>();
			int endIndex = startIndex + count;			
			SiteMapUrl urlItem;

			for(int i=startIndex; i<endIndex; i++) 
			{
				urlItem = siteMapItems[i];
                url = new url();
                url.loc = urlItem.Location;
                if (urlItem.LastModified != DateTime.MinValue)
                {
                    url.lastmod = urlItem.LastModified.ToString("yyyy-MM-ddTHH:mm:sszzzzzz");
                }
                if (urlItem.ChangeFrequencySpecified)
                {
                    url.changefreqSpecified = true;
                    url.changefreq = urlItem.ChangeFrequency;
                }
                else
                {
                    url.changefreqSpecified = false;
                }

                if (urlItem.Priority >= 0 && urlItem.Priority <= 1)
                {
                    url.priority = urlItem.Priority;
                    url.prioritySpecified = true;
                }
                else
                {
                    url.prioritySpecified = false;
                }

                urlList.Add(url);
			}

            siteMap.url = urlList.ToArray();
		    
		    return XmlUtility.Utf8BytesToString(XmlUtility.Serialize(siteMap));
	    } 

        protected void GenerateSiteMapIndexFile(SiteMapOptions options, int nFiles, bool compressed)
        {
			sitemapindex smIndex = new sitemapindex();
			sitemap smap;
            List<sitemap> smapList = new List<sitemap>();

            for (int i = 1; i <= nFiles; i++)
            {                
                smap = new sitemap();
                if(compressed)
                    smap.loc = GetAbsoluteUrl("~/" + "SiteMap" + i + ".xml.gz");
                else
                    smap.loc = GetAbsoluteUrl("~/" + "SiteMap" + i + ".xml");
                smap.lastmod = LocaleHelper.LocalNow.ToString("yyyy-MM-ddTHH:mm:sszzzzzz");
                smapList.Add(smap);
            }

            smIndex.sitemap = smapList.ToArray();

            string fileData = XmlUtility.Utf8BytesToString(XmlUtility.Serialize(smIndex));
            string fileName = Path.Combine(options.SiteMapDataPath, "SiteMapIndex.xml");
            
            StreamWriter SiteMapIndexWriter = File.CreateText(fileName);
            using (SiteMapIndexWriter)
            {
                SiteMapIndexWriter.Write(fileData);
                SiteMapIndexWriter.Flush();
                SiteMapIndexWriter.Close();
            }
	    }


    private static string GetAbsoluteUrl(string url)
    {
        string newUrl = FixupUrl(url);
        
        //We should be getting protocol and port from store url.
        //Protocol should always be HTTP (see bug 6464)
        Store store = Token.Instance.Store;
        string Protocol = "http://";
        string Port = "";
        string host = HttpContext.Current.Request.ServerVariables["SERVER_NAME"];

        if (store != null)
        {
            UriBuilder uriBuilder = new UriBuilder(store.StoreUrl);
            if (uriBuilder.Port != 80 && uriBuilder.Port != 443)
            {
                Port = uriBuilder.Port.ToString();
            }
            host = uriBuilder.Host;
        }

        string basePath = Protocol + host + Port;

        return basePath + newUrl.ToString();
    }

    private static string FixupUrl(string Url)
    {
        if (Url == null) return "";

        string returnUrl = Url;

        if (Url.StartsWith("~"))
        {
            returnUrl = (HttpContext.Current.Request.ApplicationPath + Url.Substring(1)).Replace("//", "/");
        }

        return returnUrl;
    }

    private static bool IsRelativeUrl(string virtualPath)
    {
        if (virtualPath.IndexOf(":") != -1)
        {
            return false;
        }
        return !IsRooted(virtualPath);
    }

    private static bool IsRooted(string basepath)
    {
        if (!string.IsNullOrEmpty(basepath) && (basepath[0] != '/'))
        {
            return (basepath[0] == '\\');
        }
        return true;
    }

}
