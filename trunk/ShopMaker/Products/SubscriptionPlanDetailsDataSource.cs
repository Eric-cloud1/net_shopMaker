using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Utility;
using MakerShop.Payments;

namespace MakerShop.Products
{
    /// <summary>
    /// DataSource class for SubscriptionPlan objects
    /// </summary>
    [DataObject(true)]
    public partial class SubscriptionPlanDetailsDataSource
    {
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static SubscriptionPlanDetailsCollection LoadByProductId(Int32 ProductId)
        {
            return LoadForCriteria("productId = " + ProductId.ToString());
        }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static SubscriptionPlanDetails Load(Int32 pProductId, PaymentTypes pPaymentType)
        {
            SubscriptionPlanDetails varSubscriptionPlanDetails = new SubscriptionPlanDetails();
            if (varSubscriptionPlanDetails.Load(pProductId, (short)pPaymentType)) return varSubscriptionPlanDetails;
            return null;
        }

    }
    
}
