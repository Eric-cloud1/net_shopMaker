using System.ComponentModel;

namespace MakerShop.Orders
{
    [DataObject(true)]
    public partial class OrderSubscriptionPlanDetailsDataSource
    {
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static OrderSubscriptionPlanDetailsCollection LoadByOrderId(int orderId)
        {
            return LoadForCriteria("OrderId = " + orderId.ToString());
        }
    }
}
