<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="ForecastAuthorizedReport.aspx.cs" 
Inherits="Admin_Reports_ForecastReport" Title="Forecast Authorize Report" %>
<%@ Register Src="~/Admin/UserControls/PickerAndCalendar.ascx" TagName="PickerAndCalendar" TagPrefix="uc1" %>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>


<%@ Register src="~/Admin/UserControls/DatesAndTime.ascx" tagname="DatesAndTime" tagprefix="uc1" %>

<%@ Register src="../../UserControls/downloadbutton.ascx" tagname="downloadbutton" tagprefix="uc2" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">

<script type="text/javascript" src="<%= Page.ResolveUrl("~")%>js/jquery.min.js"></script>
<script type="text/javascript">
    function logMenuFocusOn(menuInstance) { $("#" + menuInstance).attr("mouseonmenu", "1"); }
    function logMenuFocusOut(menuInstance) { $("#" + menuInstance).attr("mouseonmenu", "0"); }
    function closeMenu(menuInstance) {
        if ($("#" + menuInstance).attr("mouseonmenu") == 0) { $("#" + menuInstance).hide(); }
    }
    jQuery.fn.setMouseEvents = function() {
        return this.each(function() {
            var me = jQuery(this);
            me.click(function() {
                $("div").filter("[type=ContextMenu]").hide();
                InstantiateMenu(me.attr("id"));
            });
            me.dblclick(function() { closeMenu("Menu" + me.attr("id")); });
            me.blur(function() { closeMenu("Menu" + me.attr("id")); });

        });
    };

    function InstantiateMenu(ParentID) {
        if ($("#Menu" + ParentID).html() == null) {
            var p = $("#" + ParentID);
            var offset = p.offset();
            var left = offset.left + $("#" + ParentID).width() - 10;
            var top = offset.top + $("#" + ParentID).height() - 5;

            var strMenuHTML = "<div type=\"ContextMenu\" id=\"Menu" + ParentID + "\" style=\"position:absolute;left:" + left + "px;top:" + top + "\px; mouseonmenu=\"1\">loading...</div>";
            $("#grid").append(strMenuHTML);
            ShowHoverPanel(ParentID)
        }
        var offset = $("#" + ParentID).offset();
        $("#Menu" + ParentID).css({ "top": (offset.top + $("#" + ParentID).height() - 5) + "px" }).show();
        $("#Menu" + ParentID).mouseout(function() { logMenuFocusOut("Menu" + ParentID); $("#" + ParentID)[0].focus(); });
        $("#Menu" + ParentID).mouseover(function() { logMenuFocusOn("Menu" + ParentID); });
    }

    function ShowHoverPanel(Id1) {

        $.post("ViewIdsDetails.ashx?ids=" + Id1, {}, function(response) { $("#Menu" + Id1).html(response); });

    }

    $(document).ready(function() {

        $("td").filter("[parent=GridTable]").setMouseEvents();
    }
);
</script>
    <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">
      
    
        <ContentTemplate>

            <div class="pageHeader">
                <div class="caption">
                    
                    <h1>
                    <asp:Localize ID="Caption" runat="server" Text="Forecast Authorized Report"></asp:Localize>
                        <asp:Localize ID="ReportCaption" runat="server"  Visible="False" 
                            EnableViewState="False"></asp:Localize>
                      
                    </h1>
                </div>
                            </div>
           
            
              <table align="center"  class="form" cellpadding="0" cellspacing="0" border="0" width="100%">
                 <tr>
                 <td style="width:100%" colspan="4">
                <asp:Label ID="SavedMessage" runat="server" Text="Template saved at {0}." EnableViewState="false"
                    Visible="false" SkinID="GoodCondition"></asp:Label>
                <asp:Label ID="ErrorMessageLabel" runat="server" Text="" EnableViewState="false"
                    Visible="false" SkinID="ErrorCondition"></asp:Label>
            </td>
            </tr>
                  
               <uc1:DatesAndTime ID="dtForecastAuthorize" runat="server" />
                          <tr>    
                    <td style="width:10%"><div style="font-weight: bold; margin-top: 0px;"><cb:ToolTipLabel ID="DisplayLabel" runat="server" Text="Display: " ToolTip="(First) Displays payments attempted for the first time.(Retry) Displays payments unsuccessfully attempted previously."></cb:ToolTipLabel><br /></div></td>
                 
                     <td >
                     <asp:RadioButtonList ID="RdDisplay" RepeatDirection="Horizontal" runat="server" >
                <asp:ListItem Value="First" Text="First" />
                <asp:ListItem Value="Retry" Text="Retry" />
                <asp:ListItem Selected="True" Value="Both" Text="Both" />
                </asp:RadioButtonList>  </div>   </td>
               
                 <td style="width:10%"><div style="font-weight: bold"><cb:ToolTipLabel ID="MethodLabel" runat="server" Text="Pivot: " ToolTip="(Gateway) Displays counts/amounts for each Payment Gateway used.(Instrument) Displays counts/amounts for each Payment Gateway and instrument used."></cb:ToolTipLabel></div></td> 
            
                 <td > <asp:RadioButtonList ID="rblGatewayInstrument" runat="server"  RepeatDirection=Horizontal>
                 <asp:ListItem Selected="True" Value="0">Gateway</asp:ListItem>
                            <asp:ListItem Value="1">Instrument</asp:ListItem>
                        </asp:RadioButtonList></td>
                               <td></td> 
                </tr>
                <tr> <td style="width:10%"> <asp:Label ID="Label1" runat="server" Text="Group By:" Font-Bold="True"></asp:Label></td>
                   <td colspan="3"><asp:RadioButtonList ID="rdGroupBy" RepeatDirection="Horizontal" runat="server" >
                <asp:ListItem Selected="True" Value="Date" Text="Date"/>
                <asp:ListItem Value="affiliateId" Text="Affiliate"/>
                <asp:ListItem Value="Subaffiliate" Text="Subaffiliate"/></asp:RadioButtonList> </td> </tr>
  
                <tr><td style="width:10%"><asp:Label ID="lbAffiliates" Text="Affiliates:" runat="server" style="display:inline;font-weight:bold;width:50px;" /></td>
                <td colspan="3"><asp:TextBox ID="txtAffiliates" runat="server" style="display:inline;font-weight:normal;width:250px;"/>
                   <ajax:TextBoxWatermarkExtender ID="twAffiliates" runat="server" targetControlID="txtAffiliates" WatermarkText="Comma separated AffiliateID(s)..." /></td></tr>
                <tr>
                <td style="width:10%" ><asp:Label ID="lbSubaffiliates" Text="Subaffiliates:" runat="server" style="display:inline;font-weight:bold;width:110px;" /></td>
                <td colspan="3" ><asp:TextBox ID="txtSubAffiliate" runat="server" style="display:inline;font-weight:normal;width:250px;"/> 
                <ajax:TextBoxWatermarkExtender ID="twSubAffiliates" runat="server" targetControlID="txtSubAffiliate" WatermarkText="Comma separated SubAffiliate code(s)..." />&nbsp;
                <asp:CheckBox ID="chkSubAffiliate" runat="server" Checked="False" Text="Exact match" /> </td>
              
                </tr>
                
                   <tr>
            <td style="width: 10%">
                <asp:Button ID="ProcessButton0" runat="server" OnClick="ProcessButton_Click" Text="GO.."
                    Style="vertical-align: top; height: 25px" />
            </td>
            <td  colspan="2">
                      <uc2:downloadbutton ID="downloadbutton1" FileName="ForecastAuthorize" runat="server" />
            </td>
            <td>&nbsp; </td>
        </tr>
        
                
                <tr>
                    <td class="dataSheet" colspan="4"> <div style="overflow-x:scroll;width:970px;">
                    <div id="grid">
                        <asp:GridView  ID="forecastReportgrid" runat="server" AutoGenerateColumns="true" 
                        OnPreRender="forecastReportgrid_PreRender" OnSorting="forecastReportgrid_Sorting"
                        onrowdatabound="forecastReportgrid_RowDataBound" OnPageIndexChanging="forecastReportgrid_PageIndexChanging"
                            DefaultSortExpression="Name" DefaultSortDirection="Ascending" AllowPaging="false" AllowSorting="true"
                            SkinID="Summary" PageSize="80" Width="100%">
                            <Columns>
                          
                            </Columns>
                            <EmptyDataTemplate>
                             <asp:Label ID="EmptyResultsMessage" runat="server" Text="There are no results for the selected time period."></asp:Label>
                             </EmptyDataTemplate>
                        </asp:GridView>
                        </div>
                        </div>
                    </td>
                </tr>
               
            </table>

        </ContentTemplate>
   
     </ajax:UpdatePanel>
     


</asp:Content>