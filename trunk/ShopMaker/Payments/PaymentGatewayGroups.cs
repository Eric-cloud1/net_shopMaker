using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Payments;
using MakerShop.Stores;
using MakerShop.Utility;


namespace MakerShop.Payments
{
    public partial class PaymentGatewayGroups
    {

       //TODO: get PaymentGatewayId from
       // table xm_PaymentGatewayGroupsPaymentGateways
       // private Int32 _PaymentGatewayId =0;
       //  [DataObjectField(true, false, false)]
       //  public Int32 PaymentGatewayId
       //  {
       //      get { return this._PaymentGatewayId; }
       //      set
       //     {
       //         if (this._PaymentGatewayId != value)
       //         {
       //            this._PaymentGatewayId = value;
       //            this.IsDirty = true;
       //        }
       //    }
       // }


        public PaymentGatewaysCollection PaymentGateways
        {
            get
            {
                return PaymentGatewaysDataSource.LoadGatewayPaymentGroupForCriteria(string.Format(" PaymentGatewayGroupId ={0}", this.PaymentGatewayGroupId));
            }
        }
    }
}
