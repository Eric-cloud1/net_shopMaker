
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Utility;

namespace MakerShop.Orders
{
    public partial class Order_Ex : IPersistable
    {

        #region Constructors

        public Order_Ex() { }

        public Order_Ex(Int32 pOrderId)
        {
            this.OrderId = pOrderId;

        }

        public static string GetColumnNames(string prefix)
        {
            if (string.IsNullOrEmpty(prefix)) prefix = string.Empty;
            else prefix = prefix + ".";
            List<string> columnNames = new List<string>();
            columnNames.Add(prefix + "OrderId");
            columnNames.Add(prefix + "CellPhone");
            columnNames.Add(prefix + "HomePhone");
            columnNames.Add(prefix + "DOB");
            columnNames.Add(prefix + "PeopleInHousehold");
            columnNames.Add(prefix + "AnnualIncome");
            columnNames.Add(prefix + "MonthlyIncome");
            columnNames.Add(prefix + "HouseholdAnnualIncome");
            columnNames.Add(prefix + "HouseholdMonthlyIncome");
            columnNames.Add(prefix + "PeopleEmployed");
            columnNames.Add(prefix + "Children");
            columnNames.Add(prefix + "GovernmentProgramId");
            columnNames.Add(prefix + "Last4SSN");
            columnNames.Add(prefix + "BillToMiddleName");
            columnNames.Add(prefix + "Signature");
            columnNames.Add(prefix + "SignatureTime");


            return string.Join(",", columnNames.ToArray());
        }

        public static void LoadDataReader(Order_Ex pOrder_Ex, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA

            pOrder_Ex.OrderId = dr.GetInt32(0);


            pOrder_Ex.CellPhone = NullableData.GetString(dr, 1);

            pOrder_Ex.HomePhone = NullableData.GetString(dr, 2);

            pOrder_Ex.DOB = NullableData.GetDateTime(dr, 3);

            pOrder_Ex.PeopleInHousehold = NullableData.GetByte(dr, 4);

            pOrder_Ex.AnnualIncome = NullableData.GetDecimal(dr, 5);

            pOrder_Ex.MonthlyIncome = NullableData.GetDecimal(dr, 6);

            pOrder_Ex.HouseholdAnnualIncome = NullableData.GetDecimal(dr, 7);

            pOrder_Ex.HouseholdMonthlyIncome = NullableData.GetDecimal(dr, 8);


            pOrder_Ex.PeopleEmployed = NullableData.GetByte(dr, 9);

            pOrder_Ex.Children = NullableData.GetByte(dr, 10);


            pOrder_Ex.GovernmentProgramId = NullableData.GetInt16(dr, 11);


            pOrder_Ex.Last4SSN = NullableData.GetString(dr, 12);

            pOrder_Ex.BillToMiddleName = NullableData.GetString(dr, 13);
            pOrder_Ex.Signature = NullableData.GetString(dr, 14);
            pOrder_Ex.SignatureTime= NullableData.GetDateTime(dr, 15);
            if (pOrder_Ex.SignatureTime.HasValue)
                pOrder_Ex.SignatureTime = LocaleHelper.ToLocalTime(pOrder_Ex.SignatureTime.Value);
            

            pOrder_Ex.IsDirty = false;
        }

        #endregion

        #region Class Data

        private bool _IsDirty;

        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }


        private Int32 _OrderId;

        [DataObjectField(true, false, false)]
        public Int32 OrderId
        {
            get { return this._OrderId; }
            set
            {
                if (this._OrderId != value)
                {
                    this._OrderId = value;
                    this.IsDirty = true;
                }
            }
        }


        private String _CellPhone;

        [DataObjectField(false, false, true)]
        public String CellPhone
        {
            get { return this._CellPhone; }
            set
            {
                if (this._CellPhone != value)
                {
                    this._CellPhone = value;
                    this.IsDirty = true;
                }
            }
        }


        private String _HomePhone;

        [DataObjectField(false, false, true)]
        public String HomePhone
        {
            get { return this._HomePhone; }
            set
            {
                if (this._HomePhone != value)
                {
                    this._HomePhone = value;
                    this.IsDirty = true;
                }
            }
        }


        private DateTime? _DOB;
        [DataObjectField(false, false, true)]
        public DateTime? DOB
        {
            get { return this._DOB; }
            set
            {
                if (this._DOB != value)
                {
                    this._DOB = value;
                    this.IsDirty = true;
                }
            }
        }



