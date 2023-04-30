<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"
CodeFile="TopProducts.aspx.cs" Inherits="Admin_Reports_TopProducts" Title="Sales
by Product" %> <%@ Register TagPrefix="ComponentArt"
Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb"
%>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
  <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
      <div class="pageHeader">
        <div class="caption">
          <h1>
            <asp:Localize
              ID="Caption"
              runat="server"
              Text="Sales by Product"
            ></asp:Localize
            ><asp:Localize
              ID="ReportCaption"
              runat="server"
              Text=" {0:d} to {1:d}"
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
            ><br />
          </td>
        </tr>
        <tr>
          <td class="dataSheet">
            <cb:SortedGridView
              ID="TopProductGrid"
              runat="server"
              AutoGenerateColumns="False"
              DataSourceID="TopProductDs"
              DefaultSortExpression="TotalQuantity"
              DefaultSortDirection="Descending"
              AllowPaging="True"
              AllowSorting="true"
              SkinID="Summary"
              PageSize="80"
              OnSorting="TopProductGrid_Sorting"
              Width="100%"
            >
              <Columns>
                <asp:TemplateField HeaderText="Name" SortExpression="Name">
                  <ItemTemplate>
                    <asp:HyperLink
                      ID="ProductLink"
                      runat="server"
                      Text='<%# Eval("Name") %>'
                      NavigateUrl='<%#Eval("ProductId", "../Products/EditProduct.aspx?ProductId={0}")%>'
                    ></asp:HyperLink>
                  </ItemTemplate>
                  <HeaderStyle HorizontalAlign="Left" />
                </asp:TemplateField>
                <asp:TemplateField
                  HeaderText="Total Quantity"
                  SortExpression="TotalQuantity"
                >
                  <ItemStyle HorizontalAlign="center" width="100" />
                  <ItemTemplate>
                    <asp:Label
                      ID="Label1"
                      runat="server"
                      Text='<%# Eval("TotalQuantity") %>'
                    ></asp:Label>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField
                  HeaderText="Total Sales"
                  SortExpression="TotalPrice"
                >
                  <ItemStyle HorizontalAlign="right" width="100" />
                  <ItemTemplate>
                    <asp:Label
                      ID="Label2"
                      runat="server"
                      Text='<%# Eval("TotalPrice", "{0:lc}") %>'
                    ></asp:Label>
                  </ItemTemplate>
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
    </ContentTemplate>
  </ajax:UpdatePanel>
  <asp:ObjectDataSource
    ID="TopProductDs"
    runat="server"
    OldValuesParameterFormatString="original_{0}"
    SelectMethod="GetSalesByProduct"
    TypeName="MakerShop.Reporting.ReportDataSource"
    SortParameterName="sortExpression"
    EnablePaging="true"
    SelectCountMethod="GetSalesByProductCount"
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
</asp:Content>
