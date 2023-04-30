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
using MakerShop.Common;
using MakerShop.Marketing;
using MakerShop.Orders;
using MakerShop.Utility;
using MakerShop.Catalog;
using MakerShop.Products;
using MakerShop.Payments.Providers.PayPal;
using System.Collections.Generic;

public partial class ConLib_CommunicationPreferencesSection : System.Web.UI.UserControl
{
    protected void Page_Init(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            EmailListCollection publicLists = GetPublicEmailLists();
            if (publicLists.Count > 0)
            {
                dlEmailLists.DataSource = publicLists;
                dlEmailLists.DataBind();
            }
            else
            {
                EmailListPanel.Visible = false;
            }
        }
    }

    protected bool IsInList(object dataItem)
    {
        EmailList list = (EmailList)dataItem;
        return list.IsMember(Token.Instance.User.Email);
    }

    protected EmailListCollection GetPublicEmailLists()
    {
        EmailListCollection publicLists = new EmailListCollection();
        EmailListCollection allLists = EmailListDataSource.LoadForStore();
        foreach (EmailList list in allLists)
        {
            if (list.IsPublic) publicLists.Add(list);
        }
        return publicLists;
    }

    protected void UpdateButton_Click(object sender, EventArgs e)
    {

        List<int> offList = new List<int>();
        List<int> onList = new List<int>();
        //LOOP THROUGH SIGNUP LIST
        int index = 0;
        foreach (DataListItem item in dlEmailLists.Items)
        {
            int tempListId = (int)dlEmailLists.DataKeys[index];
            CheckBox selected = (CheckBox)item.FindControl("Selected");
            if ((selected != null) && (selected.Checked))
            {
                onList.Add(tempListId);
            }
            else
            {
                offList.Add(tempListId);
            }
            index++;
        }
        string email = Token.Instance.User.Email;
        //PROCESS LISTS THAT SHOULD NOT BE SUBSCRIBED
        foreach (int emailListId in offList)
        {
            EmailListUser elu = EmailListUserDataSource.Load(emailListId, email);
            if (elu != null) elu.Delete();
        }
        //PROCESS LISTS THAT SHOULD BE SUBSCRIBED
        foreach (int emailListId in onList)
        {
            EmailListUser elu = EmailListUserDataSource.Load(emailListId, email);
            if (elu == null)
            {
                EmailList list = EmailListDataSource.Load(emailListId);
                if (list != null) list.ProcessSignupRequest(email);
            }
        }
        //DISPLAY CONFIRMATION
        UpdatedMessage.Visible = true;
    }
}