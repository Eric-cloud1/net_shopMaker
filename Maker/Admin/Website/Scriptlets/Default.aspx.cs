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

public partial class Admin_Website_Scriptlets_Default : MakerShop.Web.UI.MakerShopAdminPage
{
    StoreSettingCollection _Settings;
    private string _SelectedTheme = string.Empty;

    public string SelectedTheme
    {
        get { return _SelectedTheme; }
        set { _SelectedTheme = value; }
    }

    private void RebindGrid()
    {
        ScriptletGrid.DataBind();
        ScriptletAjax.Update();
    }

    protected void ScriptletGrid_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Copy")
        {
            string[] selected = e.CommandArgument.ToString().Split(":".ToCharArray());
            Scriptlet copy = Scriptlet.Copy(_SelectedTheme, selected[1].Trim(), (ScriptletType)Enum.Parse(typeof(ScriptletType), selected[0]));
            copy.Save();
            Response.Redirect("EditScriptlet.aspx?s=" + copy.Identifier + "&t=" + copy.ScriptletType.ToString() + "&tid=" + _SelectedTheme);
        } else if (e.CommandName == "DoDelete")
        {
            string[] selected = e.CommandArgument.ToString().Split(":".ToCharArray());
            Scriptlet delete = ScriptletDataSource.Load(_SelectedTheme, selected[1].Trim(), (ScriptletType)Enum.Parse(typeof(ScriptletType), selected[0]));
            if (delete != null && delete.IsCustom)
            {
                if (!delete.Delete())
                {
                    ErrorMessage.Text = "An error has occured while deleting scriptlet, for details check the <a href='../../Help/ErrorLog.aspx'>error log</a>.";
                    ErrorMessage.Visible = true;
                }
                else
                {
                    ScriptletDataSource.ClearCache(_SelectedTheme);
                    RebindGrid();
                }
            }
        }
    }

    protected void NewScriptletButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            ScriptletType newScriptletType = (ScriptletType)Enum.Parse(typeof(ScriptletType), NewScriptletType.SelectedValue);
            string newScriptletIdentifier = NewScriptletName.Text.Trim();
            Scriptlet scriptlet = ScriptletDataSource.Load(_SelectedTheme, newScriptletIdentifier, newScriptletType, false);
            if (scriptlet != null)
            {
                CustomValidator uniqueValidator = new CustomValidator();
                uniqueValidator.ControlToValidate = "NewScriptletName";
                uniqueValidator.ValidationGroup = "Add";
                uniqueValidator.IsValid = false; 
                uniqueValidator.Text = "*";
                uniqueValidator.ErrorMessage = "A " + newScriptletType.ToString() + " scriptlet with that name already exists. Name must be unique.";
                NewScriptletNameUnique.Controls.Add(uniqueValidator);
            }
            else
            {
                scriptlet = new Scriptlet();
                scriptlet.ThemeId = _SelectedTheme;
                scriptlet.ScriptletType = newScriptletType;
                scriptlet.Identifier = newScriptletIdentifier;
                scriptlet.IsCustom = true;
                scriptlet.Save();
                Response.Redirect("EditScriptlet.aspx?tid=" + _SelectedTheme + "&s=" + scriptlet.Identifier + "&t=" + scriptlet.ScriptletType.ToString());
            }
        }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            // INITIALIZE SORTING FROM COOKIES
            HttpCookie sortExpressionCookie = Request.Cookies[Page.UniqueID + ":SortExpression"];
            HttpCookie sortDirectionCookie = Request.Cookies[Page.UniqueID + "SortDirection"];

            if (sortDirectionCookie != null && sortExpressionCookie != null)
            {
                ScriptletGrid.DefaultSortExpression = sortExpressionCookie.Value;
                ScriptletGrid.DefaultSortDirection = (SortDirection)Enum.Parse(typeof(SortDirection), sortDirectionCookie.Value);
            }
        }
    }

    protected void ScriptletGrid_Sorting(object sender, GridViewSortEventArgs e)
    {
        if (Request.Form["__EVENTTARGET"] == ScriptletGrid.UniqueID)
        {
            // IF SORTING IS INITIATED            
            HttpCookie sortExpressionCookie = new HttpCookie(Page.UniqueID + ":SortExpression");
            sortExpressionCookie.Value = e.SortExpression;
            HttpCookie sortDirectionCookie = new HttpCookie(Page.UniqueID + "SortDirection");
            sortDirectionCookie.Value = e.SortDirection.ToString();


            Response.Cookies.Add(sortExpressionCookie);
            Response.Cookies.Add(sortDirectionCookie);
        }
    }
    

    protected void Page_Load(object sender, EventArgs e)
    {
        _Settings = Token.Instance.Store.Settings;

        if (!Page.IsPostBack)
        {
            _SelectedTheme = Request.QueryString["tid"];
            if (string.IsNullOrEmpty(_SelectedTheme))
            {
                _SelectedTheme = _Settings.StoreTheme;
            }

            BindThemes();

            ListItem selectedItem = ThemeDropdown.Items.FindByValue(_SelectedTheme);
            if (selectedItem != null) selectedItem.Selected = true;
        }
        else
        {
            _SelectedTheme = ThemeDropdown.SelectedValue;
        }        
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        UsingDefaultLabel.Text = string.Format("The selected theme <font color='red'>{0}</font> uses shared scriptlets from <font color='red'>App_Data/Scriptlets</font> folder. <br/>To use custom scriptlets for this theme, copy the <font color='red'>Scriptlets</font> folder from <font color='red'>App_Data</font> to <font color='red'>App_Themes/{1}/</font> folder.", SelectedTheme, SelectedTheme);
        if (string.IsNullOrEmpty(SelectedTheme) || ThemeHasScriptlets(SelectedTheme))
        {
            System.IO.FileInfo fileinfo;
            // PERFORM A WRITE PERMISSION TEST
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            Exception testException = null;
            if (System.IO.Directory.Exists(System.IO.Path.Combine(baseDirectory, "App_Themes\\" + SelectedTheme + "\\Scriptlets\\Custom")))
            {
                String fileName = System.IO.Path.Combine(baseDirectory, "App_Themes/" + SelectedTheme + "/Scriptlets/Custom/DELETEME.TXT");                
                testException = FileHelper.CanCreateFile(fileName, "WRITE PERMISSIONS TEST - THIS FILE IS SAFE TO DELETE");
                if (testException == null)
                {
                    testException = FileHelper.CanDeleteFile(fileName);
                }
            }
            if (testException != null)
            {
                phUsingDefault.Visible = true;
                UsingDefaultLabel.Visible = false;
                WriteAccessErrorMessage.Visible = true;
                WriteAccessErrorMessage.Text = "To properly function scriptlet manager requires write access for \"~App_Themes/" + SelectedTheme + "/Scriptlets/Custom/\" directory and sub-directories.";
            }
            else
            {                
                phUsingDefault.Visible = false;
            }
        }
        else
        {
            phUsingDefault.Visible = true;
        }
    }

    protected void BindThemes()
    {
        List<MakerShop.UI.Styles.Theme> themes = MakerShop.UI.Styles.ThemeDataSource.Load();
        foreach (MakerShop.UI.Styles.Theme theme in themes)
        {
            if (!theme.IsAdminTheme)
            {
                //THIS IS A STORE THEME
                ThemeDropdown.Items.Add(new ListItem(theme.DisplayName, theme.Name));
            }
        }
    }

    protected void ScriptletDs_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
        e.InputParameters["themeId"] = _SelectedTheme;
    }

    protected void ThemeDropdown_SelectedIndexChanged(object sender, EventArgs e)
    {
        RebindGrid();
    }

    protected bool ThemeHasScriptlets(string themeName)
    {
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        if (string.IsNullOrEmpty(themeName)) return false;
        if (System.IO.Directory.Exists(System.IO.Path.Combine(baseDirectory, "App_Themes\\" + themeName + "\\Scriptlets")))
        {
            return true;
        }
        return false;
    }

}
