using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.DataClient.Api
{
    /// <summary>
    /// This class will be a data field only
    /// </summary>
    public class DataField : DataObjectField
    {       
        Object defaultValue;

        public Object DefaultValue
        {
            get { return defaultValue; }
            set { defaultValue = value; }
        }
    }
}
