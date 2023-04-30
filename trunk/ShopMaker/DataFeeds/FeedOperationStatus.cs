using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.DataFeeds
{
    /// <summary>
    /// Used to keep track the status of feed creation, uploading operations
    /// </summary>
    [Serializable]
    public class FeedOperationStatus
    {

        List<String> messages = new List<String>();
        string statusMessage = ""; 
        int percent = 0;
        bool success = false;


        /// <summary>
        /// Represents if the operation is successfull or not
        /// </summary>
        public bool Success
        {
            get { return success; }
            set { success = value; }
        }
        

        /// <summary>
        /// Current status message
        /// </summary>
        public string StatusMessage
        {
            get { return statusMessage; }
            set { statusMessage = value; }
        }

        /// <summary>
        /// Indicates the precent completion of operation
        /// </summary>
        public int Percent
        {
            get { return percent; }
            set { percent = value; }
        }

        /// <summary>
        /// List of messages
        /// </summary>
        public List<string> Messages
        {
            get { return messages; }
            set { messages = value; }
        }

        enum Status:byte
        {
            NotStarted,
            InProgress,
            Completed
        }
    }
}
