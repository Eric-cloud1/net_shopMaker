using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace MakerShop.Common
{
    /// <summary>
    /// A Dictionary object in which values are IPersistable objects
    /// </summary>
    /// <typeparam name="String">The key of type string</typeparam>
    /// <typeparam name="T">The value of type IPersistable</typeparam>
    public class PersistentDictionary<String, T> : Dictionary<String, T> where T : IPersistable
    {

        /// <summary>
        /// Is any persistable object in this dictionary dirty? 
        /// </summary>
        public bool IsDirty
        {
            get
            {
                foreach (T item in this.Values)
                {
                    if (item.IsDirty) return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Save the persistable objects in this dictionary to database
        /// </summary>
        /// <returns><b>true</b> if all objects are saved successfuly, <b>false</b> otherwise</returns>
        public bool Save()
        {
            bool allSaved = true;
            foreach (T item in this.Values)
            {
                allSaved = (allSaved && (item.Save() != SaveResult.Failed));
            }
            return allSaved;
        }
    }
}
