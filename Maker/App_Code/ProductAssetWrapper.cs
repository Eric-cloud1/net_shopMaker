public class ProductAssetWrapper
{

    private string _NavigateUrl;

    private string _Text;

    public ProductAssetWrapper()
    {
    }

    public ProductAssetWrapper(string navigateUrl, string text)
    {
        _NavigateUrl = navigateUrl;
        _Text = text;
    }

    public string NavigateUrl
    {
        get
        {
            return _NavigateUrl;
        }
        set
        {
            _NavigateUrl = value;
        }
    }

    public string Text
    {
        get
        {
            return _Text;
        }
        set
        {
            _Text = value;
        }
    }
}