using System;
using System.Collections.Generic;
using System.Text;
using MakerShop.Payments;

namespace MakerShop.Orders
{
    /// <summary>
    /// Class representing a checkout request
    /// </summary>
    public class CheckoutRequest
    {
        private Payment _Payment;
        private string _CustomerIP;
        /// <summary>
        /// IP associated with this checkout request
        /// </summary>
        public string CustomerIP
        {
            get { return _CustomerIP; }
            set { _CustomerIP = value; }
        }

        /// <summary>
        /// Payment associated with this checkout request
        /// </summary>
        public Payment Payment
        {
            get { return _Payment; }
            set { _Payment = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="payment">Payment object for the checkout request</param>
        public CheckoutRequest(Payment payment)
        {
            _Payment = payment;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="payment">Payment object for the checkout request</param>
        public CheckoutRequest(Payment payment, string customerIP)
        {
            _Payment = payment;
            _CustomerIP = customerIP;
        }

    }
}
