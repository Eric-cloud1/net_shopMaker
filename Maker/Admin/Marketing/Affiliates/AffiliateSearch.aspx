<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.master"
AutoEventWireup="true" CodeFile="AffiliateSearch.aspx.cs"
Inherits="Admin_Marketing_Affiliates_AffiliateSearch" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb"
%> <%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI"
TagPrefix="ComponentArt" %> <%@ Register assembly="wwhoverpanel"
Namespace="Westwind.Web.Controls" TagPrefix="wwh" %> <%@ Register
Src="~/Admin/UserControls/PickerAndCalendar.ascx" TagName="PickerAndCalendar"
TagPrefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
  <script type="text/javascript">
    function ShowHoverPanel(event, Id) {
      OrderHoverLookupPanel.startCallback(
        event,
        "OrderId=" + Id.toString(),
        null,
        OnError
      );
    }
    function HideHoverPanel() {
      OrderHoverLookupPanel.hide();
    }
    function OnError(Result) {
      alert("*** Error:\r\n\r\n" + Result.message);
    }

    function toggleCheckBoxState(id, checkState) {
      var cb = document.getElementById(id);
      if (cb != null) cb.checked = checkState;
    }

    function toggleSelected(checkState) {
      // Toggles through all of the checkboxes defined in the CheckBoxIDs array
      // and updates their value to the checkState input parameter
      if (CheckBoxIDs != null) {
        for (var i = 0; i < CheckBoxIDs.length; i++)
          toggleCheckBoxState(CheckBoxIDs[i], checkState.checked);
      }
    }
  </script>
  <div class="pageHeader">
    <div class="caption">
      <h1>
        <asp:Localize
          ID="Caption"
          runat="server"
          Text="Affiliate Manager"
        ></asp:Localize>
      </h1>
    </div>
  </div>
  <ajax:UpdatePanel ID="SearchFormAjax" runat="server">
    <ContentTemplate>
      <div class="searchPanel">
        <table cellpadding="4" cellspacing="0" class="inputForm">
          <tr>
            <td colspan="6" valign="top">
              <asp:Panel ID="HeaderPanel" runat="server">
                <table>
                  <tr>
                    <td>
                      <asp:Localize
                        ID="SearchHelpText"
                        runat="server"
                        Text="Set the criteria below, then click <b>Search</b> to filter the order listing."
                      ></asp:Localize>
                    </td>
                  </tr>
                </table>
              </asp:Panel>
            </td>
          </tr>
        </table>
      </div>
      <asp:panel ID="pnSearch" BorderWidth="0" Width="80%" runat="server">
        <table cellpadding="4" cellspacing="0" class="inputForm">
          <tr>
            <th class="rowHeader" valign="top">
              <cb:ToolTipLabel
                ID="AffiliateIdLabel"
                runat="server"
                Text="Find Affiliate by Id:"
                AssociatedControlID="AffiliateIdDropDown"
                ToolTip="Select a affiliate id to find."
              ></cb:ToolTipLabel>
            </th>
            <td colspan="3" valign="top">
              <asp:DropDownList ID="AffiliateIdDropDown" runat="server" />
            </td>
          </tr>

          <tr>
            <th class="rowHeader" valign="top">
              <cb:ToolTipLabel
                ID="AffiliateNameLabel"
                runat="server"
                Text="Affiliate by Name:"
                AssociatedControlID="AffiliateName"
                ToolTip="Enter a affiliate name to find.  Use of wildcards * and ? is allowed."
              ></cb:ToolTipLabel>
            </th>
            <td colspan="3" valign="top">
              <asp:TextBox ID="AffiliateName" runat="server" />
            </td>
            <th class="rowHeader" valign="top">
              <cb:ToolTipLabel
                ID="PageSizeLabel"
                runat="server"
                Text="Page Size:"
                AssociatedControlID="PageSize"
                ToolTip="Indicates the number of records to display per page."
              />
            </th>
            <td valign="top">
              <asp:DropDownList ID="PageSize" runat="server">
                <asp:ListItem Text="10 per page" Value="10"></asp:ListItem>
                <asp:ListItem
                  Text="20 per page"
                  Value="20"
                  Selected="true"
                ></asp:ListItem>
                <asp:ListItem Text="50 per page" Value="50"></asp:ListItem>
                <asp:ListItem Text="show all" Value=""></asp:ListItem>
              </asp:DropDownList>
            </td>
          </tr>
          <tr>
            <td>&nbsp;</td>
            <td colspan="5">
              <asp:Button
                ID="SearchButton"
                runat="server"
                Text="Search"
                OnClick="SearchButton_Click"
                CausesValidation="false"
              />
            </td>
          </tr>
        </table>
      </asp:panel>
    </ContentTemplate>
  </ajax:UpdatePanel>
  <ajax:UpdatePanel
    ID="SearchResultAjax"
    runat="server"
    UpdateMode="Conditional"
  >
    <ContentTemplate>
      <cb:SortedGridView
        ID="AffiliateGrid"
        runat="server"
        SkinID="PagedList"
        Width="100%"
        AllowPaging="True"
        PageSize="20"
        AutoGenerateColumns="False"
        DataKeyNames="AffiliateId"
        DataSourceID="AffiliateDs"
        AllowSorting="False"
        DefaultSortExpression="AffiliateId"
        DefaultSortDirection="Descending"
        OnRowDataBound="AffiliateGrid_RowDataBound"
        ShowWhenEmpty="False"
        OnPageIndexChanging="AffiliateGrid_PageIndexChanging"
      >
        <Columns>
          <asp:TemplateField>
            <HeaderStyle HorizontalAlign="Center" />
            <ItemStyle HorizontalAlign="Center" />
            <ItemTemplate>
              <asp:PlaceHolder
                ID="phLogoCaption"
                runat="server"
              ></asp:PlaceHolder>
            </ItemTemplate>
          </asp:TemplateField>

          <asp:TemplateField HeaderText="afid" SortExpression="AffiliateId">
            <HeaderStyle HorizontalAlign="Center" />
            <ItemStyle HorizontalAlign="Center" />
            <ItemTemplate>
              <asp:HyperLink
                ID="AffiliateId"
                runat="server"
                Text='<%# Eval("AffiliateId")  %>'
                SkinID="Link"
                NavigateUrl='<%#String.Format("AffiliatePayment.aspx?AffiliateId={0}", Eval("AffiliateId"))%>'
              ></asp:HyperLink>
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
            ID="EmptyMessage"
            runat="server"
            Text="No orders match criteria."
          ></asp:Label>
        </EmptyDataTemplate>
      </cb:SortedGridView>
    </ContentTemplate>
  </ajax:UpdatePanel>

  <asp:ObjectDataSource ID="AffiliateDs" runat="server" />

  <asp:ObjectDataSource
    ID="AffiliateSearchDs"
    runat="server"
    OldValuesParameterFormatString="original_{0}"
    SelectMethod="LoadForCriteria"
    TypeName="MakerShop.Marketing.AffiliateDataSource"
    SelectCountMethod="CountForStore"
    SortParameterName=""
    DataObjectTypeName="MakerShop.Marketing.Affiliate"
    DeleteMethod="Delete"
    UpdateMethod="Update"
  >
  </asp:ObjectDataSource>
</asp:Content>
