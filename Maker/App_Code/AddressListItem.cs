using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// A helper class for displaying formatted addresses in data lists.
/// </summary>
public class AddressListItem
{
    private int _AddressId;
    private string _Text;
    private bool _Selected;
    public int AddressId
    {
        get { return _AddressId; }
        set { _AddressId = value; }
    }
    public string Text
    {
        get { return _Text; }
        set { _Text = value; }
    }
    public bool Selected
    {
        get { return _Selected; }
        set { _Selected = value; }
    }
    public AddressListItem(int addressId, string text)
    {
        _AddressId = addressId;
        _Text = text;
    }
}
