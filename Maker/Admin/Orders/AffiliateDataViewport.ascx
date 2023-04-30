<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AffiliateDataViewport.ascx.cs"
    Inherits="Admin_Orders_AffiliateDataViewport" %>
<ajax:updatepanel id="AccountDataAjax" runat="server">
    <ContentTemplate>
    <asp:LinkButton ID="bShowData" runat="server" Text="Show Affiliate Details" OnClick="ShowData_Click" EnableViewState="false" />
<asp:Panel ID="pData" runat="server" EnableViewState="false" Visible="false">
<table>
       <tr>
             <th class="rowHeader">
                <asp:Label ID="OrderReferrerLabel" runat="server" Text="Referrer:" SkinID="FieldHeader"></asp:Label>
            </th>
            <td colspan="5">
               <asp:HyperLink ID="OrderReferrer" runat="server" SkinID="Link"></asp:HyperLink>
            </td>
        </tr>
        <tr>  <th class="rowHeader">
            <asp:Label ID="AffiliateIDLabel" runat="server" Text="AffiliateID:" SkinID="FieldHeader"></asp:Label>
                <br />
                <asp:Label ID="AffiliateLabel" runat="server" Text="Affiliate:" SkinID="FieldHeader"></asp:Label>
                <br />
                <asp:Label ID="SubaffiliateLabel" runat="server" Text="Sub:" SkinID="FieldHeader"></asp:Label>
            </th>
            <td>
              <asp:Label ID="AffiliateID" runat="server" Text=""></asp:Label>
                <br />
                <asp:Label ID="Affiliate" runat="server" Text=""></asp:Label>
                <br />
                <asp:Label ID="Sub" runat="server" Text=""></asp:Label>
            </td>
            </tr>
            </table>
            </asp:Panel>
    </ContentTemplate>
</ajax:updatepanel>
