<%@ Control Language="C#" AutoEventWireup="true" CodeFile="OnePageCheckout.ascx.cs" Inherits="ConLib_OnePageCheckout" %>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
<%--
<conlib>
<summary>Displays a one page checkout form.</summary>
<param name="EnableGiftWrap" default="true">Possible values are true or false.  Indicates whether gift wrap options should be shown to the customer.</param>
<param name="EnableMultiShipTo" default="true">Possible values are true or false.  Indicates whether multiple shipping destinations are allowed and should be made available to the customer.</param>
<param name="EnableShipMessage" default="false">Possible values are true or false.  When true, a field is displayed for the customer to enter delivery instructions for the shipment.</param>
<param name="AllowAnonymousCheckout" default="false">Possible values are true or false.  When true, customers are allowed to check out without creating a user account.</param>
<param name="DisableBots" default="true">Indicates whether to attempt to prevent bots and spiders from accessing the checkout page.  This can be disabled if it causes undesired behavior.</param>
<param name="MaxCheckoutAttemptsPerMinute" default="10">Number of checkout attempts a user can make within one minute. A user exceeding this limit will be considered a bot. The default value of 10 is pretty lenient.</param>
</conlib>
--%>
<%@ Register Src="~/ConLib/BasketTotalSummary.ascx" TagName="BasketTotalSummary" TagPrefix="uc" %>
<%@ Register Src="~/ConLib/CouponDialog.ascx" TagName="CouponDialog" TagPrefix="uc" %>
<%@ Register Src="~/Checkout/PaymentForms/CreditCardPaymentForm.ascx" TagName="CreditCardPaymentForm" TagPrefix="uc" %>
<%@ Register Src="~/Checkout/PaymentForms/CheckPaymentForm.ascx" TagName="CheckPaymentForm" TagPrefix="uc" %>
<%@ Register Src="~/Checkout/PaymentForms/PayPalExpressPaymentForm.ascx" TagName="PayPalExpressPaymentForm" TagPrefix="uc" %>
<%@ Register Src="~/Checkout/PaymentForms/PayPalPaymentForm.ascx" TagName="PayPalPaymentForm" TagPrefix="uc" %>
<%@ Register Src="~/Checkout/PaymentForms/MailPaymentForm.ascx" TagName="MailPaymentForm" TagPrefix="uc" %>
<%@ Register Src="~/Checkout/PaymentForms/PhoneCallPaymentForm.ascx" TagName="PhoneCallPaymentForm" TagPrefix="uc" %>
<%@ Register Src="~/Checkout/PaymentForms/PurchaseOrderPaymentForm.ascx" TagName="PurchaseOrderPaymentForm" TagPrefix="uc" %>
<%@ Register Src="~/Checkout/PaymentForms/GiftCertificatePaymentForm.ascx" TagName="GiftCertificatePaymentForm" TagPrefix="uc" %>
<%@ Register Src="~/Checkout/PaymentForms/ZeroValuePaymentForm.ascx" TagName="ZeroValuePaymentForm" TagPrefix="uc" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Src="~/ConLib/Utility/BasketItemDetail.ascx" TagName="BasketItemDetail" TagPrefix="uc" %>
<ajax:UpdatePanel ID="ShippingAddressAjax" runat="server">
    <ContentTemplate>
	    <asp:DataList ID="WarningMessageList" runat="server" EnableViewState="false">
	        <HeaderTemplate><ul></HeaderTemplate>
	        <ItemTemplate>
	            <li><%# Container.DataItem %></li>
	        </ItemTemplate>
	        <FooterTemplate></ul></FooterTemplate>
	    </asp:DataList>
	    <asp:PlaceHolder ID="CheckoutPanel" runat="server">
            <div class="checkoutPageHeader">
                <h1>Checkout</h1>
                <div class="checkoutAlert">
                    We make online shopping convenient and secure!  If you need any assistance with the online checkout process, or would rather place your order by phone, please give us a call during regular business hours.
                </div>
            </div>
            <asp:PlaceHolder ID="LoginPanel" runat="server" EnableViewState="False">
                <p class="LoginMessage">If you have previously created an account, you can <asp:HyperLink ID="LoginLink" runat="server" Text="log in" EnableViewState="false"></asp:HyperLink> to retrieve your saved addresses.</p><br />
            </asp:PlaceHolder>
            <!-- split page into columns -->
            <table cellpadding="0" cellspacing="0" class="opcFrame">
                <tr>
                    <td valign="top" class="opcMainPanel">
                        <table cellpadding="0" cellspacing="0" style="width:100%">
                            <tr>
                                <td>
                                    <br />
                                    <h2 class="sectionHeader">Billing Address</h2>
                                    <asp:Panel ID="EnterBillingPanel" runat="server" EnableViewState="False" DefaultButton="ContinueButton">
                                        <table class="inputForm" cellpadding="3">
                                            <tr>                        
                                                <th class="rowHeader">
                                                    <asp:Label ID="BillToFirstNameLabel" runat="server" Text="First Name:" AssociatedControlID="BillToFirstName" EnableViewState="false"></asp:Label>
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="BillToFirstName" runat="server" EnableViewState="false" Width="120px" MaxLength="20" ValidationGroup="OPC"></asp:TextBox> 
                                                    <asp:RequiredFieldValidator ID="BillToFirstNameRequired" runat="server" Text="*"
                                                        ErrorMessage="First name is required." Display="Static" ControlToValidate="BillToFirstName"
                                                        EnableViewState="False" SetFocusOnError="false" ValidationGroup="OPC"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>                        
                                                <th class="rowHeader">
                                                    <asp:Label ID="BillToLastNameLabel" runat="server" Text="Last Name:" AssociatedControlID="BillToLastName" EnableViewState="false"></asp:Label>
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="BillToLastName" runat="server" EnableViewState="false" Width="120px" MaxLength="30" ValidationGroup="OPC"></asp:TextBox> 
                                                    <asp:RequiredFieldValidator ID="BillToLastNameRequired" runat="server" Text="*"
                                                        ErrorMessage="Last name is required." Display="Static" ControlToValidate="BillToLastName"
                                                        EnableViewState="False" SetFocusOnError="false" ValidationGroup="OPC"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th class="rowHeader">
                                                    <asp:Label ID="BillToCompanyLabel" runat="server" Text="Company:" AssociatedControlID="BillToCompany" EnableViewState="false"></asp:Label>
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="BillToCompany" runat="server" EnableViewState="false" Width="200px" MaxLength="50"></asp:TextBox> 
                                                </td>
                                            </tr>
                                            <tr>
                                                <th class="rowHeader">
                                                    <asp:Label ID="BillToAddress1Label" runat="server" Text="Address:" AssociatedControlID="BillToAddress1" EnableViewState="false"></asp:Label>
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="BillToAddress1" runat="server" EnableViewState="false" Width="200px" MaxLength="100" ValidationGroup="OPC"></asp:TextBox> 
                                                    <asp:RequiredFieldValidator ID="BillToAddress1Required" runat="server" Text="*"
                                                        ErrorMessage="Billing address is required." Display="Static" ControlToValidate="BillToAddress1"
                                                        EnableViewState="false" SetFocusOnError="false" ValidationGroup="OPC"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>&nbsp;</td>
                                                <td>
                                                    <asp:TextBox ID="BillToAddress2" runat="server" EnableViewState="false" Width="200px" MaxLength="100"></asp:TextBox> 
                                                </td>
                                            </tr>
                                            <tr>
                                                <th class="rowHeader">
                                                    <asp:Label ID="BillToCityLabel" runat="server" Text="City:" AssociatedControlID="BillToCity" EnableViewState="false"></asp:Label>
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="BillToCity" runat="server" EnableViewState="false" Width="200px" MaxLength="50" ValidationGroup="OPC"></asp:TextBox> 
                                                    <asp:RequiredFieldValidator ID="BillToCityRequired" runat="server" Text="*"
                                                        ErrorMessage="Billing city is required." Display="Static" ControlToValidate="BillToCity"
                                                        EnableViewState="false" SetFocusOnError="false" ValidationGroup="OPC"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th class="rowHeader">
                                                    <asp:Label ID="BillToProvinceLabel" runat="server" Text="State / Province:" AssociatedControlID="BillToProvince" EnableViewState="false"></asp:Label>
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="BillToProvince" runat="server" Visible="false" EnableViewState="false" MaxLength="50" Width="200px"></asp:TextBox> 
                                                    <asp:DropDownList ID="BillToProvinceList" runat="server" EnableViewState="false" Width="200px"></asp:DropDownList>
                                                    <asp:CustomValidator ID="BillToProvinceInvalid" runat="server" Text="*"
                                                        ErrorMessage="The billing state or province you entered was not recognized.  Please choose from the list." Display="Dynamic" ControlToValidate="BillToProvinceList" ValidationGroup="OPC"></asp:CustomValidator>
                                                    <asp:RequiredFieldValidator ID="BillToProvinceRequired" runat="server" Text="*"
                                                        ErrorMessage="Billing state or province is required." Display="Static" ControlToValidate="BillToProvinceList" ValidationGroup="OPC"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th class="rowHeader">
                                                    <asp:Label ID="BillToPostalCodeLabel" runat="server" Text="ZIP / Postal Code:" AssociatedControlID="BillToPostalCode" EnableViewState="false"></asp:Label>
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="BillToPostalCode" runat="server" EnableViewState="false" Width="90px" MaxLength="10" ValidationGroup="OPC"></asp:TextBox> 
                                                    <asp:RequiredFieldValidator ID="BillToPostalCodeRequired" runat="server" Text="*"
                                                        ErrorMessage="Billing ZIP or Postal Code is required." Display="Static" ControlToValidate="BillToPostalCode"
                                                        EnableViewState="False" SetFocusOnError="false" ValidationGroup="OPC"></asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th class="rowHeader">
                                                    <asp:Label ID="BillToCountryLabel" runat="server" Text="Country:" AssociatedControlID="BillToCountry" EnableViewState="false"></asp:Label>
                                                </th>
                                                <td>
                                                    <asp:DropDownList ID="BillToCountry" runat="server" DataTextField="Name" DataValueField="CountryCode" 
                                                        AutoPostBack="true" EnableViewState="false" Width="200px"></asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th class="rowHeader" valign="top">
                                                    <asp:Label ID="BillToAddressTypeLabel" runat="server" Text="Type:" AssociatedControlID="BillToAddressType" EnableViewState="false"></asp:Label>
                                                </th>
                                                <td valign="top">
                                                    <asp:DropDownList ID="BillToAddressType" runat="server" EnableViewState="false">
                                                        <asp:ListItem Text="This is a residence" Value="1" Selected="true"></asp:ListItem>
                                                        <asp:ListItem Text="This is a business" Value="0"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th class="rowHeader">
                                                    <asp:Label ID="BillToPhoneLabel" runat="server" Text="Phone:" AssociatedControlID="BillToPhone" EnableViewState="false"></asp:Label>
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="BillToPhone" runat="server" EnableViewState="false" Width="200px" MaxLength="30"></asp:TextBox> 
                                                    <asp:RequiredFieldValidator ID="BillToPhoneRequired" runat="server" Text="*"
                                                        ErrorMessage="Phone number is required." Display="Static" ControlToValidate="BillToPhone"
                                                        EnableViewState="False" SetFocusOnError="false" ValidationGroup="OPC"></asp:RequiredFieldValidator>
                                                    
                                                </td>
                                            </tr>
                                            <tr id="trEmail" runat="server" enableviewstate="false">
                                                <th class="rowHeader">
                                                    <asp:Label ID="BillToEmailLabel" runat="server" Text="Email:" AssociatedControlID="BillToEmail" EnableViewState="false"></asp:Label>
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="BillToEmail" runat="server" EnableViewState="false" Width="200px" MaxLength="250" ValidationGroup="OPC"></asp:TextBox> 
                                                    <cb:EmailAddressValidator ID="EmailAddressValidator1" runat="server" ControlToValidate="BillToEmail" ValidationGroup="OPC" Display="static" Required="true" ErrorMessage="Email address should be in the format of name@domain.tld." Text="*" EnableViewState="False"></cb:EmailAddressValidator>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:Panel>
                                    <asp:PlaceHolder ID="ViewBillingPanel" runat="server" Visible="false" EnableViewState="false">
                                        <asp:Literal ID="FormattedBillingAddress" runat="server" EnableViewState="false"></asp:Literal>
                                        <br />
                                        <div style="margin-top:10px;"><asp:LinkButton ID="EditAddressesButton" runat="server" Text="Edit" SkinID="Button" EnableViewState="false"></asp:LinkButton></div>
                                    </asp:PlaceHolder>
                                </td>
                            </tr>
                            <tr id="trShippingAddress" runat="server" enableviewstate="false">
                                <td>
                                    <br />
                                    <h2 class="sectionHeader">Shipping Address</h2>
                                    <asp:Panel ID="EnterShippingPanel" runat="server" EnableViewState="false" DefaultButton="ContinueButton">
                                        <table>
                                            <tr>
                                                <td>
                                                    <asp:RadioButton ID="UseBillingAddress" runat="server" Checked="true" GroupName="UseShippingAddress" AutoPostBack="true" EnableViewState="false" />
                                                </td>
                                                <td>
                                                    <asp:Label ID="UseBillingAddressLabel" runat="server" AssociatedControlID="UseBillingAddress" Text="Ship to my billing address" EnableViewState="false"></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td valign="top">
                                                    <asp:RadioButton ID="UseShippingAddress" runat="server" GroupName="UseShippingAddress" AutoPostBack="true" EnableViewState="false" />
                                                </td>
                                                <td>
                                                    <asp:Label ID="UseShippingAddressLabel" runat="server" AssociatedControlID="UseShippingAddress" Text="Ship to a different address" EnableViewState="false"></asp:Label>
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:PlaceHolder ID="ShipAddressPanel" runat="server" Visible="false" EnableViewState="false">
                                            <table class="inputForm">
                                                <tr id="trAddressBook" runat="server">
                                                    <th class="rowHeader">
                                                        Address Book:
                                                    </th>
                                                    <td>
                                                        <asp:DropDownList ID="AddressBook" runat="server" Width="300px" DataTextField="Value" 
                                                            DataValueField="Key" AutoPostBack="true" EnableViewState="false"></asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>                        
                                                    <th class="rowHeader">
                                                        <asp:Label ID="ShipToFirstNameLabel" runat="server" Text="First Name:" AssociatedControlID="ShipToFirstName" EnableViewState="false"></asp:Label>
                                                    </th>
                                                    <td>
                                                        <asp:TextBox ID="ShipToFirstName" runat="server" EnableViewState="false" Width="120px" MaxLength="20" ValidationGroup="OPC"></asp:TextBox> 
                                                        <asp:RequiredFieldValidator ID="ShipToFirstNameRequired" runat="server" Text="*"
                                                            ErrorMessage="Ship to first name is required." Display="Static" ControlToValidate="ShipToFirstName"
                                                            EnableViewState="false" SetFocusOnError="false" ValidationGroup="OPC"></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr>                        
                                                    <th class="rowHeader">
                                                        <asp:Label ID="ShipToLastNameLabel" runat="server" Text="Last Name:" AssociatedControlID="ShipToLastName" EnableViewState="false"></asp:Label>
                                                    </th>
                                                    <td>
                                                        <asp:TextBox ID="ShipToLastName" runat="server" EnableViewState="false" Width="120px" MaxLength="30" ValidationGroup="OPC"></asp:TextBox> 
                                                        <asp:RequiredFieldValidator ID="ShipToLastNameRequired" runat="server" Text="*"
                                                            ErrorMessage="Ship to last name is required." Display="Static" ControlToValidate="ShipToLastName"
                                                            EnableViewState="false" SetFocusOnError="false" ValidationGroup="OPC"></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th class="rowHeader">
                                                        <asp:Label ID="ShipToCompanyLabel" runat="server" Text="Company:" AssociatedControlID="ShipToCompany" EnableViewState="false"></asp:Label>
                                                    </th>
                                                    <td>
                                                        <asp:TextBox ID="ShipToCompany" runat="server" EnableViewState="false" Width="200px" MaxLength="50"></asp:TextBox> 
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th class="rowHeader">
                                                        <asp:Label ID="ShipToAddress1Label" runat="server" Text="Address:" AssociatedControlID="ShipToAddress1" EnableViewState="false"></asp:Label>
                                                    </th>
                                                    <td>
                                                        <asp:TextBox ID="ShipToAddress1" runat="server" EnableViewState="false" Width="200px" MaxLength="100" ValidationGroup="OPC"></asp:TextBox> 
                                                        <asp:RequiredFieldValidator ID="ShipToAddress1Required" runat="server" Text="*"
                                                            ErrorMessage="Shipping address is required." Display="Static" ControlToValidate="ShipToAddress1"
                                                            EnableViewState="false" SetFocusOnError="false" ValidationGroup="OPC"></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>&nbsp;</td>
                                                    <td>
                                                        <asp:TextBox ID="ShipToAddress2" runat="server" EnableViewState="false" Width="200px" MaxLength="100"></asp:TextBox> 
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th class="rowHeader">
                                                        <asp:Label ID="ShipToCityLabel" runat="server" Text="City:" AssociatedControlID="ShipToCity" EnableViewState="false"></asp:Label>
                                                    </th>
                                                    <td>
                                                        <asp:TextBox ID="ShipToCity" runat="server" EnableViewState="false" Width="200px" MaxLength="50" ValidationGroup="OPC"></asp:TextBox> 
                                                        <asp:RequiredFieldValidator ID="ShipToCityRequired" runat="server" Text="*"
                                                            ErrorMessage="Shipping city is required." Display="Static" ControlToValidate="ShipToCity"
                                                            EnableViewState="false" SetFocusOnError="false" ValidationGroup="OPC"></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th class="rowHeader">
                                                        <asp:Label ID="ShipToProvinceLabel" runat="server" Text="State / Province:" AssociatedControlID="ShipToProvince" EnableViewState="false"></asp:Label>
                                                    </th>
                                                    <td>
                                                        <asp:TextBox ID="ShipToProvince" runat="server" Visible="false" EnableViewState="false" Width="200px" MaxLength="50"></asp:TextBox> 
                                                        <asp:DropDownList ID="ShipToProvinceList" runat="server" EnableViewState="false" Width="200px"></asp:DropDownList>
                                                        <asp:CustomValidator ID="ShipToProvinceInvalid" runat="server" Text="*"
                                                            ErrorMessage="The shipping state or province you entered was not recognized.  Please choose from the list." Display="Dynamic" ControlToValidate="ShipToProvinceList" ValidationGroup="OPC"></asp:CustomValidator>
                                                        <asp:RequiredFieldValidator ID="ShipToProvinceRequired" runat="server" Text="*"
                                                            ErrorMessage="Shipping state or province is required." Display="Static" ControlToValidate="ShipToProvinceList" ValidationGroup="OPC"></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th class="rowHeader">
                                                        <asp:Label ID="ShipToPostalCodeLabel" runat="server" Text="ZIP / Postal Code:" AssociatedControlID="ShipToPostalCode" EnableViewState="false"></asp:Label>
                                                    </th>
                                                    <td>
                                                        <asp:TextBox ID="ShipToPostalCode" runat="server" EnableViewState="false" Width="90px" MaxLength="10" ValidationGroup="OPC"></asp:TextBox> 
                                                        <asp:RequiredFieldValidator ID="ShipToPostalCodeRequired" runat="server" Text="*"
                                                            ErrorMessage="Shipping ZIP or Postal Code is required." Display="Static" ControlToValidate="ShipToPostalCode"
                                                            EnableViewState="false" SetFocusOnError="false" ValidationGroup="OPC"></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th class="rowHeader">
                                                        <asp:Label ID="ShipToCountryLabel" runat="server" Text="Country:" AssociatedControlID="ShipToCountry" EnableViewState="false"></asp:Label>
                                                    </th>
                                                    <td>
                                                        <asp:DropDownList ID="ShipToCountry" runat="server" DataTextField="Name" DataValueField="CountryCode" 
                                                            AutoPostBack="true" EnableViewState="false" Width="200px"></asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th class="rowHeader" valign="top">
                                                        <asp:Label ID="ShipToAddressTypeLabel" runat="server" Text="Type:" AssociatedControlID="ShipToAddressType" EnableViewState="false"></asp:Label>
                                                    </th>
                                                    <td valign="top">
                                                        <asp:DropDownList ID="ShipToAddressType" runat="server" EnableViewState="false">
                                                            <asp:ListItem Text="This is a residence" Value="1" Selected="true"></asp:ListItem>
                                                            <asp:ListItem Text="This is a business" Value="0"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <th class="rowHeader">
                                                        <asp:Label ID="ShipToPhoneLabel" runat="server" Text="Phone:" AssociatedControlID="ShipToPhone" EnableViewState="false"></asp:Label>
                                                    </th>
                                                    <td>
                                                        <asp:TextBox ID="ShipToPhone" runat="server" EnableViewState="false" Width="200px" MaxLength="30"></asp:TextBox> 
                                                        <asp:RequiredFieldValidator ID="ShipToPhoneRequired" runat="server" Text="*"
                                                            ErrorMessage="Phone number is required." Display="Static" ControlToValidate="ShipToPhone"
                                                            EnableViewState="false" SetFocusOnError="false" ValidationGroup="OPC"></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:PlaceHolder>
                                    </asp:Panel>
                                    <asp:PlaceHolder ID="ViewShippingPanel" runat="server" Visible="false" EnableViewState="false">
                                        <asp:Literal ID="FormattedShippingAddress" runat="server" EnableViewState="false"></asp:Literal>
                                        <br />
                                        <div style="margin-top:10px;"><asp:LinkButton ID="EditAddressesButton2" runat="server" Text="Edit" SkinID="Button" EnableViewState="false"></asp:LinkButton>&nbsp;<asp:LinkButton ID="AddAddressesButton" runat="server" Text="Add New" SkinID="Button" EnableViewState="false"></asp:LinkButton></div>
                                    </asp:PlaceHolder>
                                </td>
                            </tr>
                            <tr id="trContinue" runat="server" enableviewstate="false">
                                <td valign="top">
                                    <asp:ValidationSummary ID="AddressValidationSummary" runat="server" EnableViewState="false" ValidationGroup="OPC" />
                                    <br />
                                    <asp:ImageButton ID="ContinueButton" runat="server" AlternateText="Continue" SkinID="Continue" EnableViewState="false" OnClick="ContinueButton_Click" ValidationGroup="OPC" />
                                    <br /><br />
                                </td>
                            </tr>
                            <tr id="trShowRates" runat="server" enableviewstate="false">
                                <td>
                                    <br />
                                    <h2 class="sectionHeader">Shipping Method</h2>
                                    <asp:PlaceHolder ID="MultipleShipmentsMessage" runat="server" EnableViewState="false" Visible="false">
                                        <br />Your order contains items that must be sent in more than one shipment.  View the order details at the bottom of this page for the contents of each shipment.<br /><br />
                                    </asp:PlaceHolder>
                                    <asp:Repeater ID="ShipmentList" runat="server" OnItemDataBound="ShipmentList_ItemDataBound" EnableViewState="false">
                                        <ItemTemplate>
                                            <asp:PlaceHolder ID="ShipmentCounter" runat="server" Visible='<%# (ShipmentCount > 1) %>' EnableViewState="false"><b>Shipment <%# (Container.ItemIndex + 1) %>:&nbsp; </b></asp:PlaceHolder>
                                            <asp:Label ID="ShipMethodListLabel" runat="server" Visible='<%# (ShipmentCount == 1) %>' EnableViewState="false" Text="Select Method:" SkinID="FieldHeader"></asp:Label>&nbsp;
                                            <asp:DropDownList ID="ShipMethodList" runat="server" DataTextField="Name" DataValueField="ShipMethodId" EnableViewState="false" AutoPostBack="true"></asp:DropDownList><br />
                                            <asp:PlaceHolder ID="ShipMessagePanel" runat="server" Visible='<%# EnableShipMessage %>'>
                                                <asp:Label ID="ShipMessageLabel" runat="server" Text="Delivery Instructions?" SkinID="FieldHeader"></asp:Label>&nbsp;
                                                <asp:TextBox ID="ShipMessage" runat="server" Text="" MaxLength="200" Width="200px"></asp:TextBox>
                                            </asp:PlaceHolder>
                                            <br /><br />
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <asp:Localize ID="ShipMethodErrorMessage" runat="server" Visible="false" EnableViewState="false" Text="There are no shipping methods available to the selected destination(s)."></asp:Localize>
                                </td>
                            </tr>
                            <tr id="trAlreadyRegistered" runat="server" visible="false" enableviewstate="false">
                                <td>
                                    <br />
                                    <h2 class="sectionHeader">Already Registered</h2>
                                    <asp:Literal ID="RegisteredUserName" runat="server" EnableViewState="false"></asp:Literal> has already been registered.  You must <asp:LinkButton ID="LoginLink2" runat="server" Text="LOG IN" EnableViewState="false" OnClick="LoginLink2_Click"></asp:LinkButton> in order to check out with this email address.
                                </td>
                            </tr>
                            <tr id="trAccount" runat="server" visible="false" enableviewstate="false">
                                <td>
                                    <br />
                                    <h2 class="sectionHeader">Create Account</h2>
                                    <asp:Label ID="CreateAccountLabel" runat="Server" Text="We invite you to register an account with us.  Simply enter a password in the field below.  This will enable you to return and check the status of your order and speed up future purchases." EnableViewState="false"></asp:Label>
                                    <asp:PlaceHolder ID="AnonymousCheckoutPanel" runat="server" EnableViewState="false">
                                        <br /><br /><asp:CheckBox ID="AnonymousCheckoutIndicator" runat="server" AutoPostBack="true" />
                                        Check here to continue your purchase without making an account.  If you do not create an account, you will not be able to return at a later time to view your order receipt.<br /><br />
                                    </asp:PlaceHolder>
                                    <asp:PlaceHolder ID="CreateAccountPanel" runat="server" EnableViewState="false">
                                        <asp:ValidationSummary ID="CreateAccountValidationSummary" runat="server" EnableViewState="false" ValidationGroup="CreateAccount" />
                                        <table class="inputForm">
                                            <tr id="trNewUserName1" runat="server" enableviewstate="false">
                                                <th class="rowHeader">
                                                    Username:
                                                </th>
                                                <td>
                                                    <asp:Literal ID="NewUserName" runat="server" EnableViewState="false"></asp:Literal>
                                                </td>
                                            </tr>
                                            <tr id="trNewUserName2" runat="server" enableviewstate="false" visible="false">
                                                <th class="rowHeader">
                                                    Username:
                                                </th>
                                                <td>
                                                    <asp:Localize ID="UsernameIsEmailMessage" runat="server" EnableViewState="false" Text="Your username will be your email address."></asp:Localize>
                                                    <asp:PlaceHolder ID="AlreadyRegisteredValidator" runat="server" EnableViewState="false"></asp:PlaceHolder>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th class="rowHeader">
                                                    Password:
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="NewUserPassword" runat="server" TextMode="password" EnableViewState="false" ValidationGroup="CreateAccount"></asp:TextBox>
                                                    <asp:PlaceHolder ID="PasswordValidatorPanel" runat="server" EnableViewState="false"></asp:PlaceHolder>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>&nbsp;</td>
                                                <td>
                                                    <i><asp:Localize ID="PasswordPolicyLength" runat="server" Text="Your password must be at least {0} characters long."></asp:Localize>
                                                    <asp:Localize ID="PasswordPolicyRequired" runat="server" Text="You must include at least one {0}."></asp:Localize></i>
                                                </td>
                                            </tr>
                                            <tr>
                                                <th class="rowHeader">
                                                    Retype:
                                                </th>
                                                <td>
                                                    <asp:TextBox ID="NewUserPassword2" runat="server" TextMode="password" EnableViewState="false" ValidationGroup="CreateAccount"></asp:TextBox>
                                                    <asp:CompareValidator ID="NewUserPasswordValidator" runat="server" Text="*" Display="Static" 
                                                        ErrorMessage="You did not retype your new password correctly." ControlToValidate="NewUserPassword"
                                                        ControlToCompare="NewUserPassword2" EnableViewState="false" SetFocusOnError="false" ValidationGroup="CreateAccount"></asp:CompareValidator>
                                                </td>
                                            </tr>
                                        </table>
                                    </asp:PlaceHolder>
                                </td>
                            </tr>
                            <tr id="trEmailLists" runat="server" visible="false" enableviewstate="false">
                                <td>
                                    <br />
                                    <asp:Localize ID="EmailListHeader" runat="server" Text='<h2 class="sectionHeader">Communication Preferences</h2>' EnableViewState="false"></asp:Localize>
                                    <asp:PlaceHolder ID="EmailListPanel" runat="server" EnableViewState="false">
                                        <asp:Literal ID="EmailListHelpText" runat="server" Text="Select any mailing lists you might like to join:" EnableViewState="false"></asp:Literal>
                                        <asp:DataList ID="EmailLists" runat="server" EnableViewState="false">
                                            <ItemTemplate>
                                                <br /><asp:CheckBox ID="Selected" runat="server" Checked="<%#IsEmailListChecked(Container.DataItem)%>" EnableViewState="false" />
                                                <%#Eval("Name")%><br />
                                                <%#Eval("Description")%><br />
                                            </ItemTemplate>
                                        </asp:DataList>
                                    </asp:PlaceHolder>
                                </td>
                            </tr>
                            <tr id="trPayment" runat="server" visible="false" enableviewstate="false">
                                <td>
                                    <br />
                                    <h2 class="sectionHeader">Payment Method</h2>
                                    <asp:PlaceHolder ID="phTermsAndConditions" runat="server" Visible="false" EnableViewState="false">
                                        <table cellpadding="2" cellspacing="0">
                                            <tr>
                                                <td>
                                                    <asp:CheckBox ID="AcceptTC" runat="server" EnableViewState="false" />
                                                </td>
                                                <td>
                                                    <asp:Label ID="AcceptTCLabel" runat="server" EnableViewState="false" AssociatedControlID="AcceptTC" Text="I accept the Terms and Conditions" SkinID="FieldHeader"></asp:Label>
                                                    <asp:CustomValidator ID="AcceptTCValidator" runat="server" Text="*" Display="Static" 
                                                        ErrorMessage="You must accept the terms and conditions." ClientValidationFunction="validateTC"
                                                        OnServerValidate="ValidateTC" EnableViewState="false" SetFocusOnError="false" ValidationGroup="OPC"></asp:CustomValidator>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>&nbsp;</td>
                                                <td>
                                                    <asp:LinkButton ID="ShowTCLink" runat="server" EnableViewState="false" Text="view the Terms and Conditions"></asp:LinkButton>
                                                </td>
                                            </tr>
                                        </table>
                                        <asp:Panel ID="TCDialog" runat="server" Style="display: none" CssClass="modalPopup">
                                            <asp:Panel ID="TCDialogHeader" runat="server" CssClass="modalPopupHeader">
                                                <div>
                                                    <p>Terms and Conditions:</p>
                                                </div>
                                            </asp:Panel>
                                            <div class="modalPopupText">
                                                <div class="modalPopupScroller">
                                                    <asp:Literal ID="TermsAndConditionsText" runat="server" EnableViewState="false"></asp:Literal>
                                                    <br /><br />
                                                    <div align="center">
                                                        <asp:LinkButton ID="DeclineTermsAndConditions" runat="server" EnableViewState="false" SkinID="Button" Text="I Decline"></asp:LinkButton>
                                                        <asp:LinkButton ID="AcceptTermsAndConditions" runat="server" EnableViewState="false" SkinID="Button" Text="I Accept"></asp:LinkButton>
                                                    </div><br />
                                                </div>
                                            </div>
                                        </asp:Panel>
                                        <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtender" runat="server" 
                                            TargetControlID="ShowTCLink"
                                            PopupControlID="TCDialog" 
                                            BackgroundCssClass="modalBackground" 
                                            OkControlID="AcceptTermsAndConditions"
                                            OnOkScript="toggleTC(true)"
                                            OnCancelScript="toggleTC(false)"
                                            CancelControlID="DeclineTermsAndConditions" 
                                            DropShadow="true"                                            
                                            PopupDragHandleControlID="TCDialogHeader" />
                                            <br />
                                    </asp:PlaceHolder>
                                    <asp:ValidationSummary ID="PaymentValidationSummary" runat="server" EnableViewState="false" ValidationGroup="OPC" />
                                    <ajax:UpdatePanel ID="PaymentAjax" runat="server">
                                        <ContentTemplate>
                                            <table cellpadding="0" cellspacing="0" class="opcPaymentFrame">
                                                <tr>
                                                    <td id="tdPaymentMethodList" runat="server" class="opcPaymentMethods" valign="top">
                                                        <asp:RadioButtonList ID="PaymentMethodList" runat="server" DataTextField="Value" DataValueField="Key" AutoPostBack="true"></asp:RadioButtonList>
                                                    </td>
                                                    <td valign="top" class="opcPaymentForm">
                                                        <asp:PlaceHolder ID="phPaymentForms" runat="server" EnableViewState="False"></asp:PlaceHolder>
                                                    </td>
                                                </tr>
                                            </table>
                                        </ContentTemplate>
                                    </ajax:UpdatePanel>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td valign="top" class="opcSidebar">
                        <uc:BasketTotalSummary ID="BasketTotalSummary1" runat="server"/>
                        <asp:Panel ID="GiftWrapPanel" runat="server" CssClass="section" EnableViewState="false">
                            <div class="header"><h2>Gift Options</h2></div>
                            <div class="onePageCheckoutCell" align="center">
                                Are you sending a gift?<br /><br />
                                <asp:HyperLink ID="EditGiftOptionsLink" runat="server" SkinID="Button" Text="Edit Gift Options" EnableViewState="false"></asp:HyperLink>
                            </div>
                        </asp:Panel>
                        <asp:Panel ID="ShipMultiPanel" runat='server' CssClass="section" EnableViewState="false">
                            <div class="header"><h2>Multiple Destinations?</h2></div>
                            <div class="onePageCheckoutCell">
                                <p align="justify">
                                Click below to send your items to more than one location.<asp:Localize ID="ShipMultiRegisterMessage" runat="server" Text="  You will be asked to register or log to use this feature." EnableViewState="False"></asp:Localize>
                                </p><br />
                                <div align="center">
                                    <asp:HyperLink ID="MultipleAddressLink" runat="server" Text="Multiple Destinations" SkinID="Button" NavigateUrl="~/Checkout/ShipAddresses.aspx" EnableViewState="false"></asp:HyperLink>
                                </div>
                            </div>
                        </asp:Panel>
                        <uc:CouponDialog ID="CouponDialog1" runat="server" EnableViewState="false" />
						<br/>
						<br/>
						<br/>
						<asp:Panel ID="CheckoutMessagePanel" runat="server" CssClass="section" EnableViewState="false" Visible="false">
                            <div class="header"><h2>Checkout Message</h2></div>
                            <div class="onePageCheckoutCell" align="left">
                                <asp:Label Id="CheckoutMessage" runat="server" Text="" CssClass="errorCondition" />
                                <asp:Localize ID="RefreshRatesMessage" runat="server" Text="<br /><br />If any or all of the shipping methods are missing, there may have been a problem communicating with the shipper.  <a href='default.aspx'>Click here</a> to restart the checkout."></asp:Localize>
                            </div>
                        </asp:Panel>
                    </td>
                </tr>
                <tr id="trItemGrid" runat="server" enableviewstate="false">
                    <td colspan="2">
                        <br />
                        <div class="section">
                            <div class="header"><h2>ORDER CONTENTS</h2></div>
                            <div class="content" style="padding:0px;">
                                <asp:GridView ID="BasketGrid" runat="server" AutoGenerateColumns="False"
                                    ShowFooter="False" Width="100%" SkinID="PagedList">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Shipment">
                                            <ItemStyle HorizontalAlign="Center" />
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <%# GetShipmentNumber(Container.DataItem) %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="SKU">
                                            <ItemStyle HorizontalAlign="Center" />
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <%# ProductHelper.GetSKU(Container.DataItem) %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Item">
                                            <ItemStyle HorizontalAlign="Left" />
                                            <ItemTemplate>
                                                <asp:PlaceHolder ID="ProductPanel" runat="server" Visible='<%#((OrderItemType)Eval("OrderItemType") == OrderItemType.Product)%>'>
                                                    <uc:BasketItemDetail id="BasketItemDetail1" runat="server" BasketItem='<%#(BasketItem)Container.DataItem%>' ShowAssets="true" ShowSubscription="true" LinkProducts="false" /><br />
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="OtherPanel" runat="server" Visible='<%#((OrderItemType)Eval("OrderItemType") != OrderItemType.Product)%>' EnableViewState="false">
                                                    <%# Eval("Name") %>
                                                </asp:PlaceHolder>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Tax">
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" Width="50px" />
                                            <ItemTemplate>
                                                <%#TaxHelper.GetTaxRate((BasketItem)Container.DataItem).ToString("0.####")%>%
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Price">
                                            <HeaderStyle HorizontalAlign="Right" />
                                            <ItemStyle HorizontalAlign="Right" Width="80px" />
                                            <ItemTemplate>
                                                <%#TaxHelper.GetInvoicePrice(Token.Instance.User.Basket, (BasketItem)Container.DataItem).ToString("ulc")%>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Qty">
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
                                            <ItemTemplate>
                                                <%# Eval("Quantity") %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Total">
                                            <HeaderStyle HorizontalAlign="Right" />
                                            <ItemStyle HorizontalAlign="Right" Width="80px" />
                                            <ItemTemplate>
                                                <%#TaxHelper.GetInvoiceExtendedPrice(Token.Instance.User.Basket, (BasketItem)Container.DataItem).ToString("ulc")%>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </td>
                </tr>
            </table>
	    </asp:PlaceHolder>
	    <asp:Panel runat="server" ID="BotMessagePanel" Visible="false" >
	        <div class="checkoutPageHeader">
                <h1>Bot Activity Detected!</h1>
                <div class="checkoutAlert">
                    <asp:Label runat="server" ID="BotMessage" Text="" />
                </div>
            </div>
	    </asp:Panel>
        <asp:HiddenField ID="VS_CustomState" runat="server" EnableViewState="false" />
        <ajaxToolkit:NoBot
          ID="OPCNotBot"
          runat="server"  
          ResponseMinimumDelaySeconds="0"
          CutoffWindowSeconds="60"
          CutoffMaximumInstances="10" />        
    </ContentTemplate>
</ajax:UpdatePanel>
