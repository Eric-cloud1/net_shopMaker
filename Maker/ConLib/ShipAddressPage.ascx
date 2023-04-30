<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ShipAddressPage.ascx.cs" Inherits="ConLib_ShipAddressPage" EnableViewState="false" %>
<%--
<conlib>
<summary>Display page where a customer can select a shipping address, or define multiple shipments and can select their respective addresses.</summary>
</conlib>
--%>

<%@ Register Src="~/ConLib/CheckoutProgress.ascx" TagName="CheckoutProgress" TagPrefix="uc" %>
<div class="checkoutPageHeader">
    <uc:CheckoutProgress ID="CheckoutProgress1" runat="server" />
    <h1><asp:Localize ID="Caption" runat="server" Text="Select Shipping Address"></asp:Localize></h1>
    <div class="content">
        <asp:Label ID="SingleAddressHelpText" runat="server" Text="To ship everything to the same destination, select an address below." Font-Bold="true"></asp:Label>
        <asp:HyperLink ID="MultipleAddressLink" runat="server" Text="Multiple Destinations" NavigateUrl="~/Checkout/ShipAddresses.aspx"></asp:HyperLink> 
    </div>
</div>
<table class="addressBook" align="center" cellpadding="0" cellspacing="2" border="0">
    <tr>
        <th class="caption" valign="middle">
            <asp:Label ID="ShipToCaption" runat="server" Text="{0}'s Address Book" CssClass="text"></asp:Label>
            <span class="buttons"><asp:LinkButton ID="NewAddressButton" runat="server" SkinId="Button" Text="New Address" OnClick="NewAddressButton_Click" CausesValidation="false"></asp:LinkButton></span>
        </th>
    </tr>
    <tr>
        <td class="entries">
            <asp:Repeater ID="ShipToAddressList" runat="server" OnItemCommand="ShipToAddressList_ItemCommand" >
                <ItemTemplate>
                    <div class="entry">
                        <div class="buttons">
                            <asp:LinkButton ID="PickAddressButton" runat="server" SkinID="Button" Text="Ship" CommandName="Pick" CommandArgument='<%#Eval("AddressId")%>'></asp:LinkButton>
                            <asp:LinkButton ID="EditAddressButton" runat="server" CommandArgument='<%#Eval("AddressId")%>' CommandName="Edit" SkinID="Button" Text="Edit"></asp:LinkButton>
                        </div>
                        <div class="address">
                            <asp:Literal ID="Address" runat="server" Text='<%#Container.DataItem.ToString()%>'></asp:Literal>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
         </td>
    </tr>
</table>
