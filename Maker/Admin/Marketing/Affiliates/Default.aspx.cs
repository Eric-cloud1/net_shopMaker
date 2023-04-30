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

public partial class Admin_Marketing_Affiliates_Default : MakerShopAdminPage
{
    protected void AddAffiliateButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            Affiliate affiliate = new Affiliate();
            affiliate.Name = AddAffiliateName.Text;
            
            affiliate.Save();
            Response.Redirect("EditAffiliate.aspx?AffiliateId=" + affiliate.AffiliateId.ToString());
        }
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        PageHelper.SetDefaultButton(AddAffiliateName, AddAffiliateButton.ClientID);
        AddAffiliateName.Focus();
        if (!Page.IsPostBack)
        {
            TrackerUrl.Text = Token.Instance.Store.Settings.AffiliateTrackerUrl;
        }
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

    protected void TrackerSaveButton_Click(object sender, EventArgs e)
    {
        StoreSettingCollection settings = Token.Instance.Store.Settings;
        settings.AffiliateTrackerUrl = StringHelper.Truncate(TrackerUrl.Text.Trim(), 200);
        settings.Save();
        TrackerUrlSaved.Visible = true;
    }
}
