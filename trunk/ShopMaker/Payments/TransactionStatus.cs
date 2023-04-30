using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.Payments
{
    /// <summary>
    /// Enumeration that represents the status of a transaction
    /// </summary>
    public enum TransactionStatus : int 
    {
        /// <summary>
        /// Transaction has failed
        /// </summary>
        Failed = 0,

        /// <summary>
        /// Transaction is successful
        /// </summary>
        Successful,

        /// <summary>
        /// Transaction is pending
        /// </summary>
        Pending
    }
}
