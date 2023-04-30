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
using MakerShop.Stores;
using MakerShop.Utility;
using MakerShop.Common;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;


public partial class Admin_Store_Regions_AddLanguageDialog : System.Web.UI.UserControl
{
    public event PersistentItemEventHandler ItemAdded;
   

  //  protected void Page_Load(object sender, EventArgs e)
  //  {
   //     object test = ItemAdded;
  //      if (!Page.IsPostBack)
  //      {
   //         GetLanguageList();
   //         FieldName.Text = string.Empty;
   //         FieldName.Focus();
   //         FieldValue.Text = string.Empty;
   //         Comment.Text = string.Empty;
   //         Culturedl.SelectedIndex = 0;
   //     }
  //  }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        GetLanguageList();

        FieldAddName.Text = string.Empty;
        FieldAddValue.Text = string.Empty;
        AddComment.Text = string.Empty;
        CultureAdddl.SelectedIndex = 0;
    }



     public struct keys
    {
        public string Name;
        public string Iso;

        public keys(string name, string iso)
        {
            Name = name;
            Iso = iso;
        }
    }



    private void GetLanguageList()
    {
        CultureAdddl.Items.Clear();

        List<keys> cultureList = new List<keys>();
        ListItemCollection Languages = new ListItemCollection();
        CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.NeutralCultures | CultureTypes.SpecificCultures);

        foreach (CultureInfo culture in cultures)
        {
            if (culture.Name == string.Empty)
                continue;

            if (!(cultureList.Contains(new keys(culture.Name, culture.Name))))
                cultureList.Add(new keys(culture.Name, culture.Name));
        }


        var cultureListSorted = from cul in cultureList orderby cul.Name select cul;
        foreach (keys key in cultureListSorted)
        {
            if (key.Iso == "en")
                CultureAdddl.Items.Insert(0, new ListItem(key.Name, key.Iso));

            if(key.Iso.Length <6)
                CultureAdddl.Items.Add(new ListItem(key.Name, key.Iso));
        }

        CultureAdddl.Items.Insert(0, new ListItem("-Select Language-", ""));

    }

    protected void CreateTranslation()
    {
       // if ((Page.IsValid)&&(CultureAdddl.SelectedIndex!=0))

         if (Page.IsValid) 
        {
            LanguageTranslation translation = new LanguageTranslation();

            translation.FieldName = FieldAddName.Text;
            translation.FieldValue = FieldAddValue.Text; 
            translation.Comment = AddComment.Text;

            ListItem cultureSelected = CultureAdddl.SelectedItem;
            translation.Culture = CultureAdddl.SelectedValue;
            try
            {
                translation.Save();
                //RESET FORM
                FieldAddName.Text = string.Empty;
                FieldAddName.Focus();
                FieldAddValue.Text = string.Empty;
                AddComment.Text = string.Empty;
                CultureAdddl.SelectedIndex = 0;
                SavedMessage.Visible = true;
                ErrorMessage.Visible = false;

              
            }
            catch (Exception e)
            {
                ErrorMessage.Font.Size = FontUnit.XSmall;
                ErrorMessage.Text = e.Message;
                ErrorMessage.Visible = true;
                SavedMessage.Visible = false;
            }

            if (ItemAdded != null) ItemAdded(this, new PersistentItemEventArgs(translation.LanguageId, translation.FieldName));

        }
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        CreateTranslation();
    }


    protected void CancelButton_Click(object sender, EventArgs e)
    {
        FieldAddName.Text = string.Empty;
        FieldAddName.Focus();
        FieldAddValue.Text = string.Empty;
        AddComment.Text = string.Empty;
        CultureAdddl.SelectedIndex = 0;
    }
}
