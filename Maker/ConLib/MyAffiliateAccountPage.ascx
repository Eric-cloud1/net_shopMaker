<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MyAffiliateAccountPage.ascx.cs" Inherits="ConLib_MyAffiliateAccountPage" %>
<%@ Register Src="AffiliateRegForm.ascx" TagName="AffiliateRegForm" TagPrefix="uc1" %>
<%--
<conlib>
<summary>Content page to show options for affiliate registration and reports.</summary>
</conlib>
--%>
<table cellspacing="0" cellpadding="0" class="page" width="100%">
    <tr id="trNoAffiliatePanel" runat="server"  visible="false">
        <td class="pageMain">
            <asp:Label ID="NoAffiliateMessage" runat="server" Text="You are currently logged on as {0}, and no affiliate record is associated with this user."></asp:Label>
            <br />
            <asp:Label ID="LoginMessage" runat="server" Text="To log in as a different user, "></asp:Label><asp:HyperLink ID="LoginLink" runat="server" NavigateUrl='~/Login.aspx' >click here.</asp:HyperLink>
            <br />
            <asp:Label ID="RegisterMessage" runat="server" Text=" To register as a new affiliate, "></asp:Label><asp:HyperLink ID="RegisterLink" runat="server" Text="click here." NavigateUrl="~/Members/EditAffiliateAccount.aspx">click here.</asp:HyperLink>
        </td>
    </tr>
    <tr id="trAffiliateReport" runat="server"  visible="false">
        <td class="pageMain">            
            Affiliate Report here.            
        </td>
    </tr>
    <tr id="trAffiliateInfo" runat="server"  visible="false">
        <td class="pageMain">
            <asp:Label ID="InstructionText" runat="server" Text="Your affiliate ID is {0}. To associate a link with this affiliate, add <b>afid={0}</b> to the url. For example:<br />{1}?afid={0}<br /><br />"></asp:Label>            
            <br />
            <asp:Label ID="EditRegistrationMessage" runat="server" Text="To edit your affiliate registration information, "></asp:Label><asp:HyperLink ID="LinkButton1" runat="server" Text="click here." NavigateUrl="~/Members/EditAffiliateAccount.aspx" ></asp:HyperLink>
        </td>
    </tr>    
</table>