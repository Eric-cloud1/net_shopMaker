using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.DataFeeds
{
    /// <summary>
    /// Feed Option Keys for Shopping.com feed.
    /// </summary>
    public class ShoppingComOptionKeys : IFeedOptionKeys
    {
        /// <summary>
        /// Name of the feed file
        /// </summary>
        public string FeedFileName { get{ return "ShoppingCom_FeedFileName";}}
        
        /// <summary>
        /// Whether to overwrite any existing feed file?
        /// </summary>
        public string OverwriteFeedFile { get{ return "ShoppingCom_OverwriteFeedFile";}}

        /// <summary>
        /// Whether to Include all products in feed or exclude the ones marked for feed exclusion?
        /// </summary>
        public string IncludeAllProducts { get { return "ShoppingCom_IncludeAllProducts"; } }

        /// <summary>
        /// File name for the compressed feed file
        /// </summary>
        public string CompressedFeedFileName { get { return "ShoppingCom_CompressedFeedFileName"; } }

        /// <summary>
        /// Whether to overwrite any existing compressed feed file?
        /// </summary>
        public string OverwriteCompressedFile { get { return "ShoppingCom_OverwriteCompressedFile"; } }

        /// <summary>
        /// The FTP host to upload the feed to
        /// </summary>
        public string FtpHost { get { return "ShoppingCom_FtpHost"; } }

        /// <summary>
        /// The FTP user name
        /// </summary>
        public string FtpUser { get { return "ShoppingCom_FtpUser"; } }

        /// <summary>
        /// The FTP password
        /// </summary>
        public string FtpPassword { get { return "ShoppingCom_FtpPassword"; } }

        /// <summary>
        /// Remote file name to use on the ftp server
        /// </summary>
        public string RemoteFileName { get { return "ShoppingCom_RemoteFileName"; } }
    }
}
