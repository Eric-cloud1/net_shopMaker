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
using MakerShop.Taxes;

public partial class Admin_Taxes_TaxCodeProducts : MakerShop.Web.UI.MakerShopAdminPage
{


    private int _TaxCodeId;
    private TaxCode _TaxCode;
    protected void Page_Load(object sender, EventArgs e)
    {
        _TaxCodeId = AlwaysConvert.ToInt(Request.QueryString["TaxCodeId"]);
        _TaxCode  = TaxCodeDataSource.Load(_TaxCodeId);
        if (_TaxCode == null) Response.Redirect("Default.aspx");
        if (!Page.IsPostBack)
        {
            Caption.Text = string.Format(Caption.Text, _TaxCode.Name);
            List<TaxCode> targets = new List<TaxCode>();
            TaxCodeCollection allTaxCodes = TaxCodeDataSource.LoadForStore();
            foreach (TaxCode w in allTaxCodes)
            {
                if (w.TaxCodeId != AlwaysConvert.ToInt(_TaxCodeId)) targets.Add(w);
            }
            if (targets.Count == 0)
            {
                NewTaxCodePanel.Visible = false;
                ProductGrid.Columns[0].Visible = false;
            }
            else
            {
                NewTaxCode.DataSource = targets;
                NewTaxCode.DataBind();
            }
        }
    }

    protected void BackButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("Default.aspx");
    }

    protected void NewTaxCodeUpdateButton_Click(object sender, EventArgs e)
    {
        int newTaxCodeId = AlwaysConvert.ToInt(NewTaxCode.SelectedValue);
        TaxCode newTaxCode = TaxCodeDataSource.Load(newTaxCodeId);
        if (newTaxCode != null)
        {
            foreach (GridViewRow row in ProductGrid.Rows)
            {
                CheckBox selected = (CheckBox)row.FindControl("Selected");
                if ((selected != null) && (selected.Checked))
                {
                    int productId = AlwaysConvert.ToInt(ProductGrid.DataKeys[row.DataItemIndex].Value);
                    Product product = ProductDataSource.Load(productId);
                    if (product != null)
                    {
                        product.TaxCodeId = newTaxCodeId;
                        product.Save();
                    }
                }
            }
            ProductGrid.DataBind();
            if (SearchResultsGrid.Visible)
            {
                SearchResultsGrid.DataBind();
                SearchAjax.Update();
            }
        }
    }

    protected void ProductGrid_DataBound(object sender, EventArgs e)
    {
        if (ProductGrid.Rows.Count == 0) NewTaxCodePanel.Visible = false;
        foreach (GridViewRow gvr in ProductGrid.Rows)
        {
            CheckBox cb = (CheckBox)gvr.FindControl("Selected");
            ScriptManager.RegisterArrayDeclaration(ProductGrid, "CheckBoxIDs", String.Concat("'", cb.ClientID, "'"));
        }
    }

    protected void RemoveButton2_Click(object sender, ImageClickEventArgs e)
    {
        ImageButton removeButton = (ImageButton)sender;
        int productId = AlwaysConvert.ToInt(removeButton.CommandArgument);
        UpdateAssociation(productId, false);
        ProductGrid.DataBind();
        if (SearchResultsGrid.Visible )
        {
            SearchResultsGrid.DataBind();
            SearchAjax.Update();
        }

    }

    private void UpdateAssociation(int relatedProductId, bool associate)
    {
        Product product = ProductDataSource.Load(relatedProductId);
        if (product != null)
        {
            if (associate)
            {
                product.TaxCodeId = _TaxCodeId;
            }
            else product.TaxCodeId = 0;
            product.Save();
        }
    }
    
    protected void ProductGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        int relatedProductId = (int)e.Keys[0];
        UpdateAssociation(relatedProductId, false);
        //TODO:
        ////CHECK THE SEARCH RESULTS GRID TO SEE IF THIS ITEMS APPEARS
        //int tempIndex = 0;
        //foreach (DataKey key in SearchResultsGrid.DataKeys)
        //{
        //    int tempId = (int)key.Value;
        //    if (relatedProductId == tempId)
        //    {
        //        //CHANGE THE REMOVE BUTTON TO ADD FOR THIS ROW
        //        ImageButton removeButton = SearchResultsGrid.Rows[tempIndex].FindControl("RemoveButton") as ImageButton;
        //        if (removeButton != null) removeButton.Visible = false;
        //        ImageButton attachButton = SearchResultsGrid.Rows[tempIndex].FindControl("AttachButton") as ImageButton;
        //        if (attachButton != null) attachButton.Visible = true;
        //        break;
        //    }
        //    tempIndex++;
        //}
        ProductGrid.DataBind();
        e.Cancel = true;
    }

    protected void AttachButton_Click(object sender, EventArgs e)
    {
        ImageButton attachButton = (ImageButton)sender;
        int dataItemIndex = AlwaysConvert.ToInt(attachButton.CommandArgument);
        GridView grid = SearchResultsGrid;
        dataItemIndex = (dataItemIndex - (grid.PageSize * grid.PageIndex));
        int productId = (int)grid.DataKeys[dataItemIndex].Value;
        UpdateAssociation(productId, true);
        ImageButton removeButton = attachButton.Parent.FindControl("RemoveButton") as ImageButton;
        if (removeButton != null) removeButton.Visible = true;
        attachButton.Visible = false;

        Label taxCodeNameLabel = attachButton.Parent.FindControl("TaxCodeName") as Label;
        taxCodeNameLabel.Text = _TaxCode.Name;

        ProductGrid.DataBind();
        RelatedProductsAjax.Update();         
    }

    protected void RemoveButton_Click(object sender, EventArgs e)
    {
        ImageButton removeButton = (ImageButton)sender;
        int dataItemIndex = AlwaysConvert.ToInt(removeButton.CommandArgument);
        GridView grid = SearchResultsGrid;
        dataItemIndex = (dataItemIndex - (grid.PageSize * grid.PageIndex));
        int productId = (int)grid.DataKeys[dataItemIndex].Value;
        UpdateAssociation(productId, false);
        ImageButton attachButton = removeButton.Parent.FindControl("AttachButton") as ImageButton;
        if (attachButton != null) attachButton.Visible = true;
        removeButton.Visible = false;

        Label taxCodeNameLabel = attachButton.Parent.FindControl("TaxCodeName") as Label;
        taxCodeNameLabel.Text = String.Empty;

        ProductGrid.DataBind();
        RelatedProductsAjax.Update();
    }

    // SEARCH RLEATED CODE
    protected void SearchButton_Click(object sender, EventArgs e)
    {
        SearchResultsGrid.Visible = true;
        SearchResultsGrid.PageIndex = 0;
        SearchResultsGrid.DataBind();
    }

    protected bool IsProductLinked(Product product)
    {
        if (product != null)
        {
            return (product.TaxCodeId == _TaxCodeId);
        }
        return false;
    }

    protected void ProductSearchDs_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
        SearchResultsGrid.Columns[0].Visible = ShowImages.Checked;
        SearchResultsGrid.Columns[2].Visible = !NoTaxCode.Checked;
        if (NoTaxCode.Checked)
            ProductSearchDs.SelectParameters["taxCodeId"].DefaultValue = "-1";
        else
            ProductSearchDs.SelectParameters["taxCodeId"].DefaultValue = "0";
    }
    


}
