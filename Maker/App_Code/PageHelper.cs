using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using MakerShop.Common;
using MakerShop.Products;
using MakerShop.Payments;
using MakerShop.Orders;
using MakerShop.Stores;
using MakerShop.Utility;
using MakerShop.Catalog;
using MakerShop.DigitalDelivery;
using System.Text.RegularExpressions;
using System.Collections;

public class PageHelper
{
    public static void RegisterBasketControl(Page page)
    {
        MakerShop.Web.UI.MakerShopPage acPage = page as MakerShop.Web.UI.MakerShopPage;
        if (acPage != null) acPage.HasBasketControl = true;
    }

    public static bool HasBasketControl(Page page)
    {
        MakerShop.Web.UI.MakerShopPage acPage = page as MakerShop.Web.UI.MakerShopPage;
        if (acPage != null) return acPage.HasBasketControl;
        return false;
    }

    /// <summary>
    /// Traps the enter key for a control and triggers the click event for the specified button control.
    /// </summary>
    /// <param name="inputControl">The control to trap enter key for</param>
    /// <param name="defaultButton">The button to click when enter is pressed</param>
    public static void SetDefaultButton(WebControl inputControl, WebControl defaultButton)
    {
        SetDefaultButton(inputControl, defaultButton.ClientID);
    }

    /// <summary>
    /// Prevents firefox browser to submit the form when enter key is pressed on text area controls etc.
    /// </summary>
    /// <param name="inputControl"></param>
    /// <param name="clientId"></param>
    public static void PreventFirefoxSubmitOnKeyPress(WebControl inputControl, string clientId)
    {
        if (HttpContext.Current.Request.Browser.Browser.Equals("Firefox"))
        {
            StringBuilder script = new StringBuilder();
            script.Append("<script language=\"javascript\">\r\n");
            script.Append("<!--\r\n");
            script.Append("function fnPreventFirefoxSubmitOnKeyPress(e,n) {\r\n");
            script.Append("try {\r\n");
            script.Append("if (!document.all) {\r\n");        
            script.Append("if (e && e.keyCode && e.keyCode==13) {\r\n");
            script.Append("e.stopPropagation();\r\n");
            script.Append("}\r\n");        
            script.Append("}\r\n");
            script.Append("} catch (e) {}\r\n");
            script.Append("}\r\n");
            script.Append("// -->\r\n");
            script.Append("</script>\r\n");

            inputControl.Attributes.Add("onkeypress", "return fnPreventFirefoxSubmitOnKeyPress(event,\'" + clientId + "\',false)");
            inputControl.Page.ClientScript.RegisterClientScriptBlock(string.Empty.GetType(), "fnPreventFirefoxSubmitOnKeyPress", script.ToString());
        }
    }

    /// <summary>
    /// Traps the enter key for a control and triggers the click event for the specified button control.
    /// </summary>
    /// <param name="inputControl">The control to trap enter key for</param>
    /// <param name="buttonClientId">The client ID of the button to click when enter is pressed</param>
    public static void SetDefaultButton(WebControl inputControl, string buttonClientId)
    {
        StringBuilder script = new StringBuilder();
        script.Append("<script language=\"javascript\">\r\n");
        script.Append("<!--\r\n");
        script.Append("function fnSubmitOnCR(e,n){\r\n");
        script.Append("var key;\r\n");
        script.Append("if(window.event) key = window.event.keyCode;\r\n");
        script.Append("else key = e.which;\r\n");
        script.Append("if (key == 13) {\r\n");
        script.Append("var o=document.getElementById(n);\r\n");
        script.Append("if(o!=null){\r\n");
        script.Append("o.click();\r\n");
        script.Append("return false;\r\n");
        script.Append("}}\r\n");
        script.Append("return true;\r\n");
        script.Append("}\r\n");
        script.Append("// -->\r\n");
        script.Append("</script>\r\n");
        inputControl.Attributes.Add("onkeypress", "return fnSubmitOnCR(event,\'" + buttonClientId + "\')");
        inputControl.Page.ClientScript.RegisterClientScriptBlock(string.Empty.GetType(), "fnSubmitOnCR", script.ToString());
    }

