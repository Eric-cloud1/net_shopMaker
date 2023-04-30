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
/// Arguments provided for an event related to a persistent object.
/// </summary>
public class PersistentItemEventArgs
{
    private int _Id;
    private string _Name;

    public PersistentItemEventArgs(int id, string name)
    {
        _Id = id;
        _Name = name;
    }

    public int Id { get { return _Id; } }
    public string Name { get { return _Name; } }
}
