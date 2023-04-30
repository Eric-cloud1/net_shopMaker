using System;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;

using MakerShop.Common;
using MakerShop.Data;

namespace MakerShop.Validation
{
    
    public partial class WhiteLists
    {
        public WhiteListTypes WhiteListType
        {
            set
            {
                this.WhiteListTypeId = (short)value;
            }
            get
            {
                try
                {
                    return (WhiteListTypes)this.WhiteListTypeId;
                }
                catch
                {
                    return 0;
                }
            }
        }
      
    }
}