    /// <summary>
    /// Traps the enter key for all page and triggers the click event for the specified button control.
    /// This function will take care of firefox problem on pressing enter on textarea key submits form.
    /// </summary>    
    /// <param name="defaultButton">The button to click when enter is pressed</param>
    public static void SetPageDefaultButton(Page page, WebControl defaultButton)
    {
        if (!page.Request.Browser.Browser.Equals("Firefox"))
        {
            // IF NOT FIRE FOX
            page.Form.DefaultButton = defaultButton.UniqueID;
        }
        else
        {
            StringBuilder script = new StringBuilder();
            script.Append("<script language=\"javascript\">\r\n");
            script.Append("<!--\r\n");
            script.Append("function Page_FireDefaultButton(event, target) {\r\n");
            script.Append("if ((event.keyCode == 13 || event.which == 13) && !(event.target && (event.target.tagName.toLowerCase() == 'textarea'))) \r\n");
            script.Append("{\r\n");
            script.Append("var defaultButton = document.getElementById(target);\r\n");
            script.Append("if (defaultButton == 'undefined') defaultButton = document.all[target]; \r\n");
            script.Append("if (defaultButton && typeof(defaultButton.click) != 'undefined') \r\n");
            script.Append("{\r\n");
            script.Append("defaultButton.click();\r\n");
            script.Append("event.cancelBubble = true;\r\n");
            script.Append("if (event.stopPropagation) event.stopPropagation();\r\n");
            script.Append("return false;\r\n");
            script.Append("}}\r\n");
            script.Append("return true;\r\n");
            script.Append("}\r\n");
            script.Append("// -->\r\n");
            script.Append("</script>\r\n");
            page.Form.Attributes.Add("onkeypress", "javascript:return Page_FireDefaultButton(event,\'" + defaultButton.ClientID + "\')");
            page.ClientScript.RegisterClientScriptBlock(string.Empty.GetType(), "Page_FireDefaultButton", script.ToString());
        }
    }

    /// <summary>
    /// Traps the enter key for a panel and triggers the click event for the specified button control.
    /// This function will take care of firefox problem on pressing enter on textarea key submits form.
    /// </summary>    
    /// <param name="defaultButton">The button to click when enter is pressed</param>
    public static void SetPanelDefaultButton(Panel panel, WebControl defaultButton)
    {
        if (!panel.Page.Request.Browser.Browser.Equals("Firefox"))
        {
            // IF NOT FIRE FOX
            panel.DefaultButton = defaultButton.UniqueID;
        }
        else
        {
            StringBuilder script = new StringBuilder();
            script.Append("<script language=\"javascript\">\r\n");
            script.Append("<!--\r\n");
            script.Append("function Panel_FireDefaultButton(event, target) {\r\n");
            script.Append("if ((event.keyCode == 13 || event.which == 13) && !(event.target && (event.target.tagName.toLowerCase() == 'textarea'))) \r\n");
            script.Append("{\r\n");
            script.Append("var defaultButton = document.getElementById(target);\r\n");
            script.Append("if (defaultButton == 'undefined') defaultButton = document.all[target]; \r\n");
            script.Append("if (defaultButton && typeof(defaultButton.click) != 'undefined') \r\n");
            script.Append("{\r\n");
            script.Append("defaultButton.click();\r\n");
            script.Append("event.cancelBubble = true;\r\n");
            script.Append("if (event.stopPropagation) event.stopPropagation();\r\n");
            script.Append("return false;\r\n");
            script.Append("}}\r\n");
            script.Append("return true;\r\n");
            script.Append("}\r\n");
            script.Append("// -->\r\n");
            script.Append("</script>\r\n");
            panel.Attributes.Add("onkeypress", "javascript:return Panel_FireDefaultButton(event,\'" + defaultButton.ClientID + "\')");
            panel.Page.ClientScript.RegisterClientScriptBlock(string.Empty.GetType(), "Panel_FireDefaultButton", script.ToString());
        }
    }


