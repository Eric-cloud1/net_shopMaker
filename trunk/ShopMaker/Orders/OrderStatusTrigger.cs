using MakerShop.Stores;

namespace MakerShop.Orders
{
    /// <summary>
    /// Class representing an Order Status Trigger
    /// </summary>
    public partial class OrderStatusTrigger
    {
        /// <summary>
        /// The store event associated with this trigger
        /// </summary>
        public StoreEvent StoreEvent
        {
            get
            {
                return (StoreEvent)this.StoreEventId;
            }
            set
            {
                this.StoreEventId = (int)value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="storeEvent">store event for this trigger</param>
        public OrderStatusTrigger(StoreEvent storeEvent)
        {
            this.StoreEvent = storeEvent;
        }
    }
}
