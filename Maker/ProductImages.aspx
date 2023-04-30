<%@ Page Language="C#" Theme="" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en" >
<head runat="server">
	<title>Image Gallery</title>
    <script runat="server">
        protected void Page_Init(object sender, EventArgs e)
        {
            int productId = PageHelper.GetProductId();
            Product product = ProductDataSource.Load(productId);
            if (product != null)
            {
                this.Page.Title = "Images of " + product.Name;
                this.GalleryCaption.Text = string.Format(this.GalleryCaption.Text, product.Name);
                ProductImageRepeater.DataSource = product.Images;
                ProductImageRepeater.DataBind();
            }
        }

        protected string GetSafeUrl(string url)
        {
            return this.Page.ResolveUrl(url);
        }
    </script>
	<script type="text/javascript" src="js/mootools-for-gallery.js"></script>
	<script type="text/javascript" src="js/UvumiGallery-compressed.js"></script>
	<style type="text/css">
        img{border:0}
        div.progress-bar{border:1px solid #fff; background-color:#000}
        div.missing-thumbnail{background:#333 url('images/redx.gif') center center no-repeat; cursor:pointer; border:1px solid #bbb}
        div.error-message{color:#bbb; font-size:1.2em; margin-top:-0.6em; position:relative; text-align:center}
        div.caption{background-color:#000; color:#fff}
        div.caption a{color:#fff}
        body, html{background-color:#333; margin:0; padding:0; font-family:Trebuchet MS,Helvetica,sans-serif}
        #gallery{margin:20px auto; height:520px; width:750px; position:relative; color:#aaa; padding:20px; overflow:hidden; border:1px solid #888; background-color:#000}
	</style>
	<script type="text/javascript">
		new UvumiGallery({container:'gallery'});
	</script>
</head>
<body>
	<div id="gallery"> 
	    <asp:Localize ID="GalleryCaption" runat="server" Text="Images of {0}:"></asp:Localize>
        <asp:Repeater ID="ProductImageRepeater" runat="server">
            <HeaderTemplate>
            </HeaderTemplate>
            <ItemTemplate>
                <a href="<%#GetSafeUrl(Eval("ImageUrl").ToString())%>"><img src="<%#GetSafeUrl(Eval("ImageUrl").ToString())%>" alt="<%#Eval("ImageAltText")%>" /></a>
            </ItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
        </asp:Repeater>
	</div>
</body>
</html>