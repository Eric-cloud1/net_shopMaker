<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ReportDownload.aspx.cs" Inherits="Admin_Reports_ReportDownload" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<title>Report Download</title>
<script src="../../js/jquery.js" type="text/javascript"></script>
<style type="text/css">
#progressBackgroundFilter {
    position:fixed; 
    top:0px; 
    bottom:0px; 
    left:0px;
    right:0px;
    overflow:hidden; 
    padding:0; 
    margin:0; 
    background-color:#000; 
    filter:alpha(opacity=50); 
    opacity:0.5; 
    z-index:1000; 
}

#processMessage { 
    position:fixed; 
    top:30%; 
    left:43%;
    padding:10px; 
    width:14%; 
    z-index:1001; 
    background-color:White;
    border:solid 1px #000;
}

</style>
</head>
<script type="text/javascript">
 
$(document).ready(function(){
 
	$('#page_effect').fadeIn(2000);
 
});
</script>

  
<body>
    <form id="form2" runat="server">
    <ajax:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server" EnablePartialRendering="true"
        AsyncPostBackTimeout="900">
    </ajax:ToolkitScriptManager>
    
     <asp:UpdateProgress AssociatedUpdatePanelID="updatePanelResult" ID="updateProgress" runat="server">
     <ProgressTemplate>
     <div id="progressBackgroundFilter">
          <div id="Div1"> Loading...<br /><br />
             <img alt="Loading" src="../../images/indicator.gif" />
        </div>
        </div> 
     </ProgressTemplate>
   </asp:UpdateProgress>
   
<asp:UpdatePanel ID="updatePanelResult" runat="server">
            <ContentTemplate >
    <div id="page_effect" style="display:none;background-color:#D1DCED;">
     <table  class="form" cellpadding="0" cellspacing="0" border="0" width="100%">
             
              <tr>
                 <td style="width:100%;" colspan="4">
                <asp:Label ID="SavedMessage" runat="server" Text="Template saved at {0}." Font-Size="X-Small" EnableViewState="false"
                    Visible="false" SkinID="GoodCondition"></asp:Label>
                <asp:Label ID="ErrorMessageLabel" runat="server" Font-Size="X-Small" Text="" EnableViewState="false"
                    Visible="false" SkinID="ErrorCondition"></asp:Label>
            </td>
            </tr>
            </table>
            </div>
              </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