        private Byte? _PeopleInHousehold;
        [DataObjectField(false, false, true)]
        public Byte? PeopleInHousehold
        {
            get { return this._PeopleInHousehold; }
            set
            {
                if (this._PeopleInHousehold != value)
                {
                    this._PeopleInHousehold = value;
                    this.IsDirty = true;
                }
            }
        }


        private Decimal? _AnnualIncome;
        [DataObjectField(false, false, true)]
        public Decimal? AnnualIncome
        {
            get { return this._AnnualIncome; }
            set
            {
                if (this._AnnualIncome != value)
                {
                    this._AnnualIncome = value;
                    this.IsDirty = true;
                }
            }
        }


        private Decimal? _MonthlyIncome;
        [DataObjectField(false, false, true)]
        public Decimal? MonthlyIncome
        {
            get { return this._MonthlyIncome; }
            set
            {
                if (this._MonthlyIncome != value)
                {
                    this._MonthlyIncome = value;
                    this.IsDirty = true;
                }
            }
        }


        private Decimal? _HouseholdAnnualIncome;
        [DataObjectField(false, false, true)]
        public Decimal? HouseholdAnnualIncome
        {
            get { return this._HouseholdAnnualIncome; }
            set
            {
                if (this._HouseholdAnnualIncome != value)
                {
                    this._HouseholdAnnualIncome = value;
                    this.IsDirty = true;
                }
            }
        }


        private Decimal? _HouseholdMonthlyIncome;
        [DataObjectField(false, false, true)]
        public Decimal? HouseholdMonthlyIncome
        {
            get { return this._HouseholdMonthlyIncome; }
            set
            {
                if (this._HouseholdMonthlyIncome != value)
                {
                    this._HouseholdMonthlyIncome = value;
                    this.IsDirty = true;
                }
            }
        }



        private Byte? _PeopleEmployed;
        [DataObjectField(false, false, true)]
        public Byte? PeopleEmployed
        {
            get { return this._PeopleEmployed; }
            set
            {
                if (this._PeopleEmployed != value)
                {
                    this._PeopleEmployed = value;
                    this.IsDirty = true;
                }
            }
        }



        private Byte? _Children;
        [DataObjectField(false, false, true)]
        public Byte? Children
        {
            get { return this._Children; }
            set
            {
                if (this._Children != value)
                {
                    this._Children = value;
                    this.IsDirty = true;
                }
            }
        }


        private Int16? _GovernmentProgramId;
        [DataObjectField(false, false, true)]
        public Int16? GovernmentProgramId
        {
            get { return this._GovernmentProgramId; }
            set
            {
                if (this._GovernmentProgramId != value)
                {
                    this._GovernmentProgramId = value;
                    this.IsDirty = true;
                }
            }
        }


        /// <summary>
        /// GovernmentProgram lookup
        /// </summary>
        public GovernmentPrograms? GovernmentProgram
        {
            get
            {
                if (this.GovernmentProgramId.HasValue)
                    return (GovernmentPrograms)this.GovernmentProgramId;
                else return null;
            }
            set { this.GovernmentProgramId = (short)value; }
        }


        private String _Last4SSN;
        [DataObjectField(false, false, true)]
        public String Last4SSN
        {
            get { return this._Last4SSN; }
            set
            {
                if (this._Last4SSN != value)
                {
                    this._Last4SSN = value;
                    this.IsDirty = true;
                }
            }
        }

        private String _BillToMiddleName;
        [DataObjectField(false, false, true)]
        public String BillToMiddleName
        {
            get { return this._BillToMiddleName; }
            set
            {
                if (this._BillToMiddleName != value)
                {
                    this._BillToMiddleName = value;
                    this.IsDirty = true;
                }
            }
        }
        private String _Signature;
        [DataObjectField(false, false, true)]
        public String Signature
        {
            get { return this._Signature; }
            set
            {
                if (this._Signature != value)
                {
                    this._Signature = value;
                    this.IsDirty = true;
                }
            }
        }
        private DateTime? _SignatureTime;
        [DataObjectField(false, false, true)]
        public DateTime? SignatureTime
        {
            get { return this._SignatureTime; }
            set
            {
                if (this._SignatureTime != value)
                {
                    this._SignatureTime = value;
                    this.IsDirty = true;
                }
            }
        }
        #endregion



