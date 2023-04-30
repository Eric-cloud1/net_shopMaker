<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="Returns.aspx.cs" Inherits="Admin_Tools_Shipping_Returns" Title="Untitled Page" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">

 <div class="pageHeader">
        <div class="caption">
            <h1>
                <asp:Localize ID="Caption" runat="server" Text="Returns"></asp:Localize>
            
            </h1>
        </div>
    </div>
    <ajax:UpdatePanel runat="server" ID="up" UpdateMode="Conditional">
        <ContentTemplate>
        <asp:Panel ID="SubmitTracking"  DefaultButton="submit" runat="server">
          
 <table style="width: 100%;width:400px">
  <tr><td style="height:75px;">&nbsp;</td></tr>
        <tr>
            <td style="text-align: left;">&nbsp;&nbsp;<asp:Label ID="lblTracking" runat="server" Text="Tracking:" style="font-size:12px;font-weight: bold; padding-bottom: 3px;" />&nbsp;&nbsp;
                <asp:TextBox ID="trackingNumber" Width="250px" runat="server" />   
            </td>
            <td style="text-align:left;">
              <asp:Button ID="submit" runat="server" 
                    style="color:#134FA3;font-size:12px;font-weight:bold;border:1px solid #7D90B1;height:23px;padding:2px 2px;" 
                    Text="Submit" onclick="submit_Click" />
            </td>
        </tr>
        <tr><td style="height:100px;">&nbsp;</td></tr>
</table>
<asp:Label ID="SavedMessage" runat="server" text="Return updated at {0:t}<br/>{1}" Visible="false" Width="100%" EnableViewState="false" style="font-size:15px;font-weight: bold; padding-bottom: 3px;">
          </asp:Label>
</asp:Panel>
        
       </ContentTemplate>

    </ajax:UpdatePanel>

</asp:Content>


