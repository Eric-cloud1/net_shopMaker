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
using MakerShop.DigitalDelivery;
using MakerShop.Orders; 
using MakerShop.Products;
using MakerShop.Taxes;
using MakerShop.Users;
using MakerShop.Utility;
using MakerShop.Payments;

public partial class ConLib_BuyProductDialogOptionsList : System.Web.UI.UserControl
{
    int _ProductId = 0;
    Product _Product = null;
    List<int> _SelectedKitProducts = null;
    ProductVariantManager _VariantManager;
    PersistentCollection<ProductVariant> _AvailableVariants;
    DataTable _DataTable;
    ProductOptionCollection _ProdOptions;

    private bool _ShowSku = true;
    public bool ShowSku
    {
        get { return _ShowSku; }
        set { _ShowSku = value; }
    }

    private bool _ShowPartNumber = false;
    /// <summary>
    ///  Indicates whether the Part/Model Number will be shown or not.
    /// </summary>
    [Personalizable, WebBrowsable]
    public bool ShowPartNumber
    {
        get { return _ShowPartNumber; }
        set { _ShowPartNumber = value; }
    }

    private bool _ShowPrice = true;
    public bool ShowPrice
    {
        get { return _ShowPrice; }
        set { _ShowPrice = value; }
    }

    private bool _ShowSubscription = true;
    public bool ShowSubscription
    {
        get { return _ShowSubscription; }
        set { _ShowSubscription = value; }
    }

    private bool _ShowMSRP = true;
    public bool ShowMSRP
    {
        get { return _ShowMSRP; }
        set { _ShowMSRP = value; }
    }

