<%@ Control Language="C#" ClassName="GiftCertificatePaymentForm" %>

<script runat="server">
    //DEFINE EVENTS TO TRIGGER FOR CHECKOUT
    public event CheckingOutEventHandler CheckingOut;
    public event CheckedOutEventHandler CheckedOut;

    //private List<string> _GiftCertErrorMessages=null;
    private string _GiftCertErrorMessage = string.Empty;
    private string _ValidationGroup = "GiftCertificate";
    public string ValidationGroup
    {
        get { return _ValidationGroup; }
        set { _ValidationGroup = value; }
    }

    private bool _ValidationSummaryVisible = true;
    public bool ValidationSummaryVisible
    {
        get { return _ValidationSummaryVisible; }
        set { _ValidationSummaryVisible = value; }
    }

    private void UpdateValidationOptions()
    {
        GiftCertificateNumber.ValidationGroup = _ValidationGroup;
        GiftCertificateNumberRequired.ValidationGroup = _ValidationGroup;
        GiftCertificateButton.ValidationGroup = _ValidationGroup;
        ValidationSummary1.ValidationGroup = _ValidationGroup;
        ValidationSummary1.Visible = _ValidationSummaryVisible;
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        UpdateValidationOptions();
    }
        
    private GridView _NonShippingItemsGrid = null;

    public GridView NonShippingItemsGrid
    {
        set { _NonShippingItemsGrid = value; }
        get { return _NonShippingItemsGrid; }
    }

    private bool HasGiftCertErrorMessage()
    {
        return !string.IsNullOrEmpty(_GiftCertErrorMessage);
    }

    private void SetGiftCertErrorMessage(string message)
    {        
        _GiftCertErrorMessage = message;
    }

    private string GetGiftCertErrorMessage()
    {
        return _GiftCertErrorMessage;
    }
    
	private void RebindNonShippingItems() {
		if (NonShippingItemsGrid != null)
        {
			BasketItemCollection nonShippingItems = BasketHelper.GetNonShippingItems(Token.Instance.User.Basket);
			if (nonShippingItems.Count > 0)
			{
				//NonShippingItemsPanel.Visible = true;
                NonShippingItemsGrid.Parent.Visible = true;
				NonShippingItemsGrid.DataSource = nonShippingItems;
				NonShippingItemsGrid.DataBind();
			}
       }
	}

    protected void GiftCertificateButton_Click(object sender, EventArgs e)
    {
        SetGiftCertErrorMessage("");
        if (Page.IsValid)
        {
            bool checkOut = true;
            if (CheckingOut != null)
            {
                CheckingOutEventArgs c = new CheckingOutEventArgs();
                CheckingOut(this, c);
                checkOut = !c.Cancel;
            }
            if (checkOut)
            {
                Basket basket = Token.Instance.User.Basket;
                GiftCertificateNumber.Text = StringHelper.StripHtml(GiftCertificateNumber.Text);
                GiftCertificate gc = GiftCertificateDataSource.LoadForSerialNumber(GiftCertificateNumber.Text);
                if (gc == null)
                {
                    SetGiftCertErrorMessage("This is not a valid gift certificate : " + GiftCertificateNumber.Text);
                }
                else if (gc.Balance <= 0)
                {
                    SetGiftCertErrorMessage("There is no balance left for this gift certificate : " + GiftCertificateNumber.Text);
                }
                else if (gc.IsExpired())
                {
                    SetGiftCertErrorMessage("This gift certificate is expired : " + GiftCertificateNumber.Text);
                }
                else if (AlreadyInUse(basket, gc))
                {
                    SetGiftCertErrorMessage("This gift certificate is already applied to your basket : " + GiftCertificateNumber.Text);
                }
                else
                {
                    //process this gift certificate
                    LSDecimal basketTotal = basket.Items.TotalPrice();
                    BasketItem bitem = new BasketItem();
                    bitem.OrderItemType = OrderItemType.GiftCertificatePayment;
                    bitem.Price = -(gc.Balance > basketTotal ? basketTotal : gc.Balance);
                    bitem.Quantity = 1;
                    bitem.Name = gc.Name;
                    bitem.Sku = gc.SerialNumber;
                    basket.Items.Add(bitem);
                    basket.Save();
                    LSDecimal remBalance = basket.Items.TotalPrice();
                    if (remBalance > 0)
                    {
                        SetGiftCertErrorMessage(string.Format("A payment of {0:ulc} will be made using gift certificate {1}. It will leave a balance of {2:ulc} for this order. Please make additional payments.", gc.Balance, GiftCertificateNumber.Text, remBalance));
                    }
                    else
                    {
                        //payment done. process checkout
                        CheckoutRequest checkoutRequest = new CheckoutRequest(null);
                        CheckoutResponse checkoutResponse = basket.Checkout(checkoutRequest);
                        if (checkoutResponse.Success)
                        {
                            if (CheckedOut != null) CheckedOut(this, new CheckedOutEventArgs(checkoutResponse));
                            Response.Redirect(NavigationHelper.GetReceiptUrl(checkoutResponse.OrderId));
                        }
                        else
                        {
                            List<string> warningMessages = checkoutResponse.WarningMessages;
                            if (warningMessages.Count == 0)
                                warningMessages.Add("The order could not be submitted at this time.  Please try again later or contact us for assistance.");
                            if (CheckedOut != null) CheckedOut(this, new CheckedOutEventArgs(checkoutResponse));
                        }
                    }
                    RebindNonShippingItems();
                }
                if (HasGiftCertErrorMessage())
                {
                    GiftCertErrorsPanel.Visible = true;
                    GiftCertPaymentErrors.Text = GetGiftCertErrorMessage();
                }
            }
        }
    }

    private bool AlreadyInUse(Basket basket, GiftCertificate gc)
    {
        foreach (BasketItem item in basket.Items)
        {
            if (item.OrderItemType == OrderItemType.GiftCertificatePayment
                && item.Sku == gc.SerialNumber)
            {
                return true;
            }
        }
        return false;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        GiftCertificateNumber.Attributes.Add("autocomplete", "off");
    }
