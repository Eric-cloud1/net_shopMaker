<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProductDiscountsDialog.ascx.cs" Inherits="ConLib_ProductDiscountsDialog" EnableViewState="false" %>
<%--
<conlib>
<summary>Display all the discounts that can be applicable on selected product.</summary>
<param name="Caption" default="Available Discounts">Possible value can be any string.  Title of the control.</param>
</conlib>
--%>
<div class="section">
    <div class="header">
        <asp:Localize ID="phCaption" runat="server" Text="Available Discounts"></asp:Localize>
    </div>
    <div class="content">
        <asp:GridView ID="DiscountGrid" runat="server" AutoGenerateColumns="false" 
            Width="100%" ShowHeader="false" SkinID="PagedList">
            <Columns>
                <asp:TemplateField>
                    <ItemTemplate>
                        <%# Eval("Name") %>
                        <%# GetLevels(Container.DataItem) %>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
</div>