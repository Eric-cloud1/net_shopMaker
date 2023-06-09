using System.Text;
using System.Data.Common;
using System.Data;

using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Users;

namespace MakerShop.Payments
{
    public partial class PaymentGatewayFailover
    {
        private PaymentGateway _SourcePaymentGateway = null;
        public PaymentGateway SourcePaymentGateway
        {
            get
            {
                if (_SourcePaymentGateway == null)
                    _SourcePaymentGateway = PaymentGatewayDataSource.Load(this.SourcePaymentGatewayId);
                return _SourcePaymentGateway;
            }
        }
        private PaymentGateway _DestinationPaymentGateway = null;
        public PaymentGateway DestinationPaymentGateway
        {
            get
            {
                if (_DestinationPaymentGateway == null)
                    _DestinationPaymentGateway= PaymentGatewayDataSource.Load(this.DestinationPaymentGatewayId);
                return _DestinationPaymentGateway;
            }
        }
        public virtual bool Load(int pPaymentGatewayFailoverId)
        {
            bool result = false;
            this.PaymentGatewayFailoverId = pPaymentGatewayFailoverId;

            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SET NOCOUNT ON; ");
            selectQuery.Append("SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM xm_PaymentGatewayFailover (NOLOCK) ");
            selectQuery.Append(" WHERE  PaymentGatewayFailoverId = @PaymentGatewayFailoverId ");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());


            database.AddInParameter(selectCommand, "@PaymentGatewayFailoverId", System.Data.DbType.Int32, this.PaymentGatewayFailoverId);

            //EXECUTE THE COMMAND
            using (IDataReader dr = database.ExecuteReader(selectCommand))
            {
                if (dr.Read())
                {
                    result = true;
                    LoadDataReader(this, dr); ;
                }
                dr.Close();
            }
            return result;
        }
        public string Name
        {
            get
            {
                string s = SourcePaymentGateway.Name;
                if (TwoWay)
                    s += " ⇔ ";
                else
                    s += " ⇒ ";
                s += SourcePaymentGateway.Name;

                return s;
            }
        }
    }
}