<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SendWishlistPage.ascx.cs" Inherits="ConLib_SendWishlistPage" %>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
<%--
<conlib>
<summary>Displays a form using that a wishlist link can be sent to a friend email address.</summary>
</conlib>
--%>
<table class="inputForm">
    <tr>        
        <td colspan="2">
            <asp:Literal ID="Instructions" Text="Send your wishlist to a friend using the form below.  You may customize the message, but it may not contain any HTML as the message is sent in plain text." runat="server"></asp:Literal><br />
        </td>
    </tr>
    <tr>
        <td>&nbsp;</td>
        <td>
            <asp:Label ID="SentMessage" runat="server" EnableViewState="false" Visible="false" Text="Message has been sent to {0}" SkinID="GoodCondition"></asp:Label>   
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="SendWishListGroup" />
        </td>
    </tr>
    <tr>
        <th class="rowHeader">       
            <asp:Label ID="SendToLabel" runat="server" Text="To:"></asp:Label>
        </th>
        <td>
            <asp:TextBox ID="SendTo" runat="server" ValidationGroup="SendWishListGroup" Columns="40" MaxLength="200"></asp:TextBox>     
            <cb:EmailAddressValidator ID="ToEmailAddressValidator" runat="server" ControlToValidate="SendTo" ValidationGroup="SendWishListGroup" Display="Dynamic" Required="true" ErrorMessage="'To email' address should be in the format of name@domain.tld." Text="*" EnableViewState="False"></cb:EmailAddressValidator>            
        </td>
    </tr>
    <tr>
        <th class="rowHeader"> 
            <asp:Label ID="FromAddressLabel" runat="server" Text="From:"></asp:Label>            
        </th>
        <td>
            <asp:TextBox ID="FromAddress" runat="server" ValidationGroup="SendWishListGroup" Columns="40" MaxLength="200"></asp:TextBox>
            <cb:EmailAddressValidator ID="FromAddressAddressValidator" runat="server" ControlToValidate="FromAddress" ValidationGroup="SendWishListGroup" Display="Dynamic" Required="true" ErrorMessage="'From email' address should be in the format of name@domain.tld." Text="*" EnableViewState="False"></cb:EmailAddressValidator>            
        </td>
    </tr>
    <tr>
        <th class="rowHeader">            
            <asp:Label ID="SubjectLabel" runat="server" Text="Subject:"></asp:Label>
        </th>
        <td>
            <asp:TextBox ID="Subject" runat="server" ValidationGroup="SendWishListGroup" Columns="40" Text="My Wishlist" MaxLength="100"></asp:TextBox>
            <asp:RequiredFieldValidator ID="SubjectRequired" runat="server" ControlToValidate="Subject"
            ErrorMessage="Subject is Required" ValidationGroup="SendWishListGroup">*</asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <th class="rowHeader" valign="top">
            <asp:Label ID="MessageLabel" runat="server" Text="Message:"></asp:Label>
        </th>
        <td>
            <asp:TextBox ID="Message" runat="server" Height="200px" TextMode="MultiLine" Width="400px" ValidationGroup="SendWishListGroup"></asp:TextBox>
            <asp:RequiredFieldValidator ID="MessageRequired" runat="server" ControlToValidate="Message"
            ErrorMessage="Message is Required" ValidationGroup="SendWishListGroup">*</asp:RequiredFieldValidator><br />
        </td>
    </tr>
    <tr>
        <td>&nbsp;</td>
        <th>
            <asp:Button ID="BackButton" runat="server" Text="Cancel" CausesValidation="False" OnClick="BackButton_Click"/>
            &nbsp;<asp:Button ID="SendButton" runat="server" Text="Send Message" ValidationGroup="SendWishListGroup" OnClick="SendButton_Click" />
        </th>
    </tr>
</table>