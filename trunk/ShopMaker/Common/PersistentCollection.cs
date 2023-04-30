using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace MakerShop.Common
{
    /// <summary>
    /// A generic collection of persistable objects
    /// </summary>
    /// <typeparam name="T">Persistable object class</typeparam>
    public class PersistentCollection<T> : SortableCollection<T> where T : IPersistable
    {
        /// <summary>
        /// Does the collection needs to be saved to the persistent storage?
        /// </summary>
        public bool IsDirty
        {
            get
            {
                bool dirty = false;
                int index = 0;
                while (index < this.Count && !dirty)
                {
                    dirty = this[index].IsDirty;
                    index += 1;
                }
                return dirty;
            }
        }

        /// <summary>
        /// Delete the object at given index
        /// </summary>
        /// <param name="index">The index from which to delete the object</param>
        public void DeleteAt(int index)
        {
            this[index].Delete();
            this.RemoveAt(index);
        }

        /// <summary>
        /// Delete all objects of this collection.
        /// </summary>
        public void DeleteAll()
        {
            while (this.Count > 0)
            {
                this[0].Delete();
                this.RemoveAt(0);
            }
        }

        /// <summary>
        /// Save all objects of this collection to persistent storage.
        /// </summary>
        /// <returns></returns>
        public virtual bool Save()
        {
            bool allSaved = true;
            int index = 0;
            while (index < this.Count)
            {
                allSaved = (allSaved && (this[index].Save() != SaveResult.Failed));
                index += 1;
            }
            return allSaved;
        }

    }
}
