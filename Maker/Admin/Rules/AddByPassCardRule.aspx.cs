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

using MakerShop.Validation;

public partial class Admin_Rules_AddByPassCardRule : System.Web.UI.Page
{

 
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!Page.IsPostBack)
        {

            dlWhiteListDataSource.Items.Clear();
            foreach (WhiteListTypes cs in Enum.GetValues(typeof(WhiteListTypes)))
            {
                dlWhiteListDataSource.Items.Add(new ListItem(cs.ToString(), ((short)cs).ToString()));
            }

            BindList();
        }

       
       
    }

   
    protected WhiteListTypes SelectedWhiteList()
    {
        string listType = dlWhiteListDataSource.SelectedValue;

        WhiteListTypes wlst = new WhiteListTypes();
        switch (listType)
        {
            case "CreditCard":
                wlst = WhiteListTypes.CreditCard;
                break;
            case "Phone":
                wlst = WhiteListTypes.Phone;
                break;
            case "eMail":
                wlst = WhiteListTypes.eMail;
                break;
            case "IP":
                wlst = WhiteListTypes.IP;
                break;

            default:
                wlst = WhiteListTypes.CreditCard;
                break;

        }

        return wlst;

    }

 

    protected void BindList()
    {
        BindList(SelectedWhiteList());
    }

    protected void BindList(WhiteListTypes whiteList)
    {

        WhiteListsCollection dataSource = WhiteListsDataSource.Load(whiteList);
        this.lstRule.DataSource = dataSource;
    
        lstRule.DataBind();

        
    }

   

  

    protected void ProcessButton_Click(object sender, EventArgs e)
    {
        BindList();
    }

    protected void AddNewButton_Click(object sender, EventArgs e)
    {
        WhiteLists whiteList = null;

        StringReader reader = new StringReader(txtAddCardNumber.Value);
        string readerValue = string.Empty;
        while (true)
        {
            readerValue = reader.ReadLine();

            if (readerValue != null)
            {
                whiteList = new WhiteLists();
                whiteList.CreateUser = Token.Instance.User.UserName;
                whiteList.ChangeUser = Token.Instance.User.UserName;
                whiteList.CreateDate = System.DateTime.Now.Date;
                whiteList.ChangeDate = System.DateTime.Now.Date;
                whiteList.Value = readerValue;

                whiteList.WhiteListTypeId = (short)SelectedWhiteList();

                WhiteListsDataSource.Insert(whiteList);
                whiteList = null;
            }

            else
                break;
  
        }
        txtAddCardNumber.InnerText = string.Empty;
        BindList();
    }
    

    protected void Delete_Click(object sender, EventArgs e)
    {

        string itemIds = string.Empty;
        WhiteLists whiteList = null;

        IEnumerator datalistEnumerator = lstRule.Rows.GetEnumerator();
        GridViewRow Currentitem = null;
      
        while(datalistEnumerator.MoveNext())
        {
            Currentitem = (GridViewRow)datalistEnumerator.Current;
            CheckBox chkDelete = (CheckBox)Currentitem.FindControl("DeleteThis");
            Label lbItemid = (Label)Currentitem.FindControl("itemId");
            if ((chkDelete.Checked) && (lbItemid.Text.Length > 0))
            {
                whiteList = new WhiteLists();
                whiteList.WhiteListId = int.Parse(lbItemid.Text);
                whiteList.Delete();
                whiteList = null;
            }
        }

     BindList();
    }


    protected void lstRule_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        lstRule.PageIndex = e.NewPageIndex;
        BindList();
    }


    protected void lstRule_EditCommand(object sender, GridViewEditEventArgs e)
    {
        lstRule.EditIndex = e.NewEditIndex;
        BindList();
    }

    protected void lstRule_CancelCommand(object sender, GridViewCancelEditEventArgs e)
    {
        lstRule.EditIndex = -1;
        BindList();

    }

    protected void lstRule_UpdateCommand(Object Sender, GridViewUpdateEventArgs e)
    {
        WhiteLists wl = new WhiteLists();
        int index = lstRule.EditIndex;
        GridViewRow row = this.lstRule.Rows[index];

        wl.ChangeUser = Token.Instance.User.UserName;
        wl.ChangeDate = System.DateTime.Now.Date;
        wl.WhiteListType = SelectedWhiteList();
        wl.WhiteListId = int.Parse(((System.Web.UI.WebControls.Label)row.FindControl("lbId")).Text);
        wl.Value = ((System.Web.UI.WebControls.TextBox)row.FindControl("txtValue")).Text;

        WhiteListsDataSource.Update(wl);

        lstRule.EditIndex = -1;
        
        BindList();
    }



    protected void ProcessWhiteListSelection_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        BindList();
    }
}
 


