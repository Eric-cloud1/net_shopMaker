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
using MakerShop.Stores;
using MakerShop.Utility;
using MakerShop.Common;
using MakerShop.Orders;

public partial class Admin_UserControls_AddOrderNotes : System.Web.UI.UserControl
{

    public event PersistentItemEventHandler ItemAdded;


    private int _OrderId = 0;
    private Order _Order;
    protected void Page_Load(object sender, EventArgs e)
    {
        _Order = OrderHelper.GetOrderFromContext();
        if ((_Order != null)&&(!Page.IsPostBack))
        {
            _OrderId = _Order.OrderId;

            AddIsPrivate.Checked = true;
        }

    }


  

    protected void AddButton_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(AddComment.Text))
        {
            _Order.Notes.Add(new OrderNote(_Order.OrderId, Token.Instance.UserId, LocaleHelper.LocalNow, AddComment.Text, AddIsPrivate.Checked ? NoteType.Private : NoteType.Public));
            _Order.Notes.Save();

           
            AddComment.Text = string.Empty;
        }

        AddIsPrivate.Checked = true;

        if (ItemAdded != null) ItemAdded(this, new PersistentItemEventArgs(_Order.OrderId, "Comment"));
        HideDisplayOptions_Click(null, null);
    }

    protected void HideDisplayOptions_Click(object sender, ImageClickEventArgs e)
    {
       
        if (sender != null) AddPopup.Show();
    }

}
