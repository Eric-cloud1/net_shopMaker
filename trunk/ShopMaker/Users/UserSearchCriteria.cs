//-----------------------------------------------------------------------
// <copyright file="UserSearchCriteria.cs" company="Able Solutions Corporation">
//     Copyright (c) 2009 Able Solutions Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace MakerShop.Users
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Text;
    using System.Collections.Generic;
    using MakerShop.Common;
    using MakerShop.Data;

    /// <summary>
    /// Class representing a user search critera
    /// </summary>
    [Serializable()]
    public class UserSearchCriteria
    {
        private string _UserName;
        private string _Email;
        private string _FirstName;
        private string _LastName;
        private string _Company;
        private int _GroupId;
        private List<int> _GroupIds = new List<int>();
        private bool _IncludeAnonymous;
        private int _UserId;
        private bool _IsGateway = false;


        /// <summary>
        /// Gets or sets the userId to search for
        /// </summary>
        public int UserId
        {
            get
            {
                return _UserId;
            }
            set
            {
                _UserId = value;
            }
        }

        /// <summary>
        /// Gets or sets the userId to search for
        /// </summary>
        public bool IsGateway
        {
            get
            {
                return _IsGateway;
            }
            set
            {
                _IsGateway = value;
            }

        }

        /// <summary>
        /// Gets or sets the username to search for
        /// </summary>
        public string UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                _UserName = value;
            }
        }

        /// <summary>
        /// Gets or sets the email to search for
        /// </summary>
        public string Email
        {
            get
            {
                return _Email;
            }
            set
            {
                _Email = value;
            }
        }

        /// <summary>
        /// Gets or sets the first name of the user to search for
        /// </summary>
        public string FirstName
        {
            get
            {
                return _FirstName;
            }
            set
            {
                _FirstName = value;
            }
        }

        /// <summary>
        /// Gets or sets the last name of the user to search for
        /// </summary>
        public string LastName
        {
            get
            {
                return _LastName;
            }
            set
            {
                _LastName = value;
            }
        }

        /// <summary>
        /// Gets or sets the company of the user to search for
        /// </summary>
        public string Company
        {
            get
            {
                return _Company;
            }
            set
            {
                _Company = value;
            }
        }

        /// <summary>
        /// Gets or sets the group of the user to search for
        /// </summary>
        public int GroupId
        {
            get
            {
                return _GroupId;
            }
            set
            {
                _GroupId = value;
            }
        }
        /// <summary>
        /// Gets or sets the group of the user to search for
        /// </summary>
        public List<int> GroupIds
        {
            get
            {
                return _GroupIds;
            }
            set
            {
                _GroupIds = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether anonymous users should be 
        /// included in the results.
        /// </summary>
        public bool IncludeAnonymous
        {
            get
            {
                return _IncludeAnonymous;
            }
            set
            {
                _IncludeAnonymous = value;
            }
        }

        internal DbCommand GenerateDatabaseCommand(bool isCount, string sortExpression)
        {
            Token token = Token.Instance;
            Database database = token.Database;
            StringBuilder sql = new StringBuilder();
            sql.Append("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT");
            if (isCount) sql.Append(" COUNT(U.UserId) AS TotalRecords");
            else sql.Append(" U.UserId");
            if (NeedsAddressTable(sortExpression))
            {
                sql.Append(" FROM (ac_Users U LEFT JOIN ac_Addresses A ON U.PrimaryAddressId = A.AddressId)");
            }
            else
            {
                sql.Append(" FROM ac_Users U");
            }
            sql.Append(" WHERE U.StoreId = @storeId");
            
            // BUILD THE WHERE CRITERIA
            if (!string.IsNullOrEmpty(_UserName)) sql.Append(" AND U.LoweredUserName " + GetOperator(_UserName) + " @userName");
            if (!string.IsNullOrEmpty(_Email)) sql.Append(" AND U.LoweredEmail " + GetOperator(_Email) + " @email");
            if (!string.IsNullOrEmpty(_FirstName)) sql.Append(" AND A.FirstName " + GetOperator(_FirstName) + " @firstName");
            if (!string.IsNullOrEmpty(_LastName)) sql.Append(" AND A.LastName " + GetOperator(_LastName) + " @lastName");
            if (!string.IsNullOrEmpty(_Company)) sql.Append(" AND A.Company " + GetOperator(_Company) + " @company");
            if (_GroupId > 0) sql.Append(" AND U.UserId IN (SELECT UserId FROM ac_UserGroups WHERE GroupId = @groupId)");
            if (_GroupIds.Count > 0)
            {
                sql.Append(" AND U.UserId IN (SELECT UserId FROM ac_UserGroups WHERE GroupId IN (");
                foreach (int i in _GroupIds)
                    sql.Append(i.ToString() + ",");
                sql.Append("-1)");

                if (_IsGateway == true)
                {
                   // sql.Append(" AND UserId IN (SELECT UserId FROM dbo.ac_UserSettings WHERE FieldName='Gateway' AND FieldValue ");
                   // sql.Append(" IN (SELECT FieldValue FROM dbo.ac_UserSettings WHERE FieldName='Gateway' AND UserId = @userId))");
                    sql.Append(" AND UserId IN (SELECT UserId FROM dbo.ac_UserSettings WHERE FieldName='GatewayID' AND FieldValue ");
                    sql.Append(" IN (SELECT FieldValue FROM dbo.ac_UserSettings WHERE FieldName='GatewayID' AND UserId = @userId))");
                }
                sql.Append(")");
            }
            

            if (!IncludeAnonymous) sql.Append(" AND (U.IsAnonymous = 0 AND U.UserName NOT LIKE 'zz_anonymous_%@domain.xyz')");
            if (!isCount && !string.IsNullOrEmpty(sortExpression)) sql.Append(" ORDER BY " + sortExpression);

            // GENERATE THE DATABASE COMMAND
            DbCommand selectCommand = database.GetSqlStringCommand(sql.ToString());

            // POPULATE THE VARIABLES
            database.AddInParameter(selectCommand, "@storeId", DbType.Int32, token.StoreId);
            if (!string.IsNullOrEmpty(_UserName))
                database.AddInParameter(selectCommand, "@userName", DbType.String, _UserName.Replace("*", "%").Replace("?", "_").ToLowerInvariant());
            if (!string.IsNullOrEmpty(_Email))
                database.AddInParameter(selectCommand, "@email", DbType.String, _Email.Replace("*", "%").Replace("?", "_").ToLowerInvariant());
            if (!string.IsNullOrEmpty(_FirstName))
                database.AddInParameter(selectCommand, "@firstName", DbType.String, _FirstName.Replace("*", "%").Replace("?","_"));
            if (!string.IsNullOrEmpty(_LastName))
                database.AddInParameter(selectCommand, "@lastName", DbType.String, _LastName.Replace("*", "%").Replace("?","_"));
            if (!string.IsNullOrEmpty(_Company))
                database.AddInParameter(selectCommand, "@company", DbType.String, _Company.Replace("*", "%").Replace("?","_"));
            if (_GroupId > 0)
                database.AddInParameter(selectCommand, "@groupId", DbType.Int32, _GroupId);
             
            if (_UserId > 0)
                database.AddInParameter(selectCommand, "@userId", DbType.Int32, _UserId);

            // RETURN THE GENERATED COMMAND
            return selectCommand;
        }

        private string GetOperator(string value)
        {
            if (value.IndexOfAny("*?%_".ToCharArray()) > -1) return "LIKE";
            return "=";
        }

        private bool NeedsAddressTable(string sortExpression)
        {
            if (!string.IsNullOrEmpty(_FirstName)) return true;
            if (!string.IsNullOrEmpty(_LastName)) return true;
            if (!string.IsNullOrEmpty(_Company)) return true;
            string loweredSortExpression = sortExpression.ToLowerInvariant();
            if (loweredSortExpression == "firstname") return true;
            if (loweredSortExpression == "lastname") return true;
            if (loweredSortExpression == "company") return true;
            return loweredSortExpression.StartsWith("a.");
        }
    }
}
