<%@ Page Language="C#" MasterPageFile="Product.master"
CodeFile="EditSimilarProducts.aspx.cs"
Inherits="Admin_Products_EditSimilarProducts" Title="Similar Products" %> <%@
Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI"
Assembly="ComponentArt.Web.UI" %> <%@ Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb"
%>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
  <div class="pageHeader">
    <div class="caption">
      <h1>
        <asp:Localize
          ID="Caption"
          runat="server"
          Text="Similar Products for '{0}'"
        ></asp:Localize>
      </h1>
    </div>
  </div>
  <table cellpadding="4" cellspacing="0" class="inputForm" width="100%">
    <tr>
      <td width="100%">
        <p align="justify">
          <asp:HyperLink
            ID="InstructionText"
            runat="server"
            Text="Select the products that are related, that should be highlighted in cross-selling scenarios."
          ></asp:HyperLink>
        </p>
      </td>
    </tr>
    <tr>
      <td valign="top">
        <ajax:UpdatePanel
          ID="MainContentAjax"
          runat="server"
          UpdateMode="Conditional"
        >
          <ContentTemplate>
            <div class="section">
              <div class="header">
                <h2>
                  <asp:Localize
                    ID="FindProductsCaption"
                    runat="server"
                    Text="Find Products"
                  ></asp:Localize>
                </h2>
              </div>
              <asp:Panel
                ID="SearchFormPanel"
                runat="server"
                CssClass="content"
                DefaultButton="SearchButton"
              >
                <table class="inputForm">
                  <tr>
                    <th class="rowHeader">
                      <cb:ToolTipLabel
                        ID="SearchNameLabel"
                        runat="server"
                        Text="Product Name:"
                        ToolTip="Enter all or part of a product name.  Wildcard characters * and ? are accepted."
                      />
                    </th>
                    <td>
                      <asp:TextBox
                        ID="SearchName"
                        runat="server"
                        Text=""
                      ></asp:TextBox>
                    </td>
                    <th class="rowHeader">
                      <cb:ToolTipLabel
                        ID="ShowImagesLabel"
                        runat="server"
                        Text="Show Thumbnails:"
                        ToolTip="When checked, product images will be displayed in the search results."
                      />
                    </th>
                    <td>
                      <asp:CheckBox ID="ShowImages" runat="server" />
                    </td>
                    <td>
                      <asp:LinkButton
                        ID="SearchButton"
                        runat="server"
                        Text="Search"
                        SkinID="Button"
                        OnClick="SearchButton_Click"
                      />
                    </td>
                  </tr>
                </table>
                <cb:SortedGridView
                  ID="SearchResultsGrid"
                  runat="server"
                  AutoGenerateColumns="False"
                  DataKeyNames="ProductId"
                  GridLines="Both"
                  SkinId="PagedList"
                  DataSourceId="ProductSearchDs"
                  Width="100%"
                  Visible="false"
                  AllowPaging="true"
                  PageSize="20"
                  AllowSorting="true"
                  DefaultSortExpression="Name"
                >
                  <Columns>
                    <asp:TemplateField HeaderText="Thumbnail">
                      <ItemStyle HorizontalAlign="Center" />
                      <ItemTemplate>
                        <asp:HyperLink
                          ID="NodeImageLink"
                          runat="server"
                          NavigateUrl='<%# UrlGenerator.GetBrowseUrl((int)Eval("ProductId"), CatalogNodeType.Product, (string)Eval("Name")) %>'
                        >
                          <asp:Image
                            ID="NodeImage"
                            runat="server"
                            ImageUrl='<%# Eval("ThumbnailUrl") %>'
                            Visible='<%# !string.IsNullOrEmpty((string)Eval("ThumbnailUrl")) %>'
                            AlternateText='<%# Eval("Name") %>'
                          />
                        </asp:HyperLink>
                      </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Item" SortExpression="Name">
                      <HeaderStyle HorizontalAlign="Left" />
                      <ItemTemplate>
                        <asp:HyperLink
                          ID="ProductName"
                          runat="server"
                          Text='<%#Eval("Name")%>'
                          SkinID="FieldHeader"
                        /><br />
                      </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Related">
                      <ItemStyle Width="50px" HorizontalAlign="Center" />
                      <ItemTemplate>
                        <asp:ImageButton
                          ID="AttachButton"
                          runat="server"
                          CommandArgument="<%#Container.DataItemIndex%>"
                          AlternateText="Add"
                          ToolTip="Add"
                          SkinId="AddIcon"
                          OnClientClick="this.visible=false"
                          OnClick="AttachButton_Click"
                          Visible="<%#!IsProductLinked(((Product)Container.DataItem).ProductId)%>"
                        />
                        <asp:ImageButton
                          ID="RemoveButton"
                          runat="server"
                          CommandArgument="<%#Container.DataItemIndex%>"
                          AlternateText="Remove"
                          ToolTip="Remove"
                          SkinId="DeleteIcon"
                          OnClientClick="this.visible=false"
                          OnClick="RemoveButton_Click"
                          Visible="<%#IsProductLinked(((Product)Container.DataItem).ProductId)%>"
                        />
                      </ItemTemplate>
                    </asp:TemplateField>
                  </Columns>
                  <EmptyDataTemplate>
                    <asp:HyperLink
                      ID="EmptyMessage"
                      runat="server"
                      Text="There are no products that match the search text."
                    ></asp:HyperLink>
                  </EmptyDataTemplate>
                </cb:SortedGridView>
                <asp:ObjectDataSource
                  ID="ProductSearchDs"
                  runat="server"
                  OldValuesParameterFormatString="original_{0}"
                  SelectMethod="FindProducts"
                  SortParameterName="sortExpression"
                  TypeName="MakerShop.Products.ProductDataSource"
                  OnSelecting="ProductSearchDs_Selecting"
                  SelectCountMethod="FindProductsCount"
                >
                  <SelectParameters>
                    <asp:ControlParameter
                      Name="name"
                      ControlID="SearchName"
                      PropertyName="Text"
                      Type="String"
                    />
                    <asp:Parameter Name="sku" Type="String" />
                    <asp:Parameter Name="categoryId" Type="Object" />
                    <asp:Parameter Name="manufacturerId" Type="Object" />
                    <asp:Parameter Name="vendorId" Type="Object" />
                    <asp:Parameter Name="featured" Type="Object" />
                  </SelectParameters>
                </asp:ObjectDataSource>
              </asp:Panel>
            </div>
            <div class="section">
              <div class="header">
                <h2>Assigned Products</h2>
              </div>
              <div class="content">
                <asp:GridView
                  ID="RelatedProductGrid"
                  runat="server"
                  AutoGenerateColumns="False"
                  DataSourceID="RelatedProductsDs"
                  DataKeyNames="ChildProductId"
                  ShowHeader="False"
                  Width="100%"
                  GridLines="Both"
                  SkinID="PagedList"
                  OnRowCommand="RelatedProductGrid_RowCommand"
                  OnRowDeleting="RelatedProductGrid_RowDeleting"
                >
                  <Columns>
                    <asp:TemplateField HeaderText="Order">
                      <ItemStyle HorizontalAlign="center" Width="60px" />
                      <ItemTemplate>
                        <asp:ImageButton
                          ID="UpButton"
                          runat="server"
                          SkinID="UpIcon"
                          CommandName="MoveUp"
                          CommandArgument="<%#Container.DataItemIndex%>"
                          AlternateText="Up"
                        />
                        <asp:ImageButton
                          ID="DownButton"
                          runat="server"
                          SkinID="DownIcon"
                          CommandName="MoveDown"
                          CommandArgument="<%#Container.DataItemIndex%>"
                          AlternateText="Down"
                        />
                      </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Name">
                      <ItemTemplate>
                        <asp:HyperLink
                          ID="ProductName2"
                          runat="server"
                          Text='<%#Eval("ChildProduct.Name")%>'
                          NavigateUrl='<%#Eval("ChildProductId", "EditProduct.aspx?ProductId={0}")%>'
                        />
                      </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                      <ItemStyle HorizontalAlign="Center" Width="50px" />
                      <ItemTemplate>
                        <asp:ImageButton
                          ID="RemoveButton2"
                          runat="server"
                          SkinID="DeleteIcon"
                          CommandName="Delete"
                          AlternateText="Remove"
                          ToolTip="Remove"
                        />
                      </ItemTemplate>
                    </asp:TemplateField>
                  </Columns>
                  <EmptyDataTemplate>
                    <asp:HyperLink
                      ID="EmptyMessage"
                      runat="server"
                      Text="There are no related products."
                    ></asp:HyperLink>
                  </EmptyDataTemplate>
                </asp:GridView>
                <asp:ObjectDataSource
                  ID="RelatedProductsDs"
                  runat="server"
                  OldValuesParameterFormatString="original_{0}"
                  SelectMethod="LoadForProduct"
                  TypeName="MakerShop.Products.RelatedProductDataSource"
                >
                  <SelectParameters>
                    <asp:QueryStringParameter
                      Name="productId"
                      QueryStringField="ProductId"
                      Type="Object"
                    />
                  </SelectParameters>
                </asp:ObjectDataSource>
              </div>
            </div>
            <asp:Button
              ID="FinishButton"
              runat="server"
              Text="Finish"
              OnClick="FinishButton_Click"
            /><br /><br />
          </ContentTemplate>
        </ajax:UpdatePanel>
      </td>
    </tr>
  </table>
</asp:Content>
