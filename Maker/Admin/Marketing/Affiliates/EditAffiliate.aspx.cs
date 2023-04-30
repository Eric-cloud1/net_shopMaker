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
using MakerShop.Utility;
using MakerShop.Orders;
using MakerShop.Shipping;
using MakerShop.Common;
using System.Collections.Generic;


public partial class Admin_Marketing_Affiliates_EditAffiliate : MakerShopAdminPage
{
    int _AffiliateId = 0;
    Affiliate _Affiliate;

    protected void Page_Load(object sender, System.EventArgs e)
    {
        _AffiliateId = AlwaysConvert.ToInt(Request.QueryString["AffiliateId"]);
        _Affiliate = AffiliateDataSource.Load(_AffiliateId);


        if (_Affiliate == null) Response.Redirect("Default.aspx");
        InstructionText.Text = string.Format(InstructionText.Text, _AffiliateId, GetHomeUrl());
        if (!Page.IsPostBack)
        {
            InitEditForm();
        }
        Caption.Text = string.Format(Caption.Text, _Affiliate.Name);
    }

    private string GetHomeUrl()
    {
        return string.Format("http://{0}{1}", Request.Url.Authority, this.ResolveUrl(NavigationHelper.GetHomeUrl()));
    }

    protected string GetSubtotal(object dataItem)
    {
        Order order = (Order)dataItem;
        return order.Items.TotalPrice(OrderItemType.Product, OrderItemType.Discount, OrderItemType.Coupon).ToString("lc");
    }

    protected void CancelButton_Click(object sender, System.EventArgs e)
    {
        Response.Redirect("Default.aspx");
    }

    protected void AddButton_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
            //BANKING INFORMATION
            
