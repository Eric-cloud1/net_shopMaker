<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CategoryDropDownList.ascx.cs" Inherits="ConLib_CategoryDropDownList" EnableViewState="false" %>
<%--
<conlib>
<summary>Displays a drop down list of categories in your store.</summary>
<param name="HomeText" default="Home">The string to use for the top level item that directs to your home page.</param>
<param name="Prefix" default=" . . ">The string to place before a subdirectory, repeated for each category level.</param>
<param name="Levels" default="0">The maximum number of category levels to include in the list.  Set to 0 for all levels.</param>
<param name="GoButtonText" default="">The text for the button that when clicked, will display the selected category.  If you set this to an empty string, the button will be hidden.</param>
<param name="AutoPostBack" default="true">If true, the page will automatically redirect to the category when selected.  If false, the button must be clicked to navigate to the selected category.</param>
<param name="CacheDuration" default="1440">Number of minutes the category list will remain cached in memory.  Set to 0 to disable caching.</param>
</conlib>
--%>
<ajax:UpdatePanel ID="CategoryUpdatePanel" runat="server">
    <ContentTemplate>
        <asp:DropDownList ID="CategoryList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="CategoryList_Changed">
        </asp:DropDownList>
        <asp:LinkButton ID="GoButton" runat="server" SkinID="Button" Text="Go" OnClick="GoButton_Click"></asp:LinkButton>
    </ContentTemplate>
</ajax:UpdatePanel>
