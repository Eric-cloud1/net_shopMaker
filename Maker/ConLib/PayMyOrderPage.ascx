<%@ Control Language="C#" AutoEventWireup="true" CodeFile="PayMyOrderPage.ascx.cs" Inherits="ConLib_PayMyOrderPage" EnableViewState="false" %>
<%--
<conlib>
<summary>Customer account page that displays a form where customer can pay his due balance for an order.</summary>
</conlib>
--%>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
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
<%@ Register Src="~/ConLib/Utility/OrderItemDetail.ascx" TagName="OrderItemDetail" TagPrefix="uc" %>
<ajax:UpdatePanel ID="PaymentAjaxPanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div class="checkoutPageHeader">
            <h1><asp:Localize ID="Caption" runat="server" Text="Make Payment for Order #{0}"></asp:Localize></h1>
            <asp:PlaceHolder ID="PaymentFailedPanel" runat="server" Visible="false" EnableViewState="false">
            <div class="checkoutAlert">
                Oops! We could not process your last payment. The bank said: <asp:Literal ID="PaymentFailedReason" runat="server" EnableViewState="false"></asp:Literal><br /><br />
                Your order has been placed, but it will not be processed until payment is completed.
            </div>
            </asp:PlaceHolder>
        </div>
        <!-- split page into columns -->
        <table cellpadding="0" cellspacing="0" class="opcFrame">
            <tr>
                <td valign="top" class="opcMainPanel">
                    <br />
                    <h2 class="sectionHeader"><asp:Localize ID="BalanceLabel" runat="server" Text="BALANCE DUE: "></asp:Localize><asp:Literal ID="Balance" runat="server" Text=""></asp:Literal></h2>
                    <asp:PlaceHolder ID="TooManyTriesPanel" runat="server" Visible="false">
                        <asp:Label ID="TooManyTriesMessage" runat="server" SkinID="ErrorCondition" Text="There have been too many failed payment attempts.  Please contact us for assistance in completing your order."></asp:Label>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="PaymentPanel" runat="server">
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
					                <asp:Label ID="BillToCountryLabel" runat="server" Text="Country:" AssociatedControlID="BillToCountry" EnableViewState="false"></asp:Label>
				                </th>
				                <td>
					                <asp:DropDownList ID="BillToCountry" runat="server" DataTextField="Name" DataValueField="CountryCode" 
						                AutoPostBack="true" EnableViewState="false" Width="200px"></asp:DropDownList>
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
					                <asp:Label ID="BillToPhoneLabel" runat="server" Text="Phone:" AssociatedControlID="BillToPhone" EnableViewState="false"></asp:Label>
				                </th>
				                <td>
					                <asp:TextBox ID="BillToPhone" runat="server" EnableViewState="false" Width="200px" MaxLength="30"></asp:TextBox> 
					                <asp:RequiredFieldValidator ID="BillToPhoneRequired" runat="server" Text="*"
						                ErrorMessage="Phone number number is required." Display="Static" ControlToValidate="BillToPhone"
						                EnableViewState="False" SetFocusOnError="false" ValidationGroup="OPC"></asp:RequiredFieldValidator>
				                </td>
			                </tr>
			            </table><br />
                        <ajax:UpdatePanel ID="PaymentAjax" runat="server">
                            <ContentTemplate>
                                <asp:ValidationSummary ID="PaymentValidationSummary" runat="server" EnableViewState="false" ValidationGroup="OPC" />
                                <table cellpadding="0" cellspacing="0" class="opcPaymentFrame">
                                    <tr>
                                        <td id="tdPaymentMethodList" runat="server" class="opcPaymentMethods" valign="top">
                                            <asp:RadioButtonList ID="PaymentMethodList" runat="server" DataTextField="Value" DataValueField="Key" AutoPostBack="true"></asp:RadioButtonList>
                                        </td>
                                        <td valign="top" class="opcPaymentForm">
                                            <asp:PlaceHolder ID="PaymentErrorPanel" runat="server" Visible="False">
                                                <div style="padding-left:8px;"><asp:Label ID="PaymentErrorMessage" runat="server" SkinID="ErrorCondition"></asp:Label></div>
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder ID="phPaymentForms" runat="server" EnableViewState="False"></asp:PlaceHolder>
                                        </td>
                                    </tr>
                                </table>
                            </ContentTemplate>
                        </ajax:UpdatePanel>
                    </asp:PlaceHolder>
                </td>
		        <td valign="top" class="opcSidebar">
			        <asp:Label ID="OrderNumberLabel" runat="server" Text="Order Number:" SkinID="FieldHeader"></asp:Label><br />
			        <asp:Literal ID="OrderNumber" runat="server" Text=""></asp:Literal><br /><br />
			        <asp:Label ID="OrderDateLabel" runat="server" Text="Order Date:" SkinID="FieldHeader"></asp:Label><br />
			        <asp:Literal ID="OrderDate" runat="server" Text=""></asp:Literal><br /><br />
			        <asp:Label ID="OrderStatusLabel" runat="server" Text="Status:" SkinID="FieldHeader"></asp:Label><br />
			        <asp:Literal ID="OrderStatus" runat="server" Text=""></asp:Literal><br /><br />
			        <asp:Label ID="TotalChargesLabel" runat="server" Text="Order Total:" SkinID="FieldHeader"></asp:Label><br />
			        <asp:Literal ID="TotalCharges" runat="server" Text=""></asp:Literal><br /><br />
			        <asp:Label ID="TotalPaymentsLabel" runat="server" Text="Payments:" SkinID="FieldHeader"></asp:Label><br />
			        <asp:Literal ID="TotalPayments" runat="server" Text=""></asp:Literal>
		        </td>
            </tr>
            <tr>
               <td colspan="2">
                    <br />
                    <div class="section">
                        <div class="header"><h2>PAYMENT HISTORY</h2></div>
                        <div class="content" style="padding:0px;">
                            <asp:GridView ID="PaymentGrid" runat="server" AutoGenerateColumns="False"
                                ShowFooter="False" Width="100%" SkinID="PagedList">
                                <Columns>
                                    <asp:TemplateField HeaderText="Date">
                                        <ItemStyle HorizontalAlign="Center" />
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemTemplate>
                                            <%# Eval("PaymentDate") %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Type">
                                        <ItemStyle HorizontalAlign="Left" />
                                        <ItemTemplate>
                                            <%# GetPaymentName(Container.DataItem) %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Amount">
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Right" />
                                        <ItemTemplate>
                                            <%# Eval("Amount", "{0:lc}") %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Status">
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                        <ItemTemplate>
                                            <%# GetPaymentStatus(Container.DataItem) %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Reason">
                                        <ItemStyle HorizontalAlign="Left" />
                                        <ItemTemplate>
                                            <%# GetPaymentStatusReason(Container.DataItem) %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <br />
                    <div class="section">
                        <div class="header"><h2>ORDER CONTENTS</h2></div>
                        <div class="content" style="padding:0px;">
                            <asp:GridView ID="BasketGrid" runat="server" AutoGenerateColumns="False"
                                ShowFooter="False" Width="100%" SkinID="PagedList">
                                <Columns>
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
                                                <uc:OrderItemDetail id="OrderItemDetail1" runat="server" OrderItem='<%#(OrderItem)Container.DataItem%>' ShowAssets="true" ShowSubscription="true" LinkProducts="false" /><br />
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
                                            <%#TaxHelper.GetTaxRate(Order, (OrderItem)Container.DataItem).ToString("0.####")%>%
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Price">
                                        <HeaderStyle HorizontalAlign="Right" />
                                        <ItemStyle HorizontalAlign="Right" />
                                        <ItemTemplate>
                                            <%#TaxHelper.GetInvoicePrice(Order, (OrderItem)Container.DataItem).ToString("ulc")%><br />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Qty">
                                        <ItemStyle HorizontalAlign="Center" />
                                        <ItemTemplate>
                                            <%# Eval("Quantity") %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Total">
                                        <HeaderStyle HorizontalAlign="Right" />
                                        <ItemStyle HorizontalAlign="Right" />
                                        <ItemTemplate>
                                            <%#TaxHelper.GetInvoiceExtendedPrice(Order, (OrderItem)Container.DataItem).ToString("ulc")%><br />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </td>
            </tr>
       </table>
    </ContentTemplate>
</ajax:UpdatePanel>