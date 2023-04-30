<%@ Control Language="C#" AutoEventWireup="true" CodeFile="downloadbutton.ascx.cs" Inherits="Admin_UserControls_downloadbutton" %>
<div id="buttons">
 <asp:ImageButton ID="btnCSV" OnClientClick="downloadCSV()" runat="server" ImageUrl="~/App_Themes/MakerShopAdmin/images/CSV.png" />&nbsp;&nbsp;&nbsp;
<asp:ImageButton ID="btnExcel" OnClientClick="downloadXml()" runat="server" ImageUrl="~/App_Themes/MakerShopAdmin/images/XML.png"  />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
  <cb:ToolTipLabel ID="ToolTipLabel1" runat="server" Text="Email:" ToolTip="Check to have the report emailed to you. Keep unchecked to download it." />
                <asp:CheckBox ID="cbEmail" runat="server" Text="" Checked="true" />
</div>
