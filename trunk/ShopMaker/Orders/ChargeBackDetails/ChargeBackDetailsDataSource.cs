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

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ChargeBackDetailsCollection LoadDetailsByTransactionId(int TransactionId)
        {
            return LoadForCriteria("TransactionId = " + TransactionId);
        }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static ChargeBackDetails LoadByTransactionId(int TransactionId)
        {
            ChargeBackDetailsCollection results = ChargeBackDetailsDataSource.LoadDetailsByTransactionId(TransactionId);

       
            if(results.Count == 1)
                return (ChargeBackDetails)results[0];

            return new ChargeBackDetails();
        }
    }
   
}
