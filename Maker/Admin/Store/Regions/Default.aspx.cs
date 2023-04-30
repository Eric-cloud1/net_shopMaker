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

using System.Globalization;
using System.Linq;

public partial class Admin_Store_Regions_Default : System.Web.UI.Page
{

    public bool Update
    {
        get
        {
            if (Session["Update_Regions_Default"] == null)
                return false;
            return (bool)Session["Update_Regions_Default"];
        }
        set
        {
            Session.Add("Update_Regions_Default", value);
        }
    }


    protected void Page_Init(object sender, EventArgs e)
    {
        this.AddLanguageDialog1.ItemAdded += new PersistentItemEventHandler(AddLanguageDialog1_ItemAdded);
 
        EditLanguageDialog1.ItemUpdated += new PersistentItemEventHandler(EditLanguageDialog1_ItemUpdated);
       // EditLanguageDialog1.Cancelled += new EventHandler(EditLanguageDialog1_Cancelled);
    }


    private void AddLanguageDialog1_ItemAdded(object sender, PersistentItemEventArgs e)
    {
        this.LanguageGrid.DataBind();
        this.LanguageAjax.Update();
    }

    private void EditLanguageDialog1_ItemUpdated(object sender, PersistentItemEventArgs e)
    {
        Update = true;
        FinishEdit(true);
    }


    private void FinishEdit(bool rebind)
    {
        LanguageGrid.EditIndex = -1;
        if (rebind)
        {
            LanguageGrid.DataBind();

            LanguageAjax.Update();
        }
        
    }

    private void EditLanguageDialog1_Cancelled(object sender, EventArgs e)
    {
        Update = false;

        FinishEdit(false);
    }


    protected void CurrencyGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Delete")
        {
            string criteria = string.Format(@"LanguageId = {0}", AlwaysConvert.ToInt(e.CommandArgument));
            LanguageTranslationCollection languages = LanguageTranslationDataSource.LoadForCriteria(criteria);
          
           if((languages.Count > 0)&&(languages[0]!=null))
           {
                 LanguageTranslation language = languages[0];

                try
                {
                    language.Delete();
                    FinishEdit(true); 
                }
                catch (Exception ex)
                {
                    // Throw the exception with proper error message
                    throw new Exception("Unable to update the exchange rate.  The rate provider may be experiencing technical difficulties, please try again later.  Exception message: " + ex.Message, ex);
                }
            }
        }
    }


    protected void LanguageGrid_RowEditing(object sender, GridViewEditEventArgs e)
    {
        int languageId = (int)LanguageGrid.DataKeys[e.NewEditIndex].Value;
        string criteria = string.Format(@"LanguageId = {0}", languageId);

        LanguageTranslationCollection languages = LanguageTranslationDataSource.LoadForCriteria(criteria);
        if (languages != null)
        {
            ASP.admin_store_regions_editlanguagedialog_ascx editDialog = LanguageAjax.FindControl("EditLanguageDialog1") as ASP.admin_store_regions_editlanguagedialog_ascx;
            if (editDialog != null) editDialog.LanguageId = languageId;
        }
    }


    protected string GetLanguage(object dataItem)
    {

        LanguageTranslation language = (LanguageTranslation)dataItem;

        return string.Format(@"({0})", language.Culture);

    }


    protected void Page_Load(object sender, EventArgs e)
    {

   
        
    }
}
