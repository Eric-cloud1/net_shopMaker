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
    public partial class SubscriptionPlanDownsellsDataSource
    {
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static SubscriptionPlanDownsellsCollection LoadByProductId(Int32 ProductId)
        {
            return LoadForCriteria("productId = " + ProductId.ToString());
        }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static SubscriptionPlanDownsellsCollection Load(Int32 ProductId, PaymentTypes PaymentType)
        {
            
            return LoadForCriteria("productId = " + ProductId.ToString() + " and PaymentTypeId = " + ((short)PaymentType).ToString());
           
        }

    }
    
}
