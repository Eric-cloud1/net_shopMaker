using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MakerShop.Common;
using MakerShop.Orders;
using MakerShop.Products;
using MakerShop.Shipping;
using MakerShop.Taxes;
using MakerShop.Users;
using MakerShop.Utility;

public partial class ConLib_ShipMultiPage : System.Web.UI.UserControl
{
    public User _User;
    public Basket _Basket;
    public List<Address> _SortedAddresses;

    protected void Page_Load(object sender, EventArgs e)
    {
        // INIT LOCAL VARIABLES
        _User = Token.Instance.User;
        _Basket = _User.Basket;
        if (_Basket.Items.Count == 0) Response.Redirect(NavigationHelper.GetBasketUrl());
        if (!_Basket.Items.HasShippableProducts) Response.Redirect("Payment.aspx");


        //RECALCULATE BASKET FOR DISPLAY
        _Basket.Recalculate();
        BasketGrid.DataSource = GetShippableItems(true);
        BasketGrid.DataBind();
        BasketGrid.Columns[2].Visible = TaxHelper.ShowTaxColumn;
        
        // BIND SHIPPING ADDRESSES ON INIT SO THEY CAN PARTICIPATE IN POSTBACK EVENTS
        BindShippingAddresses();
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        // REBIND ADDRESSES ON PRE_RENDER SO THAT ANY CHANGES TO ADDRESS BOOK WILL BE INCLUDED
        BindShippingAddresses();
    }

    private void BindShippingAddresses()
    {
        //INITIALIZE ADDRESS COLLECTION
        _SortedAddresses = new List<Address>();
        foreach (Address a in _User.Addresses) _SortedAddresses.Add(a);
        _SortedAddresses.Sort(new AddressComparer(_User.PrimaryAddressId));
        //LOOP ITEMS PANEL, REBIND AVAILABLE SHIPPING ADDRESSES
        foreach (GridViewRow row in BasketGrid.Rows)
        {
            DropDownList shippingAddress = row.FindControl("ShippingAddress") as DropDownList;
            if (shippingAddress != null)
            {
                int basketItemId = (int)BasketGrid.DataKeys[row.DataItemIndex].Value;
                BasketItem basketItem = GetBasketItem(basketItemId);
                int addressId = ((basketItem != null) && (basketItem.BasketShipment != null)) ? basketItem.BasketShipment.AddressId : 0;
                BindShippingAddress(shippingAddress, addressId);
            }
        }
    }

    private void BindShippingAddress(DropDownList shippingAddress, int shippingAddressId)
    {
        //BIND (OR RE-BIND) SHIPPING ADDRESSES, PRESERVE CURRENT SELECTION
        if (Page.IsPostBack) shippingAddressId = AlwaysConvert.ToInt(Request.Form[shippingAddress.UniqueID]);
        shippingAddress.Items.Clear();
        foreach (Address address in _SortedAddresses)
        {
            string company = string.IsNullOrEmpty(address.Company) ? string.Empty : address.Company + " ";
            string tempAddress = company + address.FullName + " " + address.Address1;
            if (tempAddress.Length > 45) tempAddress = tempAddress.Substring(0, 42) + "...";
            string itemText = string.Format("{0} {1}", tempAddress, address.City);
            ListItem listItem = new ListItem(itemText, address.AddressId.ToString());
            if (address.AddressId == shippingAddressId) listItem.Selected = true;
            shippingAddress.Items.Add(listItem);
        }
        if (shippingAddress.SelectedIndex < 0) shippingAddress.SelectedIndex = 0;
    }

    protected string GetTaxHeader()
    {
        return TaxHelper.TaxColumnHeader;
    }

