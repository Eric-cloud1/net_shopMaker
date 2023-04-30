<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Admin_Orders_Default" Title="Order Manager" %>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<%@ Register assembly="wwhoverpanel" Namespace="Westwind.Web.Controls" TagPrefix="wwh" %>
<%@ Register Src="../UserControls/PickerAndCalendar.ascx" TagName="PickerAndCalendar" TagPrefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<script type="text/javascript">
    function ShowHoverPanel(event,Id)
    { 
        OrderHoverLookupPanel.startCallback(event,"OrderId=" + Id.toString(),null,OnError);    
    }
    function HideHoverPanel()
    {
        OrderHoverLookupPanel.hide();
    }
    function OnError(Result)
    {
        alert("*** Error:\r\n\r\n" + Result.message);    
    }

    function toggleCheckBoxState(id, checkState)
    {
        var cb = document.getElementById(id);
        if (cb != null)
            cb.checked = checkState;
    }

    function toggleSelected(checkState)
    {
        // Toggles through all of the checkboxes defined in the CheckBoxIDs array
        // and updates their value to the checkState input parameter
        if (CheckBoxIDs != null)
        {
            for (var i = 0; i < CheckBoxIDs.length; i++)
                toggleCheckBoxState(CheckBoxIDs[i], checkState.checked);
        }
    }
</script>
<div class="pageHeader">
    <div class="caption">
        <h1><asp:Localize ID="Caption" runat="server" Text="Order Manager"></asp:Localize></h1>
    </div>
