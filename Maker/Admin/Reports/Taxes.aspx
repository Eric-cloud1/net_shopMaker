<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"
CodeFile="Taxes.aspx.cs" Inherits="Admin_Reports_Taxes" Title="Tax Summary
Report" %> <%@ Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
  <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
      <div class="pageHeader">
        <div class="caption">
          <h1>
            <asp:Localize
              ID="Localize1"
              runat="server"
              Text="Tax Summary Report"
            ></asp:Localize
            ><asp:Localize
              ID="ReportCaption"
              runat="server"
              EnableViewState="false"
              Text=" {0:d} to {1:d}"
              Visible="false"
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
      >
        <tr class="noPrint">
          <td>
            <asp:Label
              ID="DateFilterLabel"
              runat="server"
              Text="Time Period: "
              SkinID="FieldHeader"
            ></asp:Label>
            <asp:DropDownList
              ID="DateFilter"
              runat="server"
              OnSelectedIndexChanged="DateFilter_SelectedIndexChanged"
              AutoPostBack="true"
            >
              <asp:ListItem Value="0">Today</asp:ListItem>
              <asp:ListItem Value="1">This Week</asp:ListItem>
              <asp:ListItem Value="2">Last Week</asp:ListItem>
              <asp:ListItem Value="3">This Month</asp:ListItem>
              <asp:ListItem Value="4">Last Month</asp:ListItem>
              <asp:ListItem Value="5">Last 15 Days</asp:ListItem>
              <asp:ListItem Value="6">Last 30 Days</asp:ListItem>
              <asp:ListItem Value="7">Last 60 Days</asp:ListItem>
              <asp:ListItem Value="8">Last 90 Days</asp:ListItem>
              <asp:ListItem Value="9">Last 120 Days</asp:ListItem>
              <asp:ListItem Value="10">This Year</asp:ListItem>
              <asp:ListItem Value="11"
                >All Dates</asp:ListItem
              > </asp:DropDownList
            ><br />
          </td>
        </tr>
        <tr>
          <td class="dataSheet">
            <cb:SortedGridView
              ID="TaxesGrid"
              runat="server"
              AutoGenerateColumns="False"
              Width="100%"
              AllowPaging="True"
              PageSize="20"
              AllowSorting="true"
              DefaultSortDirection="ascending"
              DefaultSortExpression="TaxName"
              SkinID="Summary"
              DataSourceId="TaxesDs"
            >
              <Columns>
                <asp:TemplateField
                  HeaderText="Tax Name"
                  SortExpression="TaxName"
                >
                  <headerstyle horizontalalign="Left" />
                  <itemtemplate>
                    <asp:HyperLink
                      ID="TaxName"
                      runat="server"
                      Text='<%# Eval("TaxName") %>'
                      NavigateUrl="<%#GetTaxLink(Container.DataItem)%>"
                    ></asp:HyperLink>
                  </itemtemplate>
                </asp:TemplateField>
                <asp:TemplateField
                  HeaderText="Total Collected"
                  SortExpression="TaxAmount"
                >
                  <headerstyle horizontalalign="right" />
                  <itemstyle horizontalalign="Right" />
                  <itemtemplate>
                    <%# Eval("TaxAmount", "{0:lc}") %>
                  </itemtemplate>
                </asp:TemplateField>
              </Columns>
              <EmptyDataTemplate>
                <div class="emptyResult">
                  <asp:Label
                    ID="EmptyResultsMessage"
                    runat="server"
                    Text="There are no results for the selected time period."
                  ></asp:Label>
                </div>
              </EmptyDataTemplate>
            </cb:SortedGridView>
          </td>
        </tr>
      </table>
      <asp:HiddenField ID="HiddenStartDate" runat="server" Value="" />
      <asp:HiddenField ID="HiddenEndDate" runat="server" Value="" />
      <asp:ObjectDataSource
        ID="TaxesDs"
        runat="server"
        OldValuesParameterFormatString="original_{0}"
        SelectMethod="LoadSummaries"
        SortParameterName="sortExpression"
        EnablePaging="true"
        TypeName="MakerShop.Reporting.TaxReportDataSource"
        SelectCountMethod="CountSummaries"
        DataObjectTypeName="MakerShop.Reporting.TaxReportSummaryItem"
        EnableViewState="false"
      >
        <SelectParameters>
          <asp:ControlParameter
            ControlID="HiddenStartDate"
            Name="startDate"
            PropertyName="Value"
            Type="DateTime"
          />
          <asp:ControlParameter
            ControlID="HiddenEndDate"
            Name="endDate"
            PropertyName="Value"
            Type="DateTime"
          />
        </SelectParameters>
      </asp:ObjectDataSource>
    </ContentTemplate>
  </ajax:UpdatePanel>
</asp:Content>
