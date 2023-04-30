<%@Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Default.aspx.cs" Inherits="Admin_Store_Security_Default" Title="Configure Security"  %>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageHeader">
        <div class="caption">
            <h1><asp:Localize ID="Caption" runat="server" Text="Configure Security"></asp:Localize></h1>
        </div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td width="50%" valign="top">
                <ajax:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <table cellpadding="0" cellspacing="0"><tr><td>
			                <div class="section">
                                <div class="header">
                                    <h2 class="visa"><asp:Localize ID="PaymentSecurityCaption" runat="server" Text="Payment Account Data Storage"></asp:Localize></h2>
                                </div>
                                <div class="content">
        		                    <asp:Label ID="SavedMessage" runat="server" Text="Settings saved.<br /><br />" Visible="false" SkinID="GoodCondition"></asp:Label>
                                    <asp:Label ID="PaymentSecurityHelpText" runat="server" Text="After a payment is successfuly processed, how many days would you like to retain associated account details (e.g. credit card numbers)?  The most secure option is to not save (0), but you may need to retain the details for post order processing."></asp:Label>
                                    <table class="inputForm">
                                        <tr>
                                            <th class="rowHeader">
                                                <asp:Label ID="PaymentSecurityLabel" runat="Server" SkinID="FieldHeader" Text="Days to Save: "></asp:Label>
                                            </th>
                                            <td>
                                                <asp:DropDownList ID="PaymentLifespan" runat="server">
                                                </asp:DropDownList>
                                            </td>
                                        </tr>
                                    </table>
                                    <asp:Label ID="EnableCreditCardStorageHelpText" runat="server" Text="When credit card storage is enabled, encrypted card data is saved in the database according to setting above.  If you choose not to enable storage of account data, credit card numbers will never be saved to the database under any circumstance."></asp:Label>
                                    <table class="inputForm">
                                        <tr>
                                            <th class="rowHeader">
                                                <asp:Label ID="EnableCreditCardStorageLabel" runat="Server" Text="Enable Credit Card Storage:" AssociatedControlID="EnableCreditCardStorage" ToolTip=""></asp:Label>
                                            </th>
                                            <td>
                                                <asp:CheckBox ID="EnableCreditCardStorage" runat="server" />
                                            </td>
                                        </tr>
                                    </table>
                                    <asp:Button Id="SaveButon" runat="server" Text="Save" OnClick="SaveButon_Click" />
                                </div>
                            </div>
                         </td></tr></table>
                    </ContentTemplate>
                </ajax:UpdatePanel>
            </td>
            <td width="50%" valign="top">
                <ajax:UpdatePanel ID="SSLAjax" runat="server">
                    <ContentTemplate>
                        <table cellpadding="0" cellspacing="0"><tr><td>
                            <div class="section">
                                <div class="header">
                                    <h2 class="ssl"><asp:Localize ID="CurrentSSLCaption" runat="server" Text="Secure Sockets Layer (SSL)"></asp:Localize></h2>
                                </div>
                                <div class="content">
                                    <asp:Label ID="SSLHelpText" runat="server" Text="SSL is used to create a secure connection between the browser and the server.  You must enable SSL in order to safely collect credit card numbers over the Internet."></asp:Label>
			                            <asp:Panel ID="ViewSSL_Panel" runat="server" CssClass="section">
                                            <table class="inputForm">
                                                <tr>
                                                    <th class="rowHeader">
                                                        <asp:Label ID="SSLEnabledLabel" runat="Server" Text="SSL Enabled:"></asp:Label>
                                                    </th>
                                                    <td>
                                                        <asp:Label ID="SSLEnabled" runat="server" Text="yes" SkinID="GoodCondition" />
                                                        <asp:Label ID="SSLDisabled" runat="server" Text="no" SkinID="ErrorCondition" />
                                                    </td>
                                                </tr>
                                                <tr id="SecureDomainPanel" runat="server">
                                                    <th class="rowHeader">
                                                        <asp:Label ID="SecureDomainLabel" runat="Server" Text="Secure Domain:"></asp:Label>
                                                    </th>
                                                    <td>
                                                        <asp:Label ID="SecureDomain" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                            </table>
                                            <asp:Button ID="ChangeSSLButton" runat="server" Text="Change" OnClick="ChangeSSLButton_Click" />
                                        </asp:Panel>
                                 </div>
    		                        
		                                <asp:Panel ID="ChangeSSL_Panel" runat="server" CssClass="section">
		                                <table class="inputForm">
		                                    <tr>
		                                        <th class="rowHeader" nowrap>
		                                            <asp:Label ID="ChangeSSL_EnableLabel" runat="server" Text="SSL Enabled:"></asp:Label>
		                                        </th>
		                                        <td>
		                                            <asp:CheckBox ID="ChangeSSL_Enable" runat="server" OnCheckedChanged="ChangeSSL_Enable_CheckedChanged" AutoPostBack="true" />
		                                        </td>
		                                    </tr>
		                                    <tr ID="ChangeSSL_SecureDomainPanel" runat="server">
		                                        <th class="rowHeader" valign="bottom" nowrap>
		                                            <asp:Label ID="ChangeSSL_SecureDomainLabel" runat="server" Text="SSL Domain:" SkinID="FieldHeader"></asp:Label>
		                                        </th>
		                                        <td valign="bottom">
    		                                        <asp:Label ID="ChangeSSL_SecureDomainHelpText" runat="server" Text="If your SSL domain is different from your regular domain, provide it here.  For example: secure.yoursite.com"></asp:Label><br /><br />
    		                                        <asp:TextBox ID="ChangeSSL_SecureDomain" runat="Server" Text="" width="200px"></asp:TextBox>&nbsp;<asp:Label ID="ChangeSSL_SecureDomainOptionalLabel" runat="server" Text="(optional)"></asp:Label>
		                                        </td>
		                                    </tr>
		                                    <tr ID="ChangeSSL_ConfirmPanel" runat="server">
		                                        <td colspan="2">
                                                    <br /><asp:Label ID="ChangeSSL_ConfirmHelpText" runat="server" Text="Click the link below to open your store homepage in a new window.  Verify the page loads successfully, then return to this screen.  Check the box to confirm the URL is accessible and click Finish to save your settings.  Failure to follow these steps could render your merchant administration inaccessible."></asp:Label><br /><br />
                                                    <asp:HyperLink ID="ChangeSSL_ConfirmLink" runat="server" Target="_blank"></asp:HyperLink><br /><br />
                                                    <asp:CheckBox ID="ChangeSSL_Confirmed" runat="server" AutoPostBack="true" OnCheckedChanged="ChangeSSL_Confirmed_CheckChanged" />
                                                    <asp:Label ID="ChangeSSL_ConfirmedLabel" runat="server" Text="The link above is accessible, save the new SSL settings."></asp:Label><br />
		                                        </td>
		                                    </tr>
		                                </table>
                                        <asp:Button ID="ChangeSSL_CancelButton" runat="server" Text="Cancel" OnClick="ChangeSSL_CancelButton_Click" />
                                        <asp:Button ID="ChangeSSL_NextButton" runat="server" Text="Next" OnClick="ChangeSSL_NextButton_Click" />
                                        <asp:Button ID="ChangeSSL_FinishButton" runat="server" Text="Finish" Visible="false" OnClick="ChangeSSL_FinishButton_Click" />
		                            </asp:Panel>
                                </div>
                            </div>
                          </td></tr></table>
                    </ContentTemplate>
                    <Triggers>
                        <ajax:PostBackTrigger ControlID="ChangeSSL_FinishButton"></ajax:PostBackTrigger>
                    </Triggers>
                </ajax:UpdatePanel>
            </td>
        </tr>
    </table>
</asp:Content>

