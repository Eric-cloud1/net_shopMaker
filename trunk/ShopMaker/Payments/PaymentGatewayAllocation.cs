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
    public partial class PaymentGatewayAllocation
    {
        private PaymentMethod _PaymentMethod;
        public PaymentMethod PaymentMethod
        {
            get
            {
                if (_PaymentMethod == null)
                    _PaymentMethod = PaymentMethodDataSource.Load(_PaymentMethodId);
                if (_PaymentMethod == null)
                    return new PaymentMethod();
                return _PaymentMethod;
            }
        }
        
    }
}
