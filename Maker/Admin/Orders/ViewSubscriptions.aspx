<%@ Page Language="C#" MasterPageFile="~/Admin/Orders/Order.master"
CodeFile="ViewSubscriptions.aspx.cs" Inherits="Admin_Orders_ViewSubscriptions"
Title="View Subscriptions" %> <%@ Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %> <%@ Register
Src="~/Admin/UserControls/PickerAndCalendar.ascx" TagName="PickerAndCalendar2"
TagPrefix="uc1" %>
<asp:Content ID="Content" ContentPlaceHolderID="MainContent" Runat="Server">
  <div class="pageHeader">
    <div class="caption">
      <h1>
        <asp:Localize
          ID="Caption"
          runat="server"
          Text="Subscriptions for Order #{0}"
        ></asp:Localize>
        <asp:Localize
          ID="SubscriptionStatus"
          runat="server"
          Text=" -- Status {0}"
        />
      </h1>
    </div>
  </div>
  <div style="margin: 4px 0px">
    <cb:SortedGridView
      ID="SubscriptionGrid"
      runat="server"
      AutoGenerateColumns="False"
      DataKeyNames="OrderId,PaymentTypeId"
      DataSourceID="SubscriptionDs"
      SkinID="PagedList"
      AllowSorting="False"
      ShowWhenEmpty="False"
      AllowPaging="false"
      EnableViewState="False"
      OnRowCommand="SubscriptionGrid_RowCommand"
      Width="100%"
      OnRowUpdating="SubscriptionGrid_RowUpdating"
    >
      <Columns>
        <asp:TemplateField HeaderText="Payment Type">
          <HeaderStyle HorizontalAlign="Left" />
          <ItemStyle HorizontalAlign="Left" />
          <ItemTemplate>
            <asp:Label
              ID="PaymentType"
              runat="server"
              text='<%#GetPaymentTypeName(Eval("PaymentTypeId"))%>'
            ></asp:Label>
          </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Payment Amount">
          <ItemStyle HorizontalAlign="Right" />
          <ItemTemplate>
            <asp:Label
              ID="PaymentAmount"
              runat="server"
              text='<%#Eval("PaymentAmount","{0:0.00}")%>'
            />
          </ItemTemplate>
          <EditItemTemplate>
            <asp:DropDownList
              runat="server"
              ID="DownSell"
              DataSource='<%#GetDownSell(Eval("PaymentTypeId")) %>'
              DataTextField="ChargeAmount"
              DataValueField="ChargeAmount"
            />
          </EditItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Ship Rate">
          <ItemStyle HorizontalAlign="Right" />
          <ItemTemplate>
            <asp:Label
              ID="ShipRate"
              runat="server"
              text='<%#GetShipRate(Eval("ShipMethodId"))%>'
            />
          </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Payment Days">
          <ItemStyle HorizontalAlign="Center" />
          <ItemTemplate>
            <asp:Label
              ID="PaymentDays"
              runat="server"
              text='<%#Eval("PaymentDays")%>'
            />
          </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Capture">
          <ItemStyle HorizontalAlign="Center" />
          <ItemTemplate>
            <asp:Label
              ID="DaysToCapture"
              runat="server"
              text='<%#Eval("DaysToCapture")%>'
            />
          </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Number of Payments">
          <ItemStyle HorizontalAlign="Center" />
          <ItemTemplate>
            <asp:Label
              ID="NumofPayments"
              runat="server"
              text='<%#Eval("NumberofPayments")%>'
            />
          </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField>
          <ItemStyle HorizontalAlign="right" Width="120px" />
          <ItemTemplate>
            <asp:LinkButton ID="EditButton" runat="server" CommandName="Edit"
              ><asp:Image ID="EditIcon" runat="server" SkinID="EditIcon"
            /></asp:LinkButton>
          </ItemTemplate>
          <EditItemTemplate>
            <asp:ImageButton
              ID="EditSaveButton"
              runat="server"
              CommandName="Update"
              SkinID="SaveIcon"
              ToolTip="Save"
            ></asp:ImageButton>
            <asp:ImageButton
              ID="EditCancelButton"
              runat="server"
              CausesValidation="False"
              CommandName="Cancel"
              SkinID="CancelIcon"
              ToolTip="Cancel Editing"
            ></asp:ImageButton>
          </EditItemTemplate>
        </asp:TemplateField>
      </Columns>
      <EmptyDataTemplate>
        <asp:Label
          ID="EmptyMessage"
          runat="server"
          Text="There are no subscriptions associated with this order."
        ></asp:Label>
      </EmptyDataTemplate>
    </cb:SortedGridView>
  </div>
  <asp:ObjectDataSource
    ID="SubscriptionDs"
    runat="server"
    OldValuesParameterFormatString="original_{0}"
    SelectMethod="LoadByOrderId"
    TypeName="MakerShop.Orders.OrderSubscriptionPlanDetailsDataSource"
    DataObjectTypeName="MakerShop.Orders.OrderSubscriptionPlanDetails"
  >
    <SelectParameters>
      <asp:QueryStringParameter Name="OrderId" QueryStringField="OrderId" />
    </SelectParameters>
  </asp:ObjectDataSource>
</asp:Content>
