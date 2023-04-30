using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MakerShop.Common;
using MakerShop.Marketing;
using MakerShop.Orders;
using MakerShop.Utility;
using MakerShop.Catalog;
using MakerShop.Products;

public partial class ConLib_SimpleCategoryList : System.Web.UI.UserControl
{
    private string _CssClass;
    private string _HeaderCssClass;
    private string _HeaderText;
    private string _ContentCssClass;
    private int _CategoryId = -1;

    public int CategoryId
    {
        get { return _CategoryId; }
        set { _CategoryId = value; }
    }

    [Personalizable(), WebBrowsable()]
    public string CssClass
    {
        get { return _CssClass; }
        set { _CssClass = value; }
    }

    [Personalizable(), WebBrowsable()]
    public string HeaderCssClass
    {
        get { return _HeaderCssClass; }
        set { _HeaderCssClass = value; }
    }

    [Personalizable(), WebBrowsable()]
    public string HeaderText
    {
        get { return _HeaderText; }
        set { _HeaderText = value; }
    }

    [Personalizable(), WebBrowsable()]
    public string ContentCssClass
    {
        get { return _ContentCssClass; }
        set { _ContentCssClass = value; }
    }

	protected void Page_Load(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(CssClass)) MainPanel.CssClass = CssClass;
        if (!string.IsNullOrEmpty(HeaderCssClass)) HeaderPanel.CssClass = HeaderCssClass;
        if (!string.IsNullOrEmpty(HeaderText)) HeaderTextLabel.Text = HeaderText;
        if (!string.IsNullOrEmpty(ContentCssClass)) ContentPanel.CssClass = ContentCssClass;
        if (_CategoryId < 0) _CategoryId = PageHelper.GetCategoryId();
        CategoryCollection subCategories = CategoryDataSource.LoadForParent(_CategoryId, true);
        CategoryList.DataSource = subCategories;
        CategoryList.DataBind();
        if(subCategories.Count == 0){
            Category category = CategoryDataSource.Load(_CategoryId);

            if (category != null)
            {
                NoSubCategoriesText.Text = String.Format(NoSubCategoriesText.Text, category.Name);
                NoSubCategoriesText.Visible = true;
            }
            
        }
	}

}