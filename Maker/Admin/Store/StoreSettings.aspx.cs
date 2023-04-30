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
using MakerShop.Marketing;
using MakerShop.Shipping;
using MakerShop.Search;

public partial class Admin_Store_StoreSettings : MakerShop.Web.UI.MakerShopAdminPage
{

    protected void Page_Init(object sender, System.EventArgs e)
    {
        if (Token.Instance.Database.SqlServerMajorVersion < 9)
        {
            FullTextSearchVersionErrorLabel.Visible = true;
        }
        else
        {
            // FULLTEXT SEARCH
            bool ftsIsInstalled = KeywordSearchHelper.IsFullTextSearchInstalled(false);
            bool ftsIsEnabled = false;
            if (ftsIsInstalled)
            {
                ftsIsEnabled = KeywordSearchHelper.IsFullTextSearchEnabled(false);
                if (!ftsIsEnabled)
                {
                    // ATTEMPT TO ENABLE FULLTEXT SEARCH IF POSSIBLE
                    KeywordSearchHelper.EnableFullTextSearch(false);
                    ftsIsEnabled = KeywordSearchHelper.IsFullTextSearchEnabled(false);
                }
            }
            EnableFullTextSearch.Visible = ftsIsInstalled && ftsIsEnabled;
            FullTextSearchNotInstalledLabel.Visible = !EnableFullTextSearch.Visible;
        }
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {
        Store store = Token.Instance.Store;
        StoreSettingCollection settings = store.Settings;

        if (!Page.IsPostBack)
        {
            // GENERAL
            StoreName.Text = store.Name;
            StoreUrl.Text = store.StoreUrl;
            SSLEnabled.Text = settings.SSLEnabled ? "Yes" : "No";
            if (!String.IsNullOrEmpty(settings.SiteDisclaimerMessage))
            {
                SiteDisclaimerMessage.Text = settings.SiteDisclaimerMessage;
            }
            
            // VOLUME DISCOUNTS
            DiscountMode.SelectedIndex = (int)store.VolumeDiscountMode;
            //INVENTORY
            EnableInventory.Checked = store.EnableInventory;
            
            if (store.Settings.InventoryDisplayDetails)
            {                
                CurrentInventoryDisplayMode.SelectedIndex = 1;
                ProductInventoryPanel.Visible = true;
                InStockMessage.Text = settings.InventoryInStockMessage;
                OutOfStockMessage.Text = settings.InventoryOutOfStockMessage;            
            }
            
            //ORDER SETTINGS
            UpdateNextOrderNumber(store);
            
            OrderIdIncrement.Text = store.OrderIdIncrement.ToString();
            OrderMinAmount.Text = (settings.OrderMinimumAmount > 0) ? settings.OrderMinimumAmount.ToString() : string.Empty;
            OrderMaxAmount.Text = (settings.OrderMaximumAmount > 0) ? settings.OrderMaximumAmount.ToString() : string.Empty;
            if (!String.IsNullOrEmpty(settings.CheckoutTermsAndConditions))
            {
                CheckoutTerms.Text = settings.CheckoutTermsAndConditions;
            }
            EnableFullTextSearch.Checked = settings.FullTextSearch;

            // UNITS
            WeightUnit.DataSource = EnumToHashtable(typeof(MakerShop.Shipping.WeightUnit));
            WeightUnit.DataTextField = "Key";
            WeightUnit.DataValueField = "Value";            
            WeightUnit.DataBind();
            
            MeasurementUnit.DataSource = EnumToHashtable(typeof(MakerShop.Shipping.MeasurementUnit));
            MeasurementUnit.DataTextField = "Key";
            MeasurementUnit.DataValueField = "Value";            
            MeasurementUnit.DataBind();            
            
            ListItem item = WeightUnit.Items.FindByValue(store.WeightUnitId.ToString());
            if (item != null) item.Selected = true;
            item = MeasurementUnit.Items.FindByValue(store.MeasurementUnitId.ToString());
            if (item != null) item.Selected = true;
            BindTimeZone();

            PostalCodeCountries.Text = settings.PostalCodeCountries;

            //PRODUCTS PURCHASING
            ProductPurchasingDisabled.Checked = settings.ProductPurchasingDisabled;

            //MINIMUM SEARCH LENGHT
            MinimumSearchLength.Text = settings.MinimumSearchLength.ToString();

            USPSUserId.Text = settings.USPSUserId.ToString();
        }
    }

    private void UpdateNextOrderNumber(Store store)
    {
        OrigNextOrderNumber.Value = store.NextOrderId.ToString();
        NextOrderNumberRangeValidator1.MinimumValue = (StoreDataSource.GetMaxOrderNumber() + 1).ToString();
        NextOrderNumberRangeValidator1.ErrorMessage = String.Format(NextOrderNumberRangeValidator1.ErrorMessage, NextOrderNumberRangeValidator1.MinimumValue);
        NextOrderId.Text = OrigNextOrderNumber.Value;
        if (store.NextOrderId > 99999999)
        {
            NextOrderIdLabel.Text = OrigNextOrderNumber.Value;
            NextOrderIdLabel.Visible = true;
            NextOrderId.Visible = false;
            NextOrderNumberRangeValidator1.Enabled = false;
            NextOrderNumberRangeValidator1.Visible = false;
            
        }
    }
    
    protected void CurrentInventoryDisplayMode_SelectedIndexChanged(object sender, EventArgs e)
    {        
        ProductInventoryPanel.Visible = (CurrentInventoryDisplayMode.SelectedIndex == 1);
        if (ProductInventoryPanel.Visible)
        {
            CurrentInventoryDisplayMode.SelectedIndex = 1;
        }
                
    }

    protected Hashtable EnumToHashtable(Type enumType)
    {
        // get the names from the enumeration
        string[] names = Enum.GetNames(enumType);
        // get the values from the enumeration
        Array values = Enum.GetValues(enumType);
        // turn it into a hash table
        Hashtable ht = new Hashtable();
        for (int i = 0; i < names.Length; i++)
            // note the cast to integer here is important
            // otherwise we'll just get the enum string back again
            ht.Add(names[i], (int)values.GetValue(i));
        // return the dictionary to be bound to
        return ht;
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        Response.Redirect("../Default.aspx");
    }

    private double GetTimeZoneOffset(string timeZoneCode)
    {
        switch (timeZoneCode)
        {

            case "AST": return -4;
            case "ADT": return -3;
            case "AKST": return -9;
            case "AKDT": return -8;
            case "CST": return -6;
            case "CDT": return -5;
            case "EST": return -5;
            case "EDT": return -4;
            case "HAST": return -10;
            case "HADT": return -9;
            case "MST": return -7;
            case "MDT": return -6;
            case "NST": return -3.5;
            case "NDT": return -2.5;
            case "PST": return -8;
            case "PDT": return -7;
            default: return AlwaysConvert.ToDouble(timeZoneCode);
        }
    }

    private static string GetUnencryptedUri(string storeUrl)
    {
        Match m = Regex.Match(storeUrl, "^https?://(.+?)/");
        if (m.Success)
        {
            return m.Groups[1].Value;
        }
        return string.Empty;
    }

    private void SaveSettings()
    {
        Store store = Token.Instance.Store;
        StoreSettingCollection settings = store.Settings;
        store.Name = StoreName.Text;
        store.StoreUrl = StoreUrl.Text;
        if (!string.IsNullOrEmpty(settings.EncryptedUri))
            settings.UnencryptedUri = GetUnencryptedUri(store.StoreUrl);
        else settings.UnencryptedUri = string.Empty;
        store.VolumeDiscountMode = (VolumeDiscountMode)(DiscountMode.SelectedIndex);
        store.WeightUnit = (WeightUnit)AlwaysConvert.ToInt(WeightUnit.SelectedValue);
        store.MeasurementUnit = (MeasurementUnit)AlwaysConvert.ToInt(MeasurementUnit.SelectedValue);
        settings.TimeZoneCode = TimeZoneOffset.SelectedValue;
        settings.TimeZoneOffset = GetTimeZoneOffset(TimeZoneOffset.SelectedValue);
        settings.PostalCodeCountries = PostalCodeCountries.Text.Replace(" ", string.Empty);
        settings.SiteDisclaimerMessage = SiteDisclaimerMessage.Text.Trim();
        
        //INVENTORY        
        store.EnableInventory = EnableInventory.Checked;
        settings.InventoryDisplayDetails = (CurrentInventoryDisplayMode.SelectedIndex == 1);
        settings.InventoryInStockMessage = InStockMessage.Text;
        settings.InventoryOutOfStockMessage = OutOfStockMessage.Text;
        //ORDERS        
        short increment = AlwaysConvert.ToInt16(OrderIdIncrement.Text);
        if (increment >= 1) store.OrderIdIncrement = increment;
        settings.CheckoutTermsAndConditions = CheckoutTerms.Text.Trim();
        settings.OrderMinimumAmount = AlwaysConvert.ToDecimal(OrderMinAmount.Text);
        settings.OrderMaximumAmount = AlwaysConvert.ToDecimal(OrderMaxAmount.Text);
        
		//PRODUCTS PURCHASING
        settings.ProductPurchasingDisabled = ProductPurchasingDisabled.Checked;

        //MINIMUM SEARCH LENGHT
        settings.MinimumSearchLength = AlwaysConvert.ToInt(MinimumSearchLength.Text);

        settings.USPSUserId = USPSUserId.Text;
        store.Save();
        //CHECK NEXT ORDER NUMBER
        if (OrigNextOrderNumber.Value != NextOrderId.Text)
        {
            //NEXT ORDER NUMBER WAS UPDATED
            store.NextOrderId = StoreDataSource.SetNextOrderNumber(AlwaysConvert.ToInt(NextOrderId.Text));
            OrigNextOrderNumber.Value = store.NextOrderId.ToString();
        }
        else
        {
            //DETERMINE CORRECT VALUE FOR NEXT ORDER NUMBER
            OrigNextOrderNumber.Value = StoreDataSource.GetNextOrderNumber(false).ToString();
        }        
        OrderIdIncrement.Text = store.OrderIdIncrement.ToString();
        UpdateNextOrderNumber(store);

        // HANDLE ANY CHANGES TO FULLTEXT SEARCH
        UpdateFullTextSearchSetting(settings);        
    }

    private void UpdateFullTextSearchSetting(StoreSettingCollection settings)
    {
        // BY DEFAULT, DISABLE THE SEARCH
        settings.FullTextSearch = false;
        if (EnableFullTextSearch.Visible)
        {
            // FTS IS AVAILABLE
            if (EnableFullTextSearch.Checked)
            {
                // FTS IS TURNED ON, MAKE SURE THE CATALOG IS AVAILABLE
                bool catalogExists = KeywordSearchHelper.EnsureCatalog();
                if (catalogExists)
                {
                    // CATALOG IS FOUND, MAKE SURE INDEXES ARE AVAILABLE
                    bool indexesExist = KeywordSearchHelper.EnsureIndexes();
                    if (indexesExist)
                    {
                        // FTS CAN BE SAFELY ENABLED
                        settings.FullTextSearch = true;
                    }
                }
            }
        }
        settings.Save();
        if (!settings.FullTextSearch) KeywordSearchHelper.RemoveCatalog();
        EnableFullTextSearch.Checked = settings.FullTextSearch;
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


    protected void BindTimeZone()
    {
        for (int i = -12; i < 13; i++)
        {
            int iPos = (int)Math.Abs(i);
            double halfSet;
            if (i < 0)
            {
                halfSet = i - 0.5;
                if (iPos < 12) TimeZoneOffset.Items.Add(new ListItem(string.Format("UTC - {0}:30", iPos), halfSet.ToString()));
                TimeZoneOffset.Items.Add(new ListItem(string.Format("UTC - {0}", iPos), i.ToString()));
            }
            else if (i > 0)
            {
                halfSet = i + 0.5;
                TimeZoneOffset.Items.Add(new ListItem(string.Format("UTC + {0}", i), i.ToString()));
                if (iPos < 12) TimeZoneOffset.Items.Add(new ListItem(string.Format("UTC + {0}:30", i), halfSet.ToString()));
            }
            else
            {
                TimeZoneOffset.Items.Add(new ListItem("UTC", "0"));
                TimeZoneOffset.Items.Add(new ListItem("UTC + 0:30", "0.5"));
            }
        }
        //UPDATE SELECTED
        Store store = Token.Instance.Store;
        StoreSettingCollection settings = store.Settings;
        string tempCode = settings.TimeZoneCode;
        if (!string.IsNullOrEmpty(tempCode))
        {
            ListItem selected = TimeZoneOffset.Items.FindByValue(tempCode);
            if (selected != null) selected.Selected = true;
        }
    }

    protected void SSLEnabled_Click(object sender, EventArgs e)
    {
        SaveSettings();
        Response.Redirect("Security/Default.aspx");
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        int maxOrderId = StoreDataSource.GetMaxOrderNumber();
        int nextOrderId = StoreDataSource.GetNextOrderNumber(false);
        if (maxOrderId >= nextOrderId)
        {
            NextOrderNumberWarning.Text = string.Format(NextOrderNumberWarning.Text, nextOrderId, maxOrderId, (maxOrderId + 1));
        }
        else NextOrderNumberWarning.Visible = false;
    }

}
