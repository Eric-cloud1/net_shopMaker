<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="EditGatewayGroupTemplate.aspx.cs" Inherits="Admin_Payment_GatewayGroups_EditGatewayGroupTemplate" Title="Untitled Page" %>

<%@ Register Assembly="MetaBuilders.WebControls" Namespace="MetaBuilders.WebControls"
    TagPrefix="mb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
    	<div class="caption">
    		<h1><asp:Localize ID="Caption" runat="server" Text="Payment Group Gateways Templates"></asp:Localize></h1>
    	</div>
    </div>
   <center>
<div style="vertical-align:middle;text-align:left; width:80%;" > 
<table>
    <tr>
        <td colspan="2">
            <asp:Label ID="ErrorMessage" runat="server" SkinID="ErrorCondition" EnableViewState="false"
                Visible="false"></asp:Label>
            <asp:Label ID="AddedMessage" runat="server" Text="Payment Gateway added to group {0}." SkinID="GoodCondition"
                EnableViewState="false" Visible="false"></asp:Label>        </td>
    </tr>
    <tr>
    <td>
    <asp:Label ID="groupLbl" runat="server" Text="Gateway group: " SkinID="FieldHeader"></asp:Label> 
    </td>
    <td>
<asp:DropDownList ID="groupdl" runat="server" AutoPostBack="true" OnSelectedIndexChanged="groupdl_select" />&nbsp;
 <asp:ImageButton ID="SaveButton" runat="server" CausesValidation="False" OnClick="Save_click"  SkinID="SaveIcon" 
  OnClientClick="return confirm('Are you sure you want to save?')" AlternateText="Update group" />
  </td>
  </tr>
  <tr>
  <td colspan="2" valign="top">
   <mb:DualList ID="gatewayGroupLists"  LeftListStyle-Height="500" LeftListStyle-Width="300" ButtonStyle-Width="75" RightListStyle-Width="300" RightListStyle-Height="500" runat="server"/>
   </td>

</tr>
</table> 
</div>
</center>
</asp:Content>