    /// <summary>
    /// Gets the ShippableItem list for the current basket
    /// </summary>
    /// <param name="multiplyQuantity">If true, creates a ShippableItem record for each quantity of the basket items.</param>
    /// <returns>A ShippableItem list for the current basket.</returns>
    private List<ShippableItem> GetShippableItems(bool multiplyQuantity)
    {
        List<ShippableItem> shippableItems = new List<ShippableItem>();
        // KEEP TRACK OF MASTER KIT PRODUCTS THAT ARE ADDED TO THE SHIPPABLE ITEMS LIST
        List<int> kitMasterItems = new List<int>();
        Basket basket = _Basket;
        basket.Items.Sort(new BasketItemComparer());
        foreach (BasketItem basketItem in basket.Items)
        {
            ShippableItem shippableItem = GetShippableItem(basketItem, kitMasterItems);
            if (shippableItem != null)
            {
                if (multiplyQuantity)
                {
                    // MUST DUPLICATE THE ITEM FOR EACH QUANTITY TO ALLOW MULTIPLE SHIPPING ADDRESSES
                    // IF THE ITEM IS A CHILD ITEM, ONLY DUPLICATE FOR PARENT QUANTITY
                    int shippableQuantity;
                    if (basketItem.IsChildItem) shippableQuantity = basketItem.GetParentItem(true).Quantity;
                    else shippableQuantity = basketItem.Quantity;
                    for (int i = 0; i < shippableQuantity; i++)
                    {
                        shippableItems.Add(shippableItem);
                    }
                }
                else
                {
                    shippableItems.Add(shippableItem);
                }
            }
        }
        return shippableItems;
    }

    private ShippableItem GetShippableItem(BasketItem basketItem, List<int> kitMasterItems)
    {
        // MAKE SURE THIS IS A SHIPPABLE PRODUCT
        if (basketItem.OrderItemType == OrderItemType.Product && basketItem.Shippable != Shippable.No)
        {
            if (!basketItem.IsChildItem)
            {
                // NOT A CHILD ITEM
                Product product = basketItem.Product;
                if (product.KitStatus == KitStatus.Master)
                {
                    // THIS IS A KIT MASTER PRODUCT, NO NEED TO CONTINUE IF WE ALREADY CREATED A SHIPPABLE ITEM
                    if (kitMasterItems.Contains(basketItem.BasketItemId)) return null;
                    // RECORD THAT WE ARE ADDING A KIT MASTER PRODUCT
                    kitMasterItems.Add(basketItem.BasketItemId);
                }

                // CREATE THE SHIPPABLE ITEM INSTANCE
                return new ShippableItem(basketItem);
            }

            // SPECIAL HANDLING FOR CHILD PRODUCTS (FROM KITS)
            BasketItem parentItem = basketItem.GetParentItem(true);
            Product parentProduct = parentItem.Product;

            // ENSURE THE PARENT PRODUCT IS A KIT
            if (parentProduct != null && parentProduct.KitStatus == KitStatus.Master)
            {
                // WE ONLY WANT A SINGLE SHIPPABLE ITEM FOR THE ROOT PRODUCT
                if (kitMasterItems.Contains(parentItem.BasketItemId)) return null;
                kitMasterItems.Add(parentItem.BasketItemId);
                return new ShippableItem(parentItem);
            }
        }
        return null;
    }

    protected BasketItem GetBasketItem(int basketItemId)
    {
        foreach (BasketItem basketItem in _Basket.Items)
            if (basketItem.BasketItemId == basketItemId) return basketItem;
        return null;
    }

    protected BasketItem GetBasketItem(object dataItem)
    {
        ShippableItem si = dataItem as ShippableItem;
        if (si != null) return GetBasketItem(si.BasketItemId);
        return null;
    }

    protected void ContinueButton_Click(object sender, EventArgs e)
    {
        List<ShippableItem> shippableItems = GetShippableItems(false);
        foreach (GridViewRow gridRow in BasketGrid.Rows)
        {
            if (gridRow.RowType == DataControlRowType.DataRow)
            {
                int basketItemId = (int)BasketGrid.DataKeys[gridRow.DataItemIndex].Value;
                DropDownList shippingAddress = (DropDownList)gridRow.FindControl("ShippingAddress");
                int addressId = AlwaysConvert.ToInt(shippingAddress.SelectedValue);
                int index = IndexOfShippableItem(shippableItems, basketItemId);
                if (index > -1)
                {
                    ShippableItem shippableItem = shippableItems[index];
                    if (shippableItem.Destinations.ContainsKey(addressId)) shippableItem.Destinations[addressId] += 1;
                    else shippableItem.Destinations[addressId] = 1;
                }
            }
        }
        //CLEAR ALL DESTINATIONS IN THE EXISTING SHIPMENTS
        ClearShipments();
        //NOW LOOP SHIPPABLE ITEMS AND ADJUST BASKET AS NEEDED
        RepackageBasket(shippableItems);
        Response.Redirect("ShipMethod.aspx");
    }

