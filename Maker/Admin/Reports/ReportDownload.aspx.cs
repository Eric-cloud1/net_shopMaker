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
using System.IO;
using MakerShop.Utility;

public partial class Admin_Reports_ReportDownload : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            string session = Request.QueryString["data"];

            DataTable summaries = (DataTable)Session[session];

        MemoryStream mm = new MemoryStream();
        StreamWriter sw = new StreamWriter(mm);
        sw.AutoFlush = true;

        if ((Request.QueryString["downloadCSV"] == "0") || (Request.QueryString["downloadCSV"] == "1"))  //CSV
            {
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
           
            }
            else
            {
                sw.Write(ExcelLibrary.ExcelXml.ConvertHTMLToExcelXML(ExcelLibrary.ExcelXml.GenerateWorkSheet(summaries)));
                sw.Write(sw.NewLine);
            }


            sw.Flush();
            mm.Seek(0, 0);


            if (Request.QueryString["downloadCSV"] == "1")
            {
                Response.BufferOutput = true;
                Response.ClearContent();
                Response.ClearHeaders();
                Response.AddHeader("Cache-Control", "no-cache");
                Response.AddHeader("Pragma", "public");
                Response.ContentType = "application/vnd.ms-excel";
                Response.AppendHeader("content-disposition", "attachment; filename=" + Request.QueryString["fileName"] + "_" + Request.QueryString["startDate"] + '-' + Request.QueryString["endDate"] + ".csv");
                Response.AppendHeader("content-length", mm.Length.ToString());
                Response.BinaryWrite(mm.ToArray());
                Response.Flush();
                Response.End();
                sw.Close();
                System.Collections.Generic.List<string> messages = new System.Collections.Generic.List<string>();
                messages.Add("File downloaded");
                UpdateMessagePanel(true, messages);
            }

            else if (Request.QueryString["downloadCSV"] == "0") //email
            {
                System.Net.Mail.Attachment attachement = new System.Net.Mail.Attachment(mm, Request.QueryString["fileName"] + "_" + Request.QueryString["startDate"] + '-' +
           Request.QueryString["endDate"] + ".csv");

                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                mail.From = new System.Net.Mail.MailAddress("viroplex@viralplex.com");
                mail.Subject = Request.QueryString["fileName"] + "_" + Request.QueryString["startDate"] + '-' + Request.QueryString["endDate"];
                mail.To.Add(new System.Net.Mail.MailAddress(MakerShop.Common.Token.Instance.User.Email));
                mail.Attachments.Add(attachement);
                MakerShop.Messaging.EmailClient.Send(mail);
                System.Collections.Generic.List<string> messages = new System.Collections.Generic.List<string>();
                messages.Add("Email has been sent. <br/>Attachment: " + attachement.Name);
                UpdateMessagePanel(true, messages);
            }

            if (Request.QueryString["downloadXML"] == "1")
            {
                Response.BufferOutput = true;
                Response.ClearContent();
                Response.ClearHeaders();
                Response.AddHeader("Cache-Control", "no-cache");
                Response.AddHeader("Pragma", "public");
                Response.ContentType = "application/vnd.ms-excel";
                Response.AppendHeader("content-disposition", "attachment; filename=" + Request.QueryString["fileName"] + "_" + Request.QueryString["startDate"] + '-' + Request.QueryString["endDate"] + ".xml");
                Response.AppendHeader("content-length", mm.Length.ToString());
                Response.BinaryWrite(mm.ToArray());
                Response.Flush();
                Response.End();
                sw.Close();
                System.Collections.Generic.List<string> messages = new System.Collections.Generic.List<string>();
                messages.Add("File downloaded");
                UpdateMessagePanel(true, messages);
            }

            else if (Request.QueryString["downloadXML"] == "0") //email
            {
                System.Net.Mail.Attachment attachement = new System.Net.Mail.Attachment(mm, Request.QueryString["fileName"] + "_" + Request.QueryString["startDate"] + '-' +
                Request.QueryString["endDate"] + ".xml");

                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                mail.From = new System.Net.Mail.MailAddress("viroplex@viralplex.com");
                mail.Subject = Request.QueryString["fileName"] + "_" + Request.QueryString["startDate"] + '-' + Request.QueryString["endDate"];
                mail.To.Add(new System.Net.Mail.MailAddress(MakerShop.Common.Token.Instance.User.Email));
                mail.Attachments.Add(attachement);
                MakerShop.Messaging.EmailClient.Send(mail);
                System.Collections.Generic.List<string> messages = new System.Collections.Generic.List<string>();
                messages.Add("Email has been sent. <br/> Attachment: " + attachement.Name);
                UpdateMessagePanel(true, messages);
            }

        
        }
        catch (Exception x)
        {
            System.Collections.Generic.List<string> messages = new System.Collections.Generic.List<string>();
            messages.Add("File could not be sent/downloaded:<br/> " + x.Message.ToString());
            UpdateMessagePanel(false, messages);
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

        ClientScript.RegisterStartupScript(this.Page.GetType(), "", "setTimeout('window.close()', 5000);", true);


    }



}
