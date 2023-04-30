<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"
CodeFile="TaxCodeTaxRules.aspx.cs" Inherits="Admin_Taxes_TaxCodeTaxRules"
Title="Tax Code Rules" %> <%@ Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
  <ajax:UpdatePanel ID="PageAjax" runat="server">
    <ContentTemplate>
      <div class="pageHeader">
        <div class="caption">
          <h1>
            <asp:Localize
              ID="Caption"
              runat="server"
              Text="{0}: Linked Tax Rules"
            ></asp:Localize>
          </h1>
        </div>
      </div>
      <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
          <td>
            <cb:SortedGridView
              ID="TaxRuleGrid"
              runat="server"
              AutoGenerateColumns="False"
              DataKeyNames="TaxRuleId"
              DataSourceID="TaxRuleDs"
              Width="100%"
              SkinID="PagedList"
              AllowPaging="True"
              PageSize="20"
              AllowSorting="true"
              DefaultSortExpression="Name"
            >
              <Columns>
                <asp:TemplateField HeaderText="Linked">
                  <HeaderStyle Width="60px" />
                  <ItemStyle HorizontalAlign="Center" />
                  <ItemTemplate>
                    <asp:CheckBox
                      ID="Linked"
                      runat="server"
                      Checked="<%#IsLinked(Container.DataItem)%>"
                      Text='<%#Eval("TaxRuleId")%>'
                      CssClass="hiddenText"
                      AutoPostBack="true"
                      OnCheckedChanged="Linked_CheckChanged"
                    />
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Name" SortExpression="Name">
                  <HeaderStyle HorizontalAlign="Left" />
                  <ItemTemplate>
                    <asp:HyperLink
                      ID="NameLink"
                      runat="server"
                      Text='<%# Eval("Name") %>'
                      NavigateUrl='<%#String.Format("EditTaxRule.aspx?TaxRuleId={0}&TaxCodeId={1}", Eval("TaxRuleId"),Request.QueryString["TaxCodeId"] )%>'
                    ></asp:HyperLink>
                  </ItemTemplate>
                </asp:TemplateField>
              </Columns>
              <EmptyDataTemplate>
                <asp:Label
                  ID="EmptyDataMessage"
                  runat="server"
                  Text="There are no tax rules defined for your store."
                ></asp:Label>
              </EmptyDataTemplate>
            </cb:SortedGridView>
          </td>
        </tr>
        <tr>
          <td>
            <br />
            <asp:Label
              ID="SavedMessage"
              runat="server"
              Text="Saved at {0:t}<br />"
              SkinID="GoodCondition"
              EnableViewState="false"
              Visible="false"
            ></asp:Label>
            <asp:HyperLink
              ID="BackLink"
              runat="server"
              Text="Back"
              SkinID="Button"
              NavigateUrl="TaxCodes.aspx"
            ></asp:HyperLink>
            <asp:LinkButton
              ID="SaveButton"
              runat="server"
              Text="Save"
              SkinID="Button"
              OnClick="SaveButton_Click"
            ></asp:LinkButton>
          </td>
        </tr>
      </table>
    </ContentTemplate>
  </ajax:UpdatePanel>
  <asp:ObjectDataSource
    ID="TaxRuleDs"
    runat="server"
    EnablePaging="True"
    OldValuesParameterFormatString="original_{0}"
    SelectCountMethod="CountForStore"
    SelectMethod="LoadForStore"
    SortParameterName="sortExpression"
    TypeName="MakerShop.Taxes.TaxRuleDataSource"
  >
  </asp:ObjectDataSource>
</asp:Content>
