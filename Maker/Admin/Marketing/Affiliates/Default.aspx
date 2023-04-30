<%@ Page Language="C#" MasterPageFile="Affiliate.master" AutoEventWireup="true"
CodeFile="Default.aspx.cs" Inherits="Admin_Marketing_Affiliates_Default"
Title="Manage Affiliates" %> <%@ Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %> <%@ Register
Namespace="Westwind.Web.Controls" assembly="wwhoverpanel" TagPrefix="wwh" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
  <script type="text/javascript">
    function ShowHoverPanel(event) {
      TrackerHelpHover.startCallback(event, "", null, OnError);
    }
    function HideHoverPanel() {
      TrackerHelpHover.hide();
    }
    function OnError(Result) {
      alert("*** Error:\r\n\r\n" + Result.message);
    }
  </script>
  <div class="pageHeader">
    <div class="caption">
      <h1>
        <asp:Localize
          ID="Caption"
          runat="server"
          Text="Affiliates"
        ></asp:Localize>
      </h1>
    </div>
  </div>

  <table cellpadding="0" cellspacing="0" class="innerLayout">
    <tr>
      <td width="50%" valign="top">
        <div class="section" style="width: 480px">
          <div class="header">
            <h2 class="addaffiliate">
              <asp:Localize
                ID="AddCaption"
                runat="server"
                Text="Add Affiliate"
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
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
                <table class="inputForm">
                  <tr>
                    <th class="rowHeader">
                      <asp:Label
                        ID="AddAffiliateNameLabel"
                        runat="server"
                        Text="Name: "
                        SkinID="FieldHeader"
                      ></asp:Label>
                    </th>
                    <td>
                      <asp:TextBox
                        ID="AddAffiliateName"
                        runat="server"
                        Width="150px"
                        MaxLength="100"
                      ></asp:TextBox>
                      <asp:RegularExpressionValidator
                        ID="AddAffiliateNameNameValidator"
                        runat="server"
                        ErrorMessage="Maximum length for Affiliate Name is 100 characters."
                        Text="*"
                        ControlToValidate="AddAffiliateName"
                        ValidationExpression=".{0,100}"
                      ></asp:RegularExpressionValidator>
                      <asp:RequiredFieldValidator
                        ID="AddAffiliateNameRequired"
                        runat="server"
                        ControlToValidate="AddAffiliateName"
                        Display="Static"
                        ErrorMessage="Affiliate name is required."
                        Text="*"
                      ></asp:RequiredFieldValidator>
                    </td>
                    <td>
                      <asp:Button
                        ID="AddAffiliateButton"
                        runat="server"
                        Text="Add"
                        OnClick="AddAffiliateButton_Click"
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

    <tr>
      <td width="50%" align="left" valign="top">
        <ajax:UpdatePanel ID="GridAjax" runat="server" UpdateMode="Conditional">
          <ContentTemplate>
            <cb:SortedGridView
              ID="AffiliateGrid"
              runat="server"
              AllowPaging="true"
              AllowSorting="true"
              PageSize="20"
              AutoGenerateColumns="False"
              DataKeyNames="AffiliateId"
              DataSourceID="AffiliateDs"
              OnRowDataBound="AffiliateGrid_RowDataBound"
              ShowFooter="False"
              DefaultSortExpression="Name"
              SkinID="PagedList"
              Width="400"
            >
              <Columns>
                <asp:TemplateField HeaderText="Logo">
                  <HeaderStyle HorizontalAlign="Center" />
                  <ItemStyle HorizontalAlign="Center" />
                  <ItemTemplate>
                    <asp:PlaceHolder
                      ID="phLogoCaption"
                      runat="server"
                    ></asp:PlaceHolder>
                  </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField
                  HeaderText="afid"
                  SortExpression="AffiliateId"
                >
                  <HeaderStyle HorizontalAlign="Center" />
                  <ItemStyle HorizontalAlign="Center" />
                  <ItemTemplate>
                    <asp:Label
                      ID="AffiliateIdLabel"
                      runat="server"
                      Text='<%# Eval("AffiliateId") %>'
                    ></asp:Label>
                  </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField
                  HeaderText="Affiliate Type"
                  SortExpression="AffiliateType"
                >
                  <HeaderStyle HorizontalAlign="Center" />
                  <ItemStyle HorizontalAlign="Center" />
                  <ItemTemplate>
                    <asp:Label
                      ID="AffiliateTypeLabel"
                      runat="server"
                      Text='<%# Eval("AffiliateType") %>'
                    ></asp:Label>
                  </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField
                  HeaderText="Parent Affiliate"
                  SortExpression="ParentAffiliate"
                >
                  <HeaderStyle HorizontalAlign="Center" />
                  <ItemStyle HorizontalAlign="Center" />
                  <ItemTemplate>
                    <asp:Label
                      ID="ParentAffiliateLabel"
                      runat="server"
                      Text='<%# Eval("ParentAffiliate") %>'
                    ></asp:Label>
                  </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Name" SortExpression="Name">
                  <ItemTemplate>
                    <asp:HyperLink
                      ID="NameLink"
                      runat="server"
                      NavigateUrl='<%#Eval("AffiliateId", "EditAffiliate.aspx?AffiliateId={0}")%>'
                      Text='<%#Eval("Name")%>'
                      SkinId="Link"
                    ></asp:HyperLink>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Commission" Visible="false">
                  <ItemTemplate>
                    <asp:Label
                      ID="CommissionRate"
                      runat="server"
                      isible="false"
                      Text="<%# GetCommissionRate(Container.DataItem) %>"
                    ></asp:Label>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField
                  HeaderText="Orders"
                  ItemStyle-HorizontalAlign="Center"
                >
                  <HeaderStyle HorizontalAlign="Center" />
                  <ItemStyle HorizontalAlign="Center" />
                  <ItemTemplate>
                    <asp:HyperLink
                      ID="OrdersLink"
                      runat="server"
                      text="<%#GetOrderCount(Container.DataItem)%>"
                      NavigateUrl='<%#Eval("AffiliateId", "../../Reports/SalesByAffiliateDetail.aspx?AffiliateId={0}") %>'
                    ></asp:HyperLink>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="False">
                  <ItemTemplate>
                    <asp:HyperLink
                      ID="EditLink"
                      runat="server"
                      NavigateUrl='<%#Eval("AffiliateId", "EditAffiliate.aspx?AffiliateId={0}")%>'
                      ><asp:Image
                        ID="EditIcon"
                        SkinID="EditIcon"
                        runat="server"
                        AlternateText="Edit"
                    /></asp:HyperLink>
                    <asp:LinkButton
                      ID="DeleteButton"
                      runat="server"
                      CausesValidation="False"
                      CommandName="Delete"
                      OnClientClick='<%# Eval("Name", "return confirm(\"Are you sure you want to delete {0}?\");") %>'
                      ><asp:Image
                        ID="DeleteIcon"
                        runat="server"
                        SkinID="DeleteIcon"
                        AlternateText="Delete"
                    /></asp:LinkButton>
                  </ItemTemplate>
                  <ItemStyle Wrap="false" />
                </asp:TemplateField>
              </Columns>
              <EmptyDataTemplate>
                <asp:Label
                  ID="EmptyDataText"
                  runat="server"
                  Text="No Affiliates are defined for your store."
                ></asp:Label>
              </EmptyDataTemplate>
            </cb:SortedGridView>
          </ContentTemplate>
        </ajax:UpdatePanel>
      </td>
    </tr>
  </table>
  <div class="section" style="padding: 2px 0 0 0; visibility: hidden">
    <div class="header">
      <h2 class="thirdpartytracker">
        <asp:Localize
          ID="TrackerCaption"
          runat="server"
          Text="Third Party Tracker"
        />
      </h2>
    </div>
    <div class="content">
      <ajax:UpdatePanel
        ID="TrackerAjax"
        runat="server"
        UpdateMode="Conditional"
      >
        <ContentTemplate>
          <asp:Localize
            id="TrackerHelpText"
            runat="server"
            Text="If you are using a third party tracker such as AffiliateWiz, you can provide the tracking URL here."
          ></asp:Localize>
          <a
            onmouseover="ShowHoverPanel(event)"
            class="link"
            onmouseout="HideHoverPanel();"
            href="#"
            >More Help</a
          ><br /><br />
          <asp:Label
            id="TrackerUrlSaved"
            runat="server"
            Text="URL Updated.<br />"
            SkinID="GoodCondition"
            EnableViewState="false"
            Visible="false"
          ></asp:Label>
          <asp:TextBox
            id="TrackerUrl"
            runat="server"
            Width="250px"
            MaxLength="200"
          ></asp:TextBox>
          <asp:Button
            id="TrackerSaveButton"
            onclick="TrackerSaveButton_Click"
            runat="server"
            Text="Save"
            CausesValidation="false"
          ></asp:Button>
        </ContentTemplate>
      </ajax:UpdatePanel>
    </div>
  </div>
  <asp:ObjectDataSource
    ID="AffiliateDs"
    runat="server"
    OldValuesParameterFormatString="original_{0}"
    SelectMethod="LoadForStore"
    TypeName="MakerShop.Marketing.AffiliateDataSource"
    SelectCountMethod="CountForStore"
    SortParameterName="sortExpression"
    DataObjectTypeName="MakerShop.Marketing.Affiliate"
    DeleteMethod="Delete"
    UpdateMethod="Update"
  >
  </asp:ObjectDataSource>
  <wwh:wwHoverPanel
    ID="TrackerHelpHover"
    runat="server"
    serverurl="~/Admin/Marketing/Affiliates/TrackerHelp.htm"
    Navigatedelay="250"
    scriptlocation="WebResource"
    style="display: none; background: white"
    panelopacity="0.89"
    shadowoffset="8"
    shadowopacity="0.18"
    PostBackMode="None"
    AdjustWindowPosition="true"
  >
  </wwh:wwHoverPanel>
</asp:Content>
