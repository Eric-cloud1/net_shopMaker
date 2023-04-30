using System;
using System.Collections.Generic;
using System.Text;
using MakerShop.Common;
using MakerShop.Exceptions;
using MakerShop.Products;
using MakerShop.Shipping;
using MakerShop.Payments;
using MakerShop.Utility;

namespace MakerShop.Orders
{
    /// <summary>
    /// Class that represents an order item
    /// </summary>
    public partial class OrderItem
    {
        private ProductVariant _ProductVariant = null;

        /// <summary>
        /// Product variant for this order item.
        /// </summary>
        public ProductVariant ProductVariant
        {
            get
            {
                if (!string.IsNullOrEmpty(this.OptionList) && _ProductVariant == null)
                {
                    _ProductVariant = ProductVariantDataSource.LoadForOptionList(this.ProductId, this.OptionList);
                }
                return _ProductVariant;
            }
        }

        /// <summary>
        /// Indicates whether an inventory item has been destocked
        /// </summary>
        public InventoryStatus InventoryStatus
        {
            get { return (InventoryStatus)this.InventoryStatusId; }
            set { this.InventoryStatusId = (short)value; }
        }

        /// <summary>
        /// Indicates whether the item is a child of another item.
        /// </summary>
        public bool IsChildItem
        {
            get
            {
                return ((this.OrderItemId != this.ParentItemId) && (this.ParentItemId != 0));
            }
        }

        /// <summary>
        /// Indicates the type of order item.
        /// </summary>
        public OrderItemType OrderItemType
        {
            get { return (OrderItemType)this.OrderItemTypeId; }
            set { this.OrderItemTypeId = (short)value; }
        }

        /// <summary>
        /// Indicates whether an item is shippable.
        /// </summary>
        public Shippable Shippable
        {
            get { return (Shippable)this.ShippableId; }
            set { this.ShippableId = (byte)value; }
        }

        /// <summary>
        /// The total price of the line item (quantity * price)
        /// </summary>
        public LSDecimal ExtendedPrice
        {
            get { return Math.Round(this.Quantity * (Decimal)this.Price, 4); }
        }

        /// <summary>
        /// The total weight of the line item (quantity * weight)
        /// </summary>
        public LSDecimal ExtendedWeight
        {
            get { return Math.Round(this.Quantity * (Decimal)this.Weight, 2); }
        }

        /// <summary>
        /// Generates the subscriptions associated with this order item.
        /// </summary>
        /// <param name="activate">Indicates whether to activate the subscription.</param>
        /// <returns>An array of subscriptions created for the order</returns>
        public Subscription[] GenerateSubscriptions(bool activate)
        {
            System.Collections.Generic.List<Subscription> subscriptions = new System.Collections.Generic.List<Subscription>();
            Product product = this.Product;
            if (product != null)
            {
                SubscriptionPlan subscriptionPlan = product.SubscriptionPlan;
                if (subscriptionPlan != null)
                {
                    for (int counter = 0; counter < this.Quantity; counter++)
                    {
                        Subscription subscription = subscriptionPlan.Generate(this.OrderItemId, this.Order.UserId, activate);
                        if (subscription != null) subscriptions.Add(subscription);
                    }
                }
            }
            if (subscriptions.Count > 0) return subscriptions.ToArray();
            return null;
        }

        /// <summary>
        /// Generates the gift certificates associated with this order item.
        /// Does not activate the gift certificates.
        /// </summary>
        public void GenerateGiftCertificates()
        {
            GenerateGiftCertificates(false);
        }

        /// <summary>
        /// Generates the gift certificates associated with this order item.
        /// </summary>
        /// <param name="activate">Indicates whether to activate the gift certificates.</param>
        public void GenerateGiftCertificates(bool activate)
        {
            if ((this.Product != null) && (this.Product.IsGiftCertificate))
            {
                IGiftCertKeyProvider provider = new DefaultGiftCertKeyProvider();

                GiftCertificate gc;
                for (int counter = 0; counter < this.Quantity; counter++)
                {
                    gc = new GiftCertificate();
                    gc.Balance = this.Price;
                    gc.CreateDate = LocaleHelper.LocalNow;
                    gc.CreatedBy = this.Order.UserId;
                    int daysToExpire = this.Order.Store.Settings.GiftCertificateDaysToExpire;
                    if (daysToExpire > 0)
                    {
                        gc.ExpirationDate = LocaleHelper.LocalNow.AddDays(daysToExpire);
                    }
                    else
                    {
                        gc.ExpirationDate = DateTime.MinValue;
                    }
                    gc.Name = this.Product.Name;
                    gc.StoreId = Token.Instance.StoreId;
                    gc.OrderItemId = this.OrderItemId;

                    GiftCertificateTransaction trans = new GiftCertificateTransaction();
                    trans.Amount = gc.Balance;
                    trans.Description = "Gift certificate created.";
                    trans.OrderId = this.Order.OrderId;
                    trans.TransactionDate = LocaleHelper.LocalNow;
                    gc.Transactions.Add(trans);

                    if (activate)
                    {
                        gc.SerialNumber = provider.GenerateGiftCertificateKey();
                        trans = new GiftCertificateTransaction();
                        trans.Amount = gc.Balance;
                        trans.Description = "Gift certificate activated.";
                        trans.OrderId = this.Order.OrderId;
                        trans.TransactionDate = LocaleHelper.LocalNow;
                        gc.Transactions.Add(trans);
                    }
                    else
                    {
                        gc.SerialNumber = "";
                    }

                    gc.Save();
                }
            }
        }

