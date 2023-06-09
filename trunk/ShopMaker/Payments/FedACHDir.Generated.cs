//Generated by PersistableBaseGenerator

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Orders;
using MakerShop.Payments;
using MakerShop.Stores;
using MakerShop.Utility;

namespace MakerShop.Payments
{
    /// <summary>
    /// This class represents a FedACHDir object in the database.
    /// </summary>
    public partial class FedACHDir : IPersistable
    {

        #region Constructors

        public FedACHDir() { }

        public FedACHDir(String pRoutingNumber)
        {
            this.RoutingNumber = pRoutingNumber;

        }

        public static string GetColumnNames(string prefix)
        {
            if (string.IsNullOrEmpty(prefix)) prefix = string.Empty;
            else prefix = prefix + ".";
            List<string> columnNames = new List<string>();
            columnNames.Add(prefix + "RoutingNumber");
            columnNames.Add(prefix + "OfficeCode");
            columnNames.Add(prefix + "ServicingFRBNumber");
            columnNames.Add(prefix + "RecordTypeCode");
            columnNames.Add(prefix + "ChangeDate_str");
            columnNames.Add(prefix + "ChangeDate");
            columnNames.Add(prefix + "NewRoutingNumber");
            columnNames.Add(prefix + "CustomerName");
            columnNames.Add(prefix + "Address");
            columnNames.Add(prefix + "City");
            columnNames.Add(prefix + "State");
            columnNames.Add(prefix + "ZipCode");
            columnNames.Add(prefix + "Zip4");
            columnNames.Add(prefix + "AreaCode");
            columnNames.Add(prefix + "Prefix");
            columnNames.Add(prefix + "Suffix");
            columnNames.Add(prefix + "StatusCode");
            columnNames.Add(prefix + "DataViewCode");
            columnNames.Add(prefix + "Filler");


            return string.Join(",", columnNames.ToArray());
        }

        public static void LoadDataReader(FedACHDir pFedACHDir, IDataReader dr)
        {
            //SET FIELDS FROM ROW DATA

            pFedACHDir.RoutingNumber = dr.GetString(0);


            pFedACHDir.OfficeCode = NullableData.GetString(dr, 1);

            pFedACHDir.ServicingFRBNumber = NullableData.GetString(dr, 2);

            pFedACHDir.RecordTypeCode = NullableData.GetString(dr, 3);

            pFedACHDir.ChangeDate_str = NullableData.GetString(dr, 4);
            pFedACHDir.ChangeDate = LocaleHelper.ToLocalTime(NullableData.GetDateTime(dr, 5));

            pFedACHDir.NewRoutingNumber = NullableData.GetString(dr, 6);

            pFedACHDir.CustomerName = NullableData.GetString(dr, 7);

            pFedACHDir.Address = NullableData.GetString(dr, 8);

            pFedACHDir.City = NullableData.GetString(dr, 9);

            pFedACHDir.State = NullableData.GetString(dr, 10);

            pFedACHDir.ZipCode = NullableData.GetString(dr, 11);

            pFedACHDir.Zip4 = NullableData.GetString(dr, 12);

            pFedACHDir.AreaCode = NullableData.GetString(dr, 13);

            pFedACHDir.Prefix = NullableData.GetString(dr, 14);

            pFedACHDir.Suffix = NullableData.GetString(dr, 15);

            pFedACHDir.StatusCode = NullableData.GetString(dr, 16);

            pFedACHDir.DataViewCode = NullableData.GetString(dr, 17);

            pFedACHDir.Filler = NullableData.GetString(dr, 18);

            pFedACHDir.IsDirty = false;
        }

        #endregion

        #region Class Data

        private bool _IsDirty;

        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }


        private String _RoutingNumber;

        [DataObjectField(true, false, false)]
        public String RoutingNumber
        {
            get { return this._RoutingNumber; }
            set
            {
                if (this._RoutingNumber != value)
                {
                    this._RoutingNumber = value;
                    this.IsDirty = true;
                }
            }
        }


        private String _OfficeCode;

        [DataObjectField(true, false, false)]
        public String OfficeCode
        {
            get { return this._OfficeCode; }
            set
            {
                if (this._OfficeCode != value)
                {
                    this._OfficeCode = value;
                    this.IsDirty = true;
                }
            }
        }


        private String _ServicingFRBNumber;

