<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditLanguageDialog.ascx.cs" Inherits="Admin_Store_Regions_EditLanguageDialog" %>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register assembly="wwhoverpanel" Namespace="Westwind.Web.Controls" TagPrefix="wwh" %>
<script type="text/javascript">
    function ShowHoverPanel(event,Id)
    { 
        CCHoverLookupPanel2.startCallback(event,"ADDCC",null,OnError);    
    }
    function HideHoverPanel()
    {
        CCHoverLookupPanel2.hide();
        
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
<ajax:UpdatePanel ID="EditCurrencyAjax" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
    <asp:Button ID="EditLink" runat="server" EnableViewState="false" style="display:none"/>
    <asp:Panel ID="EditPanel" runat="server" Style="display:none;width:450px" CssClass="modalPopup">
        <asp:Panel ID="EditDialogHeader" runat="server" CssClass="modalPopupHeader" EnableViewState="false">
            <div class="commonicon"><asp:Localize ID="EditCaption" runat="server" Text="Edit ({0})" EnableViewState="false"/></div>
        </asp:Panel>
        <div style="padding-top:5px;">        
        <table cellpadding="4" cellspacing="0" class="inputForm">
              <tr>
              <td>&nbsp;</td>
                <td  class="validation" align="center">
                    <asp:Label ID="AddedMessage" runat="server" SkinID="GoodCondition" EnableViewState="false" Visible="false" Text="Language {0} added."></asp:Label>
                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="EditLanguage" />
                </td>
            </tr>
             <tr id="trDisplayOptions1a" runat="server" visible="false">
                <th class="rowHeader" nowrap>
                    <cb:ToolTipLabel ID="CultureLabel" runat="server" Text="Culture:" AssociatedControlID="Culture" ToolTip="SpecificCultures Language"></cb:ToolTipLabel>
                </th>
                <td>
                    <asp:DropDownList ID="Culturedl" runat="server">  
                    </asp:DropDownList>
                </td>
            </tr>
            
            
            <tr>
                <th class="rowHeader" nowrap>
                    <cb:ToolTipLabel ID="FieldNameLabel" runat="server" Text="Key:" AssociatedControlID="FieldName" ToolTip="Word to translate."></cb:ToolTipLabel>
                </th>
                <td colspan="3">
                    <asp:TextBox ID="FieldName" runat="server" MaxLength="250" Width="250px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="NameValidator" runat="server" ControlToValidate="FieldName"
                        Display="Static" ErrorMessage="Text is required." ValidationGroup="EditLanguage" Text="*"></asp:RequiredFieldValidator>
                </td>
            </tr>
         
            <tr>
                <th class="rowHeader" valign="top" nowrap>
                    <cb:ToolTipLabel ID="FieldValueLabel" runat="server" Text="Value:" AssociatedControlID="FieldValue" ToolTip=""></cb:ToolTipLabel>
                </th>
                <td colspan="3">
                    <asp:TextBox ID="FieldValue" runat="server"  Width="350px" TextMode="MultiLine"  Height="50px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="FieldValueValidator" runat="server" ControlToValidate="FieldValue"
                        Display="Static" ErrorMessage="Translation required." ValidationGroup="EditLanguage" Text="*"></asp:RequiredFieldValidator>
                </td>
            </tr>
        
            
             
            <tr>
                <th class="rowHeader" valign="top"  nowrap>
                    <cb:ToolTipLabel ID="ToolTipLabel1" runat="server" Text="Comment:" AssociatedControlID="Comment" ToolTip=""></cb:ToolTipLabel>
                </th>
                <td colspan="3">
                    <asp:TextBox ID="Comment" runat="server"  Width="350px" MaxLength="250" TextMode="MultiLine" Height="50px"></asp:TextBox>
   
                </td>
            </tr>
        
      
        
            <tr>
                <td>&nbsp;</td>
                <td colspan="3">
                    <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" ValidationGroup="EditLanguage" />
                    <asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" CausesValidation="false" />
                </td>
            </tr>           
        </table>
        </div>
        </asp:Panel>
        
 

        <ajax:ModalPopupExtender ID="EditPopup" runat="server"
                TargetControlID="EditLink"
                PopupControlID="EditPanel" 
                BackgroundCssClass="modalBackground"                         
                CancelControlID="CancelButton" 
                DropShadow="false"                
                PopupDragHandleControlID="EditDialogHeader" />
    </ContentTemplate>
</ajax:UpdatePanel>
