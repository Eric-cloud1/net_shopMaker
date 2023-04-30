using System;
using System.ComponentModel;
using MakerShop.Common;
using MakerShop.Data;
using System.Data;
using System.Data.Common;
using MakerShop.Utility;
using System.Text;

namespace MakerShop.Payments
{
    /// <summary>
    /// DataSource class for PaymentGateway objects.
    /// </summary>
    [DataObject(true)]
    public partial class PaymentGatewayDataSource
    {
        /// <summary>
        /// Given the class Id of a payment gateway implementation, gets the corresponding object Id in database.
        /// </summary>
        /// <param name="classId">Class Id of a payment gateway implementation</param>
        /// <returns>The corresponding object Id in database</returns>
        public static int GetPaymentGatewayIdByClassId(string classId)
        {
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand("SET NOCOUNT ON; SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT PaymentGatewayId FROM ac_PaymentGateways WHERE StoreId=@storeId AND ClassId=@classId");
            database.AddInParameter(selectCommand, "@storeId", DbType.Int32, Token.Instance.StoreId);
            database.AddInParameter(selectCommand, "@classId", DbType.String, classId);
            return AlwaysConvert.ToInt(database.ExecuteScalar(selectCommand));
        }

        public static DateTime GatewayBatchProcess_LastDate(string Gateway)
        {
            Database database = Token.Instance.Database;
            using (System.Data.Common.DbCommand cmd = database.GetStoredProcCommand("wsp_DigitalGoods_PendingCancel"))
            {
                database.AddInParameter(cmd, "@Gateway", DbType.String, Gateway);
                database.AddOutParameter(cmd, "@Date", DbType.Date, 4);

                cmd.ExecuteNonQuery();
                return DateTime.Parse(cmd.Parameters["@Date"].Value.ToString());
            }
        }


        /// <summary>
        /// Loads a collection of PaymentGateway objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <returns>A collection of PaymentGateway objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PaymentGatewayCollection LoadAllGateways()
        {

            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();

        
           selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT PaymentGatewayId, Name  FROM ac_PaymentGateways ");

       
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());

            //EXECUTE THE COMMAND
            PaymentGatewayCollection results = new PaymentGatewayCollection();
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {

                    PaymentGateway paymentGateway = new PaymentGateway();
                    paymentGateway.PaymentGatewayId = dr.GetInt32(0);
                    paymentGateway.Name = dr.GetString(1);

                    results.Add(paymentGateway);
                    rowCount++;

                }
                dr.Close();
            }
            return results;

        }



        /// <summary>
        /// Loads a collection of PaymentGateway objects from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <returns>A collection of PaymentGateway objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PaymentGatewayCollection LoadAllGateways(bool group)
        {
    
            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();

            if (group == false)
            {
                selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT PaymentGatewayId, Name  FROM ac_PaymentGateways ");
                selectQuery.Append(" WHERE PaymentGatewayId NOT IN(SELECT PaymentGatewayId FROM xm_PaymentGatewayGroupsPaymentGateways)");
                selectQuery.Append(" AND isActive =1");
            }

            else
            {
                selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT PaymentGatewayId, Name  ");
                selectQuery.Append(" FROM ac_PaymentGateways WHERE PaymentGatewayId IN(SELECT PaymentGatewayId FROM xm_PaymentGatewayGroupsPaymentGateways) ");
                selectQuery.Append(" AND isActive =1");

            }

            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());

            //EXECUTE THE COMMAND
            PaymentGatewayCollection results = new PaymentGatewayCollection();
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read() )
                {
                    
                        PaymentGateway paymentGateway = new PaymentGateway();
                        paymentGateway.PaymentGatewayId = dr.GetInt32(0);
                        paymentGateway.Name = dr.GetString(1);
              
                        results.Add(paymentGateway);
                        rowCount++;
                 
                }
                dr.Close();
            }
            return results;
    
        }

        /// <summary>
        /// Loads a collection of PaymentGateway objects by GroupId from the database based on given SQL criteria
        /// </summary>
        /// <param name="sqlCriteria">The SQL criteria string that determines the objects that should be loaded.</param>
        /// <returns>A collection of PaymentGateway objects</returns>
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static PaymentGatewayCollection LoadAllGateways(Int32 paymentGatewayGroupId)
        {


            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT pg.PaymentGatewayId,Name FROM ac_PaymentGateways pg ");
            selectQuery.Append(" JOIN xm_PaymentGatewayGroupsPaymentGateways pggp ON pg.PaymentGatewayId = pggp.PaymentGatewayId");
            selectQuery.Append(" WHERE pggp.PaymentGatewayGroupId= @PaymentGatewayGroupId");

            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@PaymentGatewayGroupId", System.Data.DbType.Int32, paymentGatewayGroupId);

            //EXECUTE THE COMMAND
            PaymentGatewayCollection results = new PaymentGatewayCollection();
            int rowCount = 0;
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                while (dr.Read())
                {

                    PaymentGateway paymentGateway = new PaymentGateway();
                    paymentGateway.PaymentGatewayId = dr.GetInt32(0);
                    paymentGateway.Name = dr.GetString(1);

                    results.Add(paymentGateway);
                    rowCount++;

                }
                dr.Close();
            }
            return results;

        }

  


    }
}
