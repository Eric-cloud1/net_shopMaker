using System;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;

using MakerShop.Common;
using MakerShop.Data;

namespace MakerShop.Validation
{
    
    public partial class BlackLists
    {
        public BlackListTypes BlackListType
        {
            set
            {
                this.BlackListTypeId = (short)value;
            }
            get
            {
                try
                {
                    return (BlackListTypes)this.BlackListTypeId;
                }
                catch
                {
                    return 0;
                }
            }
        }
      
    }
}