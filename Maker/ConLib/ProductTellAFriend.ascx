<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProductTellAFriend.ascx.cs" Inherits="ConLib_ProductTellAFriend" %>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
<%--
<conlib>
<summary>Display product a friend form, using that an email can be send to a friend about a specific product. This form can be added to side bars.</summary>
<param name="Caption" default="Email A Friend">Possible value can be any string.  Title of the control.</param>
</conlib>
--%>
<div class="section">
    <div class="header">
        <h2><asp:Localize ID="CaptionLabel" runat="server" Text="Email A Friend" /></h2>
    </div>
    <div class="content" >
		<div class="labelheading">
			<strong><asp:Literal ID="InstructionsText" runat="server" Text="Send this product to a friend..." /></strong>
		</div>
        <ajax:UpdatePanel ID="TellAFriendAjax" runat="server" UpdateMode="Conditional">
            <ContentTemplate>            
                 <table class="inputForm" width="100%">
                    <tr>
                        <th>
                             <asp:Label ID="NameLabel" runat="server" Text="Your Name:" AssociatedControlID="Name" CssClass="H2"></asp:Label>
                             <asp:RequiredFieldValidator ID="NameRequired" runat="server" ControlToValidate="Name"
                                ErrorMessage="Your name is required" ValidationGroup="TellAFriend">*</asp:RequiredFieldValidator>
                        </th>
                    </tr>    
                    <tr>
                        <td align="center">
                            <asp:TextBox ID="Name" runat="server" Text="" Width="150px" MaxLength="100"></asp:TextBox>
                        </td>                        
                    </tr>
                    <tr>
                        <th>
                            <asp:Label ID="FromEmailLabel" runat="server" Text="Your Email:" AssociatedControlID="FromEmail" CssClass="H2"></asp:Label>                            
                            <cb:EmailAddressValidator ID="FromEmailAddressValidator" runat="server" ControlToValidate="FromEmail" ValidationGroup="TellAFriend" Required="true" ErrorMessage="From email address should be in the format of name@domain.tld." Text="*" EnableViewState="False"></cb:EmailAddressValidator>
                        </th>
                    </tr>    
                    <tr>
                        <td align="center">
                            <asp:TextBox ID="FromEmail" runat="server" Text="" Width="150px" MaxLength="200"></asp:TextBox>
                        </td>                        
                    </tr>
                    <tr>
                        <th>
                            <asp:Label ID="FriendEmailLabel" runat="server" Text="Friend Email:" AssociatedControlID="FriendEmail" CssClass="H2"></asp:Label>
                            <cb:EmailAddressValidator ID="SendToEmailAddressValidator" runat="server" ControlToValidate="FriendEmail" ValidationGroup="TellAFriend" Required="true" ErrorMessage="Your friend email address should be in the format of name@domain.tld." Text="*" EnableViewState="False"></cb:EmailAddressValidator>                            
                        </th>
                    </tr>                
                    <tr>
                        <td align="center">
                            <asp:TextBox ID="FriendEmail" runat="server" Text="" Width="150px" MaxLength="200"></asp:TextBox>
                        </td>
                    </tr>
                    <tr id="trCaptchaCaption" runat="server" visible="false">
                        <th>
                            <asp:Label ID="CaptchaImageLabel" runat="server" Text="Verification Number:" AssociatedControlID="CaptchaImage" CssClass="H2" EnableViewState="False"></asp:Label><br />
                        </th>
                    </tr>
                    <tr id="trCaptchaImage" runat="server" visible="false">
                        <td>
                            <ajax:UpdatePanel ID="CaptchaPanel" runat="server">
                                <ContentTemplate>
                                    <div style="text-align:center">
                                        <cb:CaptchaImage ID="CaptchaImage" runat="server" Height="80px" Width="150px" EnableViewState="False" /><br />
                                        <asp:LinkButton ID="ChangeImageLink" runat="server" Text="different image" CausesValidation="false" EnableViewState="False"></asp:LinkButton>
                                    </div>
                                </ContentTemplate>
                            </ajax:UpdatePanel> 
                        </td>
                    </tr>
                    <tr id="trCaptchaInputLabel" runat="server" visible="false">
                        <th>
                            <asp:Label ID="CaptchaInputLabel" runat="server" Text="Enter Number:" AssociatedControlID="CaptchaInput" CssClass="H2" EnableViewState="False"></asp:Label><br />
                        </th>
                    </tr>
                    <tr id="trCaptchaInput" runat="server" visible="false">
                        <td>
                            <asp:TextBox ID="CaptchaInput" runat="server" Width="80px" EnableViewState="False"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="CaptchaRequired" runat="server" ControlToValidate="CaptchaInput"
                                ErrorMessage="You must enter the number in the image." ToolTip="You must enter the number in the image."
                                Display="Dynamic" ValidationGroup="Login" Text="*" EnableViewState="False"></asp:RequiredFieldValidator>
                            <asp:PlaceHolder ID="phCaptchaValidators" runat="server" EnableViewState="false"></asp:PlaceHolder>
                        </td>
                    </tr>
                    </tr>
                    <tr>                
                        <td>
                            <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="TellAFriend" />
                        </td>
                    </tr>
                    <tr>            
                        <td>
                            <asp:Label ID="SentMessage" runat="server" SkinID="GoodCondition" EnableViewState="false" Visible="false" Text="Email has been sent."></asp:Label>
                            <asp:Label ID="FailureMessage" runat="server" SkinID="ErrorCondition" EnableViewState="false" Visible="false" Text="Message not sent."></asp:Label>
                        </td>
                    </tr>
                    <tr>                
                        <td align="right">
                            <asp:Button ID="SendEmailButton" runat="server" Text="Send Email" OnClick="SendEmailButton_Click" ValidationGroup="TellAFriend"></asp:Button>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </ajax:UpdatePanel>
    </div>
</div>