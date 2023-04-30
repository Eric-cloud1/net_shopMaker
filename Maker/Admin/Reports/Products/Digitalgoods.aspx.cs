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
using System.Text.RegularExpressions;
using MakerShop.Common;
using MakerShop.Catalog;
using MakerShop.Orders;
using MakerShop.Products;
using MakerShop.Stores;
using MakerShop.Users;
using MakerShop.Utility;
using MakerShop.Reporting;
using System.Text;
using MakerShop.Marketing;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Reflection;
using MakerShop.DigitalDelivery;
using System.Linq;

public partial class Admin_Reports_Products_Digitalgoods : System.Web.UI.Page
{



    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
      

            digitalGoods.DataSource = OrderItemDigitalGoodDataSource.loadDigitalGoodsId();
            digitalGoods.DataTextField = "name";
            digitalGoods.DataValueField = "digitalgoodid";
            digitalGoods.DataBind();
        }    
    }

 
    protected void ProcessButton_Click(object sender, EventArgs e)
    {

        DataTable SessionDataSource = OrderItemDigitalGoodDataSource.getDigitalGoodsbyDate(dtDigitalgoods.StartDate, dtDigitalgoods.EndDate, int.Parse(digitalGoods.SelectedValue)).Tables[0];

        MemoryStream mm = new MemoryStream();
        StreamWriter sw = new StreamWriter(mm);
        sw.AutoFlush = true;


        sw.Write("OrderNumber", "BillToFirstName", "BillToLastName", "BillToEmail", "BillToPostalCode", "Name", "ActivationDate", "canceldate", "SerialKeyData");
        sw.Write(sw.NewLine);


        for (int k = 0; k < SessionDataSource.Rows.Count; ++k)
        {

            sw.Write(SessionDataSource.Rows[k].Field<Int32>("OrderNumber"));
            sw.Write(",");
            sw.Write(SessionDataSource.Rows[k].Field<string>("BillToFirstName"));
            sw.Write(",");
            sw.Write(SessionDataSource.Rows[k].Field<string>("BillToLastName"));
            sw.Write(",");
            sw.Write(SessionDataSource.Rows[k].Field<string>("BillToEmail"));
            sw.Write(",");
            sw.Write(SessionDataSource.Rows[k].Field<string>("BillToPostalCode"));
            sw.Write(",");
            sw.Write(SessionDataSource.Rows[k].Field<string>("Name"));
            sw.Write(",");
            sw.Write(SessionDataSource.Rows[k].Field<DateTime?>("ActivationDate"));
            sw.Write(",");
            sw.Write(SessionDataSource.Rows[k].Field<DateTime?>("canceldate"));
            sw.Write(",");
            sw.Write(SessionDataSource.Rows[k].Field<string>("SerialKeyData"));


            sw.Write(sw.NewLine);
        }
        sw.Flush();

        mm.Seek(0, 0);
        System.Net.Mail.Attachment attachement = new System.Net.Mail.Attachment(mm, "DigitalGoods_" + DateTime.Now.Month.ToString() + '-' +
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
}
