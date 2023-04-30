<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"
CodeFile="CouponUsage.aspx.cs" Inherits="Admin_Reports_CouponUsage" Title="Sales
by Coupon" %> <%@ Register TagPrefix="ComponentArt"
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
              Text="Sales by Coupon"
            ></asp:Localize>
            <asp:Localize
              ID="ReportFromDate"
              runat="server"
              Text=" from {0:d}"
              EnableViewState="false"
              Visible="false"
            ></asp:Localize>
            <asp:Localize
              ID="ReportToDate"
              runat="server"
              Text=" to {0:d}"
              EnableViewState="false"
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
              <asp:ListItem Value="TODAY">Today</asp:ListItem>
              <asp:ListItem Value="THISWEEK">This Week</asp:ListItem>
              <asp:ListItem Value="LASTWEEK">Last Week</asp:ListItem>
              <asp:ListItem Value="THISMONTH">This Month</asp:ListItem>
              <asp:ListItem Value="LASTMONTH">Last Month</asp:ListItem>
              <asp:ListItem Value="LAST15">Last 15 Days</asp:ListItem>
              <asp:ListItem Value="LAST30">Last 30 Days</asp:ListItem>
              <asp:ListItem Value="LAST60">Last 60 Days</asp:ListItem>
              <asp:ListItem Value="LAST90">Last 90 Days</asp:ListItem>
              <asp:ListItem Value="LAST120">Last 120 Days</asp:ListItem>
              <asp:ListItem Value="THISYEAR">This Year</asp:ListItem>
              <asp:ListItem Value="ALL">All Dates</asp:ListItem>
            </asp:DropDownList>
          </td>
        </tr>

        <tr>
          <td class="dataSheet">
            <cb:SortedGridView
              ID="CouponSalesGrid"
              runat="server"
              AutoGenerateColumns="False"
              DataSourceID="CouponUsageDs"
              DefaultSortExpression="OrderTotal"
              DefaultSortDirection="Descending"
              AllowPaging="True"
              AllowSorting="true"
              PageSize="40"
              OnSorting="CouponSalesGrid_Sorting"
              Width="100%"
              SkinID="Summary"
            >
              <Columns>
                <asp:TemplateField
                  HeaderText="Coupon"
                  SortExpression="CouponCode"
                >
                  <ItemTemplate>
                    <asp:HyperLink
                      ID="CouponLink"
                      runat="server"
                      Text='<%# Eval("CouponCode") %>'
                      NavigateUrl='<%#Eval("Coupon.CouponId", "../Marketing/Coupons/EditCoupon.aspx?CouponId={0}")%>'
                    ></asp:HyperLink>
                  </ItemTemplate>
                  <HeaderStyle HorizontalAlign="Left" />
                </asp:TemplateField>
                <asp:TemplateField
                  HeaderText="Order Count"
                  SortExpression="OrderCount"
                >
                  <ItemStyle HorizontalAlign="Center" />
                  <ItemTemplate>
                    <asp:Label
                      ID="OrderCount"
                      runat="server"
                      Text='<%# Eval("OrderCount") %>'
                    ></asp:Label>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField
                  HeaderText="Order Total"
                  SortExpression="OrderTotal"
                >
                  <ItemStyle HorizontalAlign="Center" />
                  <ItemTemplate>
                    <asp:Label
                      ID="OrderTotal"
                      runat="server"
                      Text='<%# Eval("OrderTotal", "{0:c}") %>'
                    ></asp:Label>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Usage Details">
                  <ItemStyle HorizontalAlign="Center" />
                  <ItemTemplate>
                    <asp:HyperLink
                      ID="UsageDetailLink"
                      runat="server"
                      Text="details"
                      NavigateUrl='<%# string.Format("CouponUsageDetail.aspx?DateFilter={0}&CouponCode={1}", DateFilter.SelectedValue, Server.UrlEncode(Eval("CouponCode").ToString())) %>'
                    />
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
    ID="CouponUsageDs"
    runat="server"
    OldValuesParameterFormatString="original_{0}"
    SelectMethod="GetSalesByCoupon"
    TypeName="MakerShop.Reporting.ReportDataSource"
    SortParameterName="sortExpression"
    EnablePaging="true"
    SelectCountMethod="GetSalesByCouponCount"
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
      <asp:Parameter Name="couponCode" Type="String" DefaultValue="" />
    </SelectParameters>
  </asp:ObjectDataSource>
</asp:Content>
