<%@ Page Language="C#" Title="Edit HTML"
Inherits="MakerShop.Web.UI.MakerShopPage" EnableTheming="false"
ValidateRequest="false" %> <%@ Register TagPrefix="FCKeditorV2"
Namespace="FredCK.FCKeditorV2" Assembly="FredCK.FCKeditorV2" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
  <head id="Head1" runat="server">
    <title>Edit HTML</title>
  </head>
  <script language="javascript">
    function load() {
      var callerField = top.opener.document.getElementById(
        '<%=Request.QueryString["Field"]%>'
      );
      if (callerField) {
        // THIS CODE IS WORKING FOR IE7, FF2
        var localField = document.getElementById("<%= Editor.ClientID %>");
        if (localField) localField.value = callerField.value;

        // THIS CODE IS WORKING FOR FF3
        var oEditor = FCKeditorAPI.GetInstance("Editor");
        if (oEditor) {
          oEditor.SetHTML(callerField.value);
        }
      }
    }

    function save() {
      var callerField = top.opener.document.getElementById(
        '<%=Request.QueryString["Field"]%>'
      );
      if (callerField) {
        var oEditor = FCKeditorAPI.GetInstance("Editor");
        if (oEditor) {
          var hasInnerText =
            document.getElementsByTagName("body")[0].innerText != undefined
              ? true
              : false;
          if (hasInnerText) callerField.innerText = oEditor.GetHTML();
          else callerField.value = oEditor.GetHTML();
          window.close();
        }
      }
    }

    function FCKeditor_OnComplete(editorInstance) {
      editorInstance.LinkedField.form.onsubmit = save;
      editorInstance.Commands.GetCommand("FitWindow").Execute();
    }
  </script>
  <script runat="server">
    protected override void OnPreInit(EventArgs e)
    {
        //DISABLE DYNAMIC THEMING FOR THIS PAGE
        base.OnPreInit(e);
        this.Theme = string.Empty;
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        //SET THEME FOR EDITOR
        Editor.BasePath = this.Page.ResolveUrl("~/FCKeditor/");
        Editor.EditorAreaCSS = this.EditorAreaCSS;
        Session["FCKEditor:UserFilesPath"] = this.Page.ResolveUrl("~/Assets");
    }

    private string GetStoreTheme()
    {
        string theme = Token.Instance.Store.Settings.StoreTheme;
        if (!string.IsNullOrEmpty(theme)) return theme;
        return "MakerShop";
    }

    protected string EditorAreaCSS
    {
        get
        {
            string theme = GetStoreTheme();
            return this.Page.ResolveUrl("~/App_Themes/" + theme + "/style.css");
        }
    }
  </script>
  <body onload="setTimeout('load()',500)" style="margin: 0px">
    <form id="form1" runat="server">
      <FCKeditorV2:FCKeditor id="Editor" runat="server" Height="580" />
    </form>
  </body>
</html>
