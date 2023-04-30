using System;
using MakerShop.Stores;
using MakerShop.Utility;

public partial class ConLib_SimpleSearch : System.Web.UI.UserControl
{
    protected void Page_Init()
    {
        string handleEnterScript = "if(event.which || event.keyCode){if ((event.which == 13) || (event.keyCode == 13)) {document.getElementById('"
            + SearchButton.UniqueID + "').click();return false;}} else {return true}; ";
        SearchPhrase.Attributes.Add("onkeydown", handleEnterScript);
        int minimumSearchLength = Store.GetCachedSettings().MinimumSearchLength;
        SearchPhraseValidator.MinimumLength = minimumSearchLength;
        SearchPhraseValidator.ErrorMessage = string.Format(SearchPhraseValidator.ErrorMessage, minimumSearchLength);
        string searchUrl = this.Page.ResolveUrl("~/Search.aspx");
        SearchButton.OnClientClick = string.Format(SearchButton.OnClientClick, searchUrl, SearchPhrase.ClientID);
    }

    protected void SearchButton_Click(object sender, EventArgs e)
    {
        string safeSearchPhrase = StringHelper.StripHtml(SearchPhrase.Text);
        if (!string.IsNullOrEmpty(safeSearchPhrase))
            Response.Redirect("~/Search.aspx?k=" + Server.UrlEncode(safeSearchPhrase));
        Response.Redirect("~/Search.aspx");
    }
}
