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
using MakerShop.Common;
using MakerShop.Stores;
using MakerShop.Products;
using MakerShop.Utility;
using MakerShop.Users;
using MakerShop.Personalization;
using MakerShop.Web.UI.WebControls.WebParts;
using MakerShop.Web;

public partial class Layouts_AdminPanel : System.Web.UI.UserControl
{
    WebPartManager _manager;
    ICatalogable _Catalogable = null;
    CatalogNodeType _CatalogableType = CatalogNodeType.Category;
    bool _IsEditMode = false;

    protected void Page_Init(object sender, EventArgs e)
    {
        if (Token.Instance.User.IsInRole(Role.WebsiteAdminRoles))
        {
            //this.Page.InitComplete += new EventHandler(Page_InitComplete);
            InitializePage();
        }
        else
        {
            this.Controls.Clear();
        }
    }

    private bool _PageInitialized = false;
    private void InitializePage()
    {
        if (!_PageInitialized)
        {
            //GET THE WEBPART MANAGER
            _manager = WebPartManager.GetCurrentWebPartManager(this.Page);

            //INITIALIZE THE PAGE PATH
            PagePath.Text = Request.AppRelativeCurrentExecutionFilePath;
            PagePath2.Text = string.Format(PagePath2.Text, PagePath.Text);

            //DECIDE WHETHER RESET BUTTON IS VISIBLE
            UserPersonalization up = UserPersonalizationDataSource.LoadForPath(Request.AppRelativeCurrentExecutionFilePath, Context.User.Identity.Name, false);
            SharedPersonalization sp = SharedPersonalizationDataSource.LoadForPath(Request.AppRelativeCurrentExecutionFilePath, false);
            ResetPagePanel.Visible = ((up != null) || (sp != null));

            //BIND THE BROWSE / EDIT PANELS
            BindBrowsePanel();
            BindEditPanel();            
        }
        _PageInitialized = true;
    }

    private void BindEditPanel()
    {
        //IF THIS IS NOT A CATALOGABLE PAGE, WE SHOW THE PAGE THEME
        PageHelper.FindCatalogable(out _Catalogable, out _CatalogableType);
        BindThemes();
        BindDisplayPages();
        EditScriptlet.Saved += new EventHandler(EditScriptlet_Saved);
        EditScriptlet.Cancelled += new EventHandler(EditScriptlet_Cancelled);                
    }

    #region Themes
    protected void BindThemes()
    {
        List<MakerShop.UI.Styles.Theme> allThemes = MakerShop.UI.Styles.ThemeDataSource.Load();
        foreach (MakerShop.UI.Styles.Theme theme in allThemes)
        {
            if (!theme.IsAdminTheme)
            {
                PageTheme.Items.Add(new ListItem(theme.DisplayName, theme.Name));
                CatalogTheme.Items.Add(new ListItem(theme.DisplayName, theme.Name));
            }
        }
        if (_Catalogable != null)
        {
            //BIND THE CATALOG THEME
            trPageTheme.Visible = false;
            trCatalogTheme.Visible = true;
            ListItem selectedItem = CatalogTheme.Items.FindByValue(_Catalogable.Theme);
            if (selectedItem != null) CatalogTheme.SelectedIndex = CatalogTheme.Items.IndexOf(selectedItem);
            CatalogName.Text = string.Format(CatalogName.Text, _Catalogable.Name);
        }
        else
        {
            //BIND THE PAGE THEME
            trCatalogTheme.Visible = false;
            trPageTheme.Visible = true;
            SharedPersonalization personalization = SharedPersonalizationDataSource.LoadForPath(Request.AppRelativeCurrentExecutionFilePath, false);
            if (personalization != null)
            {
                ListItem selectedItem = PageTheme.Items.FindByValue(personalization.Theme);
                if (selectedItem != null) PageTheme.SelectedIndex = PageTheme.Items.IndexOf(selectedItem);
            }
        }
    }