        #region CURD

        public virtual bool Delete()
        {
            int recordsAffected = 0;
            StringBuilder deleteQuery = new StringBuilder();
            deleteQuery.Append("DELETE FROM ac_Orders_Ex");
            deleteQuery.Append(" WHERE  OrderId = @OrderId  ");
            Database database = Token.Instance.Database;
            MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null);
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {

                database.AddInParameter(deleteCommand, "@OrderId", System.Data.DbType.Int32, this.OrderId);

                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }
            MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();
            return (recordsAffected > 0);
        }

        public virtual bool Load(Int32 pOrderId)
        {
            bool result = false;

            this.OrderId = pOrderId;


            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SET NOCOUNT ON; SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM ac_Orders_Ex");
            selectQuery.Append(" WHERE  OrderId = @OrderId  ");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());


            database.AddInParameter(selectCommand, "@OrderId", System.Data.DbType.Int32, this.OrderId);


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

        public virtual SaveResult Save()
        {
            if (this.IsDirty)
            {
                Database database = Token.Instance.Database;
                MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null); 
                bool recordExists = true;

                if (recordExists)
                {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SET NOCOUNT ON; SELECT COUNT(1) As RecordCount FROM ac_Orders_Ex");
                    selectQuery.Append(" WHERE  OrderId = @OrderId ");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {

                        database.AddInParameter(selectCommand, "@OrderId", System.Data.DbType.Int32, this.OrderId);


                        if ((int)database.ExecuteScalar(selectCommand) == 0)
                        {
                            recordExists = false;
                        }
                    }
                }

                int result = 0;
                if (recordExists)
                {
                    //UPDATE
                    StringBuilder updateQuery = new StringBuilder();
                    updateQuery.Append("UPDATE ac_Order_Ex SET ");

                    updateQuery.Append("CellPhone = @CellPhone,HomePhone = @HomePhone,DOB = @DOB,PeopleInHousehold = @PeopleInHousehold,AnnualIncome = @AnnualIncome,MonthlyIncome = @MonthlyIncome,");
                    updateQuery.Append("HouseholdAnnualIncome = @HouseholdAnnualIncome,HouseholdMonthlyIncome = @HouseholdMonthlyIncome,PeopleEmployed = @PeopleEmployed,Children = @Children,GovernmentProgramId = @GovernmentProgramId,Last4SSN = @Last4SSN,");
                    updateQuery.Append("Signature = @Signature, SignatureTime = @SignatureTime");
                    updateQuery.Append(" WHERE  OrderId = @OrderId ");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {


                        database.AddInParameter(updateCommand, "@OrderId", System.Data.DbType.Int32, this.OrderId);

                        database.AddInParameter(updateCommand, "@CellPhone", System.Data.DbType.String, this.CellPhone);

                        database.AddInParameter(updateCommand, "@HomePhone", System.Data.DbType.String, this.HomePhone);

                        database.AddInParameter(updateCommand, "@DOB", System.Data.DbType.DateTime, this.DOB);

                        database.AddInParameter(updateCommand, "@PeopleInHousehold", System.Data.DbType.Int16, this.PeopleInHousehold);

                        database.AddInParameter(updateCommand, "@AnnualIncome", System.Data.DbType.Decimal, this.AnnualIncome);

                        database.AddInParameter(updateCommand, "@MonthlyIncome", System.Data.DbType.Decimal, this.MonthlyIncome);

                        database.AddInParameter(updateCommand, "@HouseholdAnnualIncome", System.Data.DbType.Decimal, this.HouseholdAnnualIncome);

                        database.AddInParameter(updateCommand, "@HouseholdMonthlyIncome", System.Data.DbType.Decimal, this.HouseholdMonthlyIncome);

                        database.AddInParameter(updateCommand, "@PeopleEmployed", System.Data.DbType.Int16, this.PeopleEmployed);

                        database.AddInParameter(updateCommand, "@Children", System.Data.DbType.Int16, this.Children);

                        database.AddInParameter(updateCommand, "@GovernmentProgramId", System.Data.DbType.Int16, this.GovernmentProgramId);

                        database.AddInParameter(updateCommand, "@Last4SSN", System.Data.DbType.String, this.Last4SSN);
                        database.AddInParameter(updateCommand, "@BillToMiddleName", System.Data.DbType.String, this.BillToMiddleName);
                        database.AddInParameter(updateCommand, "@Signature", System.Data.DbType.String, this.Signature);


                        if (this.SignatureTime.HasValue)
                            database.AddInParameter(updateCommand, "@SignatureTime", System.Data.DbType.DateTime, LocaleHelper.FromLocalTime(this.SignatureTime.Value));
                        else
                            database.AddInParameter(updateCommand, "@SignatureTime", System.Data.DbType.DateTime, null);

                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO ac_Order_Ex (OrderId,CellPhone,HomePhone,DOB,PeopleInHousehold,AnnualIncome,MonthlyIncome,HouseholdAnnualIncome,HouseholdMonthlyIncome,PeopleEmployed,Children,GovernmentProgramId,Last4SSN, BillToMiddleName, Signature, SignatureTime )");
                    insertQuery.Append(" VALUES (@OrderId,@CellPhone,@HomePhone,@DOB,@PeopleInHousehold,@AnnualIncome,@MonthlyIncome,@HouseholdAnnualIncome,@HouseholdMonthlyIncome,@PeopleEmployed,@Children,@GovernmentProgramId,@Last4SSN, @BillToMiddleName, @Signature, @SignatureTime )");


                    insertQuery.Append("; SELECT @@IDENTITY");


                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {

                        database.AddInParameter(insertCommand, "@OrderId", System.Data.DbType.Int16, this.OrderId);

                        database.AddInParameter(insertCommand, "@CellPhone", System.Data.DbType.String, this.CellPhone);

                        database.AddInParameter(insertCommand, "@HomePhone", System.Data.DbType.String, this.HomePhone);

                        database.AddInParameter(insertCommand, "@DOB", System.Data.DbType.DateTime, this.DOB);

                        database.AddInParameter(insertCommand, "@PeopleInHousehold", System.Data.DbType.Int16, this.PeopleInHousehold);

                        database.AddInParameter(insertCommand, "@AnnualIncome", System.Data.DbType.Decimal, this.AnnualIncome);

                        database.AddInParameter(insertCommand, "@MonthlyIncome", System.Data.DbType.Decimal, this.MonthlyIncome);

                        database.AddInParameter(insertCommand, "@HouseholdAnnualIncome", System.Data.DbType.Decimal, this.HouseholdAnnualIncome);

                        database.AddInParameter(insertCommand, "@HouseholdMonthlyIncome", System.Data.DbType.Decimal, this.HouseholdMonthlyIncome);

                        database.AddInParameter(insertCommand, "@PeopleEmployed", System.Data.DbType.Int16, this.PeopleEmployed);

                        database.AddInParameter(insertCommand, "@Children", System.Data.DbType.Int16, this.Children);

                        database.AddInParameter(insertCommand, "@GovernmentProgramId", System.Data.DbType.Int16, this.GovernmentProgramId);

                        database.AddInParameter(insertCommand, "@Last4SSN", System.Data.DbType.String, this.Last4SSN);

                        database.AddInParameter(insertCommand, "@BillToMiddleName", System.Data.DbType.String, this.BillToMiddleName);

                        database.AddInParameter(insertCommand, "@Signature", System.Data.DbType.String, this.Signature);
                        

                        if (this.SignatureTime.HasValue)
                            database.AddInParameter(insertCommand, "@SignatureTime", System.Data.DbType.DateTime, LocaleHelper.FromLocalTime(this.SignatureTime.Value));
                        else
                            database.AddInParameter(insertCommand, "@SignatureTime", System.Data.DbType.DateTime, null);

                        //RESULT IS NUMBER OF RECORDS AFFECTED;
                        result = database.ExecuteNonQuery(insertCommand);

                        this.OrderId = result;

                    }
                }
                MakerShop.Stores.AuditEventDataSource.AuditInfoEnd(); 
                //OBJECT IS DIRTY IF NO RECORDS WERE UPDATED OR INSERTED
                this.IsDirty = (result == 0);
                if (this.IsDirty) { return SaveResult.Failed; }
                else { return (recordExists ? SaveResult.RecordUpdated : SaveResult.RecordInserted); }
            }

            //SAVE IS SUCCESSFUL IF OBJECT IS NOT DIRTY
            return SaveResult.NotDirty;
        }

        #endregion
    }
}
