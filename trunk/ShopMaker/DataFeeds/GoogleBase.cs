using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MakerShop.Common;
using MakerShop.Catalog;
using MakerShop.Products;
using MakerShop.Utility;


namespace MakerShop.DataFeeds
{
    /// <summary>
    /// Feed Provider implementation for GoogleBase
    /// </summary>
    public class GoogleBase : FeedProviderBase
    {
        //private static GoogleBase _Instance = null;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public GoogleBase()
        {
            this.FeedOperationStatus = new FeedOperationStatus();
        }

        ///// <summary>
        ///// Singleton Instance of the GoogleBase feed provider implementation
        ///// </summary>
        //public static GoogleBase Instance { 
        //    get{
        //        if (_Instance == null)
        //        {
        //            _Instance = new GoogleBase();
        //        }

        //        if (_Instance.FeedOperationStatus == null)
        //        {
        //            _Instance.FeedOperationStatus = new FeedOperationStatus();
        //        }
        //        return _Instance;
        //    }
        //}
        
        /// <summary>
        /// Returns the header row for the feed
        /// </summary>
        /// <returns>The header row.</returns>
        protected override string GetHeaderRow()
        {
            StringBuilder feedLine = new StringBuilder();
            feedLine.Append("description\t");
            feedLine.Append("id\t");
            feedLine.Append("link\t");
            feedLine.Append("price\t");
            feedLine.Append("title\t");

            //Recommended
            //	brand, condition, image_link, isbn, mpn, upc 
            feedLine.Append("brand\t");
            feedLine.Append("condition\t");
            feedLine.Append("image_link\t");
            //feedLine.Append("isbn\t");
            feedLine.Append("mpn\t");
            feedLine.Append("upc\t");
            feedLine.Append("product_type");

            //Optional
            //	color, expiration_date, height,length,model_number,payment_accepted,payment_notes 
            //	price_type,product_type,quantity,size,weight,width,year

            feedLine.Append("\r\n");
            return feedLine.ToString();
        }


        /// <summary>
        /// Generates the feed data for given products in GoogleBase feed format
        /// </summary>
        /// <param name="products">The products to generate feed for</param>
        /// <returns>The feed data for given products in GoogleBase feed format</returns>
        protected override string GetFeedData(ProductCollection products)
        {
            StringWriter feedWriter = new StringWriter();
            StringBuilder feedLine;

            string storeUrl = Token.Instance.Store.StoreUrl;
            storeUrl = storeUrl + (storeUrl.EndsWith("/") ? "" : "/");

            string url, name, desc, price, imgurl, id, brand, condition, upc, mpn;
            foreach (Product product in products)
            {
                url = product.NavigateUrl;
                if (url.StartsWith("~/"))
                {
                    url = url.Substring(2);
                }
                url = storeUrl + url;

                name = StringHelper.CleanupSpecialChars(product.Name);
                desc = StringHelper.CleanupSpecialChars(product.Summary);
                
                price = string.Format("{0:F2}", product.Price);

                imgurl = product.ImageUrl;
                if (!string.IsNullOrEmpty(imgurl) && !IsAbsoluteURL(imgurl))
                {
                    if (imgurl.StartsWith("~/"))
                    {
                        imgurl = imgurl.Substring(2);
                    }
                    imgurl = storeUrl + imgurl;
                }
                
                id = product.ProductId.ToString();
                brand = product.Manufacturer != null ? product.Manufacturer.Name : "";
                condition = "new";
                mpn = product.ModelNumber;
                upc = this.IsUpcCode(product.Sku) ? product.Sku : string.Empty;

                feedLine = new StringBuilder();
                feedLine.Append(desc + "\t" + id + "\t" + url + "\t" + price + "\t" + name + "\t");
                feedLine.Append(brand + "\t" + condition + "\t" + imgurl + "\t" + mpn + "\t" + upc + "\t");
                feedLine.Append(GetProductType(product));

                feedWriter.WriteLine(feedLine.ToString());
            }

            string returnData = feedWriter.ToString();
            feedWriter.Close();

            return returnData;
        }

        private string GetProductType(Product product)
        {
            if (product.Categories.Count > 0)
            {
                List<CatalogPathNode> pathNodes = CatalogDataSource.GetPath(product.Categories[0], false);
                List<string> categoryNames = new List<string>();
                foreach (CatalogPathNode node in pathNodes)
                {
                    categoryNames.Add(node.Name);
                }
                return string.Join(" > ", categoryNames.ToArray());
            }
            return string.Empty;
        }

    }
}
