using System;
using MakerShop.Common;
using MakerShop.Payments;
using MakerShop.Orders;
using MakerShop.Users;
using MakerShop.Utility;

namespace MakerShop.Products
{
    public partial class SubscriptionPlan
    {
        /// <summary>
        /// Indicates the unit of time that applies to the payment interval.
        /// </summary>
        public PaymentFrequencyUnit PaymentFrequencyUnit
        {
            get { return (PaymentFrequencyUnit)this.PaymentFrequencyUnitId; }
            set { this.PaymentFrequencyUnitId = (byte)value; }
        }

        /// <summary>
        /// Indicates whether this subscription plan includes a recurring payment.
        /// </summary>
        public bool IsRecurring
        {
            get
            {
                if (this.NumberOfPayments == 0) return true;
                return this.NumberOfPayments > 1;
            }
        }
        /// <summary>
        /// Create a subscription for this plan for the given customer.
        /// </summary>
        /// <param name="orderItemId">ID of the order item for the purchase of this subscription.</param>
        /// <param name="userId">ID of the user who has purchased the subscription.</param>
        /// <param name="activate">Indicates whether the subscription should be activated when it is generated.</param>
        /// <returns>The subscription created</returns>
        public Subscription Generate(int orderItemId, int userId, bool activate)
        {
            User user = UserDataSource.Load(userId);
            if (user != null)
            {
                Subscription subscription = new Subscription();
                subscription.ProductId = this.ProductId;
                subscription.OrderItemId = orderItemId;
                subscription.UserId = userId;
                subscription.Save();
                if (activate) subscription.Activate();
                return subscription;
            }
            return null;
        }

        /// <summary>
        /// Calculates the expiration date for a subscription being activated.
        /// </summary>
        /// <returns>The expiration date for the subscription, or DateTime.MaxValue if there is no expiration.</returns>
        public DateTime CalculateExpiration()
        {
            //CHECK IF SUBSCRIPTION EXPIRES
            if (this.NumberOfPayments > 0)
            {
                DateTime tempDate = DateTime.MinValue;
                //EXPIRATION IS SPECIFIED, CALCULATE TERMINATION DATE
                switch (this.PaymentFrequencyUnit)
                {
                    case PaymentFrequencyUnit.Day:
                        int days = this.PaymentFrequency * this.NumberOfPayments;
                        tempDate = LocaleHelper.LocalNow.AddDays(days);
                        return new DateTime(tempDate.Year,tempDate.Month,tempDate.Day,23,59,59);
                    case PaymentFrequencyUnit.Month:
                        int months = this.PaymentFrequency * this.NumberOfPayments;
                        tempDate = LocaleHelper.LocalNow.AddMonths(months);
                        return new DateTime(tempDate.Year, tempDate.Month, tempDate.Day, 23, 59, 59);
                }
            }
            return DateTime.MaxValue;
        }

        /// <summary>
        /// Calculates the recurring charge for this subscription.
        /// </summary>
        /// <param name="price">The price of this product for the customer.</param>
        /// <returns>The recurring charge for the subscription.</returns>
        /// <remarks>Recurring charge is either the product price or the specified charge.</remarks>
        public LSDecimal CalculateRecurringCharge(LSDecimal price)
        {
            if (!this.RecurringChargeSpecified) return price;
            return this.RecurringCharge;
        }

    }
}
