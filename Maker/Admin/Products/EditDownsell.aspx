<%@ Page Language="C#" MasterPageFile="~/Admin/Products/Product.master"
CodeFile="EditDownsell.aspx.cs" Inherits="Admin_Products_EditDownsell"
Title="Edit Downsell" %> <%@ Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">
  <div class="pageHeader">
    <div class="caption">
      <h1>
        <asp:Localize
          ID="Caption"
          runat="server"
          Text="Downsell for '{0}'"
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
            Text="This product does not have any Downsell plan."
            EnableViewState="False"
          ></asp:Localize>
        </p>
        <asp:Button
          ID="ShowAddForm"
          runat="server"
          Text="Add Downsell(s)"
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
                Text="Downsell saved at {0}."
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
            <th>
              <cb:ToolTipLabel
                ID="ToolTipLabel1"
                runat="server"
                Text="Payment"
                ToolTip="Which payment in the cycle (Trial,Recurring)."
                EnableViewState="false"
              ></cb:ToolTipLabel>
            </th>
            <th>
              <cb:ToolTipLabel
                ID="ToolTipLabel2"
                runat="server"
                Text="Available"
                ToolTip="The amounts available for this cycle."
                EnableViewState="false"
              ></cb:ToolTipLabel>
            </th>
            <th>
              <cb:ToolTipLabel
                ID="ToolTipLabel3"
                runat="server"
                Text="Add"
                ToolTip="Amount to add to this cycle"
                EnableViewState="false"
              ></cb:ToolTipLabel>
            </th>
          </tr>

          <tr>
            <td style="text-align: right">Trial</td>
            <td>
              <asp:DropDownList ID="ddltAvailable" runat="server" />
              <asp:Button
                ID="btRemove"
                runat="server"
                Text="Remove"
                onclick="btRemove_Click"
              />
            </td>
            <td>
              <asp:Textbox runat="server" ID="tbtAdd" />
              <asp:Button
                ID="btAdd"
                runat="server"
                Text="Add"
                onclick="btAdd_Click"
              />
            </td>
          </tr>
          <tr>
            <td style="text-align: right">Recurring</td>
            <td>
              <asp:DropDownList ID="ddlrAvailable" runat="server" />
              <asp:Button
                ID="brRemove"
                runat="server"
                Text="Remove"
                onclick="brRemove_Click"
              />
            </td>
            <td>
              <asp:Textbox runat="server" ID="tbrAdd" />
              <asp:Button
                ID="brAdd"
                runat="server"
                Text="Add"
                onclick="brAdd_Click"
              />
            </td>
          </tr>
        </table>
      </asp:Panel>
    </ContentTemplate>
  </ajax:UpdatePanel>
</asp:Content>
