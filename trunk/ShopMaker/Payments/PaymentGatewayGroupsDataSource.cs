using System;
using System.ComponentModel;
using MakerShop.Common;
using MakerShop.Data;
using System.Data;
using System.Data.Common;
using MakerShop.Utility;

namespace MakerShop.Payments
{
   

    /// <summary>
    /// DataSource class for PaymentGateway objects.
    /// </summary>
    [DataObject(true)]
    public partial class PaymentGatewayGroupsDataSource
    {

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PaymentGatewayGroupsCollection Load()
        {
            return LoadForCriteria("", 0, 0, string.Empty);
        }

     


    }
}
