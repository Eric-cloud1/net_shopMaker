using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MakerShop.Payments;
using MakerShop.Reporting;
using System.Collections.Generic;
using MakerShop.Common;
using MakerShop.Utility;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using ExcelLibrary;

public partial class Admin_Reports_RebillCaptureReport : System.Web.UI.Page
{

    private int rebillsattempted = -1;
    private int rebillscaptured = -1;
    private int rebillsdeclined = -1;



    protected DataTable SessionDataSource
    {
        get
        {
            Object ds = Session["DataSource_RebillCaptureReport"];
            if (ds != null)
                return (DataTable)ds;

            return null;
        }
        set { Session["DataSource_RebillCaptureReport"] = value; }
    }


    private string SortDirection
    {
        get
        {

            if (Session["SortDirection_RebillCaptureReport"] == null)
                return "Asc";
            return Session["SortDirection_RebillCaptureReport"].ToString();
        }
        set
        {
            Session.Add("SortDirection_RebillCaptureReport", value);
        }
    }
    private string SortColumn
    {
        get
        {

            if (Session["SortColumn_RebillCaptureReport"] == null)
                return "AffiliateID";
            return Session["SortColumn_RebillCaptureReport"].ToString();
        }
        set
        {
            Session.Add("SortColumn_RebillCaptureReport", value);
        }
    }


    protected void Page_Init(object sender, EventArgs e)
    {

        if (dtRebillCapture.FirstHit)
        {
            dtRebillCapture.TimePeriod = Admin_UserControls_DatesAndTime.Time_Period.Last_15_Days;
        }

        dtRebillCapture.Update += new Admin_UserControls_DatesAndTime.UpdateEventHandler(BindGrid);
    }


  protected void Page_Load(object sender, EventArgs e)
  {
     

      if (!Page.IsPostBack)
      {
          
          BindGrid();
          //SessionDataSource = ReportDataSource.GetRebillReport(StartDate.SelectedDate, EndDate.SelectedDate, int.Parse(paymentInstrument.SelectedValue));
          //RebillReportGrid.DataSource = SessionDataSource;
          //RebillReportGrid.DataBind();
      }

  }

  public static DataTable Rebillingreport(DataTable dataValues)
  {
      List<string> pivotData = new List<string>();
      pivotData.Add("Rebills captured Count");
      pivotData.Add("Rebills captured Amount");

      return MakerShop.Utility.DataHelper.Pivot(dataValues, "transactionDate", "PaymentGatway", pivotData);


  }

  protected DataTable Convert(DataTable dt)
  {
      DataTable convert = new DataTable();

      foreach (DataColumn dc in dt.Columns)
      {
          if (dc.ColumnName.ToUpper().Contains("COUNT"))
              continue;
          else if (dc.ColumnName.ToUpper().Contains("AMOUNT"))
              convert.Columns.Add(dc.ColumnName.Replace("Amount", "").Trim(), typeof(string));
          else
              convert.Columns.Add(dc.ColumnName, dc.DataType);
      }
      foreach (DataRow dr in dt.Rows)
      {

          DataRow DR = convert.NewRow();
          foreach (DataColumn dc in convert.Columns)
          {

              if (dt.Columns[dc.ColumnName] == null)
              {
                  if (dr[dc.ColumnName + " Amount"] != null)
                  {
                      DR[dc.ColumnName] = string.Format("{0:C}", decimal.Parse(dr[dc.ColumnName + " Amount"].ToString())) + DR[dc.ColumnName];
                  }
                  if (dr[dc.ColumnName + " Count"] != null)
                  {
                      DR[dc.ColumnName] = DR[dc.ColumnName] + "<br/>Count:" + dr[dc.ColumnName + " Count"].ToString();
                  }
              }
              else
              {
                  DR[dc.ColumnName] = dr[dc.ColumnName];
              }

          }
          convert.Rows.Add(DR);
      }

      return convert;

  }

  public static DataTable CreateTable(DataView obDataView)
  {
      if (null == obDataView)
      {
          throw new ArgumentNullException
          ("DataView", "Invalid DataView object specified");
      }

      DataTable obNewDt = obDataView.Table.Clone();
      int idx = 0;
      string[] strColNames = new string[obNewDt.Columns.Count];
      foreach (DataColumn col in obNewDt.Columns)
      {
          strColNames[idx++] = col.ColumnName;
      }

      IEnumerator viewEnumerator = obDataView.GetEnumerator();
      while (viewEnumerator.MoveNext())
      {
          DataRowView drv = (DataRowView)viewEnumerator.Current;
          DataRow dr = obNewDt.NewRow();
          try
          {
              foreach (string strName in strColNames)
              {
                  dr[strName] = drv[strName];
              }
          }
          catch
          {
          }
          obNewDt.Rows.Add(dr);
      }

      return obNewDt;
  }

  protected void BindGrid()
  {

      DataTable rebilldt;
      rebilldt = ReportDataSource.GetRebillShortCaptureReport(dtRebillCapture.StartDate, dtRebillCapture.EndDate, short.Parse(rblGatewayInstrument.SelectedValue));
      SessionDataSource = Rebillingreport(rebilldt);

      this.downloadbutton1.DownloadData = rebilldt;
      this.downloadbutton1.StartDate = dtRebillCapture.StartDate;
      this.downloadbutton1.EndDate = dtRebillCapture.EndDate;
      this.downloadbutton1.FileName = "RebillCapture";

      string colName = string.Empty;
      for (int i = 0; i < Convert(SessionDataSource).Columns.Count; ++i)
      {
          colName = Convert(SessionDataSource).Columns[i].ColumnName;

          switch (colName)
          {
              case "Rebills attempted":
                  rebillsattempted = i;
                  break;
              case "Rebills captured":
                  rebillscaptured = i;
                  break;
              case "Rebills declined":
                  rebillsdeclined = i;
                  break;
          }
      }

      RebillReportGrid.PageIndex = 0;
      RebillReportGrid.DataSource = Convert(SessionDataSource).DefaultView;
      RebillReportGrid.DataBind();
  }

