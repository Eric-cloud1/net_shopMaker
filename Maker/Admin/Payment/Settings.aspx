<%@ Page Language="C#" MasterPageFile="../Admin.master"
CodeFile="Settings.aspx.cs" Inherits="Admin_Payment_Settings" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb"
%> <%@ Register src="../UserControls/autoRefreshImage.ascx"
tagname="autoRefreshImage" tagprefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
  <div class="pageHeader">
    <div class="caption">
      <h1>
        <asp:Localize
          ID="Caption"
          runat="server"
          Text="Payment Settings"
        ></asp:Localize>
      </h1>
    </div>
  </div>
  <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
      <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
          <td valign="top" align="center">
            <div class="section" style="padding: 0 0 2px 0">
              <div class="header">
                <h2 class="commonicon" align="Left">
                  <asp:Localize
                    ID="PaymentSettingCaption"
                    runat="server"
                    Text="Configure Payment Settings"
                  ></asp:Localize>
                </h2>
              </div>
              <div class="content">
                <table class="inputForm">
                  <tr>
                    <td colspan="5">
                      <asp:ValidationSummary
                        ID="ValidationSummary"
                        runat="server"
                      />
                      <asp:Label
                        ID="SavedMessage"
                        runat="server"
                        Text="The settings have been saved."
                        Visible="false"
                        SkinID="GoodCondition"
                      ></asp:Label>
                    </td>
                  </tr>
                  <tr>
                    <th class="rowHeader">
                      <asp:Label ID="Label1" runat="server" Text=""></asp:Label>
                    </th>
                    <th class="rowHeader">
                      <div align="center">
                        <cb:ToolTipLabel
                          ID="enableLabel"
                          runat="server"
                          Text="Active"
                          ToolTip="Identifies current status of process and activates or de-activates the process"
                        >
                        </cb:ToolTipLabel>
                      </div>
                    </th>
                    <th class="rowHeader">
                      <div align="center">
                        <cb:ToolTipLabel
                          ID="threadsLabel"
                          runat="server"
                          Text="Threads"
                          ToolTip="Identifies the number of threads to run for the process"
                        >
                        </cb:ToolTipLabel>
                      </div>
                    </th>
                    <th class="rowHeader">
                      <div align="center">
                        <cb:ToolTipLabel
                          ID="statusLabel"
                          runat="server"
                          Text="Status"
                          ToolTip="Identifies current process status"
                        >
                        </cb:ToolTipLabel>
                      </div>
                    </th>
                    <th class="rowHeader">
                      <div align="center">
                        <cb:ToolTipLabel
                          ID="pauseLabel"
                          runat="server"
                          Text="Pause(s)"
                          ToolTip="How long to wait between cycles in seconds. Implies 1 thread if > 0"
                        >
                        </cb:ToolTipLabel>
                      </div>
                    </th>
                  </tr>
                  <tr>
                    <td colspan="5" style="background-color: #c0c0c0">
                      Automated Queues
                    </td>
                  </tr>
                  <tr>
                    <th class="rowHeader">
                      <asp:Label
                        ID="AuthorizeLabel"
                        runat="server"
                        Text="Authorize:"
                      ></asp:Label>
                    </th>
                    <td>
                      <asp:CheckBox
                        ID="cbenableAuth"
                        runat="server"
                        value="true"
                        Text=""
                      ></asp:CheckBox>
                    </td>
                    <td>
                      <asp:TextBox
                        ID="tbAuthorize"
                        runat="server"
                        Columns="4"
                        MaxLength="2"
                      ></asp:TextBox>
                      <asp:RangeValidator
                        ID="AuthorizeValidator"
                        runat="server"
                        Type="Integer"
                        MinimumValue="0"
                        MaximumValue="10"
                        ControlToValidate="tbAuthorize"
                        ErrorMessage="Threads Must be less than 10"
                        Text="*"
                      ></asp:RangeValidator>
                    </td>
                    <td>
                      <uc1:autoRefreshImage
                        ID="AuthImage"
                        runat="server"
                        autoRefresh="false"
                      />
                    </td>
                    <td></td>
                  </tr>
                  <tr>
                    <th class="rowHeader">
                      <asp:Label
                        ID="CaptureLabel"
                        runat="server"
                        Text="Capture:"
                      ></asp:Label>
                    </th>
                    <td>
                      <asp:CheckBox
                        ID="cbenablecap"
                        runat="server"
                        value="true"
                        Text=""
                      ></asp:CheckBox>
                    </td>
                    <td>
                      <asp:TextBox
                        ID="tbCapture"
                        runat="server"
                        Columns="4"
                        MaxLength="2"
                      ></asp:TextBox>
                      <asp:RangeValidator
                        ID="CaptureValidator"
                        runat="server"
                        Type="Integer"
                        MinimumValue="0"
                        MaximumValue="10"
                        ControlToValidate="tbCapture"
                        ErrorMessage="Threads Must be less than 10"
                        Text="*"
                      ></asp:RangeValidator>
                    </td>
                    <td>
                      <uc1:autoRefreshImage
                        ID="CaptureImage"
                        runat="server"
                        autoRefresh="false"
                      />
                    </td>
                    <td />
                  </tr>
                  <tr>
                    <th class="rowHeader">
                      <asp:Label
                        ID="RefundLabel"
                        runat="server"
                        Text="Refund:"
                      ></asp:Label>
                    </th>
                    <td>
                      <asp:CheckBox
                        ID="cbenableref"
                        runat="server"
                        value="true"
                        Text=""
                      ></asp:CheckBox>
                    </td>
                    <td>&nbsp;</td>
                    <td>
                      <uc1:autoRefreshImage
                        ID="RefundImage"
                        runat="server"
                        autoRefresh="false"
                      />
                    </td>
                    <td />
                  </tr>
                  <tr>
                    <th class="rowHeader">
                      <asp:Label
                        ID="VoidLabel"
                        runat="server"
                        Text="Void:"
                      ></asp:Label>
                    </th>
                    <td>
                      <asp:CheckBox
                        ID="cbenablevoid"
                        runat="server"
                        value="true"
                        Text=""
                      ></asp:CheckBox>
                    </td>
                    <td>&nbsp;</td>
                    <td>
                      <uc1:autoRefreshImage
                        ID="VoidImage"
                        runat="server"
                        autoRefresh="false"
                      />
                    </td>
                    <td />
                  </tr>
                  <tr>
                    <td colspan="5" style="background-color: #c0c0c0">
                      Reschedule Queues
                    </td>
                  </tr>
                  <tr>
                    <th class="rowHeader">
                      <asp:Label
                        ID="authorizeRescheduleLb"
                        runat="server"
                        Text="Authorize:"
                      ></asp:Label>
                    </th>
                    <td>
                      <asp:CheckBox
                        ID="cbEnableAuthOverrided"
                        runat="server"
                        value="true"
                        Text=""
                      ></asp:CheckBox>
                    </td>
                    <td>
                      <asp:TextBox
                        ID="tbThreadAuthOverride"
                        runat="server"
                        Columns="4"
                        MaxLength="2"
                      ></asp:TextBox>
                      <asp:RangeValidator
                        ID="RangeValidator5"
                        runat="server"
                        Type="Integer"
                        MinimumValue="0"
                        MaximumValue="10"
                        ControlToValidate="tbThreadAuthOverride"
                        ErrorMessage="Threads Must be less than 10"
                        Text="*"
                      ></asp:RangeValidator>
                    </td>
                    <td>
                      <uc1:autoRefreshImage
                        ID="RescheduleAuthImage"
                        runat="server"
                        autoRefresh="false"
                      />
                    </td>
                    <td />
                  </tr>
                  <tr>
                    <th class="rowHeader">
                      <asp:Label
                        ID="captureRescheduleLb"
                        runat="server"
                        Text="Capture:"
                      ></asp:Label>
                    </th>
                    <td>
                      <asp:CheckBox
                        ID="cbEnableCapOverride"
                        runat="server"
                        value="true"
                        Text=""
                      ></asp:CheckBox>
                    </td>
                    <td>
                      <asp:TextBox
                        ID="tbThreadCapOverride"
                        runat="server"
                        Columns="4"
                        MaxLength="2"
                      ></asp:TextBox>
                      <asp:RangeValidator
                        ID="RangeValidator6"
                        runat="server"
                        Type="Integer"
                        MinimumValue="0"
                        MaximumValue="10"
                        ControlToValidate="tbThreadCapOverride"
                        ErrorMessage="Threads Must be less than 10"
                        Text="*"
                      ></asp:RangeValidator>
                    </td>
                    <td>
                      <uc1:autoRefreshImage
                        ID="RescheduleCaptureImage"
                        runat="server"
                        autoRefresh="false"
                      />
                    </td>
                    <td />
                  </tr>
                  <tr>
                    <td colspan="5" style="background-color: #c0c0c0">
                      [Q1_Rebill] CTPE_Matt_RB_Stubina
                    </td>
                  </tr>
                  <tr>
                    <th class="rowHeader">
                      <asp:Label
                        ID="Q1Authlb"
                        runat="server"
                        Text="Authorize:"
                      ></asp:Label>
                    </th>
                    <td>
                      <asp:CheckBox
                        ID="cbAuthorize_Q1"
                        runat="server"
                        value="true"
                        Text=""
                      ></asp:CheckBox>
                    </td>
                    <td>
                      <asp:TextBox
                        ID="tbThreads_Q1"
                        runat="server"
                        Columns="4"
                        MaxLength="2"
                      ></asp:TextBox>
                      <asp:RangeValidator
                        ID="RangeValidator1"
                        runat="server"
                        Type="Integer"
                        MinimumValue="0"
                        MaximumValue="10"
                        ControlToValidate="tbThreads_Q1"
                        ErrorMessage="Threads Must be less than 10"
                        Text="*"
                      ></asp:RangeValidator>
                    </td>
                    <td>
                      <uc1:autoRefreshImage
                        ID="Q1_RebillAuthImage"
                        runat="server"
                        autoRefresh="false"
                      />
                    </td>
                    <td>
                      <asp:TextBox
                        ID="tbPause_Q1"
                        runat="server"
                        Columns="5"
                        MaxLength="3"
                      ></asp:TextBox
                      >s
                      <asp:RangeValidator
                        ID="RangeValidator7"
                        runat="server"
                        Type="Integer"
                        MinimumValue="0"
                        MaximumValue="999"
                        ControlToValidate="tbPause_Q1"
                        ErrorMessage="Pause Must be less than 999"
                        Text="*"
                      ></asp:RangeValidator>
                    </td>
                  </tr>
                  <tr>
                    <td colspan="5" style="background-color: #c0c0c0">
                      [Q2_Rebill] CTPE_Matt_RB_Tenereck
                    </td>
                  </tr>
                  <tr>
                    <th class="rowHeader">
                      <asp:Label
                        ID="Q2Authlb"
                        runat="server"
                        Text="Authorize:"
                      ></asp:Label>
                    </th>
                    <td>
                      <asp:CheckBox
                        ID="cbAuthorize_Q2"
                        runat="server"
                        value="true"
                        Text=""
                      ></asp:CheckBox>
                    </td>
                    <td>
                      <asp:TextBox
                        ID="tbThreads_Q2"
                        runat="server"
                        Columns="4"
                        MaxLength="2"
                      ></asp:TextBox>
                      <asp:RangeValidator
                        ID="RangeValidator2"
                        runat="server"
                        Type="Integer"
                        MinimumValue="0"
                        MaximumValue="10"
                        ControlToValidate="tbThreads_Q2"
                        ErrorMessage="Threads Must be less than 10"
                        Text="*"
                      ></asp:RangeValidator>
                    </td>
                    <td>
                      <uc1:autoRefreshImage
                        ID="Q2_RebillAuthImage"
                        runat="server"
                        autoRefresh="false"
                      />
                    </td>
                    <td>
                      <asp:TextBox
                        ID="tbPause_Q2"
                        runat="server"
                        Columns="5"
                        MaxLength="3"
                      ></asp:TextBox
                      >s
                      <asp:RangeValidator
                        ID="RangeValidator8"
                        runat="server"
                        Type="Integer"
                        MinimumValue="0"
                        MaximumValue="999"
                        ControlToValidate="tbPause_Q2"
                        ErrorMessage="Pause Must be less than 999"
                        Text="*"
                      ></asp:RangeValidator>
                    </td>
                  </tr>
                  <tr>
                    <td colspan="5" style="background-color: #c0c0c0">
                      [Q5_Rebill] CTPE_Matt_RB_Punitron
                    </td>
                  </tr>
                  <tr>
                    <th class="rowHeader">
                      <asp:Label
                        ID="Q5Authlb"
                        runat="server"
                        Text="Authorize:"
                      ></asp:Label>
                    </th>
                    <td>
                      <asp:CheckBox
                        ID="cbAuthorize_Q5"
                        runat="server"
                        value="true"
                        Text=""
                      ></asp:CheckBox>
                    </td>
                    <td>
                      <asp:TextBox
                        ID="tbThreads_Q5"
                        runat="server"
                        Columns="4"
                        MaxLength="2"
                      ></asp:TextBox>
                      <asp:RangeValidator
                        ID="RangeValidator11"
                        runat="server"
                        Type="Integer"
                        MinimumValue="0"
                        MaximumValue="10"
                        ControlToValidate="tbThreads_Q5"
                        ErrorMessage="Threads Must be less than 10"
                        Text="*"
                      ></asp:RangeValidator>
                    </td>
                    <td>
                      <uc1:autoRefreshImage
                        ID="Q5_RebillAuthImage"
                        runat="server"
                        autoRefresh="false"
                      />
                    </td>
                    <td>
                      <asp:TextBox
                        ID="tbPause_Q5"
                        runat="server"
                        Columns="5"
                        MaxLength="3"
                      ></asp:TextBox
                      >s
                      <asp:RangeValidator
                        ID="RangeValidator12"
                        runat="server"
                        Type="Integer"
                        MinimumValue="0"
                        MaximumValue="999"
                        ControlToValidate="tbPause_Q5"
                        ErrorMessage="Pause Must be less than 999"
                        Text="*"
                      ></asp:RangeValidator>
                    </td>
                  </tr>
                  <tr>
                    <td colspan="5" style="background-color: #c0c0c0">
                      [Q3_Rebill] Matt A 8a81941a2f071a3c012f9237fced18b4
                    </td>
                  </tr>
                  <tr>
                    <th class="rowHeader">
                      <asp:Label
                        ID="Q3Authlb"
                        runat="server"
                        Text="Authorize:"
                      ></asp:Label>
                    </th>
                    <td>
                      <asp:CheckBox
                        ID="cbAuthorize_Q3"
                        runat="server"
                        value="true"
                        Text=""
                      ></asp:CheckBox>
                    </td>
                    <td>
                      <asp:TextBox
                        ID="tbThreads_Q3"
                        runat="server"
                        Columns="4"
                        MaxLength="2"
                      ></asp:TextBox>
                      <asp:RangeValidator
                        ID="RangeValidator3"
                        runat="server"
                        Type="Integer"
                        MinimumValue="0"
                        MaximumValue="10"
                        ControlToValidate="tbThreads_Q3"
                        ErrorMessage="Threads Must be less than 10"
                        Text="*"
                      ></asp:RangeValidator>
                    </td>
                    <td>
                      <uc1:autoRefreshImage
                        ID="Q3_RebillAuthImage"
                        runat="server"
                        autoRefresh="false"
                      />
                    </td>
                    <td />
                    <asp:TextBox
                      ID="tbPause_Q3"
                      runat="server"
                      Columns="5"
                      MaxLength="3"
                    ></asp:TextBox
                    >s
                    <asp:RangeValidator
                      ID="RangeValidator9"
                      runat="server"
                      ControlToValidate="tbPause_Q3"
                      ErrorMessage="Pause Must be less than 999"
                      MaximumValue="999"
                      MinimumValue="0"
                      Text="*"
                      Type="Integer"
                    ></asp:RangeValidator>
                  </tr>
                  <tr>
                    <td colspan="5" style="background-color: #c0c0c0">
                      [Q4_Rebill] Matt B 8a81941a2f071a3c012f9c0081a62b41y
                    </td>
                  </tr>
                  <tr>
                    <th class="rowHeader">
                      <asp:Label
                        ID="Q4Authlb"
                        runat="server"
                        Text="Authorize:"
                      ></asp:Label>
                    </th>
                    <td>
                      <asp:CheckBox
                        ID="cbAuthorize_Q4"
                        runat="server"
                        value="true"
                        Text=""
                      ></asp:CheckBox>
                    </td>
                    <td>
                      <asp:TextBox
                        ID="tbThreads_Q4"
                        runat="server"
                        Columns="4"
                        MaxLength="2"
                      ></asp:TextBox>
                      <asp:RangeValidator
                        ID="RangeValidator4"
                        runat="server"
                        Type="Integer"
                        MinimumValue="0"
                        MaximumValue="10"
                        ControlToValidate="tbThreads_Q4"
                        ErrorMessage="Threads Must be less than 10"
                        Text="*"
                      ></asp:RangeValidator>
                    </td>
                    <td>
                      <uc1:autoRefreshImage
                        ID="Q4_RebillAuthImage"
                        runat="server"
                        autoRefresh="false"
                      />
                    </td>
                    <td>
                      <asp:TextBox
                        ID="tbPause_Q4"
                        runat="server"
                        Columns="5"
                        MaxLength="3"
                      ></asp:TextBox
                      >s
                      <asp:RangeValidator
                        ID="RangeValidator10"
                        runat="server"
                        Type="Integer"
                        MinimumValue="0"
                        MaximumValue="999"
                        ControlToValidate="tbPause_Q4"
                        ErrorMessage="Pause Must be less than 999"
                        Text="*"
                      ></asp:RangeValidator>
                    </td>
                  </tr>
                </table>
              </div>
            </div>
          </td>
        </tr>
        <tr>
          <td align="center" colspan="2">
            <asp:Button
              ID="SaveButon"
              runat="server"
              Text="Save"
              OnClick="SaveButton_Click"
              CssClass="button"
            />
            <asp:Button
              ID="SaveAndCloseButton"
              runat="server"
              Text="Save and Close"
              OnClick="SaveAndCloseButton_Click"
              CssClass="button"
            />
            <asp:Button
              ID="CancelButton"
              runat="server"
              Text="Cancel"
              OnClick="CancelButton_Click"
              CssClass="button"
            />
          </td>
        </tr>
      </table>
    </ContentTemplate>
  </ajax:UpdatePanel>
</asp:Content>
