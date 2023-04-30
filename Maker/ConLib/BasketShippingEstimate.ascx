<%@ Control Language="C#" AutoEventWireup="true" CodeFile="BasketShippingEstimate.ascx.cs" Inherits="ConLib_BasketShippingEstimate" EnableViewState="false" %>
<%-- 
<conlib>
<summary>A sidebar control to estimate shipping charges for the current user's basket.</summary>
<param name="Caption" default="Shipping Estimate">Possible value can be any string.  Title of the control.</param>
<param name="InstructionText" default="To estimate shipping charges for this order, enter the delivery address below.">Possible value cab be any suitable text.</param>
<param name="ShowCity" default="false">Possible value can be "true" or "false". Indicates whether the city field should be displayed or not.</param>
<param name="AssumeCommercialRates" default="false">Possible value can be "true" or "false". Indicates whether the control assumes commercial rates for anonymous users.  Default value is false, which assumes resdential rates for anonymous users.</param>
</conlib>
--%>
<!-- BEGIN CONLIB/BASKETSHIPPINGESTIMATE.ASCX -->
<asp:Panel ID="ShippingEstimatePanel" runat="server" CssClass="section" DefaultButton="SubmitButton">
    <div class="header">
        <h2><asp:Localize ID="phCaption" runat="server" Text="Shipping Estimate"></asp:Localize></h2>
    </div>
    <div class="shippingEstimateCell">
        <asp:Localize ID="phInstructionText" runat="server" Text="To estimate shipping charges for this order, enter the delivery address below."></asp:Localize><br />
        <ajax:UpdatePanel ID="EstimateForm" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="Estimate" />
                <asp:Label ID="CountryLabel" runat="server" Text="Country:" AssociatedControlID="Country" CssClass="H2"></asp:Label>
                <asp:DropDownList ID="Country" runat="server" DataTextField="Name" DataValueField="CountryCode" AutoPostBack="true" Width="150px" />
                <asp:PlaceHolder ID="phProvinceField" runat="server" Visible="false">
                    <br /><asp:Label ID="ProvinceLabel" runat="server" Text="State:" AssociatedControlID="Province" CssClass="H2"></asp:Label>
                    <asp:RequiredFieldValidator ID="ProvinceRequired" runat="server" Text="*"
                        ErrorMessage="State is required." Display="Static" ControlToValidate="Province" ValidationGroup="Estimate"></asp:RequiredFieldValidator>
                    <br /><asp:DropDownList ID="Province" runat="server" Width="150px"></asp:DropDownList>                                
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phCityField" runat="server" Visible="false">
                    <br /><asp:Label ID="CityLabel" runat="server" Text="City:" AssociatedControlID="City" CssClass="H2"></asp:Label>
                    <asp:RequiredFieldValidator ID="CityRequired" runat="server" Text="*"
                        ErrorMessage="City is required." Display="Static" ControlToValidate="City" ValidationGroup="Estimate"></asp:RequiredFieldValidator>
                     <br /><asp:TextBox ID="City" runat="server" MaxLength="30" Width="150px" ></asp:TextBox> 
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phPostalCodeField" runat="server" Visible="false">
                    <br /><asp:Label ID="PostalCodeLabel" runat="server" Text="ZIP Code:" AssociatedControlID="PostalCode" CssClass="H2"></asp:Label>
                    <asp:PlaceHolder ID="phPostalCodeValidator" runat="server"></asp:PlaceHolder>
                    <asp:RequiredFieldValidator ID="PostalCodeRequired" runat="server" Text="*" ErrorMessage="ZIP code is required."
                        ControlToValidate="PostalCode" ValidationGroup="Estimate"></asp:RequiredFieldValidator>
                    <br /><asp:TextBox ID="PostalCode" runat="server" MaxLength="14" Width="150px"></asp:TextBox>
                </asp:PlaceHolder>
                <br /><asp:Button ID="SubmitButton" Text="Go" runat="server" OnClick="SubmitButton_Click" ValidationGroup="Estimate"></asp:Button>
                <asp:PlaceHolder ID="phResultPanel" runat="server" Visible="false">
                    <hr />
                    <asp:PlaceHolder ID="MultipleShipmentsMessage" runat="server" Visible="false">
                        Your order contains items that must be sent in more than one shipment.<br />
                    </asp:PlaceHolder>
                    <asp:Repeater ID="ShipmentList" runat="server" OnItemDataBound="ShipmentList_ItemDataBound">
                        <ItemTemplate>
                            <asp:PlaceHolder ID="MultiShipmentHeader" runat="server" Visible="false">
                                <b>Shipment <%# (Container.ItemIndex + 1) %>:</b><br />
                                <asp:Label ID="ItemsCaption" runat="server" Text="Items" SkinID="FieldHeader"></asp:Label>
                                <asp:Repeater ID="ItemsRepeater" runat="server" DataSource='<%#GetShipmentProducts(Container.DataItem)%>'>
                                    <HeaderTemplate><ul class="orderItemsList"></HeaderTemplate>
                                    <ItemTemplate><li><%#Eval("Quantity")%> of: <%#Eval("Name")%></li></ItemTemplate>
                                    <FooterTemplate></ul></FooterTemplate>
                                </asp:Repeater>
                            </asp:PlaceHolder>
                            <asp:GridView ID="ShipRateGrid" runat="server" AutoGenerateColumns="false" Width="100%" GridLines="none">
                                <Columns>
                                    <asp:BoundField DataField="Name" HeaderText="Method" HeaderStyle-HorizontalAlign="Left" />
                                    <asp:TemplateField HeaderText="Rate">
                                        <HeaderStyle HorizontalAlign="Right" />
                                        <ItemStyle HorizontalAlign="Right" />
                                        <ItemTemplate><%# Eval("TotalRate", "{0:ulc}") %></ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    <asp:Label ID="NoShipRatesMessage" runat="server" Text="No shipping methods are available for the current items and/or the given destination."></asp:Label>
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </ItemTemplate>
                    </asp:Repeater>
                </asp:PlaceHolder>
            </ContentTemplate>
        </ajax:UpdatePanel>
    </div>
</asp:Panel>
<!-- END CONLIB/BASKETSHIPPINGESTIMATE.ASCX -->