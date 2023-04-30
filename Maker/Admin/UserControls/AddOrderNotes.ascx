<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AddOrderNotes.ascx.cs" Inherits="Admin_UserControls_AddOrderNotes" %>
<%@ Register assembly="wwhoverpanel" Namespace="Westwind.Web.Controls" TagPrefix="wwh" %>
<script type="text/javascript">
    function ShowHoverPanel(event,Id)
    { 
        CCHoverLookupPanel1.startCallback(event,"ADDCC",null,OnError);    
    }
    function HideHoverPanel()
    {
        CCHoverLookupPanel1.hide();
        
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
      
</script>
<ajax:UpdatePanel ID="AddCurrencyAjax" runat="server" UpdateMode="Conditional" >
    
    <ContentTemplate>
        <asp:Button ID="AddLink" runat="server" Text="Add Comment" EnableViewState="false" />
   <asp:Panel ID="AddDialog" runat="server" Style="display:none;width:450px" CssClass="modalPopup">
        <table class="inputForm" Style="width:425px">
            <tr class="sectionHeader">
                <th style="text-align:left">
                    <asp:Localize ID="AddCommentCaption" runat="server" Text="Add Comment"></asp:Localize>
                </th>
            </tr>
            <tr>
                <td Style="width:425px">
                    <asp:TextBox ID="AddComment" runat="server" TextMode="MultiLine" Rows="4" Columns="50" Width="425px"></asp:TextBox><br />
                    <asp:CheckBox ID="AddIsPrivate" runat="server" Text="Make comment private." />
                </td>
            </tr>
            <tr>
                <td>
                 <asp:Button ID="AddButton" runat="server" Text="Save Comment" OnClick="AddButton_Click" />
                <asp:Button ID="CancelButton" runat="server" Text="Cancel"  CausesValidation="false"/>
                </td>
            </tr>
        </table>
    </asp:Panel>

   
        <ajax:ModalPopupExtender ID="AddPopup" runat="server" 
                TargetControlID="AddLink"
                PopupControlID="AddDialog" 
                BackgroundCssClass="modalBackground"
                CancelControlID="CancelButton"
                DropShadow="false"                
                PopupDragHandleControlID="AddDialogHeader" />
   </ContentTemplate>
</ajax:UpdatePanel>