    protected void SaveThemeButton_Click(object sender, EventArgs e)
    {
        bool _RequireRedirect = false;
        if (_Catalogable != null)
        {
            _Catalogable.Theme = CatalogTheme.SelectedValue;
            switch (_CatalogableType)
            {
                case CatalogNodeType.Category:
                    ((Category)_Catalogable).Save();
                    break;
                case CatalogNodeType.Product:
                    ((Product)_Catalogable).Save();
                    break;
                case CatalogNodeType.Webpage:
                    ((Webpage)_Catalogable).Save();
                    break;
                case CatalogNodeType.Link:
                    ((Link)_Catalogable).Save();
                    break;
            }
            _RequireRedirect = true;
        }
        else
        {
            //UPDATE THE PAGE THEME
            SharedPersonalization personalization = SharedPersonalizationDataSource.LoadForPath(Request.AppRelativeCurrentExecutionFilePath, true);
            personalization.Theme = PageTheme.SelectedValue;
            if (personalization.IsDirty)
            {
                personalization.Save();
                WebflowManager.ClearCache();
                _RequireRedirect = (_RequireRedirect || true);
            }
        }
        //IF DISPLAY PAGE PANEL IS VISIBLE, TRY TO UPDATE VALUES
        if ((trDisplayPage.Visible) && (_Catalogable != null))
        {
            _RequireRedirect = (UpdateDisplayPage(_Catalogable, _CatalogableType) || _RequireRedirect);
        }
        //RETURN VALUE INDICATING WHETHER REDIRECT IS REQUIRED
        if (_RequireRedirect) RefreshPage(true);
    }
    #endregion

    #region Display Pages
    protected void BindDisplayPages()
    {
        trDisplayPage.Visible = (_Catalogable != null);
        if (trDisplayPage.Visible)
        {
            //A NODE WAS FOUND, 
            List<ListItem> categoryPages = new List<ListItem>();
            List<ListItem> productPages = new List<ListItem>();
            List<ListItem> webpagePages = new List<ListItem>();
            //GET THE DISPLAY PAGE COLLECTIONS
            List<MakerShop.UI.Styles.DisplayPage> displayPages = MakerShop.UI.Styles.DisplayPageDataSource.Load();
            foreach (MakerShop.UI.Styles.DisplayPage displayPage in displayPages)
            {
                string displayName = string.Format("{0} ({1})", displayPage.Name, displayPage.DisplayPageFile);
                switch (displayPage.NodeType)
                {
                    case CatalogNodeType.Category:
                        categoryPages.Add(new ListItem(displayName, displayPage.DisplayPageFile));
                        break;
                    case CatalogNodeType.Product:
                        productPages.Add(new ListItem(displayName, displayPage.DisplayPageFile));
                        break;
                    case CatalogNodeType.Webpage:
                        webpagePages.Add(new ListItem(displayName, displayPage.DisplayPageFile));
                        break;
                }
            }

            //TAKE APPROPRIATE ACTION BASED ON NODE TYPE
            ListItem item;
            switch (_CatalogableType)
            {
                case CatalogNodeType.Category:
                    CurrentDisplayPageName.Text = string.Format(CurrentDisplayPageName.Text, "category");
                    CurrentDisplayPage.Items.Clear();
                    CurrentDisplayPage.Items.Add(new ListItem("Use Store Default", string.Empty));
                    AddListItems(CurrentDisplayPage, categoryPages);
                    break;
                case CatalogNodeType.Product:
                    CurrentDisplayPageName.Text = string.Format(CurrentDisplayPageName.Text, "product");
                    CurrentDisplayPage.Items.Clear();
                    CurrentDisplayPage.Items.Add(new ListItem("Use Store Default", string.Empty));
                    AddListItems(CurrentDisplayPage, productPages);
                    break;
                case CatalogNodeType.Webpage:
                    CurrentDisplayPageName.Text = string.Format(CurrentDisplayPageName.Text, "webpage");
                    CurrentDisplayPage.Items.Clear();
                    CurrentDisplayPage.Items.Add(new ListItem("Use Store Default", string.Empty));
                    AddListItems(CurrentDisplayPage, webpagePages);
                    break;
            }
            item = CurrentDisplayPage.Items.FindByValue(_Catalogable.DisplayPage);
            if (item != null) CurrentDisplayPage.SelectedIndex = CurrentDisplayPage.Items.IndexOf(item);
        }
    }

    private void AddListItems(DropDownList list, List<ListItem> items)
    {
        foreach (ListItem item in items)
        {
            list.Items.Add(new ListItem(item.Text, item.Value));
        }
    }

