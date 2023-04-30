namespace MakerShop.DigitalDelivery
{
    public partial class ProductDigitalGood
    {
        public Payments.PaymentTypes PaymentType
        {
            get
            {
                return (MakerShop.Payments.PaymentTypes)this.PaymentTypeId;
            }
            set
            {
                this.PaymentTypeId = (short)value;
            }
        }

        public Payments.PaymentStatus PaymentStatus
        {
            get
            {
                return (MakerShop.Payments.PaymentStatus)this.PaymentStatusId;
            }
            set
            {
                this.PaymentStatusId = (short)value;
            }
        }

        /// Loads a ProductDigitalGood object for given key from the database.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="digitalGoodId"></param>
        /// <returns><b>true</b> if load is successful, <b>false</b> otherwise</returns>
        public virtual bool Load(int productId, int digitalGoodId)
        {
            bool result = false;
            this.ProductId = productId;
            this.DigitalGoodId = digitalGoodId;

            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            System.Text.StringBuilder selectQuery = new System.Text.StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_ProductDigitalGoods");
            selectQuery.Append(" WHERE ProductId = @productId");
            selectQuery.Append(" AND DigitalGoodId = @digitalGoodId");
            MakerShop.Data.Database database = MakerShop.Common.Token.Instance.Database;
            System.Data.Common.DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());
            database.AddInParameter(selectCommand, "@productId", System.Data.DbType.Int32, productId);
            database.AddInParameter(selectCommand, "@digitalGoodId", System.Data.DbType.Int32, digitalGoodId);
            //EXECUTE THE COMMAND
            using (System.Data.IDataReader dr = database.ExecuteReader(selectCommand))
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
    }
}
