<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SimpleSearch.ascx.cs" Inherits="ConLib_SimpleSearch" EnableViewState="false" %>
<%--
<conlib>
<summary>Displays a simple search box where search keywords can be entered, for results the customer will be redirected to search page.</summary>
</conlib>
--%>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
<div style="display:none"><asp:ValidationSummary ID="SearchValidation" runat="server" ShowMessageBox="true" ValidationGroup="Search" /></div>
<asp:Panel ID="SearchPanel" runat="server">
<asp:TextBox ID="SearchPhrase" runat="server" CssClass="searchPhrase" MaxLength="60"></asp:TextBox><asp:Button ID="SearchButton" runat="server" Text="Search"
     OnClick="SearchButton_Click" CssClass="searchButton" SkinID="ignore" 
     CausesValidation="true" ValidationGroup="Search"
     OnClientClick="if(Page_ClientValidate('Search')){{window.location='{0}?k='+encodeURIComponent({1}.value);}}return false;" />
<cb:SearchKeywordValidator ID="SearchPhraseValidator" runat="server" ControlToValidate="SearchPhrase" 
    ErrorMessage="Search keyword must be at least {0} characters in length excluding spaces and wildcards."
    Text="*" ValidationGroup="Search" Display="None" KeywordRequired="true"></cb:SearchKeywordValidator>     
</asp:Panel>