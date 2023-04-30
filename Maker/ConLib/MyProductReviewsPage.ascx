<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MyProductReviewsPage.ascx.cs" Inherits="ConLib_MyProductReviewsPage" EnableViewState="false" %>
<%--
<conlib>
<summary>Display page to show reviewer profile and all the reviews. Customer and edit/remove reviews here.</summary>
</conlib>
--%>
<div class="pageHeader">
    <h1><asp:Localize ID="Caption" runat="server" Text="Product Reviews"></asp:Localize></h1>
</div>
<asp:PlaceHolder ID="ProfilePanel" runat="server">
    <div class="section">
        <div class="header">
            <h2><asp:Localize ID="ReviewerProfileCaption" runat="server" Text="Reviewer Profile"></asp:Localize></h2>
        </div>
        <div class="content">
            <table class="inputForm">
                <tr>
                    <th class="rowHeader">
                        Email:
                    </th>
                    <td>
                        <asp:Label ID="Email" runat="server" Text=""></asp:Label><br />
                        <asp:Label ID="EmailVerified" runat="server" Text=" (verified)" visible="false" EnableViewState="false"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader">
                        Name:
                    </th>
                    <td>
                        <asp:Label ID="DisplayName" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <th class="rowHeader">
                        Location:
                    </th>
                    <td>
                        <asp:Label ID="Location" runat="server"></asp:Label>
                    </td>
                </tr>                        
            </table>
        </div>
    </div>
</asp:PlaceHolder>
<asp:Panel ID="ReviewsPanel" runat="server" CssClass="section">
    <asp:Panel ID="ReviewsCaptionPanel" runat="server" CssClass="header">
        <h2><asp:Localize ID="ReviewsCaption" runat="server" Text="" /></h2>
    </asp:Panel>
    <div class="content">
        <asp:GridView ID="ReviewsGrid" runat="server" AutoGenerateColumns="False" Width="100%" DataSourceID="ReviewDs" AllowPaging="True" PageSize="10" AllowSorting="True" 
            CellPadding="4" SkinID="PagedList" DataKeyNames="ProductReviewId" OnDataBound="ReviewsGrid_DataBound">
            <Columns>
                <asp:TemplateField HeaderText="Product">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemTemplate>
                        <asp:HyperLink ID="ProductLink" runat="server" NavigateUrl='<%# Eval("Product.NavigateUrl") %>' Text='<%# Eval("Product.Name") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Rating" SortExpression="Rating">
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:Image ID="ReviewRating" runat="server" ImageUrl='<%# NavigationHelper.GetRatingImage(AlwaysConvert.ToDecimal(Eval("Rating"))) %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Review" SortExpression="ReviewTitle">
                    <ItemStyle Width="300px" />
                    <ItemTemplate>
                        <asp:Label ID="ReviewTitleLabel" Text='<%#Eval("ReviewTitle")%>' runat="server" SkinID="FieldHeader"></asp:Label><br />
                        <asp:Label ID="ReviewBodyLabel" Text='<%# "<pre class=Reviews>" + Eval("ReviewBody") + "</pre>"%>' runat="server"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Published" SortExpression="IsApproved">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:Label ID="IsApproved" Text='<%# GetApprovedText(AlwaysConvert.ToBool(Eval("IsApproved"), false)) %>' runat="server"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Top" Width="54px" Wrap="false" />
                    <ItemTemplate>
                        <a href="<%# Page.ResolveClientUrl(Eval("ProductReviewId", "EditMyReview.aspx?ReviewId={0}"))%>"><asp:Image ID="EditIcon" runat="server" SkinID="EditIcon" AlternateText="Edit" /></a>
                        <asp:ImageButton ID="DeleteLink" runat="server" CommandName="Delete" OnClientClick="return confirm('Are you sure you want to delete this review?')" SkinID="DeleteIcon" AlternateText="Delete" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <asp:Label ID="NoReviewsMessage" runat="server" Text="You have not yet submitted any product reviews."></asp:Label>
            </EmptyDataTemplate>
        </asp:GridView>
        <asp:ObjectDataSource ID="ReviewDs" runat="server" EnablePaging="True" OldValuesParameterFormatString="original_{0}"
            SelectCountMethod="CountForReviewerProfile" SelectMethod="LoadForReviewerProfile" SortParameterName="sortExpression"
            TypeName="MakerShop.Products.ProductReviewDataSource" OnSelecting="ReviewDs_Selecting" 
            DataObjectTypeName="MakerShop.Products.ProductReview" DeleteMethod="Delete">
        </asp:ObjectDataSource>
    </div>
</asp:Panel>