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
using MakerShop.Web.SiteMap;

public partial class Admin_Website_CommonXMLSiteMap : MakerShop.Web.UI.MakerShopAdminPage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        { 
			initializeControls();
        }
		
    }

	private void initializeControls() {
            SiteMapOptions options = new SiteMapOptions();
            options.Load(new CommonSiteMapOptionKeys());

            if(!string.IsNullOrEmpty(options.SiteMapFileName)) {
                SiteMapFileName.Text = options.SiteMapFileName;
            }else{
                SiteMapFileName.Text = "SiteMap.xml";
            }
            
            if(options.OverwriteSiteMapFile) {
                AllowOverwrite.SelectedIndex = 1;
            }else{
                AllowOverwrite.SelectedIndex = 0;
            }

            if(options.IncludeProducts) {
                IncludeProducts.SelectedIndex = 1;
            }else{
                IncludeProducts.SelectedIndex = 0;
            }

            if(options.IncludeCategories) {
                IncludeCategories.SelectedIndex = 1;
            }else{
                IncludeCategories.SelectedIndex = 0;
            }

            if(options.IncludeWebpages) {
                IncludeWebpages.SelectedIndex = 1;
            }else{
                IncludeWebpages.SelectedIndex = 0;
            }

            if (!string.IsNullOrEmpty(options.CompressedSiteMapFileName))
            {
                CompressedSiteMapFileName.Text = options.CompressedSiteMapFileName;
            }
            else
            {
                CompressedSiteMapFileName.Text = "SiteMap.xml.gz";
            }

            if (options.OverwriteCompressedFile)
            {
                AllowOverwriteCompressed.SelectedIndex = 1;
            }
            else
            {
                AllowOverwriteCompressed.SelectedIndex = 0;
            }

			DefaultUrlPriority.Text = options.DefaultUrlPriority.ToString();

			BindChangeFrequency(options.DefaultChangeFrequency);

            string dataPath = options.SiteMapDataPath;
            if (string.IsNullOrEmpty(dataPath))
            {
                dataPath = Request.MapPath("~/");
            }
            //SiteMapDataPath.Text = dataPath;

			
            MessagePanel.Visible = false;
	}

    protected void BindChangeFrequency(changefreq fType)
    {
        DefaultChangeFrequency.Items.Clear();
        foreach (changefreq fqType in Enum.GetValues(typeof(changefreq)))
        {
            ListItem newItem = new ListItem(fqType.ToString(), fqType.ToString());
            DefaultChangeFrequency.Items.Add(newItem);
            if (fqType == fType)
            {
                newItem.Selected = true;
            }
        }
    }


    protected void Page_Init(object sender, EventArgs e)
    {
        BindSiteMapActions();
    }

	protected void BindSiteMapActions()
    {
        SiteMapAction.Items.Clear();
        //ADD BLANK ITEM TO START
        SiteMapAction.Items.Add(new ListItem("","NONE"));
        
        //ADD ACTIONS
        SiteMapAction.Items.Add(new ListItem("Create SiteMap", "CREATE"));
        //SiteMapAction.Items.Add(new ListItem("Compress SiteMap", "COMPRESS"));
        SiteMapAction.Items.Add(new ListItem("Create and Compress SiteMap", "CREATE_COMPRESS"));
        //SiteMapAction.Items.Add(new ListItem("Delete SiteMap", "DELETE"));
        //SiteMapAction.Items.Add(new ListItem("Delete Compressed SiteMap", "DELETE_COMPRESSED"));
    }

    protected SiteMapOptions GetPostedOptions()
    {
        SiteMapOptions options = new SiteMapOptions();
        options.Load(new CommonSiteMapOptionKeys());
        
        string dataPath = options.SiteMapDataPath;
        if (string.IsNullOrEmpty(dataPath))
        {
            dataPath = Request.MapPath("~/");
        }
        options.SiteMapDataPath = dataPath;       

        options.CompressedSiteMapFileName = CompressedSiteMapFileName.Text;
        options.SiteMapFileName = SiteMapFileName.Text;
		options.IncludeProducts = IncludeProducts.SelectedIndex == 1;
		options.IncludeCategories = IncludeCategories.SelectedIndex == 1;
        options.IncludeWebpages = IncludeWebpages.SelectedIndex == 1;
        options.OverwriteCompressedFile = AllowOverwriteCompressed.SelectedIndex == 1;
        options.OverwriteSiteMapFile = AllowOverwrite.SelectedIndex == 1;
        options.DefaultChangeFrequency = (changefreq)Enum.Parse(typeof(changefreq), DefaultChangeFrequency.SelectedValue);
        options.DefaultUrlPriority = AlwaysConvert.ToDecimal(DefaultUrlPriority.Text);
        
        return options;
    }

    protected void BtnSaveSettings_Click(object sender, EventArgs e)
    {
        SiteMapOptions options = GetPostedOptions();
        options.Save(new CommonSiteMapOptionKeys());
        List<string> messages = new List<string>();
        messages.Add("Settings Have Been Saved.");
        UpdateMessagePanel(true, messages);
    }

    protected void CancelButton_Click(object sender, EventArgs e)
    {
        //redirect to dashboard?
		Response.Redirect("~/Admin/Default.aspx");
    }

    protected void UpdateMessagePanel(bool success, List<string> messages)
    {
        MessagePanel.Visible = true;
        SuccessMessageHeader.Visible = success;
        FailureMessageHeader.Visible = !success;
        PopulateMessages(messages);
    }
    
    protected void PopulateMessages(List<string> messages)
    {
        Messages.Items.Clear();
        if (messages == null) return;        
        foreach (string message in messages)
        {
            Messages.Items.Add(message);
        }
    }

    protected void Create()
    {
        SiteMapOptions options = GetPostedOptions();        
        List<string> messages = new List<string>();
        CommonSiteMapProvider provider = new CommonSiteMapProvider();
        bool success = provider.CreateSiteMap(options, ref messages);        
        UpdateMessagePanel(success, messages);
    }

/*    protected void Compress()
    {
        SiteMapOptions options = GetPostedOptions();
        List<string> messages = new List<string>();
        CommonSiteMapProvider provider = new CommonSiteMapProvider();
        bool success = provider.CompressSiteMap(options, ref messages);
        UpdateMessagePanel(success, messages);
    }
*/

    protected void CreateCompress()
    {
        SiteMapOptions options = GetPostedOptions();
        List<string> messages = new List<string>();
        CommonSiteMapProvider provider = new CommonSiteMapProvider();
        bool success = provider.CreateAndCompressSiteMap(options, ref messages);
        UpdateMessagePanel(success, messages);
    }

    protected void Delete()
    {
        SiteMapOptions options = GetPostedOptions();
        List<string> messages = new List<string>();
		//TODO
    }

    protected void DeleteCompressed()
    {
        SiteMapOptions options = GetPostedOptions();
        List<string> messages = new List<string>();
		//TODO
    }

    protected void SiteMapActionButton_Click(object sender, ImageClickEventArgs e)
    {		
        string action = SiteMapAction.SelectedValue;

        if (!string.IsNullOrEmpty(action))
        {
            switch (action)
            {
                case "CREATE":
					Create();
                    break;
                case "COMPRESS":
				//	Compress();
                    break;
                case "CREATE_COMPRESS":
					CreateCompress();
                    break;
                case "DELETE":
					//CreateUpload();
                    break;
                case "DELETE_COMPRESSED":
					//CreateCompressUpload();
                    break;
                default:					
                    break;
            }
        }
    }


}
