using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public partial class Admin_UserControls_DualList : System.Web.UI.UserControl
{
    private int _ListBox1width;
    public int ListBox1width
    {
        get { return _ListBox1width; }
         set { _ListBox1width = value; }

    }

    private int _ListBox1height;
    public int ListBox1height
    {
        get { return _ListBox1height; }
        set { _ListBox1height = value; }

    }


    public ListBox ListBox1DataView
    {
        get { return this.ListBox1; }


    }

 
    public DropDownList ListBox1FilterDropDownList
    {
        get { return  ListBox1Filter ;}
    }


    public Label listBox1FilterLabel
    {
        get { return filter1Label; }

    }

    private bool _ListBox1FilterVisible = true;
    public bool ListBox1FilterVisible
    {
        get { return _ListBox1FilterVisible; }
        set { _ListBox1FilterVisible = value; }

    }

    public ListItemCollection ListBox1Items
    {
        get { return this.ListBox1.Items; }

    }


    private int _ListBox2width;
    public int ListBox2width
    {
        get { return _ListBox2width; }
        set { _ListBox2width = value; }

    }

    private int _ListBox2height;
    public int ListBox2height
    {
        get { return _ListBox2height; }
        set { _ListBox2height = value; }

    }

    public ListBox ListBox2DataView
    {
        get { return this.ListBox2; }

    }

    public DropDownList ListBox2FilterDropDownList
    {
        get { return ListBox2Filter; }

    }

    public Label listBox2FilterLabel
    {
        get { return filter2Label; }

    }

    private bool _ListBox2FilterVisible = true;
    public bool ListBox2FilterVisible
    {
        get { return _ListBox2FilterVisible; }
        set { _ListBox2FilterVisible = value; }

    }


    public ListItemCollection ListBox2Items
    {
        get { return this.ListBox2.Items; }

    }


    [System.Security.Permissions.PermissionSet
    (System.Security.Permissions.SecurityAction.Demand,
     Name = "FullTrust")]
    protected override void OnInit(EventArgs e)
    {

        if (!Page.IsPostBack)
        {
            base.OnInit(e);

            this.ListBox1.Width = ListBox1width;
            this.ListBox1.Height = ListBox1height;

            this.ListBox1Filter.Visible = ListBox1FilterVisible;

            this.ListBox2.Width = ListBox2width;
            this.ListBox2.Height = ListBox2height;

            this.ListBox2Filter.Visible = ListBox2FilterVisible;
        }
          
    }




    protected void Page_Load(object sender, EventArgs e)
    {
     
    }
}
