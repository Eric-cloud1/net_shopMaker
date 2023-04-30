<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"
CodeFile="EditScriptlet.aspx.cs"
Inherits="Admin_Website_Scriptlets_EditScriptlet" Title="Edit Scriptlet"
EnableViewState="false" %> <%@ Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>

<asp:Content ID="Content" ContentPlaceHolderID="MainContent" Runat="Server">
  <div class="pageHeader">
    <div class="caption">
      <h1>
        <asp:Localize
          ID="Caption"
          runat="server"
          Text="Edit {0} Scriptlet '{1}'"
        ></asp:Localize>
      </h1>
    </div>
  </div>
  <table cellpadding="2" cellspacing="0" class="innerLayout">
    <tr>
      <td valign="top">
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
        <asp:Label
          ID="ErrorMessage"
          runat="server"
          SkinID="ErrorCondition"
          EnableViewState="false"
          Visible="false"
        ></asp:Label>
        <table class="inputForm" width="100%">
          <tr>
            <th class="rowHeader" nowrap>
              <cb:ToolTipLabel
                ID="IdentifierLabel"
                runat="server"
                Text="Name:"
                AssociatedControlID="Identifier"
                ToolTip="The name of the scriptlet; this must be unique among scriptlets with the same type."
              ></cb:ToolTipLabel>
            </th>
            <td width="100%" nowrap>
              <asp:TextBox
                ID="Identifier"
                runat="server"
                Width="200px"
              ></asp:TextBox>
              <asp:PlaceHolder
                ID="IdentifierUnique"
                runat="server"
              ></asp:PlaceHolder>
              <asp:RequiredFieldValidator
                ID="IdentifierRequired"
                runat="server"
                Text="*"
                ControlToValidate="Identifier"
                ErrorMessage="Name is required."
                Display="Dynamic"
              ></asp:RequiredFieldValidator>
              <asp:RegularExpressionValidator
                ID="RegularExpressionValidator1"
                runat="server"
                Text="*"
                ValidationExpression="[\w ]+"
                ControlToValidate="Identifier"
                ErrorMessage="Name can only contain letters, numbers, space, and underscore."
                Display="Dynamic"
              ></asp:RegularExpressionValidator>
            </td>
          </tr>
          <tr>
            <th class="rowHeader" valign="top" nowrap>
              <cb:ToolTipLabel
                ID="DescriptionLabel"
                runat="server"
                Text="Description:"
                AssociatedControlID="Description"
                ToolTip="Optional description for this scriptlet."
              ></cb:ToolTipLabel>
            </th>
            <td width="100%" nowrap>
              <asp:TextBox
                ID="Description"
                runat="server"
                TextMode="MultiLine"
                Width="98%"
                Height="50px"
              ></asp:TextBox>
            </td>
          </tr>
          <tr>
            <th class="rowHeader" valign="top" nowrap>
              <cb:ToolTipLabel
                ID="HeaderLabel"
                runat="server"
                Text="Header:"
                AssociatedControlID="Description"
                ToolTip="Optional content to place into the HEAD portion of the webpage."
              ></cb:ToolTipLabel>
            </th>
            <td width="100%" nowrap>
              <asp:TextBox
                ID="HeaderData"
                runat="server"
                TextMode="MultiLine"
                Width="98%"
                Height="80px"
              ></asp:TextBox>
            </td>
          </tr>
          <tr>
            <th class="rowHeader" valign="top" nowrap>
              <cb:ToolTipLabel
                ID="ScriptletDataLabel"
                runat="server"
                Text="Content:"
                AssociatedControlID="ScriptletData"
                ToolTip="Specify the content that is associated with this scriptlet.  You can use the enhanced NVelocity syntax."
              ></cb:ToolTipLabel
              ><br />
              <asp:ImageButton
                ID="ConLibHelpLink"
                runat="server"
                SkinID="HelpIcon"
                AlternateText="ConLib Reference"
              />
              <asp:ImageButton
                ID="ScriptletDataHtml"
                runat="server"
                SkinID="EditIcon"
                AlternateText="HTML Editor"
              />
            </th>
            <td width="100%">
              <asp:TextBox
                ID="ScriptletData"
                runat="server"
                TextMode="MultiLine"
                Width="98%"
                Height="200px"
              ></asp:TextBox
              ><br />
            </td>
          </tr>
          <tr>
            <td>&nbsp;</td>
            <td>
              <asp:Button
                ID="SaveButton"
                runat="server"
                Text="Save"
                OnClick="SaveButton_Click"
              />
              <asp:Button
                ID="CancelButton"
                runat="server"
                Text="Cancel"
                OnClick="CancelButton_Click"
                CausesValidation="false"
              />
            </td>
          </tr>
        </table>
      </td>
    </tr>
  </table>
</asp:Content>
