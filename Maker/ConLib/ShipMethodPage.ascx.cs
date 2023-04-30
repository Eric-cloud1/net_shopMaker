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
using MakerShop.Products;
using MakerShop.Orders;
using MakerShop.Taxes;
using MakerShop.Shipping;
using MakerShop.Utility;

public partial class ConLib_ShipMethodPage : System.Web.UI.UserControl
{
    private string weightFormat;
    
    private bool _ShowWeight = true;
    public bool ShowWeight
    {
        get { return _ShowWeight; }
        set { _ShowWeight = value; }
    }

    private bool ShowGiftOptionsPanel()
    {
        foreach (BasketItem item in Token.Instance.User.Basket.Items)
        {
            if (!item.IsChildItem && item.Shippable != Shippable.No) return true;
        }
        return false;
    }

    private bool HasGiftOptions()
    {
        foreach (BasketItem item in Token.Instance.User.Basket.Items)
        {
            if ((item.WrapStyleId != 0) || (item.GiftMessage.Length > 0)) return true;
        }
        return false;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        Basket basket = Token.Instance.User.Basket;
        if (basket.Items.Count == 0) Response.Redirect(NavigationHelper.GetBasketUrl());
        if (!basket.Items.HasShippableProducts) Response.Redirect("Payment.aspx");
        basket.Recalculate();
        weightFormat = "{0:0.##} " + Token.Instance.Store.WeightUnit;
        if (!Page.IsPostBack)
        {
            ShipmentRepeater.DataSource = Token.Instance.User.Basket.Shipments;
            ShipmentRepeater.DataBind();
            GiftOptionsPanel.Visible = ShowGiftOptionsPanel();
            if (GiftOptionsPanel.Visible)
            {
                ShowGiftOptions.Checked = HasGiftOptions();
            }
        }

        foreach (RepeaterItem item in ShipmentRepeater.Items)
        {
            if (item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.Item)
            {
                Control msgBox = item.FindControl("ShipMessage");
                Control msgBoxLbl = item.FindControl("ShipMessageCount");
                if (msgBox != null && msgBoxLbl != null)
                {
                    PageHelper.SetMaxLengthCountDown(msgBox as TextBox, msgBoxLbl as Label);
                }
            }

            // SHOW TAX COLUMN WHEN NEEDED
            GridView ShipmentItemsGrid = (GridView)item.FindControl("ShipmentItemsGrid");
            if (ShipmentItemsGrid != null)
            {
                DataControlField taxColumn = ShipmentItemsGrid.Columns[3];
                taxColumn.Visible = TaxHelper.ShowTaxColumn;
            }
        }
    }

    //THIS VARIABLE TRACKS SHIPPING INDEX
    //TO HELP BUILD RADIO BUTTONS
    private int _ShipmentIndex;
    protected void ShipmentRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        //CAST DATA ITEM
        BasketShipment shipment = (BasketShipment)e.Item.DataItem;
        //UPDATE CAPTION
        Localize shipmentCaption = (Localize)e.Item.FindControl("ShipmentCaption");
        if (shipmentCaption != null)
        {
            shipmentCaption.Text = string.Format(shipmentCaption.Text, shipment.Address.ToString(false));
        }
        //UPDATE SHIPPING WEIGHT
        Literal shipWeight = (Literal)e.Item.FindControl("ShipWeight");
        if (shipWeight != null)
        {
            shipWeight.Text = string.Format(weightFormat, shipment.Items.TotalWeight());
        }
        //SHOW SHIPPING METHODS
        GridView ShipMethodGrid = (GridView)e.Item.FindControl("ShipMethodGrid");
        if (ShipMethodGrid != null)
        {
            _ShipmentIndex = e.Item.ItemIndex;
            ICollection<ShipRateQuote> rateQuotes = ShipRateQuoteDataSource.QuoteForShipment(shipment);
            ShipMethodGrid.DataSource = rateQuotes;
            ShipMethodGrid.DataBind();
            if (rateQuotes.Count == 0)
            {
                NoShippingMethodMessage.Visible = true;
                ContinueButton.Visible = false;
            }
            else
            {
                // IN CASE WE HAVE DISABLED THE CONTINUE BUTTON BEFORE
                ContinueButton.Visible = true;
            }
        }
    }

    protected void ShipMethodGrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            PlaceHolder phRadio = e.Row.FindControl("phRadio") as PlaceHolder;
            if (phRadio != null)
            {
                string selectedText = (e.Row.DataItemIndex == 0) ? " checked" : string.Empty;
                int shipMethodId = ((ShipRateQuote)e.Row.DataItem).ShipMethodId;
                phRadio.Controls.Add(new LiteralControl("<input type=\"radio\" value=\"" + shipMethodId + "\" name=\"ShipMethod" + _ShipmentIndex.ToString() + "\"" + selectedText + " />"));
            }
        }
    }

    protected void ContinueButton_Click(object sender, EventArgs e)
    {
        //LOOP SHIPMENTS, GET SHIPPING METHODS
        Basket basket = Token.Instance.User.Basket;
        BasketShipmentCollection shipments = basket.Shipments;
        _ShipmentIndex = 0;
        bool allMethodsValid = true;
        foreach (BasketShipment shipment in shipments)
        {
            shipment.ShipMethodId = AlwaysConvert.ToInt(Request.Form["ShipMethod" + _ShipmentIndex]);
            TextBox shipMessage = ShipmentRepeater.Items[_ShipmentIndex].FindControl("ShipMessage") as TextBox;
            if (shipMessage != null && !string.IsNullOrEmpty(shipMessage.Text))
            {
                shipMessage.Text = StringHelper.StripHtml(shipMessage.Text.Trim());
                if (shipMessage.Text.Length > 255)
                {
                    shipMessage.Text = shipMessage.Text.Substring(0, 255);
                }
                shipment.ShipMessage = shipMessage.Text;
            }
            else
            {
                shipment.ShipMessage = "";
            }
            shipment.Save();
            if (shipment.ShipMethod == null) allMethodsValid = false;
            _ShipmentIndex++;
        }
        if (allMethodsValid)
        {
            if (ShowGiftOptions.Checked) Response.Redirect("GiftOptions.aspx?ReturnUrl=Payment.aspx");
            basket.ResetGiftOptions(true);
            Response.Redirect("Payment.aspx");
        }
        else
        {
            //HANDLE ERROR MESSAGE (UNEXPECTED)
            InvalidShipMethodMessage.Visible = true;
            //throw new Exception("Invalid shipping method selected.");
        }
    }

    protected string GetTaxHeader()
    {
        return TaxHelper.TaxColumnHeader;
    }
}