<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="DailySales.aspx.cs" Inherits="Admin_Reports_DailySales" Title="Daily Sales"  %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    
    <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
        
        <div class="pageHeader">
                <div class="caption">
                    <h1><asp:Localize ID="Caption" runat="server" Text="Daily Sales Report"></asp:Localize><asp:Localize ID="ReportCaption" runat="server" Text=" for {0:d}" Visible="false" EnableViewState="false"></asp:Localize></h1>
                </div>
        </div>
        <br />
        <ItemTemplate>
            <div class="noPrint" style="text-align:center;">
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
            </div><br />
            <table align="center" class="form" cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <td align="center" class="dataSheet">
                        <asp:GridView ID="DailySalesGrid" runat="server" AutoGenerateColumns="False" ShowFooter="True" 
                            SkinID="Summary" Width="100%" FooterStyle-CssClass="totalRow">
                            <Columns>
                                <asp:TemplateField HeaderText="Order">
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                    <FooterStyle HorizontalAlign="Right" />
                                    <ItemTemplate>
                                        <asp:Label ID="OrderLabel" runat="server" Text='<%# Eval("OrderNumber") %>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="FooterTotalsLabel" runat="server" Text="Totals:" SkinID="FieldHeader"></asp:Label>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Products">
                                    <HeaderStyle HorizontalAlign="Right" />
                                    <ItemStyle HorizontalAlign="Right" />
                                    <FooterStyle HorizontalAlign="Right" />
                                    <ItemTemplate>
                                        <asp:Label ID="Label2" runat="server" Text='<%# Eval("ProductTotal", "{0:lc}") %>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="ProductTotal" runat="server" Text='<%# GetTotal("Product") %>'></asp:Label>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Shipping">
                                    <HeaderStyle HorizontalAlign="Right" />
                                    <ItemStyle HorizontalAlign="Right" />
                                    <FooterStyle HorizontalAlign="Right" />
                                    <ItemTemplate>
                                        <asp:Label ID="Label3" runat="server" Text='<%# Eval("ShippingTotal", "{0:lc}") %>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="ShippingTotal" runat="server" Text='<%# GetTotal("Shipping") %>'></asp:Label>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Tax">
                                    <HeaderStyle HorizontalAlign="Right" />
                                    <ItemStyle HorizontalAlign="Right" />
                                    <FooterStyle HorizontalAlign="Right" />
                                    <ItemTemplate>
                                        <asp:Label ID="Label4" runat="server" Text='<%# Eval("TaxTotal", "{0:lc}") %>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="TaxTotal" runat="server" Text='<%# GetTotal("Tax") %>'></asp:Label>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Discount">
                                    <HeaderStyle HorizontalAlign="Right" />
                                    <ItemStyle HorizontalAlign="Right" />
                                    <FooterStyle HorizontalAlign="Right" />
                                    <ItemTemplate>
                                        <asp:Label ID="Label5" runat="server" Text='<%# Eval("DiscountTotal", "{0:lc}") %>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="DiscountTotal" runat="server" Text='<%# GetTotal("Discount") %>'></asp:Label>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Coupon">
                                    <HeaderStyle HorizontalAlign="Right" />
                                    <ItemStyle HorizontalAlign="Right" />
                                    <FooterStyle HorizontalAlign="Right" />
                                    <ItemTemplate>
                                        <asp:Label ID="Label6" runat="server" Text='<%# Eval("CouponTotal", "{0:lc}") %>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="CouponTotal" runat="server" Text='<%# GetTotal("Coupon") %>'></asp:Label>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Other">
                                    <HeaderStyle HorizontalAlign="Right" />
                                    <ItemStyle HorizontalAlign="Right" />
                                    <FooterStyle HorizontalAlign="Right" />
                                    <ItemTemplate>
                                        <asp:Label ID="Label7" runat="server" Text='<%# Eval("OtherTotal", "{0:lc}") %>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="OtherTotal" runat="server" Text='<%# GetTotal("Other") %>'></asp:Label>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Profit">
                                    <HeaderStyle HorizontalAlign="Right" />
                                    <ItemStyle HorizontalAlign="Right" />
                                    <FooterStyle HorizontalAlign="Right" />
                                    <ItemTemplate>
                                        <asp:Label ID="ProfitLabel" runat="server" Text='<%# Eval("ProfitTotal", "{0:lc}") %>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="ProfitTotal" runat="server" Text='<%# GetTotal("Profit") %>'></asp:Label>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Total">
                                    <HeaderStyle HorizontalAlign="Right" />
                                    <ItemStyle HorizontalAlign="Right" />
                                    <FooterStyle HorizontalAlign="Right" />
                                    <ItemTemplate>
                                        <asp:Label ID="Label8" runat="server" Text='<%# Eval("GrandTotal", "{0:lc}") %>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="GrandTotal" runat="server" Text='<%# GetTotal("Grand") %>'></asp:Label>
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemStyle HorizontalAlign="Center" CssClass="noPrint" />
                                    <ItemTemplate>
                                        <asp:HyperLink ID="DetailsLink" runat="server" Text="Details" SkinID="Button" NavigateUrl='<%#String.Format("../Orders/ViewOrder.aspx?OrderNumber={0}&OrderId={1}", Eval("OrderNumber"), Eval("OrderId"))%>'></asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                            </Columns>
                            <EmptyDataTemplate>
                                <asp:Label ID="EmptyResultsMessage" runat="server" Text="There are no orders for the selected date."></asp:Label>
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>

