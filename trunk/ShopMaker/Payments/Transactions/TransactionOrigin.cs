using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.Payments
{
    /// <summary>
    /// Enumeration that represents the origin of a transaction
    /// </summary>
    public enum TransactionOrigin
    {
        /// <summary>
        /// Transaction origin is internet/web
        /// </summary>
        Internet, 
        
        /// <summary>
        /// Transaction origin is mail
        /// </summary>
        Mail, 
        
        /// <summary>
        /// Transaction origin is telephone
        /// </summary>
        Telephone, 
        
        /// <summary>
        /// Transaction origin is retail/pos
        /// </summary>
        Retail
    }
}