         // _Affiliate.BankInformation.CustomerName
         //   _Affiliate.BankInformation.Address                 
         //   _Affiliate.BankInformation.City
         //   _Affiliate.BankInformation.ZipCode
         //   _Affiliate.BankInformation.State
         //   _Affiliate.BankInformation.AreaCode
         //   _Affiliate.BankInformation.Prefix
         //   _Affiliate.BankInformation.Suffix
        }

    }

    protected void SaveButton_Click(object sender, System.EventArgs e)
    {
        if (Page.IsValid)
        {
            _Affiliate.Name = Name.Text;
            _Affiliate.ReferralDays = AlwaysConvert.ToInt16(ReferralDays.Text);
            _Affiliate.CommissionRate = AlwaysConvert.ToDecimal(CommissionRate.Text);
            _Affiliate.CommissionIsPercent = (CommissionType.SelectedIndex > 0);
            _Affiliate.CommissionOnTotal = (CommissionType.SelectedIndex == 2);
            _Affiliate.WebsiteUrl = WebsiteUrl.Text;
            _Affiliate.Email = Email.Text;
            _Affiliate.Active = Active.Checked;
            //ADDRESS INFORMATION
            _Affiliate.FirstName = FirstName.Text;
            _Affiliate.LastName = LastName.Text;
            _Affiliate.Company = Company.Text;
            _Affiliate.Address1 = Address1.Text;
            _Affiliate.Address2 = Address2.Text;
            _Affiliate.City = City.Text;
            _Affiliate.Province = Province.Text;
            _Affiliate.PostalCode = PostalCode.Text;
            _Affiliate.CountryCode = CountryCode.SelectedValue;
            _Affiliate.PhoneNumber = PhoneNumber.Text;
            _Affiliate.FaxNumber = FaxNumber.Text;
            _Affiliate.MobileNumber = MobileNumber.Text;
            _Affiliate.PromoCode = PromoCode.Text;

            if(ParentAffiliate.SelectedIndex!=0)
             _Affiliate.ParentAffiliateID  = AlwaysConvert.ToInt16(ParentAffiliate.SelectedValue);

            _Affiliate.Save();
            SavedMessage.Visible = true;
            SavedMessage.Text = string.Format(SavedMessage.Text, LocaleHelper.LocalNow);
        }
    }

    protected void InitEditForm()
    {
        //INITATE DROPDOWNs
        AffiliateCollection affiliates = AffiliateDataSource.LoadForCriteria("Active =1");
        ParentAffiliate.DataSource = affiliates;
        ParentAffiliate.DataTextField = "Name";
        ParentAffiliate.DataValueField = "AffiliateId";

        ParentAffiliate.DataBind();

        ParentAffiliate.Items.Insert(0, new ListItem("-- No Parent Affiliate --", "-1"));

        List<AffiliateType> AffiliateTypes = new List<AffiliateType>();

        AffiliateTypes.Add(AffiliateType.Agent);
        AffiliateTypes.Add(AffiliateType.Company);
        AffiliateTypes.Add(AffiliateType.Location);
        AffiliateTypes.Add(AffiliateType.Master_Agent);
        AffiliateTypes.Add(AffiliateType.Master_Company);
        AffiliateTypes.Add(AffiliateType.None);


        AffiliateTypeDropdownList.DataSource = AffiliateTypes;
        AffiliateTypeDropdownList.DataBind();

        AffiliateTypeDropdownList.SelectedValue = _Affiliate.AffiliateType.ToString();

        //Initiate Image
        Image imageLogo = new Image();

       //  None = 0,
       //  Master_Company = 10,
       //  Company = 20,
       //  Master_Agent = 30,
       //  Agent = 40,
       //  Location = 50,

        switch (_Affiliate.AffiliateType)
        {
            case AffiliateType.Master_Company:
                imageLogo.SkinID = "MasterCompany";
                break;
            case AffiliateType.Company:
                imageLogo.SkinID = "Company";
                break;
            case AffiliateType.Master_Agent:
                imageLogo.SkinID = "MasterAgent";
                break;
            case AffiliateType.Agent:
                imageLogo.SkinID = "Agent";
                break;
            case AffiliateType.Location:
                imageLogo.SkinID = "location";
                break;
           
        }

        if(_Affiliate.AffiliateTypeId!=0)
             phAffiliateLogo.Controls.Add(imageLogo);
        
        //INITIALIZE FORM
        Name.Text = _Affiliate.Name;
        if (_Affiliate.ReferralDays != 0) ReferralDays.Text = _Affiliate.ReferralDays.ToString();
        CommissionRate.Text = _Affiliate.CommissionRate.ToString("#.##");
        if (!_Affiliate.CommissionIsPercent) CommissionType.SelectedIndex = 0;
        else CommissionType.SelectedIndex = (_Affiliate.CommissionOnTotal ? 2 : 1);
        WebsiteUrl.Text = _Affiliate.WebsiteUrl;
        Email.Text = _Affiliate.Email;
        Active.Checked = _Affiliate.Active;
        //INITIALIZE ADDRESS DATA
        FirstName.Text = _Affiliate.FirstName;
        LastName.Text = _Affiliate.LastName;
        Company.Text = _Affiliate.Company;
        Address1.Text = _Affiliate.Address1;
        Address2.Text = _Affiliate.Address2;
        City.Text = _Affiliate.City;
        Province.Text = _Affiliate.Province;
        PostalCode.Text = _Affiliate.PostalCode;
        PromoCode.Text = _Affiliate.PromoCode ;

        //BANKING INFORMATION
        
        BankAccountNumber.Text = _Affiliate.AccountNumber;
        BankRoutingNumber.Text = _Affiliate.RoutingNumber;

        bankInformationPanel.Visible = true;

        if (_Affiliate.BankInformation == null)
        {
            AddLink.Visible = true;
            bankInformationPanel.Visible = false;

        }

        if (_Affiliate.BankInformation != null)
            BankInformation.Text = String.Format("{0}<br/>{1} {2}, {3} {4} <br/>({5})-{6]-{7}", _Affiliate.BankInformation.CustomerName,
                _Affiliate.BankInformation.Address, _Affiliate.BankInformation.City,
                _Affiliate.BankInformation.State, _Affiliate.BankInformation.ZipCode,
                _Affiliate.BankInformation.AreaCode, _Affiliate.BankInformation.Prefix, _Affiliate.BankInformation.Suffix);

        
         


        //INITIALIZE COUNTRY
        CountryCode.DataSource = CountryDataSource.LoadForStore("Name");
        CountryCode.DataBind();
        if (string.IsNullOrEmpty(_Affiliate.CountryCode)) _Affiliate.CountryCode = Token.Instance.Store.DefaultWarehouse.CountryCode;
        ListItem selectedCountry = CountryCode.Items.FindByValue(_Affiliate.CountryCode);
        if (selectedCountry != null) CountryCode.SelectedIndex = CountryCode.Items.IndexOf(selectedCountry);
        PhoneNumber.Text = _Affiliate.PhoneNumber;
        FaxNumber.Text = _Affiliate.FaxNumber;
        MobileNumber.Text = _Affiliate.MobileNumber;
    }   
}
