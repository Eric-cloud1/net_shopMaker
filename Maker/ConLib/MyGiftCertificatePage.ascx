<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MyGiftCertificatePage.ascx.cs" Inherits="ConLib_MyGiftCertificatePage" %>
<%--
<conlib>
<summary>Display gift certificate details against an order.</summary>
</conlib>
--%>
<div class="noPrint">
    <div class="pageHeader">
        <h1><asp:Localize ID="Caption" runat="server" Text="Gift Certificate - {0}, Order #{1}"></asp:Localize></h1>        
    </div>
</div>    
<div class="noPrint">
    <asp:LinkButton ID="PrintButton" runat="server" Text="Print" SkinID="Button" OnClientClick="window.print();return false;" />
    <asp:LinkButton ID="BackButton" runat="server" Text="Back" SkinID="Button" OnClick="BackButton_Click" />     
</div>
<table align="center" class="form" cellpadding="0" cellspacing="0" style="border:solid 1px">            
    <tr>
        <th colspan="2" style="border-bottom:solid 1px">
            <asp:Localize ID="GiftCertificateSummayCaption" runat="server" Text="GIFT CERTIFICATE DETAILS"></asp:Localize>
        </th>
    </tr>
    <tr>
        <th style="width:33%">Name:</th>
        <td><asp:Label runat="server" ID="Name" ></asp:Label></td>
    </tr>
    <tr>
        <th class="rowHeader">Status Description:</th>
        <td><asp:Label runat="server" ID="Description" ></asp:Label></td>
    </tr>
    <tr>
        <th class="rowHeader">Certificate Number:</th>
        <td><asp:Label runat="server" ID="Serial" ></asp:Label></td>
    </tr>
     <tr>
        <th class="rowHeader">Balance:</th>
        <td><asp:Label runat="server" ID="Balance" ></asp:Label></td>
    </tr>
    <tr>
        <th class="rowHeader">Expiration Date:</th>
        <td><asp:Label runat="server" ID="Expires" ></asp:Label></td>
    </tr>
</table>  