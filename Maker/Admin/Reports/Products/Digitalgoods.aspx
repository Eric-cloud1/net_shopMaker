<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"
AutoEventWireup="true" CodeFile="Digitalgoods.aspx.cs"
Inherits="Admin_Reports_Products_Digitalgoods" Title="Untitled Page" %> <%@
Register src="../../UserControls/DatesAndTime.ascx" tagname="DatesAndTime"
tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
  <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
      <div class="pageHeader">
        <div class="caption">
          <h1>Digital Goods</h1>
        </div>
      </div>
      <table cellpadding="10" cellspacing="0" width="950px">
        <uc1:DatesAndTime ID="dtDigitalgoods" runat="server" />

        <tr>
          <td style="text-align: left; width: 150px">
            <asp:Label
              ID="lbSelectedGood"
              runat="server"
              Text="Selected Good: "
              SkinID="FieldHeader"
            />
          </td>
          <td colspan="3">
            <asp:DropDownList
              ID="digitalGoods"
              runat="server"
            ></asp:DropDownList>
          </td>
        </tr>
        <tr>
          <td colspan="4"><div style="height: 25px">&nbsp;</div></td>
        </tr>

        <tr>
          <td colspan="1">
            <asp:Label
              ID="lbEmailReport"
              runat="server"
              Text="Email Report: "
              SkinID="FieldHeader"
            />
          </td>
          <td colspan="1" align="left">
            <asp:ImageButton
              ID="ProcessButto"
              runat="server"
              ImageUrl="~/App_Themes/MakerShopAdmin/images/BtnExcel.gif"
              OnClick="ProcessButton_Click"
            />
          </td>
          <td colspan="2">&nbsp;</td>
        </tr>
      </table>
    </ContentTemplate>
  </ajax:UpdatePanel>
</asp:Content>
