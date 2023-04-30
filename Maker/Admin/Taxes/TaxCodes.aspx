<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TaxCodes.aspx.cs"
Inherits="Admin_Taxes_TaxCodes" %> <%@ Register TagPrefix="ComponentArt"
Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb"
%>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
  <ajax:UpdatePanel ID="ContentAjax" runat="server" UpdateMode="conditional">
    <ContentTemplate>
      <div class="pageHeader">
        <div class="caption">
          <h1>
            <asp:Localize
              ID="Caption"
              runat="server"
              Text="Tax Codes"
            ></asp:Localize>
          </h1>
        </div>
      </div>
      <asp:Panel
        ID="TaxSettingsWarning"
        runat="server"
        EnableViewState="false"
        Visible="false"
      >
        <asp:Label
          ID="TaxSettingsWarningMessage"
          runat="server"
          Text="Taxes are currently disabled."
          SkinID="warnCondition"
        ></asp:Label>
        <a href="TaxRules.aspx">Change Tax Settings</a>
      </asp:Panel>
      <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
          <td width="400" valign="top" class="itemlist">
            <asp:GridView
              ID="TaxCodeGrid"
              runat="server"
              AutoGenerateColumns="False"
              DataKeyNames="TaxCodeId"
              DataSourceId="TaxCodeDs"
              OnRowUpdating="TaxCodeGrid_RowUpdating"
              SkinID="PagedList"
              Width="100%"
            >
              <Columns>
                <asp:TemplateField HeaderText="Name">
                  <HeaderStyle Width="120px" HorizontalAlign="left" />
                  <ItemStyle Width="120px" HorizontalAlign="left" />
                  <ItemTemplate>
                    <asp:Label
                      ID="Name"
                      runat="server"
                      Text='<%#Eval("Name")%>'
                    ></asp:Label>
                  </ItemTemplate>
                  <EditItemTemplate>
                    <asp:TextBox
                      ID="Name"
                      runat="server"
                      Text='<%#Eval("Name")%>'
                      Width="110px"
                      MaxLength="100"
                      ValidationGroup="Edit"
                      CausesValidation="true"
                    ></asp:TextBox>
                    <asp:PlaceHolder
                      ID="phGridNameValidator"
                      runat="server"
                      EnableViewState="false"
                    ></asp:PlaceHolder>
                    <cb:RequiredRegularExpressionValidator
                      ID="EditTaxCodeNameValidator"
                      runat="server"
                      ControlToValidate="Name"
                      Display="Static"
                      SetFocusOnError="true"
                      Enabled="true"
                      ErrorMessage="Tax code name must be between 1 and 100 characters in length."
                      Text="Tax code name must be between 1 and 100 characters in length."
                      ValidationGroup="Edit"
                      ValidationExpression=".{1,100}"
                      Required="true"
                    >
                    </cb:RequiredRegularExpressionValidator>
                  </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField
                  HeaderText="Products"
                  ItemStyle-HorizontalAlign="center"
                >
                  <ItemTemplate>
                    <asp:HyperLink
                      ID="ProductsLink"
                      runat="server"
                      Text='<%#Eval("Products.Count")%>'
                      NavigateUrl='<%# Eval("TaxCodeId", "TaxCodeProducts.aspx?TaxCodeId={0}")%>'
                      SkinID="GridLink"
                    ></asp:HyperLink>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField
                  HeaderText="Tax Rules"
                  ItemStyle-HorizontalAlign="center"
                >
                  <ItemTemplate>
                    <asp:HyperLink
                      ID="TaxRulesLink"
                      runat="server"
                      Text='<%#Eval("TaxRuleTaxCodes.Count")%>'
                      NavigateUrl='<%# Eval("TaxCodeId", "TaxCodeTaxRules.aspx?TaxCodeId={0}")%>'
                      SkinID="GridLink"
                    ></asp:HyperLink>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                  <ItemStyle HorizontalAlign="Center" />
                  <EditItemTemplate>
                    <asp:LinkButton
                      ID="SaveLinkButton"
                      runat="server"
                      CausesValidation="True"
                      CommandName="Update"
                      ValidationGroup="Edit"
                      ><asp:Image
                        ID="SaveIcon"
                        runat="server"
                        SkinID="SaveIcon"
                    /></asp:LinkButton>
                    <asp:LinkButton
                      ID="CancelLinkButton"
                      runat="server"
                      CausesValidation="False"
                      CommandName="Cancel"
                      ><asp:Image
                        ID="CancelIcon"
                        runat="server"
                        SkinID="CancelIcon"
                    /></asp:LinkButton>
                  </EditItemTemplate>
                  <ItemTemplate>
                    <asp:LinkButton
                      ID="EditLinkButton"
                      runat="server"
                      CausesValidation="False"
                      CommandName="Edit"
                      ><asp:Image
                        ID="EditIcon"
                        runat="server"
                        SkinID="EditIcon"
                    /></asp:LinkButton>
                    <asp:LinkButton
                      ID="DeleteLinkButton"
                      runat="server"
                      CausesValidation="False"
                      CommandName="Delete"
                      OnClientClick='<%# Eval("Name", "return confirm(\"Are you sure you want to delete {0}?\")") %>'
                      ><asp:Image
                        ID="DeleteIcon"
                        runat="server"
                        SkinID="DeleteIcon"
                    /></asp:LinkButton>
                  </ItemTemplate>
                </asp:TemplateField>
              </Columns>
              <EmptyDataTemplate>
                <div class="emptyData">
                  <asp:Label
                    ID="TaxCodesInstructionText"
                    runat="server"
                    Text="Define the tax codes that can be assigned to products in your store. You can then create tax rules that apply to your codes."
                  ></asp:Label
                  ><br /><br />
                </div>
              </EmptyDataTemplate>
            </asp:GridView>
          </td>
          <td width="300" valign="top" style="padding-left: 4px">
            <div class="section">
              <div class="header">
                <h2 class="addcurrency">
                  <asp:Localize
                    ID="AddCaption"
                    runat="server"
                    Text="Add Tax Code"
                  />
                </h2>
              </div>
              <div class="content">
                <asp:ValidationSummary
                  ID="ValidationSummary1"
                  runat="server"
                  ValidationGroup="Add"
                />
                <table class="inputForm">
                  <tr>
                    <th class="rowHeader">
                      <asp:Label
                        ID="AddTaxCodeNameLabel"
                        runat="server"
                        Text="Name:"
                        AssociatedControlID="AddTaxCodeName"
                        ToolTip="Tax code name"
                      ></asp:Label>
                    </th>
                    <td>
                      <asp:TextBox
                        ID="AddTaxCodeName"
                        runat="server"
                        Columns="20"
                        MaxLength="100"
                      ></asp:TextBox>
                      <cb:RequiredRegularExpressionValidator
                        ID="AddTaxCodeNameValidator"
                        runat="server"
                        ControlToValidate="AddTaxCodeName"
                        Display="Static"
                        ErrorMessage="Tax code name must be between 1 and 100 characters in length."
                        Text="*"
                        ValidationGroup="Add"
                        ValidationExpression=".{1,100}"
                        Required="true"
                      >
                      </cb:RequiredRegularExpressionValidator>
                      <asp:PlaceHolder
                        ID="phNameValidator"
                        runat="server"
                        EnableViewState="false"
                      ></asp:PlaceHolder>
                    </td>
                    <td>
                      <asp:Button
                        ID="AddButton"
                        runat="server"
                        Text="Add"
                        OnClick="AddButton_Click"
                        ValidationGroup="Add"
                      />
                    </td>
                  </tr>
                </table>
              </div>
            </div>
            <div align="center">
              <asp:HyperLink
                ID="TaxRulesLink"
                runat="server"
                NavigateUrl="TaxRules.aspx"
                Text="Edit Tax Rules"
              ></asp:HyperLink>
            </div>
          </td>
        </tr>
      </table>
    </ContentTemplate>
  </ajax:UpdatePanel>
  <asp:ObjectDataSource
    ID="TaxCodeDs"
    runat="server"
    OldValuesParameterFormatString="original_{0}"
    SelectMethod="LoadForStore"
    TypeName="MakerShop.Taxes.TaxCodeDataSource"
    SelectCountMethod="CountForStore"
    SortParameterName="sortExpression"
    DataObjectTypeName="MakerShop.Taxes.TaxCode"
    DeleteMethod="Delete"
  >
  </asp:ObjectDataSource>
</asp:Content>