    public static void SetMaxLengthCountDown(TextBox inputControl, WebControl counter)
    {
        StringBuilder script = new StringBuilder();
        script.Append(("<script language=\"javascript\">\r\n"));
        script.Append(("<!--\r\n"));
        script.Append("function checkLength(tb, maxChars, counter) {\r\n");
        script.Append("if ((isNaN(maxChars) == true) || (maxChars <= 0)) return;\r\n");
        script.Append("var charCount = tb.value.length;\r\n");
        script.Append("if (charCount > maxChars) tb.value = tb.value.substr(0, maxChars);\r\n");
        script.Append("var charsLeft = (maxChars - charCount);\r\n");
        script.Append("if (charsLeft < 0) charsLeft = 0;\r\n");
        script.Append("if (counter) {\r\n");
        script.Append("if (document.all) counter.innerText = charsLeft;\r\n");
        script.Append("else counter.textContent = charsLeft;\r\n");
        script.Append("}}\r\n");
        script.Append(("// -->\r\n"));
        script.Append(("</script>\r\n"));
        inputControl.Attributes.Add("onkeyup", "return checkLength(this, " + inputControl.MaxLength.ToString() + ", document.getElementById('" + counter.ClientID + "'))");
        inputControl.Page.ClientScript.RegisterClientScriptBlock(string.Empty.GetType(), "checkLength", script.ToString());
    }

    public static void ConvertEnterToTab(WebControl webControl)
    {
        StringBuilder script = new StringBuilder();
        script.Append("<script language=\"javascript\">" + "\r\n");
        script.Append("<!--" + "\r\n");
        script.Append("function fnTabOnCR(e) {\r\n");
        script.Append("var key;\r\n");
        script.Append("if(window.event) key = window.event.keyCode;\r\n");
        script.Append("else key = e.which;\r\n");
        script.Append("if (key == 13) return false; }\r\n");
        script.Append("// -->\r\n");
        script.Append("</script>\r\n");
        webControl.Attributes.Add("onkeypress", "return fnTabOnCR(event)");
        webControl.Page.ClientScript.RegisterClientScriptBlock(string.Empty.GetType(), "fnTabOnCR", script.ToString());
    }

    public static Control RecursiveFindControl(Control parent, string controlId)
    {
        Control find = parent.FindControl(controlId);
        if (((find == null)
                    && parent.HasControls()))
        {
            int i = 0;
            while (((find == null)
                        && (i < parent.Controls.Count)))
            {
                Control child = parent.Controls[i];
                if (child.HasControls())
                {
                    find = RecursiveFindControl(child, controlId);
                }
                i++;
            }
        }
        return find;
    }

    /// <summary>
    /// Finds all controls of the specified type in the control tree.
    /// </summary>
    /// <param name="parent">The root control to check.</param>
    /// <param name="controlType">The type of control to find.</param>
    /// <returns>An array of Control.</returns>
    public static Control[] FindControls(Control parent, Type controlType)
    {
        List<Control> matchingControls = new List<Control>();
        foreach (Control childControl in parent.Controls)
        {
            if (childControl.GetType().Equals(controlType))
            {
                matchingControls.Add(childControl);
            }
            if (childControl.HasControls())
            {
                Control[] childMatchingControls = FindControls(childControl, controlType);
                if (childMatchingControls != null) matchingControls.AddRange(childMatchingControls);
            }
        }
        if (matchingControls.Count > 0) return matchingControls.ToArray();
        return null;
    }

    /// <summary>
    /// Determines the category context for the current request.
    /// </summary>
    /// <returns>The category context for the current request.</returns>
    public static int GetCategoryId()
    {
        return GetCategoryId(false);
    }

