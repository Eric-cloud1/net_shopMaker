using System;
using System.Collections.Generic;
using System.Text;
using MakerShop.Users;

namespace MakerShop.Reporting
{
    /// <summary>
    /// Class represents summary of user's orders
    /// </summary>
    public class UserSummary
    {
        private int _UserId;
        private int _OrderCount;
        private Decimal _OrderTotal;
        private User _User;

        /// <summary>
        /// Id of the user
        /// </summary>
        public int UserId
        {
            get { return _UserId; }
            set { _UserId = value; }
        }

        /// <summary>
        /// Number of orders
        /// </summary>
        public int OrderCount
        {
            get { return _OrderCount; }
            set { _OrderCount = value; }
        }

        /// <summary>
        /// Total order amount
        /// </summary>
        public Decimal OrderTotal
        {
            get { return _OrderTotal; }
            set { _OrderTotal = value; }
        }

        /// <summary>
        /// The user object
        /// </summary>
        public User User
        {
            get
            {
                if (this._User == null)
                {
                    this._User = UserDataSource.Load(this.UserId);
                }
                return this._User;
            }
        }
    }
}