        /// <summary>
        /// Create a copy of this order item
        /// </summary>
        /// <param name="orderItemId">Id of the order item to create copy of</param>
        /// <param name="deepCopy">If true order item inputs associated with the original order item are also copied</param>
        /// <returns></returns>
        public static OrderItem Copy(int orderItemId, bool deepCopy)
        {
            OrderItem item = OrderItemDataSource.Load(orderItemId, false);
            if (item == null) throw new ArgumentException("Invalid order item ID.", "orderItemId");
            if (deepCopy)
            {
                foreach (OrderItemInput input in item.Inputs)
                {
                    input.OrderItemInputId = 0;
                    input.OrderItemId = 0;
                }
            }
            item.OrderItemId = 0;
            return item;
        }

        /// <summary>
        /// Gets the parent item for this order item.
        /// </summary>
        /// <param name="recurse">If true, this method gets the top level parent for this item.  If false, only the immediate parent is returned.</param>
        /// <returns>Returns the OrderItem instance for the parent item.  If the current item has no parent, the current item is returned.</returns>
        public OrderItem GetParentItem(bool recurse)
        {
            List<int> itemPath = new List<int>();
            return InternalGetParentItem(this, recurse, itemPath);
        }

        /// <summary>
        /// Gets the path for this basket item
        /// </summary>
        /// <returns>A list of basket item IDs from top level item to this item.</returns>
        public List<int> GetPath()
        {
            List<int> path = new List<int>();
            OrderItem oi = InternalGetParentItem(this, true, path);
            return path;
        }

        /// <summary>
        /// Gets the parent item for the given basket item.
        /// </summary>
        /// <param name="item">Item for which to find the parent.</param>
        /// <param name="recurse">If true, this method gets the top level parent for this item.  If false, only the immediate parent is returned.</param>
        /// <param name="itemPath">List to track the current item path, to prevent recursive loops.</param>
        /// <returns>Returns the OrderItem instance for the parent item.  If the current item has no parent, the current item is returned.</returns>
        private static OrderItem InternalGetParentItem(OrderItem item, bool recurse, List<int> itemPath)
        {
            // IF PARENT ITEM INDICATED, LOOK FOR IT IN THE BASKET
            int parentItemId = item.ParentItemId;
            int OrderItemId = item.OrderItemId;
            itemPath.Insert(0, OrderItemId);
            if (parentItemId != OrderItemId)
            {
                Order order = item.Order;
                if (order != null)
                {
                    foreach (OrderItem otherItem in order.Items)
                    {
                        int otherItemId = otherItem.OrderItemId;
                        if (otherItemId == parentItemId)
                        {
                            // CHECK TO MAKE SURE WE ARE NOT IN A RECURSIVE LOOP
                            if (itemPath.Contains(otherItemId))
                            {
                                Logger.Error("Circular parent reference in order.  Path: " + AlwaysConvert.ToList(",", itemPath.ToArray()));
                                throw new CircularParentReference("Circular parent reference in order.  Path: " + AlwaysConvert.ToList(",", itemPath.ToArray()));
                            }

                            if (recurse) return InternalGetParentItem(otherItem, recurse, itemPath);
                            // NON-RECURSIVE, ADD THIS ITEM TO THE PATH AND RETURN
                            itemPath.Insert(0, otherItem.OrderItemId);
                            return otherItem;
                        }
                    }
                }
            }

            // NO PARENT OR NO PARENT FOUND, ENSURE PARENT ID IS CORRECTLY SET
            item.ParentItemId = OrderItemId;
            return item;
        }

        /// <summary>
        /// Indicates whether child products should be itemized on order invoices
        /// </summary>
        public bool ItemizeChildProducts
        {
            get
            {
                return (AlwaysConvert.ToInt(this.CustomFields.TryGetValue("ItemizeChildProducts")) == 1);
            }
            set
            {
                this.CustomFields["ItemizeChildProducts"] = value ? "1" : "0";
            }
        }

        /// <summary>
        /// Indicates whether this item should be hidden from the customer on invoices
        /// </summary>
        public bool IsHidden
        {
            get
            {
                return (AlwaysConvert.ToInt(this.CustomFields.TryGetValue("IsHidden")) == 1);
            }
            set
            {
                this.CustomFields["IsHidden"] = value ? "1" : "0";
            }
        }

        /// <summary>
        /// Generates a basket item from this order item
        /// </summary>
        /// <returns></returns>
        internal BasketItem GetBasketItem()
        {
            BasketItem item = new BasketItem();
            item.GiftMessage = this.GiftMessage;
            item.KitList = this.KitList;
            item.LineMessage = this.LineMessage;
            item.Name = this.Name;
            item.OptionList = this.OptionList;
            item.OrderBy = this.OrderBy;
            item.OrderItemTypeId = this.OrderItemTypeId;
            item.Price = this.Price;
            item.ProductId = this.ProductId;
            item.Quantity = this.Quantity;
            item.ShippableId = this.ShippableId;
            item.Sku = this.Sku;
            item.TaxAmount = this.TaxAmount;
            item.TaxCodeId = this.TaxCodeId;
            item.TaxRate = this.TaxRate;
            item.Weight = this.Weight;
            item.WishlistItemId = this.WishlistItemId;
            item.WrapStyleId = this.WrapStyleId;
            // COPY THE CUSTOM FIELDS
            foreach (KeyValuePair<string, string> customField in this.CustomFields)
            {
                item.CustomFields.Add(customField.Key, customField.Value);
            }

            return item;
        }
    }
}