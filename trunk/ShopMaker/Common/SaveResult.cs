using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.Common
{
    /// <summary>
    /// Enumeration that represents the result of a database save operation
    /// </summary>
    public enum SaveResult : int
    {
        /// <summary>
        /// The object record to be saved was not dirty. 
        /// </summary>
        NotDirty, 
        
        /// <summary>
        /// Record was updated successfuly.
        /// </summary>
        RecordUpdated, 
        
        /// <summary>
        /// A new record was inserted in the database.
        /// </summary>
        RecordInserted, 
        
        /// <summary>
        /// Record was deleted.
        /// </summary>
        RecordDeleted, 
        
        /// <summary>
        /// The operation failed due to some error.
        /// </summary>
        Failed
    }
}