    protected void ClearShipments()
    {
        Basket basket = _Basket;
        //clear all existing shipments 
        foreach (BasketItem items in basket.Items)
        {
            items.BasketShipmentId = 0;
            items.Save();
        }
        //delete all shipments
        basket.Shipments.DeleteAll();
    }

    private void RepackageBasket(List<ShippableItem> shippableItems)
    {
        foreach (ShippableItem shippableItem in shippableItems)
        {
            int basketItemIndex = _Basket.Items.IndexOf(shippableItem.BasketItemId);
            if (basketItemIndex > -1 && shippableItem.Destinations.Count > 0)
            {
                BasketItem basketItem = _Basket.Items[basketItemIndex];
                int originalBasketItemQuantity = basketItem.Quantity;
                List<int> parentItemIds = RepackageBasketItem(basketItem, null, shippableItem, 1, null);

                // IF THIS SHIPPABLE ITEM IS A MASTER KIT
                // WE MUST LOOK FOR CHILD ITEMS AND ALSO SET THE DESTINATIONS ACCORDING TO THE MASTER ITEM SELECTIONS
                Product product = basketItem.Product;
                if (product != null && product.KitStatus == KitStatus.Master)
                {
                    // LOOP ALL ITEMS CURRENTLY IN BASKET TO LOCATE ANY CHILD PRODUCTS
                    int currentBasketItemCount = _Basket.Items.Count;
                    for (int i = 0; i < currentBasketItemCount; i++)
                    {
                        BasketItem testItem = _Basket.Items[i];
                        if (testItem.OrderItemType == OrderItemType.Product && testItem.Shippable != Shippable.No
                            && testItem.ParentItemId == basketItem.BasketItemId && testItem.IsChildItem)
                        {
                            // THIS IS A KIT MEMBER PRODUCT THAT MUST BE SPLIT ACCORDING TO THE PARENT ITEM
                            // WE CAN CALCULATE THE KIT QUANTITY (CHILD QUANTITY DIVIDIED BY PARENT QUANTITY)
                            int quantityMultiplier = testItem.Quantity / originalBasketItemQuantity;
                            RepackageBasketItem(testItem, parentItemIds, shippableItem, quantityMultiplier, product);
                        }
                    }
                }
            }
        }

        // AFTER REPACKAGING IN SOME CASES (KITS) NON-SHIPPABLE PRODUCTS MAY BE IN SHIPMENTS
        // WE MUST DO THIS IN ORDER TO GET THE SPLITS CORRECT, BUT NOW WE CAN FIX THE CONDITION
        foreach (BasketItem item in _Basket.Items)
        {
            if (item.Shippable == Shippable.No && item.BasketShipmentId != 0)
            {
                item.BasketShipmentId = 0;
                item.Save();
            }
        }
    }

    /// <summary>
    /// Determines the correct parent item id based on destination
    /// </summary>
    /// <param name="parentItemIds">A list of potential parent item ids</param>
    /// <param name="destinationAddressId">The desired destination</param>
    /// <param name="defaultValue">The default return value if the parent cannot be determined</param>
    /// <returns>The best parent item id to assign</returns>
    private int GetParentItemId(List<int> parentItemIds, int destinationAddressId, int defaultValue)
    {
        foreach (BasketItem item in _Basket.Items)
        {
            if (parentItemIds.Contains(item.BasketItemId))
            {
                BasketShipment shipment = GetShipment(item.Product.WarehouseId, destinationAddressId, false);
                if (shipment != null && shipment.BasketShipmentId == item.BasketShipmentId)
                {
                    return item.BasketItemId;
                }
            }
        }
        return defaultValue;
    }

