using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.Common
{
    /// <summary>
    /// Objects that are presistable to database implement this interface
    /// </summary>
    public interface IPersistable
    {
        /// <summary>
        /// Delete this persistable object from persistent storage
        /// </summary>
        /// <returns></returns>
        bool Delete();

        /// <summary>
        /// Indicates whether this persistable object has changed in the memory 
        /// and needs to be saved to the persistent storage or not
        /// </summary>
        bool IsDirty
        {
            get; set;
        }

        /// <summary>
        /// Save this persistable object to persistent storage
        /// </summary>
        /// <returns></returns>
        SaveResult Save();
    }
}
