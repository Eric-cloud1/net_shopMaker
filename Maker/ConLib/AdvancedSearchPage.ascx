<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AdvancedSearchPage.ascx.cs" Inherits="ConLib_AdvancedSearchPage" %>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
<%--
<conlib>
<summary>Displays an advanced search page, to search products in the catalog.</summary>
</conlib>
--%>
<%@ Register Src="~/ConLib/Utility/ProductPrice.ascx" TagName="ProductPrice" TagPrefix="uc" %>
<%@ Register Src="~/ConLib/AddToCartLink.ascx" TagName="AddToCartLink" TagPrefix="uc" %>
<div class="pageHeader">
    <h1 class="heading">Advanced Search</h1>
</div>
<asp:ValidationSummary ID="ValidationSummary2" runat="server" />
<table class="inputForm">
    <tr>
        <th class="rowHeader" valign="top">
            <asp:Label ID="KeywordsLabel" runat="server" Text="Search Keywords:"></asp:Label>
        </th>
        <td>
            <asp:TextBox ID="Keywords" runat="server"></asp:TextBox>
            <cb:SearchKeywordValidator ID="KeywordValidator" runat="server" ControlToValidate="Keywords" 
                ErrorMessage="Search keyword must be at least {0} characters in length excluding spaces and wildcards." Text="*"></cb:SearchKeywordValidator>
            <br />
            <asp:Label ID="WildCardMessage" runat="server" Text="Wild Cards * and ? may be used."></asp:Label>                    
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
            <asp:Label ID="SearchInLabel" runat="server" Text="Search In:"></asp:Label>        
        </th>
        <td>
            <asp:CheckBox ID="SearchName" runat="server" Text="Name" Checked="true" />
            <asp:CheckBox ID="SearchDescription" runat="server" Text="Description"  />
            <asp:CheckBox ID="SearchSKU" runat="server" Text="SKU" />
        </td>
    </tr>
    <tr>        
        <th class="rowHeader">
            <asp:Label ID="CategoryLabel" runat="server" Text="Select Category:"></asp:Label>&nbsp;
        </th>
        <td>
            <asp:DropDownList ID="CategoryList" runat="server" AppendDataBoundItems="True" 
                DataTextField="Name" DataValueField="CategoryId">
                <asp:ListItem Text="- Any Category -" Value="0"></asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
    <tr>
        <th class="rowHeader">
         <asp:Label ID="ManufacturerLabel" runat="server" Text="Select Manufacturer:"></asp:Label>        
        </th>
        <td>
            <asp:DropDownList ID="ManufacturerList" runat="server" AppendDataBoundItems="True"
                DataSourceID="ManufacturerDs" DataTextField="Name" DataValueField="ManufacturerId">
                <asp:ListItem Text="- Any Manufacturer -" Value="0"></asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>            
    <tr>
        <th class="rowHeader">
            <asp:Label ID="PriceRangeLabel" runat="server" Text="Price Range:"></asp:Label>
        </th>
        <td>
            <asp:Label ID="Label4" runat="server" Text="Low:"></asp:Label>
            <asp:TextBox ID="LowPrice" runat="server" Columns="4" MaxLength="4"></asp:TextBox>
            <asp:RangeValidator ID="LowPriceValidator1" runat="server" Type="Currency" MinimumValue="0" MaximumValue="99999999" ControlToValidate="LowPrice" ErrorMessage="Low price must be a valid value." Text="*"></asp:RangeValidator>
            <asp:Label ID="Label3" runat="server" Text="High:"></asp:Label>
            <asp:TextBox ID="HighPrice" runat="server" Columns="4" MaxLength="4"></asp:TextBox>
            <asp:RangeValidator ID="HighPriceValidator1" runat="server" Type="Currency" MinimumValue="0" MaximumValue="99999999" ControlToValidate="HighPrice" ErrorMessage="High price must be a valid value.<br/>" Text="*"></asp:RangeValidator>
            <asp:CompareValidator ID="LowHighPriceValidator1" runat="server" Type="Currency" Operator="GreaterThanEqual" ControlToValidate="HighPrice" ControlToCompare="LowPrice" ErrorMessage="High price should be greater then low price." Text="*" ></asp:CompareValidator>
        </td>
    </tr>
    <tr>                
        <td colspan="2" align="right">                    
            <asp:Button ID="SearchButton" runat="server" OnClick="SearchButton_Click" Text="Search" /><br />
        </td>
    </tr>
