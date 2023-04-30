<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"
CodeFile="ThemesAndLayouts.aspx.cs" Inherits="Admin_Website_ThemesAndLayouts"
Title="Themes and Display Pages" %> <%@ Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
  <ajax:UpdatePanel ID="PageAjax" runat="server">
    <ContentTemplate>
      <div class="pageHeader">
        <div class="caption">
          <h1>
            <asp:Localize
              ID="Caption"
              runat="server"
              Text="Themes and Display Pages"
            ></asp:Localize>
          </h1>
        </div>
      </div>
      <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
          <td>
            <asp:Label
              ID="ResponseMessage"
              runat="server"
              Text="Changes to the themes and display pages have been saved."
              Visible="false"
              SkinID="GoodCondition"
            ></asp:Label>
          </td>
        </tr>
        <tr>
          <td>
            <div class="section">
              <div class="header">
                <h2 class="defaulttheme">
                  <asp:Localize
                    ID="ThemeCaption"
                    runat="server"
                    Text="Default Themes"
                  ></asp:Localize>
                </h2>
              </div>
              <div class="content">
                <table width="100%">
                  <tr>
                    <td width="50%" valign="top">
                      <table class="inputForm" width="100%">
                        <tr>
                          <td colspan="2">
                            <asp:Localize
                              ID="StoreThemeHelpText"
                              runat="server"
                              Text="Themes are a collection of CSS, skin files, and images that help to control the look and feel of your store.  Select the theme to be used by default for your store pages.  You can override this setting for a specific page, category, or product."
                            ></asp:Localize>
                          </td>
                        </tr>
                        <tr>
                          <th class="rowHeader" valign="top" nowrap>
                            <cb:ToolTipLabel
                              ID="StoreThemeLabel"
                              runat="server"
                              Text="Store Theme:"
                              AssociatedControlID="StoreTheme"
                              ToolTip="Default theme used for store"
                            />
                          </th>
                          <td>
                            <asp:DropDownList ID="StoreTheme" runat="server">
                              <asp:ListItem
                                Text="Use default set in web.config"
                                Value=""
                              ></asp:ListItem>
                            </asp:DropDownList>
                          </td>
                        </tr>
                      </table>
                    </td>
                    <td width="50%" valign="top">
                      <table class="inputForm" width="100%">
                        <tr>
                          <td colspan="2">
                            <asp:Localize
                              ID="AdminThemeHelpText"
                              runat="server"
                              Text="You may also change the admin theme if you wish to customize the look and feel of the merchant administration."
                            ></asp:Localize>
                          </td>
                        </tr>
                        <tr>
                          <th class="rowHeader" valign="top" nowrap>
                            <cb:ToolTipLabel
                              ID="AdminThemeLabel"
                              runat="server"
                              Text="Admin Theme:"
                              AssociatedControlID="AdminTheme"
                              ToolTip="Default theme used for merchant admin"
                            />
                          </th>
                          <td>
                            <asp:DropDownList ID="AdminTheme" runat="server">
                              <asp:ListItem
                                Text="Use default set in web.config"
                                Value=""
                              ></asp:ListItem>
                            </asp:DropDownList>
                          </td>
                        </tr>
                      </table>
                    </td>
                  </tr>
                </table>
              </div>
            </div>
          </td>
        </tr>
        <tr>
          <td>
            <div class="section">
              <div class="header">
                <h2 class="displaypages">
                  <asp:Localize
                    ID="LayoutCaption"
                    runat="server"
                    Text="Default Display Pages"
                  ></asp:Localize>
                </h2>
              </div>
              <div class="content">
                <asp:Localize
                  ID="LayoutHelpText"
                  runat="server"
                  Text="For the categories, products, and webpages in your catalog, you can specify the script that should be used to generate the display page.  You can override this default setting for any item in your catalog."
                ></asp:Localize
                ><br /><br />
                <table class="inputForm">
                  <tr>
                    <th class="rowHeader" valign="top" nowrap>
                      <cb:ToolTipLabel
                        ID="CategoryDisplayPageLabel"
                        runat="server"
                        Text="Category Display Page:"
                        AssociatedControlID="CategoryDisplayPage"
                        ToolTip="Default display page used for categories"
                      />
                    </th>
                    <td>
                      <ajax:UpdatePanel
                        ID="CategoryDisplayPageAjax"
                        runat="server"
                        UpdateMode="conditional"
                      >
                        <ContentTemplate>
                          <asp:DropDownList
                            ID="CategoryDisplayPage"
                            runat="server"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="CategoryDisplayPage_SelectedIndexChanged"
                          >
                            <asp:ListItem
                              Text=""
                              Value=""
                            ></asp:ListItem> </asp:DropDownList
                          ><br /><br />
                          <asp:Label
                            ID="CategoryDisplayPageDescription"
                            runat="Server"
                            Text="Select a display page to see a description."
                          ></asp:Label
                          ><br /><br />
                        </ContentTemplate>
                      </ajax:UpdatePanel>
                    </td>
                  </tr>
                  <tr>
                    <th class="rowHeader" valign="top" nowrap>
                      <cb:ToolTipLabel
                        ID="ProductDisplayPageLabel"
                        runat="server"
                        Text="Product Display Page:"
                        AssociatedControlID="ProductDisplayPage"
                        ToolTip="Default display page used for products"
                      />
                    </th>
                    <td>
                      <ajax:UpdatePanel
                        ID="ProductDisplayPageAjax"
                        runat="server"
                        UpdateMode="conditional"
                      >
                        <ContentTemplate>
                          <asp:DropDownList
                            ID="ProductDisplayPage"
                            runat="server"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="ProductDisplayPage_SelectedIndexChanged"
                          >
                            <asp:ListItem
                              Text=""
                              Value=""
                            ></asp:ListItem> </asp:DropDownList
                          ><br /><br />
                          <asp:Label
                            ID="ProductDisplayPageDescription"
                            runat="Server"
                            Text="Select a display page to see a description."
                          ></asp:Label
                          ><br /><br />
                        </ContentTemplate>
                      </ajax:UpdatePanel>
                    </td>
                  </tr>
                  <tr>
                    <th class="rowHeader" valign="top" nowrap>
                      <cb:ToolTipLabel
                        ID="WebpageDisplayPageLabel"
                        runat="server"
                        Text="Webpage Display Page:"
                        AssociatedControlID="WebpageDisplayPage"
                        ToolTip="Default display page used for webpages"
                      />
                    </th>
                    <td>
                      <ajax:UpdatePanel
                        ID="WebpageDisplayPageAjax"
                        runat="server"
                        UpdateMode="conditional"
                      >
                        <ContentTemplate>
                          <asp:DropDownList
                            ID="WebpageDisplayPage"
                            runat="server"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="WebpageDisplayPage_SelectedIndexChanged"
                          >
                            <asp:ListItem
                              Text=""
                              Value=""
                            ></asp:ListItem> </asp:DropDownList
                          ><br /><br />
                          <asp:Label
                            ID="WebpageDisplayPageDescription"
                            runat="Server"
                            Text="Select a display page to see a description."
                          ></asp:Label
                          ><br /><br />
                        </ContentTemplate>
                      </ajax:UpdatePanel>
                    </td>
                    <td colspan="2">&nbsp;</td>
                  </tr>
                </table>
              </div>
            </div>
          </td>
        </tr>
        <tr>
          <td align="left">
            <asp:Button
              ID="SaveButton"
              runat="server"
              Text="Save Changes"
              OnClick="SaveButton_Click"
            />
          </td>
        </tr>
      </table>
    </ContentTemplate>
  </ajax:UpdatePanel>
</asp:Content>