    /// <summary>
    /// Determines the category context for the current request.
    /// </summary>
    /// <param name="checkReferrer">Indicates whether the referring page should be used to determine category context.</param>
    /// <returns>The category context for the current request.</returns>
    public static int GetCategoryId(bool checkReferrer)
    {
        HttpRequest request = HttpContext.Current.Request;
        //LOOK FOR A CATEGORY IN THE QUERY STRING
        int categoryId = AlwaysConvert.ToInt(request.QueryString["CategoryId"]);
        if (categoryId != 0) return categoryId;
        int referrerCategoryId = 0;
        if (checkReferrer && HttpContext.Current.Request.UrlReferrer != null)
        {
            // IF THE CATALOG NODE (LIKE PRODUCT) HAVE MULTIPLE CATEGORIES THEN 
            // WE HAVE TO DECIDE THE PARENT CATEGORY BASED ON THE REFERRER CATEGORY
            string strRegEx = "(.*)/.*-(C)(\\d+)(?:C(\\d+))?\\.aspx(?:\\?(.*))?";
            Match urlMatch;
            urlMatch = Regex.Match(HttpContext.Current.Request.UrlReferrer.OriginalString, strRegEx, (RegexOptions.IgnoreCase & RegexOptions.Compiled));
            if (urlMatch.Success)
            {
                referrerCategoryId = AlwaysConvert.ToInt(urlMatch.Groups[3].ToString());
            }
        }
        //LOOK FOR A PRODUCT
        Product product = ProductDataSource.Load(GetProductId());
        if (product != null)
        {
            //USE REFERRING CATEGORY IF IT IS ASSOCIATED WITH THIS OBJECT
            if ((referrerCategoryId != 0) && (product.Categories.IndexOf(referrerCategoryId) > -1))
                return referrerCategoryId;
            //RETURN THE FIRST CATEGORY ASSOCIATED WITH THIS OBJECT
            if (product.Categories.Count > 0) return product.Categories[0];
        }
        //LOOK FOR A WEBPAGE
        Webpage webpage = WebpageDataSource.Load(GetWebpageId());
        if (webpage != null)
        {
            //USE REFERRING CATEGORY IF IT IS ASSOCIATED WITH THIS OBJECT
            if ((referrerCategoryId != 0) && (webpage.Categories.IndexOf(referrerCategoryId) > -1))
                return referrerCategoryId;
            //RETURN THE FIRST CATEGORY ASSOCIATED WITH THIS OBJECT
            if (webpage.Categories.Count > 0) return webpage.Categories[0];
        }
        //LOOK FOR A LINK
        Link link = LinkDataSource.Load(GetLinkId());
        if (link != null)
        {
            //USE REFERRING CATEGORY IF IT IS ASSOCIATED WITH THIS OBJECT
            if ((referrerCategoryId != 0) && (link.Categories.IndexOf(referrerCategoryId) > -1))
                return referrerCategoryId;
            //RETURN THE FIRST CATEGORY ASSOCIATED WITH THIS OBJECT
            if (link.Categories.Count > 0) return link.Categories[0];
        }
        //SEE IF A PARENT CATEGORY ID IS FOUND
        int parentCategoryId = AlwaysConvert.ToInt(request.QueryString["ParentCategoryId"]);
        if (parentCategoryId != 0) return parentCategoryId;
        //RETURN REFERRING CATEGORY IF NO BETTER OPTION IS FOUND
        return referrerCategoryId;
    }

    /// <summary>
    /// Determines the product context for the current request.
    /// </summary>
    /// <returns>The product context for the current request.</returns>
    public static int GetProductId()
    {
        HttpRequest request = HttpContext.Current.Request;
        //CHECK FOR PRODUCT
        int productId = AlwaysConvert.ToInt(request.QueryString["ProductId"]);
        if (productId != 0) return productId;
        //CHECK FOR PRODUCT ATTRIBUTE
        int optionId = AlwaysConvert.ToInt(request.QueryString["OptionId"]);
        if (optionId != 0)
        {
            //DETERMINE PRODUCT ID FROM ATTRIBUTE
            Option option = OptionDataSource.Load(optionId);
            if ((option != null) && (option.ProductOptions.Count > 0)) return option.ProductOptions[0].ProductId;
        }
        //RETURN 0, PRODUCT COULD NOT BE DETERMINED
        return 0;
    }

