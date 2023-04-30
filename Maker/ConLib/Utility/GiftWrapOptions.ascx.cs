using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using MakerShop.Common;
using MakerShop.Orders;
using MakerShop.Products;
using MakerShop.Stores;
using MakerShop.Taxes;
using MakerShop.Utility;
using MakerShop.Users;

public partial class ConLib_Utility_GiftWrapOptions : System.Web.UI.UserControl
{
    private string _FreeFormat = "{0}";
    private string _CostFormat = "{0} {1:ulc}";
    private int _WrapStyleId = 0;
    private WrapStyleCollection _WrapStyles;

    public int BasketItemId
    {
        get
        {
            if (ViewState["BasketItemId"] == null) return 0;
            return (int)ViewState["BasketItemId"];
        }
        set { ViewState["BasketItemId"] = value; }
    }

    public int WrapGroupId
    {
        get
        {
            if (ViewState["WrapGroupId"] == null) return 0;
            return (int)ViewState["WrapGroupId"];
        }
        set { ViewState["WrapGroupId"] = value; }
    }

    public int WrapStyleId
    {
        get
        {
            if (NoGiftWrapPanel.Visible) return 0;
            if (OneGiftWrapPanel.Visible)
            {
                if (!AddGiftWrap.Checked) return 0;
                return AlwaysConvert.ToInt(AddGiftWrapStyleId.Value);
            }
            else
            {
                return AlwaysConvert.ToInt(Request.Form[this.UniqueID + "_Wrap" + this.BasketItemId.ToString()]);
            }
        }
        set { _WrapStyleId = value; }
    }

    public String GiftMessage
    {
        get
        {
            GiftMessageText.Text = StringHelper.StripHtml(GiftMessageText.Text);
            return GiftMessageText.Text;
        }
        set { GiftMessageText.Text = value; }
    }

