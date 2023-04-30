<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AddLanguageDialog.ascx.cs" Inherits="Admin_Store_Regions_AddLanguageDialog" %>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
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
<ajax:UpdatePanel ID="AddLanguageAjax" runat="server" UpdateMode="Conditional" >    
    <ContentTemplate> 
         <asp:Label ID="SavedMessage" runat="server" Text="Key/Value pair has been saved." Visible="false" SkinID="GoodCondition"></asp:Label>
         <asp:Label ID="ErrorMessage" runat="server" Visible="false" SkinID="ErrorCondition"></asp:Label>
         <br />
          <asp:Button ID="AddLink" runat="server" Text="Add Language" EnableViewState="false" />
        <asp:Panel ID="AddDialog" runat="server" Style="display:none;width:500px" CssClass="modalPopup">
            <asp:Panel ID="AddDialogHeader" runat="server" CssClass="modalPopupHeader" EnableViewState="false">
                <div class="addlanguage"><asp:Localize ID="AddCaption" runat="server" Text="Add Language" /></div>
            </asp:Panel>
            <div style="padding-top:5px;">
                <table cellpadding="4" cellspacing="0" width="100%" class="inputForm">
                    <tr>
                    <td>&nbsp;</td>
                        <td  class="validation" align="center" >                            
                            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="Addlanguage" />
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader" nowrap>
                            <cb:ToolTipLabel ID="FieldNameLabel" runat="server" Text="Key:" AssociatedControlID="FieldAddName" ToolTip="Key(s) to Translate"></cb:ToolTipLabel>
                        </th>
                        <td>
                            <asp:TextBox ID="FieldAddName" runat="server" MaxLength="250"  Width="180px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="FieldNameValidator" runat="server" ControlToValidate="FieldAddName"
                                Display="Static" ErrorMessage="Key to Translate is required." ValidationGroup="Addlanguage" Text="*"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    
                     <tr>
                        <th class="rowHeader" valign="top" nowrap>
                            <cb:ToolTipLabel ID="FieldValueLabel" runat="server" Text="Value:" AssociatedControlID="FieldAddValue" ToolTip="Value Translation."></cb:ToolTipLabel>
                        </th>
                        <td>
                            <asp:TextBox ID="FieldAddValue" runat="server"  Width="350px"  TextMode="MultiLine" Height="50px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="FieldValueValidator" runat="server" ControlToValidate="FieldAddValue"
                                Display="Static" ErrorMessage="Value Translation is required." ValidationGroup="Addlanguage" Text="*"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                
                
                    <tr>
                        <th class="rowHeader" nowrap>
                            <cb:ToolTipLabel ID="CultureLabel" runat="server" Text="Culture:" AssociatedControlID="CultureAdddl" ToolTip="Culture Language"></cb:ToolTipLabel>
                        </th>
                        <td>
                        <asp:DropDownList ID="CultureAdddl" runat="server" Width="180px"></asp:DropDownList>
                        <asp:RequiredFieldValidator ID="CultureAdddlValidator" runat="server" ControlToValidate="CultureAdddl"  Text="*" ValidationGroup="Addlanguage" ErrorMessage="Please Select Language" InitialValue=""></asp:RequiredFieldValidator>

                        </td>
                    </tr>
                           
                     <tr>
                        <th class="rowHeader" valign="top" nowrap>
                            <cb:ToolTipLabel ID="ToolTipLabel1" runat="server" Text="Comment(s):" AssociatedControlID="AddComment" ToolTip="Comment(s)."></cb:ToolTipLabel>
                        </th>
                        <td>
                            <asp:TextBox ID="AddComment" runat="server" Width="350px" MaxLength="250" Text="" TextMode="MultiLine" Height="50px"></asp:TextBox>
                          
                        </td>
                    </tr>   
               
                    <tr >
                        <td>&nbsp;</td>
                        <td>
                            <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" ValidationGroup="Addlanguage" />
                            <asp:Button ID="CancelButton" runat="server" Text="Cancel"  CausesValidation="false"/>
                        </td>
                    </tr>
                </table>
            </div>
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
