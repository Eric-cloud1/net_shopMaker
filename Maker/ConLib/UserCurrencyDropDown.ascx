<%@ Control Language="C#" AutoEventWireup="true" CodeFile="UserCurrencyDropDown.ascx.cs" Inherits="ConLib_UserCurrencyDropDown" %>
<%--
<conlib>
<summary>Ajax enabled simple dropdown control to select a preferred currency.</summary>
</conlib>
--%>
<ajax:UpdatePanel ID="UserCurrencyAjax" runat="server">
    <ContentTemplate>
        <asp:DropDownList ID="UserCurrency" runat="server" DataSourceID="CurrencyDs" DataTextField="Name" DataValueField="CurrencyId" AutoPostBack="true" OnSelectedIndexChanged="UserCurrency_SelectedIndexChanged" AppendDataBoundItems="true" OnDataBound="UserCurrency_DataBound">
            <asp:ListItem Value="" Text=""></asp:ListItem>
        </asp:DropDownList>
		<asp:Label ID="UpdateMessage" runat="server" Visible="false" SkinID="GoodCondition" Text="Currency Updated to {0}."></asp:Label> 
        <asp:ObjectDataSource ID="CurrencyDs" runat="server" OldValuesParameterFormatString="original_{0}"
            SelectMethod="LoadForStore" TypeName="MakerShop.Stores.CurrencyDataSource" DataObjectTypeName="MakerShop.Stores.Currency"
            DeleteMethod="Delete" UpdateMethod="Update" SortParameterName="sortExpression" InsertMethod="Insert">
        </asp:ObjectDataSource>
    </ContentTemplate>
</ajax:UpdatePanel>