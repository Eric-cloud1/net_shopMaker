<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AdminPanel.ascx.cs" Inherits="Layouts_AdminPanel" %>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls.WebParts" TagPrefix="cb" %>
<%@ Register Src="~/Layouts/EditScriptlet.ascx" TagName="EditScriptlet" TagPrefix="uc" %>
<div id="webpartsPanel" class="noPrint">
<table align="center" cellspacing="0" cellpadding="2" class="outerFrame">
    <tr>
        <th align="left" style="padding:5px;" nowrap>
            <div style="text-align:left">
                <asp:Label ID="CurrentModeLabel" runat="server" Text="Mode:" SkinID="FieldHeader" AssociatedControlID="CurrentMode" EnableViewState="false"></asp:Label>
                <asp:DropDownList ID="CurrentMode" runat="server" AutoPostBack="true">
                    <asp:ListItem Text="View Page" Value="Browse"></asp:ListItem>
                    <asp:ListItem Text="Edit Page" Value="Edit"></asp:ListItem>
                </asp:DropDownList>
            </div>
        </th>
        <th align="right" style="padding:5px;" nowrap>
            <asp:Label ID="PagePath" runat="server" EnableViewState="false"></asp:Label>
        </th>
    </tr>
    <tr><td colspan="2"><hr></td></tr>
    <tr id="trBrowsePanel" runat="server">
        <td colspan="2">
            <div style="text-align:justify">
                <asp:Localize ID="BrowsePanelHelpText" runat="server" Text="You are currently in browse mode; the page is shown as it will appear to regular users.  Use &quot;Edit Page&quot; to make changes to theme, layout, and content." EnableViewState="false"></asp:Localize>
                <asp:Panel ID="EditCategoryPanel" runat="server" Visible="false" EnableViewState="false">
                    <br /><br />
                    <asp:Label ID="EditCategoryLabel" runat="server" Text="Edit Category: " SkinID="FieldHeader" EnableViewState="false"></asp:Label>
                    <asp:HyperLink ID="EditCategoryLink" runat="server" NavigateUrl="~/Admin/Catalog/Browse.aspx" Text="" EnableViewState="false"></asp:HyperLink>
                </asp:Panel>
                <asp:Panel ID="EditProductPanel" runat="server" Visible="false" EnableViewState="false">
                    <asp:Label ID="EditProductLabel" runat="server" Text="Edit Product: " SkinID="FieldHeader" EnableViewState="false"></asp:Label>
                    <asp:HyperLink ID="EditProductLink" runat="server" NavigateUrl="~/Admin/Products/EditProduct.aspx" Text="" EnableViewState="false"></asp:HyperLink>
                </asp:Panel>
                <asp:Panel ID="EditWebpagePanel" runat="server" Visible="false" EnableViewState="false">
                    <asp:Label ID="EditWebpageLabel" runat="server" Text="Edit Webpage: " SkinID="FieldHeader" EnableViewState="false"></asp:Label>
                    <asp:HyperLink ID="EditWebpageLink" runat="server" NavigateUrl="~/Admin/Catalog/EditWebpage.aspx" Text="" EnableViewState="false"></asp:HyperLink>
                </asp:Panel>
                <asp:Panel ID="EditLinkPanel" runat="server" Visible="false" EnableViewState="false">
                    <asp:Label ID="EditLinkLabel" runat="server" Text="Edit Link: " SkinID="FieldHeader" EnableViewState="false"></asp:Label>
                    <asp:HyperLink ID="EditLinkLink" runat="server" NavigateUrl="~/Admin/Catalog/EditLink.aspx" Text="" EnableViewState="false"></asp:HyperLink>
                </asp:Panel>
                <asp:Panel ID="ResetPagePanel" runat="server" EnableViewState="false">
                    <br /><br />
                    <asp:Label ID="ResetPageHelpText" runat="server" EnableViewState="false">
                        This page has been customized for theme and layout.  You can click "Reset Page" below to remove all customizations made to this page.  It will return to the default theme and layout as defined by the script and your store defaults.
                    </asp:Label><br /><br />
                    <asp:Button ID="ResetPage" runat="server" Text="Reset Page" OnClick="ResetPage_Click" OnClientClick="return confirm('Are you sure you want to reset the page? This will remove any customizations made using this website editor.')" EnableViewState="false" />
                </asp:Panel>
            </div>
        </td>
    </tr>
    <tr id="trEditPanel" runat="server">
        <td colspan="2">
            <asp:PlaceHolder ID="phEditor" runat="server" EnableViewState="false">
                <div class="section">
                    <div class="header">
                        <h2>Themes and Display Pages</h2>
                    </div>
                    <table>
                        <tr id="trPageTheme" runat="server" EnableViewState="false">
                            <th class="rowHeader" align="right">
                                <asp:Localize ID="PageThemeLocalize" runat="server" Text="Theme:" EnableViewState="false"></asp:Localize>
                            </th>
                            <td>
                                <asp:DropDownList ID="PageTheme" runat="server">
                                    <asp:ListItem Text="Use Store Default" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                &nbsp;
                                <asp:Localize ID="PagePath2" runat="server" Text="(sets theme for {0} only)" EnableViewState="false"></asp:Localize>
                            </td>
                        </tr>
                        <tr id="trCatalogTheme" runat="server" EnableViewState="false">
                            <th class="rowHeader" align="right">
                                <asp:Localize ID="CatalogThemeLocalize" runat="server" Text="Theme:" EnableViewState="false"></asp:Localize>
                            </th>
                            <td>
                                <asp:DropDownList ID="CatalogTheme" runat="server">
                                    <asp:ListItem Text="Inherit" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                &nbsp;
                                <asp:Localize ID="CatalogName" runat="server" Text="(sets theme for {0})" EnableViewState="false"></asp:Localize>
                            </td>
                        </tr>
                        <tr id="trDisplayPage" runat="server" EnableViewState="false">
                            <th class="rowHeader" align="right">
                                <asp:Label ID="CurrentDisplayPageLabel" runat="server" Text="Display Page:" AssociatedControlID="CurrentDisplayPage" EnableViewState="false"></asp:Label>
                            </th>
                            <td>
                                <asp:DropDownList ID="CurrentDisplayPage" runat="server">
                                    <asp:ListItem Text="Use Store Default" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                &nbsp;
                                <asp:Localize ID="CurrentDisplayPageName" runat="server" Text="(sets value for this {0} only)" EnableViewState="false"></asp:Localize>
                            </td>
                        </tr>
                        <tr>
                            <td>&nbsp;</td>
                            <td>
                                <asp:Button ID="SaveThemeButton" runat="server" Text="Update Theme" OnClick="SaveThemeButton_Click" EnableViewState="false" />
                            </td>
                        </tr>
                    </table>
                </div>
                <asp:EditorZone ID="EditorZone1" runat="server" CssClass="EditorZone" EnableViewState="false"> 
                    <HeaderStyle CssClass="EditorZoneHeader" />
                    <LabelStyle Font-Size="0.8em" ForeColor="#333333" />
                    <HeaderVerbStyle Font-Bold="False" Font-Size="0.8em" Font-Underline="False" ForeColor="#333333" />
                    <PartChromeStyle BorderColor="#E2DED6" BorderStyle="Solid" BorderWidth="1px" />
                    <ZoneTemplate>
                        <asp:PropertyGridEditorPart ID="PropertyGridEditorPart1" runat="server" />
                    </ZoneTemplate>
                    <PartStyle BorderColor="#F7F6F3" BorderWidth="5px" />
                    <FooterStyle CssClass="EditorZoneFooter" />
                    <EditUIStyle Font-Names="Verdana" Font-Size="0.8em" ForeColor="#333333" />
                    <InstructionTextStyle Font-Size="0.8em" ForeColor="#333333" />
                    <ErrorStyle Font-Size="0.8em" />
                    <VerbStyle Font-Names="Verdana" Font-Size="0.8em" ForeColor="#333333" />
                    <EmptyZoneTextStyle Font-Size="0.8em" ForeColor="#333333" />
                    <PartTitleStyle Font-Bold="True" Font-Size="0.8em" ForeColor="#333333" />                      
                </asp:EditorZone>
            </asp:PlaceHolder>
            <uc:EditScriptlet ID="EditScriptlet" runat="server"></uc:EditScriptlet>
        </td>
    </tr>
</table>
</div>