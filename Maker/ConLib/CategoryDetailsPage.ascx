<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CategoryDetailsPage.ascx.cs" Inherits="ConLib_CategoryDetailsPage"  %>
<%--
<conlib>
<summary>A category page that displays all contents of a category with summary description in a row format.  This page displays products, webpages, and links.</summary>
<param name="DefaultCaption" default="Catalog">Caption text that will be shown as caption when root category will be browsed.</param>
<param name="DefaultCategorySummary" default="Welcome to our store.">Summary that will be shown when root category will be browsed.</param>
</conlib>
--%>

<%@ Register Src="CategoryBreadCrumbs.ascx" TagName="CategoryBreadCrumbs" TagPrefix="uc" %>
<%@ Register Src="CategorySearchSidebar.ascx" TagName="CategorySearchSidebar" TagPrefix="uc" %>
<ajax:UpdatePanel ID="ResultsAjaxPanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:PlaceHolder ID="CategoryHeaderPanel" runat="server" EnableViewState="false">
            <uc:CategoryBreadCrumbs id="CategoryBreadCrumbs1" runat="server" HideLastNode="True" />
            <div class="pageHeader">
                <h1><asp:Literal ID="Caption" runat="server" EnableViewState="False"></asp:Literal></h1>
            </div>
            <br />
            <div >
                <asp:Literal ID="CategoryDescription" runat="server" EnableViewState="false"></asp:Literal>
            </div>            
        </asp:PlaceHolder>
        <br />
        <asp:PlaceHolder ID="phCategoryContents" runat="server">            
            <!-- Top Bar -->
            <div class="catalogWrapper" style="padding:0px;">
                <asp:Repeater ID="CatalogNodeList" runat="server" OnItemDataBound="CatalogNodeList_ItemDataBound" EnableViewState="false">
                    <HeaderTemplate>
                        <table>
                    </HeaderTemplate>
                    <ItemTemplate>                       
                        <asp:PlaceHolder ID="phItemTemplate1" runat="server"></asp:PlaceHolder>                        
                    </ItemTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                    <SeparatorTemplate>
                        <tr><td colspan="2">&nbsp;</td></tr>
                    </SeparatorTemplate>
                </asp:Repeater>                
            </div>            
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="phEmptyCategory" runat="server" Visible="false" EnableViewState="false">
            <div align="center">
                <asp:Localize ID="EmptyCategoryMessage" runat="server" Text="The category is empty." EnableViewState="false"></asp:Localize>
            </div>
        </asp:PlaceHolder>
    </ContentTemplate>
</ajax:UpdatePanel>
<asp:Panel ID="RequiredParameterMissingPanel" runat="server" Visible="false" EnableViewState="false">
    <asp:Label ID="RequiredParameterMessage" runat="server" Text="{0}: Parameter(s) {1} is/are required." EnableViewState="false" SkinID="ErrorCondition"></asp:Label>
</asp:Panel>