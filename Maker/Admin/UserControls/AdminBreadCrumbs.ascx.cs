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
using MakerShop.Orders;
using MakerShop.Products;
using MakerShop.Marketing;
using MakerShop.Payments;
using MakerShop.Utility;
using MakerShop.Catalog;

public partial class Admin_UserControls_AdminBreadCrumbs : System.Web.UI.UserControl
{
    private Order _Order;
    private Product _Product;
    private OrderShipment _OrderShipment;

    private OrderShipment GetOrderShipment()
    {
        int shipmentId = GetGuidParameter("OrderShipmentId");
        if (!shipmentId.Equals(0))
        {
            _OrderShipment = OrderShipmentDataSource.Load(shipmentId);
        }
        else
        {
            // CHECK TRACKING NUMBER
            int trackingId = GetGuidParameter("TrackingNumberId");
            if (!trackingId.Equals(0))
            {
                TrackingNumber tracking = TrackingNumberDataSource.Load(trackingId);
                if (tracking != null)
                {
                    _OrderShipment = tracking.OrderShipment;
                }
            }
        }
        return _OrderShipment;
    }

    private Order GetOrder()
    {
        if (_Order == null)
        {
            int orderId = GetGuidParameter("OrderId");
            if (!orderId.Equals(0))
            {
                _Order = OrderDataSource.LoadParent(orderId);
            }
            else
            {
                int paymentId = GetGuidParameter("PaymentId");
                if (!paymentId.Equals(0))
                {
                    Payment payment = PaymentDataSource.Load(paymentId);
                    if (payment != null)
                    {
                        _Order = payment.Order;
                    }
                }
                else
                {
                    // CHECK SHIPMENT
                    OrderShipment shipment = GetOrderShipment();
                    if (shipment != null)
                    {
                        _Order = shipment.Order;
                    }
                    else
                    {
                        // CHECK FOR ORDER ITEM DIGITAL GOOD
                        int orderItemDigitalGoodId = GetGuidParameter("OrderItemDigitalGoodId");
                        if (!orderItemDigitalGoodId.Equals(0))
                        {
                            OrderItemDigitalGood orderItemDigitalGood = OrderItemDigitalGoodDataSource.Load(orderItemDigitalGoodId);
                            if (orderItemDigitalGood != null)
                            {
                                _Order = orderItemDigitalGood.OrderItem.Order;
                            }
                        }
                    }
                }
            }
        }
        return _Order;
    }

    private Product GetProduct()
    {
        if (_Product == null)
        {
            int productId = PageHelper.GetProductId();
            if (!productId.Equals(0))
            {
                _Product = ProductDataSource.Load(productId);
            }
        }
        return _Product;
    }

    private Option _Option;
    private Option GetOption()
    {
        if (_Option == null)
        {
            int attributeId = GetGuidParameter("OptionId");
            if (attributeId != 0)
            {
                _Option = OptionDataSource.Load(attributeId);
            }
        }
        return _Option;
    }

    private ProductTemplate _ProductTemplate;
    private ProductTemplate GetProductTemplate()
    {
        if (_ProductTemplate == null)
        {
            int productTemplateId = GetGuidParameter("ProductTemplateId");
            _ProductTemplate = ProductTemplateDataSource.Load(productTemplateId);
            if (_ProductTemplate == null)
            {
                int inputFieldId = GetGuidParameter("InputFieldId");
                InputField field = InputFieldDataSource.Load(inputFieldId);
                if (field != null)
                {
                    _ProductTemplate = field.ProductTemplate;
                }
            }
        }
        return _ProductTemplate;
    }

    private EmailList _EmailList;
    private EmailList GetEmailList()
    {
        if (_EmailList == null)
        {
            int emailListId = GetGuidParameter("EmailListId");
            _EmailList = EmailListDataSource.Load(emailListId);
        }
        return _EmailList;
    }

    private Coupon _Coupon;
    private Coupon GetCoupon()
    {
        if (_Coupon == null)
        {
            int couponId = GetGuidParameter("CouponId");
            _Coupon = CouponDataSource.Load(couponId);
        }
        return _Coupon;
    }

    private VolumeDiscount _VolumeDiscount;
    private VolumeDiscount GetVolumeDiscount()
    {
        if (_VolumeDiscount == null)
        {
            int volumeDiscountId = GetGuidParameter("VolumeDiscountId");
            _VolumeDiscount = VolumeDiscountDataSource.Load(volumeDiscountId);
        }
        return _VolumeDiscount;
    }

