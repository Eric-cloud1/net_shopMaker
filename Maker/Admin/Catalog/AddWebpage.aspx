<%@ Page Language="C#" MasterPageFile="~/Admin/Catalog.master"
AutoEventWireup="true" CodeFile="AddWebpage.aspx.cs"
Inherits="Admin_Catalog_AddWebpage" Title="Add Webpage" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb"
%>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
  <div class="pageHeader">
    <div class="caption">
      <h1>
        <asp:Localize
          ID="Caption"
          runat="server"
          Text="Add Webpage to {0}"
        ></asp:Localize>
      </h1>
    </div>
  </div>
  <table cellpadding="2" cellspacing="0" class="innerLayout">
    <tr>
      <td class="validation">
        <asp:ValidationSummary ID="ErrorSummary" runat="server" />
      </td>
    </tr>
    <tr>
      <td>
        <table class="inputForm">
          <tr>
            <th class="rowHeader">
              <cb:ToolTipLabel
                ID="NameLabel"
                runat="server"
                Text="Page Title:"
                CssClass="toolTip"
                ToolTip="Name of the page as shown in the list of category contents; also used for the TITLE tag when viewed in a browser."
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
            <th class="rowHeader">
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
            <th class="rowHeader">
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
            <th class="rowHeader">
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
              ></asp:TextBox
              >&nbsp;
            </td>
          </tr>
          <tr>
            <th class="rowHeader" valign="top">
              <cb:ToolTipLabel
                ID="SummaryLabel"
                runat="server"
                Text="Summary:"
                AssociatedControlId="Summary"
                ToolTip="Brief description of the webpage that may appear on some category listing or directory pages."
              ></cb:ToolTipLabel>
            </th>
            <td colspan="3">
              <asp:TextBox
                ID="Summary"
                runat="server"
                Text=""
                TextMode="multiLine"
                Height="100px"
                MaxLength="1000"
                Width="90%"
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
            <th class="rowHeader" valign="top">
              <cb:ToolTipLabel
                ID="WebpageContentLabel"
                runat="server"
                Text="Page Content:"
                AssociatedControlId="WebpageContent"
                ToolTip="Provide the content for the webpage.  If providing HTML content, you should only provide the document content that would appear with the BODY tag.  Do you include tags such as HTML or HEAD."
              ></cb:ToolTipLabel
              ><br />
              <asp:ImageButton
                ID="WebpageContentHtml"
                runat="server"
                SkinID="EditIcon"
              />
            </th>
            <td colspan="3">
              <asp:TextBox
                ID="WebpageContent"
                runat="server"
                Width="90%"
                Height="200px"
                TextMode="multiLine"
              ></asp:TextBox>
            </td>
          </tr>
          <tr>
            <th class="rowHeader" valign="top" nowrap>
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
                Width="100%"
              ></asp:TextBox
              ><br />
            </td>
          </tr>
          <tr>
            <th class="rowHeader" valign="top">
              <cb:ToolTipLabel
                ID="DisplayPageLabel"
                runat="server"
                Text="Display Page:"
                AssociatedControlId="DisplayPage"
                ToolTip="Specifies the script that will handle the display of this item."
              ></cb:ToolTipLabel>
            </th>
            <td colspan="3">
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
                  >&nbsp;&nbsp;<asp:Label
                    ID="DisplayPageDescription"
                    runat="Server"
                    Text=""
                  ></asp:Label>
                </ContentTemplate>
              </ajax:UpdatePanel>
            </td>
          </tr>
          <tr>
            <th class="rowHeader">
              <cb:ToolTipLabel
                ID="LocalThemeLabel"
                runat="server"
                Text="Theme:"
                AssociatedControlId="LocalTheme"
                ToolTip="Specifies the theme that will be used to display this item."
              ></cb:ToolTipLabel>
            </th>
            <td colspan="3">
              <asp:DropDownList
                ID="LocalTheme"
                runat="server"
                AppendDataBoundItems="true"
                DataTextField="DisplayName"
                DataValueField="Name"
              >
                <asp:ListItem Text="Use store default" Value=""></asp:ListItem>
              </asp:DropDownList>
            </td>
          </tr>
          <tr>
            <td>&nbsp;</td>
            <td colspan="3">
              <asp:Button
                ID="FinishButton"
                runat="server"
                Text="Finish"
                OnClick="FinishButton_Click"
              />
              <asp:HyperLink
                ID="CancelButton"
                runat="server"
                Text="Cancel"
                SkinID="Button"
                NavigateUrl="Browse.aspx"
              />
            </td>
          </tr>
        </table>
      </td>
    </tr>
  </table>
</asp:Content>
