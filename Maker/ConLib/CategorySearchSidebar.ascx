<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CategorySearchSidebar.ascx.cs" Inherits="ConLib_CategorySearchSidebar" %>
<%--
<conlib>
<summary>An ajax enabled search bar that displays products in a grid format. Allows customers to narrow and expand by category, manufacturer, and keyword in a search style interface.  This search bar must be used in conjunction with the Category Grid content scriptlet.</summary>
</conlib>
--%>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
<ajax:UpdatePanel ID="SearchFilterAjaxPanel" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div class="searchFilterPanel">
            <div class="searchFilterHeader">
                <h2><asp:Localize ID="SearchFilterHeader" runat="server" Text="Narrow or Expand Results"></asp:Localize></h2>
            </div>
            <asp:Panel ID="ExpandResultPanel" runat="server" CssClass="productGridBorder">
	            <h3 class="searchCriteria">Expand Your Result</h3>
                <asp:Repeater ID="ExpandCategoryLinks" runat="server" OnItemDataBound="ExpandCategoryLinks_ItemDataBound" EnableViewState="false">
                    <ItemTemplate></ItemTemplate>
                </asp:Repeater>
                <asp:LinkButton ID="ExpandManufacturerLink" runat="server" Text="" Visible="false"  Cssclass="searchCriteria" EnableViewState="false"></asp:LinkButton>
                <asp:LinkButton ID="ExpandKeywordLink" runat="server" Text="" Visible="false"  Cssclass="searchCriteria" EnableViewState="false"></asp:LinkButton>
            </asp:Panel>
            <asp:Panel ID="NarrowByCategoryPanel" runat="server" class="productGridBorder">
	            <h3 class="searchCriteria">Narrow by Category</h3>
                <asp:DataList ID="NarrowByCategoryLinks" runat="server" DataKeyField="CategoryId"  OnItemDataBound="NarrowByCategoryLinks_ItemDataBound" EnableViewState="false">
                    <ItemTemplate></ItemTemplate>
                </asp:DataList>
            </asp:Panel>
            <asp:Panel ID="NarrowByManufacturerPanel" runat="server" class="productGridBorder" >
                <h3 class="searchCriteria" >Narrow by Manufacturer</h3>
                <asp:DataList ID="ManufacturerList" runat="server" DataKeyField="ManufacturerId"  OnItemDataBound="ManufacturerList_ItemDataBound" EnableViewState="false">
                    <ItemTemplate></ItemTemplate>
                </asp:DataList>
                <asp:LinkButton ID="ShowAllManufacturers" runat="server" Text="See All &raquo;" CssClass="showAll" EnableViewState="false"></asp:LinkButton>
            </asp:Panel>
            <asp:Panel ID="NarrowByKeywordPanel" runat="server" class="productGridBorder">
                <h3 class="searchCriteria">Narrow by Keyword</h3>
                <div align="center">
                    <asp:TextBox ID="KeywordField" runat="server" Width="120px" MaxLength="60" EnableViewState="false" ValidationGroup="SearchSideBar"></asp:TextBox>
                    <asp:LinkButton ID="KeywordButton" runat="server" SkinID="Button" Text="GO" EnableViewState="false"  ValidationGroup="SearchSideBar"/>
                    <asp:RequiredFieldValidator ID="KeywordRequired" runat="server" ControlToValidate="KeywordField" Text="*" ErrorMessage="Keyword is required." ValidationGroup="SearchSideBar" Display="none"></asp:RequiredFieldValidator>
                    <cb:SearchKeywordValidator ID="KeywordValidator" runat="server" ControlToValidate="KeywordField" Text="*" ErrorMessage="Search keyword must be at least {0} characters in length excluding spaces and wildcards." ValidationGroup="SearchSideBar" Display="none"></cb:SearchKeywordValidator>
                </div>
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="SearchSideBar" />
            </asp:Panel>
        </div>
		<asp:HiddenField ID="VS" runat="server" />
    </ContentTemplate>
</ajax:UpdatePanel>
