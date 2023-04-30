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
using MakerShop.Common;

public partial class Admin_Tools_Shipping_Returns : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }


    protected void submit_Click(object sender, EventArgs e)
    {
        string message = string.Empty;
        bool success = false;

     

        try
        {
            if (trackingNumber.Text.Trim().Length < 22)
                throw new Exception("Tracking number must be at least 22 characters");

            TrackingNumberCollection trackings = new TrackingNumberCollection();

            string criteria = string.Format(@"TrackingNumberData = '{0}'", trackingNumber.Text.Trim());
            trackings = TrackingNumberDataSource.LoadForCriteria(criteria);

            //1.    Enter Tracking #.
            TrackingNumber tracking = trackings[0];
      
            OrderShipment newShipment = new OrderShipment();
            newShipment.Load(tracking.OrderShipmentId);
            int _orderId = newShipment.OrderId;

           
            //3.	Cancel Order & Subscription
            Order order = new Order();
            order.Load(_orderId);
            message = order.OrderNumber.ToString();

            if(_orderId != 0)
                order.Cancel(false); // do not cancel the payment?
      

            int _orderNumber = order.OrderNumber;
            //2.	In Notes write Order# 12345 Returned.
            OrderNote note = new OrderNote();
            note.Comment = string.Format(@"In Notes write Order# {0} Returned.", _orderNumber);
            note.CreatedDate = DateTime.UtcNow;
            note.NoteType = NoteType.SystemPublic;
            note.OrderId = _orderId;
            note.UserId = Token.Instance.UserId;
            note.Save();

            success = true;
        }
        catch { success = false; }
        //4.	Respond back to screen with result (success/fail)


        if (success == true)
        {
            SavedMessage.ForeColor = System.Drawing.Color.Green;
        }
        else
        {
            message = string.Format(@"Invalid Number");
            SavedMessage.ForeColor = System.Drawing.Color.Red;
        }

        SavedMessage.Text = string.Format(SavedMessage.Text, DateTime.UtcNow.ToLocalTime(), message);
        SavedMessage.Visible = true;

    }
}
