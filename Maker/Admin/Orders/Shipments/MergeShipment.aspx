<%@ Page Language="C#" MasterPageFile="~/Admin/Orders/Order.master" CodeFile="MergeShipment.aspx.cs" Inherits="Admin_Orders_Shipments_MergeShipment" Title="Merge Shipment" %>



<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">    
<div class="pageHeader"> 
    <div class="caption"> 
        <h1><asp:Label ID="Caption" runat="server" Text="Merge Shipment #{0}"></asp:Label></h1> 
    </div> 
</div>
<table border="0" cellpadding="3"> 
    <tr> 
        <td colspan="2">
            When you merge this shipment, the items it holds will be moved to another shipment of your choosing.  This shipment will then be removed from the order.  This shipment contains the following order items:
        </td> 
    </tr> 
    <tr>
        <td colspan="2">
            <asp:GridView ID="ShipmentItems" runat="server" ShowHeader="true" 
                AutoGenerateColumns="false" Width="100%" SkinID="PagedList">
                <Columns>
                    <asp:TemplateField HeaderText="Sku">
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <%#Eval("Sku")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Item">
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <%#Eval("Name")%>
                            <asp:Literal ID="VariantName" runat="Server" Text='<%#Eval("VariantName", " ({0})")%>' Visible='<%#!String.IsNullOrEmpty((string)Eval("VariantName"))%>'></asp:Literal><br />
                            <asp:Panel ID="InputPanel" runat="server" Visible='<%#(((ICollection)Eval("Inputs")).Count > 0)%>'>
                                <asp:DataList ID="InputList" runat="server" DataSource='<%#Eval("Inputs") %>'>
                                    <ItemTemplate>
                                        <asp:Label ID="InputName" Runat="server" Text='<%#Eval("Name") + ":"%>' SkinID="fieldheader"></asp:Label>
                                        <asp:Label ID="InputValue" Runat="server" Text='<%#Eval("InputValue")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:DataList>
                            </asp:Panel>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Quantity">
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <%#Eval("Quantity")%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Price">
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <%# Eval("Price", "{0:lc}") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Total">
                        <ItemStyle HorizontalAlign="Right" />
                        <ItemTemplate>
                            <%# Eval("ExtendedPrice", "{0:lc}") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </td>
    </tr>
    <tr>
        <td colspan="2">
            <table class="inputForm" cellpadding="3" cellspacing="0">
                <tr>
                    <th align="right">
                        Move these items to: 
                    </th>
                    <td>
                        <asp:DropDownList ID="ShipmentsList" runat="server"></asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>
                        <br />
                        <asp:LinkButton ID="MergeButton" runat="server" Text="Merge" OnClick="MergeButton_Click" SkinID="Button" />
                        <asp:HyperLink ID="CancelLink" runat="server" Text="Cancel" NavigateUrl="Default.aspx" SkinID="Button" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
</asp:Content>

