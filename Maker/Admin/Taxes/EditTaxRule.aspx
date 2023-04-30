<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"
AutoEventWireup="true" CodeFile="EditTaxRule.aspx.cs"
Inherits="Admin_Taxes_EditTaxRule" Title="Edit Tax Rule" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb"
%>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
  <ajax:UpdatePanel ID="TaxRuleAjax" runat="server">
    <ContentTemplate>
      <div class="pageHeader">
        <div class="caption">
          <h1>
            <asp:Localize
              ID="Caption"
              runat="server"
              Text="Edit Tax Rule {0}"
              EnableViewState="false"
            ></asp:Localize>
          </h1>
        </div>
      </div>
      <table class="inputForm" cellpadding="4" cellspacing="0" width="100%">
        <tr>
          <th align="left" class="sectionHeader" colspan="2">Basic Settings</th>
        </tr>
        <tr>
          <th class="rowHeader" width="150px">
            <cb:ToolTipLabel
              id="NameLabel"
              runat="server"
              Text="Tax Name:"
              AssociatedControlID="Name"
              ToolTip="Name of the tax as it will appear in the admin and on the invoice."
            ></cb:ToolTipLabel
            ><br />
          </th>
          <td>
            <asp:TextBox
              ID="Name"
              runat="server"
              Text=""
              Width="200px"
              MaxLength="100"
            ></asp:TextBox>
            <asp:RequiredFieldValidator
              ID="NameRequired"
              runat="server"
              Text="*"
              Display="Static"
              ErrorMessage="Name is required."
              ControlToValidate="Name"
            ></asp:RequiredFieldValidator>
            <asp:PlaceHolder
              ID="phNameValidator"
              runat="server"
              EnableViewState="false"
            ></asp:PlaceHolder>
          </td>
        </tr>
        <tr>
          <th class="rowHeader">
            <cb:ToolTipLabel
              ID="TaxCodesLabel"
              runat="server"
              Text="Apply to Tax Codes:"
              AssociatedControlID="TaxCodes"
              ToolTip="Products can be assigned tax codes.  Select all tax codes that this tax should apply to."
            ></cb:ToolTipLabel
            ><br />
          </th>
          <td>
            <asp:CheckBoxList
              ID="TaxCodes"
              runat="server"
              DataTextField="Name"
              DataValueField="TaxCodeId"
              RepeatColumns="2"
            ></asp:CheckBoxList>
          </td>
        </tr>
        <tr>
          <th class="rowHeader">
            <cb:ToolTipLabel
              ID="TaxRateLabel"
              runat="server"
              Text="Tax Rate:"
              AssociatedControlID="TaxRate"
              ToolTip="The tax rate to apply.  Enter as a percent (15) rather than as a decimal (0.15)."
            ></cb:ToolTipLabel>
          </th>
          <td>
            <asp:TextBox
              ID="TaxRate"
              runat="server"
              Text=""
              Columns="4"
              MaxLength="8"
            ></asp:TextBox>
            %
            <asp:RequiredFieldValidator
              ID="TaxRateRequired"
              runat="server"
              Text="*"
              Display="Static"
              ErrorMessage="Tax rate is required."
              ControlToValidate="TaxRate"
            ></asp:RequiredFieldValidator>
            <asp:RangeValidator
              ID="TaxRateValidator"
              runat="server"
              ControlToValidate="TaxRate"
              ErrorMessage="TaxRate should fall between 0.00 and 1000.00"
              MaximumValue="1000.00"
              MinimumValue="0"
              Type="Double"
              >*</asp:RangeValidator
            >
          </td>
        </tr>
        <tr>
          <th class="rowHeader" valign="top">
            <cb:ToolTipLabel
              ID="RoundingRuleLabel"
              runat="server"
              Text="Rounding Rule:"
              ToolTip="Rounding method that should be used for calculating this tax.  In the common method, we round up on 5 so 0.105 becomes 0.11.  In round to even, we round to the nearest even number on 5 so 0.105 becomes 0.10 while 0.115 becomes 0.12.  In always round up, we round up for any fractional value so 0.10001 becomes 0.11."
            />
          </th>
          <td>
            <asp:DropDownList ID="RoundingRule" runat="server">
            </asp:DropDownList>
          </td>
        </tr>
        <tr>
          <th class="rowHeader" valign="top">
            <cb:ToolTipLabel
              ID="ZonesLabel"
              runat="server"
              Text="Address&nbsp;Filter:"
              ToolTip="Use an address filter to make this tax only apply to certain regions."
            />
          </th>
          <td>
            <asp:DropDownList ID="ZoneRule" runat="server" AutoPostBack="True">
              <asp:ListItem Text="No Address Filter" Value="0"></asp:ListItem>
              <asp:ListItem
                Text="Only Apply to Selected Zones"
                Value="1"
              ></asp:ListItem>
            </asp:DropDownList>
          </td>
        </tr>
        <tr id="trZoneList" runat="server">
          <td>&nbsp;</td>
          <td>
            <asp:CheckBoxList
              ID="ZoneList"
              runat="server"
              RepeatColumns="2"
              DataTextField="Name"
              DataValueField="ShipZoneId"
            ></asp:CheckBoxList>
          </td>
        </tr>
        <tr id="trAddressNexus" runat="server">
          <th class="rowHeader">
            <cb:ToolTipLabel
              ID="UseBillingAddressLabel"
              runat="server"
              Text="Address&nbsp;Nexus:"
              AssociatedControlID="UseBillingAddress"
              ToolTip="Use an address filter to make a tax that only applies to certain regioins.address that should be used in conjunction with the address filter to determine if the tax should apply."
            ></cb:ToolTipLabel>
          </th>
          <td>
            <asp:DropDownList ID="UseBillingAddress" runat="server">
              <asp:ListItem
                Value="0"
                Text="Customer Shipping Address"
                Selected="true"
              ></asp:ListItem>
              <asp:ListItem
                Value="1"
                Text="Customer Billing Address"
              ></asp:ListItem>
            </asp:DropDownList>
          </td>
        </tr>
        <tr>
          <th class="rowHeader" valign="top">
            <cb:ToolTipLabel
              ID="GroupRuleLabel"
              runat="server"
              Text="Group Filter:"
              ToolTip="Indicate whether or how this tax should apply to the selected groups."
            />
          </th>
          <td>
            <asp:DropDownList ID="GroupRule" runat="server" AutoPostBack="True">
              <asp:ListItem Text="No Group Filter" Value="0"></asp:ListItem>
              <asp:ListItem
                Text="Only Apply to Selected Groups"
                Value="1"
              ></asp:ListItem>
              <asp:ListItem
                Text="Do Not Apply to Selected Groups"
                Value="2"
              ></asp:ListItem>
            </asp:DropDownList>
          </td>
        </tr>
        <tr id="trGroupList" runat="server">
          <td>&nbsp;</td>
          <td>
            <asp:CheckBoxList
              ID="GroupList"
              runat="server"
              DataTextField="Name"
              DataValueField="GroupId"
              RepeatColumns="3"
            ></asp:CheckBoxList>
          </td>
        </tr>
        <tr>
          <th class="rowHeader" valign="top">
            <cb:ToolTipLabel
              ID="PerUnitCalculationLabel"
              runat="server"
              Text="Per Unit Calculation:"
              ToolTip="Indicate whether tax is calculated individually for each unit purchased. Merchants showing prices inclusive of VAT probably want to turn this on so that order totals match the expected value. US based merchants collecting sales tax probably do not want this option."
            />
          </th>
          <td>
            <asp:CheckBox ID="PerUnitCalculation" runat="server" />
          </td>
        </tr>
        <tr>
          <th align="left" class="sectionHeader" colspan="2">
            Compounding Settings (to charge tax on tax)
          </th>
        </tr>
        <tr>
          <th class="rowHeader" valign="top">
            <cb:ToolTipLabel
              ID="TaxCodeLabel"
              runat="server"
              Text="My Tax Code:"
              ToolTip='If this tax is "taxable", select the tax code that should be assigned to any generated line items in the order.'
            />
          </th>
          <td>
            <asp:DropDownList
              ID="TaxCode"
              runat="server"
              DataTextField="Name"
              DataValueField="TaxCodeId"
              AppendDataBoundItems="true"
            >
            </asp:DropDownList>
          </td>
        </tr>
        <tr>
          <th class="rowHeader" valign="top">
            <cb:ToolTipLabel
              ID="PriorityLabel"
              runat="server"
              Text="Priority:"
              ToolTip="Indicate the priority for this rule, lower priority tax rules are processed first."
            />
          </th>
          <td>
            <asp:TextBox
              ID="Priority"
              runat="server"
              Width="50px"
              MaxLength="3"
              Text="0"
            ></asp:TextBox>
            <asp:RangeValidator
              ID="PriorityValidator"
              runat="server"
              ControlToValidate="Priority"
              Text="*"
              ErrorMessage="Priority must be between 0 and 100"
              MaximumValue="100"
              MinimumValue="0"
              Type="Integer"
            ></asp:RangeValidator>
          </td>
        </tr>
        <tr>
          <td>&nbsp;</td>
          <td class="dialogSubmit">
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
            <asp:LinkButton
              ID="SaveButton"
              runat="server"
              OnClick="SaveButton_Click"
              SkinID="Button"
              Text="Save"
            ></asp:LinkButton
            >&nbsp;
            <asp:HyperLink
              ID="CancelLink"
              runat="server"
              SkinID="Button"
              Text="Cancel"
              NavigateUrl="TaxRules.aspx"
            ></asp:HyperLink>
          </td>
        </tr>
      </table>
    </ContentTemplate>
  </ajax:UpdatePanel>
</asp:Content>
