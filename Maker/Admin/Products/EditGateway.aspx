<%@ Page Language="C#" MasterPageFile="~/Admin/Products/Product.master"
CodeFile="EditGateway.aspx.cs" Inherits="Admin_Products_EditGateway" Title="Edit
Gateway" %> <%@ Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">
  <div class="pageHeader">
    <div class="caption">
      <h1>
        <asp:Localize
          ID="Caption"
          runat="server"
          Text="Gateway for '{0}'"
        ></asp:Localize>
      </h1>
    </div>
  </div>
  <ajax:UpdatePanel
    ID="PageAjax"
    runat="server"
    UpdateMode="conditional"
    ChildrenAsTriggers="true"
  >
    <ContentTemplate>
      <asp:Panel ID="NoPlanPanel" runat="server">
        <p>
          <asp:Localize
            ID="NoItems"
            runat="server"
            Text="This product does not have a Gateway Template assigned."
            EnableViewState="False"
          ></asp:Localize>
        </p>
        <asp:Button
          ID="ShowAddForm"
          runat="server"
          Text="Add Gateway"
          OnClick="ShowAddForm_Click"
          EnableViewState="false"
        />
      </asp:Panel>
      <asp:Panel ID="PlanPanel" runat="server" Visible="false">
        <table cellpadding="5" cellspacing="0" class="inputForm">
          <tr>
            <td colspan="3">
              <asp:Label
                ID="SavedMessage"
                runat="server"
                Text="Gateway saved at {0}."
                EnableViewState="false"
                Visible="false"
                SkinID="GoodCondition"
              ></asp:Label>
              <asp:Label
                ID="ErrorMessageLabel"
                runat="server"
                Text=""
                EnableViewState="false"
                Visible="false"
                SkinID="ErrorCondition"
              ></asp:Label>
            </td>
          </tr>
          <tr>
            <th colspan="3">
              <cb:ToolTipLabel
                ID="ToolTipLabel1"
                runat="server"
                Text="Which Gateway Template"
                EnableViewState="false"
              ></cb:ToolTipLabel>
            </th>
          </tr>
          <tr>
            <td style="text-align: right">
              <asp:DropDownList ID="ddl" runat="server" />
            </td>
            <td>
              <asp:Button
                ID="btRemove"
                runat="server"
                Text="Save"
                OnClick="bSave_Click"
              />
            </td>
          </tr>
        </table>
      </asp:Panel>
    </ContentTemplate>
  </ajax:UpdatePanel>
</asp:Content>
