<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CategoryGridPage.ascx.cs" Inherits="ConLib_CategoryGridPage" %>
<%--
<conlib>
<summary>The defualt category page that displays products in a grid format. Allows customers to browse your catalog.</summary>
<param name="DefaultCaption" default="Catalog">Caption text that will be shown as caption when root category will be browsed.</param>
<param name="DisplayBreadCrumbs" default="true">Indicates wheather the breadcrumbs should be displayed or not, default value is true.</param>
<param name="PagingLinksLocation" default="BOTTOM">Indicates where the paging links will be displayd, possible values are "TOP", "BOTTOM" and "TOPANDBOTTOM".</param>
</conlib>
--%>
<%@ Register Src="~/ConLib/CategoryBreadCrumbs.ascx" TagName="CategoryBreadCrumbs" TagPrefix="uc" %>
<%@ Register Src="~/ConLib/CategorySearchSidebar.ascx" TagName="CategorySearchSidebar" TagPrefix="uc" %>
<%@ Register Src="~/ConLib/Utility/ProductPrice.ascx" TagName="ProductPrice" TagPrefix="uc" %>
<%@ Register Src="~/ConLib/AddToCartLink.ascx" TagName="AddToCartLink" TagPrefix="uc" %>
<!-- ConLib/CategoryGridPage.ascx -->
<ajax:UpdatePanel ID="SearchResultsAjaxPanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:PlaceHolder ID="CategoryHeaderPanel" runat="server" EnableViewState="false">
            <uc:CategoryBreadCrumbs id="CategoryBreadCrumbs1" runat="server" HideLastNode="True" />
            <div class="pageHeader">
                <h1><asp:Localize ID="Caption" runat="server" EnableViewState="False"></asp:Localize></h1>
            </div>
            <asp:PlaceHolder ID="SubCategoryPanel" runat="server" EnableViewState="false">
                <div class="section">
                    <div class="content">
                        <asp:Repeater ID="SubCategoryRepeater" runat="server" EnableViewState="false">
                            <SeparatorTemplate>, </SeparatorTemplate>
                            <ItemTemplate><asp:HyperLink ID="SubCategoryLink" runat="server" Text='<%#String.Format("{0} ({1})", Eval("Name"), Eval("ProductCount"))%>' NavigateUrl='<%#Eval("NavigateUrl")%>'></asp:HyperLink></ItemTemplate>
                        </asp:Repeater><br /><br />
                    </div>
                </div>
            </asp:PlaceHolder>
        </asp:PlaceHolder>
        <div class="searchSortHeader">
            <table width="100%" cellpadding="3" cellspacing="0" border="0">
                <tr>
                    <td align="left">
                        <asp:Label ID="ResultIndexMessage" runat="server" Text="Displaying items {0} - {1} of {2}" EnableViewState="false"></asp:Label>
                    </td>
                    <td align="right">
                        <asp:Label ID="SortResultsLabel" runat="server" Text="Sort:" SkinID="FieldHeader"></asp:Label>&nbsp;
                        <asp:DropDownList ID="SortResults" runat="server" AutoPostBack="true" CssClass="sorting" OnSelectedIndexChanged="SortResults_SelectedIndexChanged">
                            <asp:ListItem Text="" Value="OrderBy ASC"></asp:ListItem>
                            <asp:ListItem Text="By Name (A -> Z)" Value="Name ASC"></asp:ListItem>
                            <asp:ListItem Text="By Name (Z -> A)" Value="Name DESC"></asp:ListItem>
                            <asp:ListItem Text="Featured" Value="IsFeatured DESC, Name ASC"></asp:ListItem>
                            <asp:ListItem Text="By Price (Low to High)" Value="Price ASC"></asp:ListItem>
                            <asp:ListItem Text="By Price (High to Low)" Value="Price DESC"></asp:ListItem>                            
                            <asp:ListItem Text="By Manufacturer (A -> Z)" Value="Manufacturer ASC"></asp:ListItem>
                            <asp:ListItem Text="By Manufacturer (Z -> A)" Value="Manufacturer DESC"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
        </div>
        <!-- Top Bar -->
        <div class="catalogWrapper">
            <asp:Panel ID="PagerPanelTop" runat="server">
	            <div class="paging">
	                <asp:Repeater ID="PagerControlsTop" runat="server" OnItemCommand="PagerControls_ItemCommand">
		            <ItemTemplate>
		                <a class='<%#Eval("TagClass")%>'  href='<%#Eval("NavigateUrl")%>'><%#Eval("Text")%></a>
		            </ItemTemplate>
	                </asp:Repeater>
	            </div>
            </asp:Panel> 
            <asp:DataList ID="ProductList" runat="server" RepeatColumns="3" RepeatDirection="Horizontal" Width="100%" 
                OnItemDataBound="ProductList_ItemDataBound" DataKeyField="ProductId" CssClass="catalog" EnableViewState="false" HorizontalAlign="left">
                <ItemStyle HorizontalAlign="center" VerticalAlign="bottom" Width="33%" CssClass="tableNode" />
                <ItemTemplate>
                    <asp:PlaceHolder ID="phItemTemplate1" runat="server"></asp:PlaceHolder>
                    <uc:ProductPrice ID="Price" runat="server" Product='<%#Container.DataItem%>' ShowRetailPrice="true" />
                    <asp:PlaceHolder ID="phItemTemplate2" runat="server"></asp:PlaceHolder>
                    <div style="margin-top:10px;margin-bottom:20px"><uc:AddToCartLink ID="Add2Cart" runat="server" ProductId='<%#Eval("ProductId")%>' /></div>
                </ItemTemplate>
                <SeparatorTemplate>&nbsp;</SeparatorTemplate>
                <SeparatorStyle CssClass="separator" Width="1" />
            </asp:DataList><br clear="all" />
            <asp:Panel ID="PagerPanel" runat="server">
                <div class="paging">
                    <asp:Repeater ID="PagerControls" runat="server" OnItemCommand="PagerControls_ItemCommand">
                        <ItemTemplate>
                            <a class='<%#Eval("TagClass")%>'  href='<%#Eval("NavigateUrl")%>'><%#Eval("Text")%></a>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </asp:Panel>                    
            <div align="center">
                <asp:Label ID="NoSearchResults" runat="server" Text="No products match your search criteria." Visible="false"></asp:Label>
            </div>
            <asp:HiddenField ID="HiddenPageIndex" runat="server" Value="0" />
       </div>
    </ContentTemplate>
</ajax:UpdatePanel>