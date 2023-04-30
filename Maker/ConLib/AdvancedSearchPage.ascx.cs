using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MakerShop.Common;
using MakerShop.Catalog;
using MakerShop.Products;
using MakerShop.Stores;
using MakerShop.Taxes;
using MakerShop.Utility;

public partial class ConLib_AdvancedSearchPage : System.Web.UI.UserControl
{
    protected void SearchButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid) BindProductsGrid();
    }

    private void BindProductsGrid()
    {
        SearchResultHeading.Visible = true;
        ProductsGrid.Visible = true;
        ProductsGrid.PageIndex = 0;
        ProductsGrid.DataBind();
    }

    protected void InitializeCategoryTree()
    {
        CategoryLevelNodeCollection categories = CategoryParentDataSource.GetCategoryLevels(0, true);
        foreach (CategoryLevelNode node in categories)
        {
            string prefix = string.Empty;
            for (int i = 0; i <= node.CategoryLevel; i++) prefix += " . . ";
            CategoryList.Items.Add(new ListItem(prefix + node.Name, node.CategoryId.ToString()));
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            InitializeCategoryTree();
        }
        PageHelper.SetDefaultButton(Keywords, SearchButton.ClientID);
        PageHelper.SetDefaultButton(LowPrice, SearchButton.ClientID);
        PageHelper.SetDefaultButton(HighPrice, SearchButton.ClientID);
        
        int minLength = Store.GetCachedSettings().MinimumSearchLength;
        KeywordValidator.MinimumLength = minLength;
        KeywordValidator.ErrorMessage = String.Format(KeywordValidator.ErrorMessage, minLength);
    }

    protected String GetCatsList(object product)
    {
        Product p = (Product)product;
        StringBuilder output = new StringBuilder();
        foreach (int categoryId in p.Categories)
        {
            Category category = CategoryDataSource.Load(categoryId);
            if (category != null)
            {
                output.Append("<a href=\"" + Page.ResolveUrl(category.NavigateUrl) + "\" >" + category.Name + "</a>, ");
            }
        }
        // REMOVE LAST COMMA
        String retValue = output.ToString();
        if (!String.IsNullOrEmpty(retValue))
        {
            return retValue.Substring(0, retValue.Length - 2);
        }
        else
        {
            return retValue;
        }

    }
    protected string GetManufacturerLink(int manufacturerId)
    {
        Manufacturer manufacturer = ManufacturerDataSource.Load(manufacturerId);
        if (manufacturer != null)
        {
            return manufacturer.Name;
        }
        return String.Empty;
    }

    protected string GetMSRP(object obj)
    {
        Product prod = obj as Product;
        if (prod != null && !prod.UseVariablePrice)
        {
            LSDecimal msrpWithVAT = TaxHelper.GetShopPrice(prod.MSRP, prod.TaxCodeId);
            if (msrpWithVAT > 0) return msrpWithVAT.ToString("ulc");
        }
        return string.Empty;
    }
}
