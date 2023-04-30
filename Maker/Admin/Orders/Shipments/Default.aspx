<%@ Page Language="C#" MasterPageFile="~/Admin/Orders/Order.master" CodeFile="Default.aspx.cs" Inherits="Admin_Orders_Shipments__Default" Title="Edit Shipment" EnableViewState="false" %>
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<%@ Register Src="../../UserControls/OrderItemDetail.ascx" TagName="OrderItemDetail" TagPrefix="uc" %>
<%@ Register assembly="wwhoverpanel" Namespace="Westwind.Web.Controls" TagPrefix="wwh" %>
<%@ Import Namespace="MakerShop.Shipping.Providers" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">

<style type="text/css">
.layer1 {
margin: 0;
padding: 0;
width: 100%;
}
 
.heading {
margin: 1px;
padding: 3px 3px;
cursor: pointer;
position: relative;
top:10px;
}
.content {
padding: 0px 3px;
background-color:White;

}

</style>
<script type="text/javascript" src="<%= Page.ResolveUrl("~")%>js/jquery.min.js"></script>
    <script type="text/javascript">
    
        jQuery(document).ready(function() {
            jQuery(".content").show();
            //toggle the componenet with class msg_body
            jQuery(".heading").click(function() {
                jQuery(this).next(".content").slideToggle(500);
            });
        });

        

       function ShowHoverPanel(event,Id, panelName) {

           if(panelName == "h1")
               WwHoverOrderNumber.startCallback(event, "orderNumber=" + Id, null, OnError);
          
        }
        function HideHoverPanel(panelName)
        {

            if (panelName == "h1")
                WwHoverOrderNumber.hide();
      
            // *** If you don't use shadows, you can fade out
            //LookupPanel.fadeout();
        }
        function OnCompletion(Result)
        {
            //alert('done it!\r\n' + Result);
        }
        function OnError(Result)
        {
            alert("*** Error:\r\n\r\n" + Result.message);            
        } 
        
      


        function changeIt(img_name) {

            eval("var theImg = document.images." + img_name + ".src;");

            var x = theImg.split("/");
            var t = x.length - 1;
            var y = x[t];
            

            if (y == 'plus.gif') {
                eval("document.images." + img_name +".src = '<%= Page.ResolveUrl("~")%>images/minus.gif';");
            }
            if (y == 'minus.gif') {
                 eval("document.images." + img_name +".src = '<%= Page.ResolveUrl("~")%>images/plus.gif';");
            }
        } 
