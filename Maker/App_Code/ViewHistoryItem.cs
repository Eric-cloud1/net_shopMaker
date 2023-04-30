public class ViewHistoryItem
{

    private string _Url;
    private string _Name;
    private bool _IsShoppingPage;

    public ViewHistoryItem(string url, string name, bool isShoppingPage)
    {
        this._Url = url;
        this._Name = name;
        this._IsShoppingPage = isShoppingPage;
    }

    public string Url
    {
        get { return _Url; }
    }

    public string Name
    {
        get { return _Name; }
    }

    public bool IsShoppingPage
    {
        get { return _IsShoppingPage; }
    }

}