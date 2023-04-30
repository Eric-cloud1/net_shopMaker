<%@ Page Language="C#" MasterPageFile="Order.master" CodeFile="OrderHistory.aspx.cs" Inherits="Admin_Orders_OrderHistory" Title="Order History and Notes"  %>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register src="../UserControls/AddOrderNotes.ascx" tagname="AddOrderNotes" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

<script src="../../js/jquery.js" type="text/javascript"></script>

<style type="text/css" >
 div.sc_menu_wrapper {
  position: relative;   
  height: 150px;
  width: 750px;
  margin-top: 5px;
  overflow: auto;
}
</style>

<script type="text/jscript">
function makeScrollable(wrapper, scrollable){
}
$(function(){   
  makeScrollable("div.sc_menu_wrapper", "div.sc_menu");
});



</script>
    <div class="pageHeader">
        <div class="caption"><h1><asp:Localize ID="Caption" runat="server" Text="Order History and Notes"></asp:Localize></h1></div>
    </div>
    <div  style="margin:4px 0px;width:700px;">
    <table>
      <tr>
            <th class="sectionHeader">
               <asp:ImageButton ID="btnCSV" ToolTip="Email download CSV History data" Visible="false" OnClick="downloadCSV" Width="20px" Height="20px" runat="server" ImageUrl="~/App_Themes/MakerShopAdmin/images/CSV.png" />&nbsp;&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;  Order History and Notes&nbsp;&nbsp; 
            </th>
        </tr>
        <tr><td>
        <uc1:AddOrderNotes ID="AddOrderNotes1" runat="server" /></td><td> 
       
          </td></tr>

      <tr>
            <td style="width:100%;" colspan="2">
            <asp:Label ID="SavedMessage" runat="server"  Font-Size="X-Small" EnableViewState="false"
            Visible="false" SkinID="GoodCondition"></asp:Label>
            <asp:Label ID="ErrorMessageLabel" runat="server" Font-Size="X-Small" Text="" EnableViewState="false"
            Visible="true" SkinID="ErrorCondition"></asp:Label>
            </td>
       </tr>
       <tr><td>  
     <ajax:UpdatePanel ID="OrderNotesAjax" runat="server" UpdateMode="Conditional">
       <ContentTemplate>
       <div class="sc_menu_wrapper">
        <asp:GridView ID="OrderNotesGrid" runat="server" AutoGenerateColumns="False" DataKeyNames="OrderNoteId" AllowPaging="true" 
            OnRowEditing="OrderNotesGrid_RowEditing" OnRowCancelingEdit="OrderNotesGrid_RowCancelingEdit" OnRowUpdating="OrderNotesGrid_RowUpdating" 
            OnRowDeleting="OrderNotesGrid_RowDeleting" CellSpacing="0" CellPadding="4" Width="730px" SkinID="Summary" >
            <Columns>
                <asp:TemplateField HeaderText="Created">
                    <ItemStyle VerticalAlign="Top" HorizontalAlign="Left" BorderWidth="0" Width="118px" />
                    <headerstyle horizontalalign="Left" />
                    <ItemTemplate>
                        <asp:Label ID="CreatedDate" runat="server" Text='<%# FormatNoteDate(Container.DataItem)  %>'></asp:Label><br />
                        <asp:Label ID="Author" runat="server" Text='<%# Eval("User.UserName") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Comment">
                    <ItemStyle HorizontalAlign="Left" Width="350px" />
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemTemplate>
                        <asp:Label ID="Comment" runat="server" Text='<%# Eval("Comment") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="EditComment" runat="server" Text='<%# Eval("Comment") %>' TextMode="MultiLine" Rows="4" Columns="40"></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Private">
                    <ItemStyle HorizontalAlign="Center" BorderWidth="0" Width="50" />
                    <ItemTemplate>
                        <asp:CheckBox ID="IsPrivate" runat="server" Enabled="false" Checked='<%# Eval("IsPrivate") %>'></asp:CheckBox>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:CheckBox ID="EditIsPrivate" runat="server" Checked='<%# Eval("IsPrivate") %>'></asp:CheckBox>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="False">
                     <ItemStyle HorizontalAlign="Center" BorderWidth="1" Width="75" />
                    <ItemTemplate>
                        <asp:ImageButton ID="EditLink" runat="Server" CommandName="Edit" Text="Edit" Visible='<%#!(bool)Eval("IsSystem")%>' SkinID="EditIcon" ToolTip="Edit"></asp:ImageButton>
                        <asp:ImageButton ID="DeleteLink" runat="Server" CommandName="Delete" Text="Delete" Visible='<%#!(bool)Eval("IsSystem")%>' OnClientClick="return confirm('Are you sure you want to delete this note?')" SkinID="DeleteIcon" ToolTip="Delete"></asp:ImageButton>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:ImageButton ID="SaveLink" runat="Server" CommandName="Update" Text="Save" ToolTip="Save" SkinID="SaveIcon"></asp:ImageButton>
                        <asp:ImageButton ID="CancelLink" runat="Server" CommandName="Cancel" Text="Cancel" ToolTip="Cancel" SkinID="CancelIcon"></asp:ImageButton>
                    </EditItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
            </div>
       
        </ContentTemplate>
     </ajax:UpdatePanel>
        </td></tr>
         <ajax:UpdatePanel ID="PhoneNotesAjax" runat="server" >
         <ContentTemplate>
          <tr>
            <th class="sectionHeader">
                Customer Service History and Notes
           </th>
        </tr>
        <tr><td>
       
          <div class="sc_menu_wrapper">
         <asp:GridView ID="PhoneNotesGrid" runat="server" AutoGenerateColumns="False" DataKeyNames="PhoneNoteId" AllowPaging="true" 
            OnRowEditing="PhoneNotesGrid_RowEditing" OnRowCancelingEdit="PhoneNotesGrid_RowCancelingEdit" OnRowUpdating="PhoneNotesGrid_RowUpdating" 
            OnRowDeleting="PhoneNotesGrid_RowDeleting" CellSpacing="0" CellPadding="4" Width="730px" SkinID="Summary">
            <Columns>
                <asp:TemplateField HeaderText="Created">
                    <ItemStyle VerticalAlign="Top" HorizontalAlign="Left" BorderWidth="0" Width="118px" />
                    <headerstyle horizontalalign="Left" />
                    <ItemTemplate>
                        <asp:Label ID="CreatedDate" runat="server" Text='<%# FormatPhoneDate(Container.DataItem) %>'></asp:Label><br />
                        <asp:Label ID="Author" runat="server" Text='<%# Eval("User.UserName") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Comment">
                    <ItemStyle HorizontalAlign="Left" Width="350px" />
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemTemplate>
                        <asp:Label ID="Comment" runat="server" Text='<%# Eval("Comment") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="EditComment" runat="server" Text='<%# Eval("Comment") %>' TextMode="MultiLine" Rows="4" Columns="40"></asp:TextBox>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Private">
                     <ItemStyle HorizontalAlign="Center" BorderWidth="0" Width="50" />
                    <ItemTemplate>
                        <asp:CheckBox ID="IsPrivate" runat="server" Enabled="false" Checked='<%# Eval("IsPrivate") %>'></asp:CheckBox>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:CheckBox ID="EditIsPrivate" runat="server" Checked='<%# Eval("IsPrivate") %>'></asp:CheckBox>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="False">
                 <ItemStyle HorizontalAlign="Center" BorderWidth="1" Width="75" />
                    <ItemTemplate>
                        <asp:ImageButton ID="EditLink" runat="Server" CommandName="Edit" Text="Edit" Visible='<%#!(bool)Eval("IsSystem")%>' SkinID="EditIcon" ToolTip="Edit"></asp:ImageButton>
                        <asp:ImageButton ID="DeleteLink" runat="Server" CommandName="Delete" Text="Delete" Visible='<%#!(bool)Eval("IsSystem")%>' OnClientClick="return confirm('Are you sure you want to delete this note?')" SkinID="DeleteIcon" ToolTip="Delete"></asp:ImageButton>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:ImageButton ID="SaveLink" runat="Server" CommandName="Update" Text="Save" ToolTip="Save" SkinID="SaveIcon"></asp:ImageButton>
                        <asp:ImageButton ID="CancelLink" runat="Server" CommandName="Cancel" Text="Cancel" ToolTip="Cancel" SkinID="CancelIcon"></asp:ImageButton>
                    </EditItemTemplate>
                </asp:TemplateField>
            </Columns>
             <EmptyDataTemplate>
                        <asp:Label ID="EmptyResultsMessage" runat="server" Text="There are no phone notes available."></asp:Label>
              </EmptyDataTemplate>
        </asp:GridView>
        </div>
        </ContentTemplate>
        </ajax:UpdatePanel>
            
        
        </td></tr>
        </table>
        
    </div>
  
</asp:Content>