    private bool UpdateDisplayPage(ICatalogable node, CatalogNodeType nodeType)
    {
        bool updated = false;
        switch (nodeType)
        {
            case CatalogNodeType.Category:
                if (node.DisplayPage != CurrentDisplayPage.SelectedValue)
                {
                    ((Category)node).DisplayPage = CurrentDisplayPage.SelectedValue;
                    ((Category)node).Save();
                    updated = true;
                }
                break;
            case CatalogNodeType.Product:
                if (node.DisplayPage != CurrentDisplayPage.SelectedValue)
                {
                    ((Product)node).DisplayPage = CurrentDisplayPage.SelectedValue;
                    ((Product)node).Save();
                    updated = true;
                }
                break;
            case CatalogNodeType.Webpage:
                if (node.DisplayPage != CurrentDisplayPage.SelectedValue)
                {
                    ((Webpage)node).DisplayPage = CurrentDisplayPage.SelectedValue;
                    ((Webpage)node).Save();
                    updated = true;
                }
                break;
        }
        return updated;
    }

    #endregion

    #region Edit Scriptlets
    void EditScriptlet_Cancelled(object sender, EventArgs e)
    {
        RefreshPage(false);
    }

    void EditScriptlet_Saved(object sender, EventArgs e)
    {
        RefreshPage(true);
    }
    #endregion

    #region Edit Links
    private void BindBrowsePanel()
    {
        //CHECK FOR CATEGORY
        if (!string.IsNullOrEmpty(Request.QueryString["CategoryId"])) ShowEditCategoryLink(PageHelper.GetCategoryId());
        //CHECK FOR PRODUCT
        int productId = PageHelper.GetProductId();
        Product product = ProductDataSource.Load(productId);
        if (product != null)
        {
            EditProductPanel.Visible = true;
            EditProductLink.Text = product.Name;
            int categoryId = PageHelper.GetCategoryId();
            EditProductLink.NavigateUrl += "?CategoryId=" + categoryId + "&ProductId=" + productId.ToString();
            ShowEditCategoryLink(categoryId);
        }
        //CHECK FOR WEBPAGE
        int webpageId = PageHelper.GetWebpageId();
        Webpage webpage = WebpageDataSource.Load(webpageId);
        if (webpage != null)
        {
            EditWebpagePanel.Visible = true;
            EditWebpageLink.Text = webpage.Name;
            int categoryId = PageHelper.GetCategoryId();
            EditWebpageLink.NavigateUrl += "?CategoryId=" + categoryId + "&WebpageId=" + webpageId.ToString();
            ShowEditCategoryLink(categoryId);
        }
        //CHECK FOR LINK
        int linkId = PageHelper.GetLinkId();
        Link link = LinkDataSource.Load(linkId);
        if (link != null)
        {
            EditLinkPanel.Visible = true;
            EditLinkLink.Text = link.Name;
            int categoryId = PageHelper.GetCategoryId();
            EditLinkLink.NavigateUrl += "?CategoryId=" + categoryId + "&LinkId=" + linkId.ToString();
            ShowEditCategoryLink(categoryId);
        }
    }

