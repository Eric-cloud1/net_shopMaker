<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="DailyAbandonedBaskets.aspx.cs" Inherits="Admin_Reports_DailyAbandonedBaskets" Title="Daily Abandoned Baskets"
     %>
<%@ Register Namespace="Westwind.Web.Controls" Assembly="wwhoverpanel" TagPrefix="wwh" %>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">
<script type="text/javascript">
    function ShowBasket(event,Id)
    { 
        BasketHoverLookupPanel.startCallback(event,"BasketId=" + Id.toString(),null,OnError);    
    }
    function HideBasket()
    {
        BasketHoverLookupPanel.hide();
        
        // *** If you don't use shadows, you can fade out
        //LookupPanel.fadeout();
    }
    function OnCompletion(Result)
    {
        //alert('done it!\r\n' + Result);
    }
    function OnError(Result)
    {
        alert("*** Error:\r\n\r\n" + Result.message);    
    }
</script>

    <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="pageHeader">
                <div class="caption">
                    <h1>
                        <asp:Localize ID="Caption" runat="server" Text="Abandoned Baskets"></asp:Localize><asp:Localize
                            ID="ReportCaption" runat="server" Text=" for {0:d}" Visible="false" EnableViewState="false"></asp:Localize></h1>
                </div>
            </div>
           <%-- <table cellpadding="2" cellspacing="0" class="innerLayout">--%>
            <table align="center" class="form" cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <td>
                        <div class="noPrint" style="text-align: center;">
                            <asp:Button ID="PreviousButton" runat="server" Text="&laquo; Previous" OnClick="PreviousButton_Click" />
                            &nbsp;
                            <asp:Label ID="DayListLabel" runat="server" Text="Day: " SkinID="FieldHeader"></asp:Label>
                            <asp:DropDownList ID="DayList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ReportDate_SelectedIndexChanged">
                            </asp:DropDownList>&nbsp;
                            <asp:Label ID="MonthLabel" runat="server" Text="Month: " SkinID="FieldHeader"></asp:Label>
                            <asp:DropDownList ID="MonthList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ReportDate_SelectedIndexChanged">
                                <asp:ListItem Value="1" Text="January"></asp:ListItem>
                                <asp:ListItem Value="2" Text="February"></asp:ListItem>
                                <asp:ListItem Value="3" Text="March"></asp:ListItem>
                                <asp:ListItem Value="4" Text="April"></asp:ListItem>
                                <asp:ListItem Value="5" Text="May"></asp:ListItem>
                                <asp:ListItem Value="6" Text="June"></asp:ListItem>
                                <asp:ListItem Value="7" Text="July"></asp:ListItem>
                                <asp:ListItem Value="8" Text="August"></asp:ListItem>
                                <asp:ListItem Value="9" Text="September"></asp:ListItem>
                                <asp:ListItem Value="10" Text="October"></asp:ListItem>
                                <asp:ListItem Value="11" Text="November"></asp:ListItem>
                                <asp:ListItem Value="12" Text="December"></asp:ListItem>
                            </asp:DropDownList>&nbsp;
                            <asp:Label ID="YearLabel" runat="server" Text="Year: " SkinID="FieldHeader"></asp:Label>
                            <asp:DropDownList ID="YearList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ReportDate_SelectedIndexChanged">
                            </asp:DropDownList>
                            &nbsp;
                            <asp:Button ID="NextButton" runat="server" Text="Next &raquo;" OnClick="NextButton_Click" />
                        </div>
                        <br />
                    </td>
                </tr>
                <tr>
                    <td class="dataSheet">
                    
                        <asp:GridView ID="DailyAbandonedBasketsGrid" runat="server" AutoGenerateColumns="False"
                            Width="100%" ShowFooter="False" DataSourceID="DailyAbandonedBasketsDs" SkinID="PagedList" >
                            <Columns>
                                <asp:TemplateField HeaderText="Customer">
                                    <ItemTemplate>
                                        <asp:Label ID="CustomerLabel" runat="server" Text='<%# Eval("Customer") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Items in Basket">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <asp:Label ID="Label0" runat="server" Text='<%# Eval("ItemCount", "{0}") %>'></asp:Label>
                                        <a href="../Orders/Create/CreateOrder2.aspx?UID=<%#GetUserId((int)Eval("BasketId")) %>" onmouseover='ShowBasket(event,"<%# Eval("BasketId") %>");' onmouseout='HideBasket();'>
                                            <asp:Image ID="PreviewIcon" runat="server" SkinID="PreviewIcon" />
                                        </a>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Basket Total">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <asp:Label ID="Label2" runat="server" Text='<%# Eval("BasketTotal", "{0:lc}") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Last Activity">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <asp:Label ID="Label3" runat="server" Text='<%# Eval("LastActivity") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <div class="emptyResult">
                                    <asp:Label ID="EmptyResultsMessage" runat="server" Text="There are no abandoned baskets for the selected date."></asp:Label>
                                </div>
                            </EmptyDataTemplate>
                       </asp:GridView>
                        <asp:ObjectDataSource ID="DailyAbandonedBasketsDs" runat="server" OldValuesParameterFormatString="original_{0}"
                            SelectMethod="GetDailyAbandonedBaskets" TypeName="MakerShop.Reporting.ReportDataSource">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="YearList" Name="year" PropertyName="SelectedValue"
                                    Type="Int32" />
                                <asp:ControlParameter ControlID="MonthList" Name="month" PropertyName="SelectedValue"
                                    Type="Int32" />
                                <asp:ControlParameter ControlID="DayList" Name="day" PropertyName="SelectedValue"
                                    Type="Int32" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajax:UpdatePanel>
    <wwh:wwHoverPanel ID="BasketHoverLookupPanel" runat="server" ServerUrl="~/Admin/Reports/BasketSummary.ashx"
        NavigateDelay="250" ScriptLocation="WebResource" Style="display: none; background: white;"
        PanelOpacity="0.89" ShadowOffset="8" ShadowOpacity="0.18" CssClass="HoverBasketPanel">
        <div id="BasketHoverPanelHeader" class="gridheader" style="padding: 2px">
            Basket Contents</div>
        <div id="BasketHoverPanelContent" style="padding: 10px; background: cornsilk;">
        </div>
    </wwh:wwHoverPanel>
</asp:Content>