    private List<int> RepackageBasketItem(BasketItem originalBasketItem, List<int> parentItemIds, ShippableItem shippableItem, int quantityMultiplier, Product kitMasterProduct)
    {
        List<int> newBasketItemIds = new List<int>();
        int i = 0;
        foreach (int addressId in shippableItem.Destinations.Keys)
        {
            int warehouseId;
            if (kitMasterProduct != null && !kitMasterProduct.Kit.ItemizeDisplay)
            {
                warehouseId = kitMasterProduct.WarehouseId;
            }
            else warehouseId = originalBasketItem.Product.WarehouseId;
            BasketShipment shipment = GetShipment(warehouseId, addressId, true);
            if (i == 0)
            {
                //ADJUST THE FIRST ITEM
                originalBasketItem.BasketShipmentId = shipment.BasketShipmentId;
                originalBasketItem.Quantity = (short)(shippableItem.Destinations[addressId] * quantityMultiplier);
                if (parentItemIds != null)
                {
                    originalBasketItem.ParentItemId = GetParentItemId(parentItemIds, addressId, originalBasketItem.ParentItemId);
                }
                originalBasketItem.Save();
                newBasketItemIds.Add(originalBasketItem.BasketItemId);
            }
            else
            {
                //CREATE ADDITIONAL ITEMS TO SPLIT DESTINATIONS
                BasketItem splitBasketItem = originalBasketItem.Clone();
                splitBasketItem.BasketId = _Basket.BasketId;
                splitBasketItem.Quantity = (short)(shippableItem.Destinations[addressId] * quantityMultiplier);
                splitBasketItem.BasketShipmentId = shipment.BasketShipmentId;
                if (parentItemIds != null)
                {
                    splitBasketItem.ParentItemId = GetParentItemId(parentItemIds, addressId, splitBasketItem.ParentItemId);
                }
                _Basket.Items.Add(splitBasketItem);
                splitBasketItem.Save();
                if (splitBasketItem.ParentItemId == 0)
                {
                    splitBasketItem.ParentItemId = splitBasketItem.BasketItemId;
                    splitBasketItem.Save();
                }
                newBasketItemIds.Add(splitBasketItem.BasketItemId);
            }
            i++;
        }
        return newBasketItemIds;
    }

    protected BasketShipment GetShipment(int warehouseId, int addressId, bool create)
    {
        Basket basket = _Basket;
        foreach (BasketShipment shipment in basket.Shipments)
        {
            if ((shipment.WarehouseId == warehouseId) || (shipment.WarehouseId == 0))
            {
                if ((shipment.AddressId == addressId) || (shipment.AddressId == 0))
                {
                    shipment.WarehouseId = warehouseId;
                    shipment.AddressId = addressId;
                    return shipment;
                }
            }
        }
        if (!create) return null;

        //IF WE COME THIS FAR, AN EXISTING SHIPMENT WAS NOT AVAILABLE
        //ADD A NEW SHIPMENT
        BasketShipment newShipment = new BasketShipment();
        newShipment.BasketId = basket.BasketId;
        newShipment.WarehouseId = warehouseId;
        newShipment.AddressId = addressId;
        basket.Shipments.Add(newShipment);
        newShipment.Save();
        return newShipment;
    }

    private static int IndexOfShippableItem(List<ShippableItem> shippableItems, int basketItemId)
    {
        int index = 0;
        foreach (ShippableItem item in shippableItems)
        {
            if (item.BasketItemId == basketItemId) return index;
            index++;
        }
        return -1;
    }

    public class ShippableItem
    {
        private int _BasketItemId;
        private string _Name;
        private LSDecimal _Price;
        private Dictionary<int, short> _Destinations;
        public int BasketItemId { get { return _BasketItemId; } }
        public string Name { get { return _Name; } }
        public LSDecimal Price { get { return _Price; } }
        public Dictionary<int, short> Destinations { get { return _Destinations; } }
        public ShippableItem(BasketItem basketItem)
        {
            _BasketItemId = basketItem.BasketItemId;
            _Name = basketItem.Name;
            _Price = basketItem.Price;
            _Destinations = new Dictionary<int, short>();
        }
    }
    
    private class AddressComparer : IComparer, IComparer<Address>
    {
        private int _PrimaryAddressId;
        public AddressComparer(int primaryAddressId)
        {
            _PrimaryAddressId = primaryAddressId;
        }
        public int Compare(object x, object y)
        {
            return this.Compare(x as Address, y as Address);
        }
        public int Compare(Address x, Address y)
        {
            if (x.AddressId == y.AddressId) return 0;
            if (x.AddressId == _PrimaryAddressId) return -1;
            if (y.AddressId == _PrimaryAddressId) return 1;
            string nX = (string.IsNullOrEmpty(x.Company) ? string.Empty : x.Company + " ") + x.FullName;
            string nY = (string.IsNullOrEmpty(y.Company) ? string.Empty : y.Company + " ") + y.FullName;
            return string.Compare(nX, nY);
        }
    }
}