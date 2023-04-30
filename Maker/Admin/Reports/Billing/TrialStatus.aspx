<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true"
    CodeFile="TrialStatus.aspx.cs" Inherits="Admin_Reports_Trial_TrialStatus" Title="Initial Billing" %>

<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls"
    TagPrefix="cb" %>
<%@ Register Assembly="wwhoverpanel" Namespace="Westwind.Web.Controls" TagPrefix="wwh" %>
<%@ Register Src="~/Admin/UserControls/DatesAndTime.ascx" TagName="DatesAndTime"
    TagPrefix="uc1" %>
    
      <%@ Register src="../../UserControls/downloadbutton.ascx" tagname="downloadbutton" tagprefix="uc2" %>
      
      
<asp:Content ID="Head1" ContentPlaceHolderID="Head" runat="Server">
    <link rel="stylesheet" type="text/css" href="../../../App_Themes/MakerShopAdmin/fixedHeaderTableSingleRow.css" />
    <style type="text/css">
        div.gv
        {
            display: block;
            position: relative;
            width: 950px;
            height: 400px;
            overflow: hidden;
            float: left;
        }
    </style>
    <script src="../../../js/jquery.js" type="text/javascript"></script>
    <script src="../../../js/jquery.fixedheadertable.1.0.b.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
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
            //me.dblclick(function() { $("div").filter("[type=ContextMenu]").hide(); });
           // me.blur(function() { $("div").filter("[type=ContextMenu]").hide(); });

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

            var strMenuHTML = "<div type=\"ContextMenu\" id=\"Menu" + ParentID + "\" style=\"position:absolute;left:" + left + "px;top:" + top + "\px;z-index:150; mouseonmenu=\"1\">loading...</div>";
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
    <div class="pageHeader">
        <div class="caption">
            <h1>
                Initial Bill Report</h1>
        </div>
    </div>

    <div id="grid">
    <table align="center" class="form" cellpadding="0" cellspacing="0" border="0" width="100%">
        <tr>
            <td style="width: 100%" colspan="4">
                <asp:Label ID="SavedMessage" runat="server" Text="Template saved at {0}." EnableViewState="false"
                    Visible="false" SkinID="GoodCondition"></asp:Label>
                <asp:Label ID="ErrorMessageLabel" runat="server" Text="" EnableViewState="false"
                    Visible="false" SkinID="ErrorCondition"></asp:Label>
            </td>
        </tr>
    
        <tr>
            <td style="width: 10%">
                <asp:Label ID="lbSelectProduct" runat="server" Text="Select Product:" Font-Bold="True" />
            </td>
            <td style="width: 30%">
                <asp:DropDownList ID="dlSku" runat="server" />
            </td>
            <td style="width: 60%" colspan="2">
            </td>
        </tr>
        <tr>
            <td style="width: 10%">
                <asp:Label ID="paymentGatewayPrefixLabel" runat="server" Text="Payment Gateway Prefix:"
                    Font-Bold="True" />
            </td>
            <td style="width: 30%">
                <asp:TextBox ID="paymentGatewayPrefixTxt" runat="server" />
            </td>
            <td style="width: 60%" colspan="2">
            </td>
        </tr>
        <!-- Custom control Date & Time colspan is 4 -->
        <uc1:DatesAndTime ID="dtTrialStatus" runat="server" />
        <tr>
            <td style="width: 10%">
                <asp:Label ID="lbGroupBy" Text="Group By:" runat="server" Font-Bold="True" />
            </td>
            <td  colspan="3">
                <asp:RadioButtonList ID="rdGroupBy" RepeatDirection="Horizontal" runat="server">
                    <asp:ListItem Selected="True" Value="Date" Text="Date" />
                    <asp:ListItem Value="affiliateId" Text="Affiliate" />
                    <asp:ListItem Value="Subaffiliate" Text="Subaffiliate" />
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td style="width: 10%" align="left">
                <asp:Label ID="lbAffiliates" Text="Affiliates:" runat="server" Style="display: inline;
                    font-weight: bold; width: 50px;" />
            </td>
            <td  colspan="3"">
                <asp:TextBox ID="txtAffiliates" runat="server" Style="display: inline; font-weight: normal;
                    width: 250px;" />
                <ajax:TextBoxWatermarkExtender ID="twAffiliates" runat="server" TargetControlID="txtAffiliates"
                    WatermarkText="Comma separated AffiliateID(s)..." />
            </td>
        </tr>
        <tr>
            <td style="width: 10%" align="left">
                <asp:Label ID="lbSubaffiliates" Text="Subaffiliates:" runat="server" Style="display: inline;
                    font-weight: bold; width: 110px;" />
            </td>
            <td >
                <asp:TextBox ID="txtSubAffiliate" runat="server" Style="display: inline; font-weight: normal;
                    width: 250px;" />
                <ajax:TextBoxWatermarkExtender ID="twSubAffiliates" runat="server" TargetControlID="txtSubAffiliate"
                    WatermarkText="Comma separated SubAffiliate code(s)..." />
            </td>
            <td >
                <asp:CheckBox ID="chkSubAffiliate" runat="server" Checked="False" Text="Exact match" />
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td style="width: 10%">
                <asp:Button ID="ProcessButton0" runat="server" OnClick="ProcessButton_Click" Text="GO.."
                    Style="vertical-align: top; height: 25px" />
            </td>
           <td  colspan="2">
                      <uc2:downloadbutton ID="downloadbutton1" FileName="ForecastAuthorize" runat="server" />
            </td>
            <td>&nbsp;
            </td>
        </tr>
        
        <tr>
            <td class="dataSheet" colspan="4">
                <div class="gv">
                    <asp:GridView ID="gridCharge" runat="server" AutoGenerateColumns="False" DefaultSortExpression="Name"
                        DefaultSortDirection="Ascending" AllowPaging="True" AllowSorting="true" PageSize="80" Width="100%"
                        OnSorting="gridCharge_Sorting" OnPageIndexChanging="gridCharge_PageIndexChanging"
                        OnPreRender="gridCharge_PreRender" OnRowDataBound="gridCharge_RowDataBound" >
                        <Columns>
                            <asp:TemplateField SortExpression="GroupBy">
                                <ItemStyle HorizontalAlign="left"  />
                                <HeaderStyle  />
                                <ItemTemplate>
                                    <asp:Label ID="lbGroupBy" runat="server" Text='<%# Eval("GroupBy", "{0:#,##0;}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="SubAffiliate" SortExpression="Subaffiliate" Visible="false">
                                <ItemStyle HorizontalAlign="right"  />
                                <ItemTemplate>
                                    <asp:Label ID="lbSubaffiliate" runat="server" Text='<%# formatSubAffiliate(Container.DataItem) %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Gross" SortExpression="Gross">
                                <ItemStyle HorizontalAlign="right"  />
                                <ItemTemplate>
                                    <asp:Label ID="lbGross" runat="server" Text='<%# Eval("Gross", "{0:#,##0;}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Hardx" SortExpression="Hardx">
                                <ItemStyle HorizontalAlign="right"  />
                                <ItemTemplate>
                                    <asp:Label ID="lbHardx" runat="server" Text='<%# Eval("Hardx", "{0:#,##0;}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="HardxPreTrial" SortExpression="HardxPreTrial">
                                <ItemStyle HorizontalAlign="right"  />
                                <ItemTemplate>
                                    <asp:Label ID="lbHardxPreTrial" runat="server" Text='<%# Eval("HardxPreTrial", "{0:#,##0;}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="HardXPostTrial" SortExpression="HardXPostTrial">
                                <ItemStyle HorizontalAlign="right"  />
                                <ItemTemplate>
                                    <asp:Label ID="lbHardXPostTrial" runat="server" Text='<%# Eval("HardXPostTrial", "{0:#,##0;}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="NotAttempted" SortExpression="NotAttempted">
                                <ItemStyle HorizontalAlign="right"  />
                                <ItemTemplate>
                                    <asp:Label ID="lbAttempted" runat="server" Text='<%# Eval("NotAttempted", "{0:#,##0;}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Attempted" SortExpression="Attempted">
                                <ItemStyle HorizontalAlign="right"  />
                                <ItemTemplate>
                                    <asp:Label ID="lbAttempted" runat="server" Text='<%# Eval("Attempted", "{0:#,##0;}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Authorized" SortExpression="Authorized" Visible="false">
                                <ItemStyle HorizontalAlign="right"  />
                                <ItemTemplate>
                                    <asp:Label ID="lbAuthorized" runat="server" Text='<%# Eval("Authorized", "{0:#,##0;}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Captured" SortExpression="Captured" Visible="false">
                                <ItemStyle HorizontalAlign="right"  />
                                <ItemTemplate>
                                    <asp:Label ID="lbCaptured" runat="server" Text='<%# Eval("Captured", "{0:#,##0;}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Approved" SortExpression="Approved">
                                <ItemStyle HorizontalAlign="right" />
                                <ItemTemplate>
                                    <asp:Label ID="lbApproved" runat="server" Text='<%# Eval("Approved", "{0:#,##0;}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Decline" SortExpression="Decline">
                                <ItemStyle HorizontalAlign="right"  />
                                <ItemTemplate>
                                    <asp:Label ID="lbDecline" runat="server" Text='<%# Eval("Decline", "{0:#,##0;}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Refund" SortExpression="Refund">
                                <ItemStyle HorizontalAlign="right"  />
                                <ItemTemplate>
                                    <asp:Label ID="lbRefund" runat="server" Text='<%# Eval("Refund", "{0:#,##0;}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Net Initial FULL Charge<br/>Success" SortExpression="NetFULLChargeSuccess">
                                <ItemStyle HorizontalAlign="right"  />
                                <ItemTemplate>
                                    <asp:Label ID="lbNetFULLChargeSuccess" runat="server" Text='<%# formatNetFULLChargeSuccess(Container.DataItem) %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Gross Approval FULL Attempted Charge<br/>Success"
                                SortExpression="ApprovalRate">
                                <ItemStyle HorizontalAlign="right"  />
                                <ItemTemplate>
                                    <asp:Label ID="lbApprovalRate" runat="server" Text='<%# formatApprovalRate(Container.DataItem) %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                             <asp:TemplateField HeaderText="Decline<br/>Reduced Attempted" SortExpression="DeclineReducedAttempted">
                                <ItemStyle HorizontalAlign="right"  />
                                <ItemTemplate>
                                    <asp:Label ID="lbDeclineReducedAttempted" runat="server" Text='<%#  Eval("DeclineReducedAttempted", "{0:#,##0;}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                            
                                <asp:TemplateField HeaderText="Decline<br/>Reduced Approved" SortExpression="DeclineReducedApproved">
                                <ItemStyle HorizontalAlign="right"  />
                                <ItemTemplate>
                                    <asp:Label ID="lbDeclineReducedApproved" runat="server" Text='<%#  Eval("DeclineReducedApproved", "{0:#,##0;}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            
                           
                        
                            <asp:TemplateField HeaderText="Decline<br/>ApprovalRate" SortExpression="DeclinedApprovalRate">
                                <ItemStyle HorizontalAlign="right" Width="40" />
                                <ItemTemplate>
                                    <asp:Label ID="lbDeclinedApprovalRate" runat="server" Text='<%# formatDeclinedApprovalRate(Container.DataItem) %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Net INITIAL Charge<br/>Success" SortExpression="NetINITIALChargeSuccess">
                                <ItemStyle HorizontalAlign="right" Width="40" />
                                <ItemTemplate>
                                    <asp:Label ID="lbNetINITIALChargeSuccess" runat="server" Text='<%# formatNetINITIALChargeSuccess(Container.DataItem) %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="GrossFraud" SortExpression="GrossFraud">
                                <ItemStyle HorizontalAlign="right" Width="40" />
                                <ItemTemplate>
                                    <asp:Label ID="lbGrossFraud" runat="server" Text='<%# Eval("GrossFraud", "{0:#,##0;}") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="FraudRate" SortExpression="FraudRate">
                                <ItemStyle HorizontalAlign="right" Width="40" />
                                <ItemTemplate>
                                    <asp:Label ID="lbFraudRate" runat="server" Text='<%# formatFraudRate(Container.DataItem)  %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                     
                    </asp:GridView>
                    <asp:Panel ID="EmptyResults" runat="server" BorderStyle="Double" BorderWidth="1" Visible="false" >
                    <center>
                    <br /><br />
                        <asp:Label ID="EmptyResultsMessage" runat="server" Text="There are no results for the selected time period."></asp:Label>
                            <br /><br />
                        </center>
                    </asp:Panel>
                </div>
           </td>
           </tr>
       
    </table>
</div>
<!-- make an invisble panel to make up for the empty results -->
    <script type="text/javascript">
        $(document).ready(function() {
            var rows = $('.gv').find("tbody > tr").get().length; // +$('.gv').find("thead > tr").get().length;

            if (rows >= 1) {
                var windowWidth = $('.gv').width() - 50;

                var windowHeight = (rows * 27) + 120;

                if (windowHeight > 2810)
                    windowHeight = 2810;

                $('.gv').css({ 'width': windowWidth + 'px', 'height': windowHeight + 'px' });

                $(window).resize(function() {


                    $('.gv').css({ 'width': windowWidth + 'px', 'height': windowHeight + 'px' });
                });


                $('.gv').fixedHeaderTable({ autoResize: false, fixCol1: true });
            }
        });
    </script>

 
</asp:Content>