    /// <summary>
    /// Determines the webpage context for the current request.
    /// </summary>
    /// <returns>The webpage context for the current reuqest.</returns>
    public static int GetWebpageId()
    {
        return AlwaysConvert.ToInt(HttpContext.Current.Request.QueryString["WebpageId"]);
    }

    /// <summary>
    /// Determines the link context for the current request.
    /// </summary>
    /// <returns>The link context for the current reuqest.</returns>
    public static int GetLinkId()
    {
        return AlwaysConvert.ToInt(HttpContext.Current.Request.QueryString["LinkId"]);
    }

    public static string GetControlValue(Control parent, string controlId, bool recursive)
    {
        Control findControl;
        if (recursive)
        {
            findControl = PageHelper.RecursiveFindControl(parent, controlId);
        }
        else
        {
            findControl = parent.FindControl(controlId);
        }
        if (findControl != null)
        {
            TextBox tb = findControl as TextBox;
            if (tb != null)
            {
                return tb.Text;
            }
            else
            {
                DropDownList ddl = findControl as DropDownList;
                if (ddl != null)
                {
                    return ddl.SelectedValue;
                }
            }
        }
        return String.Empty;
    }


    public static int GetOrderId()
    {
        HttpContext context = HttpContext.Current;
        if (context == null) return 0;
        int orderId = AlwaysConvert.ToInt(context.Request.QueryString["OrderId"]);
        if (orderId != 0) return orderId;
        int paymentId = AlwaysConvert.ToInt(context.Request.QueryString["PaymentId"]);
        if (paymentId != 0)
        {
            Payment payment = PaymentDataSource.Load(paymentId);
            if (payment != null) return payment.OrderId;
        }
        int shipmentId = AlwaysConvert.ToInt(context.Request.QueryString["OrderShipmentId"]);
        if (shipmentId != 0)
        {
            OrderShipment shipment = OrderShipmentDataSource.Load(shipmentId);
            if (shipment != null) return shipment.OrderId;
        }
        return 0;
    }

    public static void DisableValidationScrolling(Page page)
    {
        //PREVENT SCROLL ON SUBMIT
        //DISABLE ALL JAVASCRIPT SCROLLING FOR THIS PAGE
        string script = "<script type=\"text/javascript\" language=\"javascript\">window.scrollTo=function(x,y){return true;}</script>";
        page.ClientScript.RegisterStartupScript(typeof(Page), "DisableValidationScrolling", script, false);
    }

    public static void SendFileDataToClient(string fileData, string fileName)
    {
        if (!string.IsNullOrEmpty(fileData))
        {
            HttpResponse Response = HttpContext.Current.Response;
            Response.Clear();
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Length", fileData.Length.ToString());
            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            byte[] fileBytes = enc.GetBytes(fileData);
            Response.OutputStream.Write(fileBytes, 0, fileBytes.Length);
            Response.Flush();
            Response.Close();
        }
    }

    public static void SendFileDataToClient(DigitalGood digitalGood)
    {
        if (digitalGood == null) throw new ArgumentNullException("digitalGood");
        //STREAM TO BOWSER
        HttpResponse Response = HttpContext.Current.Response;
        int chunkSize = 10000;
        FileInfo fi = new FileInfo(digitalGood.AbsoluteFilePath);
        if (fi.Exists)
        {
            using (FileStream fs = fi.OpenRead())
            {
                Response.Clear();
                Response.ContentType = "application/octet-stream";
                if (fs.Length > 0) Response.AddHeader("Content-Length", fs.Length.ToString());
                Response.AddHeader("Content-Disposition", "attachment; filename=" + digitalGood.FileName);
                long dataToRead = fs.Length;
                while ((dataToRead > 0) && (Response.IsClientConnected))
                {
                    byte[] buffer = new byte[chunkSize];
                    int bytesRead = fs.Read(buffer, 0, chunkSize);
                    Response.OutputStream.Write(buffer, 0, bytesRead);
                    Response.Flush();
                    dataToRead -= bytesRead;
                }
                fs.Close();
            }
        }
        Response.Close();
    }

