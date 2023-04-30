<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"
AutoEventWireup="true" CodeFile="TaxRules.aspx.cs"
Inherits="Admin_Taxes_TaxRules" Title="Tax Rules" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
  <ajax:UpdatePanel ID="TaxRuleAjax" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
      <div class="pageHeader">
        <div class="caption">
          <h1>
            <asp:Localize
              ID="Caption"
              runat="server"
              Text="Tax Rules"
            ></asp:Localize>
          </h1>
        </div>
      </div>
      <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
          <td class="itemlist">
            <asp:PlaceHolder
              ID="TaxesDisabledPanel"
              runat="server"
              EnableViewState="false"
              Visible="false"
            >
              <br />
              <asp:Localize
                ID="TaxesDisabledMessage"
                runat="server"
                EnableViewState="false"
              >
                WARNING: Taxes are currently disabled and the tax rules will
                have no effect. To alter your tax settings
                <a href="Settings.aspx">click here</a>.
              </asp:Localize>
              <br /><br />
            </asp:PlaceHolder>
            <asp:GridView
              ID="TaxRuleGrid"
              runat="server"
              AutoGenerateColumns="False"
              DataSourceID="TaxRuleDs"
              DataKeyNames="TaxRuleId"
              SkinID="PagedList"
              Width="100%"
            >
              <Columns>
                <asp:BoundField
                  DataField="Priority"
                  HeaderText="Priority"
                  ItemStyle-HorizontalAlign="center"
                />
                <asp:TemplateField
                  HeaderText="Name"
                  ItemStyle-HorizontalAlign="Left"
                >
                  <HeaderStyle HorizontalAlign="Left" />
                  <ItemTemplate>
                    <asp:Label
                      ID="Name"
                      runat="server"
                      Text='<%#Eval("Name")%>'
                    ></asp:Label>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Rate">
                  <ItemStyle HorizontalAlign="Center" Width="60" />
                  <ItemTemplate>
                    <asp:Label
                      ID="TaxRate"
                      runat="server"
                      Text='<%#Eval("TaxRate", "{0:0.####}%")%>'
                    ></asp:Label>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Applies&nbsp;To">
                  <ItemStyle HorizontalAlign="Center" />
                  <ItemTemplate>
                    <%#GetTaxCodes(Container.DataItem)%>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Address&nbsp;Filter">
                  <HeaderStyle HorizontalAlign="Left" />
                  <ItemTemplate>
                    <%#GetZoneNames(Container.DataItem)%>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Group&nbsp;Filter">
                  <HeaderStyle HorizontalAlign="Left" />
                  <ItemTemplate>
                    <%#GetGroupNames(Container.DataItem)%>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Tax&nbsp;Code">
                  <ItemStyle HorizontalAlign="Center" />
                  <ItemTemplate>
                    <%#GetTaxCodeName(Container.DataItem)%>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                  <ItemStyle
                    HorizontalAlign="Center"
                    Width="60px"
                    Wrap="false"
                  />
                  <ItemTemplate>
                    <asp:HyperLink
                      ID="EditLink"
                      runat="server"
                      NavigateUrl='<%#Eval("TaxRuleId", "EditTaxRule.aspx?TaxRuleId={0}")%>'
                      ><asp:Image
                        ID="EditIcon"
                        runat="server"
                        SkinID="EditIcon"
                    /></asp:HyperLink>
                    <asp:ImageButton
                      ID="DeleteButton"
                      runat="server"
                      CausesValidation="False"
                      CommandName="Delete"
                      SkinID="DeleteIcon"
                      OnClientClick='<%#Eval("Name", "return confirm(\"Are you sure you want to delete {0}?\")") %>'
                    />
                  </ItemTemplate>
                </asp:TemplateField>
              </Columns>
              <EmptyDataTemplate>
                <div class="emptyData">
                  <asp:Label
                    ID="TaxRulesInstructionText"
                    runat="server"
                    Text="There are no tax rules defined for your store."
                  ></asp:Label
                  ><br /><br />
                </div>
              </EmptyDataTemplate>
            </asp:GridView>
            <br /><asp:HyperLink
              ID="AddTaxRuleLink"
              runat="server"
              Text="Add Tax Rule"
              NavigateUrl="AddTaxRule.aspx"
              SkinID="Button"
            ></asp:HyperLink>
          </td>
        </tr>
      </table>
    </ContentTemplate>
  </ajax:UpdatePanel>
  <asp:ObjectDataSource
    ID="TaxRuleDs"
    runat="server"
    OldValuesParameterFormatString="original_{0}"
    SelectMethod="LoadForStore"
    TypeName="MakerShop.Taxes.TaxRuleDataSource"
    SelectCountMethod="CountForStore"
    SortParameterName="sortExpression"
    DataObjectTypeName="MakerShop.Taxes.TaxRule"
    DeleteMethod="Delete"
  >
  </asp:ObjectDataSource>
</asp:Content>
