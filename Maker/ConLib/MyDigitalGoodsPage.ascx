<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MyDigitalGoodsPage.ascx.cs" Inherits="ConLib_MyDigitalGoodsPage" %>
<%--
<conlib>
    <summary>Display details of digital goods attached to an order and options to download them.</summary>
</conlib>
--%>
<div class="pageHeader">
    <h1><asp:Localize ID="Caption" runat="server" Text="Digital Goods Purchased"></asp:Localize></h1>
</div>
<div align="center">
<asp:GridView ID="DigitalGoodsGrid" runat="server" AutoGenerateColumns="false" AllowPaging="false" 
    AllowSorting="false" DataKeyNames="OrderItemDigitalGoodId"  CellPadding="4" SkinID="PagedList">
    <Columns>
        <asp:TemplateField HeaderText="Name" ItemStyle-HorizontalAlign="Left">
            <ItemTemplate>
                <asp:HyperLink ID="Name" runat="server" Text='<%#Eval("DigitalGood.Name")%>' NavigateUrl='<%# GetDownloadUrl(Container.DataItem) %>'></asp:HyperLink>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="# Downloads" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>                    
                <asp:Label ID="Downloads" runat="server" Text='<%#Eval("RelevantDownloads")%>' />
                <asp:Label ID="MaxDownloads" runat="server" Text='<%#GetMaxDownloads(Container.DataItem)%>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Serial Key" ItemStyle-HorizontalAlign="Left">
             <ItemTemplate>
                  <asp:Label ID="SerialKey" runat="server" Visible='<%#ShowSerialKey(Container.DataItem)%>' Text='<%#Eval("SerialKeyData")%>'  />
                  <asp:LinkButton Visible='<%#ShowSerialKeyLink(Container.DataItem)%>' runat="server" ID="SerialKeyLink" Text="view" OnClientClick="<%#GetPopUpScript(Container.DataItem)%>"></asp:LinkButton>
             </ItemTemplate>
        </asp:TemplateField>            
        <asp:TemplateField HeaderText="Status" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:Label ID="DownloadStatus" runat="server" Text='<%#Eval("DownloadStatus")%>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
    <EmptyDataTemplate>
        <asp:Label ID="EmptyDataMessage" runat="server" Text="There are no digital goods."></asp:Label>
    </EmptyDataTemplate>
</asp:GridView>
</div>