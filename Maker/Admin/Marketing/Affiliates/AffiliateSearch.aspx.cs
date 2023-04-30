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
using MakerShop.Web.UI;
using MakerShop.Marketing;
using MakerShop.Orders;
using MakerShop.Common;
using MakerShop.Stores;
using MakerShop.Utility;
using System.Collections.Generic;


public partial class Admin_Marketing_Affiliates_AffiliateSearch : System.Web.UI.Page
{

    //add sorting on page init for the DataSourceOb
    protected void Page_Init(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            AffiliateGrid.AllowSorting = false;

            AffiliateDs.SelectParameters.Add("maximumRows", DbType.Int32, "200");
            AffiliateDs.SelectParameters.Add("startRowIndex", DbType.Int32, "0");
            AffiliateDs.SelectParameters.Add("sortExpression", DbType.String, string.Empty);

            AffiliateDs.OldValuesParameterFormatString = "original_{0}";
            AffiliateDs.SelectMethod = "LoadForStore";
            AffiliateDs.SelectCountMethod = "CountForStore";
            AffiliateDs.DeleteMethod = "Delete";
            AffiliateDs.UpdateMethod = "Update";
            AffiliateDs.DataObjectTypeName = "MakerShop.Marketing.Affiliate";

            AffiliateDs.SortParameterName = "SortExpression";
            AffiliateDs.TypeName = "MakerShop.Marketing.AffiliateDataSource";
            AffiliateDs.Selecting += new ObjectDataSourceSelectingEventHandler(AffiliateDs_Selecting);

            AffiliateDs.DataBind();
        }
    }

    void AffiliateDs_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
    {
       // e.InputParameters["criteria"] = GetOrderSearchCriteria();
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            PageHelper.SetDefaultButton(AffiliateName, SearchButton.ClientID);
            AffiliateName.Focus();

            AffiliateCollection affiliates = AffiliateDataSource.LoadForCriteria("");
     
            AffiliateIdDropDown.DataSource = affiliates;
            AffiliateIdDropDown.DataTextField = "AffiliateId";
            AffiliateIdDropDown.DataValueField = "AffiliateId";
            AffiliateIdDropDown.DataBind();

            AffiliateIdDropDown.Items.Insert(0,(new ListItem("-- Every Id--", "0")));
   
        }
 
    }

    protected void AffiliateGrid_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        this.AffiliateGrid.PageIndex = e.NewPageIndex;
      //  this.AffiliateGrid.DataSource = Convert(SessionDataSource).DefaultView;
        AffiliateGrid.DataBind();
    }



    protected void AffiliateGrid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if ((e.Row != null) && (e.Row.RowType == DataControlRowType.DataRow) && (e.Row.DataItem != null))
        {
            Affiliate affiliate = (Affiliate)e.Row.DataItem;

            PlaceHolder phProcessorCaption = PageHelper.RecursiveFindControl(e.Row, "phLogoCaption") as PlaceHolder;
            if (phProcessorCaption != null)
            {
                Image PaymentInstrument = new Image();
                PaymentInstrument.SkinID = GetAffiliateSkinID(affiliate);

                if (!string.IsNullOrEmpty(PaymentInstrument.SkinID))
                    phProcessorCaption.Controls.Add(PaymentInstrument);
            }

        }
    }


   



    protected string GetAffiliateSkinID(Affiliate affiliate)
    {
        //if (order.OrderStatus.Name.Equals("Cancelled", StringComparison.InvariantCultureIgnoreCase)) return "CodeGreen";
        //  if (!order.OrderStatus.IsValid) return "CodeRed";

        switch (affiliate.AffiliateTypeId)
        {
            case (byte)AffiliateType.Agent: return "Agent";
            case (byte)AffiliateType.Company: return "Company";
            case (byte)AffiliateType.Location: return "location";
            case (byte)AffiliateType.Master_Agent: return "MasterAgent";
            case (byte)AffiliateType.Master_Company: return "MasterCompany";
        }

        return "";
    }


    protected void SearchButton_Click(object sender, EventArgs e)
    {
       

        int pageSize = AlwaysConvert.ToInt(PageSize.SelectedValue);
        if (pageSize == 0) AffiliateGrid.AllowPaging = false;
        else
        {
            AffiliateGrid.AllowPaging = true;
            AffiliateGrid.PageSize = pageSize;
        }

        string sqlCriteria = "";


        if(AffiliateIdDropDown.SelectedValue != "0")
            sqlCriteria = string.Format(@"1 = case when  AffiliateId = {0} then 1 else 0 end ", AffiliateIdDropDown.SelectedValue);


        else if(!string.IsNullOrEmpty(AffiliateName.Text.Trim()))
            sqlCriteria = string.Format(@"1 = case when Name like '%{0}%' or  AffiliateId = {1} then 1 else 0 end ", AffiliateName.Text, AffiliateIdDropDown.SelectedValue);


        AffiliateDs.SortParameterName = string.Empty;    
        AffiliateGrid.DataSourceID = string.Empty;
        AffiliateGrid.AllowSorting = false;
        AffiliateGrid.DefaultSortExpression = string.Empty;
        AffiliateGrid.DataSource = MakerShop.Marketing.AffiliateDataSource.LoadForCriteria(sqlCriteria);
    
      
        AffiliateGrid.DataBind();
        SearchResultAjax.Update();

    }


    protected int GetOrderCount(object dataItem)
    {
        Affiliate a = (Affiliate)dataItem;
        return OrderDataSource.CountForAffiliate(a.AffiliateId);
    }

    protected string GetCommissionRate(object dataItem)
    {
        Affiliate affiliate = (Affiliate)dataItem;
        if (affiliate.CommissionIsPercent)
        {
            string format = "{0:0.##}% of {1}";
            if (affiliate.CommissionOnTotal) return string.Format(format, affiliate.CommissionRate, "total");
            return string.Format(format, affiliate.CommissionRate, "products");
        }
        return string.Format("{0:lc} per order", affiliate.CommissionRate);
    }
}