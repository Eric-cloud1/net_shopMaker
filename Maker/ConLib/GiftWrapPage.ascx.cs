using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using MakerShop.Common;
using MakerShop.Orders;
using MakerShop.Taxes;

public partial class ConLib_GiftWrapPage : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ShipmentRepeater.DataSource = Token.Instance.User.Basket.Shipments;
        ShipmentRepeater.DataBind();
    }

    protected void ShipmentRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        GridView itemsGrid = PageHelper.RecursiveFindControl(e.Item, "ShipmentItemsGrid") as GridView;
        if (itemsGrid != null)
        {
            itemsGrid.Columns[3].Visible = TaxHelper.ShowTaxColumn;
        }
    }

    protected string GetTaxHeader()
    {
        return TaxHelper.TaxColumnHeader;
    }

    protected BasketItemCollection GetShipmentItems(object dataItem)
    {
        BasketShipment shipment = (BasketShipment)dataItem;
        BasketItemCollection singleItems = new BasketItemCollection();
        foreach (BasketItem item in shipment.Items)
        {
            if (item.OrderItemType == OrderItemType.Product)
            {
                if (!item.IsChildItem || item.Product.KitStatus != MakerShop.Products.KitStatus.Member)
                {
                    for (int i = 1; (i <= item.Quantity); i++)
                    {
                        BasketItem singleItem = item.Clone();
                        singleItem.BasketItemId = item.BasketItemId;
                        singleItem.TaxRate = item.TaxRate;
                        singleItem.TaxAmount = item.TaxAmount / item.Quantity;
                        singleItem.Quantity = 1;
                        singleItem.KitList = item.KitList;
                        singleItems.Add(singleItem);
                    }
                }
            }
        }
        return singleItems;
    }

    protected void ShipmentItemsGrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ConLib_Utility_GiftWrapOptions opts = e.Row.FindControl("GiftWrapOptions1") as ConLib_Utility_GiftWrapOptions;
            if (opts != null)
            {
                opts.CreateControls();
            }
        }
    }

    protected void ContinueButton_Click(object sender, EventArgs e)
    {
        Basket basket = Token.Instance.User.Basket;
        List<BasketItemGiftOption> giftOptions = new List<BasketItemGiftOption>();

        // LOOP EACH SHIPMENT
        foreach (RepeaterItem shipmentRepeaterItem in ShipmentRepeater.Items)
        {
            // GET THE SHIPMENT ITEM GRID
            GridView ShipmentItemsGrid = (GridView)shipmentRepeaterItem.FindControl("ShipmentItemsGrid");
            // LOOP EACH SHIPMENT ITEM
            foreach (GridViewRow row in ShipmentItemsGrid.Rows)
            {
                int basketItemId = (int)ShipmentItemsGrid.DataKeys[row.DataItemIndex].Value;
                int basketItemIndex = basket.Items.IndexOf(basketItemId);
                if (basketItemIndex > -1)
                {
                    BasketItem basketItem = basket.Items[basketItemIndex];
                    ConLib_Utility_GiftWrapOptions wrapOptions = (ConLib_Utility_GiftWrapOptions)row.FindControl("GiftWrapOptions1");
                    int wrapStyleId = wrapOptions.WrapStyleId;
                    string giftMessage = wrapOptions.GiftMessage;
                    BasketItemGiftOption optionItem = new BasketItemGiftOption(basketItemId, wrapStyleId, giftMessage, 1);
                    int existingIndex = giftOptions.IndexOf(optionItem);
                    if (existingIndex > -1) giftOptions[existingIndex].Quantity++;
                    else giftOptions.Add(optionItem);
                }
            }
        }

        // LOOP THROUGH GIFT OPTIONS AND UPDATE BASKET ITEMS AS NEEDED
        List<int> splitItems = new List<int>();
        foreach (BasketItemGiftOption giftOptionItem in giftOptions)
        {
            int basketItemId = giftOptionItem.BasketItemId;
            int basketItemIndex = basket.Items.IndexOf(basketItemId);
            if ((basketItemIndex > -1))
            {
                BasketItem basketItem = basket.Items[basketItemIndex];
                bool isSplit = (splitItems.IndexOf(basketItemId) > -1);
                if (isSplit || (basketItem.Quantity != giftOptionItem.Quantity))
                {
                    //THIS IS A SPLIT ITEM
                    //FOR THE FIRST TIME THIS CODE IS RUN WE WANT TO UPDATE THE EXISTING BASKET ITEM
                    //SUBSEQUENT RUNS OF THIS CODE WE WANT TO ADD A NEW BASKET ITEM
                    BasketItem newItem;
                    if (isSplit) newItem = basketItem.Clone();
                    else newItem = basketItem;
                    newItem.Quantity = giftOptionItem.Quantity;
                    newItem.GiftMessage = giftOptionItem.GiftMessage;
                    newItem.WrapStyleId = giftOptionItem.WrapStyleId;
                    newItem.Save();
                    if (isSplit) basket.Items.Add(newItem);
                    else splitItems.Add(basketItemId);
                }
                else
                {
                    // NOT A SPLIT ITEM, UPDATE GIFT OPTIONS AS NEEDED
                    basketItem.WrapStyleId = giftOptionItem.WrapStyleId;
                    basketItem.GiftMessage = giftOptionItem.GiftMessage;
                    basketItem.Save();
                }
            }
        }
        // NOW REDIRECT TO RETURN URL
        Response.Redirect(NavigationHelper.GetReturnUrl("Payment.aspx"));
    }

    /// <summary>
    /// Used to divide up the basket items with differing gift options.
    /// </summary>
    private class BasketItemGiftOption
    {
        private int _BasketItemId;
        private int _WrapStyleId;
        private string _GiftMessage;
        private short _Quantity;

        public BasketItemGiftOption(int basketItemId, int wrapStyleId, string giftMessage, short quantity)
        {
            this._BasketItemId = basketItemId;
            this._WrapStyleId = wrapStyleId;
            this._GiftMessage = giftMessage;
            this._Quantity = quantity;
        }

        public int BasketItemId
        {
            get { return _BasketItemId; }
            set { _BasketItemId = value; }
        }

        public int WrapStyleId
        {
            get { return _WrapStyleId; }
            set { _WrapStyleId = value; }
        }

        public string GiftMessage
        {
            get { return _GiftMessage; }
            set { _GiftMessage = value; }
        }

        public short Quantity
        {
            get { return _Quantity; }
            set { _Quantity = value; }
        }

        public override bool Equals(object obj)
        {
            BasketItemGiftOption other = obj as BasketItemGiftOption;
            if (other != null)
            {
                return (this.BasketItemId.Equals(other.BasketItemId) && (this.WrapStyleId.Equals(other.WrapStyleId) && this.GiftMessage.Equals(other.GiftMessage)));
            }
            // FALL THROUGH TO BASE IF OTHER OBJECT IS NOT A BASKETITEM
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
