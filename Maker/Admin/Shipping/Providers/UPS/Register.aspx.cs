
using MakerShop.Shipping.Providers.UPS;
using System;
using MakerShop.Shipping;
using MakerShop.Utility;
using System.Web;


partial class Admin_Shipping_Providers_UPS_Register : MakerShop.Web.UI.MakerShopAdminPage {
        
    
    protected void Page_Load(object sender, System.EventArgs e) {
       
        if (!Page.IsPostBack) {
            LicenseAgreement.Text = UPS.GetAgreement();           
        }
    }

    protected void NextButton_Click(object sender, System.EventArgs e) {
        if ((rblAgree.SelectedIndex == 0)) {
            Response.Redirect("Register1.aspx");
        }
    }
    
    protected void RedirectToManageProvider() {
        UPS provider;
        provider = new UPS();
        Response.Redirect(("../Default.aspx?ProviderClassID=" + HttpUtility.UrlEncode(Misc.GetClassId(provider.GetType()))));
    }
    
    protected void CancelButton_Click(object sender, System.EventArgs e) {
        RedirectToManageProvider();
    }

}