    public void CreateControls()
    {
        LoadWrapStyles();
        if (_WrapStyles == null)
        {
            //NO GIFT WRAP AVAILABLE
            OneGiftWrapPanel.Visible = false;
            MultiGiftWrapPanel.Visible = false;
        }
        else
        {
            NoGiftWrapPanel.Visible = false;
            if (_WrapStyles.Count == 1)
            {
                //ONE GIFT WRAP OPTION, OFF OR ON
                MultiGiftWrapPanel.Visible = false;
                AddGiftWrapStyleId.Value = _WrapStyles[0].WrapStyleId.ToString();
                AddGiftWrapLabel.Text = string.Format(AddGiftWrapLabel.Text, _WrapStyles[0].Name);
                if (_WrapStyles[0].Price > 0)
                {
                    LSDecimal shopPrice = TaxHelper.GetShopPrice(_WrapStyles[0].Price, _WrapStyles[0].TaxCodeId, null, this.ShipTaxAddress);
                    AddGiftWrapPrice.Text = string.Format(AddGiftWrapPrice.Text, shopPrice);
                }
                else
                {
                    AddGiftWrapPrice.Visible = false;
                }
                if (!String.IsNullOrEmpty(_WrapStyles[0].ThumbnailUrl))
                {
                    string wrapstyleHtml = string.Empty;
                    string image = string.Format("<img src=\"{0}\" border=\"0\" />", this.Page.ResolveClientUrl(_WrapStyles[0].ThumbnailUrl));
                    if (!String.IsNullOrEmpty(_WrapStyles[0].ImageUrl))
                    {
                        string link = "<a href=\"#\" onclick=\"{0};return false;\">{1}</a>";
                        string popup = PageHelper.GetPopUpScript(this.Page.ResolveClientUrl(_WrapStyles[0].ImageUrl), "giftwrapimage", 300, 300);
                        wrapstyleHtml = string.Format(link, popup, image);
                    }
                    else wrapstyleHtml = image;
                    AddGiftWrapHtml.Text = wrapstyleHtml;
                }
                else
                {
                    trGiftWrapImage.Visible = false;
                }
            }
            else
            {
                OneGiftWrapPanel.Visible = false;
                BuildWrapStyleList();
            }
        }
    }

    
    protected void BuildWrapStyleList()
    {
        int thumbnailHeight = Store.GetCachedSettings().ThumbnailImageHeight;
        int thumbnailWidth = Store.GetCachedSettings().ThumbnailImageWidth;
        PlaceHolder phWrapStyle = MultiGiftWrapPanel.FindControl("phWrapStyle") as PlaceHolder;
        if (phWrapStyle != null)
        {
            StringBuilder table = new StringBuilder();
            //table.Append("<span style=\"display:inline-block;padding:4px;\">");
            string selected = (_WrapStyleId == 0) ? " checked" : string.Empty;
            string radioButton = string.Format("<input type=\"radio\" name=\"{0}_Wrap{1}\" value=\"{2}\"{3}>&nbsp;", this.UniqueID, this.BasketItemId, 0, selected);
            table.Append("<br /><div style=\"padding:2px 4px;\">");
            table.Append(radioButton + "None</div>");
            //GET WRAP STYLE
            for (int i = 0; i < _WrapStyles.Count; i++)
            {
                WrapStyle style = _WrapStyles[i];
                table.Append("<div style=\"padding:2px 4px;\">");
                //ADD RADIO BUTTON
                selected = (_WrapStyleId == style.WrapStyleId) ? " checked" : string.Empty;
                radioButton = string.Format("<input type=\"radio\" name=\"{0}_Wrap{1}\" value=\"{2}\"{3}>&nbsp;", this.UniqueID, this.BasketItemId, style.WrapStyleId, selected);
                table.Append(radioButton);
                //ADD NAME
                if (style.Price > 0)
                {
                    //NAME WITH PRICE
                    LSDecimal shopPrice = TaxHelper.GetShopPrice(style.Price, style.TaxCodeId, null, this.ShipTaxAddress);
                    table.Append(string.Format(_CostFormat, style.Name, shopPrice));
                }
                else
                {
                    //JUST NAME
                    table.Append(string.Format(_FreeFormat, style.Name));
                }
                table.Append("<br />");
                if (!string.IsNullOrEmpty(style.ThumbnailUrl))
                {
                    //ADD CLICKABLE IMAGE
                    string thumbnailUrl = string.IsNullOrEmpty(style.ThumbnailUrl) ? "~/images/spacer.gif" : style.ThumbnailUrl;
                    string image = string.Format("<img src=\"{0}\" border=\"0\" vspace=\"2\" height=\"" + thumbnailHeight + "\" width=\"" + thumbnailWidth + "\" alt=\"" + Server.HtmlEncode(style.Name) + "\" />", this.Page.ResolveClientUrl(thumbnailUrl));
                    if (!string.IsNullOrEmpty(style.ImageUrl))
                    {
                        //CLICKABLE THUMBNAIL
                        string link = "<a href=\"#\" onclick=\"{0}\">{1}</a>";
                        string popup = PageHelper.GetPopUpScript(this.Page.ResolveClientUrl(style.ImageUrl), "giftwrapimage", 300, 300);
                        table.Append(string.Format(link, popup, image));
                    }
                    else
                    {
                        //STATIC THUMBNAIL ONLY
                        table.Append(image);
                    }
                }
                table.Append("</div>");
            }
            phWrapStyle.Controls.Add(new LiteralControl(table.ToString()));
        }
    }

    private void LoadWrapStyles()
    {
        WrapGroup wrapGroup = WrapGroupDataSource.Load(this.WrapGroupId);
        if (wrapGroup != null) _WrapStyles = wrapGroup.WrapStyles;
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if ((!Page.IsPostBack) && (OneGiftWrapPanel.Visible))
        {
            if (AddGiftWrapStyleId.Value == _WrapStyleId.ToString()) AddGiftWrap.Checked = true;
        }
        //INIT GIFT MESSAGE COUNTDOWN
        PageHelper.SetMaxLengthCountDown(GiftMessageText, GiftMessageCharCount);
        GiftMessageCharCount.Text = ((int)(GiftMessageText.MaxLength - GiftMessageText.Text.Length)).ToString();
    }

    TaxAddress _ShipAddress = null;
    private TaxAddress ShipTaxAddress
    {
        get
        {
            if (_ShipAddress == null)
            {
                User user = Token.Instance.User;
                int basketItemId = this.BasketItemId;
                foreach (BasketItem bi in user.Basket.Items)
                {
                    if (bi.BasketItemId == BasketItemId)
                    {
                        if ((bi.BasketShipment != null) && (bi.BasketShipment.Address != null))
                        {
                            _ShipAddress = new TaxAddress(bi.BasketShipment.Address);
                        }
                    }
                }
                //IF WE STILL HAVEN'T FOUND SHIP ADDRESS, USE BILLING
                if (_ShipAddress == null) _ShipAddress = new TaxAddress(user.PrimaryAddress);
            }
            return _ShipAddress;
        }
    }
}
