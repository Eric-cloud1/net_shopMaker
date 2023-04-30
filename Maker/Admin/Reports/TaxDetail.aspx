<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"
AutoEventWireup="true" CodeFile="TaxDetail.aspx.cs"
Inherits="Admin_Reports_TaxDetail" Title="Tax Detail Report" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb"
%>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
  <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
      <div class="pageHeader">
        <div class="caption">
          <h1>
            <asp:Localize
              ID="ReportCaption"
              runat="server"
              Text="Tax Detail Report for {0}"
              EnableViewState="false"
            ></asp:Localize
            ><asp:Localize
              ID="ReportCaptionDateRange"
              runat="server"
              Text=": {0:d} to {1:d}"
              Visible="false"
              EnableViewState="false"
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
            >&nbsp;
            <asp:Label
              ID="TaxNameLabel"
              runat="server"
              Text="Tax Name: "
              SkinID="FieldHeader"
            ></asp:Label>
            <asp:HyperLink ID="TaxNameLink" runat="server"></asp:HyperLink>
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
              DefaultSortDirection="Ascending"
              DefaultSortExpression="OrderId"
              SkinID="Summary"
              DataSourceId="TaxesDs"
            >
              <Columns>
                <asp:TemplateField
                  HeaderText="Order"
                  SortExpression="OrderNumber"
                >
                  <headerstyle horizontalalign="Center" />
                  <itemstyle horizontalalign="Center" />
                  <itemtemplate>
                    <asp:HyperLink
                      ID="OrderLink"
                      runat="server"
                      Text='<%# Eval("OrderNumber") %>'
                      NavigateUrl="<%# GetOrderLink(Container.DataItem) %>"
                    ></asp:HyperLink>
                  </itemtemplate>
                </asp:TemplateField>
                <asp:TemplateField
                  HeaderText="Order Date"
                  SortExpression="OrderDate"
                >
                  <headerstyle horizontalalign="Left" />
                  <itemtemplate> <%# Eval("OrderDate") %> </itemtemplate>
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
      <asp:HiddenField ID="HiddenTaxName" runat="server" Value="" />
      <asp:HiddenField ID="HiddenStartDate" runat="server" Value="" />
      <asp:HiddenField ID="HiddenEndDate" runat="server" Value="" />
      <asp:ObjectDataSource
        ID="TaxesDs"
        runat="server"
        OldValuesParameterFormatString="original_{0}"
        SelectMethod="LoadDetail"
        SortParameterName="sortExpression"
        EnablePaging="true"
        TypeName="MakerShop.Reporting.TaxReportDataSource"
        SelectCountMethod="CountDetail"
        DataObjectTypeName="MakerShop.Reporting.TaxReportDetailItem"
        EnableViewState="false"
      >
        <SelectParameters>
          <asp:ControlParameter
            ControlID="HiddenTaxName"
            Name="taxName"
            PropertyName="Value"
            Type="String"
          />
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
