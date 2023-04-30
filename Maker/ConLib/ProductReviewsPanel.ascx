<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProductReviewsPanel.ascx.cs" Inherits="ConLib_ProductReviewsPanel" %>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Src="~/ConLib/ProductReviewForm.ascx" TagName="ProductReviewForm" TagPrefix="uc" %>
<%--
<conlib>
<summary>Displays all reviews for a product.</summary>
</conlib>
--%>
<ajax:UpdatePanel ID="ReviewsPanelAjax" runat="server">
    <ContentTemplate>
        <asp:Panel ID="ShowReviewsPanel" runat="server">
            <table cellpadding="4" width="100%">
                <tr>
                    <td>
                        <asp:Panel ID="AverageRatingPanel" runat="server">
                            <asp:Label ID="RatingImageLabel" runat="server" Text="Average Rating:" SkinID="FieldHeader"></asp:Label>
                            <asp:Image ID="RatingImage" runat="server" />
                            <asp:Label ID="ReviewCount" runat="server" Text=" (based on {0} review{1})" EnableViewState="false"></asp:Label>
                        </asp:Panel>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Panel ID="ReviewsCaptionPanel" runat="server">
                            <h2>
                                <asp:Localize ID="ReviewsCaption" runat="server" Text="Showing {0} Review{1}:" EnableViewState="false"/>
                                <asp:Localize ID="PagedReviewsCaption" runat="server" Text="Showing {0} - {1} of {2} Reviews:" EnableViewState="false"/>
                            </h2>
                        </asp:Panel>
                        <cb:SortedGridView ID="ReviewsGrid" runat="server" DataSourceID="ReviewDs" AutoGenerateColumns="False" 
                            ShowHeader="false" AllowPaging="True" PageSize="5" SkinID="PagedList" CellPadding="4" CellSpacing="0" Width="100%" DefaultSortExpression="ReviewDate"  DefaultSortDirection="Descending"  >
                            <Columns>
                                <asp:TemplateField HeaderText="Rating" SortExpression="Rating">
                                                                        <ItemStyle HorizontalAlign="Center" Width="60px" VerticalAlign="Top"/>
                                    <ItemTemplate>
                                        <asp:Image ID="ReviewRating" runat="server" ImageUrl='<%# NavigationHelper.GetRatingImage(AlwaysConvert.ToDecimal(Eval("Rating"))) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Reviewer">
                                    <ItemStyle Width="150px" VerticalAlign="Top"/>
                                    <ItemTemplate>
                                        <asp:Label ID="ReviewerName" runat="server" Text='<%#Eval("ReviewerProfile.DisplayName", "by {0}")%>'></asp:Label><br />
                                        <asp:Label ID="ReviewerLocation" runat="server" Text='{0}<br />' Visible="false"></asp:Label>
                                        <asp:Label ID="ReviewDate" runat="server" Text='<%# Eval("ReviewDate", "on {0:d}") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Review">
                                    <ItemStyle Width="450px" VerticalAlign="Top"/>
                                    <ItemTemplate>
                                        <asp:Label ID="ReviewTitleLabel" Text='<%#Eval("ReviewTitle")%>' runat="server" SkinID="FieldHeader"></asp:Label><br />
                                        <asp:Literal ID="ReviewBodyLabel" Text='<%# "<pre class=Reviews>" + Eval("ReviewBody") + "</pre>"%>' runat="server"></asp:Literal>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <asp:Label ID="NoReviewsMessage" runat="server" Text="Be the first to submit a review on this product!"></asp:Label>
                            </EmptyDataTemplate>
                        </cb:SortedGridView>
                        <asp:ObjectDataSource ID="ReviewDs" runat="server" EnablePaging="True" OldValuesParameterFormatString="original_{0}"
                            SelectCountMethod="SearchCount" SelectMethod="Search" SortParameterName="sortExpression"
                            TypeName="MakerShop.Products.ProductReviewDataSource">
                            <SelectParameters>
                                <asp:QueryStringParameter Name="productId" QueryStringField="ProductId" Type="Int32" />
                                <asp:Parameter Name="approved" Type="Object" DefaultValue="True" />
                            </SelectParameters>
                        </asp:ObjectDataSource>    
                    </td>
                </tr>
                <tr>
                    <td>
                         <asp:LinkButton ID="ReviewLink" runat="server" Text="Review and Rate this Item" OnClick="ReviewLink_Click"></asp:LinkButton>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="ReviewProductPanel" runat="server" Visible="false">
            <uc:ProductReviewForm ID="ProductReviewForm1" runat="server" />
        </asp:Panel>
    </ContentTemplate>
</ajax:UpdatePanel>
