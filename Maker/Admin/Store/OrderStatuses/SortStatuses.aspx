<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="SortStatuses.aspx.cs" Inherits="Admin_Store_OrderStatuses_SortStatuses" Title="Sort Order Statuses"  %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
<script>
function OrderStatusList_ItemExternalDrop(sender, eventArgs)
{
  var draggedItem = eventArgs.get_item();
  var targetItem = eventArgs.get_target();
  var targetGrid = eventArgs.get_targetControl();
  // We can now use this information to move the draggedItem to the place of the targetItem. You can do so on the server or the client. Here's how to do so on the client (note that this uses a couple of undocumented properties):
  // Get the GridTable and the index we want to move the item to
  var table = targetItem.get_table();
  var index = targetItem.get_index();
  // We operate on the raw data array, first removing the item from its original position, then inserting it into the desired one
  table.Data.splice(draggedItem.get_index(), 1);
  table.Data = table.Data.slice(0, index).concat([draggedItem.Data]).concat(table.Data.slice(index));
  // Re-draw the grid
  targetGrid.render();  
  //UPDATE THE GRID
  <%=ReorderCallback.ClientID %>.callback(draggedItem.get_index() + ":" + targetItem.get_index());
}
</script>
<div class="pageHeader">
	<div class="caption">
		<h1><asp:Localize ID="Caption" runat="server" Text="Sort Order Statuses"></asp:Localize></h1>
	</div>
</div>
<table cellpadding="2" cellspacing="0" class="innerLayout">
    <tr>
        <td align="center">
            <p><asp:Label ID="InstructionText" runat="server" Text="Drag the order statuses to put them correct order and click Finish to save.  The sort order only impacts the merchant administration, it has no bearing on what will be displayed to customers."></asp:Label></p>
            <%--
            <ajaxToolkit:ReorderList ID="OrderStatusList" runat="server"
                PostBackOnReorder="false"
                OnItemReordering="OrderStatusList_ItemReordering"
                DataKeyField="OrderStatusId"
                CallbackCssStyle="callbackStyle"
                DragHandleAlignment="Left"
                CssClass="reorderList"
                AllowReorder="true" Width="100%">
                <ItemTemplate>
                    <div class="itemArea">
                        <asp:Label ID="Name" runat="server" Text='<%# Eval("Name") %>' />
                    </div>
                </ItemTemplate>
                <ReorderTemplate>
                    <asp:Panel ID="Panel2" runat="server" CssClass="reorderCue" />
                </ReorderTemplate>
                <DragHandleTemplate>
                    <div class="dragHandle">move</div>
                </DragHandleTemplate>
            </ajaxToolkit:ReorderList>
            --%>
            <ComponentArt:Grid ID="OrderStatusList" runat="server" ItemDraggingEnabled="true" Width="300px"
                ShowHeader="false" ShowFooter="false" ExternalDropTargets="OrderStatusList">
                <Levels>
                    <ComponentArt:GridLevel HeadingCellCssClass="HeadingCell" 
                        HeadingRowCssClass="HeadingRow" 
                        HeadingTextCssClass="HeadingCellText"
                        DataCellCssClass="DataCell" 
                        RowCssClass="Row" 
                        SelectedRowCssClass="Row"
                        GroupHeadingCssClass="GroupHeading" 
                        SortAscendingImageUrl="asc.gif" 
                        SortDescendingImageUrl="desc.gif" 
                        SortImageWidth="10"
                        SortImageHeight="10">
                        <Columns>
                            <ComponentArt:GridColumn DataField="Name" />
                        </Columns>
                    </ComponentArt:GridLevel>
                </Levels>
                <ClientEvents>
                    <ItemExternalDrop EventHandler="OrderStatusList_ItemExternalDrop" />
                </ClientEvents>                
            </ComponentArt:Grid>
            <ComponentArt:CallBack ID="ReorderCallback" runat="server" OnCallback="ReorderCallback_Callback" />
            <br />
            <asp:Button ID="FinishButton" runat="server" Text="Finish" OnClick="FinishButton_Click" />
        </td>
    </tr>
</table>    
</asp:Content>

