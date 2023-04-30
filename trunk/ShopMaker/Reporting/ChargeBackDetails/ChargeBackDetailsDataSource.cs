using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Utility;

namespace MakerShop.Orders.ChargeBack
{
    /// <summary>
    /// DataSource class for ErrorMessage objects
    /// </summary>
    [DataObject(true)]
    public partial class ChargeBackDetailsDataSource
    {

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ChargeBackDetailsCollection LoadByChargeBackDate(DateTime chargeBackDate)
        {
            return LoadForCriteria("InitiateDate = '" + chargeBackDate+"'" );
        }
    
    }
   
}
