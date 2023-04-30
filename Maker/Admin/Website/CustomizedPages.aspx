<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"
AutoEventWireup="true" CodeFile="CustomizedPages.aspx.cs"
Inherits="Admin_Website_CustomizedPages" Title="Customized Pages" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
  <div class="pageHeader">
    <div class="caption">
      <h1>
        <asp:Localize
          ID="Caption"
          runat="server"
          Text="Customized Pages"
        ></asp:Localize>
      </h1>
    </div>
  </div>
  <table cellpadding="0" cellspacing="2" class="innerLayout">
    <tr>
      <td>
        <p>
          The pages below have been customized for either theme or scriptlet
          choices. You can use this page to reset the customizations in the
          event you cannot access the page.
        </p>
        <div align="center">
          <asp:GridView
            ID="PageGrid"
            runat="server"
            AutoGenerateColumns="False"
            DataKeyNames="PersonalizationPathId"
            DataSourceID="CustomizedPagesDs"
            SkinID="Summary"
            AllowSorting="False"
            OnRowDeleted="PageGrid_RowDeleted"
          >
            <Columns>
              <asp:TemplateField HeaderText="File">
                <ItemTemplate>
                  <%# Eval("PersonalizationPath.Path") %>
                </ItemTemplate>
              </asp:TemplateField>
              <asp:BoundField HeaderText="Theme" DataField="Theme" />
              <asp:TemplateField HeaderText="Scriptlets">
                <ItemStyle Width="60px" HorizontalAlign="center" />
                <ItemTemplate>
                  <%# HasData(Container.DataItem) %>
                </ItemTemplate>
              </asp:TemplateField>
              <asp:BoundField
                HeaderText="Last Modified"
                DataField="LastUpdatedDate"
              />
              <asp:TemplateField>
                <ItemStyle Width="30px" HorizontalAlign="center" />
                <ItemTemplate>
                  <asp:ImageButton
                    ID="Delete"
                    runat="server"
                    SkinId="DeleteIcon"
                    CommandName="Delete"
                    OnClientClick="return confirm('Are you sure you want to delete customizations to this page?');"
                  ></asp:ImageButton>
                </ItemTemplate>
              </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
              There are no customized pages.
            </EmptyDataTemplate>
          </asp:GridView>
        </div>
        <asp:ObjectDataSource
          ID="CustomizedPagesDs"
          runat="server"
          DeleteMethod="Delete"
          SelectMethod="LoadForCriteria"
          TypeName="MakerShop.Personalization.SharedPersonalizationDataSource"
        >
          <DeleteParameters>
            <asp:Parameter Name="personalizationPathId" Type="Int32" />
          </DeleteParameters>
          <SelectParameters>
            <asp:Parameter Name="sqlCriteria" Type="String" />
          </SelectParameters>
        </asp:ObjectDataSource>
      </td>
    </tr>
  </table>
</asp:Content>
