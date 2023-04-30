<%@ Page Language="C#" MasterPageFile="~/Admin/Orders/Order.master"
AutoEventWireup="true" CodeFile="RelatedOrders.aspx.cs"
Inherits="Admin_Orders_RelatedOrders" Title="Related Orders" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb"
%> <%@ Register assembly="wwhoverpanel" Namespace="Westwind.Web.Controls"
TagPrefix="wwh" %> <%@ Register Src="../UserControls/OrderItemDetail.ascx"
TagName="OrderItemDetail" TagPrefix="uc" %> <%@ Register
Src="../UserControls/OrderTotalSummary.ascx" TagName="OrderTotalSummary"
TagPrefix="uc" %> <%@ Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %> <%@ Register
Src="AffiliateDataViewport.ascx" TagName="AffiliateDataViewport" TagPrefix="uc"
%>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
  <script type="text/javascript">
    function ShowHoverPanel(event, Id) {
      OrderHoverLookupPanel.startCallback(
        event,
        "OrderId=" + Id.toString(),
        null,
        OnError
      );
    }
    function HideHoverPanel() {
      OrderHoverLookupPanel.hide();
    }
  </script>

  <div class="pageHeader">
    <div class="caption">
      <h1>
        <asp:Localize
          ID="Caption"
          runat="server"
          Text="Order #{0}"
          EnableViewState="false"
        ></asp:Localize>
      </h1>
    </div>
  </div>
  <div width="100%" style="margin: 4px 0px">
    <table>
      <tr>
        <td colspan="2">
          <asp:GridView
            ID="RelatedOrdersGrid"
            runat="server"
            SkinID="PagedList"
            Width="100%"
            AllowPaging="True"
            PageSize="20"
            DefaultSortExpression="OrderNumber"
            DefaultSortDirection="Ascending"
            OnSorting="RelatedOrders_Sorting"
            AutoGenerateColumns="False"
            DataKeyNames="OrderId"
            onrowdatabound="RelatedOrders_DataBound"
            ShowWhenEmpty="False"
            OnPageIndexChanging="RelatedOrders_PageIndexChanging"
            CellSpacing="0"
            CellPadding="4"
          >
            <Columns>
              <asp:TemplateField
                HeaderText="Order #"
                SortExpression="OrderNumber"
              >
                <ItemStyle HorizontalAlign="Center" />
                <ItemTemplate>
                  <asp:HyperLink
                    ID="OrderNumber"
                    runat="server"
                    Text='<%# Eval("OrderNumber") %>'
                    SkinID="Link"
                    NavigateUrl='<%#String.Format("ViewOrder.aspx?OrderNumber={0}&OrderId={1}", Eval("OrderNumber"), Eval("OrderId"))%>'
                    OnMouseOver='<%# Eval("OrderId", "ShowHoverPanel(event, \"{0}\");")%>'
                    OnMouseOut="HideHoverPanel();"
                  ></asp:HyperLink>
                </ItemTemplate>
              </asp:TemplateField>

              <asp:TemplateField HeaderText="Date" SortExpression="Date">
                <ItemStyle HorizontalAlign="Center" />
                <ItemTemplate>
                  <asp:Label
                    ID="Date"
                    runat="server"
                    Text="<%# FormatDate(Container.DataItem)  %>"
                  ></asp:Label>
                </ItemTemplate>
              </asp:TemplateField>

              <asp:TemplateField
                HeaderText="Order Type"
                SortExpression="OrderType"
              >
                <ItemStyle HorizontalAlign="Center" />
                <ItemTemplate>
                  <asp:Label
                    ID="OrderType"
                    runat="server"
                    Text='<%# Eval("OrderType") %>'
                  ></asp:Label>
                </ItemTemplate>
              </asp:TemplateField>

              <asp:TemplateField HeaderText="Product" SortExpression="Product">
                <ItemStyle HorizontalAlign="Center" />
                <ItemTemplate>
                  <asp:Label
                    ID="Product"
                    runat="server"
                    Text='<%# Eval("Product") %>'
                  ></asp:Label>
                </ItemTemplate>
              </asp:TemplateField>

              <asp:TemplateField
                HeaderText="Quantity"
                SortExpression="Quantity"
              >
                <ItemStyle HorizontalAlign="Center" />
                <ItemTemplate>
                  <asp:Label
                    ID="Quantity"
                    runat="server"
                    Text='<%# Eval("Quantity") %>'
                  ></asp:Label>
                </ItemTemplate>
              </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
              <asp:Label
                ID="EmptyMessage"
                runat="server"
                Text="No orders match criteria."
              ></asp:Label>
            </EmptyDataTemplate>
          </asp:GridView>
        </td>
      </tr>
    </table>
  </div>
  <wwh:wwHoverPanel
    ID="OrderHoverLookupPanel"
    runat="server"
    serverurl="~/Admin/Orders/OrderSummary.ashx"
    Navigatedelay="1000"
    scriptlocation="WebResource"
    style="display: none; background: white"
    panelopacity="0.89"
    shadowoffset="8"
    shadowopacity="0.18"
    PostBackMode="None"
    AdjustWindowPosition="true"
  >
  </wwh:wwHoverPanel>
</asp:Content>
