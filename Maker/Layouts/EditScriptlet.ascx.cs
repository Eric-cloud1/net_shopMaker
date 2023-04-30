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

public partial class Layouts_EditScriptlet : System.Web.UI.UserControl
{
    //DEFINE AN EVENT TO TRIGGER WHEN AN ITEM IS ADDED 
    public event EventHandler Saved;
    public event EventHandler Cancelled;

    private Scriptlet _Scriptlet;

    public string Identifier
    {
        get
        {
            if (ViewState["Identifier"] == null) return string.Empty;
            return (string)ViewState["Identifier"];
        }
        set { ViewState["Identifier"] = value; }
    }

    public ScriptletType ScriptletType
    {
        get
        {
            if (ViewState["ScriptletType"] == null) return ScriptletType.Unspecified;
            return (ScriptletType)ViewState["ScriptletType"];
        }
        set { ViewState["ScriptletType"] = value; }
    }

    private Scriptlet Scriptlet
    {
        get
        {
            if ((_Scriptlet == null) && (!string.IsNullOrEmpty(this.Identifier)) && (this.ScriptletType != ScriptletType.Unspecified))
                _Scriptlet = ScriptletDataSource.Load(this.Page.Theme, this.Identifier, this.ScriptletType);
            return _Scriptlet;
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        bool scriptletFound = (this.Scriptlet != null);
        if (scriptletFound)
        {
            Caption.Text = string.Format(Caption.Text, _Scriptlet.ScriptletType.ToString(), _Scriptlet.Identifier);
            MoreButton.NavigateUrl = this.Page.ResolveUrl(string.Format("~/Admin/Website/Scriptlets/EditScriptlet.aspx?s={0}&t={1}", _Scriptlet.Identifier, _Scriptlet.ScriptletType.ToString()));
            PageHelper.SetHtmlEditor(ScriptletData, WysiwygButton);
            if (ViewState["Initialized"] == null)
            {
                this.ScriptletData.Text = _Scriptlet.ScriptletData;
                ViewState["Initialized"] = true;
            }
        }
        else
        {
            //HIDE THIS CONTROL IF THERE IS NO SCRIPTLET
            this.Controls.Clear();
        }
    }

    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (this.Scriptlet != null)
        {
            _Scriptlet.ScriptletData = ScriptletData.Text;
            if (_Scriptlet.IsDirty) _Scriptlet.IsCustom = true;
            _Scriptlet.Save();
            SavedMessage.Text = string.Format(SavedMessage.Text, LocaleHelper.LocalNow);
            SavedMessage.Visible = true;
            if (Saved != null) Saved(this, new EventArgs());
        }
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        ViewState["Identifier"] = null;
        ViewState["ScriptletType"] = null;
        if (Cancelled != null) Cancelled(this, new EventArgs());
    }
}