</script>

<table class="paymentForm">
    <tr>
        <th class="caption" colspan="2">
            <asp:Label ID="Caption" runat="server" Text="Pay With Gift Certificate"></asp:Label>
        </th>
    </tr>
    <tr>
        <td class="pFcontent" colspan="2">
            <asp:Label ID="GiftCertificateHelpText" runat="server" Text="Enter the gift certificate number below."></asp:Label>
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <asp:Label ID="GiftCertificateNumberLabel" runat="server" Text="Gift Certificate #:" AssociatedControlID="GiftCertificateNumber"></asp:Label>
        </th>
        <td>
            <asp:TextBox ID="GiftCertificateNumber" runat="server" MaxLength="50" ValidationGroup="GiftCertificate"></asp:TextBox>
            <asp:RequiredFieldValidator ID="GiftCertificateNumberRequired" runat="server" 
                ErrorMessage="You must enter the gift certificate number." 
                ControlToValidate="GiftCertificateNumber" Display="Static" Text="*" ValidationGroup="GiftCertificate"></asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <td class="submit" colspan="2">
            <asp:ValidationSummary runat="server" ID="ValidationSummary1" ValidationGroup="GiftCertificate" />
            <asp:Button ID="GiftCertificateButton" runat="server" Text="Pay With Gift Certificate" ValidationGroup="GiftCertificate" OnClick="GiftCertificateButton_Click" />
            <asp:Panel runat="server" Visible="false" ID="GiftCertErrorsPanel">                
                <asp:Label SkinID="ErrorCondition" ID="GiftCertPaymentErrors" runat="server" Text=""></asp:Label>
            </asp:Panel>            
        </td>
    </tr>
</table>
