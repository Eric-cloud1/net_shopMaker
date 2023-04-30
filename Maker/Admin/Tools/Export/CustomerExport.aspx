<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CustomerExport.aspx.cs"
Inherits="Admin_Tools_Export_CustomerExport" Title="Customer Export" %> <%@
Register Src="~/Admin/UserControls/PickerAndCalendar.ascx"
TagName="PickerAndCalendar" TagPrefix="uc1" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb"
%> <%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI"
TagPrefix="ComponentArt" %> <%@ Register Assembly="wwhoverpanel"
Namespace="Westwind.Web.Controls" TagPrefix="wwh" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
  <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
      <div class="pageHeader">
        <div class="caption">
          <h1>
            <asp:Localize
              ID="Caption"
              runat="server"
              Text="Customer Export"
            ></asp:Localize>
            <asp:Localize
              ID="ReportCaption"
              runat="server"
              Visible="False"
              EnableViewState="False"
            ></asp:Localize>
          </h1>
        </div>
      </div>
      <table
        align="center"
        class="form"
        cellpadding="0"
        cellspacing="0"
        border="0"
        width="100%"
      >
        <tr>
          <td style="width: 100%">
            <asp:Label
              ID="SavedMessage"
              runat="server"
              Text="Template saved at {0}."
              EnableViewState="false"
              Visible="false"
              SkinID="GoodCondition"
            ></asp:Label>
            <asp:Label
              ID="ErrorMessageLabel"
              runat="server"
              Text=""
              EnableViewState="false"
              Visible="false"
              SkinID="ErrorCondition"
            ></asp:Label>
          </td>
        </tr>
      </table>
      <table
        align="center"
        class="form"
        cellpadding="0"
        cellspacing="0"
        border="0"
        width="100%"
      >
        <tr>
          <td style="width: 30%">
            <asp:Label
              ID="lbStartDate"
              runat="server"
              Text="Order Start Date: "
              SkinID="FieldHeader"
            ></asp:Label>
            <uc1:PickerAndCalendar ID="fromDate" runat="server" />
            <br />
          </td>
          <td style="width: 30%">
            <asp:Label
              ID="lbEndDate"
              runat="server"
              Text="Order End  Date: "
              SkinID="FieldHeader"
            ></asp:Label>
            <uc1:PickerAndCalendar ID="EndDate" runat="server" />
          </td>
        </tr>
      </table>
      <table
        align="center"
        class="form"
        cellpadding="0"
        cellspacing="0"
        border="0"
        width="100%"
      >
        <tr>
          <td>
            <asp:Label
              ID="reportype"
              runat="server"
              Text="Report Types: "
              SkinID="FieldHeader"
            ></asp:Label>
            <asp:CheckBoxList
              ID="CbReporttype"
              RepeatDirection="Horizontal"
              runat="server"
            >
              <asp:ListItem Value="1" Text="Abandon No Attempt" />
              <asp:ListItem Value="2" Text="Abandon with Attempt" />
              <asp:ListItem Value="3" Text="Sales" />
            </asp:CheckBoxList>
          </td>
        </tr>
        <tr>
          <td>
            <asp:Label
              ID="dupremoval"
              runat="server"
              Text="Duplicate Removal: "
              SkinID="FieldHeader"
            ></asp:Label>
            <asp:CheckBoxList
              ID="Cbdupremoval"
              runat="server"
              RepeatDirection="Vertical"
            >
              <asp:ListItem Value="1">Phone + Country Code</asp:ListItem>
              <asp:ListItem Value="2">Email</asp:ListItem>
              <asp:ListItem Value="2"
                >Postal Code + Country Code + Last Name</asp:ListItem
              >
              <asp:ListItem Value="2"
                >Address1 + Country Code + Last Name</asp:ListItem
              >
            </asp:CheckBoxList>
          </td>
        </tr>
      </table>
      <table
        align="center"
        class="form"
        cellpadding="0"
        cellspacing="0"
        border="0"
        width="100%"
      >
        <tr>
          <td style="width: 10%" align="left">
            <asp:Label
              ID="lbAffiliates"
              Text="Affiliates:"
              runat="server"
              Style="display: inline;
                            font-weight: bold; width: 110px;"
            />
          </td>
          <td>
            <asp:TextBox
              ID="txtAffiliates"
              runat="server"
              Style="display: inline; font-weight: normal;
                            width: 250px;"
            />
            <ajax:TextBoxWatermarkExtender
              ID="twAffiliates"
              runat="server"
              TargetControlID="txtAffiliates"
              WatermarkText="Commas separated affiliate codes..."
            />
          </td>
        </tr>
        <tr>
          <td style="width: 10%" align="left">
            <asp:Label
              ID="lbSubaffiliates"
              Text="Subaffiliates:"
              runat="server"
              Style="display: inline;
                            font-weight: bold; width: 110px;"
            />
          </td>
          <td>
            <asp:TextBox
              ID="txtSubAffiliate"
              runat="server"
              Style="display: inline; font-weight: normal;
                            width: 250px;"
            />
            <ajax:TextBoxWatermarkExtender
              ID="twSubAffiliates"
              runat="server"
              TargetControlID="txtSubAffiliate"
              WatermarkText="Commas separated subaffiliate codes..."
            />
            <asp:CheckBox
              ID="chkSubAffiliate"
              runat="server"
              Checked="true"
              Text="Exact match"
            />
          </td>
        </tr>
      </table>
    </ContentTemplate>
  </ajax:UpdatePanel>
  <table
    align="center"
    class="form"
    cellpadding="0"
    cellspacing="0"
    border="0"
    width="100%"
  >
    <tr>
      <td align="left" style="width: 5%">
        <asp:ImageButton
          ID="btnExcel"
          runat="server"
          ImageUrl="~/App_Themes/MakerShopAdmin/images/BtnExcel.gif"
          OnClick="ExcelReport_Click"
        />
      </td>
      <td>
        <cb:ToolTipLabel
          ID="DateFilterLabel"
          runat="server"
          Text="Email:"
          ToolTip="Check to have the report emailed to you. Keep unchecked to download it."
        />
        <asp:CheckBox ID="cbEmail" runat="server" Text="" Checked="true" />
      </td>
    </tr>
  </table>
</asp:Content>
