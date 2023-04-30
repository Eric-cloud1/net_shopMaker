using System;
using System.Collections.Generic;
using System.Text;
using MakerShop.Common;

namespace MakerShop.Reporting
{
    /// <summary>
    /// Class that holds summary data about products purchase
    /// </summary>
    public class PurchaseSummary
    {
        private int _OrderId;

        /// <summary>
        /// OrderId for this purchase
        /// </summary>
        public int OrderId
        {
            get { return _OrderId; }
            set { _OrderId = value; }
        }

        private int _OrderNumber;
        /// <summary>
        /// Order Number for this purchase
        /// </summary>
        public int OrderNumber
        {
            get { return _OrderNumber; }
            set { _OrderNumber = value; }
        }
        private DateTime _OrderDate;
        /// <summary>
        /// Order date for this purchase
        /// </summary>
        public DateTime OrderDate
        {
            get { return _OrderDate; }
            set { _OrderDate = value; }
        }

        private string _ProductName;
        /// <summary>
        /// Name of the product purchased
        /// </summary>
        public string ProductName
        {
            get { return _ProductName; }
            set { _ProductName = value; }
        }
        private LSDecimal _PurchasePrice;

        public LSDecimal PurchasePrice
        {
            get { return _PurchasePrice; }
            set { _PurchasePrice = value; }
        }
        private int _Quantity;
        /// <summary>
        /// Quantity of the product purchased
        /// </summary>
        public int Quantity
        {
            get { return _Quantity; }
            set { _Quantity = value; }
        }

        private LSDecimal _Total;
        /// <summary>
        /// Total price
        /// </summary>
        public LSDecimal Total
        {
            get { return _Total; }
            set { _Total = value; }
        }

        public PurchaseSummary(int orderId, int orderNumber, DateTime orderDate, string productName, Decimal purchasePrice, int quantity, Decimal total)
        {
            this.OrderId = orderId;
            this.OrderNumber = orderNumber;
            this.OrderDate = orderDate;
            this.ProductName = productName;
            this.PurchasePrice = purchasePrice;
            this.Quantity = quantity;
            this.Total = total;
        }
    }
}