</table>
<div class="section">
    <div id="SearchResultHeading" runat="server" class="header" visible="false">
        <h2>Search Results</h2>
    </div>
    <asp:GridView ID="ProductsGrid" runat="server" AutoGenerateColumns="False" DataKeyNames="ProductId"
        Width="100%" SkinID="PagedList" AllowPaging="true" PageSize="25" AllowSorting="true" 
        DataSourceID="ProductDs" Visible="false">
        <Columns>                                
            <asp:BoundField DataField="Sku" HeaderText="SKU" SortExpression="Sku">                
                <HeaderStyle HorizontalAlign="Left" />
            </asp:BoundField>                
            <asp:TemplateField HeaderText="Name" SortExpression="Name">
                <ItemTemplate>
                    <asp:HyperLink ID="Name" runat="server" Text='<%#Eval("Name")%>' NavigateUrl='<%#Eval("NavigateUrl")%>'></asp:HyperLink>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Manufacturer" SortExpression="Manufacturer">                    
                <ItemTemplate>
                    <asp:Label ID="Manufacturer" runat="server" Text='<%#GetManufacturerLink((int)Eval("ManufacturerId"))%>'
                        ></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Categories">                
                <ItemTemplate>
                    <asp:PlaceHolder ID="Categories" runat="server"></asp:PlaceHolder>
                    <asp:Literal ID="CategoriesList" runat="server" Text='<%#GetCatsList(Container.DataItem)%>'></asp:Literal>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Retail Price" SortExpression="MSRP">                    
                <HeaderStyle HorizontalAlign="Center" Width="80px" />
                <ItemStyle HorizontalAlign="Center" />
                <ItemTemplate>
                    <asp:Label ID="MSRP" runat="server" Text='<%#GetMSRP(Container.DataItem)%>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Our Price" SortExpression="Price">                    
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
                <ItemTemplate>
				    <uc:ProductPrice ID="Price" runat="server" Product='<%#Container.DataItem%>'></uc:ProductPrice>
                </ItemTemplate>
            </asp:TemplateField>              
            <asp:TemplateField>
                <ItemStyle HorizontalAlign="Center" />
                <ItemTemplate>
                    <uc:AddToCartLink ID="Add2Cart" runat="server" ProductId='<%#Eval("ProductId")%>' />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>            
        <EmptyDataTemplate>
            <asp:Localize ID="EmptyMessage" runat="server" Text="- no matching products -"></asp:Localize>
        </EmptyDataTemplate>
    </asp:GridView>
</div>        
<asp:ObjectDataSource ID="ProductDs" runat="server" DataObjectTypeName="MakerShop.Products.Product"
    OldValuesParameterFormatString="original_{0}" SelectMethod="AdvancedSearch" SelectCountMethod="AdvancedSearchCount" SortParameterName="sortExpression"  TypeName="MakerShop.Products.ProductDataSource">
    <SelectParameters>
        <asp:ControlParameter Name="keyword" Type="String" ControlID="Keywords" PropertyName="Text" />
        <asp:ControlParameter Name="categoryId" Type="Int32" ControlID="CategoryList" PropertyName="SelectedValue" />
        <asp:ControlParameter Name="manufacturerId" Type="Int32" ControlID="ManufacturerList" PropertyName="SelectedValue" />
        <asp:ControlParameter Name="searchName" Type="boolean" ControlID="SearchName" PropertyName="Checked" />
        <asp:ControlParameter Name="searchDescription" Type="boolean" ControlID="SearchDescription" PropertyName="Checked" />
        <asp:ControlParameter Name="searchSKU" Type="boolean" ControlID="SearchSKU" PropertyName="Checked" />
        <asp:ControlParameter Name="lowPrice" Type="decimal" ControlID="LowPrice" PropertyName="Text" />
        <asp:ControlParameter Name="highPrice" Type="decimal" ControlID="HighPrice" PropertyName="Text" />
    </SelectParameters>
</asp:ObjectDataSource>
<asp:ObjectDataSource ID="ManufacturerDs" runat="server" OldValuesParameterFormatString="original_{0}"
    SelectMethod="LoadForStore" TypeName="MakerShop.Products.ManufacturerDataSource">
    <SelectParameters>
        <asp:Parameter Name="sortExpression" DefaultValue="Name" />
    </SelectParameters>
</asp:ObjectDataSource>
