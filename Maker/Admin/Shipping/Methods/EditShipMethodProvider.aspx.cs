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
using MakerShop.Shipping;

public partial class Admin_Shipping_Methods_EditShipMethodProvider : MakerShop.Web.UI.MakerShopAdminPage
{

    int _ShipMethodId;
    ShipMethod _ShipMethod;

    protected void Page_Init(object sender, System.EventArgs e)
    {
        _ShipMethodId = AlwaysConvert.ToInt(Request.QueryString["ShipMethodId"]);
        _ShipMethod = ShipMethodDataSource.Load(_ShipMethodId);
        if (_ShipMethod == null) RedirectMe();
        //BIND TAX CODES
        TaxCode.DataSource = Token.Instance.Store.TaxCodes;
        TaxCode.DataBind();
        ListItem item = TaxCode.Items.FindByValue(_ShipMethod.TaxCodeId.ToString());
        if (item != null) TaxCode.SelectedIndex = TaxCode.Items.IndexOf(item);
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        Caption.Text = string.Format(Caption.Text, _ShipMethod.Name);
        if (!Page.IsPostBack) {
            Name.Text = _ShipMethod.Name;
            ShipMethodType.Text = _ShipMethod.ShipGateway.Name;
            ServiceCode.Items.AddRange(_ShipMethod.ShipGateway.GetProviderInstance().GetServiceListItems());
            ListItem selected = ServiceCode.Items.FindByValue(_ShipMethod.ServiceCode);
            if (selected != null) selected.Selected = true;
            if (_ShipMethod.Surcharge > 0) Surcharge.Text = string.Format("{0:F2}", _ShipMethod.Surcharge);
            SurchargeIsPercent.SelectedIndex = _ShipMethod.SurchargeIsPercent ? 1 : 0;
            SurchargeIsVisible.SelectedIndex = _ShipMethod.SurchargeIsVisible ? 1 : 0;
            UseWarehouseRestriction.SelectedIndex = (_ShipMethod.ShipMethodWarehouses.Count == 0) ? 0 : 1;
            BindWarehouses();
            UseZoneRestriction.SelectedIndex = (_ShipMethod.ShipMethodShipZones.Count == 0) ? 0 : 1;
            BindZones();
            UseGroupRestriction.SelectedIndex = (_ShipMethod.ShipMethodGroups.Count == 0) ? 0 : 1;
            BindGroups();
            if (_ShipMethod.MinPurchase > 0) MinPurchase.Text = string.Format("{0:F2}", _ShipMethod.MinPurchase);
        }
    }

    protected void RedirectMe()
    {
        Response.Redirect("Default.aspx");
    }

    protected void UseWarehouseRestriction_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindWarehouses();
    }

    protected void BindWarehouses()
    {
        WarehouseListPanel.Visible = (UseWarehouseRestriction.SelectedIndex > 0);
        if (WarehouseListPanel.Visible)
        {
            WarehouseList.DataSource = WarehouseDataSource.LoadForStore("Name");
            WarehouseList.DataBind();
            if (WarehouseList.Items.Count > 4) WarehouseList.Rows = 8;
            foreach (ShipMethodWarehouse item in _ShipMethod.ShipMethodWarehouses)
            {
                ListItem listItem = WarehouseList.Items.FindByValue(item.WarehouseId.ToString());
                if (listItem != null) listItem.Selected = true;
            }
        }
    }

    protected void UseZoneRestriction_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindZones();
    }

    protected void BindZones()
    {
        ZoneListPanel.Visible = (UseZoneRestriction.SelectedIndex > 0);
        if (ZoneListPanel.Visible)
        {
            ZoneList.DataSource = ShipZoneDataSource.LoadForStore("Name");
            ZoneList.DataBind();
            if (ZoneList.Items.Count > 4) ZoneList.Rows = 8;
            foreach (ShipMethodShipZone item in _ShipMethod.ShipMethodShipZones)
            {
                ListItem listItem = ZoneList.Items.FindByValue(item.ShipZoneId.ToString());
                if (listItem != null) listItem.Selected = true;
            }
        }
    }

    protected void UseGroupRestriction_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindGroups();
    }

    protected void BindGroups()
    {
        GroupListPanel.Visible = (UseGroupRestriction.SelectedIndex > 0);
        if (GroupListPanel.Visible)
        {
            GroupList.DataSource = GroupDataSource.LoadForStore("Name");
            GroupList.DataBind();
            foreach (ShipMethodGroup item in _ShipMethod.ShipMethodGroups)
            {
                ListItem listItem = GroupList.Items.FindByValue(item.GroupId.ToString());
                if (listItem != null) listItem.Selected = true;
            }
        }
    }

    protected void CancelButton_Click(object sender, System.EventArgs e)
    {
        RedirectMe();
    }

    protected void SaveButton_Click(object sender, System.EventArgs e)
    {
        if (Page.IsValid) {
            //UPDATE NAME
            _ShipMethod.Name = Name.Text;
            //UPDATE RATE
            _ShipMethod.ServiceCode = ServiceCode.SelectedValue;
            //UPDATE SURCHARGE
            _ShipMethod.Surcharge = AlwaysConvert.ToDecimal(Surcharge.Text);
            if (_ShipMethod.Surcharge < 0) _ShipMethod.Surcharge = 0;
            if (_ShipMethod.Surcharge > 0)
            {
                _ShipMethod.SurchargeIsPercent = (SurchargeIsPercent.SelectedIndex > 0);
                _ShipMethod.SurchargeIsVisible = (SurchargeIsVisible.SelectedIndex > 0);
            }
            else
            {
                _ShipMethod.SurchargeIsPercent = false;
                _ShipMethod.SurchargeIsVisible = false;
            }
            //UPDATE WAREHOUSES
            _ShipMethod.ShipMethodWarehouses.DeleteAll();
            if (UseWarehouseRestriction.SelectedIndex > 0)
            {
                foreach (ListItem item in WarehouseList.Items)
                {
                    if (item.Selected) _ShipMethod.ShipMethodWarehouses.Add(new ShipMethodWarehouse(_ShipMethodId, AlwaysConvert.ToInt(item.Value)));
                }
            }
            //UPDATE ZONES
            _ShipMethod.ShipMethodShipZones.DeleteAll();
            if (UseZoneRestriction.SelectedIndex > 0)
            {
                foreach (ListItem item in ZoneList.Items)
                {
                    if (item.Selected) _ShipMethod.ShipMethodShipZones.Add(new ShipMethodShipZone(_ShipMethodId, AlwaysConvert.ToInt(item.Value)));
                }
            }
            //UPDATE ROLES
            _ShipMethod.ShipMethodGroups.DeleteAll();
            if (UseGroupRestriction.SelectedIndex > 0)
            {
                foreach (ListItem item in GroupList.Items)
                {
                    if (item.Selected) _ShipMethod.ShipMethodGroups.Add(new ShipMethodGroup(_ShipMethodId, AlwaysConvert.ToInt(item.Value)));
                }
            }
            //UPDATE MIN PURCHASE
            _ShipMethod.MinPurchase = AlwaysConvert.ToDecimal(MinPurchase.Text);
            //UPDATE TAX CODE
            _ShipMethod.TaxCodeId = AlwaysConvert.ToInt(TaxCode.SelectedValue);
            //SAVE METHOD AND REDIRECT TO LIST
            _ShipMethod.Save();
            RedirectMe();
        }
    }

}