    protected void VariantGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        string optionList;
        if (e.CommandName.Equals("AddToBasket"))
        {
            optionList = e.CommandArgument.ToString();
            ProductVariant variant = _VariantManager.GetVariantFromOptions(optionList);
            if (variant != null)
            {
                BasketItem basketItem = GetBasketItem(optionList, e);
                if (basketItem != null)
                {
                    // DETERMINE IF THE LICENSE AGREEMENT MUST BE REQUESTED
                    BasketItemLicenseAgreementCollection basketItemLicenseAgreements = new BasketItemLicenseAgreementCollection(basketItem, LicenseAgreementMode.OnAddToBasket);
                    if ((basketItemLicenseAgreements.Count > 0))
                    {
                        // THESE AGREEMENTS MUST BE ACCEPTED TO ADD TO CART
                        List<BasketItem> basketItems = new List<BasketItem>();
                        basketItems.Add(basketItem);
                        string guidKey = Guid.NewGuid().ToString("N");
                        Cache.Add(guidKey, basketItems, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 10, 0), System.Web.Caching.CacheItemPriority.NotRemovable, null);
                        string acceptUrl = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("~/Basket.aspx"));
                        string declineUrl = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Page.ResolveClientUrl(_Product.NavigateUrl)));
                        Response.Redirect("~/BuyWithAgreement.aspx?Items=" + guidKey + "&AcceptUrl=" + acceptUrl + "&DeclineUrl=" + declineUrl);
                    }

                    //ADD ITEM TO BASKET
                    Basket basket = Token.Instance.User.Basket;
                    basket.Items.Add(basketItem);
                    basket.Package(true);
                    basket.Save();

                    //Determine if there are associated Upsell products
                    if (basketItem.Product.GetUpsellProducts(basket).Count > 0)
                    {
                        //redirect to upsell page
                        string returnUrl = NavigationHelper.GetEncodedReturnUrl();
                        Response.Redirect("~/ProductAccessories.aspx?ProductId=" + basketItem.ProductId + "&ReturnUrl=" + returnUrl);
                    }

                    // IF BASKET HAVE SOME VALIDATION PROBLEMS MOVE TO BASKET PAGE
                    List<string> basketMessages;
                    if (!basket.Validate(out basketMessages))
                    {
                        Session.Add("BasketMessage", basketMessages);
                        Response.Redirect(NavigationHelper.GetBasketUrl());
                    }

                    //IF THERE IS NO REGISTERED BASKET CONTROL, WE MUST GO TO BASKET PAGE
                    if (!PageHelper.HasBasketControl(this.Page)) Response.Redirect(NavigationHelper.GetBasketUrl());
                }
            }
        }
        else if (e.CommandName.Equals("AddToWishlist"))
        {
            optionList = e.CommandArgument.ToString();
            ProductVariant variant = _VariantManager.GetVariantFromOptions(optionList);
            if (variant != null)
            {
                BasketItem wishlistItem = GetBasketItem(optionList, e);
                if (wishlistItem != null)
                {
                    Wishlist wishlist = Token.Instance.User.PrimaryWishlist;
                    wishlist.Items.Add(wishlistItem);
                    wishlist.Save();
                    Response.Redirect("~/Members/MyWishlist.aspx");
                }
            }
        }
    }

    protected BasketItem GetBasketItem(string optionList, GridViewCommandEventArgs e)
    {
        //GET THE QUANTITY
        GridViewRow row = (GridViewRow)((Control)e.CommandSource).Parent.Parent;
        int tempQuantity = GetControlValue(row, "Quantity", 1);
        if (tempQuantity < 1) return null;
		if (tempQuantity > System.Int16.MaxValue) tempQuantity = System.Int16.MaxValue;

        //CREATE THE BASKET ITEM WITH GIVEN OPTIONS
        BasketItem basketItem = BasketItemDataSource.CreateForProduct(_ProductId, (short)tempQuantity, optionList, AlwaysConvert.ToList(",", _SelectedKitProducts), PaymentTypes.Initial);
        if (basketItem != null)
        {
            //ADD IN VARIABLE PRICE
            //if (_Product.UseVariablePrice) basketItem.Price = AlwaysConvert.ToDecimal(VariablePrice.Text);
            // COLLECT ANY ADDITIONAL INPUTS
            ProductHelper.CollectProductTemplateInput(basketItem, this);
        }
        return basketItem;
    }

    private int GetControlValue(GridViewRow row, string controlId, int defaultValue)
    {
        TextBox tb = row.FindControl(controlId) as TextBox;
        if (tb != null)
        {
            return AlwaysConvert.ToInt(tb.Text,defaultValue);
        }
        return 0;
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
        _Product = ProductDataSource.Load(_ProductId);
        if (_Product != null)
        {
            _VariantManager = new ProductVariantManager(_ProductId);
            _AvailableVariants = _VariantManager.LoadAvailableVariantGrid();
            CreateDynamicGrid();
        }
        else
        {
            this.Controls.Clear();
        }
    }

    private void CreateDynamicGrid()
    {
        if (_AvailableVariants == null) return;
        _DataTable = new DataTable();

        DataColumn dcol;
        dcol = new DataColumn("OptionList", typeof(System.String));
        _DataTable.Columns.Add(dcol);

        if (ShowSku)
        {
            dcol = new DataColumn("Sku", typeof(System.String));
            _DataTable.Columns.Add(dcol);
        }

        if (ShowPartNumber)
        {
            dcol = new DataColumn("PartNumber", typeof(System.String));
            _DataTable.Columns.Add(dcol);
        }

        

        if (_ProdOptions == null) _ProdOptions = _Product.ProductOptions;
        foreach (ProductOption option in _ProdOptions)
        {
            dcol = new DataColumn(option.Option.Name, typeof(System.String));
            _DataTable.Columns.Add(dcol);
        }

        if (ShowPrice)
        {
            dcol = new DataColumn("Price", typeof(System.String));
            _DataTable.Columns.Add(dcol);
        }

        DataRow drow;
        foreach (ProductVariant variant in _AvailableVariants)
        {
            drow = _DataTable.NewRow();
            drow["OptionList"] = variant.OptionList;
            if (ShowSku) drow["Sku"] = variant.Sku;

            if (ShowPartNumber) drow["PartNumber"] = _Product.ModelNumber;

            if (ShowPrice) drow["Price"] = variant.Price.ToString("ulc");
            foreach (ProductOption option in _ProdOptions)
            {
                drow[option.Option.Name] = variant.GetSelectedOptionChoiceName(option.Option.Choices);
            }
            _DataTable.Rows.Add(drow);
        }

        TemplateField tf;
        if (ShowSku)
        {
            tf = new TemplateField();
            tf.HeaderTemplate = new LabelTemplate(DataControlRowType.Header, "Model");
            tf.ItemTemplate = new LabelTemplate(DataControlRowType.DataRow, "Sku");
            VariantGrid.Columns.Add(tf);
        }

        if (ShowPartNumber)
        {
            tf = new TemplateField();
            tf.HeaderTemplate = new LabelTemplate(DataControlRowType.Header, "Part&nbsp;#");
            tf.ItemTemplate = new LabelTemplate(DataControlRowType.DataRow, "PartNumber");
            VariantGrid.Columns.Add(tf);
        }

        if (_ProdOptions == null) _ProdOptions = _Product.ProductOptions;
        foreach (ProductOption option in _ProdOptions)
        {
            tf = new TemplateField();
            tf.HeaderTemplate = new LabelTemplate(DataControlRowType.Header, option.Option.Name);
            tf.ItemTemplate = new LabelTemplate(DataControlRowType.DataRow, option.Option.Name);
            VariantGrid.Columns.Add(tf);
        }

        tf = new TemplateField();
        tf.HeaderTemplate = new QtyBoxTemplate(DataControlRowType.Header, "Qty");
        tf.ItemTemplate = new QtyBoxTemplate(DataControlRowType.DataRow, "Quantity");
        VariantGrid.Columns.Add(tf);

        if (ShowPrice)
        {
            tf = new TemplateField();
            tf.HeaderTemplate = new PriceLabelTemplate(DataControlRowType.Header, "Price", _ProductId);
            tf.ItemTemplate = new PriceLabelTemplate(DataControlRowType.DataRow, "Price", _ProductId);
            VariantGrid.Columns.Add(tf);
        }

        tf = new TemplateField();
        tf.HeaderTemplate = new ButtonsBoxTemplate(DataControlRowType.Header, "");
        tf.ItemTemplate = new ButtonsBoxTemplate(DataControlRowType.DataRow, "");
        VariantGrid.Columns.Add(tf);

        //Initialize the DataSource
        VariantGrid.DataSource = _DataTable;
        //Bind the datatable with the GridView.
        VariantGrid.DataBind();
    }

    private class LabelTemplate : ITemplate
    {
        private DataControlRowType templateType;
        private string columnName;

        public LabelTemplate(DataControlRowType type, string colname)
        {
            templateType = type;
            columnName = colname;
        }

        public void InstantiateIn(System.Web.UI.Control container)
        {
            switch (templateType)
            {
                case DataControlRowType.Header:
                    // build the header for this column
                    Literal lc = new Literal();
                    //lc.Text = "<b>" + BreakCamelCase(columnName) + "</b>";
                    lc.Text = "<b>" + columnName + "</b>";
                    container.Controls.Add(lc);
                    break;
                case DataControlRowType.DataRow:
                    // build one row in this column
                    Label l = new Label();
                    // register an event handler to perform the data binding
                    l.DataBinding += new EventHandler(this.l_DataBinding);
                    container.Controls.Add(l);
                    break;
                default:
                    break;
            }
        }

        private void l_DataBinding(Object sender, EventArgs e)
        {
            // get the control that raised this event
            Label l = (Label)sender;
            // get the containing row
            GridViewRow row = (GridViewRow)l.NamingContainer;
            // get the raw data value and make it pretty
            string RawValue = DataBinder.Eval(row.DataItem, columnName).ToString();
            l.Text = RawValue;
        }
    }

    private class QtyBoxTemplate : System.Web.UI.ITemplate
    {
        //MakerShop.Web.UI.WebControls.UpDownControl 
        private DataControlRowType templateType;
        private string columnName;

        public QtyBoxTemplate(DataControlRowType type, string colname)
        {
            templateType = type;
            columnName = colname;
        }

        public void InstantiateIn(System.Web.UI.Control container)
        {
            switch (templateType)
            {
                case DataControlRowType.Header:
                    // build the header for this column
                    Literal lc = new Literal();
                    //lc.Text = "<b>" + BreakCamelCase(columnName) + "</b>";
                    lc.Text = "<b>" + columnName + "</b>";
                    container.Controls.Add(lc);
                    break;
                case DataControlRowType.DataRow:
                    MakerShop.Web.UI.WebControls.UpDownControl updown = new MakerShop.Web.UI.WebControls.UpDownControl();
                    updown.Width = new Unit(30);
                    updown.ID = "Quantity";
                    updown.DownImageUrl = "~/images/down.gif";
                    updown.UpImageUrl = "~/images/up.gif";
                    updown.Columns = 2;
                    updown.MaxLength = 5;
					updown.MaxValue = System.Int16.MaxValue;
                    updown.Text = "1";
                    updown.Attributes.Add("onFocus", "this.select()");
                    container.Controls.Add(updown);
                    break;
                default:
                    break;
            }
        }
    }

    private class PriceLabelTemplate : ITemplate
    {
        private DataControlRowType templateType;
        private string columnName;
        private int _ProductId;

        public PriceLabelTemplate(DataControlRowType type, string colname, int productId)
        {
            templateType = type;
            columnName = colname;
            _ProductId = productId;
        }

        public void InstantiateIn(System.Web.UI.Control container)
        {
            switch (templateType)
            {
                case DataControlRowType.Header:
                    // build the header for this column
                    Literal lc = new Literal();
                    //lc.Text = "<b>" + BreakCamelCase(columnName) + "</b>";
                    lc.Text = "<b>" + columnName + "</b>";
                    container.Controls.Add(lc);
                    break;
                case DataControlRowType.DataRow:
                    // build one row in this column
                    Label l = new Label();
                    // register an event handler to perform the data binding
                    l.DataBinding += new EventHandler(this.l_DataBinding);
                    container.Controls.Add(l);
                    break;
                default:
                    break;
            }
        }

        private void l_DataBinding(Object sender, EventArgs e)
        {
            // get the control that raised this event
            Label l = (Label)sender;
            // get the containing row
            GridViewRow row = (GridViewRow)l.NamingContainer;
            // get the raw data value and make it pretty            
            string optionList = DataBinder.Eval(row.DataItem, "OptionList").ToString();
            l.Text = GetVariantPrice(optionList);
        }

        protected string GetVariantPrice(string optionList)
        {
            if (!string.IsNullOrEmpty(optionList))
            {
                Product p = ProductDataSource.Load(_ProductId);
                ProductCalculator pcalc = ProductCalculator.LoadForProduct(_ProductId, 1, optionList, string.Empty, PaymentTypes.Initial);
                return TaxHelper.GetShopPrice(pcalc.Price, p.TaxCodeId).ToString("ulc");
            }
            return "";
        }
    }

    private class ButtonsBoxTemplate : System.Web.UI.ITemplate
    {
        //MakerShop.Web.UI.WebControls.UpDownControl 
        private DataControlRowType templateType;
        private string columnName;

        public ButtonsBoxTemplate(DataControlRowType type, string colname)
        {
            templateType = type;
            columnName = colname;
        }

        public void InstantiateIn(System.Web.UI.Control container)
        {
            switch (templateType)
            {
                case DataControlRowType.Header:
                    // build the header for this column
                    Literal lc = new Literal();
                    //lc.Text = "<b>" + BreakCamelCase(columnName) + "</b>";
                    lc.Text = "<b>" + columnName + "</b>";
                    container.Controls.Add(lc);
                    break;
                case DataControlRowType.DataRow:
                    LinkButton lb1 = new LinkButton();
                    lb1.ID = "AddToBasketButton";
                    lb1.CommandName = "AddToBasket";
                    lb1.SkinID = "Button";
                    lb1.Text = "Add to Cart";
                    lb1.EnableViewState = false;
                    lb1.ValidationGroup = "AddToBasket";
                    lb1.DataBinding += new EventHandler(this.lb_DataBinding);
                    container.Controls.Add(lb1);

                    LinkButton lb2 = new LinkButton();
                    lb2.ID = "AddToWishlistButton";
                    lb2.CommandName = "AddToWishlist";
                    lb2.SkinID = "Button";
                    lb2.Text = "Add to Wishlist";
                    lb2.EnableViewState = false;
                    lb2.ValidationGroup = "AddToBasket";
                    lb2.DataBinding += new EventHandler(this.lb_DataBinding);
                    container.Controls.Add(lb2);
                    break;
                default:
                    break;
            }
        }

        private void lb_DataBinding(Object sender, EventArgs e)
        {
            // get the control that raised this event
            LinkButton l = (LinkButton)sender;
            // get the containing row
            GridViewRow row = (GridViewRow)l.NamingContainer;
            // get the raw data value 
            string optionList = DataBinder.Eval(row.DataItem, "OptionList").ToString();
            l.CommandArgument = optionList;
        }
    }
}