  protected void ProcessButton_Click(object sender, EventArgs e)
  {
      dtRebillCapture.setDateTime();
      BindGrid();
  }

  private void UpdateMessagePanel(bool success, System.Collections.Generic.List<String> messages)
  {
      if (success)
      {
          SavedMessage.Visible = true;
          SavedMessage.Text = string.Format(string.Join("<br/>", messages.ToArray()), LocaleHelper.LocalNow);
      }
      else
      {
          ErrorMessageLabel.Visible = true;
          ErrorMessageLabel.Text = string.Join("<br/>", messages.ToArray());
      }

  }


  protected void RebillReportGrid_PreRender(object sender, EventArgs e)
  {
      foreach (GridViewRow gvr in RebillReportGrid.Rows)
      {
          foreach (TableCell tc in gvr.Cells)
              fixPop(tc);
      }
  }

  private void fixPop(TableCell tc)
  {
      DateTime d;

      if (!DateTime.TryParse(tc.Text, out d))
      {
          if ((tc.Text.Contains("Count:")) && tc.Text.Substring(tc.Text.IndexOf("Count:") + 6, 1) == "0")
              return;

          if (!tc.Text.Contains("Count:"))
              return;

          if (tc.Attributes["id"] != null)
          {

              // tc.Attributes.Add("OnMouseOut", string.Format("HideHoverPanel();"));
              tc.Attributes.Add("parent", "GridTable");
              tc.Attributes.Add("SkinID", string.Format("Link"));
              tc.Attributes.Add("style", "cursor:pointer;text-decoration:underline");
          }
      }
  }



  protected void RebillReportGrid_Sorting(object sender, GridViewSortEventArgs e)
    {
        string col = e.SortExpression.ToString();

        if (!SessionDataSource.Columns.Contains(col))
            col = e.SortExpression.ToString() + " Amount";

        SortColumn = col;

        DataTable sortedTable = new DataTable();

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

        sortedTable = CreateTable(SessionDataSource.DefaultView);
   
        //bind grid to trigger a refresh
        RebillReportGrid.DataSource = sortedTable;
        RebillReportGrid.DataBind();

    }

  protected void RebillReportGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
  {
      this.RebillReportGrid.PageIndex = e.NewPageIndex;
      this.RebillReportGrid.DataSource = Convert(SessionDataSource).DefaultView;
      RebillReportGrid.DataBind();
  }

  protected void RebillReportGrid_RowDataBound(object sender, GridViewRowEventArgs e)
  {


      if (e.Row.RowType == DataControlRowType.DataRow)
      {
          DataRowView dr = ((DataRowView)e.Row.DataItem);
          string ids = string.Empty;

          for (int i = 0; i <= e.Row.Cells.Count - 1; i++)
          {
              string colName = dr.Row.Table.Columns[i].ColumnName.ToLower().Replace("rebills","").Replace("captured","").Replace("amount","").Replace("count","");

              if (e.Row.Cells[i].Text.ToUpper().Contains("&LT;BR/&GT;"))
              {
                  e.Row.Cells[i].Attributes.Add("align", "right");
                  e.Row.Cells[i].Text = Server.HtmlDecode(e.Row.Cells[i].Text);

              }
              else if (((DataRowView)e.Row.DataItem)[i].GetType().FullName == typeof(int).FullName)
              {
                  e.Row.Cells[i].Attributes.Add("align", "right");

              }
              else if (((DataRowView)e.Row.DataItem)[i].GetType().FullName == typeof(decimal).FullName)
              {
                  if (colName.Contains("%"))
                      e.Row.Cells[i].Text = string.Format("{0:P}", decimal.Parse(e.Row.Cells[i].Text) / 100);
                  else
                      e.Row.Cells[i].Text = string.Format("{0:C}", decimal.Parse(e.Row.Cells[i].Text));
                  e.Row.Cells[i].Attributes.Add("align", "right");
              }


             

              if (i == rebillsattempted) //rebills attempted
              {
              
                ids = string.Format("transactionDate={0}&reportType={1}&paymentTypes={2}", dr["transactionDate"].ToString(), "18", "4");

                if (e.Row.Cells[i].Text != "0")
                    e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));
              }


              else if   (i == rebillscaptured) //rebills captured
              {
              
                   ids = string.Format("transactionDate={0}&reportType={1}&paymentTypes={2}", dr["transactionDate"].ToString(), "20", "4");

                   if (e.Row.Cells[i].Text != "0")
                       e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

              }

              else if (i == rebillsdeclined) //rebills declined
              {

                  ids = string.Format("transactionDate={0}&reportType={1}&paymentTypes={2}", dr["transactionDate"].ToString(), "21", "4");

                  if (e.Row.Cells[i].Text != "0")
                      e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));

       
              }

              else
              {
                
                  ids = string.Format("transactionDate={0}&reportType={1}&paymentTypes={2}&paymentGatewayPrefix={3}", dr["transactionDate"].ToString(), "21", "4", colName);

                  if (e.Row.Cells[i].Text != "0")
                      e.Row.Cells[i].Attributes.Add("id", Helper.MISC.base64Encode(ids, System.Text.Encoding.UTF8).Replace("=", "-"));
              }
          }
      }
  }

  private Dictionary<string, Control> newControls = new Dictionary<string, Control>();
  void cme_ResolveControlID(object sender, AjaxControlToolkit.ResolveControlEventArgs e)
  {
      e.Control = Page.FindControl(e.ControlID);
  }

}


 