</div>
<ajax:UpdatePanel ID="SearchFormAjax" runat="server">
    <ContentTemplate>
  
        <div class="searchPanel">
        <table cellpadding="4" cellspacing="0" class="inputForm">
            <tr>
                <td colspan="6" valign="top">
                    <asp:Panel ID="HeaderPanel" runat="server" DefaultButton="JumpToOrderButton">
                        <table>
                            <tr>
                                <td>
                                    <asp:Localize ID="SearchHelpText" runat="server" Text="Set the criteria below, then click <b>Search</b> to filter the order listing.  You can also "></asp:Localize>
                                    <asp:Label ID="JumpToOrderLabel" runat="server" Text="jump to order #:" AssociatedControlID="JumpToOrderNumber" SkinID="FieldHeader"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="JumpToOrderNumber" runat="server" Width="40px" ValidationGroup="QuickOrderView"></asp:TextBox>
                                </td>
                                <td style="width:180px">
                                    <asp:Button ID="JumpToOrderButton" runat="server" ValidationGroup="JumpToOrder" Text="Go" OnClick="JumpToOrderButton_Click" />
                                    <asp:PlaceHolder ID="phJumpToOrder" runat="server"></asp:PlaceHolder>
                                    <asp:RequiredFieldValidator ID="JumpToOrderNumberRequired" runat="server"
                                        ControlToValidate="JumpToOrderNumber" Text="enter order number" 
                                        ErrorMessage="" ValidationGroup="JumpToOrder" Display="dynamic"></asp:RequiredFieldValidator><asp:RegularExpressionValidator ID="JumpToOrderNumberValidator" runat="server"
                                        ControlToValidate="JumpToOrderNumber" ValidationExpression="^\d+$" Text="enter numbers only" 
                                        ErrorMessage="" ValidationGroup="JumpToOrder" Display="dynamic"></asp:RegularExpressionValidator>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </td>
            </tr>
            </table>
            
            <asp:panel ID="pnSearch" DefaultButton="SearchButton" BorderWidth="0"  runat="server">
            <table cellpadding="4" cellspacing="0" class="inputForm">
            <tr>
                <th class="rowHeader">
                    <cb:ToolTipLabel ID="DateFilterLabel" runat="server" Text="Date Range:" ToolTip="Filter orders that were placed within the start and end dates." />
                </th>
                <td colspan="3" nowrap>
                    <uc1:PickerAndCalendar ID="OrderStartDate" runat="server" />
                    to <uc1:PickerAndCalendar ID="OrderEndDate" runat="server" />
                    &nbsp;&nbsp;
                    <asp:DropDownList ID="DateQuickPick" runat="server" onchange="alert(this.selectedIndex);">
                        <asp:ListItem Value="-- Date Quick Pick --"></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <th class="rowHeader">
                    <cb:ToolTipLabel ID="StatusFilterLabel" runat="server" Text="Order Status:" AssociatedControlID="StatusFilter" ToolTip="Filter by order status." />
                </th>
                <td>
                    <asp:DropDownList ID="StatusFilter" runat="server">
                        <asp:ListItem Text="All" Value="-1"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <th class="rowHeader">
                    <cb:ToolTipLabel ID="OrderNumberFilterLabel" runat="server" Text="Order Number(s):" AssociatedControlID="OrderNumberFilter"
                        ToolTip="You can enter order number(s) to filter the list.  Separate multiple orders with a comma.  You can also enter ranges like 4-10 for all orders numbered 4 through 10." />
                </th>
                <td colspan="1">
                    <asp:TextBox ID="OrderNumberFilter" runat="server" Text="" Width="200px"></asp:TextBox>
                    <cb:IdRangeValidator ID="OrderNumberFilterValidator" runat="server" Required="false"
                        ControlToValidate="OrderNumberFilter" Text="*" 
                        ErrorMessage="The range is invalid.  Enter a specific order number or a range of numbers like 4-10.  You can also include mulitple values separated by a comma."></cb:IdRangeValidator>                
                </td>
                <th class="rowHeader">
                    <cb:ToolTipLabel ID="PaymentStatusFilterLabel" runat="server" Text="Payment Status:" AssociatedControlID="PaymentStatusFilter" ToolTip="Filter orders by payment status." />
                </th>
                <td>
                    <asp:DropDownList ID="PaymentStatusFilter" runat="server">
                        <asp:ListItem Text="All" Value="0"></asp:ListItem>
                        <asp:ListItem Text="Unpaid" Value="1"></asp:ListItem>
                        <asp:ListItem Text="Paid" Value="2"></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <th class="rowHeader">
                    <cb:ToolTipLabel ID="ShipmentStatusFilterLabel" runat="server" Text="Shipment Status:" AssociatedControlID="ShipmentStatusFilter" ToolTip="Filter orders by shipment status." />
                </th>
                <td>
                    <asp:DropDownList ID="ShipmentStatusFilter" runat="server">
                        <asp:ListItem Text="All" Value="0"></asp:ListItem>
                        <asp:ListItem Text="Unshipped" Value="1"></asp:ListItem>
                        <asp:ListItem Text="Shipped" Value="2"></asp:ListItem>
                        <asp:ListItem Text="Non-Shippable" Value="3"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <th class="rowHeader" valign="top"> 
                    <cb:ToolTipLabel ID="KeywordSearchTextLabel" runat="server" Text="Find Keyword:" AssociatedControlID="advancedSearch" ToolTip="Enter a keyword to find.  Use of wildcards * and ? is allowed."></cb:ToolTipLabel>
               
                </th>
                <td colspan="3" valign="top">
                <asp:PlaceHolder ID="advancedSearch" runat="server"/>
                </td>
                <th class="rowHeader" valign="top" >
                    <cb:ToolTipLabel ID="PageSizeLabel" runat="server" Text="Page Size:" AssociatedControlID="PageSize" ToolTip="Indicates the number of records to display per page." />
                </th>
                <td valign="top">
                    <asp:DropDownList ID="PageSize" runat="server">
                        <asp:ListItem Text="10 per page" Value="10"></asp:ListItem>
                        <asp:ListItem Text="20 per page" Value="20" Selected="true"></asp:ListItem>
                        <asp:ListItem Text="50 per page" Value="50"></asp:ListItem>
                        <asp:ListItem Text="show all" Value=""></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
               <tr>
                   <th class="rowHeader">
                    <cb:ToolTipLabel ID="ToolTipLabel1" runat="server" Text="Amount between:" AssociatedControlID="AmountMiniPriceFilter"
                        ToolTip="You can enter an Amount to filter the list." />
                </th>
                <td >
                       <asp:TextBox ID="AmountMiniPriceFilter" runat="server" Width="100px" MaxLength="10"></asp:TextBox> &nbsp;&nbsp;&nbsp;  
                        <asp:Label ID="Label1" runat="server" Text=" and "></asp:Label>
                </td>
                      <td >
                       <asp:TextBox ID="AmountMaxPriceFilter" runat="server" Width="100px" MaxLength="10"></asp:TextBox> 
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td colspan="5">
                    <asp:Button ID="SearchButton" runat="server" Text="Search" OnClick="SearchButton_Click" CausesValidation="false"/> 
                    <asp:Button ID="CreateOrderButton" runat="server" Text="Create Order" Visible="false" OnClick="CreateOrderButton_Click" />
                </td>
            </tr>
        </table>
        </div>
        </asp:panel>
    </ContentTemplate>
