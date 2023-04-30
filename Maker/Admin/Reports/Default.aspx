<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Default.aspx.cs" Inherits="Admin_Reports_Default" Title="Reports Menu"  %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
<div class="pageHeader">
	<div class="caption">
		<h1><asp:Localize ID="Caption" runat="server" Text="Reports"></asp:Localize>
            
        </h1>
	</div>
</div>
<table cellpadding="2" cellspacing="0" class="innerLayout">
    <tr>
        <td valign="top" width="50%">
            <div class="section">
                <div class="header">
                    <h2><asp:Localize ID="SalesCaption" runat="server" Text="Sales Reports"></asp:Localize></h2>
                </div>
                <div class="content">
                    <ul class="menuList">
                        <li>
	                        <asp:HyperLink ID="SalesSummaryLink" runat="server" Text="Sales Summary" NavigateUrl="SalesSummary.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="SalesSummaryDescription" runat="server" Text="Provides sales summary for a given period."></asp:Label>
                        </li>
                        <li>
	                        <asp:HyperLink ID="DailySalesLink" runat="server" Text="Daily Sales" NavigateUrl="DailySales.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="DailySalesDescription" runat="server" Text="Provides sales totals for a single day."></asp:Label>
                        </li>
                        <li>
	                        <asp:HyperLink ID="MonthlySalesLink" runat="server" Text="Monthly Sales" NavigateUrl="MonthlySales.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="MonthlySalesDescription" runat="server" Text="Provides sales totals by month."></asp:Label>
                        </li>
                        <li>
	                        <asp:HyperLink ID="TaxReportLink" runat="server" Text="Taxes" NavigateUrl="Taxes.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="TaxesDescription" runat="server" Text="Provides a tax report for a given period."></asp:Label>
                        </li>
                    </ul>
                </div>
            </div>
              <div class="section">
                <div class="header">
                    <h2><asp:Localize ID="ShippingCaption" runat="server" Text="Shipping Reports"></asp:Localize></h2>
                </div>
                 <div class="content">
                    <ul class="menuList">
                         <li>
                            <asp:HyperLink ID="ShippingReportLink" runat ="server" Text="Shipping Summary" NavigateUrl="ShippingSummary.aspx"></asp:HyperLink><br />
                            <asp:Label ID="ShippingSummaryDetails" runat="server" Text="Exports and imports shipping summaries."></asp:Label>
                        </li>
                        <li>
	                        <asp:HyperLink ID="ForecastShipmentLink" runat="server" Text="Forecast Shipping" NavigateUrl="~/Admin/Reports/Shipping/ForecastShipment.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="ForecastShipmentLabel" runat="server" Text="Provides a forecast shipment a given period."></asp:Label>
                        </li>
                        <li>
	                        <asp:HyperLink ID="PendingShipmentLink" runat="server" Text="Pending Shipping" NavigateUrl="~/Admin/Reports/Shipping/PendingShipment.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="PendingShipmentLabel" runat="server" Text="Provides pending shipment a given period."></asp:Label>
                        </li>
                         
                          <li>
	                        <asp:HyperLink ID="ProductShipmentLink" runat="server" Text="Product Shipment" NavigateUrl="~/Admin/Reports/Shipping/ProductShipment.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="ProductShipmentLabel" runat="server" Text="Provides product shipment a given period."></asp:Label>
                        </li>
                    </ul>
                </div>
            </div>
            </div>
            
       
            <div class="section">
                <div class="header">
                    <h2><asp:Localize ID="ProductCaption" runat="server" Text="Product Reports"></asp:Localize></h2>
                </div>
                <div class="content">
                    <ul class="menuList">
                        <li>
	                        <asp:HyperLink ID="SalesByProductLink" runat="server" Text="Sales by Product" NavigateUrl="TopProducts.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="SalesByProductDescription" runat="server" Text="Provides sales totals by product."></asp:Label>
                        </li>
                        <li>
	                        <asp:HyperLink ID="PopularProductsLink" runat="server" Text="Product Popularity" NavigateUrl="PopularProducts.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="PopularProductsDescription" runat="server" Text="Ranks the products by popularity of views."></asp:Label>
                        </li>
                        <li>
	                        <asp:HyperLink ID="PopularCategoriesLink" runat="server" Text="Category Popularity" NavigateUrl="PopularCategories.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="PopularCategoriesDescription" runat="server" Text="Ranks the categories by popularity of views."></asp:Label>
                        </li>
                        <li>
	                        <asp:HyperLink ID="LowInventoryLink" runat="server" Text="Low Inventory" NavigateUrl="LowInventory.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="LowInventoryDescription" runat="server" Text="Provides a listing of products at or below their inventory warning level."></asp:Label>
                        </li>
                          <li>
	                        <asp:HyperLink ID="DigitalGoodsLink" runat="server" Text="Digital Goods" NavigateUrl="~/Admin/Reports/Products/Digitalgoods.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="DigitalGoodsLabel" runat="server" Text="Provides digital goods product by date."></asp:Label>
                        </li>
                        <li>
	                        <asp:HyperLink ID="OverallLink" runat="server" Text="Overall Summary" NavigateUrl="~/Admin/Reports/Products/Overall.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="OverallLabel" runat="server" Text="Provides overall product by date."></asp:Label>
                        </li>
                    </ul>
                </div>
            </div>
            
            
                <div class="section">
                <div class="header">
                    <h2><asp:Localize ID="GatewayCaption" runat="server" Text="Gateway Reports"></asp:Localize></h2>
                </div>
                <div class="content">
                    <ul class="menuList">
                        <li>
	                        <asp:HyperLink ID="GatewayBreakdownLink" runat="server" Text="Gateway Breakdown" NavigateUrl="~/Admin/Reports/Gateway/GatewayBreakdown.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="GatewayBreakdownLabel" runat="server" Text="Provides Gateway Breakdown by date."></asp:Label>
                        </li>
                        <li>
	                        <asp:HyperLink ID="GatewayBreakdownSummaryLink" runat="server" Text="Gateway Breakdown Summary" NavigateUrl="~/Admin/Reports/Gateway/GatewayStatusSummary.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="GatewayBreakdownSummaryLabel" runat="server" Text="Provides Gateway Breakdown Summary by date."></asp:Label>
                        </li>
                  
                    </ul>
                </div>
            </div>
            
            
             <div class="section">
                <div class="header">
                    <h2><asp:Localize ID="ContinuityReport" runat="server" Text="Continuity Reports"></asp:Localize></h2>
                </div>
                <div class="content">
                    <ul class="menuList">
                        <li>
	                        <asp:HyperLink ID="ContinuityPerformanceLink" runat="server" Text="Conversions Performance" NavigateUrl="~/Admin/Reports/Continuity/ConversionsPerformance.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="ContinuityPerformanceLabel" runat="server" Text="Continuity Perfomance report by date."></asp:Label>
                        </li>
                        <li>
	                        <asp:HyperLink ID="GatewaysPerformanceLink" runat="server" Text="Gateways Performance" NavigateUrl="GatewaysPerformance.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="GatewaysPerformanceLabel" runat="server" Text="Gateways Perfomance report by date."></asp:Label>
                        </li>
                    </ul>
                </div>
            </div>
           
                <div class="section">
                <div class="header">
                    <h2><asp:Localize ID="ChargeBackCaption" runat="server" Text="Chargeback Reports"></asp:Localize></h2>
                </div>
                <div class="content">
                    <ul class="menuList">
                        <li>
	                        <asp:HyperLink ID="ChargeBackLink" runat="server" Text="Chargeback" NavigateUrl="~/Admin/Reports/Chargeback/Chargeback.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="ChargeBackLabel" runat="server" Text="Charge back report by date."></asp:Label>
                        </li>
                        <li>
	                        <asp:HyperLink ID="ChargeBackCountLink" runat="server" Text="Chargeback Count" NavigateUrl="~/Admin/Reports/Chargeback/ChargebackCount.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="ChargeBackCountLabel" runat="server" Text="Chargeback Count report by date."></asp:Label>
                        </li>
                    </ul>
                </div>
            </div>
            
            
            
	    </td>
        <td valign="top" width="50%">
            <div class="section">
                <div class="header">
                    <h2><asp:Localize ID="Localize1" runat="server" Text="Customer Reports"></asp:Localize></h2>
                </div>
                <div class="content">
                    <ul class="menuList">
                        <li>
	                        <asp:HyperLink ID="SalesByCustomerLink" runat="server" Text="Sales by Customer" NavigateUrl="TopCustomers.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="SalesByCustomerCustomerDescription" runat="server" Text="Provides sales totals by customer."></asp:Label>
                        </li>
                        <li>
	                        <asp:HyperLink ID="AbandonedBasketsLink" runat="server" Text="Abandoned Baskets" NavigateUrl="MonthlyAbandonedBaskets.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="AbandonedBasketsDescription" runat="server" Text="Provides a summary of abandoned shopping sessions for your store."></asp:Label>
                        </li>
                        <li>
	                        <asp:HyperLink ID="PopularBrowsersLink" runat="server" Text="Browser Popularity" NavigateUrl="PopularBrowsers.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="PopularBrowsersDescription" runat="server" Text="Ranks the popularity of browsers used by your customers."></asp:Label>
                        </li>
                    </ul>
                </div>
            </div>
            <div class="section">
                <div class="header">
                    <h2><asp:Localize ID="MarketingReportsCaption" runat="server" Text="Marketing Reports"></asp:Localize></h2>
                </div>
                <div class="content">
                    <ul class="menuList">
                        <li>
	                        <asp:HyperLink ID="AffiliateSalesLink" runat="server" Text="Sales by Affiliate" NavigateUrl="SalesByAffiliate.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="AffiliateSalesDescription" runat="server" Text="Provides sales totals by affiliate."></asp:Label>
                        </li>
                        <li>
	                        <asp:HyperLink ID="CouponUsageLink" runat="server" Text="Coupon Usage" NavigateUrl="CouponUsage.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="CouponUsageDescription" runat="server" Text="Provides usage statistics and sales totals for your coupons."></asp:Label>
                        </li>
                    </ul>
                </div>
            </div>
            <div class="section">
                <div class="header">
                    <h2><asp:Localize ID="OtherReportsCaption" runat="server" Text="Misc Reports"></asp:Localize></h2>
                </div>
                <div class="content">
                    <ul class="menuList">
                        <li>
	                        <asp:HyperLink ID="WhoIsOnlineLink" runat="server" Text="Who Is Online?" NavigateUrl="WhoIsOnline.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="WhoIsOnlineDescription" runat="server" Text="Lets the merchant see who is currently browsing the store. Anonymous users not include in the report."></asp:Label>
                        </li>
                    </ul>
                </div>
            </div>
            
             <div class="section">
                <div class="header">
                    <h2><asp:Localize ID="ChartReport" runat="server" Text="Chart Reports"></asp:Localize></h2>
                </div>
                <div class="content">
                    <ul class="menuList">
                        <li>
	                        <asp:HyperLink ID="ConversionLink" runat="server" Text="Conversion Charts" NavigateUrl="~/Admin/Reports/Chart/Conversion.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="ConversionLabel" runat="server" Text="Provides Conversion charts report."></asp:Label>
                        </li>
                        <li>
	                        <asp:HyperLink ID="ConversionLeadLink" runat="server" Text="Conversion Lead Charts" NavigateUrl="~/Admin/Reports/Chart/ConversionLead.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="ConversionLeadLabel" runat="server" Text="Provides Conversion lead charts report."></asp:Label>
                        </li>
                         <li>
	                        <asp:HyperLink ID="ConversionLeadOrderLink" runat="server" Text="Conversion Lead Orders" NavigateUrl="~/Admin/Reports/Chart/ConversionLeadsOrder.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="ConversionLeadOrderLabel" runat="server" Text="Provides Conversion lead orders charts report."></asp:Label>
                        </li>
                         <li>
	                        <asp:HyperLink ID="ForeacastLink" runat="server" Text="Forecast" NavigateUrl="~/Admin/Reports/Chart/ForecastChart.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="ForeacastLabel" runat="server" Text="Provides Forecast charts report."></asp:Label>
                        </li>
                         <li>
	                        <asp:HyperLink ID="SalesAbandonLink" runat="server" Text="Sales Abandon" NavigateUrl="~/Admin/Reports/Chart/SalesAbandon.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="SalesAbandonLabel" runat="server" Text="Provides Sales Abandon charts report."></asp:Label>
                        </li>
                         <li>
	                        <asp:HyperLink ID="SalesCurrentLink" runat="server" Text="Sales Current" NavigateUrl="~/Admin/Reports/Chart/SalesCurrent.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="SalesCurrentLabel" runat="server" Text="Provides Current Sales charts report."></asp:Label>
                        </li>
                        <li>
	                        <asp:HyperLink ID="SalesFailedCurrentLink" runat="server" Text="Sales Failed Current" NavigateUrl="~/Admin/Reports/Chart/SalesFailedCurrent.aspx"></asp:HyperLink><br />
	                        <asp:Label ID="SalesFailedCurrentLabel" runat="server" Text="Provides Current Failed Sales charts report."></asp:Label>
                        </li>
                    </ul>
                </div>
            </div>
            
              <div class="section">
                <div class="header">
                    <h2><asp:Localize ID="BillingCaption" runat="server" Text="Billing Reports"></asp:Localize></h2>
                </div>
                <div class="content">
                    <ul class="menuList">
                    <li>
                        <asp:HyperLink ID="DailyBillingLink" runat="server" Text="Daily Billing" NavigateUrl="~/Admin/Reports/Billing/DailyBillingReport.aspx"></asp:HyperLink><br />
                        <asp:Label ID="DailyBillingLabel" runat="server" Text="Provides daily billing report."></asp:Label>
                    </li>
                     <li>
                        <asp:HyperLink ID="ForecastAuthorizedLink" runat="server" Text="Forecast Authorized" NavigateUrl="~/Admin/Reports/Billing/ForecastAuthorizedReport.aspx"></asp:HyperLink><br />
                        <asp:Label ID="ForecastAuthorizedLabel" runat="server" Text="Provides forecast authorize report."></asp:Label>
                    </li>
                      <li>
                        <asp:HyperLink ID="ForecastCapturedLink" runat="server" Text="Forecast Captured" NavigateUrl="~/Admin/Reports/Billing/ForecastCapturedReport.aspx"></asp:HyperLink><br />
                        <asp:Label ID="ForecastCapturedLabel" runat="server" Text="Provides forecast captured report."></asp:Label>
                    </li>
                    <li>
                        <asp:HyperLink ID="RebillAuthorizedLink" runat="server" Text="Rebill Authorized" NavigateUrl="~/Admin/Reports/Billing/RebillAuthorizeReport.aspx"></asp:HyperLink><br />
                        <asp:Label ID="RebillAuthorizedLabel" runat="server" Text="Provides rebill authorized report."></asp:Label>
                    </li>
                    <li>
                        <asp:HyperLink ID="RebillCapturedLink" runat="server" Text="Rebill Captured" NavigateUrl="~/Admin/Reports/Billing/RebillCaptureReport.aspx"></asp:HyperLink><br />
                        <asp:Label ID="RebillCapturedLabel" runat="server" Text="Provides rebill captured report."></asp:Label>
                    </li>
                     <li>
                        <asp:HyperLink ID="TransactionLink" runat="server" Text="Transactions" NavigateUrl="~/Admin/Reports/Billing/Transaction.aspx"></asp:HyperLink><br />
                        <asp:Label ID="TransactionLabel" runat="server" Text="Provides all transactions report."></asp:Label>
                    </li>
                    <li>
                        <asp:HyperLink ID="InitialBillingLink" runat="server" Text="Initial Billing" NavigateUrl="~/Admin/Reports/Billing/TrialStatus.aspx"></asp:HyperLink><br />
                        <asp:Label ID="InitialBillingLabel" runat="server" Text="Provides initial billing report."></asp:Label>
                    </li>
                    </ul>
                </div>
            </div> 
           
	    </td>
	</tr>
</table>    
</asp:Content>
