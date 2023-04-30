using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MakerShop.Common;
using MakerShop.Products;
using MakerShop.Marketing;
using MakerShop.Utility;

public partial class ConLib_ProductDiscountsDialog : System.Web.UI.UserControl
{
    private string _Caption = "Available Discounts";

    [Personalizable(), WebBrowsable()]
    public string Caption
    {
        get { return _Caption; }
        set { _Caption = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        bool discountsFound = false;
        int _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
        Product _Product = ProductDataSource.Load(_ProductId);
        if (_Product != null)
        {
            VolumeDiscountCollection availableDiscounts = VolumeDiscountDataSource.GetAvailableDiscounts(_ProductId);
            if (availableDiscounts.Count > 0)
            {
                //SEE WHETHER THERE IS ONE DISCOUNT
                //AND IT ALWAYS HAS NO VALUE
                bool show = true;
                if (availableDiscounts.Count == 1)
                {
                    VolumeDiscount testDiscount = availableDiscounts[0];
                    if (testDiscount.Levels.Count == 1)
                    {
                        VolumeDiscountLevel testLevel = testDiscount.Levels[0];
                        show = ((testLevel.MinValue > 1) || (testLevel.DiscountAmount > 0));
                    }
                }
                if (show)
                {
                    phCaption.Text = this.Caption;
                    DiscountGrid.DataSource = availableDiscounts;
                    DiscountGrid.DataBind();
                    discountsFound = true;
                }
            }
        }
        //DO NOT DISPLAY THIS CONTROL IF NO DISCOUNTS AVAILABLE
        if (!discountsFound) this.Controls.Clear();
    }

    protected string GetLevels(object dataItem)
    {
        StringBuilder levelList = new StringBuilder();
        levelList.Append("<ul>");
        VolumeDiscount discount = (VolumeDiscount)dataItem;
        string minFormat, maxFormat;
        if (discount.IsValueBased)
        {
            minFormat = "{0:ulc}";
            maxFormat = "{1:ulc}";
        }
        else
        {
            minFormat = "{0:F0}";
            maxFormat = "{1:F0}";
        }
        foreach (VolumeDiscountLevel level in discount.Levels)
        {
            levelList.Append("<li>buy ");
            if (level.MinValue != 0)
            {
                if (level.MaxValue != 0)
                {
                    levelList.Append(string.Format(minFormat + " to " + maxFormat, level.MinValue, level.MaxValue));
                }
                else
                {
                    levelList.Append(string.Format("at least " + minFormat, level.MinValue));
                }
            }
            else if (level.MaxValue != 0)
            {
                levelList.Append(string.Format("up to " + minFormat, level.MaxValue));
            }
            else
            {
                levelList.Append("any");
            }
            levelList.Append(", save ");
            if (level.IsPercent) levelList.Append(string.Format("{0:0.##}%", level.DiscountAmount));
            else levelList.Append(string.Format("{0:ulc} each", level.DiscountAmount));
            levelList.Append("</li>");
        }
        levelList.Append("</ul>");
        return levelList.ToString();
    }
}
