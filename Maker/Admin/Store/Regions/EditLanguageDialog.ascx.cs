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

public partial class Admin_Store_Regions_EditLanguageDialog : System.Web.UI.UserControl
{

    public event PersistentItemEventHandler ItemUpdated;
    public event EventHandler Cancelled;

   

    public int LanguageId
    {
        get { return AlwaysConvert.ToInt(ViewState["LanguageId"]); }
        set { ViewState["LanguageId"] = value; }
    }

    protected void UpdateLanguage()
    {
        if (Page.IsValid)
        {
            string criteria = string.Format(@"LanguageId = {0}", this.LanguageId);

            LanguageTranslationCollection languages = LanguageTranslationDataSource.LoadForCriteria(criteria);

            if ((languages.Count > 0) && (languages[0] != null))
            {
                LanguageTranslation language = languages[0];
                language.Comment = this.Comment.Text;
                language.Culture = Culturedl.SelectedValue;
                language.FieldName = this.FieldName.Text;
                language.FieldValue = this.FieldValue.Text;

                language.Save();

                //RESET FORM
                this.Comment.Text = string.Empty;
                Culturedl.SelectedValue = string.Empty;
                this.FieldName.Text = string.Empty;
                this.FieldValue.Text = string.Empty;



  
            if (ItemUpdated != null) ItemUpdated(this, new PersistentItemEventArgs(this.LanguageId, language.FieldName));

            }
            // RESET LANGUAGE ID
            this.LanguageId = 0;
        }
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        UpdateLanguage();
    }


    protected void CancelButton_Click(object sender, EventArgs e)
    {
        // RESET THE LANGUAGE ID
        this.LanguageId = 0;

        this.Comment.Text = string.Empty;
        Culturedl.SelectedIndex = 0;
        this.FieldName.Text = string.Empty;
        this.FieldValue.Text = string.Empty;
       

        //TRIGER ANY EVENT ATTACHED TO THE CANCEL
        if (Cancelled != null) Cancelled(sender, e);
    }




    protected void Page_PreRender(object sender, EventArgs e)
    {
        GetLanguageList();

        string criteria = string.Format(@"LanguageId = {0}", this.LanguageId);

        LanguageTranslationCollection languages = LanguageTranslationDataSource.LoadForCriteria(criteria);

        if((languages.Count > 0)&&(languages[0]!=null))
        {
            LanguageTranslation language = languages[0];

            string isCulture = language.Culture;
            string isKey = language.FieldName;

            EditCaption.Text = string.Format(EditCaption.Text, language.Culture);
            
            //LOAD FORM
            this.FieldName.Text = language.FieldName;
            this.FieldValue.Text = language.FieldValue;
            this.Comment.Text = language.Comment;
            Culturedl.SelectedValue = language.Culture;

            //  ListItem selectedCulture = Culture.Items.FindByValue(Token.Instance.Store.DefaultWarehouse.CountryCode);
            //  if (!String.IsNullOrEmpty(language.Culture)) selectedCulture = Culture.Items.FindByValue(language.Culture);
            //  if (selectedCulture != null) Culture.SelectedIndex = Culture.Items.IndexOf(selectedCulture);

            EditPopup.OnCancelScript = String.Format("__doPostBack('{0}','{1}')", CancelButton.UniqueID, "");
            EditPopup.Show();
          
        }
        else
        {
            this.Controls.Clear();
        }
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
        Culturedl.Items.Clear();

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
                Culturedl.Items.Insert(0, new ListItem(key.Name, key.Iso));

            if (key.Iso.Length < 6)
                Culturedl.Items.Add(new ListItem(key.Name, key.Iso));
        }

        Culturedl.Items.Insert(0, new ListItem("-Select Language-", ""));

    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }
}
