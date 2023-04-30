<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MyAddressBookPage.ascx.cs" Inherits="ConLib_MyAddressBookPage" %>
<%--
<conlib>
<summary>Displays the cutomer address book with all the shipping and billing addresses.</summary>
</conlib>
--%>
<div class="pageHeader">
    <h1><asp:Localize ID="Caption" runat="server" Text="{0}'s Address Book"></asp:Localize></h1>
</div>
<table class="addressBook" align="center" cellpadding="0" cellspacing="2" border="0">
    <tr>    
        <td class="entries">
            <div class="section">
                <div class="header">
                    <h2>Billing and Default Shipping Address</h2>
                </div>
                <div class="entry">
                    <div class="buttons">
                        <asp:LinkButton ID="EditPrimaryAddressButton" runat="server" OnClick="EditPrimaryAddressButton_Click" SkinID="Button" Text="Edit"></asp:LinkButton>
                    </div>
                    <div class="address">
                        <asp:Literal ID="PrimaryAddress" runat="server" ></asp:Literal>
                    </div>
                </div>
             </div>
        </td>
    </tr>
    <tr>
        <td class="entries">
             <div class="section">
                <div class="header">
                    <h2>Other Shipping Address(es)</h2>
                </div>
            <asp:Repeater ID="AddressList" runat="server" OnItemCommand="AddressList_ItemCommand">
                <ItemTemplate>
                    <div class="entry">
                        <div class="buttons">
                            <asp:LinkButton ID="EditAddressButton" runat="server" CommandArgument='<%#Eval("AddressId")%>' CommandName="Edit" SkinID="Button" Text="Edit"></asp:LinkButton>
                                <asp:LinkButton ID="DeleteAddressButton" runat="server" SkinID="Button" Text="Delete" CommandName="Delete" CommandArgument='<%#Eval("AddressId")%>' OnClientClick='<%# Eval("FullName", "return confirm(\"Are you sure you want to delete {0}?\")") %>'></asp:LinkButton>
                        </div>
                        <div class="address">
                            <asp:Literal ID="Address" runat="server" Text='<%#Container.DataItem.ToString().Trim() == ","? "Please fill in your primary address.":Container.DataItem.ToString() %>'></asp:Literal>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <div class="entry">
                <div class="buttons">
                    <asp:LinkButton ID="NewAddressButton" runat="server" SkinId="Button" Text="New" OnClick="NewAddressButton_Click" CausesValidation="false"></asp:LinkButton>
                </div>
                <div class="address">
                    &nbsp;
                </div>
            </div>
              </div>
         </td>
    </tr>
</table>
