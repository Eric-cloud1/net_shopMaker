using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using MakerShop.Catalog;
using MakerShop.Data;
using MakerShop.DigitalDelivery;
using MakerShop.Marketing;
using MakerShop.Messaging;
using MakerShop.Orders;
using MakerShop.Payments;
using MakerShop.Payments.Providers;
using MakerShop.Products;
using MakerShop.Reporting;
using MakerShop.Shipping;
using MakerShop.Stores;
using MakerShop.Taxes;
using MakerShop.Taxes.Providers;
using MakerShop.Users;
using MakerShop.Utility;
using MakerShop.Common;
using System.IO;

public partial class Admin_Orders_OrderHistory : MakerShop.Web.UI.MakerShopAdminPage
{

    private int _OrderId = 0;
    private Order _Order;

    protected void Page_Load(object sender, EventArgs e)
    {
        _Order = OrderHelper.GetOrderFromContext();
        if (_Order != null)
        {
            _OrderId = AlwaysConvert.ToInt(Request.QueryString["OrderId"]);
            if (!Page.IsPostBack)
            {
                if (Token.Instance.User.IsAdmin)
                    btnCSV.Visible = true;

                BindOrderNotes();
                BindPhoneNotes();

            }

        }
    }

    protected void BindOrderNotes()
    {
        OrderNotesGrid.DataSource = _Order.Notes;
        OrderNotesGrid.DataBind();
    }


    protected void Page_Init(object sender, EventArgs e)
    {
        AddOrderNotes1.ItemAdded += new PersistentItemEventHandler(AddOrderNotes1_ItemAdded);

    }

    private void AddOrderNotes1_ItemAdded(object sender, PersistentItemEventArgs e)
    {
        BindOrderNotes();
        OrderNotesAjax.Update();
    }


    protected void OrderNotesGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.OrderNotesGrid.PageIndex = e.NewPageIndex;
        BindOrderNotes();
    }

    
    protected void OrderNotesGrid_RowEditing(object sender, GridViewEditEventArgs e)
    {
        OrderNotesGrid.EditIndex = e.NewEditIndex;
        BindOrderNotes();
      
    }

    protected void OrderNotesGrid_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        OrderNotesGrid.EditIndex = -1;
        BindOrderNotes();
        
    }
    
    protected void OrderNotesGrid_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        OrderNote note = _Order.Notes[e.RowIndex];
        GridViewRow row = OrderNotesGrid.Rows[e.RowIndex];
        TextBox editComment = PageHelper.RecursiveFindControl(row, "EditComment") as TextBox;
        CheckBox editIsPrivate = PageHelper.RecursiveFindControl(row, "EditIsPrivate") as CheckBox;
        note.Comment = editComment.Text;
        note.NoteType = editIsPrivate.Checked ? NoteType.Private : NoteType.Public;
        note.Save();
        OrderNotesGrid.EditIndex = -1;
        BindOrderNotes();
       
    }

    protected void OrderNotesGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {

        _Order.Notes.DeleteAt(e.RowIndex );
        BindOrderNotes();
    }



    protected void BindPhoneNotes()
    {
        PhoneNotesGrid.DataSource = _Order.PhoneNotes;
        PhoneNotesGrid.DataBind();
    }



    protected void downloadCSV(object sender, EventArgs e)
    {
     
        MemoryStream mm = new MemoryStream();
        StreamWriter sw = new StreamWriter(mm);
        sw.AutoFlush = true;

        sw.Write("OrderId");
        sw.Write(",");

        sw.Write("UserId");
        sw.Write(",");

        sw.Write("Comment");
        sw.Write(",");

        sw.Write("CreatedDate");
        sw.Write(",");

        sw.Write("NoteType");
      
        sw.Write(sw.NewLine);

        foreach (OrderNote _notes in _Order.Notes)
        {
           sw.Write(_notes.OrderId); 
           sw.Write(",");

           sw.Write(_notes.UserId); 
           sw.Write(",");

           sw.Write(_notes.Comment);
           sw.Write(",");

           sw.Write(_notes.CreatedDate);
           sw.Write(",");

           sw.Write(_notes.NoteType);

           sw.Write(sw.NewLine);
        }


        sw.Flush();
        mm.Seek(0, 0);

        System.Net.Mail.Attachment attachement = new System.Net.Mail.Attachment(mm, "HistoryNotes_by_OrderId_"+ _Order.OrderId +".csv");

        if (ViewState["SentFile"] != null)
            ViewState.Remove("SentFile");
        
        ViewState.Add("SentFile", attachement.Name);

        System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
        mail.From = new System.Net.Mail.MailAddress("viroplex@viralplex.com");
        mail.Subject = "HistoryNotes_by_OrderId_" + _Order.OrderId + ".csv";
        mail.To.Add(new System.Net.Mail.MailAddress(MakerShop.Common.Token.Instance.User.Email));
        mail.Attachments.Add(attachement);
        MakerShop.Messaging.EmailClient.Send(mail);

        System.Collections.Generic.List<string> messages = new System.Collections.Generic.List<string>();
     
        SavedMessage.Visible = true;
        SavedMessage.Text = string.Format("File emailed at {0}.", LocaleHelper.LocalNow );


    }

   
   

    protected void PhoneNotesGrid_RowEditing(object sender, GridViewEditEventArgs e)
    {
        PhoneNotesGrid.EditIndex = e.NewEditIndex;
        BindPhoneNotes();
       
    }

    protected string FormatNoteDate(object dataItem)
    {

        OrderNote date = (OrderNote)dataItem;
        return string.Format("{0:MM/dd/yy hh:mm:ss  tt}", date.CreatedDate);

    }

    protected string FormatPhoneDate(object dataItem)
    {

        PhoneNotes date = (PhoneNotes)dataItem;
        return string.Format("{0:MM/dd/yy hh:mm:ss  tt}", date.CreatedDate);

    }

    protected void PhoneNotesGrid_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        PhoneNotesGrid.EditIndex = -1;
        BindPhoneNotes();
      
    }

    protected void PhoneNotesGrid_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        PhoneNotes note = _Order.PhoneNotes[e.RowIndex];
        GridViewRow row = PhoneNotesGrid.Rows[e.RowIndex];
        TextBox editComment = PageHelper.RecursiveFindControl(row, "EditComment") as TextBox;
        CheckBox editIsPrivate = PageHelper.RecursiveFindControl(row, "EditIsPrivate") as CheckBox;
        note.Comment = editComment.Text;
        note.NoteType = editIsPrivate.Checked ? NoteType.Private : NoteType.Public;
        note.Save();
        PhoneNotesGrid.EditIndex = -1;
        BindPhoneNotes();
     
    }

    protected void PhoneNotesGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        _Order.PhoneNotes.DeleteAt(e.RowIndex );
        BindPhoneNotes();
    }

}