    public static void BindMetaTags(Page page, ICatalogable catalogObject)
    {
        if (catalogObject != null)
        {
            StringBuilder htmlHead = new StringBuilder();
            htmlHead.Append(catalogObject.HtmlHead);
            if (!catalogObject.HtmlHead.Contains("<link rel=\"canonical\""))
            {
                string canonicalFormat = "<link rel=\"canonical\" href=\"{0}\" />";
                string objectUrl = page.ResolveUrl(catalogObject.NavigateUrl);
                htmlHead.Append(string.Format(canonicalFormat, HttpUtility.UrlEncode(objectUrl)));
            }
            page.Header.Controls.Add(new LiteralControl(htmlHead.ToString()));
        }
    }

    public static void BindCmsTheme(Page page, CatalogableBase catalogObject)
    {
        string theme = catalogObject.ActiveTheme;
        if (!string.IsNullOrEmpty(theme))
        {
            if (MakerShop.UI.Styles.Theme.Exists(theme))
            {
                page.Theme = catalogObject.ActiveTheme;
            }
            else
            {
                //INVALID THEME SPECIFIED, LOG WARNING
                Logger.Warn("Invalid theme '" + theme + "' specified for catalog item " + catalogObject.Name + "; theme not applied.");
            }
        }
    }

    private static void RegisterEditHtmlScript(Page page, int width, int height)
    {
        StringBuilder windowAttribs = new StringBuilder();
        windowAttribs.Append("width=" + width + "px," + "height=" + height + "px,");
        windowAttribs.Append("left=' + ((screen.width - " + width + ") / 2) + ',");
        windowAttribs.Append("top=' + (screen.height - " + height + ") / 2 + ',");
        windowAttribs.Append("toolbar=0,scrollbars=0,location=0,statusbar=1,menubar=0,resizable=1");
        string editUrl = page.ResolveUrl("~/Admin/Utility/EditHtml.aspx");
        string js = "function editHTML(sField) {window.open('" + editUrl + "?Field=' + sField,'EditHTML','" + windowAttribs.ToString() + "');return false;}";
        ScriptManager.RegisterClientScriptBlock(page, typeof(string), "EditHTML", js, true);
    }

    public static void SetHtmlEditor(TextBox textarea, ImageButton editButton)
    {
        RegisterEditHtmlScript(textarea.Page, 750, 580);
        editButton.OnClientClick += "return editHTML('" + textarea.ClientID + "')";
    }

    public static void SetHtmlEditor(TextBox textarea, LinkButton editButton)
    {
        RegisterEditHtmlScript(textarea.Page, 750, 580);
        editButton.OnClientClick += "return editHTML('" + textarea.ClientID + "')";
    }

    public static void OpenPopUp(WebControl opener, string pagePath, string windowName, int width, int height)
    {
        OpenPopUp(opener, pagePath, windowName, width, height, "toolbar=0,scrollbars=1,location=0,statusbar=1,menubar=0,resizable=1");
    }

    public static void OpenPopUp(WebControl opener, string pagePath, string windowName, int width, int height, string attributes)
    {
        string clientScript = GetPopUpScript(pagePath, windowName, width, height, attributes) + "return false;";
        opener.Attributes.Add("onClick", clientScript);
    }

    public static string GetPopUpScript(string pagePath, string windowName, int width, int height)
    {
        return GetPopUpScript(pagePath, windowName, width, height, string.Empty);
    }

