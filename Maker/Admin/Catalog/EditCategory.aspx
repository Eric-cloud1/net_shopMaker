<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"
AutoEventWireup="true" CodeFile="EditCategory.aspx.cs"
Inherits="Admin_Catalog_EditCategory" Title="Edit Category" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb"
%>
<asp:Content ID="Content" ContentPlaceHolderID="MainContent" Runat="Server">
  <div id="content">
    <div class="pageHeader">
      <div class="caption">
        <h1>
          <asp:Localize
            ID="Caption"
            runat="server"
            Text="Edit Category '{0}'"
          ></asp:Localize>
        </h1>
      </div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
      <tr>
        <td>
          <table width="95%" class="inputForm">
            <tr>
              <th width="15%" class="rowHeader">
                <cb:ToolTipLabel
                  ID="NameLabel"
                  runat="server"
                  Text="Name:"
                  CssClass="toolTip"
                  ToolTip="Name of the category."
                ></cb:ToolTipLabel>
              </th>
              <td>
                <asp:TextBox
                  ID="Name"
                  runat="server"
                  Text=""
                  Columns="40"
                  MaxLength="100"
                ></asp:TextBox>
                <asp:RequiredFieldValidator
                  ID="NameValidator"
                  runat="server"
                  Text="*"
                  Display="Dynamic"
                  ErrorMessage="Name is required."
                  ControlToValidate="Name"
                ></asp:RequiredFieldValidator>
              </td>
            </tr>
            <tr>
              <th width="15%" class="rowHeader">
                <cb:ToolTipLabel
                  ID="VisibilityLabel"
                  runat="server"
                  Text="Visibility:"
                  AssociatedControlID="Visibility"
                  ToolTip="Public items are accessible and display in navigation and search results.  Hidden items are accessible only by direct link, and do not appear in navigation or search results.  Private items may not be accessed from the retail store."
                ></cb:ToolTipLabel>
              </th>
              <td>
                <asp:DropDownList ID="Visibility" runat="server">
                  <asp:ListItem Value="0" Text="Public"></asp:ListItem>
                  <asp:ListItem Value="1" Text="Hidden"></asp:ListItem>
                  <asp:ListItem Value="2" Text="Private"></asp:ListItem>
                </asp:DropDownList>
              </td>
            </tr>
            <tr>
              <th width="15%" class="rowHeader">
                <cb:ToolTipLabel
                  ID="ThumbnailUrlLabel"
                  runat="server"
                  Text="Thumbnail:"
                  AssociatedControlId="ThumbnailUrl"
                  ToolTip="Specifies the thumbnail image that may be used with this item on some display pages."
                ></cb:ToolTipLabel>
              </th>
              <td>
                <asp:TextBox
                  ID="ThumbnailUrl"
                  runat="server"
                  MaxLength="250"
                  width="250px"
                ></asp:TextBox
                >&nbsp;
                <asp:ImageButton
                  ID="BrowseThumbnailUrl"
                  runat="server"
                  SkinID="FindIcon"
                  AlternateText="Browse"
                />
              </td>
            </tr>
            <tr>
              <th width="15%" class="rowHeader">
                <cb:ToolTipLabel
                  ID="ThumbnailAltTextLabel"
                  runat="server"
                  Text="Alt Text:"
                  AssociatedControlId="ThumbnailAltText"
                  ToolTip="Specifies the alternate text that should be set on the thumbnail image.  Leave blank to use the item name."
                ></cb:ToolTipLabel>
              </th>
              <td>
                <asp:TextBox
                  ID="ThumbnailAltText"
                  runat="server"
                  MaxLength="250"
                  width="250px"
                ></asp:TextBox
                >&nbsp;
              </td>
            </tr>
            <tr>
              <th width="15%" class="rowHeader" style="vertical-align: top">
                <cb:ToolTipLabel
                  ID="SummaryLabel"
                  runat="server"
                  Text="Summary:"
                  ToolTip="A summary or introduction to the category, used by some category display pages to provide a description of the category to the customer."
                ></cb:ToolTipLabel>
              </th>
              <td colspan="3">
                <asp:TextBox
                  ID="Summary"
                  runat="server"
                  Text=""
                  TextMode="multiLine"
                  Rows="5"
                  MaxLength="1000"
                  Columns="140"
                ></asp:TextBox
                ><br />
                <asp:Label
                  ID="SummaryCharCount"
                  runat="server"
                  Text="1000"
                ></asp:Label>
                <asp:Label
                  ID="SummaryCharCountLabel"
                  runat="server"
                  Text="characters remaining"
                ></asp:Label>
              </td>
            </tr>
            <tr>
              <th width="15%" class="rowHeader" style="vertical-align: top">
                <cb:ToolTipLabel
                  ID="DescriptionLabel"
                  runat="server"
                  Text="Description:"
                  ToolTip="This is used by some display pages to show a detailed description or HTML content on the category page."
                ></cb:ToolTipLabel
                ><br />
                <asp:ImageButton
                  ID="CategoryDescriptionHtml"
                  runat="server"
                  SkinID="EditIcon"
                />
              </th>
              <td colspan="3">
                <asp:TextBox
                  ID="Description"
                  runat="server"
                  Text=""
                  TextMode="multiLine"
                  Rows="5"
                  Columns="140"
                ></asp:TextBox>
              </td>
            </tr>
            <tr>
              <th width="15%" class="rowHeader" valign="top" nowrap>
                <cb:ToolTipLabel
                  ID="HtmlHeadLabel"
                  runat="server"
                  Text="HTML HEAD:"
                  ToolTip="Enter the data to include in the HTML HEAD portion of the display page, such as meta keywords and description."
                ></cb:ToolTipLabel>
              </th>
              <td colspan="3">
                <asp:TextBox
                  ID="HtmlHead"
                  runat="server"
                  Text=""
                  TextMode="multiLine"
                  Rows="5"
                  Columns="140"
                ></asp:TextBox
                ><br />
              </td>
            </tr>
            <tr>
              <th width="15%" class="rowHeader" valign="top">
                <cb:ToolTipLabel
                  ID="DisplayPageLabel"
                  runat="server"
                  Text="Display Page:"
                  AssociatedControlId="DisplayPage"
                  ToolTip="Specifies the script that will handle the display of this item."
                ></cb:ToolTipLabel>
              </th>
              <td>
                <ajax:UpdatePanel
                  ID="DisplayPageAjax"
                  runat="server"
                  UpdateMode="always"
                >
                  <ContentTemplate>
                    <asp:DropDownList
                      ID="DisplayPage"
                      runat="server"
                      AutoPostBack="true"
                      OnSelectedIndexChanged="DisplayPage_SelectedIndexChanged"
                    >
                      <asp:ListItem
                        Text="Use store default"
                        Value=""
                      ></asp:ListItem> </asp:DropDownList
                    ><br />
                    <asp:Label
                      ID="DisplayPageDescription"
                      runat="Server"
                      Text=""
                    ></asp:Label
                    ><br /><br />
                  </ContentTemplate>
                </ajax:UpdatePanel>
              </td>
            </tr>
            <tr>
              <th width="15%" class="rowHeader" valign="top">
                <cb:ToolTipLabel
                  ID="LocalThemeLabel"
                  runat="server"
                  Text="Theme:"
                  AssociatedControlId="LocalTheme"
                  ToolTip="Specifies the theme that will be used to display this item."
                ></cb:ToolTipLabel>
              </th>
              <td valign="top">
                <asp:DropDownList
                  ID="LocalTheme"
                  runat="server"
                  AppendDataBoundItems="true"
                  DataTextField="DisplayName"
                  DataValueField="Name"
                >
                  <asp:ListItem
                    Text="Use store default"
                    Value=""
                  ></asp:ListItem>
                </asp:DropDownList>
              </td>
            </tr>
            <tr>
              <td class="validation" colspan="4">
                <asp:ValidationSummary ID="ErrorSummary" runat="server" />
              </td>
            </tr>
            <tr>
              <td>&nbsp;</td>
              <td class="submit" colspan="3">
                <asp:Button
                  ID="SaveButton"
                  runat="server"
                  Text="Save"
                  OnClick="SaveButton_Click"
                />
                <asp:Button
                  ID="FinishButton"
                  runat="server"
                  Text="Save and Close"
                  OnClick="FinishButton_Click"
                />
                <asp:Button
                  ID="DeleteButton"
                  runat="server"
                  Text="Delete"
                  OnClick="DeleteButton_Click"
                />
                <asp:Button
                  ID="CancelButton"
                  runat="server"
                  Text="Close"
                  CausesValidation="False"
                  OnClick="CancelButton_Click"
                />
              </td>
            </tr>
          </table>
        </td>
      </tr>
    </table>
  </div>
</asp:Content>
