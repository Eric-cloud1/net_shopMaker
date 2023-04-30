<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"
CodeFile="TaxCodeProducts.aspx.cs" Inherits="Admin_Taxes_TaxCodeProducts"
Title="Tax Code Products" %> <%@ Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
  <script type="text/javascript">
    function toggleCheckBoxState(id, checkState) {
      var cb = document.getElementById(id);
      if (cb != null) cb.checked = checkState;
    }

    function toggleSelected(checkState) {
      // Toggles through all of the checkboxes defined in the CheckBoxIDs array
      // and updates their value to the checkState input parameter
      if (CheckBoxIDs != null) {
        for (var i = 0; i < CheckBoxIDs.length; i++)
          toggleCheckBoxState(CheckBoxIDs[i], checkState.checked);
      }
    }

    function confirmTaxCodeChange(oldCode, newCode) {
      return confirm(
        'Are you sure to change tax code from "' +
          oldCode +
          '" to "' +
          newCode +
          '"?'
      );
    }
  </script>
  <div class="pageHeader">
    <div class="caption">
      <h1>
        <asp:Localize
          ID="Caption"
          runat="server"
          Text="Products assigned to '{0}'"
        ></asp:Localize>
      </h1>
    </div>
  </div>
  <table cellpadding="4" cellspacing="0" class="inputForm" width="100%">
    <tr>
      <td valign="top" class="innerLayout" style="width: 50%">
        <div class="section">
          <div class="header">
            <h2>Assigned Products</h2>
          </div>
          <div class="content">
            <ajax:UpdatePanel
              ID="RelatedProductsAjax"
              runat="server"
              UpdateMode="Conditional"
            >
              <ContentTemplate>
                <cb:SortedGridView
                  ID="ProductGrid"
                  runat="server"
                  AutoGenerateColumns="False"
                  DataKeyNames="ProductId"
                  DataSourceID="ProductDs"
                  Width="100%"
                  SkinID="PagedList"
                  AllowPaging="True"
                  PageSize="20"
                  AllowSorting="true"
                  DefaultSortExpression="Name"
                  OnDataBound="ProductGrid_DataBound"
                  OnRowDeleting="ProductGrid_RowDeleting"
                >
                  <Columns>
                    <asp:TemplateField>
                      <HeaderTemplate>
                        <input type="checkbox" onclick="toggleSelected(this)" />
                      </HeaderTemplate>
                      <ItemStyle HorizontalAlign="Center" width="30px" />
                      <ItemTemplate>
                        <asp:CheckBox ID="Selected" runat="server" />
                      </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField
                      HeaderText="Product"
                      SortExpression="Name"
                    >
                      <HeaderStyle HorizontalAlign="Left" />
                      <ItemTemplate>
                        <asp:HyperLink
                          ID="NameLink"
                          runat="server"
                          Text='<%# Eval("Name") %>'
                          NavigateUrl='<%#Eval("ProductId", "../Products/EditProduct.aspx?ProductId={0}")%>'
                        ></asp:HyperLink>
                      </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                      <ItemStyle HorizontalAlign="Center" Width="50px" />
                      <ItemTemplate>
                        <asp:ImageButton
                          ID="RemoveButton2"
                          runat="server"
                          SkinID="DeleteIcon"
                          AlternateText="Remove"
                          ToolTip="Remove"
                          CommandArgument='<%#Eval("ProductId")%>'
                          OnClick="RemoveButton2_Click"
                        />
                      </ItemTemplate>
                    </asp:TemplateField>
                  </Columns>
                  <EmptyDataTemplate>
                    <asp:Label
                      ID="EmptyDataMessage"
                      runat="server"
                      Text="There are no products associated with this tax code."
                    ></asp:Label>
                  </EmptyDataTemplate>
                </cb:SortedGridView>
                <asp:Panel ID="NewTaxCodePanel" runat="server">
                  <table class="inputForm">
                    <tr>
                      <th class="rowHeader">
                        <asp:Label
                          ID="NewTaxCodeLabel"
                          runat="server"
                          Text="Change selected to:"
                          SkinID="FieldHeader"
                        ></asp:Label>
                      </th>
                      <td>
                        <asp:DropDownList
                          ID="NewTaxCode"
                          runat="server"
                          AppendDataBoundItems="true"
                          DataTextField="Name"
                          DataValueField="TaxCodeId"
                        >
                          <asp:ListItem Value="" Text=""></asp:ListItem>
                        </asp:DropDownList>
                      </td>
                      <td>
                        <asp:Button
                          ID="NewTaxCodeUpdateButton"
                          runat="server"
                          Text="Go"
                          OnClick="NewTaxCodeUpdateButton_Click"
                        />
                      </td>
                    </tr>
                  </table>
                </asp:Panel>
                <asp:HyperLink
                  ID="BackLink"
                  runat="server"
                  Text="Back"
                  SkinID="Button"
                  NavigateUrl="TaxCodes.aspx"
                ></asp:HyperLink>
                <asp:ObjectDataSource
                  ID="ProductDs"
                  runat="server"
                  EnablePaging="True"
                  OldValuesParameterFormatString="original_{0}"
                  SelectCountMethod="CountForTaxCode"
                  SelectMethod="LoadForTaxCode"
                  SortParameterName="sortExpression"
                  TypeName="MakerShop.Products.ProductDataSource"
                >
                  <SelectParameters>
                    <asp:QueryStringParameter
                      Name="taxCodeId"
                      QueryStringField="TaxCodeId"
                      Type="Object"
                    />
                  </SelectParameters>
                </asp:ObjectDataSource>
              </ContentTemplate>
            </ajax:UpdatePanel>
          </div>
        </div>
      </td>
      <td valign="top">
        <div class="section">
          <div class="header">
            <h2>Find and Assign Products</h2>
          </div>
          <div class="content">
            <ajax:UpdatePanel
              ID="SearchAjax"
              runat="server"
              UpdateMode="Conditional"
            >
              <ContentTemplate>
                <table class="inputForm">
                  <tr>
                    <th class="rowHeader" style="text-align: left">
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
                  </tr>
                  <tr>
                    <th class="rowHeader" style="text-align: left">
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
                  </tr>
                  <tr>
                    <th class="rowHeader" style="text-align: left">
                      <cb:ToolTipLabel
                        ID="NoTaxCodeLabel"
                        runat="server"
                        Text="Products without tax code only:"
                        ToolTip="Show products with that have no tax code assigned."
                      />
                    </th>
                    <td>
                      <asp:CheckBox
                        ID="NoTaxCode"
                        runat="server"
                        Checked="true"
                      />
                    </td>
                  </tr>
                  <tr>
                    <td colspan="2" align="right">
                      <asp:LinkButton
                        ID="SearchButton"
                        runat="server"
                        Text="Search"
                        SkinID="Button"
                        OnClick="SearchButton_Click"
                      /><br />
                    </td>
                  </tr>
                </table>
                <cb:SortedGridView
                  ID="SearchResultsGrid"
                  runat="server"
                  AutoGenerateColumns="False"
                  DataKeyNames="ProductId"
                  GridLines="Both"
                  SkinID="PagedList"
                  DataSourceID="ProductSearchDs"
                  Width="100%"
                  Visible="false"
                  AllowPaging="true"
                  PageSize="20"
                  AllowSorting="true"
                  DefaultSortExpression="Name"
                >
                  <Columns>
                    <asp:TemplateField HeaderText="Thumbnail">
                      <itemstyle horizontalalign="Center" />
                      <itemtemplate>
                        <asp:HyperLink
                          ID="NodeImageLink"
                          runat="server"
                          NavigateUrl='<%#Eval("ProductId", "~/Admin/Products/EditProduct.aspx?ProductId={0}")%>'
                        >
                          <asp:Image
                            ID="NodeImage"
                            runat="server"
                            ImageUrl='<%# Eval("ThumbnailUrl") %>'
                            Visible='<%# !string.IsNullOrEmpty((string)Eval("ThumbnailUrl")) %>'
                            AlternateText='<%# Eval("Name") %>'
                          />
                        </asp:HyperLink>
                      </itemtemplate>
                    </asp:TemplateField>
                    <asp:TemplateField
                      HeaderText="Product"
                      SortExpression="Name"
                    >
                      <headerstyle horizontalalign="Left" />
                      <itemtemplate>
                        <asp:HyperLink
                          ID="ProductName"
                          runat="server"
                          Text='<%#Eval("Name")%>'
                          SkinID="FieldHeader"
                          NavigateUrl='<%#Eval("ProductId", "~/Admin/Products/EditProduct.aspx?ProductId={0}")%>'
                        /><br />
                      </itemtemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Tax Code">
                      <headerstyle horizontalalign="Center" />
                      <itemstyle horizontalalign="Center" />
                      <itemtemplate>
                        <asp:Label
                          ID="TaxCodeName"
                          runat="server"
                          Text='<%#((Product)Container.DataItem).TaxCode != null? ((Product)Container.DataItem).TaxCode.Name:""%>'
                          SkinID="FieldHeader"
                        /><br />
                      </itemtemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Related">
                      <itemstyle width="50px" horizontalalign="Center" />
                      <itemtemplate>
                        <asp:ImageButton
                          ID="AttachButton"
                          runat="server"
                          CommandArgument="<%#Container.DataItemIndex%>"
                          AlternateText="Add"
                          ToolTip="Assign to this tax code"
                          SkinId="AddIcon"
                          OnClick="AttachButton_Click"
                          Visible="<%#!IsProductLinked((Product)Container.DataItem)%>"
                          OnClientClick="this.visible=false;"
                        />
                        <asp:ImageButton
                          ID="RemoveButton"
                          runat="server"
                          CommandArgument="<%#Container.DataItemIndex%>"
                          AlternateText="Remove"
                          ToolTip="Remove from this tax code"
                          SkinId="DeleteIcon"
                          OnClientClick="this.visible=false"
                          OnClick="RemoveButton_Click"
                          Visible="<%#IsProductLinked((Product)Container.DataItem)%>"
                        />
                      </itemtemplate>
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
                    <asp:Parameter
                      Name="taxCodeId"
                      Type="Object"
                      DefaultValue="-1"
                    />
                  </SelectParameters>
                </asp:ObjectDataSource>
              </ContentTemplate>
            </ajax:UpdatePanel>
          </div>
        </div>
      </td>
    </tr>
  </table>
</asp:Content>
