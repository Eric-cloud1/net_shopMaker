using System;
using System.Collections.Generic;
using System.Text;
using MakerShop.Common;

namespace MakerShop.Web.ConLib
{
    [Serializable]
    public class ConLibControlCollection : SortableCollection<ConLibControl>
    {
        public ConLibControl TryGetValue(string name)
        {
            foreach (ConLibControl control in this)
                if (control.Name == name) return control;
            return null;
        }
    }
}
