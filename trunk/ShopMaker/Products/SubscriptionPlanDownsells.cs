using System;
using MakerShop.Common;
using MakerShop.Payments;
using MakerShop.Orders;
using MakerShop.Users;
using MakerShop.Utility;

namespace MakerShop.Products
{
    public partial class SubscriptionPlanDownsells
    {
        public PaymentTypes PaymentType
        {
            get { return (PaymentTypes)_PaymentTypeId; }
            set
            {
                this.PaymentTypeId = (short)value;
            }
        }
    }
  
}
