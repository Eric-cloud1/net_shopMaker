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

public partial class Admin_Store_Security_Connection : MakerShop.Web.UI.MakerShopAdminPage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        ExistingConnectionString.Text = GetConnectionString();
        if (!Page.IsPostBack)
        {
            ConnectionString.Text = ExistingConnectionString.Text;
        }
    }

    private string GetConnectionString()
    {
        return ConfigurationManager.ConnectionStrings["MakerShop"].ConnectionString;
    }

    protected void UpdateButton_Click(object sender, EventArgs e)
    {
        if (AckBox.Checked)
        {
            string message;
            if (ValidateConnection(ConnectionString.Text, out message))
            {
                DatabaseHelper.UpdateConnectionString(ConnectionString.Text, EncryptIt.Checked);
                SavedMessage.Text = "Connection string updated at " + LocaleHelper.LocalNow.ToShortTimeString() + ".";
                SavedMessage.Visible = true;
                ExistingConnectionString.Text = ConnectionString.Text;
                AckBox.Checked = false;
            }
            else
            {
                ErrorMessage.Text = "A connection could not be established for the given connection string.  Connection string not updated.  (The server said: " + message + ")";
                ErrorMessage.Visible = true;
            }
        }
        else
        {
            ErrorMessage.Text = "You did not acknowledge the warning.  Connection string not updated.";
            ErrorMessage.Visible = true;
        }
    }

    protected bool ValidateConnection(string connectionString, out string message)
    {
        message = string.Empty;
        bool valid = false;
        using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connectionString))
        {
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            if (conn.State == System.Data.ConnectionState.Open)
            {
                valid = true;
                conn.Close();
            }
        }
        return valid;
    }
    
    protected void CancelButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/Admin/Store/Security/Default.aspx");
    }

}
