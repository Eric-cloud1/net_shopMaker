<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AffiliateRegForm.ascx.cs" Inherits="ConLib_AffiliateRegForm" %>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
<%--
<conlib>
<summary>Dsiplays a form to register affiliates and edit registration information.</summary>
</conlib>
--%>
<div class="dialogSection">
    <div class="header">
        <h2><asp:Localize ID="Caption" runat="server" Text="Affiliate Registration"></asp:Localize></h2>
    </div>
    <div class="content nofooter">
        Registered affiliates can obtain credit for referring customers to the store.  MakerShop can automatically calculate the amount of commission earned for a particular month, based on the rates configured for the affiliate.  Registered affiliates can also view a report on their sales.  To register as an affiliate, fill out all required fields on the form below and submit.<br />
    <ajax:UpdatePanel ID="AffiliateRegAjax" runat="server">
    <ContentTemplate>                
         <table class="inputForm" cellpadding="0" cellspacing="0" align="center">
            <tr>
                <td colspan="2">                    
                    <asp:ValidationSummary ID="RegisterValidationSummary" runat="server" ValidationGroup="RegisterAffiliate" />                    
                </td>
            </tr>
            <tr>            
                <th class="rowHeader">
                    <asp:Label ID="NameLabel" runat="server" Text="Affiliate Name:" />
                </th>
                <td>
                    <asp:TextBox ID="Name" runat="server" MaxLength="100"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="NameRequired" runat="server" ControlToValidate="Name"
                        Display="Static" ErrorMessage="Affiliate name is required." Text="*"></asp:RequiredFieldValidator>
                </td>
            </tr>
             <tr>
                <th class="rowHeader">
                    <asp:Label ID="WebsiteUrlLabel" runat="server" Text="Website Url:" AssociatedControlID="WebsiteUrl"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="WebsiteUrl" runat="server" MaxLength="255"></asp:TextBox>
                </td>
            </tr>
            <tr>
               <th class="rowHeader">
                    <asp:Label ID="FirstNameLabel" runat="server" Text="First Name:" AssociatedControlID="FirstName"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="FirstName" runat="server" MaxLength="30"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <th class="rowHeader">
                    <asp:Label ID="LastNameLabel" runat="server" Text="Last Name:" AssociatedControlID="LastName"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="LastName" runat="server" MaxLength="50"></asp:TextBox>
                </td>
            </tr>
            
            <tr>
                <td align="center" colspan="2">
                    <asp:Label ID="FailureText" runat="server" EnableViewState="False" SkinID="ErrorCondition"></asp:Label>                    
                </td>
            </tr>
            <tr>
                 <th class="rowHeader" >
                    <asp:Label ID="CompanyLabel" runat="server" Text="Company:" AssociatedControlID="Company" EnableViewState="false"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="Company" runat="server" EnableViewState="false" MaxLength="50"></asp:TextBox> 
                </td>
            </tr>             
             <tr>
                 <th class="rowHeader">
                    <asp:Label ID="Address1Label" runat="server" Text="Street Address 1:" AssociatedControlID="Address1" EnableViewState="false"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="Address1" runat="server" EnableViewState="false" MaxLength="100"></asp:TextBox> 
                    <asp:RequiredFieldValidator ID="Address1Required" runat="server" Text="*"
                        ErrorMessage="Address is required." Display="Static" ControlToValidate="Address1" ValidationGroup="RegisterAffiliate"
                        EnableViewState="false"></asp:RequiredFieldValidator>
                </td>
             </tr>
             <tr>
                 <th class="rowHeader" >
                    <asp:Label ID="Address2Label" runat="server" Text="Street Address 2:" AssociatedControlID="Address2" EnableViewState="false"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="Address2" runat="server" EnableViewState="false" MaxLength="100"></asp:TextBox> 
                </td>
             </tr>
             <tr>
                 <th class="rowHeader">
                    <asp:Label ID="CityLabel" runat="server" Text="City:" AssociatedControlID="City" EnableViewState="false"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="City" runat="server" EnableViewState="false" MaxLength="30"></asp:TextBox> 
                    <asp:RequiredFieldValidator ID="CityRequired" runat="server" Text="*" ValidationGroup="RegisterAffiliate"
                        ErrorMessage="City is required." Display="Static" ControlToValidate="City"
                        EnableViewState="false"></asp:RequiredFieldValidator>
                </td>
             </tr>
             <tr>
                <th class="rowHeader" >
                    <asp:Label ID="ProvinceLabel" runat="server" Text="State / Province:" AssociatedControlID="Province" EnableViewState="false"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="Province" runat="server" MaxLength="50"></asp:TextBox> 
                    <asp:DropDownList ID="Province2" runat="server"></asp:DropDownList>
                    <asp:RequiredFieldValidator ID="Province2Required" runat="server" Text="*" ValidationGroup="RegisterAffiliate"
                        ErrorMessage="State or province is required." Display="Static" ControlToValidate="Province2"></asp:RequiredFieldValidator>
                </td>
             </tr>
             <tr>
                 <th class="rowHeader">
                     &nbsp;<asp:Label ID="PostalCodeLabel" runat="server" AssociatedControlID="PostalCode" Text="Zip/Postal Code:"></asp:Label></th>
                 <td align="left" >
                     <asp:TextBox ID="PostalCode" runat="server" EnableViewState="false" MaxLength="10"></asp:TextBox> 
                    <asp:RequiredFieldValidator ID="PostalCodeRequired" runat="server" Text="*" ValidationGroup="RegisterAffiliate"
                        ErrorMessage="ZIP or Postal Code is required." Display="Static" ControlToValidate="PostalCode"></asp:RequiredFieldValidator>
                 </td>
             </tr>
             <tr>
                 <th class="rowHeader">
                     <asp:Label ID="CountryLabel" runat="server" AssociatedControlID="Country" Text="Country:"></asp:Label>
                 </th>
                 <td align="left" >
                     <asp:DropDownList ID="Country" runat="server" DataTextField="Name" DataValueField="CountryCode" 
                        OnSelectedIndexChanged="Country_Changed" AutoPostBack="true" EnableViewState="false"></asp:DropDownList>                     
                 </td>
             </tr>
             <tr>
                 <th class="rowHeader">
                     <asp:Label ID="PhoneLabel" runat="server" AssociatedControlID="Phone" Text="Phone:"></asp:Label>
                 </th>
                 <td align="left" >
                     <asp:TextBox ID="Phone" runat="server" MaxLength="255" Width="80"></asp:TextBox>
                     <asp:RequiredFieldValidator ID="PhoneRequired" runat="server" ControlToValidate="Phone"
                         ErrorMessage="Phone number is required" Text="*" ValidationGroup="RegisterAffiliate"></asp:RequiredFieldValidator>
                 </td>
             </tr>
             <tr>
                <th class="rowHeader">
                    <asp:Label ID="MobileNumberLabel" runat="server" Text="Mobile Number:" AssociatedControlID="MobileNumber"></asp:Label>
                </th>
                <td>
                    <asp:TextBox ID="MobileNumber" runat="server" MaxLength="20"></asp:TextBox>
                </td>
             </tr>
             <tr>
                 <th class="rowHeader">
                     <asp:Label ID="FaxLabel" runat="server" AssociatedControlID="Fax" Text="Fax:"></asp:Label>
                 </th>
                 <td align="left" >
                     <asp:TextBox ID="Fax" runat="server" MaxLength="255" Width="80"></asp:TextBox></td>
             </tr>
             <tr>
                 <th class="rowHeader" >
                     <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email" Text="Email:"></asp:Label>
                 </th>
                 <td align="left"  >
                     <asp:TextBox ID="Email" runat="server" MaxLength="255" ></asp:TextBox>
                     <cb:EmailAddressValidator ID="EmailValidator" runat="server" ControlToValidate="Email" Required="true" ErrorMessage="Email address should be in the format of name@domain.tld." Text="*" ValidationGroup="RegisterAffiliate"></cb:EmailAddressValidator>
                 </td>
             </tr>
             <tr>                 
                 <td align="center" colspan="2" style="height: 24px">
                    <asp:Label ID="SavedMessage" runat="server" Text="Saved successfully." Visible="false"
                                        SkinID="GoodCondition" EnableViewState="False"></asp:Label><br />                                        
                     <asp:HyperLink ID="CancelButton" SkinID="Button" runat="server" Text="Cancel"  NavigateUrl="~/Members/MyAffiliateAccount.aspx" />&nbsp;
                     <asp:Button ID="SaveButton" runat="server" Text="Save"   ValidationGroup="RegisterAffiliate" OnClick="SaveButton_Click"/>&nbsp;
                 </td>
             </tr>
       </table>
        </ContentTemplate>        
       </ajax:UpdatePanel>    
    </div>   
</div>


