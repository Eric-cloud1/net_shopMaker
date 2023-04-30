using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.Orders
{
    /// <summary>
    /// Class representing checkout response
    /// </summary>
    public class CheckoutResponse
    {
        private bool _Success;
        private Order _Order;
        private List<string> _WarningMessages;

        /// <summary>
        /// Indicates whether checkout was successful or not
        /// </summary>
        public bool Success
        {
            get { return _Success; }
            set { _Success = value; }
        }

        /// <summary>
        /// Id of the order associated with this checkout
        /// </summary>
        public int OrderId
        {
            get
            {
                if (_Order != null)
                    return _Order.OrderId;
                return 0;
            }

        }

        /// <summary>
        /// OrderNumber assigned to the order associated with this checkout
        /// </summary>
        public int OrderNumber
        {
            get
            {
                if (_Order != null)
                    return _Order.OrderNumber;
                return 0;
            }

        }

        /// <summary>
        /// Warning messages that may have occured during the checkout
        /// </summary>
        public List<string> WarningMessages
        {
            get
            {
                if (_WarningMessages == null)
                    return new List<string>();
                return _WarningMessages;
            }
        }
        public Order Order
        {
            get
            {
                return _Order;
            }
            set
            {
                _Order = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="success">true will create successful response, false will create failed response</param>
        /// <param name="order">The order </param>
        public CheckoutResponse(bool success, Order order, List<string> warningMessages)
        {
            _Success = success;
            Order = order;
            _WarningMessages = warningMessages;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="success">true will create successful response, false will create failed response</param>
        /// <param name="warningMessages">A list of warning messages to associate with this checkout response</param>
        public CheckoutResponse(bool success, List<string> warningMessages)
        {
            _Success = success;
            _WarningMessages = warningMessages;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="success">true will create successful response, false will create failed response</param>
        /// <param name="warningMessages">A list of warning messages to associate with this checkout response</param>
        public CheckoutResponse(bool success)
        {
            _Success = success;

        }
    }
}
