<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SubscribeToEmailList.ascx.cs" Inherits="ConLib_SubscribeToEmailList" %>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
<%--
<conlib>
<summary>Displays a simple email list signup form. This form can be added to side bars.</summary>
<param name="Caption" default="Subscribe To Email List">Possible value can be any string.  Title of the control.</param>
<param name="EmailListId" default="0">Possible value can be any integer.  This is the ID of the Email List to be subscribed to. If no email list is specified the store's default email list is used.</param>
</conlib>
--%>
<div class="section">
    <div class="header">
        <h2><asp:Localize ID="CaptionLabel" runat="server" Text="Subscribe To Email List" /></h2>
    </div>
    <div style="padding:2px;">
        <strong><asp:Literal ID="InstructionsText" runat="server" Text="Enter your email address below to subscribe to {0}." /></strong>
    </div>
    <div class="content">
        <ajax:UpdatePanel ID="SubscribeToEmailListAjax" runat="server" UpdateMode="Conditional">
            <ContentTemplate>            
                 <table class="inputForm" width="100%">
                    <tr>            
                        <td >
                            <asp:Label ID="SubscribedMessage" runat="server" SkinID="GoodCondition" EnableViewState="false" Visible="false" Text="Your email '{0}' is now subscribed to '{1}'."></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <th>
                             <asp:Label ID="UserEmailLabel" runat="server" Text="Your Email:" AssociatedControlID="UserEmail" ></asp:Label>
                             <cb:EmailAddressValidator ID="EmailAddressValidator1" runat="server" ControlToValidate="UserEmail" ValidationGroup="SubscribeEmailList" Display="Dynamic" Required="true" ErrorMessage="From email address should be in the format of name@domain.tld." Text="*" EnableViewState="False"></cb:EmailAddressValidator>                             
                        </th>
                    </tr>    
                    <tr>
                        <td align="center">
                            <asp:TextBox ID="UserEmail" runat="server" Text="" Width="150px" MaxLength="200"></asp:TextBox>
                        </td>                        
                    </tr>
                    <tr>                
                        <td >
                            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="SubscribeEmailList" />
                        </td>
                    </tr>                    
                    <tr>                
                        <td  align="right">
                            <asp:Button ID="SubscribeButton" runat="server" Text="Subscribe Now" OnClick="SubscribeButton_Click" ValidationGroup="SubscribeEmailList"></asp:Button>
                        </td>
                    </tr>
                </table>
         </ContentTemplate>
        </ajax:UpdatePanel>
    </div>
</div>
