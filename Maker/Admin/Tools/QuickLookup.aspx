<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="QuickLookup.aspx.cs" Inherits="Admin_Tools_QuickLookup" Title="Untitled Page" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">
 <div class="pageHeader">
        <div class="caption">
            <h1>
                <asp:Localize ID="Caption" runat="server" Text="Quick lookup"></asp:Localize>
            </h1>
        </div>
    </div>
    <ajax:UpdatePanel runat="server" ID="up" UpdateMode="Conditional">
        <ContentTemplate>
 <table style="width: 100%;width:700px">
  <tr><td style="height:25px;" colspan="2">&nbsp;</td></tr>
        <tr>
          <asp:Panel id="orderNumberDetails" runat="server">
        <td>&nbsp;&nbsp;&nbsp;</td>
            <td style="text-align: left;">&nbsp;&nbsp;<asp:Label ID="lblOrderNumber" runat="server" Text="Order Number:" style="font-size:12px;font-weight: bold; padding-bottom: 3px;" /><br />
                <asp:TextBox ID="orderNumber" Width="250px"  runat="server" />   
            </td>
            <td style="text-align:left;" valign="bottom">
              <asp:Button ID="getOrderNumber" runat="server" 
                    style="color:#134FA3;font-size:12px;font-weight:bold;border:1px solid #7D90B1;height:23px;padding:2px 2px;width:200px" 
                    Text="Get Order" onclick="orderNumber_Click" />
            </td>
            </asp:Panel>
        </tr>
             <asp:Panel id="orderHistoryDetails" runat="server">
          <tr><td>&nbsp;&nbsp;&nbsp;</td>
            <td style="text-align: left;">&nbsp;&nbsp;<asp:Label ID="lblOrderNumber2" runat="server" Text="Order Number History:" style="font-size:12px;font-weight: bold; padding-bottom: 3px;" /><br />
                <asp:TextBox ID="orderNumberHistory" Width="250px" runat="server" />   
            </td>
            <td style="text-align:left;" valign="bottom">
              <asp:Button ID="getOrderNumberHistory" runat="server" 
                    style="color:#134FA3;font-size:12px;font-weight:bold;border:1px solid #7D90B1;height:23px;padding:2px 2px;width:200px" 
                    Text="Get Order History" onclick="orderNumberHistory_Click" />
            </td>
        </tr>
        </asp:Panel>
        <asp:Panel id="orderIdDetails" runat="server">
        <tr><td>&nbsp;&nbsp;&nbsp;</td>
            <td style="text-align: left;">&nbsp;&nbsp;<asp:Label ID="lblOrderId" runat="server" Text="Order Id:" style="font-size:12px;font-weight: bold; padding-bottom: 3px;" /><br />
                <asp:TextBox ID="orderId" Width="250px" runat="server" />   
            </td>
            <td style="text-align:left;" valign="bottom">
              <asp:Button ID="getOrderId" runat="server" 
                    style="color:#134FA3;font-size:12px;font-weight:bold;border:1px solid #7D90B1;height:23px;padding:2px 2px;width:200px" 
                    Text="Get Order Id" onclick="orderId_Click" />
            </td>
        </tr>
        </asp:Panel>
        <asp:Panel id="paymentDetails" runat="server">
        <tr><td>&nbsp;&nbsp;&nbsp;</td>
            <td style="text-align: left;">&nbsp;&nbsp;<asp:Label ID="lblPaymentId" runat="server" Text="Payment Id:" style="font-size:12px;font-weight: bold; padding-bottom: 3px;" /><br />
                <asp:TextBox ID="paymentId" Width="250px" runat="server" />   
            </td>
            <td style="text-align:left;" valign="bottom">
              <asp:Button ID="getPaymentId" runat="server" 
                    style="color:#134FA3;font-size:12px;font-weight:bold;border:1px solid #7D90B1;height:23px;padding:2px 2px;width:200px" 
                    Text="Get Payment" onclick="paymentId_Click" />
            </td>
        </tr>
        </asp:Panel>
        <asp:Panel id="transactionDetails" runat="server">
          <tr><td>&nbsp;&nbsp;&nbsp;</td>
            <td style="text-align: left;">&nbsp;&nbsp;<asp:Label ID="lblTransactionId" runat="server" Text="Transaction Id:" style="font-size:12px;font-weight: bold; padding-bottom: 3px;width:200px" /><br />
                <asp:TextBox ID="transactionId" Width="250px" runat="server" />   
            </td>
            <td style="text-align:left;" valign="bottom">
              <asp:Button ID="getTransactionId" runat="server" 
                    style="color:#134FA3;font-size:12px;font-weight:bold;border:1px solid #7D90B1;height:23px;padding:2px 2px;width:200px" 
                    Text="Get Transaction" onclick="transactionId_Click" />
            </td>
        </tr>
        </asp:Panel>
        
        <asp:Panel id="productDetails" runat="server">
            <tr><td>&nbsp;&nbsp;&nbsp;</td>
            <td style="text-align: left;" >&nbsp;&nbsp;<asp:Label ID="lblProductId" runat="server" Text="Product Id:" style="font-size:12px;font-weight: bold; padding-bottom: 3px;" /><br />
                <asp:TextBox ID="productId" Width="250px" runat="server" />   
            </td>
            <td style="text-align:left;" valign="bottom">
              <asp:Button ID="getProductId" runat="server" 
                    style="color:#134FA3;font-size:12px;font-weight:bold;border:1px solid #7D90B1;height:23px;padding:2px 2px;width:200px" 
                    Text="Get Product" onclick="productId_Click" />
            </td>
        </tr>
        </asp:Panel>
        <tr><td style="height:100px;" colspan="2">&nbsp;</td></tr>
</table>


        
       </ContentTemplate>

    </ajax:UpdatePanel>

</asp:Content>
