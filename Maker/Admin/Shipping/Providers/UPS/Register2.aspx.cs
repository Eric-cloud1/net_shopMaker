
using MakerShop.Shipping.Providers.UPS;
using System;
using MakerShop.Shipping;
using MakerShop.Utility;
using System.Web;

partial class Admin_Shipping_Providers_UPS_Register2 : MakerShop.Web.UI.MakerShopAdminPage
{
    protected void SaveButton_Click(object sender, System.EventArgs e)
    {
        if (Page.IsValid)
        {
            UPS provider = new UPS();
            provider.UserId = UserId.Text;
            provider.Password = Password.Text;
            provider.AccessKey = AccessKey.Text;
            ShipGateway shipGateway = new ShipGateway();
            shipGateway.Name = provider.Name;
            shipGateway.ClassId = Misc.GetClassId(typeof(UPS));
            shipGateway.UpdateConfigData(provider.GetConfigData());
            shipGateway.Enabled = true;
            shipGateway.Save();
            Response.Redirect("Configure.aspx?ShipGatewayId=" + shipGateway.ShipGatewayId.ToString());
        }
    }
}
