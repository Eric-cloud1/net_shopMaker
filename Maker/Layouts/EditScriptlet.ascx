<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditScriptlet.ascx.cs" Inherits="Layouts_EditScriptlet" %>
<ajax:UpdatePanel ID="EditScriptletAjax" runat="server">
    <ContentTemplate>
        <div class="section">
            <div class="header">
                <h2><asp:Localize ID="Caption" runat="server" Text="Edit {0} Scriptlet '{1}'" EnableViewState="false" /></h2>
                <asp:HyperLink ID="EditScriptletsLink" runat="server" Text="Manage Scriptlets" NavigateUrl="~/Admin/Website/Scriptlets/Default.aspx" EnableViewState="false" Target="_blank"></asp:HyperLink>
            </div>
            <asp:Label ID="SavedMessage" runat="server" Text="Scriptlet saved at {0}" EnableViewState="false" SkinID="GoodCondition" Visible="false"></asp:Label>
            <table width="100%" class="inputForm">
                <tr>
                    <th class="rowHeader" valign="top" nowrap>
                        <asp:Label ID="ScriptletDataLabel" runat="server" Text="Content:" AssociatedControlID="ScriptletData" ToolTip="Specify the content that is associated with this scriptlet.  You can use the enhanced NVelocity syntax." EnableViewState="false"></asp:Label>
                    </th>
                    <td width="100%">
                        <asp:TextBox ID="ScriptletData" runat="server" TextMode="MultiLine" Width="98%" Height="200px" EnableViewState="false"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>
                        <asp:HyperLink ID="MoreButton" runat="server" Text="Advanced" SkinID="Button" EnableViewState="false" Target="_Blank"></asp:HyperLink>
                        <asp:LinkButton ID="WysiwygButton" runat="server" Text="HTML" SkinID="Button" EnableViewState="false"></asp:LinkButton>
                        <asp:LinkButton ID="CancelButton" runat="server" Text="Cancel" SkinID="Button" CausesValidation="false" EnableViewState="false" OnClick="CancelButton_Click" />
                        <asp:LinkButton ID="SaveButton" runat="server" Text="Save" SkinID="Button" EnableViewState="false" OnClick="SaveButton_Click" />
                    </td>
                </tr>
            </table>
        </div>
    </ContentTemplate>
</ajax:UpdatePanel>
