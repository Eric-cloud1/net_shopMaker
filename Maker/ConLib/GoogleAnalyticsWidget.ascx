<%@ Control Language="C#" AutoEventWireup="true" CodeFile="GoogleAnalyticsWidget.ascx.cs" Inherits="ConLib_GoogleAnalyticsWidget" EnableViewState="false" %>
<%--
<conlib>
<summary>Widget for Google Analytics Tracking.</summary>
</conlib>
--%>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
<asp:PlaceHolder ID="trackingPanel" runat="server">
	<script type="text/javascript">
		var gaJsHost = (("https:" == document.location.protocol) ? "https://ssl." : "http://www.");
		document.write(unescape("%3Cscript src='" + gaJsHost + "google-analytics.com/ga.js' type='text/javascript'%3E%3C/script%3E"));
	</script>

	<script type="text/javascript">
		var pageTracker = _gat._getTracker("<%=this.GoogleUrchinId %>");
		pageTracker._initData();
		pageTracker._trackPageview();
	</script>
</asp:PlaceHolder>
