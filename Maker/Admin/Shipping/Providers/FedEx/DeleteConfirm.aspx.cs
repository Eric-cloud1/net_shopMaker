using MakerShop.Shipping.Providers.FedEx;
using System;
using MakerShop.Shipping;
using MakerShop.Utility;
using System.Web;

partial class Admin_Shipping_Providers_FedEx_DeleteConfirm : MakerShop.Web.UI.MakerShopAdminPage {
    
    protected void FinishButton_Click(object sender, System.EventArgs e) {
        RedirectToManageProvider();
    }
    
    protected void RedirectToManageProvider() {
        FedEx provider;
        provider = new FedEx();
        Response.Redirect(("../Default.aspx?ProviderClassID=" + HttpUtility.UrlEncode(Misc.GetClassId(provider.GetType()))));
    }
}
