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

public partial class Admin_Website_Scriptlets_EditScriptlet : MakerShop.Web.UI.MakerShopAdminPage
{
    private string _ThemeId;
    private string _Identifier;
    private ScriptletType _ScriptletType;
    private Scriptlet _Scriptlet;

    protected void Page_Init(object sender, EventArgs e)
    {
        this.PreserveWhitespace = true;
        _Identifier = StringHelper.RemoveSpecialChars(Request.QueryString["s"]);
        if (string.IsNullOrEmpty(_Identifier)) RedirectMe();
        try
        {
            _ScriptletType = (ScriptletType)Enum.Parse(typeof(ScriptletType), Request.QueryString["t"], true);
        }
        catch (ArgumentException)
        {
            _ScriptletType = ScriptletType.Unspecified;
        }
        if (_ScriptletType == ScriptletType.Unspecified) RedirectMe();

        _ThemeId = StringHelper.RemoveSpecialChars(Request.QueryString["tid"]);
        if (string.IsNullOrEmpty(_ThemeId)) _ThemeId = string.Empty;

        _Scriptlet = ScriptletDataSource.Load(_ThemeId, _Identifier, _ScriptletType, false);

        _Scriptlet = ScriptletDataSource.Load(_ThemeId, _Identifier, _ScriptletType, false);
        if (_Scriptlet == null) RedirectMe();
        Caption.Text = string.Format(Caption.Text, _ScriptletType.ToString(), _Identifier);
        PageHelper.SetHtmlEditor(ScriptletData, ScriptletDataHtml);
        if (!Page.IsPostBack)
        {
            Identifier.Text = _Scriptlet.Identifier;
            Description.Text = _Scriptlet.Description;
            HeaderData.Text = _Scriptlet.HeaderData;
            ScriptletData.Text = _Scriptlet.ScriptletData;
        }
        PageHelper.OpenPopUp(ConLibHelpLink, "ConLibHelp.aspx", "ConLibHelp", 500, 400, "toolbar=0,scrollbars=1,location=0,statusbar=1,menubar=0,resizable=1");
    }
    
    private void RedirectMe()
    {
        Response.Redirect("Default.aspx?tid=" + Server.UrlEncode(_ThemeId));
    }
    
    protected void SaveButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            //IF NAME HAS CHANGED WE MUST VERIFY IT IS NOT A DUPLICATE
            string updatedIdentifier = Identifier.Text.Trim();
            bool validIdentifier = true;
            if (_Scriptlet.Identifier.ToLowerInvariant() != updatedIdentifier.ToLowerInvariant())
            {
                Scriptlet scriptlet = ScriptletDataSource.Load(_ThemeId, updatedIdentifier, _ScriptletType, BitFieldState.True, false);
                if (scriptlet != null)
                {
                    CustomValidator uniqueValidator = new CustomValidator();
                    uniqueValidator.ControlToValidate = "Identifier";
                    uniqueValidator.IsValid = false;
                    uniqueValidator.Text = "*";
                    uniqueValidator.ErrorMessage = "A scriptlet with that name already exists. Name must be unique.";
                    IdentifierUnique.Controls.Add(uniqueValidator);
                    validIdentifier = false;
                }
            }
            if (validIdentifier)
            {
                _Scriptlet.ThemeId = _ThemeId;
                _Scriptlet.Identifier = Identifier.Text.Trim(" \r\n\t".ToCharArray());
                _Scriptlet.Description = Description.Text.Trim(" \r\n\t".ToCharArray());
                _Scriptlet.HeaderData = HeaderData.Text.Trim(" \r\n\t".ToCharArray());
                _Scriptlet.ScriptletData = ScriptletData.Text.Trim(" \r\n\t".ToCharArray());
                if (_Scriptlet.IsDirty) _Scriptlet.IsCustom = true;
                SaveResult saveResult =  _Scriptlet.Save();
                if (saveResult == SaveResult.Failed)
                {
                    ErrorMessage.Text = "An error has occured while saving scriptlet, for details check the <a href='../../Help/ErrorLog.aspx'>error log</a>.";
                    ErrorMessage.Visible = true;
                }
                else
                {
                    ScriptletDataSource.ClearCache(_ThemeId);
                    RedirectMe();                    
                }
            }
        }
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        RedirectMe();
    }

}
