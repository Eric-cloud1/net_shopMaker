<%@ Control Language="C#" AutoEventWireup="true" CodeFile="EditMyReviewPage.ascx.cs" Inherits="ConLib_EditMyReviewPage" %>
<%--
<conlib>
<summary>Displays a form to edit a customer review for any product.</summary>
</conlib>
--%>
<%@ Register Src="~/ConLib/ProductReviewForm.ascx" TagName="ProductReviewForm" TagPrefix="uc" %>
<div class="pageHeader">
    <h1><asp:Localize ID="Caption" runat="server" Text="Edit Review"></asp:Localize></h1>
</div>
<div class="section">
    <asp:Panel ID="ReviewsCaptionPanel" runat="server" CssClass="header">
        <h2><asp:Localize ID="ProductName" runat="server" /></h2>
    </asp:Panel>
    <uc:ProductReviewForm ID="ProductReviewForm1" runat="server"></uc:ProductReviewForm>
</div>