using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.IO;
using MakerShop.Shipping;
using MakerShop.Orders;
using MakerShop.Utility;
using System.Collections.Generic;
using ExcelLibrary;

using System.Text;
using System.Xml;





public partial class Admin_Reports_ShippingSummary : System.Web.UI.Page
{
    List<string> successfulTracking = new List<string>();
    List<string> failedTracking = new List<string>();


    private string _recordcount;
    public string recordcount
    {
        get { return this._recordcount; }
        set
        {
            this._recordcount = value;

        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        SavedMessage.Visible = false;
        ErrorMessageLabel.Visible = false;
        if (!IsPostBack)
        {
            Server.ScriptTimeout = 60 * 30;
            ScriptManager.GetCurrent(this).AsyncPostBackTimeout = 60 * 30;
        }
    }
    protected void Export_Click(object sender, EventArgs e)
    {
        try
        {
            bool us = ((Button)sender).ID.Contains("US");
            DataTable table = MakerShop.Reporting.ShippingSummary.PackagesToShip(us);
            table.Columns.Remove("Name");
            table.Columns.Remove("OrderDate");

            CreateCSVFile(table, us ? "US" : "INTERNATIONAL", cbEmail.Checked);
            System.Collections.Generic.List<string> messages = new System.Collections.Generic.List<string>();
            messages.Add("Email has been sent.");
            UpdateMessagePanel(true, messages);
        }
        catch (Exception x)
        {
            System.Collections.Generic.List<string> messages = new System.Collections.Generic.List<string>();
            messages.Add("Email could not be sent: " + x.Message.ToString());
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
    
    }
    
    private void ImportData(int ordernum, string trackingnum, DateTime shipdate)
    {

        OrderShipment os = new OrderShipment();
        int orderId = 0;
        int ordershipmentId = 0;

        try
        {
            OrderShipmentDataSource.LoadForOrderIds(ordernum, shipdate, out orderId, out ordershipmentId);

            os.OrderId = orderId;
            os.OrderShipmentId = ordershipmentId;
        }
        catch
        {
            failedTracking.Add(string.Format("Order Number: {0}", ordernum));

        }


        TrackingNumber tracknumber = null;
        foreach (TrackingNumber tn in os.TrackingNumbers)
        {
            if (tn.TrackingNumberData == trackingnum)
            {
                tracknumber = tn;
                break;
            }

        }
        if (tracknumber == null)
        {
            tracknumber = new TrackingNumber();
            tracknumber.TrackingNumberData = trackingnum;
            tracknumber.OrderShipmentId = os.OrderShipmentId;
            os.TrackingNumbers.Add(tracknumber);
        }


        try
        {
            os.SaveChildren();
            successfulTracking.Add(os.TrackingNumbers[0].TrackingNumberData.ToString());
        }
        catch
        {
            failedTracking.Add(string.Concat("Tracking: ", os.TrackingNumbers[0].TrackingNumberData.ToString()));
        }


    }

    private void CreateCSVFile(DataTable dt, string FileName, bool email)
    {

        MemoryStream mm = new MemoryStream();
        StreamWriter sw = new StreamWriter(mm);
        sw.AutoFlush = true;

        for (int i = 0; i < dt.Columns.Count; ++i)
        {
            sw.Write(dt.Columns[i]);

            if (i < dt.Columns.Count - 1)
                sw.Write(",");
        }

        sw.Write(sw.NewLine);

        foreach (DataRow dr in dt.Rows)
        {
            for (int i = 0; i < dt.Columns.Count; ++i)
            {
                if (!Convert.IsDBNull(dr[i]))
                {
                    sw.Write(dr[i].ToString());
                }
                if (i < dt.Columns.Count - 1)
                {
                    sw.Write(",");
                }
            }

            sw.Write(sw.NewLine);
        }
        sw.Flush();

        mm.Seek(0, 0);
        string name = "Shippment_Summary_" + DateTime.Now.Month.ToString() + '-' +
            DateTime.Now.Day.ToString() + '-' + DateTime.Now.Year.ToString() + "_" + FileName;

        System.Net.Mail.Attachment attachement = new System.Net.Mail.Attachment(mm, name + ".csv");


        if (email)
        {
            System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
            mail.From = new System.Net.Mail.MailAddress("viroplex@viralplex.com");
            mail.Subject = name;
            mail.To.Add(new System.Net.Mail.MailAddress(MakerShop.Common.Token.Instance.User.Email));
            mail.Attachments.Add(attachement);
            MakerShop.Messaging.EmailClient.Send(mail);
            List<string> s = new List<string>();
            s.Add(attachement.Name + " emailed.");
            UpdateMessagePanel(true, s);
        }
        else
        {
            Response.BufferOutput = true;
            Response.ClearContent();
            Response.ClearHeaders();
            Response.AddHeader("Cache-Control", "max-age=3");
            Response.AddHeader("Pragma", "public");
            Response.ContentType = "application/vnd.ms-excel";
            //               Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
            //Response.AppendHeader("Pragma", "must-revalidate");
            Response.AppendHeader("content-disposition", "attachment; filename=Shippment_Summary_" + DateTime.Now.Month.ToString() + '-' +
                DateTime.Now.Day.ToString() + '-' + DateTime.Now.Year.ToString() + ".csv;");
            Response.AppendHeader("content-length", mm.Length.ToString());
            //             Response.AppendHeader("Accept-Ranges", "none");
            Response.BinaryWrite(mm.ToArray());
            Response.Flush();
            Response.End();

            sw.Close();

        }
    }


    protected void AsyncFileUpload1_UploadedComplete(object sender, AjaxControlToolkit.AsyncFileUploadEventArgs e)
    {

        //string strPath = MapPath("~/Uploads/") + Path.GetFileName(e.filename);
        // AsyncFileUpload1.SaveAs(strPath);




        System.Threading.Thread.Sleep(50);
        if (AsyncFileUpload1.HasFile)
        {
            System.Collections.Generic.List<string> messages = new System.Collections.Generic.List<string>();
            try
            {
                DataTable dt = ExcelLibrary.DataSetHelper.CreateDataTable(AsyncFileUpload1.PostedFile.InputStream, AsyncFileUpload1.FileName);
                //TODO: Check for Header -- when there is only one record.
                for (int i = 0; i < dt.Rows.Count; ++i)
                {
                    int oid;
                    if (!int.TryParse(dt.Rows[i][8].ToString(), out oid))
                        continue;
                    if (oid < 4000000)
                        continue;
                    if (string.IsNullOrEmpty(dt.Rows[i][7].ToString()))
                        continue;
                    ImportData(Convert.ToInt32(dt.Rows[i][8].ToString()), dt.Rows[i][7].ToString(), DateTime.FromOADate(Convert.ToDouble(dt.Rows[i][6])));

                }

                string message = string.Format("Tracking Count: {0} <br/>", successfulTracking.Count.ToString());

                string errors = string.Empty;

                foreach (string error in failedTracking)
                    errors += string.Concat(error, " <br/>");

                if (errors.Length > 0)
                    message += string.Format("Failed: {0} <br/>", errors);


                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "size", "top.$get(\"" + this.lblCount.ClientID + "\").innerHTML = 'Import Results: " + message + "';", true);


                // messages.Add("Imported Successfully: " + successfulTracking.Count.ToString());

                // if (failedTracking.Count > 0)
                //  {
                //     messages.Add("Failed to Imported : " + failedTracking.Count.ToString());

                //      foreach (string s in failedTracking)
                //          messages.Add(s);

                //      UpdateMessagePanel(false, messages);

                // }
                //  else
                // {

                //      UpdateMessagePanel(true, messages);
                //  }
            }
            catch (Exception x)
            {

                messages.Add("File could not be imported. Reason: " + x.Message.ToString());
                this.lblStatus.Text = x.Message.ToString();
                UpdateMessagePanel(false, messages);
            }
        }




    }




    private string GetData(string d)
    {
        if (string.IsNullOrEmpty(d))
            return d;

        return d.Replace("\"", "");
    }
}
