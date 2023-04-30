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
using System.Text.RegularExpressions;
using MakerShop.Common;
using MakerShop.Catalog;
using MakerShop.Orders;
using MakerShop.Products;
using MakerShop.Stores;
using MakerShop.Users;
using MakerShop.Utility;
using MakerShop.Reporting;
using System.Collections.Generic;

public partial class Admin_Products_EditSimilarProducts : MakerShop.Web.UI.MakerShopAdminPage
{

    private Product _Product;
    private int _ProductId = 0;
    
    protected void Page_Init(object sender, EventArgs e)
    {
        _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
        _Product = ProductDataSource.Load(_ProductId);
        if (_Product == null) Response.Redirect(NavigationHelper.GetAdminUrl("Catalog/Browse.aspx"));
        Caption.Text = string.Format(Caption.Text, _Product.Name);
    }

    protected void SearchButton_Click(object sender, EventArgs e)
    {
        SearchResultsGrid.Visible = true;
        SearchResultsGrid.PageIndex = 0;
        SearchResultsGrid.DataBind();
    }
    
    protected void AttachButton_Click(object sender, EventArgs e)
    {
        ImageButton attachButton = (ImageButton)sender;
        int dataItemIndex = AlwaysConvert.ToInt(attachButton.CommandArgument);
        GridView grid = SearchResultsGrid;
        dataItemIndex = (dataItemIndex - (grid.PageSize * grid.PageIndex));
        int productId = (int)grid.DataKeys[dataItemIndex].Value;
        SetLink(productId, true);
        ImageButton removeButton = attachButton.Parent.FindControl("RemoveButton") as ImageButton;
        if (removeButton != null) removeButton.Visible = true;
        attachButton.Visible = false;
        RelatedProductGrid.DataBind();
    }

    protected void RemoveButton_Click(object sender, EventArgs e)
    {
        ImageButton removeButton = (ImageButton)sender;
        int dataItemIndex = AlwaysConvert.ToInt(removeButton.CommandArgument);
        GridView grid = SearchResultsGrid;
        dataItemIndex = (dataItemIndex - (grid.PageSize * grid.PageIndex));
        int productId = (int)grid.DataKeys[dataItemIndex].Value;
        SetLink(productId, false);
        ImageButton attachButton = removeButton.Parent.FindControl("AttachButton") as ImageButton;
        if (attachButton != null) attachButton.Visible = true;
        removeButton.Visible = false;
        RelatedProductGrid.DataBind();
    }
    
    protected void RelatedProductGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "MoveUp")
        {
            RelatedProductCollection RelatedProducts = _Product.RelatedProducts;
            int itemIndex = AlwaysConvert.ToInt(e.CommandArgument);
            if ((itemIndex < 1) || (itemIndex > RelatedProducts.Count - 1)) return;
            RelatedProduct selectedItem = RelatedProducts[itemIndex];
            RelatedProduct swapItem = RelatedProducts[itemIndex - 1];
            RelatedProducts.RemoveAt(itemIndex - 1);
            RelatedProducts.Insert(itemIndex, swapItem);
            for (int i = 0; i < RelatedProducts.Count; i++)
            {
                RelatedProducts[i].OrderBy = (short)i;
            }
            RelatedProducts.Save();
            RelatedProductGrid.DataBind();
        }
        else if (e.CommandName == "MoveDown")
        {
            RelatedProductCollection RelatedProducts = _Product.RelatedProducts;
            int itemIndex = AlwaysConvert.ToInt(e.CommandArgument);
            if ((itemIndex > RelatedProducts.Count - 2) || (itemIndex < 0)) return;
            RelatedProduct selectedItem = RelatedProducts[itemIndex];
            RelatedProduct swapItem = RelatedProducts[itemIndex + 1];
            RelatedProducts.RemoveAt(itemIndex + 1);
            RelatedProducts.Insert(itemIndex, swapItem);
            for (int i = 0; i < RelatedProducts.Count; i++)
            {
                RelatedProducts[i].OrderBy = (short)i;
            }
            RelatedProducts.Save();
            RelatedProductGrid.DataBind();
        }
    }    

    private void SetLink(int relatedProductId, bool linked)
    {
        int index = _Product.RelatedProducts.IndexOf(_ProductId, relatedProductId);
        if (linked && (index < 0))
        {
            _Product.RelatedProducts.Add(new RelatedProduct(_ProductId, relatedProductId));
            _Product.RelatedProducts.Save();
        }
        else if (!linked && (index > -1))
        {
            _Product.RelatedProducts.DeleteAt(index);
        }
    }

    protected bool IsProductLinked(int relatedProductId)
    {
        return (_Product.RelatedProducts.IndexOf(_ProductId, relatedProductId) > -1);
    }

    protected void ProductSearchDs_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
        SearchResultsGrid.Columns[0].Visible = ShowImages.Checked;
    }

    private void RedirectToEdit()
    {
        Response.Redirect("EditProduct.aspx?ProductId=" + _ProductId.ToString());
    }

    protected void FinishButton_Click(object sender, EventArgs e)
    {
        RedirectToEdit();
    }

    protected void RelatedProductGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int relatedProductId = (int)e.Keys[0];
        SetLink(relatedProductId, false);
        //CHECK THE SEARCH RESULTS GRID TO SEE IF THIS ITEMS APPEARS
        int tempIndex = 0;
        foreach (DataKey key in SearchResultsGrid.DataKeys)
        {
            int tempId = (int)key.Value;
            if (relatedProductId == tempId)
            {
                //CHANGE THE REMOVE BUTTON TO ADD FOR THIS ROW
                ImageButton removeButton = SearchResultsGrid.Rows[tempIndex].FindControl("RemoveButton") as ImageButton;
                if (removeButton != null) removeButton.Visible = false;
                ImageButton attachButton = SearchResultsGrid.Rows[tempIndex].FindControl("AttachButton") as ImageButton;
                if (attachButton != null) attachButton.Visible = true;
                break;
            }
            tempIndex++;
        }
        RelatedProductGrid.DataBind();
        e.Cancel = true;
    }

}