    public static string GetPopUpScript(string pagePath, string windowName, int width, int height, string attributes)
    {
        StringBuilder windowAttribs = new StringBuilder();
        windowAttribs.Append("width=" + width + "px," + "height=" + height + "px,");
        windowAttribs.Append("left=' + ((screen.width - " + width + ") / 2) + ',");
        windowAttribs.Append("top=' + (screen.height - " + height + ") / 2 + ',");
        windowAttribs.Append(attributes);
        return "window.open('" + pagePath + "','" + windowName + "','" + windowAttribs.ToString() + "');";
    }

    public static void SetPickImageButton(TextBox imagePath, ImageButton browseButton)
    {
        Page page = imagePath.Page;
        string browseUrl = page.ResolveUrl("~/Admin/Products/Assets/AssetManager.aspx");
        string pickImage = "<script language=\"javascript\">\r\nfunction PickImage(sField) { window.open('" + browseUrl + "?ImageId=PI&Field=' + sField, 'PickImage'); return false; }\r\n</script>";
        page.ClientScript.RegisterClientScriptBlock(page.GetType(), "PickImage", pickImage);
        browseButton.OnClientClick = "return PickImage('" + imagePath.ClientID + "')";
    }

    public static void DisableViewState(Control parent)
    {
        foreach (Control control in parent.Controls)
        {
            control.EnableViewState = false;
        }
    }

    public static void FindCatalogable(out ICatalogable catalogable, out CatalogNodeType catalogableType)
    {
        HttpContext context = HttpContext.Current;
        if (context != null)
        {
            //CHECK FOR LINK
            int linkId = PageHelper.GetLinkId();
            Link link = LinkDataSource.Load(linkId);
            if (link != null)
            {
                catalogable = link;
                catalogableType = CatalogNodeType.Link;
                return;
            }
            //CHECK FOR WEBPAGE
            int webpageId = PageHelper.GetWebpageId();
            Webpage webpage = WebpageDataSource.Load(webpageId);
            if (webpage != null)
            {
                catalogable = webpage;
                catalogableType = CatalogNodeType.Webpage;
                return;
            }
            //CHECK FOR PRODUCT
            int productId = PageHelper.GetProductId();
            Product product = ProductDataSource.Load(productId);
            if (product != null)
            {
                catalogable = product;
                catalogableType = CatalogNodeType.Product;
                return;
            }
            //CHECK FOR CATEGORY
            int categoryId = AlwaysConvert.ToInt(context.Request.QueryString["CategoryId"]);
            Category category = CategoryDataSource.Load(categoryId);
            if (category != null)
            {
                catalogable = category;
                catalogableType = CatalogNodeType.Category;
                return;
            }
        }
        catalogable = null;
        catalogableType = CatalogNodeType.Category;
        return;
    }

	public static Control FindSearchSidebarControl(Control parent)
    {        
        Control match = null;
		foreach (Control childControl in parent.Controls)
        {
            if (childControl is ISearchSidebar)
            {
                match = childControl;
            }
			else if (childControl.HasControls())
            {
                match = FindSearchSidebarControl(childControl);
            }
			if(match!=null) break;
        }
        return match;
    }

	public static Control FindSearchSidebarAwareControl(Control parent)
    {        
        Control match = null;
		foreach (Control childControl in parent.Controls)
        {
            if (childControl is ISearchSidebarAware)
            {
                match = childControl;
            }
			else if (childControl.HasControls())
            {
                match = FindSearchSidebarAwareControl(childControl);
            }
			if(match!=null) break;
        }
        return match;
    }

    public static Hashtable EnumToHashtable(Type enumType)
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

    public static Dictionary<String, int> EnumToDictionary(Type enumType)
    {
        // get the names from the enumeration
        string[] names = Enum.GetNames(enumType);
        // get the values from the enumeration
        Array values = Enum.GetValues(enumType);
        // turn it into a hash table
        Dictionary<String, int> d = new Dictionary<String, int>();
        for (int i = 0; i < names.Length; i++)
            // note the cast to integer here is important
            // otherwise we'll just get the enum string back again
            d.Add(names[i], (int)values.GetValue(i));
        // return the dictionary to be bound to
        return d;
    }
}