        [DataObjectField(true, false, false)]
        public String ServicingFRBNumber
        {
            get { return this._ServicingFRBNumber; }
            set
            {
                if (this._ServicingFRBNumber != value)
                {
                    this._ServicingFRBNumber = value;
                    this.IsDirty = true;
                }
            }
        }


        private String _RecordTypeCode;

        [DataObjectField(true, false, false)]
        public String RecordTypeCode
        {
            get { return this._RecordTypeCode; }
            set
            {
                if (this._RecordTypeCode != value)
                {
                    this._RecordTypeCode = value;
                    this.IsDirty = true;
                }
            }
        }


        private String _ChangeDate_str;

        [DataObjectField(true, false, false)]
        public String ChangeDate_str
        {
            get { return this._ChangeDate_str; }
            set
            {
                if (this._ChangeDate_str != value)
                {
                    this._ChangeDate_str = value;
                    this.IsDirty = true;
                }
            }
        }


        private DateTime _ChangeDate;

        [DataObjectField(true, false, false)]
        public DateTime ChangeDate
        {
            get { return this._ChangeDate; }
            set
            {
                if (this._ChangeDate != value)
                {
                    this._ChangeDate = value;
                    this.IsDirty = true;
                }
            }
        }


        private String _NewRoutingNumber;

        [DataObjectField(true, false, false)]
        public String NewRoutingNumber
        {
            get { return this._NewRoutingNumber; }
            set
            {
                if (this._NewRoutingNumber != value)
                {
                    this._NewRoutingNumber = value;
                    this.IsDirty = true;
                }
            }
        }


        private String _CustomerName;

        [DataObjectField(true, false, false)]
        public String CustomerName
        {
            get { return this._CustomerName; }
            set
            {
                if (this._CustomerName != value)
                {
                    this._CustomerName = value;
                    this.IsDirty = true;
                }
            }
        }


        private String _Address;

        [DataObjectField(true, false, false)]
        public String Address
        {
            get { return this._Address; }
            set
            {
                if (this._Address != value)
                {
                    this._Address = value;
                    this.IsDirty = true;
                }
            }
        }


        private String _City;

        [DataObjectField(true, false, false)]
        public String City
        {
            get { return this._City; }
            set
            {
                if (this._City != value)
                {
                    this._City = value;
                    this.IsDirty = true;
                }
            }
        }


        private String _State;

        [DataObjectField(true, false, false)]
        public String State
        {
            get { return this._State; }
            set
            {
                if (this._State != value)
                {
                    this._State = value;
                    this.IsDirty = true;
                }
            }
        }


        private String _ZipCode;

        [DataObjectField(true, false, false)]
        public String ZipCode
        {
            get { return this._ZipCode; }
            set
            {
                if (this._ZipCode != value)
                {
                    this._ZipCode = value;
                    this.IsDirty = true;
                }
            }
        }


        private String _Zip4;

        [DataObjectField(true, false, false)]
        public String Zip4
        {
            get { return this._Zip4; }
            set
            {
                if (this._Zip4 != value)
                {
                    this._Zip4 = value;
                    this.IsDirty = true;
                }
            }
        }


        private String _AreaCode;

        [DataObjectField(true, false, false)]
        public String AreaCode
        {
            get { return this._AreaCode; }
            set
            {
                if (this._AreaCode != value)
                {
                    this._AreaCode = value;
                    this.IsDirty = true;
                }
            }
        }


        private String _Prefix;

        [DataObjectField(true, false, false)]
        public String Prefix
        {
            get { return this._Prefix; }
            set
            {
                if (this._Prefix != value)
                {
                    this._Prefix = value;
                    this.IsDirty = true;
                }
            }
        }


        private String _Suffix;

        [DataObjectField(true, false, false)]
        public String Suffix
        {
            get { return this._Suffix; }
            set
            {
                if (this._Suffix != value)
                {
                    this._Suffix = value;
                    this.IsDirty = true;
                }
            }
        }


        private String _StatusCode;

        [DataObjectField(true, false, false)]
        public String StatusCode
        {
            get { return this._StatusCode; }
            set
            {
                if (this._StatusCode != value)
                {
                    this._StatusCode = value;
                    this.IsDirty = true;
                }
            }
        }


        private String _DataViewCode;

        [DataObjectField(true, false, false)]
        public String DataViewCode
        {
            get { return this._DataViewCode; }
            set
            {
                if (this._DataViewCode != value)
                {
                    this._DataViewCode = value;
                    this.IsDirty = true;
                }
            }
        }


        private String _Filler;

