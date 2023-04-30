<%@ Control Language="C#" CodeFile="~/ConLib/PayPalExpressCheckoutButton.ascx.cs" Inherits="ConLib_PayPalExpressCheckoutButton" EnableViewState="false" %>
<%--
<conlib>
<summary>Displays featured items in a category.</summary>
<param name="ShowHeader" default="true">Possible values are true or false.  Indicates whether the header will be shown or not.</param>
<param name="ShowDescription" default="true">Possible values are true or false.  Indicates whether the description will be shown or not.</param>
<param name="PanelCSSClass" default="section">Possible value cab be any suitable text. </param>
</conlib>
--%>
<asp:UpdatePanel ID="PayPalPanel" runat="server" UpdateMode="Always">
    <ContentTemplate>
        <asp:Panel id="ExpressCheckoutPanel" runat="server" Visible="false" CssClass="section">
	        <asp:PlaceHolder ID="phHeader" runat="server">
            <div class="header">
                <h2><asp:Localize ID="Caption" runat="server" Text="Express Checkout"></asp:Localize></h2>
            </div>
	        </asp:PlaceHolder>
            <div class="expressCheckoutCell">
                <div align=center class="contentArea noBottomPadding">
                    <asp:LinkButton ID="ExpressCheckoutLink" runat="server" OnClick="ExpressCheckoutLink_Click">
                        <asp:Image ID="ExpressCheckoutLinkImage" runat="server" ImageUrl="https://www.paypal.com/en_US/i/btn/btn_xpressCheckout.gif" AlternateText="PayPal Express Checkout" />
                    </asp:LinkButton>
		        </div>
		        <asp:PlaceHolder ID="phDescription" runat="server">
                <div class="contentArea noTopPadding">
                    Save time. Check out securely. Click the PayPal button to use the shipping and 
	                billing information you have stored with PayPal.
	            </div>
		        </asp:PlaceHolder>
            </div>
        </asp:Panel>    
    </ContentTemplate>
</asp:UpdatePanel>
