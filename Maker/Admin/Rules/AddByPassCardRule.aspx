<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="AddByPassCardRule.aspx.cs" Inherits="Admin_Rules_AddByPassCardRule" Title="Untitled Page" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">

 <script type="text/javascript">
     function select_deselectAll(chkVal, idVal) {
         var frm = document.forms[0];
        
         
         // Loop through all elements
         for (i = 0; i < frm.length; i++) {
             // Look for our Header Template's Checkbox
             if (idVal.indexOf('CheckAll') != -1) {
                 // Check if main checkbox is checked, then select or deselect datagrid checkboxes
                 if (chkVal == true) {
                     frm.elements[i].checked = true;
                 }
                 else {
                     frm.elements[i].checked = false;
                 }
                 // Work here with the Item Template's multiple checkboxes
             }
             else if (idVal.indexOf('DeleteThis') != -1) {
                 // Check if any of the checkboxes are not checked, and then uncheck top select all checkbox
                 if (frm.elements[i].checked == false) {
                     frm.elements[1].checked = false; //Uncheck main select all checkbox
                 }
             }
         }

     }
</script>
 <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">


        <ContentTemplate>
        
            <div class="pageHeader">
                <div class="caption">
                    
                    <h1><asp:Localize ID="Caption" runat="server" Text="Add Credit Card whitelist rule"></asp:Localize>
                        <asp:Localize ID="ReportCaption" runat="server" Text="{0:d} to {1:d}" 
                            Visible="False" EnableViewState="False"></asp:Localize>
                      
                    </h1>
                </div>
                            </div>
            <table align="center" class="form" cellpadding="0" cellspacing="0" border="0">
            <tr><td valign="top">
            <table style="width:350px">
                 <tr class="noPrint">
                    <td style="width:40%;vertical-align:middle; ">
                        White List:
                            <br />
                    </td>
                        <td style="width:30%;vertical-align:middle; ">
                        <asp:DropDownList ID="dlWhiteListDataSource"  runat="server" AutoPostBack="True" 
                                onselectedindexchanged="ProcessWhiteListSelection_OnSelectedIndexChanged"/>
             <br />  
                    </td>
                    
                     <td style="text-align: left; vertical-align:middle; width:30%">                     
                    
                    </td>
                </tr>
                <tr><td colspan="2">&nbsp;</td></tr>
               
                <tr>
                    <td class="dataSheet" colspan="3">
                    
                    <asp:gridview  ID="lstRule" runat="server" AutoGenerateColumns="False" 
                            SkinID="PagedList" AllowPaging="true" PageSize="10" 
                             Width="100%" OnRowEditing="lstRule_EditCommand" 
                             OnPageIndexChanging="lstRule_PageIndexChanging"
                             OnRowCancelingEdit="lstRule_CancelCommand" 
                            OnRowUpdating ="lstRule_UpdateCommand" >
                            <Columns>
                             <asp:TemplateField >
                                    <HeaderStyle Width="40px" HorizontalAlign="left" />
                                    <HeaderTemplate><asp:CheckBox ID="CheckAll"  OnClick="javascript: return select_deselectAll (this.checked, this.id);" runat="server" />&nbsp
                                    <asp:LinkButton ID="lkDelete" runat="server" Text="Delete" OnClick="Delete_Click"/>
                                    </HeaderTemplate>
                                
                                    <ItemTemplate>
                                    <asp:CheckBox ID="DeleteThis" OnClick="javascript: return select_deselectAll (this.checked, this.id);" runat="server" /> 
                                    <asp:Label ID="itemId"  Font-Size="0" style="visibility:hidden;" runat="server" Text='<%# Eval("WhiteListId") %>' ></asp:Label>
                                  </ItemTemplate>
                              </asp:TemplateField>
                              
                                <asp:TemplateField HeaderText="White List Value" ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center">
                                   
                                    <ItemTemplate>
                                        <asp:Label ID="Name" runat="server" Text='<%#Eval("Value")%>'></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;
                                          <asp:ImageButton ID="EditButton" runat="server" CausesValidation="False" CommandName="Edit" SkinID="EditIcon" AlternateText="Edit" />
                                    
                                    </ItemTemplate>                         
                                    <EditItemTemplate>                                       
                                         <asp:Label ID="lbId" runat="server"  Text='<%# Eval("WhiteListId") %>' style="visibility:hidden;"  />
                                         <asp:TextBox ID="txtValue" runat="server" Text='<%# Eval("Value")  %>' />
                                         &nbsp;&nbsp;&nbsp;&nbsp;
                                         <asp:ImageButton ID="UpdateButton" runat="server" CausesValidation="False" CommandName="Update" SkinID="EditIcon" AlternateText="Edit" />
                                         <asp:ImageButton ID="CancelButton" runat="server" CausesValidation="False" CommandName="Cancel" SkinID="CancelIcon" AlternateText="Cancel" />
                                    </EditItemTemplate>
                                 
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <div class="emptyData">
                                    <asp:Label ID="lbNoRecord" runat="server" Text="No Record Found"></asp:Label><br /><br />
                                </div>
                            </EmptyDataTemplate>
                        </asp:gridview>
                    </td>
                </tr>
                
                </table>
            </td>
            <td valign="top" style="width: 350px">
                <div class="section">

                        <div class="header">
                            <h2 class="addgroup">Add white list items</h2>
                        </div>
                        <div class="content">
                            <div>
                                Enter one Item per line.
                                <br />
                                <br />
                            </div>
                            <textarea id="txtAddCardNumber" rows="10" cols="25" style="width:100%" runat="server" ></textarea><br />
                            <br />
                            <asp:Button ID="BtAddnew" runat="server" Text="Add New" OnClick="AddNewButton_Click"  class="button" />
                            <br />
                        </div>
                    
</div>
                </div>
            </td>
        </tr>
    </table>
 

            </table>

        </ContentTemplate>
    </ajax:UpdatePanel>

</asp:Content>

