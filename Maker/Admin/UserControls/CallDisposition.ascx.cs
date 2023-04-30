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
using MakerShop.Orders;
using MakerShop.Utility;
using MakerShop.Common;

public partial class Admin_UserControls_CallDisposition : System.Web.UI.UserControl
{

    protected DataTable SessionDataSource
    {
        get
        {
            Object ds = Session["DataSource_Transaction"];
            if (ds != null)
                return (DataTable)ds; // to replace with proper object

            return null;
        }
        set { Session["DataSource_Transaction"] = value; }
    }



    private string SortDirection
    {
        get
        {

            if (Session["SortDirection_Transaction"] == null)
                return "Asc";
            return Session["SortDirection_Transaction"].ToString();
        }
        set
        {
            Session.Add("SortDirection_Transaction", value);
        }
    }

    private string SortColumn
    {
        get
        {

            if (Session["SortColumn_Transaction"] == null)
                return "AffiliateID";
            return Session["SortColumn_Transaction"].ToString();
        }
        set
        {
            Session.Add("SortColumn_Transaction", value);
        }
    }

    private int _panelWidth;
    public int panelWidth
    {
        get
        {
            return _panelWidth;
        }
        set
        {
            _panelWidth = value;
        }
    }

    private Order _Order;

    protected void Page_Load(object sender, EventArgs e)
    {
        _Order = OrderHelper.GetOrderFromContext();

        if (!Page.IsPostBack)
        {
            this.statusdl.DataSource = CallDispositionsDataSource.LoadForCriteria("1=1");
            this.statusdl.DataTextField = "CallDisposition";
            this.statusdl.DataValueField = "IsCallBack";

            this.statusdl.DataBind();
       
            this.statusdl.Items.Insert(0, new ListItem("-Select Status-", "-1"));
          
          
            BindGrid();
            
        }
        CallDispositionPanel.Width = panelWidth;
    }

    protected void BindGrid()
    {
        
        if(_Order == null)
            return;

        string SqlCriteria = string.Format("OrderId ={0}", _Order.OrderId);

        gridPhoneNotes.DataSource = PhoneNotesDataSource.LoadForCriteria(SqlCriteria);
        gridPhoneNotes.DataBind();

    }

    protected void Save_Click(object sender, EventArgs e)
    {
       if(_Order.OrderId == null)
       {
            CallDispositions callDispositions = new CallDispositions();
            callDispositions.CallDisposition = this.callNotesTxt.Text;
            callDispositions.IsCallBack = AlwaysConvert.ToByte(this.statusdl.SelectedValue);

            callDispositions.Save();

            CallOrderDisposition callOrderDisposition = new CallOrderDisposition();
            callOrderDisposition.CallDispositionId = callDispositions.CallDispositionId;
            callOrderDisposition.UserId = Token.Instance.UserId;
            callOrderDisposition.OrderId = _Order.OrderId;
            callOrderDisposition.Save();
        }

    }


    protected string FormatDate(object dataItem)
    {
        DataRowView date = (DataRowView)dataItem;
        return string.Format("{0:MM/dd/yyyy}", date.Row.ItemArray[3]);
    }



    protected void Exit_Click(object sender, EventArgs e)
    {

        this.callNotesTxt.Text = string.Empty;
        this.statusdl.SelectedIndex = 0;




    }

  



    protected void gridPhoneNotes_Sorting(object sender, GridViewSortEventArgs e)
    {
        //Sorting Viewstate datasource based on the SortExpression selected
        string col = e.SortExpression.ToString();

        SortColumn = col;

        if ((SortDirection == "Asc") && (col == SortColumn))
        {
            SortDirection = "Desc";
            SessionDataSource.DefaultView.Sort = string.Concat(col, " desc");
        }

        else if ((SortDirection == "Desc") && (col == SortColumn))
        {
            SortDirection = "Asc";
            SessionDataSource.DefaultView.Sort = string.Concat(col, " asc");
        }
        //bind grid to trigger a refresh
        BindGrid();
    }



     
    protected string formatNoteDate(object dataItem)
    {
        DataRowView row = (DataRowView)dataItem;

        //pass date
        return ((DateTime)row["OrderDate"]).ToString("MM/dd/yy");
    }


    protected void gridPhoneNotes_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.gridPhoneNotes.PageIndex = e.NewPageIndex;
      
        BindGrid();
    }



}
