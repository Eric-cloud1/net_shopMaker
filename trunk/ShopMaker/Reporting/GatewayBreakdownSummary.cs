using System;
using System.Collections.Generic;
using System.Text;
using MakerShop.Common;

namespace MakerShop.Reporting.Gateway
{
    /// <summary>
    /// Class that holds summary data about product-breakdown sales
    /// </summary>
    ///
    [Serializable]
    public class GatewayBreakdownSummary
    {
        private int _PaymentGatewayId;
        /// <summary>
        /// The product PaymentGatewayId
        /// </summary>
        public int PaymentGatewayId
        {
            get { return _PaymentGatewayId; }
            set { _PaymentGatewayId = value; }
        }

        private string _name;
        /// <summary>
        /// The product Name
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }


        private DateTime _date;
        /// <summary>
        /// The product Name
        /// </summary>
        public DateTime Date
        {
            get { return _date; }
            set { _date = value; }
        }

        private string _transactionCount;
        /// <summary>
        /// The product Name
        /// </summary>
       public string TransactionCount
        {
            get { return _transactionCount; }
            set { _transactionCount = value; }
        }



     

        private string _TransactionStatus;
        /// <summary>
        /// The TransactionStatus
        /// </summary>
        public string TransactionStatus
        {
            get { return _TransactionStatus; }
            set { _TransactionStatus = value; }
        }

        private string _TransactionType;
        /// <summary>
        /// The TransactionType
        /// </summary>
        public string TransactionType
        {
            get { return _TransactionType; }
            set { _TransactionType = value; }
        }

        private MakerShop.Common.LSDecimal _Amount;
        /// <summary>
        /// The amount
        /// </summary>
        public MakerShop.Common.LSDecimal Amount
        {
            get { return _Amount; }
            set { _Amount = value; }
        }  

    }
}
