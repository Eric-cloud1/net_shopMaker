using System;
using MakerShop.Web.ConLib;

public partial class Admin_Website_Scriptlets_ConLibHelpAll2 : MakerShop.Web.UI.MakerShopAdminPage
{
    private ConLibControlCollection _ConLibControls;
    public ConLibControlCollection ConLibControls
    {
        get
        {
            if (_ConLibControls == null)
            {
                if (Page.IsPostBack)
                {
                    _ConLibControls = (ConLibControlCollection)Session["ConLibControls"];
                }
                if (_ConLibControls == null)
                {
                    _ConLibControls = ConLibDataSource.Load();
                    Session["ConLibControls"] = _ConLibControls;
                }
            }
            return _ConLibControls;
        }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        ConLibList.DataSource = ConLibControls;
        ConLibList.DataBind();
    }
}
