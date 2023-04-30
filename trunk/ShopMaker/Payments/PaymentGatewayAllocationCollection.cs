
namespace MakerShop.Payments
{
    using System;
    using MakerShop.Common;
    using System.Linq;

    public partial class PaymentGatewayAllocationCollection : PersistentCollection<PaymentGatewayAllocation>
    {
        public byte TotalAllocation
        {
            get
            {
                byte b = 0;
                foreach (PaymentGatewayAllocation pga in this)
                {
                    b += pga.PriorityPercent;
                }
                return b;
            }
        }
        public void DeleteForPaymentInstrument(PaymentInstrument paymentInstrument)
        {
            // you may need to do a FOR loop for this to work, the delete may throw an exception. Remove this comment after testing.
            foreach (PaymentGatewayAllocation pga in this)
            {
                if (pga.PaymentMethod.PaymentInstrument == paymentInstrument)
                {
                    pga.Delete();
                }
            }
        }

        public bool Save(string paymentInstrumentId)
        {
            // delete all for Template
            PaymentGatewayAllocation pga = new PaymentGatewayAllocation();

            pga.PaymentGatewayTemplateId = base[0].PaymentGatewayTemplateId;
            pga.PaymentMethodId = base[0].PaymentMethodId;
           
            pga.DeleteAll(paymentInstrumentId);

            return base.Save();
        }
 
   }
}

