using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MakerShop.Products;
using MakerShop.Utility;

public partial class ConLib_ProductCustomFieldsDialog : System.Web.UI.UserControl
{
    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (this.Visible)
        {
            List<string> keys = new List<string>();
            List<string> values = new List<string>();
            int _ProductId = AlwaysConvert.ToInt(Request.QueryString["ProductId"]);
            Product _Product = ProductDataSource.Load(_ProductId);
            if (_Product != null)
            {
                foreach (ProductTemplateField tf in _Product.TemplateFields)
                {
                    if (!string.IsNullOrEmpty(tf.InputValue) && tf.InputField.IsMerchantField)
                    {
                        keys.Add(tf.InputField.Name);
                        values.Add(tf.InputValue);
                    }
                }
                foreach (ProductCustomField cf in _Product.CustomFields)
                {
                    if (!string.IsNullOrEmpty(cf.FieldValue))
                    {
                        keys.Add(cf.FieldName);
                        values.Add(cf.FieldValue);
                    }
                }
            }
            //DO NOT DISPLAY THIS CONTROL IF NO DISCOUNTS AVAILABLE
            if (keys.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<table cellpadding=\"4\" cellspacing=\"0\">\r\n");
                for (int i = 0; i < keys.Count; i++)
                {
                    string key = keys[i];
                    string value = values[i];
                    sb.Append("<tr>\r\n");
                    sb.Append("<th align=\"right\">\r\n");
                    sb.Append(key);
                    sb.Append(":</th>\r\n");
                    sb.Append("<td>\r\n");
                    sb.Append(value);
                    sb.Append("</td>\r\n");
                    sb.Append("</tr>\r\n");
                }
                sb.Append("</table>\r\n");
                phCustomFields.Controls.Add(new LiteralControl(sb.ToString()));
            }
            else
            {
                this.Controls.Clear();
            }
        }
    }
}
