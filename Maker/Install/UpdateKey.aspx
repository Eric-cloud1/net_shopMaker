<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UpdateKey.aspx.cs"
Inherits="Install_UpdateKey" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
  <head id="Head1" runat="server">
    <title>Update MakerShop License Key</title>
    <style>
      p {
        font-size: 12px;
        margin: 12px 0;
      }
      .sectionHeader {
        background-color: #efefef;
        padding: 3px;
        margin: 12px 0px;
      }
      h2 {
        font-size: 14px;
        font-weight: bold;
        margin: 0px;
      }
      .error {
        font-weight: bold;
        color: red;
      }
      div.radio {
        margin: 2px 0px 6px 0px;
      }
      div.radio label {
        font-weight: bold;
        padding-top: 6px;
        position: relative;
        top: 1px;
      }
      .inputBox {
        padding: 6px;
        margin: 4px 40px;
        border: solid 1px #cccccc;
      }
      div.submit {
        background-color: #efefef;
        padding: 4px;
        margin: 12px 0px;
        text-align: center;
      }
    </style>
    <script type="text/javascript" language="JavaScript">
      var counter = 0;
      function plswt() {
        counter++;
        if (counter > 1) {
          alert(
            "You have already submitted this form.  Please wait while the install processes."
          );
          return false;
        }
        return true;
      }
    </script>
  </head>
  <body style="width: 780px; margin: auto">
    <form id="form1" runat="server">
      <asp:PlaceHolder
        ID="phUpdateKey"
        runat="server"
        EnableViewState="false"
        Visible="true"
      >
        <br />
        <div class="pageHeader">
          <h1 style="font-size: 16px">Update MakerShop License Key</h1>
        </div>
        <div style="padding-left: 10px; padding-right: 10px">
          <asp:Panel ID="MessagePanel" runat="server" Visible="false">
            <div class="error">
              <asp:Literal ID="ReponseMessage" runat="server"></asp:Literal>
            </div>
          </asp:Panel>
          <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
          <ajax:ScriptManager
            ID="ScriptManager"
            runat="server"
            EnablePartialRendering="true"
          ></ajax:ScriptManager>
          <ajax:UpdatePanel ID="InstallAjax" runat="server">
            <Triggers>
              <asp:PostBackTrigger ControlID="UpdateKeyButton" />
              <asp:PostBackTrigger ControlID="KeyUpload" />
            </Triggers>
            <ContentTemplate>
              <div class="radio">
                <asp:RadioButton
                  ID="KeyDemo"
                  runat="server"
                  Text="Use a 30-day demo key"
                  OnCheckedChanged="KeyDemo_CheckedChanged"
                  AutoPostBack="true"
                  GroupName="KeyOption"
                />
              </div>
              <asp:PlaceHolder ID="KeyDemoPanel" runat="server" Visible="false">
                <div class="inputBox">
                  <p>
                    To register for a 30 day demo key for MakerShop 7.0, fill in
                    the form below. Be aware that you can only register the
                    listed domain one time using this form. For extended demo
                    keys, you must contact us at 1-866-571-5888.
                  </p>
                  <table cellpadding="5" cellspacing="0">
                    <tr>
                      <th align="right" valign="top">Domain:</th>
                      <td>
                        <asp:Literal ID="Domain" runat="server"></asp:Literal>
                      </td>
                    </tr>
                    <tr>
                      <th align="right" valign="top">Email:</th>
                      <td>
                        <asp:TextBox
                          ID="Email"
                          runat="server"
                          MaxLength="200"
                        ></asp:TextBox>
                        required
                        <asp:RequiredFieldValidator
                          ID="EmailRequired"
                          runat="server"
                          Text="*"
                          ErrorMessage="You must enter your email."
                          ControlToValidate="Email"
                        ></asp:RequiredFieldValidator
                        ><br />
                        (e.g. name@yourdomain.xyz)
                      </td>
                    </tr>
                    <tr>
                      <th align="right">Name:</th>
                      <td>
                        <asp:TextBox
                          ID="Name"
                          runat="server"
                          MaxLength="50"
                        ></asp:TextBox>
                        required
                        <asp:RequiredFieldValidator
                          ID="NameRequired"
                          runat="server"
                          Text="*"
                          ErrorMessage="You must enter your name."
                          ControlToValidate="Name"
                        ></asp:RequiredFieldValidator>
                      </td>
                    </tr>
                    <tr>
                      <th align="right">Company:</th>
                      <td>
                        <asp:TextBox
                          ID="Company"
                          runat="server"
                          MaxLength="50"
                        ></asp:TextBox>
                      </td>
                    </tr>
                    <tr>
                      <th align="right">Phone:</th>
                      <td>
                        <asp:TextBox
                          ID="Phone"
                          runat="server"
                          MaxLength="50"
                        ></asp:TextBox>
                      </td>
                    </tr>
                  </table>
                </div>
                <br />
              </asp:PlaceHolder>
              <div class="radio">
                <asp:RadioButton
                  ID="LicenseKeyOption"
                  runat="server"
                  Checked="true"
                  Text="Provide my license key"
                  OnCheckedChanged="LicenseKeyOption_CheckedChanged"
                  AutoPostBack="true"
                  GroupName="KeyOption"
                />
              </div>
              <asp:PlaceHolder ID="LicenseKeyPanel" runat="server">
                <div class="inputBox">
                  <table cellpadding="5" cellspacing="0">
                    <tr>
                      <th align="right" valign="top">License Key:</th>
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
                        ></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator
                          ID="LicenseKeyFormat"
                          runat="server"
                          Text="*"
                          ErrorMessage="Your license key does not match the expected format."
                          ControlToValidate="LicenseKey"
                          ValidationExpression="^\{?[A-Fa-f0-9]{8}-?([A-Fa-f0-9]{4}-?){3}[A-Fa-f0-9]{12}\}?$"
                        ></asp:RegularExpressionValidator>
                        <br />(e.g. FD6B09C0-2AC9-4059-AE89-F27AB9285AAF)
                      </td>
                    </tr>
                  </table>
                </div>
                <br />
              </asp:PlaceHolder>
              <div class="radio">
                <asp:RadioButton
                  ID="KeyUpload"
                  runat="server"
                  Checked="false"
                  Text="Upload my license file"
                  OnCheckedChanged="KeyUpload_CheckedChanged"
                  AutoPostBack="true"
                  GroupName="KeyOption"
                />
              </div>
              <asp:PlaceHolder
                ID="KeyUploadPanel"
                runat="server"
                Visible="false"
              >
                <div class="inputBox">
                  <p>
                    If you have an existing MakerShop.lic file, upload it here.
                  </p>
                  <table cellpadding="5" cellspacing="0">
                    <tr>
                      <th align="right" valign="top">Key File:</th>
                      <td>
                        <asp:FileUpload
                          ID="KeyFile"
                          runat="server"
                          Width="200px"
                        />
                      </td>
                    </tr>
                  </table>
                </div>
                <br />
              </asp:PlaceHolder>
              <br />
              <div class="submit">
                <asp:HiddenField ID="ReturnToUpgrade" runat="server" />
                <asp:Button
                  ID="UpdateKeyButton"
                  runat="server"
                  Text="Update Key"
                  OnClick="UpdateKeyButton_Click"
                  OnClientClick="if(Page_ClientValidate()){this.value='Processing...';return plswt();}"
                />
              </div>
            </ContentTemplate>
          </ajax:UpdatePanel>
        </div>
      </asp:PlaceHolder>
      <asp:PlaceHolder
        ID="phPermissions"
        runat="server"
        EnableViewState="false"
        Visible="false"
      >
        <br />
        <div class="pageHeader">
          <h1 style="font-size: 16px">Permissions Test for Key Update</h1>
        </div>
        Sufficient file permissions are not available for MakerShop to update
        your license key file. Please ensure that the specified process identity
        has permissions to write or create '<b>~/App_Data/MakerShop.lic</b>'
        file. <br /><br />
        <b>Process Identity:</b>
        <asp:Literal ID="ProcessIdentity" runat="server"></asp:Literal
        ><br /><br /> <b>Test Result:</b
        ><asp:Literal ID="PermissionsTestResult" runat="server"></asp:Literal>
      </asp:PlaceHolder>
    </form>
  </body>
</html>
