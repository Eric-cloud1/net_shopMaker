<%@ Page Language="C#" MasterPageFile="Affiliate.master" AutoEventWireup="true" CodeFile="EditAffiliate.aspx.cs" Inherits="Admin_Marketing_Affiliates_EditAffiliate" Title="Edit Affiliate" %>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageHeader">
        <div class="caption">
            <h1>
                <asp:Localize ID="Caption" runat="server" Text="Edit {0}" EnableViewState="false"></asp:Localize></h1>
        </div>
    </div>
  <ajax:UpdatePanel ID="EditAjax" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>


    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td colspan="2" align="center">
                <asp:Label ID="InstructionText" runat="server" Visible="false" Text="To associate a link with this affiliate, add <b>afid={0}</b> to the url. For example:<br />{1}?afid={0}<br /><br />"></asp:Label>
            </td>
        </tr>
        <tr>
            <td valign="top">
              
                        <table class="inputForm" width="900">
                            <tr>
                                <td class="validation" colspan="2">
                                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
                                    <asp:Label ID="SavedMessage" runat="server" Text="Saved at {0:t}" Visible="false"
                                        SkinID="GoodCondition" EnableViewState="False"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <cb:ToolTipLabel ID="NameLabel" runat="server" Text="Affiliate Name:" ToolTip="The name of the affiliate as it will appear on reports and in the merchant admin." />
                                </th>
                                <td>
                                    <asp:TextBox ID="Name" runat="server" MaxLength="100"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="NameRequired" runat="server" ControlToValidate="Name"
                                        Display="Static" ErrorMessage="Affiliate name is required." Text="*"></asp:RequiredFieldValidator>
                                </td>
                                <td colspan="2"><asp:PlaceHolder ID="phAffiliateLogo" runat="server"></asp:PlaceHolder></td>
                               
                            </tr>

                            <tr> 
                            <th class="rowHeader">
                                    <cb:ToolTipLabel ID="AffiliateTypeLabel" runat="server" Text="Affiliate Type:" ToolTip="Enter affiliate type." />
                                </th>
                             <td >
                                    <asp:DropDownList ID="AffiliateTypeDropdownList" runat="server">
                            
                                    </asp:DropDownList>
                                </td>
                                 <th class="rowHeader">
                                    <cb:ToolTipLabel ID="PromoCodeLabel" runat="server" Text="Promo Code:" ToolTip="Affiliate Promo Code(s)." />
                                </th>
                                <td>
                                    <asp:TextBox ID="PromoCode" runat="server" Width="60px" MaxLength="4"></asp:TextBox>
                                </td>
                                
                                </tr>

                           

                                <tr>
                                <th class="rowHeader">
                                    <cb:ToolTipLabel ID="ToolTipLabel1" runat="server" Text="Active:" ToolTip="Are leads still expected from this Affiliate?" />
                                </th>
                                <td>
