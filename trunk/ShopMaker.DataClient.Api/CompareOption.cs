using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.DataClient.Api
{
    public enum CompareOption:byte
    {
        Equal,
        NotEqual,
        GreatorThan,
        LessThan,
        GreatorThanEqualTo,
        LessThanEqualTo,
        Between
    }
}