</script>          
<div class="layer1">  

            <asp:Label ID="SuccessText" runat="server" EnableViewState="false" SkinID="GoodCondition"></asp:Label>
            <asp:Repeater ID="OrderShipmentRepeater"  runat="server" OnItemDataBound="OrderShipmentRepeater_ItemDataBound"  >
             <ItemTemplate>
              <fieldset style="border-color:silver">
        <div class="heading">
         <legend > <h1><img src="<%= Page.ResolveUrl("~")%>images/minus.gif" onclick="changeIt('<%#getImageNumber(Container.DataItem)%>')" name="<%#getImageNumber(Container.DataItem)%>" alt="Group/Collapse Payments" border="0" /> <%#getOrderNumber(Container.DataItem)%></h1></legend>
         </div>
        
         <div class="content">
            <asp:Repeater ID="EditShipmentsGrid" runat="server"  OnItemDataBound="EditShipmentsGrid_ItemDataBound" >
                <ItemTemplate>
                    <div class="pageHeader">
                        <div class="caption">
                            <h1 onmouseover='ShowHoverPanel(event, "<%# GetOrderNumber(Container.DataItem) %>", "h1");' onmouseout='HideHoverPanel("h1");' ><asp:Label ID="Caption" runat="server" Text='<%#string.Format("Shipment #{0}", (Container.ItemIndex + 1))%>'></asp:Label></h1>
                        </div>
                    </div>            
                    <div style="clear: both;">
                    <table width="100%" cellspacing="5" cellpadding="2" class="contentPanel" >
                        <tr>
                            <td rowspan="4" valign="top" width="300px">
                                <table class="inputForm" width="100%" cellspacing="5" >
                                    <tr>
                                        <th align="left" class="rowHeader" valign="top"><asp:Label ID="ShippingAddressLabel" runat="server" Text="Ship To:"></asp:Label></th>
                                        <td align="left"><asp:Label ID="ShippingAddress" runat="server" Text='<%#GetShipToAddress(Container.DataItem)%>'></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <th align="left" class="rowHeader"><asp:Label ID="AddressTypeLabel" runat="server" Text="Address Type:"></asp:Label></th>
                                        <td align="left"><asp:Label ID="AddressType" runat="server" Text='<%#GetAddressType(Container.DataItem)%>'></asp:Label></td>                                
                                    </tr>
                                    <tr>
                                        <th align="left" class="rowHeader"><asp:Label ID="ShippingMethodLabel" runat="server" Text="Method:"></asp:Label></th>
                                        <td align="left">
                                            <asp:Label ID="ShippingMethod" runat="server" Text='<%#Eval("ShipMethodName")%>'></asp:Label>
                                            <asp:LinkButton ID="ChangeShipMethod" runat="server" Text="[change]" CommandName="ChangeShipMethod" CommandArgument='<%#Eval("OrderShipmentId")%>'></asp:LinkButton>
                                        </td>                                
                                    </tr>
                                    <tr>
                                        <th align="left" class="rowHeader"><asp:Label ID="ShipDateLabel" runat="server" Text="Ship Date:"></asp:Label></th>
                                        <td align="left"><asp:Label ID="ShipDate" runat="server" Text='<%#Eval("ShipDate")%>' Visible='<%#!System.DateTime.MinValue.Equals(Eval("ShipDate")) %>'></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <th align="left" class="rowHeader" valign="top"><asp:Label ID="ShipMessageLebel" runat="server" Text="Delivery Instructions:"></asp:Label></th>
                                        <td align="left"><asp:Label ID="ShipMessage" runat="server" Text='<%#Eval("ShipMessage")%>' ></asp:Label></td>
                                    </tr>
                                    <tr id="trTracking" runat="server">
                                        <th align="left" class="rowHeader" valign="top">
                                            <asp:Label ID="TrackingNumbersLabel" runat="server" Text="Tracking:"></asp:Label>
                                        </th>
                                        <td align="left" >
                                            <div style="overflow:auto;width:290px;height:30px; vertical-align:middle;">
                                                <asp:Repeater ID="TrackingRepeater" runat="server" DataSource='<%#Eval("TrackingNumbers")%>'>
                                                    <ItemTemplate>
                                                        <asp:HyperLink ID="TrackingNumberData" runat="server" Target="_blank" Text='<%#Eval("TrackingNumberData")%>' NavigateUrl='<%#GetTrackingUrl(Container.DataItem)%>'></asp:HyperLink>
                                                    </ItemTemplate>
                                                    <SeparatorTemplate>, </SeparatorTemplate>
                                                </asp:Repeater>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td valign="top" style="padding:0px;">
                                <asp:GridView ID="ShipmentItems" runat="server" ShowHeader="true" DataSource='<%#GetDisplayItems(Eval("OrderItems"))%>' AutoGenerateColumns="false" Width="100%" GridLines="Both" CellPadding="2" CellSpacing="1" RowStyle-CssClass="even" SkinID="PagedList">
                                    <Columns>
                                        <asp:TemplateField HeaderText="SKU" SortExpression="Sku">
                                            <ItemTemplate>
                                                <asp:Label ID="Sku" runat="server" Text='<%# GetSku(Container.DataItem) %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Item">
                                            <ItemStyle BorderWidth="0" />
                                            <ItemTemplate>
                                                <uc:OrderItemDetail ID="OrderItemDetail1" runat="server" OrderItem='<%#(OrderItem)Container.DataItem%>' ShowAssets="False" LinkProducts="True" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Quantity" HeaderText="Quantity" ItemStyle-HorizontalAlign="Center" />
                                        <asp:TemplateField HeaderText="Total" SortExpression="ExtendedPrice">
                                            <itemstyle horizontalalign="Right" />
                                            <ItemTemplate>
                                                <asp:Label ID="ExtendedPrice" runat="server" Text='<%# Eval("ExtendedPrice", "{0:lc}") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align:right; font-weight: bold;" class="middle">
                                <asp:Label ID="ItemSubtotalLabel" runat="server" Text="Item Subtotal:"></asp:Label>
                                <asp:Label ID="ItemSubtotal" runat="server" Text='<%#String.Format("{0:lc}", GetItemSubtotal(Container.DataItem))%>'></asp:Label><br />
                            </td>
                        </tr>
                        <tr>
                            <td style="text-align:right;">  
                                <asp:Label ID="ShippingAndHandlingLabel" runat="server" Text="Shipping and Handling:"></asp:Label>
                                <asp:Label ID="ShippingAndHandling" runat="server" Text='<%#String.Format("{0:lc}", GetShippingSubtotal(Container.DataItem))%>'></asp:Label><br />
                                <asp:Label ID="SalesTaxLabel" runat="server" Text="Tax:"></asp:Label>
                                <asp:Label ID="SalesTax" runat="server" Text='<%#String.Format("{0:lc}", GetTaxSubtotal(Container.DataItem))%>'></asp:Label><br />
                            </td>
                       </tr>     
                       <tr>
                            <td style="text-align:right; font-weight: bold;" class="total">
                                <asp:Label ID="TotalForShipmentLabel" runat="server" Text="Shipment Total:"></asp:Label>
                                <asp:Label ID="TotalForShipment" runat="server" Text='<%#String.Format("{0:lc}", GetTotal(Container.DataItem))%>'></asp:Label><br />
                            </td>
                        </tr>
                        <asp:Panel runat="server" ID="pShipmentControls">
                         <tr>
                            <td colspan="2">
                             <asp:Button ID="ReshipOrder" runat="server" CssClass="button" text="Reship" OnClick="ReshipOrder_Click" OnClientClick="Javascript:alert('Please confirm, this order needs to be reshipped?');" />
                               
                                <asp:HyperLink ID="RecordShipmentLink" runat="server" SkinID="Button" Text="Ship Items" NavigateUrl='<%#Eval("OrderShipmentId", "ShipOrder.aspx?OrderShipmentId={0}")%>' Visible='<%#System.DateTime.MinValue.Equals(Eval("ShipDate")) %>'></asp:HyperLink>
                                &nbsp;<asp:HyperLink ID="PackingListLink" runat="server"  SkinID="Button" Text="Packing List" NavigateUrl='<%#Eval("OrderShipmentID", "PackingList.aspx?OrderShipmentID={0}")%>'></asp:HyperLink>
                                &nbsp;<asp:HyperLink ID="EditShipmentLink"  runat="server" SkinID="Button" Text="Edit" NavigateUrl='<%#Eval("OrderShipmentId", "EditShipment.aspx?OrderShipmentId={0}")%>' ></asp:HyperLink>
                                <asp:PlaceHolder ID="phSplit" runat="server" Visible='<%#ShowSplitLink(Container.DataItem)%>'>
                                    &nbsp;<asp:HyperLink ID="SplitShipmentLink" runat="server" SkinID="Button" Text="Split" NavigateUrl='<%#string.Format("SplitShipment.aspx?OrderNumber={0}&OrderId={1}&ShipmentId={2}", OrderNumber, Eval("OrderId"), Eval("OrderShipmentId"))%>'></asp:HyperLink>
                                </asp:PlaceHolder>
                                <asp:HyperLink ID="ReturnShipmentLink" runat="server" SkinID="Button" Text="Return" NavigateUrl='<%#string.Format("ReturnShipment.aspx?OrderNumber={0}&OrderId={1}&OrderShipmentId={2}", OrderNumber, Eval("OrderId"), Eval("OrderShipmentId"))%>' Visible='<%#IsReturnButtonVisible(Container.DataItem)%>' ToolTip="Handle if a shipped item is returned."></asp:HyperLink>
                                <asp:PlaceHolder ID="phMerge" runat="server" Visible='<%#ShowMergeLink(Container.DataItem)%>'>
                                    &nbsp;<asp:HyperLink ID="MergeShipmentLink" runat="server" SkinID="Button" Text="Merge" NavigateUrl='<%#string.Format("MergeShipment.aspx?OrderNumber={0}&OrderId={1}&ShipmentId={2}", OrderNumber, Eval("OrderId"), Eval("OrderShipmentId"))%>'></asp:HyperLink>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="phCancelOrder" runat="server" Visible='<%#ShowCancelOrderLink(Container.DataItem)%>'>
                                    &nbsp;<asp:HyperLink ID="CancelOrderLink" runat="server" SkinID="Button" Text="Returned Order" NavigateUrl='<%#string.Format("ReturnShipment.aspx?OrderNumber={0}&OrderId={1}&ShipmentId={2}", OrderNumber, Eval("OrderId"), Eval("OrderShipmentId"))%>'></asp:HyperLink>
                                </asp:PlaceHolder>
                            
                                 
                                &nbsp;<asp:HyperLink ID="DeleteShipmentLink" runat="server" SkinID="Button" Text="Delete" NavigateUrl='<%#string.Format("DeleteShipment.aspx?OrderNumber={0}&OrderId={1}&ShipmentId={2}", OrderNumber, Eval("OrderId"), Eval("OrderShipmentId"))%>' Visible='<%#ShowDeleteButton(Container.DataItem)%>'></asp:HyperLink>
                                <asp:LinkButton ID="DeleteShipmentButton" runat="server" SkinID="Button" Text="Delete" Visible='<%#ShowDeleteLink(Container.DataItem)%>' OnClientClick="return confirm('Are you sure you want to delete this shipment?')" CommandName="DelShp" CommandArgument='<%#Eval("OrderShipmentId")%>'></asp:LinkButton>
                            </td>
                        </tr> 
                        </asp:Panel>
                    </table>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
          </div>
          </ItemTemplate>
          </asp:Repeater>         
            <hr /><br />
            <asp:HyperLink ID="AddShipmentLink" runat="server" SkinID="Button">Add a new shipment</asp:HyperLink>
            <asp:Panel ID="ChangeShipMethodDialog" runat="server" Style="display:none;width:500px" CssClass="modalPopup">
                <asp:Panel ID="ChangeShipMethodDialogHeader" runat="server" CssClass="modalPopupHeader">
                    <asp:Localize ID="ChangeShipMethodDialogCaption" runat="server" Text="Shipment #{0}: Change Shipping Method"></asp:Localize>
                </asp:Panel>
                <div style="padding-top:5px;">
                    <table class="inputForm" cellpadding="3">
                        <tr>
                            <th class="rowHeader" nowrap>
                                <asp:Literal ID="ExistingShipMethodLabel" runat="server" Text="Current Method:"></asp:Literal>
                            </th>
                            <td>
                                <asp:Literal ID="ExistingShipMethod" runat="server" EnableViewState="false"></asp:Literal>
                            </td>
                        </tr>
                        <tr>
                            <th class="rowHeader" valign="top" nowrap>
                                <asp:Literal ID="NewShipMethodLabel" runat="server" Text="New Method:"></asp:Literal>
                            </th>
                            <td valign="top">
                                <asp:DropDownList ID="NewShipMethod" runat="server">
                                    <asp:ListItem Text="None" Value="0"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:Localize ID="HiddenShipMethodWarning" runat="server" Text="<br />** Unavailable to customer at checkout." Visible="false" EnableViewState="false"></asp:Localize>
                            </td>
                        </tr>
                        <tr>
                            <td></td>
                            <td>
                                <asp:HiddenField ID="ChangeShipMethodShipmentId" runat="server" />
                                 <asp:Button ID="ChangeShipMethodOKButton" runat="server" text="Update" OnClick="ChangeShipMethodOKButton_Click" />
                                <asp:Button ID="ChangeShipMethodCancelButton" runat="server" text="Cancel" />
                               
                            </td>
                        </tr>
                    </table>
                </div>
            </asp:Panel>
            
            
            <asp:HiddenField ID="DummyChangeShipMethod" runat="server" />
            <ajax:ModalPopupExtender ID="ChangeShipMethodPopup" runat="server" 
                TargetControlID="DummyChangeShipMethod"
                PopupControlID="ChangeShipMethodDialog" 
                BackgroundCssClass="modalBackground"                         
                CancelControlID="ChangeShipMethodCancelButton" 
                DropShadow="true"
                PopupDragHandleControlID="ChangeShipMethodDialogHeader" />
   
         <wwh:wwHoverPanel ID="WwHoverOrderNumber"
        runat="server" 
        serverurl="~/Admin/Orders/Shipments/viewDetails.ashx"
        Navigatedelay="500"              
        scriptlocation="WebResource"
        style="display: none; background: white;" 
        panelopacity="0.89" 
        shadowoffset="8"
        shadowopacity="0.18"
        PostBackMode="None"
        HoverOffsetBottom ="3"
        HoverOffsetRight ="10"
        AdjustWindowPosition="true">
    </wwh:wwHoverPanel>
</asp:Content>
