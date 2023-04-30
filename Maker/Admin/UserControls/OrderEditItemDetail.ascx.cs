using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MakerShop.Catalog;
using MakerShop.DigitalDelivery;
using MakerShop.Orders;
using MakerShop.Products;
using MakerShop.Users;

public partial class Admin_UserControls_OrderEditItemDetail : System.Web.UI.UserControl
{

    OrderItem _OrderEditdItem;
    public OrderItem OrderEditItem
    {
        get { return _OrderEditdItem; }
        set { _OrderEditdItem = value; }
    }

    public bool _LinkProducts = false;
    public bool LinkProducts
    {
        get { return _LinkProducts; }
        set { _LinkProducts = value; }
    }

   

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (_OrderEditdItem != null)//&&(isPostBack !=true))
        {
           
            Product product = _OrderEditdItem.Product;
            if (product != null)
            {
                string productName = _OrderEditdItem.Name;
                if (!string.IsNullOrEmpty(_OrderEditdItem.VariantName))
                {
                    string variantName = string.Format(" ({0})", _OrderEditdItem.VariantName);
                    if (!productName.EndsWith(variantName)) productName += variantName;
                }
                if (this.LinkProducts)
                {
                    ProductLink.NavigateUrl = "~/Admin/Products/EditProduct.aspx?ProductId=" + product.ProductId.ToString();
                    ProductLink.Text = productName;
                    ProductName.Visible = false;
                }
                else
                {
                    ProductName.Text = productName;
                    ProductLink.Visible = false;
                }
               //SHOW INPUTS
                for(int i =0; i< _OrderEditdItem.Inputs.Count;i++)
                {
                    ((LiteralControl)Controls[i + 5]).Text = _OrderEditdItem.Inputs[i].Name;
                    ((LiteralControl)Controls[i + 5]).Visible = true;
             

                    ((TextBox)Controls[i+6]).Text = _OrderEditdItem.Inputs[i].InputValue;
                    ((TextBox)Controls[i+6]).Visible = true;

                    ((LiteralControl)Controls[i + 7]).Visible = true;

                }
 
            }
        }
         else
         {
             //NO ITEM TO DISPLAY
             this.Controls.Clear();
         }
    }
}
