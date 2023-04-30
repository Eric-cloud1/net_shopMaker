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

public partial class Admin_Tools_Export_CustomerExport : System.Web.UI.Page
{


    protected void Page_Load(object sender, EventArgs e)
    {

        if (!Page.IsPostBack)
        {
            EndDate.SelectedDate = DateTime.Today;

            fromDate.SelectedDate = DateTime.Now.Date.AddMonths(-1);
        }
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



    protected void ExcelReport_Click(object sender, ImageClickEventArgs e)

    {
        string strReportTypeList = "";
        string strdupremovalList = "";
        for (int i = 0; i < CbReporttype.Items .Count; i++)

        {
            if (CbReporttype.Items[i].Selected)
            {
                strReportTypeList = strReportTypeList + CbReporttype.Items[i].Value + ",";
            }
      
        }

        for (int i = 0; i < Cbdupremoval.Items.Count; i++)
        {
            if (Cbdupremoval.Items[i].Selected)
            {
                strdupremovalList = strdupremovalList + Cbdupremoval.Items[i].Value + ",";
            }
      
        }

     

        if (this.txtAffiliates.Text.Trim() == "Comma separated AffiliateID(s)...")
        {
            this.txtAffiliates.Text = string.Empty;
        }

        if (this.txtSubAffiliate.Text.Trim() == "Comma separated SubAffiliate code(s)...")
        {
            this.txtSubAffiliate.Text = string.Empty;
        }

     
            DataTable summaries = ReportDataSource.GetCustomerExport(fromDate.SelectedDate, EndDate.SelectedDate, strReportTypeList, strdupremovalList, this.txtAffiliates.Text.Trim(), this.txtSubAffiliate.Text.Trim(), chkSubAffiliate.Checked);
            MemoryStream mm = new MemoryStream();
            StreamWriter sw = new StreamWriter(mm);
            sw.AutoFlush = true;


            for (int i = 0; i < summaries.Columns.Count; ++i)
            {
                DataColumn dc = summaries.Columns[i];

                sw.Write(dc.ColumnName);
                if (i != summaries.Columns.Count - 1)
                    sw.Write(",");
            }

            sw.Write(sw.NewLine);


            foreach (DataRow _row in summaries.Rows)
            {

                for (int i = 0; i < summaries.Columns.Count; i++)
                {

                    sw.Write(_row[i].ToString());
                    if (i != summaries.Columns.Count - 1)
                        sw.Write(",");

                }


                sw.Write(sw.NewLine);

            }
            sw.Flush();

            mm.Seek(0, 0);
            System.Net.Mail.Attachment attachement = new System.Net.Mail.Attachment(mm, "CustomerExport_" + fromDate.SelectedDate + '-' +
                EndDate.SelectedDate + ".csv");
            try
            {
            if (cbEmail.Checked)
            {




                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                mail.From = new System.Net.Mail.MailAddress("viroplex@viralplex.com");
                mail.Subject = "CustomerExport_" + fromDate.SelectedDate + '-' + this.EndDate.SelectedDate;
                mail.To.Add(new System.Net.Mail.MailAddress(MakerShop.Common.Token.Instance.User.Email));
                mail.Attachments.Add(attachement);
                MakerShop.Messaging.EmailClient.Send(mail);
         
                System.Collections.Generic.List<string> messages = new System.Collections.Generic.List<string>();
                messages.Add("Email has been sent. Attachment: " + attachement .Name );
                UpdateMessagePanel(true, messages);
            }
            else
            {
                Response.BufferOutput = true;
                Response.ClearContent();
                Response.ClearHeaders();
                Response.AddHeader("Cache-Control", "max-age=3");
                Response.AddHeader("Pragma", "public");
                Response.ContentType = "application/vnd.ms-excel";
                Response.AppendHeader("content-disposition", "attachment; filename=CustomerExport_" + fromDate.SelectedDate + '-' + EndDate.SelectedDate + ".csv;");
                Response.AppendHeader("content-length", mm.Length.ToString());
                Response.BinaryWrite(mm.ToArray());
                Response.Flush();
                Response.End();

                sw.Close();





            }
        }
        catch (Exception x)
        {
            System.Collections.Generic.List<string> messages = new System.Collections.Generic.List<string>();
            messages.Add("Email could not be sent/downloaded: " + x.Message.ToString());
            UpdateMessagePanel(false, messages);
        }



    }
   
}
