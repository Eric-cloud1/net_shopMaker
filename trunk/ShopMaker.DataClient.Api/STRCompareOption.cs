using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.DataClient.Api
{
    public enum StrCompareOption:byte
    {
        Matches,        //exact match
        ContainsString, //string is contained in the other
        ContainsWords   //words of the string are contained in the other
    }
}
