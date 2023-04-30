<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CallDisposition.ascx.cs" Inherits="Admin_UserControls_CallDisposition" %>
<asp:Panel runat="server" ID="CallDispositionPanel" Style="display: inline;" Enabled="true">
<table width="100%">
<tr >
<td width="100%" align="left" colspan="2">
<asp:Label ID="lbStatus" Text="Status:" runat="server" Style="display: inline; font-weight: bold;" />
</td></tr>
<tr><td align="left" width="100%" colspan="2"><asp:DropDownList ID="statusdl" runat="server" /></td></tr>

<tr >
<td width="100%" align="left" colspan="2">
<asp:Label ID="lbReached" Text="Reached on return call:" runat="server" Style="display: inline; font-weight: bold;" />
</td></tr>


<tr><td width="100%" align="left" colspan="2">
<asp:TextBox ID="callNotesTxt" runat="server" TextMode="MultiLine" Height="50px" Wrap="true" Width="100%" />
</td></tr>

<tr><td width="100%" colspan="2" align="center">
<asp:Button ID="Save" runat="server" Width="47%" OnClick="Save_Click" Text="Save"  SkinID="Button" />&nbsp;<asp:Button ID="Exit" Width="47%" OnClick="Exit_Click" runat="server" Text="Exit" SkinID="Button"  />

</td></tr>
<tr><td width="100%" align="left" colspan="2">
 <asp:GridView ID="gridPhoneNotes" runat="server" AutoGenerateColumns="False" DefaultSortExpression=""
   DefaultSortDirection="Ascending" AllowPaging="true" AllowSorting="true" SkinID="Summary"
    OnSorting="gridPhoneNotes_Sorting" PageSize="10" OnPageIndexChanging="gridPhoneNotes_PageIndexChanging" Width="100%">
    <Columns>
    <asp:TemplateField HeaderText="OrderId" SortExpression="OrderId" Visible="false">
     <ItemStyle HorizontalAlign="right" Width="90%" />
      <ItemTemplate>
        <asp:Label ID="lbOrderId" runat="server" Text='<%# Eval("OrderId", "{0}") %>' Width="75px"></asp:Label>
      </ItemTemplate>
     </asp:TemplateField>
     
  
     
         <asp:TemplateField HeaderText="Created Date" SortExpression="CreatedDate" >
     <ItemStyle HorizontalAlign="right" Width="90%" />
      <ItemTemplate>
        <asp:Label ID="lbOrderId" runat="server" Text='<% # FormatDate(Container.DataItem) %>' Width="75px"></asp:Label>
      </ItemTemplate>
     </asp:TemplateField>
     
            <asp:TemplateField HeaderText="Comment" SortExpression="Comment" >
     <ItemStyle HorizontalAlign="right" Width="90%" />
      <ItemTemplate>
        <asp:Label ID="lbComment" runat="server" Text='<%# Eval("Comment", "{0}") %>' Width="75px"></asp:Label>
      </ItemTemplate>
     </asp:TemplateField>
     
     </Columns>
     </asp:GridView>
     </td></tr>
     </table>
</asp:Panel>
                                    