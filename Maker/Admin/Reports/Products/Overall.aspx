<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"
AutoEventWireup="true" CodeFile="Overall.aspx.cs"
Inherits="Admin_Reports_Products_Overall" Title="Untitled Page" %> <%@ Register
src="../../UserControls/DatesAndTime.ascx" tagname="DatesAndTime"
tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
  <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
      <div class="pageHeader">
        <div class="caption">
          <h1>Overall</h1>
        </div>
      </div>
      <table
        align="center"
        class="form"
        cellpadding="0"
        cellspacing="0"
        border="0"
      >
        <uc1:DatesAndTime ID="dtOverall" runat="server" />

        <tr>
          <td>
            <asp:Label
              ID="lbSelectedproduct"
              runat="server"
              Text="Selected Product: "
              SkinID="FieldHeader"
            />
          </td>
          <td colspan="3">
            <asp:DropDownList ID="productIds" runat="server"></asp:DropDownList>
          </td>
        </tr>
        <tr>
          <td colspan="4"><div style="height: 10px">&nbsp;</div></td>
        </tr>
        <tr>
          <td>
            <asp:ImageButton
              ID="ExcelButton"
              runat="server"
              ImageUrl="~/App_Themes/MakerShopAdmin/images/BtnExcel.gif"
              OnClick="EmailButton_Click"
            />
          </td>
          <td>
            <asp:Button
              ID="ProcessButton"
              runat="server"
              Text="GO.."
              OnClick="ProcessButton_Click"
            />
          </td>
          <td
            colspan="2"
            style="text-align: left; vertical-align: middle; width: 50%"
          >
            &nbsp;
          </td>
        </tr>
        <tr>
          <td colspan="4"><div style="height: 25px">&nbsp;</div></td>
        </tr>
        <tr>
          <td colspan="4">
            <asp:GridView
              ID="OverallGrid"
              runat="server"
              AutoGenerateColumns="False"
              DefaultSortExpression="Name"
              DefaultSortDirection="Ascending"
              AllowPaging="True"
              AllowSorting="true"
              SkinID="Summary"
              PageSize="15"
              OnSorting="OverallGrid_Sorting"
              OnPageIndexChanging="OverallGrid_PageIndexChanging"
              Width="910px"
            >
              <Columns>
                <asp:TemplateField HeaderText="Amount" SortExpression="Amount">
                  <ItemStyle HorizontalAlign="Left" width="200" />
                  <ItemTemplate>
                    <asp:Label
                      ID="lbAmount"
                      runat="server"
                      Text='<%# Eval("Amount", "{0:0,#}")  %>'
                    ></asp:Label>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Count" SortExpression="Count">
                  <ItemStyle HorizontalAlign="center" width="100" />
                  <ItemTemplate>
                    <asp:Label
                      ID="lbCount"
                      runat="server"
                      Text='<%# Eval("Count", "{0:0,#}") %>'
                    ></asp:Label>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField
                  HeaderText="Chargeback Count"
                  SortExpression="ChargeBackCount"
                >
                  <ItemStyle HorizontalAlign="center" width="100" />
                  <ItemTemplate>
                    <asp:Label
                      ID="lbChargebackCount"
                      runat="server"
                      Text='<%# Eval("ChargeBackCount", "{0:0,#}") %>'
                    ></asp:Label>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField
                  HeaderText="PaymentStatus"
                  SortExpression="PaymentStatus"
                >
                  <ItemStyle HorizontalAlign="center" width="100" />
                  <ItemTemplate>
                    <asp:Label
                      ID="lbPaymentStatus"
                      runat="server"
                      Text='<%# Eval("PaymentStatus") %>'
                    ></asp:Label>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField
                  HeaderText="Payment Type"
                  SortExpression="PaymentType"
                >
                  <ItemStyle HorizontalAlign="center" width="100" />
                  <ItemTemplate>
                    <asp:Label
                      ID="lbPaymentType"
                      runat="server"
                      Text='<%# Eval("PaymentType") %>'
                    ></asp:Label>
                  </ItemTemplate>
                </asp:TemplateField>
              </Columns>
              <EmptyDataTemplate>
                <div>
                  <asp:Label
                    ID="EmptyResultsMessage"
                    runat="server"
                    Text="There are no results for the selected time period."
                  ></asp:Label>
                </div>
              </EmptyDataTemplate>
            </asp:GridView>
          </td>
        </tr>
      </table>
    </ContentTemplate>
  </ajax:UpdatePanel>
</asp:Content>
