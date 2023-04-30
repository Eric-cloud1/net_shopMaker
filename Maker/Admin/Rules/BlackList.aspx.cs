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


public partial class Admin_Rules_BlackList : System.Web.UI.Page
{

 

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!Page.IsPostBack)
        {

         
            dlBlackListDataSource.Items.Clear();
            foreach (BlackListTypes cs in Enum.GetValues(typeof(BlackListTypes)))
            {
                if (cs.ToString().Contains("2"))
                    continue;
                dlBlackListDataSource.Items.Add(new ListItem(cs.ToString(), ((int)cs).ToString()));
            }

            BindList();
        }  
    }

   
    protected BlackListTypes SelectedBlackList()
    {
       return (BlackListTypes)Enum.Parse(typeof(BlackListTypes), dlBlackListDataSource.SelectedValue);

    }

 

    protected void BindList()
    {
        BindList(SelectedBlackList());
    }

    protected void BindList(BlackListTypes blackList)
    {
       BlackListsCollection ds = BlackListsDataSource.Load(blackList);
      
        this.lstBlackList.DataSource = ds;
        lstBlackList.DataBind();

       
    }



    protected void ProcessButton_Click(object sender, EventArgs e)
    {
        BindList();
    }

    protected void AddNewButton_Click(object sender, EventArgs e)
    {
        BlackLists blackList = null;

        StringReader reader = new StringReader(txtAddCardNumber.Value);
        string readerValue = string.Empty;
        while (true)
        {
            readerValue = reader.ReadLine();

            if (readerValue != null)
            {
                blackList = new BlackLists();
                blackList.CreateUser = Token.Instance.User.UserName;
                blackList.ChangeUser = Token.Instance.User.UserName;
                blackList.CreateDate = System.DateTime.Now.Date;
                blackList.ChangeDate = System.DateTime.Now.Date;
                blackList.Value = readerValue;

                blackList.BlackListTypeId = (short)SelectedBlackList();

                BlackListsDataSource.Insert(blackList);
                blackList = null;
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
        BlackLists blackList = null;

        IEnumerator datalistEnumerator = lstBlackList.Rows.GetEnumerator();
        GridViewRow Currentitem = null;
      
        while(datalistEnumerator.MoveNext())
        {
            Currentitem = (GridViewRow)datalistEnumerator.Current;
            CheckBox chkDelete = (CheckBox)Currentitem.FindControl("DeleteThis");
            Label lbItemid = (Label)Currentitem.FindControl("itemId");
            if ((chkDelete.Checked) && (lbItemid.Text.Length > 0))
            {
                blackList = new BlackLists();
                blackList.BlackListId = int.Parse(lbItemid.Text);
                blackList.Delete();
                blackList = null;
            }
        }

     BindList();
    }


    protected void lstBlackList_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        lstBlackList.PageIndex = e.NewPageIndex;
        BindList(); 
    }

    protected void lstBlackList_EditCommand(object sender, GridViewEditEventArgs e)
    {
        lstBlackList.EditIndex = e.NewEditIndex;
        BindList();
    }

    protected void lstBlackList_CancelCommand(object sender, GridViewCancelEditEventArgs e)
    {
        lstBlackList.EditIndex = -1;
        BindList();

    }

    protected void lstBlackList_UpdateCommand(Object Sender, GridViewUpdateEventArgs e)
    {
        BlackLists bl = new BlackLists();

       
        int index = lstBlackList.EditIndex;
        GridViewRow row = lstBlackList.Rows[index];



        bl.ChangeUser = Token.Instance.User.UserName;
        bl.ChangeDate = System.DateTime.Now.Date;
        bl.BlackListType = SelectedBlackList();
        bl.BlackListId = int.Parse(((System.Web.UI.WebControls.Label)row.FindControl("lbId")).Text);
        bl.Value = ((System.Web.UI.WebControls.TextBox)row.FindControl("txtValue")).Text;

        BlackListsDataSource.Update(bl);

        this.lstBlackList.EditIndex = -1;
        BindList();
    }



    protected void ProcessBlackListSelection_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        BindList();
    }
}
 


