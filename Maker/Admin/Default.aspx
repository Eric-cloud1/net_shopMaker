<%@ Page Language="C#" MasterPageFile="Admin.master" CodeFile="Default.aspx.cs" Inherits="Admin_Default" Title="Merchant Dashboard"  %>
<%@ Register Src="Dashboard/PopularCategories.ascx" TagName="PopularCategories" TagPrefix="uc" %>
<%@ Register Src="Dashboard/AdminAlerts.ascx" TagName="AdminAlerts" TagPrefix="uc" %>
<%@ Register Src="Dashboard/OrderSummary.ascx" TagName="OrderSummary" TagPrefix="uc" %>
<%@ Register Src="Dashboard/NewsReader.ascx" TagName="NewsReader" TagPrefix="uc" %>
<%@ Register Src="Dashboard/UserStatus.ascx" TagName="UserStatus" TagPrefix="uc" %>
<%@ Register Src="Dashboard/AdminWebpartsPanel.ascx" TagName="AdminWebpartsPanel" TagPrefix="uc" %>
<%@ Register Src="Dashboard/ActivityByHour.ascx" TagName="ActivityByHour" TagPrefix="uc" %>
<%@ Register src="Dashboard/Conversion.ascx" tagname="Conversion" tagprefix="uc" %>
<%@ Register src="Dashboard/PopularProductsCount.ascx" tagname="PopularProductsCount" tagprefix="uc" %>
<%@ Register src="Dashboard/QtySalesOverTime.ascx" tagname="QtySalesOverTime" tagprefix="uc" %>
<%@ Register src="Dashboard/Snapshot.ascx" tagname="Snapshot" tagprefix="uc" %>
<%@ Register src="Dashboard/Attempts.ascx" tagname="Attempts" tagprefix="uc" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="MainContent">
<script language="javascript" type="text/javascript">
   // setTimeout("timerPost()", 60000);
    function timerPost() {
 
        <%=base.Page.GetPostBackEventReference(this)%>;
    }
</script>
<asp:WebPartManager ID="wpm" runat="server"></asp:WebPartManager>
    
<div class="pageHeader">
	<div class="caption">
		<h1><asp:Localize ID="Caption" runat="server" Text="Dashboard"></asp:Localize>
           
		</h1>    
		<div class="links">
            <asp:LinkButton ID="CustomizeLink" runat="server" OnClick="CustomizeLink_Click">Customize</asp:LinkButton>
        </div>
	</div>
</div>
<table cellpadding="2" cellspacing="2" class="innerLayout">
	<tr>
		<td align="left" valign="top" width="48%">		
			<asp:WebPartZone ID="WebPartZone1" runat="server" HeaderText="Zone 1" Width="100%" SkinID="AdminZone">
				<ZoneTemplate>
				    <uc:Snapshot ID="Snapshot1" title="SnapShot" runat="server" />
				    <uc:Conversion ID="Conversion1" title="Conversion"  titleUrl = "~/Admin/Reports/Continuity/ConversionsPerformance.aspx" runat="server" />
				</ZoneTemplate>
			</asp:WebPartZone>
	        </td>
		<td align="left" valign="top" width="48%">
		
			<asp:WebPartZone ID="WebPartZone2" runat="server" HeaderText="Zone 2" Width="100%" SkinID="AdminZone">
				<ZoneTemplate>
					<uc:Attempts ID="Attempts1"   title="Gateway Performance"  titleUrl = "~/Admin/Reports/Continuity/GatewaysPerformance.aspx" runat="server" />
					<uc:AdminAlerts ID="AdminAlerts1" runat="server" Title="My Alerts" />
				    <uc:UserStatus ID="UserStatus1" runat="server" Title="User Status" />

				</ZoneTemplate>
			</asp:WebPartZone>
		</td>
	</tr>
	<tr>
		<td colspan="2">
        	<uc:AdminWebpartsPanel ID="AdminWebpartsPanel1" runat="server" />
    	</td>
	</tr>
</table>
</asp:Content>