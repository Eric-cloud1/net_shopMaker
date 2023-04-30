using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MakerShop.Web.UI.WebControls
{
    public class ToolTipLabel : System.Web.UI.WebControls.WebControl
    {
        private string _HoverCssClass;
        private string _LabelCssClass;
        private string _Text;
        private string _AssociatedControlID;
        private long _DelayTime;

        public string HoverCssClass
        {
            get
            {
                if (string.IsNullOrEmpty(_HoverCssClass) && !string.IsNullOrEmpty(this.CssClass))
                {
                    return this.CssClass + "Hover";
                }
                return _HoverCssClass;
            }
            set { _HoverCssClass = value; }
        }

        public string LabelCssClass
        {
            get { return _LabelCssClass; }
            set { _LabelCssClass = value; }
        }

        public long DelayTime
        {
            get
            {
                if (_DelayTime < 1) _DelayTime = 900;
                return _DelayTime;
            }
            set { _DelayTime = value; }
        }

        public string Text
        {
            get { return _Text; }
            set { _Text = value; }
        }

        public string AssociatedControlID
        {
            get { return _AssociatedControlID; }
            set { _AssociatedControlID = value; }
        }

        private static void RegisterClientScripts(Page page)
        {
            if (page != null)
            {
                Type type = typeof(ToolTipLabel);
                if (IsIE6()) ScriptManager.RegisterClientScriptResource(page, type, "MakerShop.Web.UI.WebControls.ToolTipLabel_IE6.js");
                else ScriptManager.RegisterClientScriptResource(page, type, "MakerShop.Web.UI.WebControls.ToolTipLabel.js");
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (this.ToolTip.Length > 0)
            {
                RegisterClientScripts(this.Page);

                HyperLink tooltip = new HyperLink();
                tooltip.ID = Guid.NewGuid().ToString("N");
                tooltip.CssClass = this.CssClass;
                tooltip.NavigateUrl = "#";
                tooltip.Attributes.Add("onclick", "return false;");
                tooltip.Attributes.Add("onmouseover", "ShowToolTip(this,"+DelayTime.ToString()+",'" + this.HoverCssClass + "');");
                tooltip.Attributes.Add("onmouseout", "HideToolTip(this,'" + this.CssClass + "');");
                tooltip.TabIndex = -1;
                Label tooltipLabel = new Label();
                tooltipLabel.Text = this.Text;
                tooltipLabel.CssClass = this.LabelCssClass;
                tooltipLabel.AssociatedControlID = this.AssociatedControlID;
                
                StringBuilder tooltipTextBuilder = new StringBuilder();
                if (IsIE6())
                {
                    // APPEND A FRAME AS BACKGROUND TO FIX SELECT DROPDOWN BUG IN IE6                    
                    tooltipTextBuilder.Append("<iframe frameborder='0' style='position:absolute;z-index:89;overflow:hidden;display:none;' scrolling='no' src='" + this.ResolveClientUrl("~/images/spacer.gif") + "'></iframe>");
                }
                tooltipTextBuilder.Append("<div class=" + this.CssClass + ">");
                tooltipTextBuilder.Append(this.ToolTip);
                tooltipTextBuilder.Append("</div>");
                Literal tooltipText = new Literal();
                tooltipText.Text = tooltipTextBuilder.ToString();

                tooltip.Controls.Add(tooltipLabel);
                tooltip.Controls.Add(tooltipText);
                this.Controls.Add(tooltip);            
            
            }
            else
            {
                Label tooltipLabel = new Label();
                tooltipLabel.Text = this.Text;
                tooltipLabel.AssociatedControlID = this.AssociatedControlID;
                this.Controls.Add(tooltipLabel);
            }
        }

        private static bool IsIE6()
        {
            //FOR LESS THAN IE7, WE NEED TO HIDE DROPDOWN CONTROLS ON POPUP
            bool isIE6 = false;
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                HttpBrowserCapabilities browser = context.Request.Browser;
                isIE6 = ((browser.Browser == "IE") && (browser.MajorVersion < 7));
            }
            return isIE6;
        }
        
        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            this.RenderChildren(writer);
        }
    }
}
