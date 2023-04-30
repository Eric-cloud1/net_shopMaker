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

public partial class Admin_People_Vendors_DeleteVendor : MakerShop.Web.UI.MakerShopAdminPage
{

    int _VendorId = 0;
    Vendor _Vendor;

    protected void Page_Init(object sender, System.EventArgs e)
    {
        _VendorId = AlwaysConvert.ToInt(Request.QueryString["VendorId"]);
        _Vendor = VendorDataSource.Load(_VendorId);
        if (_Vendor == null) Response.Redirect("Default.aspx");
        Caption.Text = string.Format(Caption.Text, _Vendor.Name);
        InstructionText.Text = string.Format(InstructionText.Text, _Vendor.Name);
        BindVendors();
    }

    protected void CancelButton_Click(object sender, System.EventArgs e)
    {
        Response.Redirect("Default.aspx");
    }

    protected void DeleteButton_Click(object sender, System.EventArgs e)
    {
        if (VendorList.SelectedIndex != 0)
        {
            int newVendorId = AlwaysConvert.ToInt(VendorList.SelectedValue);
            Vendor newVendor = VendorDataSource.Load(newVendorId);
            if (newVendor != null)
            {
                foreach (Product product in _Vendor.Products)
                {
                    product.VendorId = newVendorId;
                }
                _Vendor.Products.Save();
            }
        }
        _Vendor.Delete();
        Response.Redirect("Default.aspx");
    }

    private void BindVendors()
    {
        VendorCollection vendors = VendorDataSource.LoadForStore("Name");
        int index = vendors.IndexOf(_VendorId);
        if (index > -1) vendors.RemoveAt(index);
        VendorList.DataSource = vendors;
        VendorList.DataBind();
    }

}
