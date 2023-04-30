using System;
using System.Collections;
using System.Collections.Generic;
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
using System.IO;
using System.Threading;
using System.Data.SqlClient;
using MakerShop.Affiliates;


public partial class Admin_Tools_Import_AffiliateCounts : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            responsePanel.Visible = false;
            //pStatus.Visible = false;
            bindSubmitUpdateLabel.Visible = false;
        }
        statusPanel.Visible = false;

    }

    protected void Submit_Click(object sender, EventArgs e)
    {

        if (FileUpload1.HasFile)
        {

            try
            {
                TextReader tr = (TextReader)new StreamReader(FileUpload1.PostedFile.InputStream);
                string[] row = tr.ReadLine().Split(',');
                DataTable dt = new DataTable();

                foreach (string i in row)
                {
                    dt.Columns.Add(GetData(i).ToUpper().Trim());
                }

                string data;

                while ((data = tr.ReadLine()) != null)
                {
                    row = data.Split(',');
                    if (row.Count() != dt.Columns.Count)
                    {
                        row = new string[dt.Columns.Count];
                    }
                    DataRow dr = dt.NewRow();

                    for (int i = 0; i < row.Length; ++i)
                    {
                        dr[i] = GetData(row[i]);
                    }
                    dt.Rows.Add(dr);
                }


                string[] title = FileUpload1.FileName.Split('_');
                string[] date = title[4].Split('-');
                string tempdate = date[1] + '/' + date[2] + '/' + date[0];
                DateTime Date;// = Convert.ToDateTime(tempdate + " 12:00:00 AM");

                List<string> CodesToBind = new List<string>();
                AffiliateCounts affiliateCount = null;
             
                
                for (int i = 0; i < dt.Rows.Count; ++i)
                {
     

                    affiliateCount = new AffiliateCounts();
                   
                    affiliateCount.AffiliateCode = dt.Rows[i]["Camp ID"].ToString();
      

                    for (int j = 2; j < dt.Columns.Count - 1; ++j)
                    {
                        
         
                        int temphour = Convert.ToDateTime("01/01/2010 " + dt.Columns[j].ToString()).Hour;

                        Date = DateTime.Parse(tempdate).AddHours(temphour).AddHours(-MakerShop.Common.Token.Instance.Store.Settings.TimeZoneOffset);
                        affiliateCount.SaleCount = Convert.ToInt32(dt.Rows[i][j]);
                        affiliateCount.Date = Date;
                        affiliateCount.Hour = Date.Hour ;
                        affiliateCount.CreateDate = DateTime.UtcNow;

                        
                        affiliateCount.Save();

                        if (!CodesToBind.Contains(affiliateCount.AffiliateCode))
                            CodesToBind.Add(affiliateCount.AffiliateCode);
                        
                    }
                }

                if (CodesToBind.Count > 0)
                {
                    gridBindCodes.DataSource = CodesToBind.ToArray();
                    gridBindCodes.DataBind();
                    responsePanel.Visible = true;

                }
                else
                {
                    gridBindCodes.DataSource = null;
                    gridBindCodes.DataBind();
                    responsePanel.Visible = false;
                }

                lUploadStatus.Text = "Upload Successful";
                statusPanel.Visible = true;
            }
            catch (Exception ex)
            {
                lUploadStatus.Text = "Upload Failed. Reason: " + ex.Message;
                statusPanel.Visible = true;
            }
        }
    }

    private DataTable PopulateDataTable(string affiliateCode, DataTable dt)
    {

        return dt;
    }


    private string GetData(string d)
    {
        if (string.IsNullOrEmpty(d))
            return d;

        return d.Replace("\"", "");
    }

    protected void gridBindCodes_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType != DataControlRowType.DataRow)
            return;
        Label affiliatelabel = (Label)e.Row.FindControl("affiliateCodeLabel");

        affiliatelabel.Text = (string)e.Row.DataItem;

    }

    protected void Submit_Click_Two(object sender, EventArgs e)
    {
        AffiliateCode_AffiliateId affiliateCode = null;
        foreach (GridViewRow gvr in gridBindCodes.Rows)
        {
            if (gvr.RowType != DataControlRowType.DataRow)
                continue;
            Label affiliateCodeLabel = (Label)gvr.FindControl("affiliateCodeLabel");
            TextBox affiliateId = (TextBox)gvr.FindControl("affiliateIdText");
            affiliateCode = new AffiliateCode_AffiliateId();

            try
            {
                affiliateCode.AffiliateCode = affiliateCodeLabel.Text;
                affiliateCode.AffiliateId = int.Parse(affiliateId.Text.Trim());
                affiliateCode.Save();

                bindSubmitUpdateLabel.Text = "Bind Done";
                bindSubmitUpdateLabel.Visible = true;
            }
            catch (Exception ex)
            {
                bindSubmitUpdateLabel.Text = "Could not complete bind. Reason: " + ex.Message;
                bindSubmitUpdateLabel.Visible = true;
            }


        }
    }

}