    private void ShowEditCategoryLink(int categoryId)
    {
        if (!EditCategoryPanel.Visible)
        {
            Category category;
            if (categoryId != 0) category = CategoryDataSource.Load(categoryId);
            else
            {
                category = new Category();
                category.Name = "Home";
            }
            if (category != null)
            {
                EditCategoryPanel.Visible = true;
                EditCategoryLink.Text = category.Name;
                EditCategoryLink.NavigateUrl += "?CategoryId=" + categoryId.ToString();
            }
        }
    }
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (_manager != null)
        {
            //SEE IF THE EDITOR ZONE CLOSE BUTTON WAS PRESSED
            if ((Request.Form["__EVENTTARGET"] == EditorZone1.UniqueID) && 
                (Request.Form["__EVENTARGUMENT"] == "headerClose" ||
                 Request.Form["__EVENTARGUMENT"] == "cancel"
                ))
            {
                //REDIRECT BACK TO THIS PAGE TO END THE EDITING
                //(JUST DISABLING EDIT MODE CAUSES AN UNCATCHABLE EXCEPTION)
                RefreshPage(false);
            }          
            //FIGURE OUT WHAT MODE WE ARE IN (EDIT OR BROWSE)
            _IsEditMode = ((Request.Form[CurrentMode.UniqueID] == "Edit") || ((string)Context.Items[this.GetType().ToString()] == "Edit") || ((!Page.IsPostBack && AlwaysConvert.ToInt(Request.QueryString["EditMode"]) == 1)));
            //UPDATE THE CURRENT PAGE MODE
            if (_IsEditMode)
            {
                _manager.DisplayMode = _manager.SupportedDisplayModes["Edit"];
                if (_manager.Personalization.Scope == PersonalizationScope.User)
                {
                    Context.Items[this.GetType().ToString()] = "Edit";
                    _manager.Personalization.ToggleScope();
                }
            }
            else
            {
                _manager.DisplayMode = _manager.SupportedDisplayModes["Browse"];
            }
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        Trace.Write(this.GetType().ToString(), "Begin PreRender");
        //if we are rendering as the result of a postback
        //and the edit panel is already visible,
        //we should redirect back to the page to make any changes
        //to the layout scriptlet visible
        if (_manager != null)
        {
            //HIDE THE WEBPART CHROME
            string style = @"<style> .ctl00_PageContent_WebPartZone1_0 { border: none !important; } .ctl00_PageContent_WebPartZone1_2 { display: none; } </style>";
            this.Page.Header.Controls.Add(new LiteralControl(style));

            // IF OK BUTTON IS CLICKED THEN WE NEED TO HIDE THE EDITOR, CHANGES MAY HAVE ALREADY BEEN APPLIED
            if ((Request.Form["__EVENTTARGET"] == EditorZone1.UniqueID) &&
                (Request.Form["__EVENTARGUMENT"] == "ok"))
            {
                _manager.DisplayMode = _manager.SupportedDisplayModes["Browse"];
            }


            //IF EDIT MODE, MAKE SURE A SCRIPTLET IS EDITED
            if (_manager.DisplayMode == _manager.SupportedDisplayModes["Edit"])
            {
                //UPDATE PANEL VISIBILITY
                trBrowsePanel.Visible = false;
                trEditPanel.Visible = true;
                CurrentMode.SelectedIndex = 1;
                //EDIT THE FIRST SCRIPTLET PART (IF FOUND)
                Control[] scriptlets = PageHelper.FindControls(this.Page, typeof(ScriptletPart));
                if ((scriptlets != null) && (scriptlets.Length > 0))
                {
                    ScriptletPart targetScriptlet = (ScriptletPart)scriptlets[0];
                    try
                    {
                        _manager.BeginWebPartEditing(targetScriptlet);
                    }
                    catch (ArgumentException) { }
                }
            }
            else
            {
                //UPDATE PANEL VISIBILITY
                trBrowsePanel.Visible = true;
                trEditPanel.Visible = false;
                CurrentMode.SelectedIndex = 0;
            }
        }
        Trace.Write(this.GetType().ToString(), "End PreRender");
    }

    protected void ResetPage_Click(object sender, EventArgs e)
    {
        bool refreshRequired = false;
        UserPersonalization up = UserPersonalizationDataSource.LoadForPath(Request.AppRelativeCurrentExecutionFilePath, Context.User.Identity.Name, false);
        if (up != null)
        {
            up.Delete();
            refreshRequired = true;
        }
        SharedPersonalization sp = SharedPersonalizationDataSource.LoadForPath(Request.AppRelativeCurrentExecutionFilePath, false);
        if (sp != null)
        {
            sp.Delete();
            MakerShop.Web.WebflowManager.ClearCache();
            refreshRequired = true;
        }
        //REDIRECT TO RELOAD WITHOUT PERSONALIZATION
        if (refreshRequired) RefreshPage(false);
    }

    //WE NEED TO DO THIS TO REFRESH THE THEME, SCRIPTLET UPDATES, ETC.
    private void RefreshPage(bool editMode)
    {
        string myUrl;
        if (_Catalogable != null) myUrl = _Catalogable.NavigateUrl;
        else myUrl = Request.Url.ToString();
        if (editMode)
        {
            if (!myUrl.Contains("EditMode="))
            {
                if (!myUrl.Contains("?"))
                {
                    myUrl += "?EditMode=1";
                }
                else
                {
                    myUrl += "&EditMode=1";
                }
            }
        }
        else
        {
            myUrl = myUrl.Replace("?EditMode=1", string.Empty);
            myUrl = myUrl.Replace("&EditMode=1", string.Empty);
        }
        Response.Redirect(myUrl);
    }
}
