<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"
CodeFile="DeleteOrderStatus.aspx.cs"
Inherits="Admin_Store_OrderStatuses_DeleteOrderStatus" Title="Delete Order
Status" %> <%@ Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
  <div class="pageHeader">
    <div class="caption">
      <h1>
        <asp:Localize
          ID="Caption"
          runat="server"
          Text="Delete {0}"
          EnableViewState="false"
        ></asp:Localize>
      </h1>
    </div>
  </div>
  <table cellpadding="2" cellspacing="0" class="innerLayout">
    <tr>
      <td valign="top" width="300">
        <ajax:UpdatePanel ID="EditAjax" runat="server" UpdateMode="Conditional">
          <ContentTemplate>
            <asp:Label
              ID="InstructionText"
              runat="server"
              Text="This order status has one or more associated orders.  Indicate what order status these orders should be changed to when {0} is deleted."
              EnableViewState="false"
            ></asp:Label>
            <table class="inputForm" width="100%">
              <tr>
                <th class="rowHeader">
                  <asp:Label
                    ID="NameLabel"
                    runat="server"
                    Text="Move to OrderStatus:"
                    AssociatedControlID="OrderStatusList"
                    ToolTip="New OrderStatus for associated Orders"
                  ></asp:Label
                  ><br />
                </th>
                <td>
                  <asp:DropDownList
                    ID="OrderStatusList"
                    runat="server"
                    DataTextField="Name"
                    DataValueField="OrderStatusId"
                    AppendDataBoundItems="True"
                  >
                    <asp:ListItem Value="" Text="-- none --"></asp:ListItem>
                  </asp:DropDownList>
                  <asp:RequiredFieldValidator
                    ID="OrderStatusRequired"
                    runat="server"
                    ControlToValidate="OrderStatusList"
                    Display="Static"
                    ErrorMessage="You must select an OrderStatus to assign the Orders to."
                    Text="*"
                  ></asp:RequiredFieldValidator>
                </td>
              </tr>
              <tr>
                <td class="submit" colspan="2">
                  <asp:ValidationSummary
                    ID="ValidationSummary1"
                    runat="server"
                  />
                  <asp:Button
                    ID="DeleteButton"
                    runat="server"
                    Text="Delete"
                    OnClick="DeleteButton_Click"
                  />
                  <asp:Button
                    ID="CancelButton"
                    runat="server"
                    Text="Cancel"
                    OnClick="CancelButton_Click"
                    CausesValidation="false"
                  />
                </td>
              </tr>
            </table>
          </ContentTemplate>
        </ajax:UpdatePanel>
      </td>
      <td valign="top" width="300">
        <div class="section">
          <div class="header">
            <h2>
              <asp:Localize
                ID="OrdersCaption"
                runat="server"
                Text="Associated Orders"
              ></asp:Localize>
            </h2>
          </div>
          <div class="content">
            <ajax:UpdatePanel
              ID="PagingAjax"
              runat="server"
              UpdateMode="conditional"
            >
              <ContentTemplate>
                <cb:SortedGridView
                  ID="OrdersGrid"
                  runat="server"
                  DataSourceID="OrdersDs"
                  AllowPaging="True"
                  AllowSorting="True"
                  AutoGenerateColumns="False"
                  DataKeyNames="OrderId"
                  PageSize="20"
                  SkinID="PagedList"
                  DefaultSortExpression="OrderNumber"
                  Width="100%"
                >
                  <Columns>
                    <asp:TemplateField
                      HeaderText="Order"
                      SortExpression="OrderNumber"
                    >
                      <HeaderStyle HorizontalAlign="Left" />
                      <ItemTemplate>
                        <asp:HyperLink
                          ID="OrderNumber"
                          runat="server"
                          Text='<%# Eval("OrderNumber") %>'
                          NavigateUrl='<%#String.Format("~/Admin/Orders/ViewOrder.aspx?OrderNumber={0}&OrderId={1}", Eval("OrderNumber"), Eval("OrderId"))%>'
                        ></asp:HyperLink>
                      </ItemTemplate>
                    </asp:TemplateField>
                  </Columns>
                  <EmptyDataTemplate>
                    <asp:Label
                      ID="EmptyMessage"
                      runat="server"
                      Text="There are no orders associated with this order status."
                    ></asp:Label>
                  </EmptyDataTemplate>
                </cb:SortedGridView>
              </ContentTemplate>
            </ajax:UpdatePanel>
          </div>
        </div>
        <asp:ObjectDataSource
          ID="OrdersDs"
          runat="server"
          EnablePaging="True"
          OldValuesParameterFormatString="original_{0}"
          SelectCountMethod="CountForOrderStatus"
          SelectMethod="LoadForOrderStatus"
          SortParameterName="sortExpression"
          TypeName="MakerShop.Orders.OrderDataSource"
        >
          <SelectParameters>
            <asp:QueryStringParameter
              Name="orderStatusId"
              QueryStringField="OrderStatusId"
              Type="Object"
            />
          </SelectParameters>
        </asp:ObjectDataSource>
      </td>
    </tr>
  </table>
</asp:Content>
