<%@ Control Language="C#" CodeFile="Disclaimer.ascx.cs" Inherits="ConLib_Disclaimer" %>
<%--
<conlib>
<summary>Content page to show disclaimer message.</summary>
<param name="NoCookiesMessage" default="Your browser does not support cookies or cookies are disabled. This site uses cookies to store information, please enable cookies otherwise you will not be able to use our site.">Possible value can be any string.  A message which will be displayed when cookies are disabled or the customer browser does not support cookies.</param>
</conlib>
--%>
<table cellspacing="0" cellpadding="0" class="page" width="90%">
    <tr id="trDisclaimerMessage" runat="server">
        <td class="pageMain">
            <asp:Label ID="DisclaimerText" runat="server"></asp:Label><br />
            <br />
            <div align="center">
                <asp:Button ID="OkButton" runat="server" Text="I Accept" OnClick="OkButton_Click"
                    OnClientClick="document.cookie ='SiteDisclaimerAccepted=True';return true;" />
            </div>
        </td>
    </tr>
    <tr id="trNoCookies" runat="server">
        <td class="pageMain">
            <asp:Label SkinID="ErrorCondition" ID="NoCookiesMessageLabel" runat="server" />
        </td>
    </tr>
</table>