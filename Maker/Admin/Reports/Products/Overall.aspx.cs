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
using System.Linq;
using System.IO;
using MakerShop.Products;


public partial class Admin_Reports_Products_Overall : System.Web.UI.Page
{

    protected DataTable SessionDataSource
    {
        get
        {
            Object ds = ViewState["DataSource"];
            if (ds != null)
                return (DataTable)ds;

            return null;
        }
        set { ViewState["DataSource"] = value; }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {


            productIds.DataSource = ProductDataSource.LoadForCriteria(" ProductId > 10000","Name");
            productIds.DataTextField = "Name";
            productIds.DataValueField = "ProductId";
            productIds.DataBind();

            BindGrid();
        }
    }

    protected void BindGrid()
    {
        SessionDataSource = PageViewDataSource.GetProductStatusoverall(dtOverall.StartDate, dtOverall.EndDate, AlwaysConvert.ToInt(productIds.SelectedValue)).Tables[0];

        //Casting the Ienumerable collection to the datasource datetype
    
        
        this.OverallGrid.DataSource = SessionDataSource;
        OverallGrid.DataBind();
    }

    protected void ProcessButton_Click(object sender, EventArgs e)
    {
        BindGrid();
    }


    protected void EmailButton_Click(object sender, EventArgs e)
    {

        if(SessionDataSource == null)
            SessionDataSource = PageViewDataSource.GetProductStatusoverall(dtOverall.StartDate, dtOverall.EndDate, int.Parse(productIds.SelectedValue)).Tables[0];


        MemoryStream mm = new MemoryStream();
        StreamWriter sw = new StreamWriter(mm);
        sw.AutoFlush = true;


        sw.Write("Amount", "Count", "ChargeBackCount", "PaymentStatus", "PaymentType");
        sw.Write(sw.NewLine);


    

        for (int k = 0; k < SessionDataSource.Rows.Count; ++k)
        {

            sw.Write(SessionDataSource.Rows[k].Field<decimal>("Amount"));
            sw.Write(",");
            sw.Write(SessionDataSource.Rows[k].Field<Int32>("Count"));
            sw.Write(",");
            sw.Write(SessionDataSource.Rows[k].Field<Int32>("ChargeBackCount"));
            sw.Write(",");
            sw.Write(SessionDataSource.Rows[k].Field<string>("PaymentStatus"));
            sw.Write(",");
            sw.Write(SessionDataSource.Rows[k].Field<string>("PaymentType"));
 

            sw.Write(sw.NewLine);
        }
        sw.Flush();

        mm.Seek(0, 0);
        System.Net.Mail.Attachment attachement = new System.Net.Mail.Attachment(mm, "ProductOverallStatus_" + DateTime.Now.Month.ToString() + '-' +
            DateTime.Now.Day.ToString() + '-' + DateTime.Now.Year.ToString() + ".csv");
        if (ViewState["SentFile"] != null)
        {
            if (ViewState["SentFile"].ToString() == attachement.Name)
                return;
            else
                ViewState.Remove("SentFile");
        }
        ViewState.Add("SentFile", attachement.Name);
        System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
        mail.From = new System.Net.Mail.MailAddress("viroplex@viralplex.com");
        mail.Subject = "DigitalGoods" + DateTime.Now.Month.ToString() + '-' + DateTime.Now.Day.ToString() + '-' + DateTime.Now.Year.ToString();
        mail.To.Add(new System.Net.Mail.MailAddress(MakerShop.Common.Token.Instance.User.Email));
        mail.Attachments.Add(attachement);
        MakerShop.Messaging.EmailClient.Send(mail);
    }

    protected void OverallGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        OverallGrid.PageIndex = e.NewPageIndex;
        OverallGrid.DataSource = SessionDataSource;
        OverallGrid.DataBind(); ;
    }

    protected void OverallGrid_Sorting(object sender, GridViewSortEventArgs e)
    {
        //Sorting Viewstate datasource based on the SortExpression selected
        string order = e.SortExpression.ToString();

        var overallProduct = SessionDataSource.AsEnumerable();
        var sortedOverallProduct = from ov in overallProduct orderby ov.Field<Int32>(0) select ov;

        switch (order)
        {
            case "Amount":
                sortedOverallProduct = from ov in overallProduct orderby ov.Field<Int32>(0) descending select ov;
                break;

            case "Count":
                sortedOverallProduct = from ov in overallProduct orderby ov.Field<Int32>(1) descending select ov;
                break;

            case "ChargeBackCount":
                sortedOverallProduct = from ov in overallProduct orderby ov.Field<Int32>(2) descending select ov;
                break;
            case "PaymentStatus":
                sortedOverallProduct = from ov in overallProduct orderby ov.Field<string>(3) descending select ov;
                break;
            case "PaymentType":
                sortedOverallProduct = from ov in overallProduct orderby ov.Field<string>(4) descending select ov;
                break;
            default:
                sortedOverallProduct = from ov in overallProduct orderby ov.Field<Int32>(0) descending select ov;
                break;

        }

      

        //bind grid to trigger a refresh
        OverallGrid.DataSource = sortedOverallProduct;
        OverallGrid.DataBind();

    }




}
