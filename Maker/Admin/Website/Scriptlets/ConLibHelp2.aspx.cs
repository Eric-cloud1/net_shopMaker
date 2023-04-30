using System;
using System.Web.UI.WebControls;
using MakerShop.Web.ConLib;

public partial class Admin_Website_Scriptlets_ConLibHelp2 : MakerShop.Web.UI.MakerShopAdminPage
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
        SelectControl.Items.Clear();
        SelectControl.Items.Add(new ListItem(string.Empty));
        SelectControl.DataSource = ConLibControls;
        SelectControl.DataBind();
    }

    protected void SelectControl_SelectedIndexChanged(object sender, EventArgs e)
    {
        string controlName = SelectControl.SelectedValue;
        if (!string.IsNullOrEmpty(controlName))
        {
            ConLibControl control = ConLibControls.TryGetValue(controlName);
            if (control != null)
            {
                phControlDetails.Visible = true;
                Usage.Text = control.Usage;
                Summary.Text = control.Summary;
                if (control.Params.Count > 0)
                {
                    ParamList.Visible = true;
                    ParamList.DataSource = control.Params;
                    ParamList.DataBind();
                }
            }
        }
    }
}
