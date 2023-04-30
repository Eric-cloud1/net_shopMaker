using System;
using MakerShop.Common;
using MakerShop.Payments;
using MakerShop.Orders;
using MakerShop.Users;
using MakerShop.Utility;

namespace MakerShop.Products
{
    public partial class SubscriptionPlanDetails
    {
        public SubscriptionPlanDetails(Int32 pProductId, PaymentTypes pPaymentType)
        {
            this.ProductId = pProductId;
            this.PaymentTypeId = (short)pPaymentType;

        }

        public static SubscriptionPlanDetailsCollection CreateBlankCollection(Int32 pProductId)
        {
            return CreateBlankCollection(pProductId);
        }
        public static SubscriptionPlanDetailsCollection CreateBlankCollection(Int32 pProductId, decimal pProductPrice)
        {
            SubscriptionPlanDetailsCollection subscriptionPlanDetails = new SubscriptionPlanDetailsCollection();
            subscriptionPlanDetails.Add(CreateBlankPlan_Initial(pProductId));
            SubscriptionPlanDetails spd =  CreateBlankPlan_Trial(pProductId);
            spd.PaymentAmount = pProductPrice;
            subscriptionPlanDetails.Add(spd);
            spd = CreateBlankPlan_Recurring(pProductId);
            spd.PaymentAmount = pProductPrice;
            subscriptionPlanDetails.Add(spd);
            return subscriptionPlanDetails;
        }
        public static SubscriptionPlanDetails CreateBlankPlan_Initial(Int32 pProductId)
        {
            SubscriptionPlanDetails i = new SubscriptionPlanDetails(pProductId, PaymentTypes.Initial);
            i.NumberOfPayments = 1;
            i.PaymentAmount = 0;
            i.PaymentDays = 0;
            return i;
        }
        public static SubscriptionPlanDetails CreateBlankPlan_Trial(Int32 pProductId)
        {
            SubscriptionPlanDetails t = new SubscriptionPlanDetails(pProductId, PaymentTypes.Trial);
            t.NumberOfPayments = 1;
            return t;
        }
        public static SubscriptionPlanDetails CreateBlankPlan_Recurring(Int32 pProductId)
        {

            SubscriptionPlanDetails r = new SubscriptionPlanDetails(pProductId, PaymentTypes.Recurring);
            return r;
        }
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
