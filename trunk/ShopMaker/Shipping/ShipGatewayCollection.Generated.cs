namespace MakerShop.Shipping
{
    using System;
    using MakerShop.Common;
    /// <summary>
    /// This class implements a PersistentCollection of ShipGateway objects.
    /// </summary>
    public partial class ShipGatewayCollection : PersistentCollection<ShipGateway>
    {
        /// <summary>
        /// Gets the index of the CatalogNode object in this collection whose primary key
        /// matches the given value.
        /// </summary>
        /// <param name="shipGatewayId">Value of ShipGatewayId of the required object.</param>
        /// <returns>Index of the required object.</returns>
        public int IndexOf(Int32 shipGatewayId)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (shipGatewayId == this[i].ShipGatewayId) return i;
            }
            return -1;
        }
    }
}