        [DataObjectField(true, false, false)]
        public String Filler
        {
            get { return this._Filler; }
            set
            {
                if (this._Filler != value)
                {
                    this._Filler = value;
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
            deleteQuery.Append("DELETE FROM xm_FedACHDir");
            deleteQuery.Append(" WHERE  RoutingNumber = @RoutingNumber  ");
            Database database = Token.Instance.Database;
            MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null);
            using (DbCommand deleteCommand = database.GetSqlStringCommand(deleteQuery.ToString()))
            {

                database.AddInParameter(deleteCommand, "@RoutingNumber", System.Data.DbType.String, this.RoutingNumber);

                recordsAffected = database.ExecuteNonQuery(deleteCommand);
            }
            MakerShop.Stores.AuditEventDataSource.AuditInfoEnd();
            return (recordsAffected > 0);
        }

        public virtual bool Load(String pRoutingNumber)
        {
            bool result = false;

            this.RoutingNumber = pRoutingNumber;


            //CREATE THE DYNAMIC SQL TO LOAD OBJECT
            StringBuilder selectQuery = new StringBuilder();
            selectQuery.Append("SET NOCOUNT ON; SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT " + GetColumnNames(string.Empty));
            selectQuery.Append(" FROM xm_FedACHDir");
            selectQuery.Append(" WHERE  RoutingNumber = @RoutingNumber  ");
            Database database = Token.Instance.Database;
            DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString());


            database.AddInParameter(selectCommand, "@RoutingNumber", System.Data.DbType.String, this.RoutingNumber);


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
                MakerShop.Stores.AuditEventDataSource.AuditInfoBegin(null);
                Database database = Token.Instance.Database;
                bool recordExists = true;

                if (recordExists)
                {
                    //verify whether record is already present
                    StringBuilder selectQuery = new StringBuilder();
                    selectQuery.Append("SET NOCOUNT ON; SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT COUNT(*) As RecordCount FROM xm_FedACHDir");
                    selectQuery.Append(" WHERE  RoutingNumber = @RoutingNumber ");
                    using (DbCommand selectCommand = database.GetSqlStringCommand(selectQuery.ToString()))
                    {

                        database.AddInParameter(selectCommand, "@RoutingNumber", System.Data.DbType.String, this.RoutingNumber);


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
                    updateQuery.Append("UPDATE xm_FedACHDir SET ");

                    updateQuery.Append("OfficeCode = @OfficeCode,ServicingFRBNumber = @ServicingFRBNumber,RecordTypeCode = @RecordTypeCode,ChangeDate_str = @ChangeDate_str,ChangeDate = @ChangeDate,NewRoutingNumber = @NewRoutingNumber,CustomerName = @CustomerName,Address = @Address,City = @City,State = @State,ZipCode = @ZipCode,Zip4 = @Zip4,AreaCode = @AreaCode,Prefix = @Prefix,Suffix = @Suffix,StatusCode = @StatusCode,DataViewCode = @DataViewCode,Filler = @Filler");

                    updateQuery.Append(" WHERE  RoutingNumber = @RoutingNumber ");
                    using (DbCommand updateCommand = database.GetSqlStringCommand(updateQuery.ToString()))
                    {


                        database.AddInParameter(updateCommand, "@RoutingNumber", System.Data.DbType.String, this.RoutingNumber);

                        database.AddInParameter(updateCommand, "@OfficeCode", System.Data.DbType.String, this.OfficeCode);

                        database.AddInParameter(updateCommand, "@ServicingFRBNumber", System.Data.DbType.String, this.ServicingFRBNumber);

                        database.AddInParameter(updateCommand, "@RecordTypeCode", System.Data.DbType.String, this.RecordTypeCode);

                        database.AddInParameter(updateCommand, "@ChangeDate_str", System.Data.DbType.String, this.ChangeDate_str);

                        database.AddInParameter(updateCommand, "@ChangeDate", System.Data.DbType.DateTime, this.ChangeDate);

                        database.AddInParameter(updateCommand, "@NewRoutingNumber", System.Data.DbType.String, this.NewRoutingNumber);

                        database.AddInParameter(updateCommand, "@CustomerName", System.Data.DbType.String, this.CustomerName);

                        database.AddInParameter(updateCommand, "@Address", System.Data.DbType.String, this.Address);

                        database.AddInParameter(updateCommand, "@City", System.Data.DbType.String, this.City);

                        database.AddInParameter(updateCommand, "@State", System.Data.DbType.String, this.State);

                        database.AddInParameter(updateCommand, "@ZipCode", System.Data.DbType.String, this.ZipCode);

                        database.AddInParameter(updateCommand, "@Zip4", System.Data.DbType.String, this.Zip4);

                        database.AddInParameter(updateCommand, "@AreaCode", System.Data.DbType.String, this.AreaCode);

                        database.AddInParameter(updateCommand, "@Prefix", System.Data.DbType.String, this.Prefix);

                        database.AddInParameter(updateCommand, "@Suffix", System.Data.DbType.String, this.Suffix);

                        database.AddInParameter(updateCommand, "@StatusCode", System.Data.DbType.String, this.StatusCode);

                        database.AddInParameter(updateCommand, "@DataViewCode", System.Data.DbType.String, this.DataViewCode);

                        database.AddInParameter(updateCommand, "@Filler", System.Data.DbType.String, this.Filler);


                        //RESULT IS NUMBER OF RECORDS AFFECTED
                        result = database.ExecuteNonQuery(updateCommand);
                    }
                }
                else
                {
                    //INSERT
                    StringBuilder insertQuery = new StringBuilder();
                    insertQuery.Append("INSERT INTO xm_FedACHDir (RoutingNumber,OfficeCode,ServicingFRBNumber,RecordTypeCode,ChangeDate_str,ChangeDate,NewRoutingNumber,CustomerName,Address,City,State,ZipCode,Zip4,AreaCode,Prefix,Suffix,StatusCode,DataViewCode,Filler )");
                    insertQuery.Append(" VALUES (@RoutingNumber,@OfficeCode,@ServicingFRBNumber,@RecordTypeCode,@ChangeDate_str,@ChangeDate,@NewRoutingNumber,@CustomerName,@Address,@City,@State,@ZipCode,@Zip4,@AreaCode,@Prefix,@Suffix,@StatusCode,@DataViewCode,@Filler )");


                    insertQuery.Append("; SELECT @@IDENTITY");


                    using (DbCommand insertCommand = database.GetSqlStringCommand(insertQuery.ToString()))
                    {

                        database.AddInParameter(insertCommand, "@RoutingNumber", System.Data.DbType.String, this.RoutingNumber);

                        database.AddInParameter(insertCommand, "@OfficeCode", System.Data.DbType.String, this.OfficeCode);

                        database.AddInParameter(insertCommand, "@ServicingFRBNumber", System.Data.DbType.String, this.ServicingFRBNumber);

                        database.AddInParameter(insertCommand, "@RecordTypeCode", System.Data.DbType.String, this.RecordTypeCode);

                        database.AddInParameter(insertCommand, "@ChangeDate_str", System.Data.DbType.String, this.ChangeDate_str);

                        database.AddInParameter(insertCommand, "@ChangeDate", System.Data.DbType.DateTime, this.ChangeDate);

                        database.AddInParameter(insertCommand, "@NewRoutingNumber", System.Data.DbType.String, this.NewRoutingNumber);

                        database.AddInParameter(insertCommand, "@CustomerName", System.Data.DbType.String, this.CustomerName);

                        database.AddInParameter(insertCommand, "@Address", System.Data.DbType.String, this.Address);

                        database.AddInParameter(insertCommand, "@City", System.Data.DbType.String, this.City);

                        database.AddInParameter(insertCommand, "@State", System.Data.DbType.String, this.State);

                        database.AddInParameter(insertCommand, "@ZipCode", System.Data.DbType.String, this.ZipCode);

                        database.AddInParameter(insertCommand, "@Zip4", System.Data.DbType.String, this.Zip4);

                        database.AddInParameter(insertCommand, "@AreaCode", System.Data.DbType.String, this.AreaCode);

                        database.AddInParameter(insertCommand, "@Prefix", System.Data.DbType.String, this.Prefix);

                        database.AddInParameter(insertCommand, "@Suffix", System.Data.DbType.String, this.Suffix);

                        database.AddInParameter(insertCommand, "@StatusCode", System.Data.DbType.String, this.StatusCode);

                        database.AddInParameter(insertCommand, "@DataViewCode", System.Data.DbType.String, this.DataViewCode);

                        database.AddInParameter(insertCommand, "@Filler", System.Data.DbType.String, this.Filler);

                        //RESULT IS NUMBER OF RECORDS AFFECTED;
                        result = database.ExecuteNonQuery(insertCommand);


                     //   this.RoutingNumber = result;


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
