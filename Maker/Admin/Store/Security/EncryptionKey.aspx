<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"
CodeFile="EncryptionKey.aspx.cs" Inherits="Admin_Store_Security_EncryptionKey"
Title="Encryption Key" %> <%@ Import Namespace="MakerShop.Configuration" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
  <ajax:UpdatePanel ID="EncryptionAjax" runat="server">
    <Triggers>
      <ajax:PostBackTrigger ControlID="RestoreButton" />
    </Triggers>
    <ContentTemplate>
      <div class="pageHeader">
        <div class="caption">
          <h1>
            <asp:Localize
              ID="Caption"
              runat="server"
              Text="Encryption Key"
            ></asp:Localize>
          </h1>
        </div>
      </div>
      <table cellpadding="2" cellspacing="2" class="innerLayout">
        <tr>
          <td colspan="2">
            <p align="justify">
              <asp:Localize
                ID="InstructionText"
                runat="server"
                Text="Sensitive account data is encrypted within the database using a secret key.  Without this key, the data cannot be read.  You must update your key on a regular schedule, at least once per year but every 90 days is recommended."
              ></asp:Localize>
            </p>
            <p>
              <asp:Label
                ID="LastSetLabel"
                runat="server"
                Text="Key Last Updated:"
                SkinID="FieldHeader"
              ></asp:Label>
              <asp:Label ID="LastSet" runat="server" Text=""></asp:Label>
            </p>
          </td>
        </tr>
        <tr>
          <td width="50%" valign="top">
            <div class="section" style="padding: 0 0 2px 0">
              <div class="header">
                <h2 class="encrypt">
                  <asp:Localize
                    ID="ChangeCaption"
                    runat="server"
                    Text="Change Encryption Key"
                  ></asp:Localize>
                </h2>
              </div>
              <div class="content">
                <p align="justify">
                  <asp:Localize
                    ID="ChangeInstructionText"
                    runat="server"
                    Text="To change your key, all data in the database must be decrypted with the old key and then re-encrypted with the new key.  This process can take some time depending on the size of your database; the estimated workload is shown below.  Once you intiate a key change, a progress indicator will be shown to let you know when the process is complete.  Always ensure you have both a database backup and a key backup before initiating a key change."
                  ></asp:Localize>
                </p>
                <p align="justify">
                  <asp:Localize
                    ID="ChangeInstructionText2"
                    runat="server"
                    Text="To ensure maximum security of your data, provide some random text to help generate the key.  You must type at least 20 characters; the more random the better."
                  ></asp:Localize>
                </p>
                <asp:Panel ID="ChangePanel" runat="server">
                  <asp:ValidationSummary
                    ID="ValidationSummary1"
                    runat="server"
                    ValidationGroup="ChangeKey"
                  />
                  <table class="inputForm" cellpadding="2" cellspacing="2">
                    <tr>
                      <th class="rowHeader">
                        <asp:Label
                          ID="EstimatedWorkloadLabel"
                          runat="server"
                          Text="Estimated Workload:"
                        ></asp:Label>
                      </th>
                      <td>
                        <asp:Label
                          ID="EstimatedWorkload"
                          runat="server"
                          Text=""
                        ></asp:Label>
                        <asp:Label
                          ID="EstimatedWorkoadLabel2"
                          runat="server"
                          Text=" records"
                        ></asp:Label>
                      </td>
                    </tr>
                    <tr>
                      <th class="rowHeader">
                        <asp:Label
                          ID="RandomTextLabel"
                          runat="server"
                          Text="Random Text:"
                        ></asp:Label>
                      </th>
                      <td>
                        <asp:TextBox
                          ID="RandomText"
                          runat="server"
                          Text=""
                          AutoCompleteType="None"
                          autocomplete="off"
                        ></asp:TextBox>
                        <asp:RequiredFieldValidator
                          ID="RandomTextValidator1"
                          runat="server"
                          ControlToValidate="RandomText"
                          Text="*"
                          ErrorMessage="You must type at least 20 random characters to generate a new key."
                          Display="Dynamic"
                          ValidationGroup="ChangeKey"
                        ></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator
                          ID="RequiredFieldValidator2"
                          runat="server"
                          ControlToValidate="RandomText"
                          Text="*"
                          ErrorMessage="You must type at least 20 random characters to generate a new key."
                          Display="Dynamic"
                          ValidationExpression="^(DECRYPT)|(.{20,})$"
                          ValidationGroup="ChangeKey"
                        ></asp:RegularExpressionValidator>
                      </td>
                    </tr>
                    <tr>
                      <td>&nbsp;</td>
                      <td>
                        <asp:Button
                          ID="UpdateButton"
                          runat="server"
                          Text="Change Encryption Key"
                          OnClick="UpdateButton_Click"
                          OnClientClick="if (Page_ClientValidate('ChangeKey')){if (confirm('The encryption key is about to be changed.  Click OK to confirm.')){this.value='Processing...';return true;}return false;}return false;"
                          ValidationGroup="ChangeKey"
                        />
                      </td>
                    </tr>
                  </table>
                </asp:Panel>
                <asp:Panel
                  ID="ChangeProgressPanel"
                  runat="server"
                  Visible="false"
                >
                  <ajax:Timer
                    ID="ChangeProgressTimer"
                    runat="server"
                    Interval="10000"
                    OnTick="ChangeProgressTimer_Tick"
                  ></ajax:Timer>
                  <asp:Label
                    ID="KeyUpdatingMessage"
                    runat="server"
                    SkinID="GoodCondition"
                    Text="Your key is being updated..."
                  ></asp:Label>
                  <table class="inputForm" cellpadding="2" cellspacing="2">
                    <tr>
                      <th class="rowHeader">
                        <asp:Label
                          ID="RemainingWorkloadLabel"
                          runat="server"
                          Text="Remaining Workload:"
                        ></asp:Label>
                      </th>
                      <td>
                        <asp:Label
                          ID="RemainingWorkload"
                          runat="server"
                          Text=""
                        ></asp:Label>
                        <asp:Label
                          ID="RemainingWorkloadLabel2"
                          runat="server"
                          Text=" records"
                        ></asp:Label>
                      </td>
                    </tr>
                    <tr>
                      <th class="rowHeader">
                        <asp:Label
                          ID="ProgressDateLabel"
                          runat="server"
                          Text="Status as of:"
                        ></asp:Label>
                      </th>
                      <td>
                        <asp:Label
                          ID="ProgressDate"
                          runat="server"
                          Text=""
                          EnableViewState="false"
                        ></asp:Label>
                      </td>
                    </tr>
                  </table>
                  <asp:Panel
                    ID="trRestartCancel"
                    runat="server"
                    Visible="false"
                  >
                    <p align="justify">
                      <asp:Label
                        ID="CancelChangeInstructionText"
                        runat="server"
                        Text="Has the key change stopped progressing?  You can try to restart or cancel.  It is recommended that you review the documentation before you choose either option."
                      ></asp:Label>
                    </p>
                    <asp:LinkButton
                      ID="RestartChangeButton"
                      runat="server"
                      Text="Restart"
                      OnClientClick="return confirm('Are you sure you wish to restart the key change?')"
                      OnClick="RestartChangeButton_Click"
                    ></asp:LinkButton
                    >&nbsp;&nbsp;
                    <asp:LinkButton
                      ID="CancelChangeButton"
                      runat="server"
                      Text="Cancel"
                      OnClientClick="return confirm('Are you sure you wish to cancel the key change?')"
                      OnClick="CancelChangeButton_Click"
                    ></asp:LinkButton>
                  </asp:Panel>
                </asp:Panel>
                <asp:Label
                  ID="KeyUpdatedMessage"
                  runat="server"
                  SkinID="GoodCondition"
                  Text="Your key has been updated."
                  Visible="false"
                  EnableViewState="false"
                ></asp:Label>
              </div>
            </div>
          </td>
          <td width="50%" valign="Top">
            <div class="section" style="padding: 0 0 2px 0">
              <div class="header">
                <h2 class="download">
                  <asp:Localize
                    ID="BackupCaption"
                    runat="server"
                    Text="Back-up Encryption Key"
                  ></asp:Localize>
                </h2>
              </div>
              <div class="content">
                <asp:Panel ID="BackupPanel" runat="server">
                  <p align="justify">
                    <asp:Localize
                      ID="BackupInstructionText"
                      runat="server"
                      Text="To keep your key secure, it is securely stored apart from the database.  In the event that you must restore your database to another location, it will be vital that you have this key.  Whenever you change your key, download the key backup files and store them in a physically secure location.  You need both backup files to restore the key."
                    ></asp:Localize>
                  </p>
                  <asp:Button
                    ID="ShowBackupLinks"
                    runat="server"
                    Text="Get Backups"
                    OnClick="ShowBackupLinks_Click"
                  />
                  <asp:HyperLink
                    ID="GetBackup1"
                    runat="server"
                    Text="Backup Part 1"
                    Visible="false"
                    NavigateUrl="GetKeyBackup.ashx?part=1"
                  ></asp:HyperLink
                  >&nbsp;&nbsp;&nbsp;
                  <asp:HyperLink
                    ID="GetBackup2"
                    runat="server"
                    Text="Backup Part 2"
                    Visible="false"
                    NavigateUrl="GetKeyBackup.ashx?part=2"
                  ></asp:HyperLink>
                </asp:Panel>
                <asp:Panel
                  ID="NoKeyNoBackupPanel"
                  runat="server"
                  Visible="false"
                >
                  <p align="justify">
                    <asp:Localize
                      ID="NoKeyNoBackupInstructionText"
                      runat="server"
                      Text="You do not currently have an encryption key set.  You must set a key before you can use the backup tool."
                    ></asp:Localize>
                  </p>
                </asp:Panel>
              </div>
            </div>
            <div class="section" style="padding: 0 0 2px 0">
              <div class="header">
                <h2 class="upload">
                  <asp:Localize
                    ID="RestoreCaption"
                    runat="server"
                    Text="Restore Encryption Key"
                  ></asp:Localize>
                </h2>
              </div>
              <div class="content">
                <p align="justify">
                  <asp:Localize
                    ID="RestoreInstructionText"
                    runat="server"
                    Text="If you need to restore your key, provide the backup files below.  The key currently being used will be replaced with the backup.  No re-encryption takes place during this process, since presumably the existing data is already encrypted with the backup key you wish to restore."
                  ></asp:Localize>
                </p>
                <asp:ValidationSummary
                  ID="RestoreValidationSummary"
                  runat="server"
                  ValidationGroup="Restore"
                />
                <asp:Label
                  ID="RestoredMessage"
                  runat="server"
                  Text="Key backup restored at {0:t}"
                  SkinID="GoodCondition"
                  EnableViewState="false"
                  Visible="false"
                ></asp:Label>
                <table class="inputForm">
                  <tr>
                    <th class="rowHeader">
                      <asp:Label
                        ID="BackupPart1Label"
                        runat="server"
                        Text="Backup Part 1:"
                      ></asp:Label>
                    </th>
                    <td>
                      <asp:FileUpload ID="BackupPart1" runat="server" />
                      <asp:RequiredFieldValidator
                        ID="BackupPart1Validator"
                        runat="server"
                        ControlToValidate="BackupPart1"
                        Text="*"
                        ErrorMessage="You must provide part 1 of the backup set."
                        Display="Dynamic"
                        ValidationGroup="Restore"
                      ></asp:RequiredFieldValidator>
                      <asp:PlaceHolder
                        ID="phRestoreValidators"
                        runat="server"
                      ></asp:PlaceHolder>
                    </td>
                  </tr>
                  <tr>
                    <th class="rowHeader">
                      <asp:Label
                        ID="BackupPart2Label"
                        runat="server"
                        Text="Backup Part 2:"
                      ></asp:Label>
                    </th>
                    <td>
                      <asp:FileUpload ID="BackupPart2" runat="server" />
                      <asp:RequiredFieldValidator
                        ID="BackupPart2Validator"
                        runat="server"
                        ControlToValidate="BackupPart2"
                        Text="*"
                        ErrorMessage="You must provide part 2 of the backup set."
                        Display="Dynamic"
                        ValidationGroup="Restore"
                      ></asp:RequiredFieldValidator>
                    </td>
                  </tr>
                  <tr>
                    <td>&nbsp;</td>
                    <td>
                      <asp:Button
                        ID="RestoreButton"
                        runat="server"
                        Text="Restore Key"
                        OnClientClick="if (Page_ClientValidate('Restore')){return confirm('The encryption key is about to be restored from backup.  Click OK to confirm.')}"
                        OnClick="RestoreButton_Click"
                        ValidationGroup="Restore"
                      />
                    </td>
                  </tr>
                </table>
              </div>
            </div>
          </td>
        </tr>
      </table>
    </ContentTemplate>
  </ajax:UpdatePanel>
</asp:Content>
