using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using MakerShop.Common;
using MakerShop.Orders;
using MakerShop.Payments;
using MakerShop.Shipping;
using MakerShop.Stores;
using MakerShop.Utility;
using MakerShop.Taxes;
using MakerShop.Users;
using MakerShop.Products;


public partial class Admin_Orders_CustomerService_Default : System.Web.UI.Page
{
    
    private Order _Order;

    private string _Src;
    public string Src
    {
        get { return _Src; }
        set { _Src = value; }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        _Order = OrderHelper.GetOrderFromContext();
        Product _Product = new Product();
        _Product.Load(AlwaysConvert.ToInt(Request.QueryString["productId"]));


        if (!String.IsNullOrEmpty(_Product.CustomerServiceUrl))
        {
            this.Src = _Product.CustomerServiceUrl;

            if (_Order != null)
                this.Src += string.Format(@"?ordernumber={0}", _Order.OrderNumber);
        }
        else
        {
            CallDisposition1.Visible = false;
            CallDispositionCaption.Visible = false;

        }

    }
   

    protected void Page_Load(object sender, EventArgs e)
    {
        if (_Order != null)
            Caption.Text = String.Format(Caption.Text, _Order.OrderNumber);

        else
            Caption.Visible = false;


    }
}
