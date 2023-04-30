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
using System.Collections.Generic;

public partial class Admin_Store_Maintenance : MakerShop.Web.UI.MakerShopAdminPage
{

    protected void Page_Load(object sender, System.EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            Store store = Token.Instance.Store;
            StoreSettingCollection settings = store.Settings;
                    
            
            StoreClosedOptions.DataSource = EnumToDictionary(typeof(StoreClosureType));
            StoreClosedOptions.DataTextField = "Key";
            StoreClosedOptions.DataValueField = "Value";
            StoreClosedOptions.DataBind();
            ListItem option = StoreClosedOptions.Items.FindByValue(((int)settings.StoreFrontClosed).ToString());
            if (option != null) option.Selected = true;
            
            StoreClosedMessage.Text = settings.StoreFrontClosedMessage;
                       
            //USER MAINTENANCE
            if (settings.AnonymousUserLifespan > 0) AnonymousLifespan.Text = settings.AnonymousUserLifespan.ToString();
            if (settings.AnonymousAffiliateUserLifespan > 0) AnonymousAffiliateLifespan.Text = settings.AnonymousAffiliateUserLifespan.ToString();

			//GIFT CERTIFICATE EXPIRY
            GiftCertExpireDays.Text = settings.GiftCertificateDaysToExpire.ToString();
			
			//SUBSCRIPTIONS MAINTENANCE
			//MaintenanceDays.Text = settings.SubscriptionsDaysBeforeDeleteExpired.ToString();
        }		

    }

    protected Dictionary<string,int> EnumToDictionary(Type enumType)
    {
        // get the names from the enumeration
        string[] names = Enum.GetNames(enumType);
        // get the values from the enumeration
        Array values = Enum.GetValues(enumType);
        // turn it into a dictionary
        Dictionary<string,int> ht = new Dictionary<string,int>();
        for (int i = 0; i < names.Length; i++)
            // note the cast to integer here is important
            // otherwise we'll just get the enum string back again
			ht.Add(StringHelper.SpaceName(names[i]), (int)values.GetValue(i));
        // return the dictionary to be bound to
        return ht;
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("../Default.aspx");
    }

    private void SaveSettings()
    {
        Store store = Token.Instance.Store;
        StoreSettingCollection settings = store.Settings;

        settings.StoreFrontClosed = (StoreClosureType)AlwaysConvert.ToInt(StoreClosedOptions.SelectedValue); 
        settings.StoreFrontClosedMessage = StoreClosedMessage.Text;
        
        //USER MAINTENANCE
        settings.AnonymousUserLifespan = AlwaysConvert.ToInt(AnonymousLifespan.Text);
        settings.AnonymousAffiliateUserLifespan = AlwaysConvert.ToInt(AnonymousAffiliateLifespan.Text);        
        //Gift Certificate Days to Expire
        settings.GiftCertificateDaysToExpire = AlwaysConvert.ToInt(GiftCertExpireDays.Text);
		
		//settings.SubscriptionsDaysBeforeDeleteExpired = AlwaysConvert.ToInt(MaintenanceDays.Text,90);
		
        store.Save();
    }
    
    protected void SaveButton_Click(object sender, EventArgs e)
    {
        SaveSettings();
        SavedMessage.Visible = true;
    }
    
    protected void SaveAndCloseButton_Click(object sender, EventArgs e) {
        SaveSettings();
        Response.Redirect("../Default.aspx");
    }

    protected void SSLEnabled_Click(object sender, EventArgs e)
    {
        SaveSettings();
        Response.Redirect("Security/Default.aspx");
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {

    }
    

}