<asp:CheckBox runat="server" ID="Active" />
                                </td>
                                <th class="rowHeader">
                                    <cb:ToolTipLabel ID="ReferralDaysLabel" runat="server" Text="Referral Period:" ToolTip="Time length of time (in days) that the affiliate will get credit for a sale made by a referred customer.  Leave blank to have an unlimited timeframe for affiliate orders." />
                                </th>
                                <td>
                                    <asp:TextBox ID="ReferralDays" runat="server" Width="60px" MaxLength="4"></asp:TextBox>
                                    <asp:Localize ID="ReferralDaysLabel2" runat="server" Text=" days"></asp:Localize>
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <cb:ToolTipLabel ID="CommissionRateLabel" runat="server" Text="Commission Rate:"
                                        ToolTip="The rate used for the calculation of commission - either a dollar amount or a percentage." />
                                </th>
                                <td>
                                    <asp:TextBox ID="CommissionRate" runat="server" Width="60px" MaxLength="8"></asp:TextBox>
                                    <asp:RegularExpressionValidator ID="CommissionRateValidator" runat="server" Display="Static"
                                        ErrorMessage="Commission rate is invalid." Text="*" ControlToValidate="CommissionRate"
                                        ValidationExpression="\d{0,4}(\.\d{0,3})?"></asp:RegularExpressionValidator>
                                </td>
                                <th class="rowHeader">
                                    <cb:ToolTipLabel ID="CommissionTypeLabel" runat="server" Text="Commission Type:"
                                        ToolTip="Indicates the way the commission should be calculated.  Flat rate pays a fixed amount for each order.  Percentage of products subtotal calculates on the order total less taxes, shipping, etc.  Percentage of order total calculates on the order total including taxes and shipping." />
                                </th>
                                <td>
                                    <asp:DropDownList ID="CommissionType" runat="server">
                                        <asp:ListItem Text="Flat rate"></asp:ListItem>
                                        <asp:ListItem Text="% of product subtotal"></asp:ListItem>
                                        <asp:ListItem Text="% of order total"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <asp:Label ID="WebsiteUrlLabel" runat="server" Text="Website Url:" AssociatedControlID="WebsiteUrl"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="WebsiteUrl" runat="server" MaxLength="255"></asp:TextBox>
                                </td>
                                <th class="rowHeader">
                                    <asp:Label ID="EmailLabel" runat="server" Text="Email:" AssociatedControlID="Email"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="Email" runat="server" MaxLength="255"></asp:TextBox>
                                    <cb:EmailAddressValidator ID="FromEmailAddressValidator" runat="server" ControlToValidate="Email" Required="false" ErrorMessage="Email address should be in the format of name@domain.tld." Text="*" EnableViewState="False"></cb:EmailAddressValidator>
                                </td>
                            </tr>
                            
                               <tr>
                                <th class="rowHeader">
                                    <asp:Label ID="ParentAffiliateLabel" runat="server" Text="Parent Affiliate:" AssociatedControlID="ParentAffiliate"></asp:Label>
                                </th>
                                <td colspan="3">
                                <asp:DropDownList ID="ParentAffiliate" runat="server" ></asp:DropDownList>
                                   
                                </td>
                              
                             
                            </tr>
                            
                            
                            <tr class="sectionHeader">
                                <th colspan="4">
                                    <asp:Label ID="AddressCaption" runat="server" Text="Address Information"></asp:Label>
                                </th>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <asp:Label ID="FirstNameLabel" runat="server" Text="First Name:" AssociatedControlID="FirstName"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="FirstName" runat="server" MaxLength="30"></asp:TextBox>
                                </td>
                                <th class="rowHeader">
                                    <asp:Label ID="LastNameLabel" runat="server" Text="Last Name:" AssociatedControlID="LastName"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="LastName" runat="server" MaxLength="50"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <asp:Label ID="CompanyLabel" runat="server" Text="Company:" AssociatedControlID="Company"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="Company" runat="server" MaxLength="50"></asp:TextBox>
                                </td>
                                <th class="rowHeader">
                                    <asp:Label ID="Address1Label" runat="server" Text="Street Address 1:" AssociatedControlID="Address1"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="Address1" runat="server" MaxLength="100"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <asp:Label ID="Address2Label" runat="server" Text="Street Address 2:" AssociatedControlID="Address2"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="Address2" runat="server" MaxLength="100"></asp:TextBox>
                                </td>
                                <th class="rowHeader">
                                    <asp:Label ID="CityLabel" runat="server" Text="City:" AssociatedControlID="City"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="City" runat="server" MaxLength="50"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <asp:Label ID="ProvinceLabel" runat="server" Text="State / Province:" AssociatedControlID="Province"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="Province" runat="server" MaxLength="50"></asp:TextBox>
                                </td>
                                <th class="rowHeader">
                                    <asp:Label ID="PostalCodeLabel" runat="server" Text="ZIP / Postal Code:" AssociatedControlID="PostalCode"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="PostalCode" runat="server" MaxLength="15"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <asp:Label ID="CountryCodeLabel" runat="server" Text="Country:" AssociatedControlID="CountryCode"></asp:Label>
                                </th>
                                <td>
                                    <asp:DropDownList ID="CountryCode" runat="server" DataTextField="Name" DataValueField="CountryCode">
                                    </asp:DropDownList>
                                </td>
                                <th class="rowHeader" valign="top">
                                    <asp:Label ID="PhoneNumberLabel" runat="server" Text="Phone:" AssociatedControlID="PhoneNumber"></asp:Label>
                                </th>
                                <td colspan="3">
                                    <asp:TextBox ID="PhoneNumber" runat="server" MaxLength="50"></asp:TextBox><br />
                                </td>
                            </tr>
                            <tr>
                                <th class="rowHeader">
                                    <asp:Label ID="FaxNumberLabel" runat="server" Text="Fax Number:" AssociatedControlID="FaxNumber"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="FaxNumber" runat="server" MaxLength="20"></asp:TextBox>
                                </td>
                                <th class="rowHeader">
                                    <asp:Label ID="MobileNumberLabel" runat="server" Text="Mobile Number:" AssociatedControlID="MobileNumber"></asp:Label>
                                </th>
                                <td>
                                    <asp:TextBox ID="MobileNumber" runat="server" MaxLength="20"></asp:TextBox>
                                </td>
                            </tr>

                             <tr class="sectionHeader">
        <td colspan="4">
            BANK INFORMATION
        </td>
    </tr>
    <tr>
      <th class="rowHeader" nowrap>
            <cb:ToolTipLabel ID="RoutingLabel" runat="server" Text="Routing #:" ToolTip="Bank routing number"></cb:ToolTipLabel>
        </th>
        <td>
         <asp:TextBox ID="BankRoutingNumber" runat="server" width="50px" MaxLength="25"  EnableViewState="false"></asp:TextBox>
       
        </td>
      <th class="rowHeader" nowrap>
            <cb:ToolTipLabel ID="AccountLabel" runat="server" Text="Account #:" ToolTip="Bank Account Number."></cb:ToolTipLabel>
        </th>
      <td>
   
        <asp:TextBox ID="BankAccountNumber" runat="server" width="50px" MaxLength="25"  EnableViewState="false"></asp:TextBox>
 
      </td>
    </tr>
    <tr><td colspan="2"><asp:HyperLink ID="AddLink" runat="server" Text="Add new Banking Information" Visible="false" NavigateUrl="#" EnableViewState="false"/></td></tr>
    <asp:Panel ID="bankInformationPanel" runat="server">
      <tr >
      <th class="rowHeader" nowrap>
            <cb:ToolTipLabel ID="BankInformationLabel" runat="server" Text="Bank Information:"  ToolTip="Bank information and address"></cb:ToolTipLabel>
        </th>
        <td colspan=2>
        <cb:ToolTipLabel ID="BankInformation" runat="server" Text="Routing #:"  ></cb:ToolTipLabel>
        </td>

    </tr>
    </asp:Panel>
                            <tr>
                                <td class="submit" colspan="4">                                    
                                    <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
									<asp:Button ID="CancelButton" runat="server" Text="Close" OnClick="CancelButton_Click" CausesValidation="false" />
                                </td>
                            </tr>
           </table>
      

       
   
                <div class="section">
                    <div class="header">
                        <h2><asp:Localize ID="OrdersCaption" runat="server" Text="Associated Orders"></asp:Localize></h2>
                    </div>
                    <div class="content">
      
                        <cb:SortedGridView ID="OrdersGrid" runat="server" DataSourceID="OrdersDs" AllowPaging="True"
                            AllowSorting="True" AutoGenerateColumns="False" DataKeyNames="OrderId" PageSize="20"
                            SkinID="PagedList" DefaultSortExpression="OrderDate" DefaultSortDirection="Descending"
                            Width="100%">
                            <Columns>
                                <asp:TemplateField HeaderText="Order #" SortExpression="OrderId">
                                    <headerstyle horizontalalign="Center" />
                                    <itemstyle horizontalalign="Center" />
                                    <itemtemplate>
                                        <asp:HyperLink ID="OrderNumber" runat="server" Text='<%# Eval("OrderNumber") %>' NavigateUrl='<%#String.Format("../../Orders/ViewOrder.aspx?OrderNumber={0}&OrderId={1}", Eval("OrderNumber"), Eval("OrderId")) %>' SkinId="Link"></asp:HyperLink>
                                    </itemtemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Date" SortExpression="OrderDate">
                                    <headerstyle horizontalalign="Center" />
                                    <itemstyle horizontalalign="Center" />
                                    <itemtemplate>
                                        <asp:Label ID="OrderDate" runat="server" Text='<%# Eval("OrderDate", "{0:d}") %>'></asp:Label>
                                    </itemtemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Subtotal" SortExpression="ProductSubtotal">
                                    <headerstyle horizontalalign="Center" />
                                    <itemstyle horizontalalign="Center" />
                                    <itemtemplate>
                                        <asp:Label ID="Subtotal" runat="server" Text='<%# Eval("ProductSubtotal", "{0:lc}") %>'></asp:Label>
                                    </itemtemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Total" SortExpression="TotalCharges">
                                    <headerstyle horizontalalign="Center" />
                                    <itemstyle horizontalalign="Center" />
                                    <itemtemplate>
                                        <asp:Label ID="Total" runat="server" Text='<%# Eval("TotalCharges", "{0:lc}") %>'></asp:Label>
                                    </itemtemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <asp:Label ID="EmptyMessage" runat="server" Text="There are no orders associated with this affiliate."></asp:Label>
                            </EmptyDataTemplate>
                        </cb:SortedGridView>
                  
                    </div>
                </div>
   <ajax:ModalPopupExtender ID="AddBankingInformation" runat="server" 
        TargetControlID="AddLink"
        PopupControlID="AddDialog" 
        BackgroundCssClass="modalBackground"                         
        DropShadow="true"
        PopupDragHandleControlID="AddDialogHeader" />
         </ContentTemplate>
     </ajax:UpdatePanel>

    <asp:Panel ID="AddDialog" runat="server" Style="display:none;width:400px" CssClass="modalPopup">
        <asp:Panel ID="AddDialogHeader" runat="server" CssClass="modalPopupHeader">
            ADD BANKING INFORMATION
        </asp:Panel>
        <div style="padding-top:5px;">
            <table class="inputForm" cellpadding="3">
            <tr>
            <th class="rowHeader" valign="top" nowrap>
            <cb:ToolTipLabel ID="BankNameLabel" runat="server" Text="Bank Name:" ToolTip="Enter the bank data."></cb:ToolTipLabel>
            </th>
            <td colspan="3" >
                <asp:TextBox ID="BankName" runat="server" Text=""   Columns="40"></asp:TextBox>
            </td
        </tr>

         <tr>
            <th class="rowHeader" valign="top" nowrap>
            <cb:ToolTipLabel ID="BankAddressLabel" runat="server" Text="Address:" ToolTip="Enter the bank city address."></cb:ToolTipLabel>
            </th>
            <td colspan="3">
                <asp:TextBox ID="BankAddress" runat="server"   Columns="40"></asp:TextBox>
            </td>
         </tr>

         <tr>
            <th class="rowHeader" valign="top" nowrap>
            <cb:ToolTipLabel ID="ToolTipLabel2" runat="server" Text="City:" ToolTip="Enter the bank state."></cb:ToolTipLabel>
            </th>
            <td colspan="3">
                <asp:TextBox ID="BankCity" runat="server"  Columns="20"></asp:TextBox>
            </td>
        
        </tr>

            <tr>
            <th class="rowHeader" valign="top" nowrap>
            <cb:ToolTipLabel ID="StateLabel" runat="server" Text="State:" ToolTip="Enter the bank city address."></cb:ToolTipLabel>
            </th>
            <td >
                <asp:TextBox ID="State" runat="server"   Columns="10"></asp:TextBox>
            </td>

            <th class="rowHeader" valign="top" nowrap>
            <cb:ToolTipLabel ID="ZipCodeLabel" runat="server" Text="Zip:" ToolTip="Enter the bank state."></cb:ToolTipLabel>
            </th>
            <td >
                <asp:TextBox ID="ZipCode" runat="server"   Columns="10"></asp:TextBox>
            </td>
        
        </tr>

          <tr>
            <th class="rowHeader" valign="top" nowrap>
            <cb:ToolTipLabel ID="PhoneLabel" runat="server" Text="Phone:" ToolTip="Enter the bank phone."></cb:ToolTipLabel>
            </th>
            <td colspan="3" >
                <asp:TextBox ID="Phone" runat="server"  Columns="5"></asp:TextBox>
                <asp:TextBox ID="Prefix" runat="server"   Columns="5"></asp:TextBox>
                <asp:TextBox ID="Suffix" runat="server"   Columns="5"></asp:TextBox>
            </td>

        
        </tr>
             <tr>
                    <td>&nbsp;</td>
                    <td colspan="2">
                        <asp:Button ID="AddButton" runat="server" Text="Save" OnClick="AddButton_Click" ValidationGroup="Add" />
                        <asp:Button ID="CancelAddButton" runat="server" Text="Cancel" CausesValidation="false" /><br />
                    </td>
                </tr>

            </table>
         </div>

          
   </asp:Panel>

    
    


    <asp:ObjectDataSource ID="OrdersDs" runat="server" EnablePaging="True" OldValuesParameterFormatString="original_{0}"
        SelectCountMethod="CountForAffiliate" SelectMethod="LoadForAffiliate" SortParameterName="sortExpression"
        TypeName="MakerShop.Orders.OrderDataSource">
        <SelectParameters>
            <asp:QueryStringParameter Name="affiliateId" QueryStringField="AffiliateId" Type="Object" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>
