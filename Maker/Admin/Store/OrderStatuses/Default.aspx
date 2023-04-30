<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"
CodeFile="Default.aspx.cs" Inherits="Admin_Store_OrderStatuses_Default"
Title="Manage Order Statuses" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
  <div class="pageHeader">
    <div class="caption">
      <h1>
        <asp:Localize
          ID="Caption"
          runat="server"
          Text="Configure Order Statuses"
        ></asp:Localize>
      </h1>
    </div>
  </div>
  <table cellpadding="2" cellspacing="0" class="innerLayout">
    <tr>
      <td align="left" valign="top">
        <p>
          <asp:Label
            ID="InstructionText"
            runat="server"
            Text="You can configure the statuses that can be assigned to orders in your store.  Statuses can be assigned to orders automatically through the use of event rules, or they can be assigned manually."
          ></asp:Label>
        </p>
      </td>
    </tr>
    <tr>
      <td align="center" valign="top">
        <asp:GridView
          runat="server"
          ID="StatusGrid"
          AutoGenerateColumns="False"
          AllowSorting="False"
          SkinID="PagedList"
          DataKeyNames="OrderStatusId"
          Width="600px"
          DataSourceID="OrderStatusDs"
        >
          <Columns>
            <asp:BoundField
              HeaderText="Name"
              DataField="Name"
              SortExpression="Name"
              HeaderStyle-HorizontalAlign="Left"
            />
            <asp:TemplateField
              HeaderText="Report"
              ItemStyle-HorizontalAlign="Center"
            >
              <ItemTemplate> <%#GetX(Eval("IsActive"))%> </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField
              HeaderText="Cancelled"
              ItemStyle-HorizontalAlign="Center"
            >
              <ItemTemplate> <%#GetX(!(bool)Eval("IsValid"))%> </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField
              HeaderText="Stock"
              ItemStyle-HorizontalAlign="Center"
            >
              <ItemTemplate> <%#Eval("InventoryAction")%> </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField
              HeaderText="Triggers"
              ItemStyle-HorizontalAlign="Center"
            >
              <ItemTemplate>
                <%#GetTriggers(Container.DataItem)%>
              </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
              <ItemStyle HorizontalAlign="center" Width="60px" />
              <ItemTemplate>
                <asp:HyperLink
                  ID="EditLink"
                  runat="server"
                  NavigateUrl='<%#Eval("OrderStatusId", "EditOrderStatus.aspx?OrderStatusId={0}")%>'
                  ><asp:Image ID="EditIcon" runat="server" SkinID="EditIcon"
                /></asp:HyperLink>
                <asp:ImageButton
                  ID="DeleteButton"
                  runat="server"
                  SkinID="DeleteIcon"
                  CommandName="Delete"
                  OnClientClick='<%#Eval("Name", "return confirm(\"Are you sure you want to delete {0}?\")") %>'
                  Visible="<%# !HasOrders(Container.DataItem) %>"
                />
                <asp:HyperLink
                  ID="DeleteLink"
                  runat="server"
                  NavigateUrl='<%# Eval("OrderStatusId", "DeleteOrderStatus.aspx?OrderStatusId={0}")%>'
                  Visible="<%# HasOrders(Container.DataItem) %>"
                  ><asp:Image
                    ID="DeleteIcon2"
                    runat="server"
                    SkinID="DeleteIcon"
                    AlternateText="Delete"
                /></asp:HyperLink>
              </ItemTemplate>
            </asp:TemplateField>
          </Columns>
          <EmptyDataTemplate>
            <asp:label
              runat="server"
              id="NoStatusesDefinedLabel"
              EnableViewState="false"
              Text="No order statuses have been defined."
            />
          </EmptyDataTemplate>
        </asp:GridView>
      </td>
    </tr>
    <tr>
      <td align="center" valign="middle">
        <br />
        <asp:HyperLink
          ID="SortStatusesLink"
          SkinId="Button"
          runat="server"
          Text="Sort Statuses"
          NavigateUrl="SortStatuses.aspx"
        ></asp:HyperLink>
        <asp:HyperLink
          ID="AddOrderStatusLink"
          SkinId="Button"
          runat="server"
          Text="Add Order Status"
          NavigateUrl="AddOrderStatus.aspx"
        ></asp:HyperLink>
      </td>
    </tr>
  </table>
  <asp:ObjectDataSource
    ID="OrderStatusDs"
    runat="server"
    OldValuesParameterFormatString="original_{0}"
    SelectMethod="LoadForStore"
    TypeName="MakerShop.Orders.OrderStatusDataSource"
    SelectCountMethod="CountForStore"
    SortParameterName="sortExpression"
    DataObjectTypeName="MakerShop.Orders.OrderStatus"
    DeleteMethod="Delete"
    InsertMethod="Insert"
    UpdateMethod="Update"
  >
  </asp:ObjectDataSource>
</asp:Content>
