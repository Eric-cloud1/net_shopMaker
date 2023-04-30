<%@ Page Language="C#" MasterPageFile="~/Admin/Orders/Order.master" AutoEventWireup="true"
    CodeFile="PaymentChargeBack.aspx.cs" Inherits="Admin_Reports_ChargeBack_PaymentChargeBack"
    Title="Untitled Page" %>

<%@ Register Src="../../../UserControls/PickerAndCalendar.ascx" TagName="PickerAndCalendar"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PrimarySidebarContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional" />
    <contenttemplate>
 
             <div class="pageHeader">
                <div class="caption">
                    <h1><asp:Localize ID="Caption" runat="server" Text="Chargeback "></asp:Localize> </h1>
                </div>
                </div>
<table align="left" style="width:500px" cellpadding="0" cellspacing="0" border="0">
    <tr><td style="width:10%;">&nbsp;</td><td colspan="2"> <asp:Label ID="lCreatedDate" runat="server" Visible ="false"  SkinID="FieldHeader"/></td></tr>
    <tr><td colspan="3">&nbsp;</td></tr>
    <tr ><td style="width:10%;">&nbsp;</td>
        <td style="width:30%">
            <asp:Label ID="lbChargeBackDate" runat="server" Text="Charge Back Initiated: " SkinID="FieldHeader"></asp:Label></td><td>                
              <uc1:PickerAndCalendar ID="ChargeBackDate"  runat="server" />                   
                <br />
      </td> </tr>
      
    <tr><td style="width:10%;">&nbsp;</td><td colspan="2">&nbsp;</td></tr>
    
    <tr><td style="width:10%;">&nbsp;</td><td style="width:30%"><asp:Label ID="lbChargebackStatus" runat="server" Text="Chargeback Status:" SkinID="FieldHeader"/></td><td>
    <asp:DropDownList ID="dlChargebackStatus" runat="server"></asp:DropDownList></td></tr>

    <tr><td style="width:10%;">&nbsp;</td><td colspan="2">&nbsp;</td></tr>
          <tr><td style="width:10%;">&nbsp;</td><td>Provider Id:</td><td>
    <asp:DropDownList ID="dlProviderTransactionId" AutoPostBack="true" OnSelectedIndexChanged="Select_Provider" runat="server"></asp:DropDownList></td></tr>

      <tr><td style="width:10%;">&nbsp;</td><td style="width:30%">Reason Code:</td><td style="width:60%"><asp:TextBox ID="chargeBackReasonCode" MaxLength="10" Width="100%" runat="server" /></td></tr>
     <tr><td style="width:10%;">&nbsp;</td><td style="width:30%">Reference #:</td><td style="width:60%"><asp:TextBox ID="chargeBackReference" Width="100%" runat="server" /></td></tr>
     <tr><td style="width:10%;">&nbsp;</td><td style="width:30%">Description:</td><td style="width:60%"><asp:TextBox ID="chargeBackDescription" Width="100%" runat="server" /></td></tr>
     <tr><td style="width:10%;">&nbsp;</td><td style="width:30%">Case Number:</td><td style="width:60%"><asp:TextBox ID="chargeBackCaseNumber" Width="100%" runat="server" /></td></tr>

     <tr><td style="width:10%;">&nbsp;</td><td colspan="2">&nbsp;</td></tr>
     <tr><td style="width:10%;">&nbsp;</td><td colspan="2" class="sectionHeader" style="text-align: middle; width:100%">Add Chargeback Comment</td></tr>
     <tr><td style="width:10%;">&nbsp;</td><td colspan="2"><asp:TextBox ID="chargeBackComment" Height="50" rows="100" Width="99%" runat="server" cols="5"></asp:TextBox></td></tr>
   
    <tr><td colspan="3">&nbsp;</td></tr>
     <tr><td colspan="3" style="text-align:center">
     <asp:Button ID="saveChargeBack" Text="Save" runat="server" CssClass="button" 
             onclick="saveChargeBack_Click"  />&nbsp;&nbsp;  
     <asp:Button ID="cancelChargeBack" Text="Cancel" runat="server" CssClass="button" 
             onclick="cancelChargeBack_Click" />  
     </td></tr>

</table>
</contenttemplate>
</asp:Content>
