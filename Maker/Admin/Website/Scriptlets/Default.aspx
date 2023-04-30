<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"
CodeFile="Default.aspx.cs" Inherits="Admin_Website_Scriptlets_Default"
Title="Manage Scriptlets" EnableViewState="true" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb"
%> <%@ Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>

<asp:Content ID="Content" ContentPlaceHolderID="MainContent" Runat="Server">
  <div class="pageHeader">
    <div class="caption">
      <h1>
        <asp:Localize
          ID="Caption"
          runat="server"
          Text="Manage Content and Layout (Scriptlets)"
        ></asp:Localize>
      </h1>
    </div>
  </div>
  <table cellpadding="2" cellspacing="0" class="innerLayout">
    <tr>
      <td colspan="2">
        <asp:Label
          ID="InstructionText"
          runat="server"
          Text="Scriptlets are used to control layout and content for pages in your store website."
        ></asp:Label>
        <br /><br />
      </td>
    </tr>
    <tr>
      <td>
        <table width="100%">
          <tr>
            <td colspan="2">
              <asp:Label
                ID="StoreThemeLabel"
                runat="server"
                Text="Show Scriptlets From Theme:"
              />
              <asp:DropDownList
                ID="ThemeDropdown"
                runat="server"
                AutoPostBack="True"
                OnSelectedIndexChanged="ThemeDropdown_SelectedIndexChanged"
              >
                <asp:ListItem
                  Text="-- Default Store Scriptlets --"
                  Value=""
                ></asp:ListItem>
              </asp:DropDownList>
            </td>
          </tr>
        </table>
      </td>
      <td valign="top" width="30%">
        <div class="section">
          <div class="header">
            <h2 class="addscriptlet">
              <asp:Localize
                ID="AddCaption"
                runat="server"
                Text="Add Scriptlet"
              />
            </h2>
          </div>
          <div class="content">
            <ajax:UpdatePanel
              ID="AddAjax"
              runat="server"
              UpdateMode="Conditional"
            >
              <ContentTemplate>
                <asp:ValidationSummary
                  ID="ValidationSummary1"
                  runat="server"
                  ValidationGroup="Add"
                />
                <table class="inputForm">
                  <tr>
                    <th align="right">
                      <cb:ToolTipLabel
                        ID="NewScriptletTypeLabel"
                        runat="server"
                        Text="Type:"
                        AssociatedControlID="NewScriptletType"
                        ToolTip="The type of the scriptlet to add."
                      ></cb:ToolTipLabel>
                    </th>
                    <td colspan="2">
                      <asp:DropDownList ID="NewScriptletType" runat="server">
                        <asp:ListItem Text=""></asp:ListItem>
                        <asp:ListItem Text="Layout"></asp:ListItem>
                        <asp:ListItem Text="Header"></asp:ListItem>
                        <asp:ListItem Text="Footer"></asp:ListItem>
                        <asp:ListItem Text="Sidebar"></asp:ListItem>
                        <asp:ListItem Text="Content"></asp:ListItem>
                      </asp:DropDownList>
                      <asp:RequiredFieldValidator
                        ID="NewScriptletTypeRequired"
                        runat="server"
                        Text="*"
                        ValidationGroup="Add"
                        ControlToValidate="NewScriptletType"
                        ErrorMessage="Type is required."
                        Display="Dynamic"
                      ></asp:RequiredFieldValidator>
                    </td>
                  </tr>
                  <tr>
                    <th align="right">
                      <cb:ToolTipLabel
                        ID="NewScriptletNameLabel"
                        runat="server"
                        Text="Name:"
                        AssociatedControlID="NewScriptletName"
                        ToolTip="The name of the scriptlet; this must be unique among all scriptlets of this type."
                      ></cb:ToolTipLabel>
                    </th>
                    <td>
                      <asp:TextBox
                        ID="NewScriptletName"
                        runat="server"
                        Text=""
                        ValidationGroup="Add"
                      ></asp:TextBox>
                      <asp:PlaceHolder
                        ID="NewScriptletNameUnique"
                        runat="server"
                      ></asp:PlaceHolder>
                      <asp:RequiredFieldValidator
                        ID="NewScriptletNameRequired"
                        runat="server"
                        Text="*"
                        ValidationGroup="Add"
                        ControlToValidate="NewScriptletName"
                        ErrorMessage="Name is required."
                        Display="Dynamic"
                      ></asp:RequiredFieldValidator>
                      <asp:RegularExpressionValidator
                        ID="RegularExpressionValidator1"
                        runat="server"
                        Text="*"
                        ValidationGroup="Add"
                        ValidationExpression="[\w ]+"
                        ControlToValidate="NewScriptletName"
                        ErrorMessage="Name can only contain letters, numbers, space, and underscore."
                        Display="Dynamic"
                      ></asp:RegularExpressionValidator>
                    </td>
                    <td>
                      <asp:Button
                        ID="NewScriptletButton"
                        runat="server"
                        Text="Add"
                        ValidationGroup="Add"
                        OnClick="NewScriptletButton_Click"
                      />
                    </td>
                  </tr>
                </table>
              </ContentTemplate>
            </ajax:UpdatePanel>
          </div>
        </div>
      </td>
    </tr>
    <asp:PlaceHolder ID="phUsingDefault" runat="server" Visible="false">
      <tr>
        <td
          colspan="2"
          valign="top"
          width="100%"
          style="padding: 5px 20px 15px 20px"
        >
          <asp:Label
            SkinID="GoodCondition"
            runat="server"
            ID="UsingDefaultLabel"
            Text=""
            EnableViewState="false"
          ></asp:Label>
          <asp:Label
            ID="WriteAccessErrorMessage"
            runat="server"
            SkinID="errorCondition"
            EnableViewState="false"
            Visible="false"
          ></asp:Label>
        </td>
      </tr>
    </asp:PlaceHolder>
    <tr>
      <td colspan="2" valign="top" width="100%">
        <ajax:UpdatePanel
          ID="ScriptletAjax"
          runat="server"
          UpdateMode="Conditional"
        >
          <ContentTemplate>
            <asp:Label
              ID="ErrorMessage"
              runat="server"
              SkinID="ErrorCondition"
              EnableViewState="false"
              Visible="false"
            ></asp:Label>
            <cb:SortedGridView
              ID="ScriptletGrid"
              runat="server"
              AutoGenerateColumns="False"
              DataSourceID="ScriptletDs"
              SkinID="Summary"
              AllowSorting="True"
              DefaultSortExpression="Identifier"
              DefaultSortDirection="Ascending"
              Width="100%"
              ShowWhenEmpty="False"
              DataKeyNames="Identifier,ScriptletType"
              OnRowCommand="ScriptletGrid_RowCommand"
            >
              <Columns>
                <asp:BoundField
                  DataField="Identifier"
                  HeaderText="Name"
                  SortExpression="Identifier"
                  ReadOnly="True"
                >
                  <ItemStyle HorizontalAlign="Left" Width="150px" />
                  <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:BoundField
                  DataField="ScriptletType"
                  HeaderText="Type"
                  SortExpression="ScriptletType"
                  ReadOnly="True"
                >
                  <ItemStyle HorizontalAlign="Center" Width="80px" />
                  <HeaderStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField
                  DataField="Description"
                  HeaderText="Description"
                  SortExpression="Description"
                  ReadOnly="True"
                >
                  <ItemStyle HorizontalAlign="Left" />
                  <HeaderStyle HorizontalAlign="Left" />
                </asp:BoundField>
                <asp:TemplateField
                  HeaderText="Custom"
                  SortExpression="IsCustom"
                >
                  <ItemStyle HorizontalAlign="Center" Width="70px" />
                  <HeaderStyle HorizontalAlign="Center" />
                  <ItemTemplate>
                    <%# ((bool)Eval("IsCustom") ? "X" : "") %>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                  <ItemStyle
                    HorizontalAlign="Center"
                    Width="81px"
                    Wrap="false"
                  />
                  <ItemTemplate>
                    <asp:ImageButton
                      ID="CopyButton"
                      runat="server"
                      SkinID="CopyIcon"
                      CommandName="Copy"
                      CommandArgument='<%#string.Format("{0}:{1}", Eval("ScriptletType"), Eval("Identifier"))%>'
                      AlternateText="Copy"
                    />
                    <asp:HyperLink
                      ID="EditLink"
                      runat="server"
                      NavigateUrl='<%# string.Format("EditScriptlet.aspx?s={0}&t={1}&tid={2}", Eval("Identifier"), Eval("ScriptletType"),SelectedTheme) %>'
                      ><asp:Image
                        ID="EditIcon"
                        runat="server"
                        SkinID="EditIcon"
                        AlternateText="Edit"
                    /></asp:HyperLink>
                    <asp:ImageButton
                      ID="DeleteButton"
                      runat="server"
                      SkinID="DeleteIcon"
                      CommandName="DoDelete"
                      CommandArgument='<%#string.Format("{0}:{1}", Eval("ScriptletType"), Eval("Identifier"))%>'
                      OnClientClick='<%#Eval("Identifier", "return confirm(\"Are you sure you want to delete {0}?\")") %>'
                      AlternateText="Delete"
                      Visible='<%#(bool)Eval("IsCustom")%>'
                    />
                  </ItemTemplate>
                </asp:TemplateField>
              </Columns>
              <EmptyDataTemplate>
                <asp:Localize
                  ID="EmptyMessage"
                  runat="server"
                  Text="No scriptlets have been created."
                ></asp:Localize>
              </EmptyDataTemplate>
            </cb:SortedGridView>
          </ContentTemplate>
        </ajax:UpdatePanel>
        <asp:ObjectDataSource
          ID="ScriptletDs"
          runat="server"
          OldValuesParameterFormatString="original_{0}"
          SelectMethod="CacheLoad"
          TypeName="MakerShop.Stores.ScriptletDataSource"
          DataObjectTypeName="MakerShop.Stores.Scriptlet"
          SortParameterName="sortExpression"
          OnSelecting="ScriptletDs_Selecting"
        >
          <SelectParameters>
            <asp:Parameter
              DefaultValue="Unspecified"
              Name="scriptletType"
              Type="Object"
            />
          </SelectParameters>
        </asp:ObjectDataSource>
      </td>
    </tr>
  </table>
</asp:Content>
