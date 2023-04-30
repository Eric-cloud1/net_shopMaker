<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="forecastchart.aspx.cs" 
Inherits="Admin_Reports_Charts_ForecastChart" Title="Forecast Charts" %>
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<%@ register tagprefix="web" namespace="WebChart" assembly="WebChart"%>
<%@ Register src="~/Admin/UserControls/PickerAndCalendar.ascx" tagname="PickerAndCalendar" tagprefix="uc1" %>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
  <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
<div  class="pageHeader ">
<div class="caption ">
<h1>Forecast</h1>
</div>
</div>

<table cellpadding="0" cellspacing="0" width="100%">
<tr><td><div style="height:25px;">&nbsp;</div></td></tr>
    <tr><td><div style="vertical-align:middle; text-align:center; width:50%;"     
    <asp:Label ID="lbStartDate" runat="server" Text="Payment Date: " SkinID="FieldHeader"></asp:Label>                 
                 <uc1:PickerAndCalendar ID="forecastDate"  runat="server" /> </div>            
       </td><td><div style="vertical-align:middle;text-align:center;width:50%;">   
                    <asp:Button ID="ProcessButton" runat="server" Text="GO.." 
                OnClick="ProcessButton_Click" />
      
                </div></td></tr>
                 <tr><td colspan="2">&nbsp;</td></tr>
                
    <tr>
     <td valign="top" colspan="2" style="background-color=#E3EFFF;">
        
                 <web:chartcontrol runat="server" id="ForecastChart" SkinID="Line" Visible="true" Width="900" Height="600"
                    GridLines="Both" HasChartLegend="true" Legend-Width="130" Background-Type="Hatch"  ChartTitle-Text="Forecast"   
                       CacheDuration="5" >
                     </web:chartcontrol>
                     <web:chartcontrol runat="server" id="AlertForecastChart" SkinID="alertLine" Visible="false"  Width="900" Height="600"
                    GridLines="Both" HasChartLegend="true" Legend-Width="130" Background-Type="Hatch"  ChartTitle-Text="Forecast"   
                       CacheDuration="5" >
                     </web:chartcontrol>
                    <div align="center" style="padding:4px;">
                    <asp:Literal ID="noViewsMessage" runat="server" Visible="false" EnableViewState="false"></asp:Literal> 
                        <asp:Literal ID="CacheDate1" runat="server" Text="as of {0:t}" EnableViewState="false"></asp:Literal> 
                        <asp:LinkButton ID="RefreshLink1" runat="server" Text="refresh" OnClick="RefreshLink_Click"></asp:LinkButton>
                    </div>
        </td>
    </tr>
</table>
</ContentTemplate>
</ajax:UpdatePanel>
</asp:Content>

