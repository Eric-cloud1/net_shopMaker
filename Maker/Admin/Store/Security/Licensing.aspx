<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"
CodeFile="Licensing.aspx.cs" Inherits="Admin_Store_Security_Licensing"
Title="MakerShop License" %> <%@ Import Namespace="System.IO" %> <%@ Import
Namespace="System.Net" %> <%@ Import Namespace="MakerShop.Configuration" %> <%@
Import Namespace="System.Collections.Generic" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
  <div class="pageHeader">
    <div class="caption">
      <h1>
        <asp:Localize
          ID="Caption"
          runat="server"
          Text="Store License"
        ></asp:Localize>
      </h1>
    </div>
  </div>
  <table cellpadding="2" cellspacing="2" class="innerLayout">
    <tr>
      <td colspan="2">
        <asp:Label
          ID="SavedMessage"
          runat="server"
          Text="Store license updated at {0}."
          Visible="false"
          SkinID="GoodCondition"
          EnableViewState="false"
        ></asp:Label>
        <asp:Label
          ID="FailedMessage"
          runat="server"
          Text="The store license could not be updated: {0}"
          Visible="false"
          SkinID="ErrorCondition"
          EnableViewState="false"
        ></asp:Label>
      </td>
    </tr>
    <tr>
      <td align="left" valign="top" width="50%">
        <div class="section" style="padding: 0 0 2px 0">
          <div class="header">
            <h2 class="commonicon">
              <asp:Localize
                ID="LicenseCaption"
                runat="server"
                Text="License Details"
              ></asp:Localize>
            </h2>
          </div>
          <div class="content">
            <table class="inputForm">
              <tr>
                <th align="right">
                  <asp:Localize
                    ID="LicenseTypeLabel"
                    runat="server"
                    Text="License Type:"
                  ></asp:Localize>
                </th>
                <td>
                  <asp:Literal
                    ID="LicenseType"
                    runat="server"
                    Text=""
                    EnableViewState="false"
                  ></asp:Literal>
                </td>
              </tr>
              <tr>
                <th align="right">
                  <asp:Localize
                    ID="SubscriptionDateLabel"
                    runat="server"
                    Text="Subscription:"
                  ></asp:Localize>
                </th>
                <td>
                  <asp:Literal
                    ID="SubscriptionDate"
                    runat="server"
                    Text=""
                    EnableViewState="false"
                  ></asp:Literal>
                </td>
              </tr>
              <tr
                id="trExpiration"
                runat="server"
                visible="false"
                enableviewstate="false"
              >
                <th align="right">
                  <asp:Localize
                    ID="ExpirationLabel"
                    runat="server"
                    Text="Expires:"
                    EnableViewState="False"
                  ></asp:Localize>
                </th>
                <td>
                  <asp:Literal
                    ID="Expiration"
                    runat="server"
                    Text=""
                    EnableViewState="false"
                  ></asp:Literal>
                </td>
              </tr>
              <tr>
                <th align="right" valign="top">
                  <asp:Label
                    ID="LicensedDomainLabel"
                    runat="server"
                    Text="Registered Domain(s):"
                  ></asp:Label>
                </th>
                <td>
                  <asp:Repeater ID="DomainList" runat="server">
                    <ItemTemplate>
                      <asp:Label
                        ID="LicensedDomain"
                        runat="server"
                        Text="<%#Container.DataItem%>"
                      ></asp:Label
                      ><br />
                    </ItemTemplate>
                  </asp:Repeater>
                </td>
              </tr>
            </table>
          </div>
        </div>
        <asp:PlaceHolder ID="DemoModePanel" runat="server">
          <div class="section" style="padding: 0 0 2px 0">
            <div class="header">
              <h2 class="commonicon">
                <asp:Localize
                  ID="DemoModeCaption"
                  runat="server"
                  Text="Demo Mode"
                ></asp:Localize>
              </h2>
            </div>
            <div class="content">
              <asp:Localize
                ID="DemoModeHelpText"
                runat="server"
                Text="Your license is registered to the domain(s) listed above, and one of these must used to access the store site.  You can also configure your store to run in a non-expiring demo mode.  In demo mode any domain may be used to access the store site, but order billing and shipping addresses will not be recorded."
              ></asp:Localize
              ><br /><br />
              <asp:Localize
                ID="DemoModeEnabledText"
                runat="server"
                Text="Demo mode is currently <b>ENABLED</b>.  You may use any url to access your site, but order billing and shipping addresses will not be recorded."
              ></asp:Localize>
              <asp:Localize
                ID="DemoModeDisabledText"
                runat="server"
                Text="Demo mode is currently <b>DISABLED</b>.  You must use a registered domain to access your site.  To turn on demo mode, click below."
              ></asp:Localize
              ><br /><br />
              <asp:Button
                ID="DemoModeButton"
                runat="server"
                Text="Enable Demo Mode"
                OnClick="DemoModeButton_Click"
                CausesValidation="false"
              />
            </div>
          </div>
        </asp:PlaceHolder>
      </td>
      <td valign="top" width="50%">
        <div class="section" style="padding: 0 0 2px 0">
          <div class="header">
            <h2 class="commonicon">
              <asp:Localize
                ID="UpdateCaption"
                runat="server"
                Text="Update License"
              ></asp:Localize>
            </h2>
          </div>
          <asp:PlaceHolder ID="phUpdateKey" runat="server" Visible="true">
            <div class="content">
              <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
              <table class="inputForm">
                <tr>
                  <td colspan="2">
                    <asp:Label
                      ID="LicenseKeyLabel"
                      runat="server"
                      Text="Enter New License Key:"
                      SkinID="FieldHeader"
                      AssociatedControlID="LicenseKey"
                      EnableViewState="false"
                    ></asp:Label
                    ><br />
                  </td>
                </tr>
                <tr>
                  <td>
                    <asp:TextBox
                      ID="LicenseKey"
                      runat="server"
                      Text=""
                      MaxLength="38"
                      Width="300px"
                    ></asp:TextBox>
                    <asp:RequiredFieldValidator
                      ID="LicenseKeyRequired"
                      runat="server"
                      Text="*"
                      ErrorMessage="You must enter the license key."
                      ControlToValidate="LicenseKey"
                      Display="dynamic"
                    ></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator
                      ID="LicenseKeyFormat"
                      runat="server"
                      Text="*"
                      ErrorMessage="Your license key does not match the expected format."
                      ControlToValidate="LicenseKey"
                      ValidationExpression="^\{?[A-Fa-f0-9]{8}-?([A-Fa-f0-9]{4}-?){3}[A-Fa-f0-9]{12}\}?$"
                      Display="dynamic"
                    ></asp:RegularExpressionValidator>
                  </td>
                  <td>
                    <asp:Button
                      Id="SaveButon"
                      runat="server"
                      Text="Save"
                      OnClick="SaveButton_Click"
                      OnClientClick="if(Page_ClientValidate()){return confirm('Are you sure you want to update the license key?')}"
                    />
                  </td>
                </tr>
                <tr>
                  <td colspan="2">
                    (e.g. FD6B09C0-2AC9-4059-AE89-F27AB9285AAF)
                  </td>
                </tr>
              </table>
            </div>
          </asp:PlaceHolder>
          <asp:PlaceHolder
            ID="phPermissions"
            runat="server"
            EnableViewState="false"
            Visible="false"
          >
            <div class="pageHeader">
              <h1 style="font-size: 14px">
                Testing permissions for key update
              </h1>
            </div>
            <div style="margin-left: 5px">
              Sufficient file permissions are not available for MakerShop to
              update your license key file. Please ensure that the specified
              process identity has permissions to write or create
              '<b>~/App_Data/MakerShop.lic</b>' file. <br /><br />
              <b>Process Identity:</b>
              <asp:Literal ID="ProcessIdentity" runat="server"></asp:Literal
              ><br /><br /> <b>Test Result:</b
              ><asp:Literal
                ID="PermissionsTestResult"
                runat="server"
              ></asp:Literal>
            </div>
          </asp:PlaceHolder>
        </div>
      </td>
    </tr>
  </table>
</asp:Content>