    private WrapGroup _WrapGroup;
    protected WrapGroup GetWrapGroup()
    {
        if (_WrapGroup == null)
        {
            int wrapGroupId = GetGuidParameter("WrapGroupId");
            _WrapGroup = WrapGroupDataSource.Load(wrapGroupId);
        }
        return _WrapGroup;
    }

    int GetGuidParameter(string parameterName)
    {
        return AlwaysConvert.ToInt(Request.QueryString[parameterName]);
    }

    protected void Page_Load(object sender, System.EventArgs e)
    {

    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        BreadCrumbs.SiteMapProvider = "AdminBreadCrumbsMap";
        BreadCrumbs.DataBind();

        if (BreadCrumbs.Controls.Count < 2)
        {
            this.Controls.Clear();
        }
    }

    protected void BreadCrumbs_ItemDataBound(object sender, System.Web.UI.WebControls.SiteMapNodeItemEventArgs e)
    {
        if ((e.Item.ItemType == SiteMapNodeItemType.Parent) || (e.Item.ItemType == SiteMapNodeItemType.Current))
        {
            string nodeUrl = e.Item.SiteMapNode.Url;
            if (nodeUrl.Contains("/"))
            {
                nodeUrl = nodeUrl.Substring(nodeUrl.LastIndexOf("/") + 1);
            }
            switch (nodeUrl)
            {
                case "EditShipment.aspx":
                    int OrderShipmentId = GetGuidParameter("OrderShipmentId");
                    if (OrderShipmentId > 0)
                    {
                        // OrderShipmentId FOUND, VERIFY CONTROLS IN SITEMAPNODE
                        if (e.Item.Controls.Count > 0)
                        {
                            // DETERMINE IF WE HAVE A LINK OR A LITERAL
                            if (e.Item.Controls[0].GetType().Equals(typeof(HyperLink)))
                            {
                                HyperLink link = (HyperLink)e.Item.Controls[0];
                                link.NavigateUrl += "?OrderShipmentId=" + OrderShipmentId.ToString();
                                link.Text = string.Format(link.Text, OrderShipmentId);
                            }
                            else if (e.Item.Controls[0].GetType().Equals(typeof(Literal)))
                            {
                                Literal literal = (Literal)e.Item.Controls[0];
                                literal.Text = string.Format(literal.Text, OrderShipmentId);
                            }
                        }
                    }
                    break;
                case "Default.aspx":
                    if (!(e.Item.SiteMapNode.Url.EndsWith("Shipments/Default.aspx")
                        || e.Item.SiteMapNode.Url.EndsWith("Payments/Default.aspx")))
                    {
                        // BREAK AND DONT FALL IN NEXT CASE
                        break;
                    }
                    else goto case "ViewOrder.aspx"; // GO TO NEXT SAME CASE
                case "ViewOrder.aspx":
                case "EditOrderItems.aspx":
                case "ViewDigitalGoods.aspx":
                    Order order = GetOrder();
                    if (order != null)
                    {
                        // ORDER FOUND, VERIFY CONTROLS IN SITEMAPNODE
                        if (e.Item.Controls.Count > 0)
                        {
                            // DETERMINE IF WE HAVE A LINK OR A LITERAL
                            if (e.Item.Controls[0].GetType().Equals(typeof(HyperLink)))
                            {
                                HyperLink link = (HyperLink)e.Item.Controls[0];
                                link.NavigateUrl += "?OrderNumber=" + order.OrderNumber.ToString() + "&OrderId=" + order.OrderId.ToString();
                                link.Text = string.Format(link.Text, order.OrderNumber);
                            }
                            else if (e.Item.Controls[0].GetType().Equals(typeof(Literal)))
                            {
                                Literal literal = (Literal)e.Item.Controls[0];
                                literal.Text = string.Format(literal.Text, order.OrderNumber);
                            }
                        }
                    }
                    break;
                case "ViewTracking.aspx":
                    OrderShipment shipment = GetOrderShipment();
                    if (shipment != null)
                    {
                        // ORDER FOUND, VERIFY CONTROLS IN SITEMAPNODE
                        if (e.Item.Controls.Count > 0)
                        {
                            // DETERMINE IF WE HAVE A LINK OR A LITERAL
                            if (e.Item.Controls[0].GetType().Equals(typeof(HyperLink)))
                            {
                                HyperLink link = (HyperLink)e.Item.Controls[0];
                                link.NavigateUrl = (link.NavigateUrl + ("?OrderShipmentId=" + shipment.OrderShipmentId.ToString()));
                            }
                        }
                    }
                    break;
                case "EditProduct.aspx":
                case "Variants.aspx":
                case "Options.aspx":
                case "DigitalGoods.aspx":
                case "EditKit.aspx":
                case "Images.aspx":
                    Product product = GetProduct();
                    if (product != null)
                    {
                        // ORDER FOUND, VERIFY CONTROLS IN SITEMAPNODE
                        if (e.Item.Controls.Count > 0)
                        {
                            // DETERMINE IF WE HAVE A LINK OR A LITERAL
                            if (e.Item.Controls[0].GetType().Equals(typeof(HyperLink)))
                            {
                                HyperLink link = (HyperLink)e.Item.Controls[0];
                                link.NavigateUrl = (link.NavigateUrl + ("?ProductId=" + product.ProductId.ToString()));
                                link.Text = string.Format(link.Text, product.Name);
                            }
                            else if (e.Item.Controls[0].GetType().Equals(typeof(Literal)))
                            {
                                Literal literal = (Literal)e.Item.Controls[0];
                                literal.Text = string.Format(literal.Text, product.Name);
                            }
                        }
                    }
                    break;
                case "EditCoupon.aspx":
                    Coupon coupon = GetCoupon();
                    if (coupon != null)
                    {
                        // ORDER FOUND, VERIFY CONTROLS IN SITEMAPNODE
                        if (e.Item.Controls.Count > 0)
                        {
                            // DETERMINE IF WE HAVE A LINK OR A LITERAL
                            if (e.Item.Controls[0].GetType().Equals(typeof(HyperLink)))
                            {
                                HyperLink link = (HyperLink)e.Item.Controls[0];
                                link.NavigateUrl = link.NavigateUrl + "?CouponId=" + coupon.CouponId.ToString();
                                link.Text = string.Format(link.Text, coupon.Name);
                            }
                            else if (e.Item.Controls[0].GetType().Equals(typeof(Literal)))
                            {
                                Literal literal = (Literal)e.Item.Controls[0];
                                literal.Text = string.Format(literal.Text, coupon.Name);
                            }
                        }
                    }
                    break;
                case "Choices.aspx":
                    Option attribute = GetOption();
                    if (attribute != null)
                    {
                        // ORDER FOUND, VERIFY CONTROLS IN SITEMAPNODE
                        if (e.Item.Controls.Count > 0)
                        {
                            // DETERMINE IF WE HAVE A LINK OR A LITERAL
                            if (e.Item.Controls[0].GetType().Equals(typeof(HyperLink)))
                            {
                                HyperLink link = (HyperLink)e.Item.Controls[0];
                                link.NavigateUrl = (link.NavigateUrl + ("?OptionId=" + attribute.OptionId.ToString()));
                                link.Text = string.Format(link.Text, attribute.Name);
                            }
                            else if (e.Item.Controls[0].GetType().Equals(typeof(Literal)))
                            {
                                Literal literal = (Literal)e.Item.Controls[0];
                                literal.Text = string.Format(literal.Text, attribute.Name);
                            }
                        }
                    }
                    break;
                case "EditProductTemplate.aspx":
                    ProductTemplate template = GetProductTemplate();
                    if (template != null)
                    {
                        // ORDER FOUND, VERIFY CONTROLS IN SITEMAPNODE
                        if (e.Item.Controls.Count > 0)
                        {
                            // DETERMINE IF WE HAVE A LINK OR A LITERAL
                            if (e.Item.Controls[0].GetType().Equals(typeof(HyperLink)))
                            {
                                HyperLink link = (HyperLink)e.Item.Controls[0];
                                link.NavigateUrl = (link.NavigateUrl + ("?ProductTemplateId=" + template.ProductTemplateId.ToString()));
                                link.Text = string.Format(link.Text, template.Name);
                            }
                            else if (e.Item.Controls[0].GetType().Equals(typeof(Literal)))
                            {
                                Literal literal = (Literal)e.Item.Controls[0];
                                literal.Text = string.Format(literal.Text, template.Name);
                            }
                        }
                    }
                    break;
                case "EditWrapGroup.aspx":
                    WrapGroup wrapGroup = GetWrapGroup();
                    if (wrapGroup != null)
                    {
                        // ORDER FOUND, VERIFY CONTROLS IN SITEMAPNODE
                        if (e.Item.Controls.Count > 0)
                        {
                            // DETERMINE IF WE HAVE A LINK OR A LITERAL
                            if (e.Item.Controls[0].GetType().Equals(typeof(HyperLink)))
                            {
                                HyperLink link = (HyperLink)e.Item.Controls[0];
                                link.NavigateUrl = (link.NavigateUrl + ("?WrapGroupId=" + wrapGroup.WrapGroupId.ToString()));
                                link.Text = string.Format(link.Text, wrapGroup.Name);
                            }
                            else if (e.Item.Controls[0].GetType().Equals(typeof(Literal)))
                            {
                                Literal literal = (Literal)e.Item.Controls[0];
                                literal.Text = string.Format(literal.Text, wrapGroup.Name);
                            }
                        }
                    }
                    break;
                case "ManageList.aspx":
                    EmailList emailList = GetEmailList();
                    if (emailList != null)
                    {
                        // ORDER FOUND, VERIFY CONTROLS IN SITEMAPNODE
                        if (e.Item.Controls.Count > 0)
                        {
                            // DETERMINE IF WE HAVE A LINK OR A LITERAL
                            if (e.Item.Controls[0].GetType().Equals(typeof(HyperLink)))
                            {
                                HyperLink link = (HyperLink)e.Item.Controls[0];
                                link.NavigateUrl = link.NavigateUrl + "?EmailListId=" + emailList.EmailListId.ToString();
                                link.Text = string.Format(link.Text, emailList.Name);
                            }
                            else if (e.Item.Controls[0].GetType().Equals(typeof(Literal)))
                            {
                                Literal literal = (Literal)e.Item.Controls[0];
                                literal.Text = string.Format(literal.Text, emailList.Name);
                            }
                        }
                    }
                    break;
                case "EditDiscount.aspx":
                    VolumeDiscount volumeDiscount = GetVolumeDiscount();
                    if (volumeDiscount != null)
                    {
                        // ORDER FOUND, VERIFY CONTROLS IN SITEMAPNODE
                        if (e.Item.Controls.Count > 0)
                        {
                            // DETERMINE IF WE HAVE A LINK OR A LITERAL
                            if (e.Item.Controls[0].GetType().Equals(typeof(HyperLink)))
                            {
                                HyperLink link = (HyperLink)e.Item.Controls[0];
                                link.NavigateUrl = link.NavigateUrl + "?VolumeDiscountId=" + volumeDiscount.VolumeDiscountId.ToString();
                                link.Text = string.Format(link.Text, volumeDiscount.Name);
                            }
                            else if (e.Item.Controls[0].GetType().Equals(typeof(Literal)))
                            {
                                Literal literal = (Literal)e.Item.Controls[0];
                                literal.Text = string.Format(literal.Text, volumeDiscount.Name);
                            }
                        }
                    }
                    break;
                case "EditWebpage.aspx":
                    int  webpageId = PageHelper.GetWebpageId();
                    if (webpageId > 0)
                    {
                        // WEBPAGE FOUND, VERIFY CONTROLS IN SITEMAPNODE
                        if (e.Item.Controls.Count > 0)
                        {
                            // DETERMINE IF WE HAVE A LINK OR A LITERAL
                            if (e.Item.Controls[0].GetType().Equals(typeof(HyperLink)))
                            {
                                HyperLink link = (HyperLink)e.Item.Controls[0];
                                link.NavigateUrl = (link.NavigateUrl + ("?WebpageId=" + webpageId.ToString()));
                            }
                        }
                    }
                    break;
                case "EditLink.aspx":
                    int linkId = PageHelper.GetLinkId();
                    if (linkId > 0)
                    {
                        // LINK FOUND, VERIFY CONTROLS IN SITEMAPNODE
                        if (e.Item.Controls.Count > 0)
                        {
                            // DETERMINE IF WE HAVE A LINK OR A LITERAL
                            if (e.Item.Controls[0].GetType().Equals(typeof(HyperLink)))
                            {
                                HyperLink link = (HyperLink)e.Item.Controls[0];
                                link.NavigateUrl = (link.NavigateUrl + ("?LinkId=" + linkId.ToString()));
                            }
                        }
                    }
                    break;
            }
        }
    }
}