</ajax:UpdatePanel>
<ajax:UpdatePanel ID="SearchResultAjax" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <cb:SortedGridView ID="OrderGrid" runat="server" SkinID="PagedList" Width="100%" AllowPaging="True" PageSize="20" 
            AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="OrderId" DataSourceID="OrderDs"
            OnRowCommand="OrderGrid_RowCommand"  OnSorting="OrderGrid_Sorting" DefaultSortExpression="OrderId" DefaultSortDirection="Descending" 
            OnRowDataBound="OrderGrid_RowDataBound" OnDataBound="OrderGrid_DataBound" ShowWhenEmpty="False" OnRowCreated="OrderGrid_RowCreated">
            <Columns>
                <asp:TemplateField HeaderText="Select">
                    <HeaderTemplate>
                        <input type="checkbox" onclick="toggleSelected(this)" />
                    </HeaderTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="20px" />
                   
                    <ItemTemplate>
                        <asp:CheckBox ID="Selected" runat="server" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Order #" SortExpression="OrderNumber">
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:HyperLink ID="OrderNumber" runat="server" Text='<%# Eval("OrderNumber") %>' SkinID="Link" NavigateUrl='<%#String.Format("ViewOrder.aspx?OrderNumber={0}&OrderId={1}", Eval("OrderNumber"), Eval("OrderId"))%>' OnMouseOver='<%# Eval("OrderId", "ShowHoverPanel(event, \"{0}\");")%>' OnMouseOut="HideHoverPanel();"></asp:HyperLink>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Status" SortExpression="OrderStatusId">
                    <ItemStyle HorizontalAlign="Center" Height="30px" />
                    <ItemTemplate>
                        <asp:Label ID="OrderStatus" runat="server" Text='<%# GetOrderStatus(Eval("OrderStatusId")) %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Customer" SortExpression="FullName" >
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:Label ID="CustomerName" runat="server" Text='<%# string.Format("{1}, {0}", Eval("BillToFirstName"), Eval("BillToLastName")) %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Amount" SortExpression="TotalCharges">
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:Label ID="Label5" runat="server" Text='<%# Eval("TotalCharges", "{0:lc}") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Date" SortExpression="OrderDate">
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:Label ID="Label6" runat="server" Text='<%# Eval("OrderDate", "{0:MM-dd-yyyy HH:mm:ss}") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Payment">
                    <ItemStyle HorizontalAlign="Left" />
          
                    <ItemTemplate>
                        <asp:PlaceHolder ID="phPaymentStatus" runat="server"></asp:PlaceHolder>
                        <asp:Label ID="PaymentStatus" runat="server" Text='<%# GetPaymentStatus(Container.DataItem) %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Shipment" Visible=false>
                    <ItemStyle HorizontalAlign="Left" />
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:PlaceHolder ID="phShipmentStatus" runat="server"></asp:PlaceHolder>
                        <asp:Label ID="ShipmentStatus" runat="server" Text='<%# Eval("ShipmentStatus") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                  <asp:TemplateField HeaderText="Promo <br/> Code">
                    <ItemStyle HorizontalAlign="Left" />

                    <ItemTemplate>
                        <asp:PlaceHolder ID="phPromoCode" runat="server"></asp:PlaceHolder>
                        <asp:Label ID="PromoCode" runat="server" Text='<%# Eval("AffiliateId") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                  <asp:TemplateField HeaderText="Sub Id">
                    <ItemStyle HorizontalAlign="Left" />

                    <ItemTemplate>
                        <asp:PlaceHolder ID="phSubId" runat="server"></asp:PlaceHolder>
                        <asp:Label ID="SubId" runat="server" Text='<%# Eval("SubAffiliate") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                 <asp:TemplateField HeaderText="Response<br/>Message" >
                    <ItemStyle HorizontalAlign="Left" />
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemTemplate>
    
                         <asp:Label ID="ResponseMessage" runat="server" Text='<%# GetResponseMessage(Container.DataItem) %>'></asp:Label>
                                                           
                    </ItemTemplate>
                </asp:TemplateField>
                  <asp:TemplateField HeaderText="Processor<br/>Caption">
                    <ItemStyle HorizontalAlign="Left" />

                    <ItemTemplate>
                        <asp:PlaceHolder ID="phProcessorCaption" runat="server"></asp:PlaceHolder>
                        <asp:Label ID="ProcessorCaption" runat="server" Text='<%#  GetChargeInfo(Container.DataItem) %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
         
            </Columns>
            <EmptyDataTemplate>
                <asp:Label ID="EmptyMessage" runat="server" Text="No orders match criteria."></asp:Label>
            </EmptyDataTemplate>
        </cb:SortedGridView>
        <table class="inputForm" cellpadding="3" cellspacing="0" ID="selectedOrdersPanel" runat="server">
            <tr>
                <th class="rowHeader">
                    <asp:Label ID="BatchLabel" runat="server" Text="Update Selected Orders:" SkinID="FieldHeader" AssociatedControlID="BatchAction"></asp:Label>
                </th>
                <td>
                    <asp:DropDownList ID="BatchAction" runat="server">
                        <asp:ListItem Text=""></asp:ListItem>
                        <asp:ListItem Text="Process Payment" Value="PAY"></asp:ListItem>
                        <asp:ListItem Text="Mark Shipped" Value="SHIP"></asp:ListItem>
                        <asp:ListItem Text="Mark Shipped with Options" Value="SHIPOPT"></asp:ListItem>
                        <asp:ListItem Text="Cancel" Value="CANCEL"></asp:ListItem>
                        <asp:ListItem Text="-----------"></asp:ListItem>
                        <asp:ListItem Text="Print Invoices" Value="INVOICE"></asp:ListItem>
                        <asp:ListItem Text="Print Packing Slips" Value="PACKSLIP"></asp:ListItem>
                        <asp:ListItem Text="Print Pull Sheet" Value="PULLSHEET"></asp:ListItem>
                        <asp:ListItem Text="-----------"></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:Button ID="BatchButton" runat="server" Text="GO" OnClick="BatchButton_Click" />
                </td>
            </tr>
            <tr id="trBatchMessage" runat="server" visible="false" enableviewstate="false">
                <td colspan="3">
                    <asp:Label ID="BatchMessage" runat="server" SkinID="GoodCondition"></asp:Label>
                </td>
            </tr>
        </table>
    </ContentTemplate>
</ajax:UpdatePanel>
<asp:HiddenField ID="HiddenStartDate" runat="server" />
<asp:HiddenField ID="HiddenEndDate" runat="server" />
<asp:ObjectDataSource ID="OrderDs"  runat="server"></asp:ObjectDataSource>
<wwh:wwHoverPanel ID="OrderHoverLookupPanel"
    runat="server" 
    serverurl="~/Admin/Orders/OrderSummary.ashx"
    Navigatedelay="1000"              
    scriptlocation="WebResource"
    style="display: none; background: white;" 
    panelopacity="0.89" 
    shadowoffset="8"
    shadowopacity="0.18"
    PostBackMode="None"
    AdjustWindowPosition="true">
</wwh:wwHoverPanel>
</asp:Content>