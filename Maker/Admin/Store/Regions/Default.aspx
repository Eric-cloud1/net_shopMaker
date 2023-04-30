<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"
AutoEventWireup="true" CodeFile="Default.aspx.cs"
Inherits="Admin_Store_Regions_Default" Title="Untitled Page" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb"
%> <%@ Register Src="AddLanguageDialog.ascx" TagName="AddLanguageDialog"
TagPrefix="uc1" %> <%@ Register Src="EditLanguageDialog.ascx"
TagName="EditLanguageDialog" TagPrefix="uc1" %>
<asp:Content ID="Content" ContentPlaceHolderID="MainContent" runat="Server">
  <div class="pageHeader">
    <div class="caption">
      <h1>
        <asp:Localize
          ID="Caption"
          runat="server"
          Text="Manage Languages"
        ></asp:Localize>
      </h1>
    </div>
  </div>
  <div style="margin: 10px 20px 10px 20px">
    <asp:Localize
      ID="InstructionText"
      runat="server"
      Text="Define the languages that the system support.  English is the base language, other language is a display-only feature."
    ></asp:Localize>
  </div>
  <table cellpadding="2" cellspacing="0" class="innerLayout">
    <tr>
      <td colspan="2" class="itemList">
        <ajax:UpdatePanel
          ID="LanguageAjax"
          runat="server"
          UpdateMode="Conditional"
        >
          <ContentTemplate>
            <uc1:AddLanguageDialog
              ID="AddLanguageDialog1"
              runat="server"
            ></uc1:AddLanguageDialog>

            <cb:SortedGridView
              ID="LanguageGrid"
              runat="server"
              AutoGenerateColumns="False"
              DataKeyNames="LanguageId"
              DataSourceID="LanguageDs"
              Width="100%"
              SkinID="Summary"
              AllowSorting="True"
              DefaultSortExpression="Culture"
              DefaultSortDirection="Ascending"
              ShowWhenEmpty="False"
              OnRowEditing="LanguageGrid_RowEditing"
            >
              <Columns>
                <asp:TemplateField
                  HeaderText="Culture"
                  SortExpression="Culture"
                >
                  <ItemStyle HorizontalAlign="Left" />
                  <HeaderStyle HorizontalAlign="Left" Width="60px" />
                  <ItemTemplate>
                    <asp:Label
                      ID="Culture"
                      runat="server"
                      Text="<%#GetLanguage(Container.DataItem)%>"
                    ></asp:Label>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Key" SortExpression="FieldName">
                  <ItemStyle HorizontalAlign="Left" />
                  <HeaderStyle HorizontalAlign="Left" Width="70px" />
                  <ItemTemplate>
                    <asp:Label
                      ID="FieldName"
                      runat="server"
                      Text='<%#Eval("FieldName")%>'
                    ></asp:Label>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField
                  HeaderText="Value"
                  SortExpression="FieldValue"
                >
                  <ItemStyle HorizontalAlign="Left" />
                  <HeaderStyle HorizontalAlign="Left" />
                  <ItemTemplate>
                    <asp:Label
                      ID="FieldValue"
                      runat="server"
                      Text='<%#Eval("FieldValue")%>'
                    ></asp:Label>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField
                  HeaderText="Comment"
                  SortExpression="Comment"
                >
                  <ItemStyle HorizontalAlign="Left" />
                  <HeaderStyle HorizontalAlign="Left" />
                  <ItemTemplate>
                    <asp:Label
                      ID="Comment"
                      runat="server"
                      Text='<%#Eval("Comment")%>'
                    ></asp:Label>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                  <ItemStyle
                    HorizontalAlign="Center"
                    Width="90px"
                    Wrap="false"
                  />
                  <ItemTemplate>
                    <asp:ImageButton
                      ID="EditButton"
                      runat="server"
                      SkinID="EditIcon"
                      CommandName="Edit"
                      AlternateText="Edit"
                    />
                    <asp:ImageButton
                      ID="DeleteButton"
                      runat="server"
                      SkinID="DeleteIcon"
                      CommandName="Delete"
                      OnClientClick='<%#Eval("FieldName", "return confirm(\"Are you sure you want to delete {0}?\")") %>'
                      AlternateText="Delete"
                    />
                  </ItemTemplate>
                </asp:TemplateField>
              </Columns>
              <EmptyDataTemplate>
                <asp:Localize
                  ID="EmptyMessage"
                  runat="server"
                  Text="There is no translations defined for your store."
                ></asp:Localize>
              </EmptyDataTemplate>
            </cb:SortedGridView>

            <uc1:EditLanguageDialog
              ID="EditLanguageDialog1"
              runat="server"
            ></uc1:EditLanguageDialog>
          </ContentTemplate>
        </ajax:UpdatePanel>
        <asp:ObjectDataSource
          ID="LanguageDs"
          runat="server"
          OldValuesParameterFormatString="original_{0}"
          SelectMethod="LoadForAll"
          TypeName="MakerShop.Utility.LanguageTranslationDataSource"
          DataObjectTypeName="MakerShop.Utility.LanguageTranslation"
          DeleteMethod="Delete"
          UpdateMethod="Update"
          SortParameterName="sortExpression"
          InsertMethod="Insert"
        >
        </asp:ObjectDataSource>
      </td>
    </tr>
  </table>
</asp:Content>
