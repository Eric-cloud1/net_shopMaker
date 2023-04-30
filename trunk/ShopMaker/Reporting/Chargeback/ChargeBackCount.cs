using System;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.ComponentModel;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Stores;
using MakerShop.Utility;


namespace MakerShop.Reporting
{
 
    public partial class ChargeBackCount
    {

        [DataObjectField(false, false, false)]
        public Payments.PaymentGateway PaymentGateway
        {
            get { return Payments.PaymentGatewayDataSource.Load(this._PaymentGatewayId); }
            set
            {
                if (this._PaymentGatewayId != value.PaymentGatewayId)
                {
                    this._PaymentGatewayId = value.PaymentGatewayId;
                    this.IsDirty = true;
                }
            }
        }
    }
}