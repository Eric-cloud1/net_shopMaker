<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AddToCartLink.ascx.cs" Inherits="ConLib_AddToCartLink" EnableViewState="false" %>
<%--
<conlib>
<summary>Displays a link to add an item to the cart.</summary>
<param name="ProductId">The ID of the product to add to the cart when the link is clicked.</param>
<param name="ShowImage" default="true">Indicates whether an image should be displayed with the link.  The image is determined by your theme.</param>
<param name="LinkText" default="">Text to use for the link.  You can use the string <b>{0}</b> as a place holder for the product name.</param>
</conlib>
--%>
<asp:LinkButton ID="AC" runat="server" SkinID="BuyNowButton" CausesValidation="false"><asp:Image ID="LI" runat="server" SkinID="BuyNowButton" AlternateText='<%# String.Format("Add {0} to Cart",ProductName) %>' /><asp:Literal ID="LT" runat="server" Text=""></asp:Literal></asp:LinkButton>
<asp:HyperLink ID="MoreDetailsLink" runat="server" SkinId="MoreDetailsButton" Text="More Details" EnableViewState="false" Visible="false" ></asp:HyperLink>

<asp:HiddenField ID="VS" runat="server" />