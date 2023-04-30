<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Login.aspx.cs" Inherits="Admin_Login" Title="Merchant Login" %>
<%@ Register Src="UserControls/LoginDialog.ascx" TagName="LoginDialog" TagPrefix="uc" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="loginWrapper" style="width:400px;margin:0 auto;padding:20px 0;">
        <uc:LoginDialog ID="LoginDialog1" runat="server" />
    </div>
</asp:Content>

