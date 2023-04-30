<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Conversion.ascx.cs" Inherits="Admin_Dashboard_Conversion" %>
<%@ Register Src="~/Admin/UserControls/DatesAndTime.ascx" TagName="DatesAndTime"
    TagPrefix="uc1" %>
<style type="text/css">
    /* default link styling -- NOT OVERIDING DEFAULT STYLE */a
    {
        color: Black;
    }
    a:hover
    {
        color: Black;
    }
    a img
    {
        border: 0;
    }
    /* link that should show like a traditional HTML link (underlined text) */.HSorting
    {
        text-decoration: underline;
        font-weight: bold;
        color: Black;
        width: 50px;
        text-align: center;
    }
    .Hborder
    {
        border-left: solid 1px black;
        width: 1px;
    }
</style>
<asp:UpdatePanel runat="server" ID="up" ChildrenAsTriggers="true" UpdateMode="Conditional">
    <ContentTemplate>
        <table cellpadding="1" cellspacing="1" width="100%">
            <uc1:DatesAndTime ID="dtDashBoardConversion" runat="server" ShowDate="false"  ShowSubmitRange="True" />
        </table>
        <div style="float: right;">
            <asp:Label ID="ReportTime" runat="server" />
        </div>
        <asp:GridView ID="Report" runat="server" ShowHeader="True" Width="100%" AutoGenerateColumns="false"
            BorderWidth="1" GridLines="Both" AllowSorting="true" OnSortCommand="SortGrid"
            EnableViewState="false" SkinID="Summary" OnRowDataBound="Report_RowDataBound">
            <Columns>
                <asp:TemplateField HeaderText="Affiliate<br/>ID">
                    <ItemStyle HorizontalAlign="Right" />
                    <ItemTemplate>
                        <asp:Label ID="lbAffiliateID" runat="Server" Text='<%# ((TrackingCount)Container.DataItem).AffiliateId %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Affiliate">
                    <ItemStyle HorizontalAlign="Left" />
                    <ItemTemplate>
                        <asp:Label ID="lbAffiliate" Font-Size="9px" runat="Server" Text='<%# ((TrackingCount)Container.DataItem).Affiliate %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Clicks">
                    <ItemStyle HorizontalAlign="Right" />
                    <ItemTemplate>
                        <asp:Label ID="lbClicks" runat="Server" Text='<%# ((TrackingCount)Container.DataItem).Clicks %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Leads">
                    <ItemStyle HorizontalAlign="Right" />
                    <ItemTemplate>
                        <asp:Label ID="lbLeads" runat="Server" Text='<%# ((TrackingCount)Container.DataItem).Leads %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Sales">
                    <ItemStyle HorizontalAlign="Right" />
                    <ItemTemplate>
                        <asp:Label ID="lbSales" runat="Server" Text='<%# ((TrackingCount)Container.DataItem).Sales%>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Conv%">
                    <ItemStyle HorizontalAlign="Right" />
                    <ItemTemplate>
                        <asp:Label ID="lbConversionPercent" runat="Server" Text='<%# formatConversionPercent(Container.DataItem) %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="CX">
                    <ItemStyle HorizontalAlign="Right" />
                    <ItemTemplate>
                        <asp:Label ID="lbCX" runat="Server" Text='<%# ((TrackingCount)Container.DataItem).CX  %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Orders">
                    <ItemStyle HorizontalAlign="Right" />
                    <ItemTemplate>
                        <asp:Label ID="lbOrders" runat="Server" Text='<%# ((TrackingCount)Container.DataItem).Orders%>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Last Order">
                <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:Label ID="lbLastOrderDate" Font-Size="9px" runat="Server" Text='<%# formatLastOrderDate(Container.DataItem) %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </ContentTemplate>
</asp:UpdatePanel>
