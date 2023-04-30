<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"
CodeFile="Maintenance.aspx.cs" Inherits="Admin_Store_Maintenance"
Title="Maintenance Settings" %> <%@ Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
  <div class="pageHeader">
    <div class="caption">
      <h1>
        <asp:Localize
          ID="Caption"
          runat="server"
          Text="Configure Store"
        ></asp:Localize>
      </h1>
    </div>
  </div>
  <table cellpadding="2" cellspacing="2" class="innerLayout">
    <tr>
      <td colspan="2">
        <asp:ValidationSummary ID="ValidationSummary2" runat="server" />
        <asp:Label
          ID="SavedMessage"
          runat="server"
          Text="The store settings have been saved."
          Visible="false"
          SkinID="GoodCondition"
        ></asp:Label>
      </td>
    </tr>
    <tr>
      <td align="left" valign="top" width="50%">
        <div class="section" style="padding: 0 0 2px 0">
          <div class="header">
            <h2 class="AnonymousUserMaintenance">
              <asp:Localize
                ID="UserMaintenanceCaption"
                runat="server"
                Text="Anonymous User Maintenance"
              ></asp:Localize>
            </h2>
          </div>
          <div class="content">
            <asp:Label
              ID="AnonymousLifespanLabel1"
              runat="server"
              Text="For how many days should anonymous users (and their abandoned baskets) be retained in the database? Reports on abandoned baskets are only valid when these records are available for the reporting period.  (blank = forever)"
            ></asp:Label
            ><br /><br />
            <asp:Label
              ID="AnonymousLifespanLabel2"
              runat="Server"
              SkinID="FieldHeader"
              Text="Days to Save: "
            ></asp:Label>
            <asp:TextBox
              ID="AnonymousLifespan"
              runat="server"
              Width="60px"
              MaxLength="4"
            ></asp:TextBox>
            <asp:RangeValidator
              ID="AnonymousLifespanValidator"
              runat="server"
              Type="Integer"
              MinimumValue="0"
              MaximumValue="99999"
              ControlToValidate="AnonymousLifespan"
              ErrorMessage="Anonymous Lifespan days must be a numaric value."
              Text="*"
            ></asp:RangeValidator
            ><br />
            <asp:Label
              ID="AnonymousAffiliateLifespanLabel1"
              runat="server"
              Text="If you use affiliates, how long should anonymous users be retained when they have an affiliate association?  Reports on user referrals and conversion rates are only valid when these records are available for the reporting period.  (blank = forever)"
            ></asp:Label
            ><br /><br />
            <asp:Label
              ID="AnonymousAffiliateLifespanLabel2"
              runat="Server"
              SkinID="FieldHeader"
              Text="Days to Save: "
            ></asp:Label>
            <asp:TextBox
              ID="AnonymousAffiliateLifespan"
              runat="server"
              Width="60px"
              MaxLength="4"
            ></asp:TextBox>
            <asp:RangeValidator
              ID="AnonymousAffiliateLifespanValidator"
              runat="server"
              Type="Integer"
              MinimumValue="0"
              MaximumValue="99999"
              ControlToValidate="AnonymousAffiliateLifespan"
              ErrorMessage="Anonymous Affiliate Lifespan days must be a numaric value."
              Text="*"
            ></asp:RangeValidator>
          </div>
        </div>
        <div class="section" style="padding: 0 0 2px 0">
          <div class="header">
            <h2 class="giftcertificateexpiry">
              <asp:Localize
                ID="GiftCertificateExpiryLabel"
                runat="server"
                Text="Gift Certificate Expiry"
              ></asp:Localize>
            </h2>
          </div>
          <div class="content">
            <asp:Label
              ID="GiftCertExpireDaysLbl1"
              runat="server"
              Text="How many days should it take before before a gift certificate expires - 0 means no expiration. (Expiration setting affects new gift certificates only)."
            ></asp:Label
            ><br /><br />
            <asp:Label
              ID="GiftCertExpireDaysLbl2"
              runat="Server"
              SkinID="FieldHeader"
              Text="Days to Gift Certificate Expiry: "
            ></asp:Label>
            <asp:TextBox
              ID="GiftCertExpireDays"
              runat="server"
              Width="60px"
              MaxLength="4"
            ></asp:TextBox>
            <asp:RangeValidator
              ID="GiftCertExpireDaysValidator"
              runat="server"
              Type="Integer"
              MinimumValue="0"
              MaximumValue="99999"
              ControlToValidate="GiftCertExpireDays"
              ErrorMessage="Gift Certificate expiry days must be a numaric value."
              Text="*"
            ></asp:RangeValidator>
          </div>
        </div>
      </td>
      <td valign="top" width="50%">
        <div class="section" style="padding: 0 0 2px 0">
          <div class="header">
            <h2 class="commonicon">
              <asp:Localize
                ID="StoreMaintenanceCaption"
                runat="server"
                Text="Store Maintenance"
              ></asp:Localize>
            </h2>
          </div>
          <div class="content">
            <table class="inputForm">
              <tr>
                <th class="rowHeader">
                  <cb:ToolTipLabel
                    ID="IsStoreClosedLabel"
                    runat="Server"
                    Text="Store Status:"
                    AssociatedControlID="StoreClosedOptions"
                    ToolTip="Use this setting to temporarily close the store front for your maintenance or testing purposes. This setting has no impact on the availability of the admin pages."
                  ></cb:ToolTipLabel>
                </th>
                <td>
                  <asp:DropDownList ID="StoreClosedOptions" runat="server">
                  </asp:DropDownList>
                </td>
              </tr>
            </table>
            <table class="inputForm">
              <tr>
                <th class="rowHeader" valign="top">
                  <cb:ToolTipLabel
                    ID="StoreClosedMessageLabel"
                    runat="server"
                    Text="Closed Message:"
                    ToolTip="If the storefront is temporarily closed, this message will be displayed to customers instead."
                  ></cb:ToolTipLabel
                  ><br />
                  <asp:RequiredFieldValidator
                    ID="StoreClosedMessageRequired"
                    runat="server"
                    Text="*"
                    ErrorMessage="Store close message is required."
                    ControlToValidate="StoreClosedMessage"
                  ></asp:RequiredFieldValidator>
                </th>
                <td>
                  <asp:TextBox
                    ID="StoreClosedMessage"
                    runat="server"
                    Rows="7"
                    TextMode="MultiLine"
                    Width="260px"
                    Columns="50"
                  ></asp:TextBox>
                </td>
              </tr>
            </table>
          </div>
        </div>
        <!--
				<div class="section" style="padding:0 0 2px 0;">
                    <div class="header">
                        <h2 class="commonicon"><asp:Localize ID="SubscriptionsMaintenance" runat="server" Text="Subscriptions Maintenance"></asp:Localize></h2>
                    </div>
                    <div class="content">
                        <asp:Label ID="SubscriptionsMaintenanceLbl1" runat="server" Text="How many days after which expired subscriptions should be deleted?"></asp:Label><br /><br />
                        <asp:Label ID="SubscriptionsMaintenanceLbl2" runat="Server" SkinID="FieldHeader" Text="Days Before Deleting Expired Subscriptions : "></asp:Label>
                        <asp:TextBox ID="MaintenanceDays" runat="server" Width="60px" MaxLength="4"></asp:TextBox>
                        <asp:RangeValidator ID="MaintenanceDaysValidator" runat="server" Type="Integer" MinimumValue="0" MaximumValue="99999" ControlToValidate="MaintenanceDays" ErrorMessage="Maintenance expiry days must be a numaric value." Text="*"></asp:RangeValidator>
                    </div>
                </div>
                -->
      </td>
    </tr>
    <tr>
      <td align="center" colspan="2">
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
        <br />
        <asp:Button
          Id="SaveButon"
          runat="server"
          Text="Save"
          OnClick="SaveButton_Click"
          CssClass="button"
        />
        <asp:Button
          Id="SaveAndCloseButton"
          runat="server"
          Text="Save and Close"
          OnClick="SaveAndCloseButton_Click"
          CssClass="button"
        />
        <asp:Button
          Id="CancelButton"
          runat="server"
          Text="Cancel"
          OnClick="CancelButton_Click"
          CssClass="button"
        />
      </td>
    </tr>
  </table>
</asp:Content>
