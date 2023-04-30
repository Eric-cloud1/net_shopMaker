
namespace MakerShop.Payments
{
    using System;
    using MakerShop.Common;

    public partial class GatewayResponseActionCollection : PersistentCollection<GatewayResponseAction>
    {
        public int IndexOf(Int32 pGatewayResponseId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if ((pGatewayResponseId == this[i].GatewayResponseId)) return i;
            }
            return -1;
        }
    }
}
