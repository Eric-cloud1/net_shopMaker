using System;
using System.Text;

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
    public partial class PaymentGatewayAllocationDataSource
    {
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PaymentGatewayAllocationCollection Load(Int32 pPaymentGatewayTemplateId)
        {
            return PaymentGatewayAllocationDataSource.LoadForCriteria("PaymentGatewayTemplateId = " + pPaymentGatewayTemplateId.ToString());

        }

      

    
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PaymentGatewayAllocationCollection Load(Int32 pPaymentGatewayTemplateId, PaymentInstrument paymentInstrument)
        {
            //CREATE SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            selectQuery.Append(" " + PaymentGatewayAllocation.GetColumnNames("pga"));
            selectQuery.Append(" FROM xm_PaymentGatewayAllocations pga join ac_PaymentMethods pm on pga.PaymentMethodId = pm.PaymentMethodId ");
            selectQuery.Append(" WHERE pm.PaymentInstrumentId = " + ((short)paymentInstrument).ToString());
            selectQuery.Append(" AND pga.PaymentGatewayTemplateId = " + pPaymentGatewayTemplateId.ToString());
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            //EXECUTE THE COMMAND
            PaymentGatewayAllocationCollection results = new PaymentGatewayAllocationCollection();
            int thisIndex = 0;
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {
                    PaymentGatewayAllocation varPaymentGatewayAllocations = new PaymentGatewayAllocation();
                    PaymentGatewayAllocation.LoadDataReader(varPaymentGatewayAllocations, dr);
                    results.Add(varPaymentGatewayAllocations);
                    rowCount++;
                    thisIndex++;
                }
                dr.Close();
            }
            return results;
        }
    }
}
