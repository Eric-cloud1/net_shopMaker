<%@ Page Language="C#" MasterPageFile="~/Admin/Products/Product.master"
CodeFile="EditSubscription.aspx.cs" Inherits="Admin_Products_EditSubscription"
Title="Edit Subscription" %> <%@ Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">
  <div class="pageHeader">
    <div class="caption">
      <h1>
        <asp:Localize
          ID="Caption"
          runat="server"
          Text="Subscription Plan for '{0}'"
        ></asp:Localize>
      </h1>
    </div>
  </div>
  <ajax:UpdatePanel ID="PageAjax" runat="server" UpdateMode="conditional">
    <ContentTemplate>
      <asp:Panel ID="NoSubscriptionPlanPanel" runat="server">
        <p>
          <asp:Localize
            ID="NoSubscriptionPlanDescription"
            runat="server"
            Text="This product does not have any associated subscription plan."
            EnableViewState="False"
          ></asp:Localize>
        </p>
        <asp:Button
          ID="ShowAddForm"
          runat="server"
          Text="Add Subscription Plan"
          OnClick="ShowAddForm_Click"
          EnableViewState="false"
        />
      </asp:Panel>
      <asp:Panel ID="SubscriptionPlanForm" runat="server" Visible="false">
        <table cellpadding="5" cellspacing="0" class="inputForm">
          <tr>
            <td colspan="6">
              <asp:Label
                ID="SavedMessage"
                runat="server"
                Text="Subscription plan saved at {0}."
                EnableViewState="false"
                Visible="false"
                SkinID="GoodCondition"
              ></asp:Label>
              <asp:Label
                ID="ErrorMessageLabel"
                runat="server"
                Text=""
                EnableViewState="false"
                Visible="false"
                SkinID="ErrorCondition"
              ></asp:Label>
            </td>
          </tr>
          <tr>
            <th>
              <cb:ToolTipLabel
                ID="ToolTipLabel1"
                runat="server"
                Text="Payment"
                ToolTip="Which payment in the cycle (Initial,Trial,Recurring)."
                EnableViewState="false"
              ></cb:ToolTipLabel>
            </th>
            <th>
              <cb:ToolTipLabel
                ID="ToolTipLabel2"
                runat="server"
                Text="Amount"
                ToolTip="How much to charge this cycle (excluding shipping)"
                EnableViewState="false"
              ></cb:ToolTipLabel>
            </th>
            <th>
              <cb:ToolTipLabel
                ID="ToolTipLabel3"
                runat="server"
                Text="Ship Method"
                ToolTip="Ship Method to use."
                EnableViewState="false"
              ></cb:ToolTipLabel>
            </th>
            <th width="85px">
              <cb:ToolTipLabel
                ID="ToolTipLabel4"
                runat="server"
                Text="Split Ship"
                ToolTip="Split Amount & Shipping into 2 charges"
                EnableViewState="false"
              ></cb:ToolTipLabel>
            </th>
            <th width="100px">
              <cb:ToolTipLabel
                ID="ToolTipLabel5"
                runat="server"
                Text="Payment Days"
                ToolTip="Number of days to charge from Trial charge, if recurring then from it's last charge."
                EnableViewState="false"
              ></cb:ToolTipLabel>
            </th>
            <th>
              <cb:ToolTipLabel
                ID="ToolTipLabel7"
                runat="server"
                Text="Capture"
                ToolTip="Days to Capture"
                EnableViewState="false"
              ></cb:ToolTipLabel>
            </th>

            <th width="100px">
              <cb:ToolTipLabel
                ID="ToolTipLabel6"
                runat="server"
                Text="Number of Payments"
                ToolTip="Number of times to do this charge. 0 = no end."
                EnableViewState="false"
              ></cb:ToolTipLabel>
            </th>
          </tr>
          <tr>
            <td style="text-align: right">Initial</td>
            <td>
              <asp:TextBox
                runat="server"
                Style="text-align: right;"
                ID="tbiAmount"
                MaxLength="6"
                EnableViewState="false"
                Text="{0:0.00}"
                Width="50px"
              />
              <asp:RequiredFieldValidator
                ID="RequiredFieldValidator1"
                runat="server"
                Text="*"
                Display="Dynamic"
                ErrorMessage="You must enter an amount for the Initial charge."
                ControlToValidate="tbiAmount"
                EnableViewState="false"
              ></asp:RequiredFieldValidator>
              <asp:RegularExpressionValidator
                ID="RegularExpressionValidator1"
                runat="server"
                Text="*"
                Display="dynamic"
                ErrorMessage="Initial Amount invalid."
                ControlToValidate="tbiAmount"
                EnableViewState="false"
                ValidationExpression="\d+\.\d{2}"
              ></asp:RegularExpressionValidator>
            </td>
            <td>
              <asp:DropDownList
                runat="server"
                ID="ddliShipMethods"
                DataTextField="Name"
                DataValueField="ShipMethodId"
              />
            </td>

            <td align="center">
              <asp:CheckBox Checked="false" runat="server" ID="cbiSplit" />
            </td>
            <td>
              <asp:TextBox
                Style="text-align: right;"
                runat="server"
                ID="tbiPaymentDays"
                ReadOnly="true"
                EnableViewState="false"
                Text="0"
                Width="50px"
              />
              <asp:RequiredFieldValidator
                ID="RequiredFieldValidator4"
                runat="server"
                Text="*"
                Display="dynamic"
                ErrorMessage="You must enter an amount for the Initial days."
                ControlToValidate="tbiPaymentDays"
                EnableViewState="false"
              ></asp:RequiredFieldValidator>
              <asp:RegularExpressionValidator
                ID="RegularExpressionValidator4"
                runat="server"
                Text="*"
                Display="dynamic"
                ErrorMessage="Initial Payment Days invalid."
                ControlToValidate="tbiPaymentDays"
                EnableViewState="false"
                ValidationExpression="\d"
              ></asp:RegularExpressionValidator>
            </td>
            <td>
              <asp:TextBox
                Style="text-align: right;"
                runat="server"
                ID="tbiDaysToCapture"
                EnableViewState="false"
                Text="0"
                Width="50px"
              />
              <asp:RequiredFieldValidator
                ID="RequiredFieldValidator10"
                runat="server"
                Text="*"
                Display="dynamic"
                ErrorMessage="You must enter an amount for the Days to Capture."
                ControlToValidate="tbiDaysToCapture"
                EnableViewState="false"
              ></asp:RequiredFieldValidator>
              <asp:RegularExpressionValidator
                ID="RegularExpressionValidator10"
                runat="server"
                Text="*"
                Display="dynamic"
                ErrorMessage="Initial Days to Capture invalid."
                ControlToValidate="tbiDaysToCapture"
                EnableViewState="false"
                ValidationExpression="\d"
              ></asp:RegularExpressionValidator>
            </td>
            <td>
              <asp:TextBox
                Style="text-align: right;"
                runat="server"
                ID="tbiNOP"
                ReadOnly="true"
                EnableViewState="false"
                Text="1"
                Width="50px"
              />
              <asp:RequiredFieldValidator
                ID="RequiredFieldValidator7"
                runat="server"
                Text="*"
                Display="dynamic"
                ErrorMessage="You must enter an amount for the Initial payments."
                ControlToValidate="tbiNOP"
                EnableViewState="false"
              ></asp:RequiredFieldValidator>
              <asp:RegularExpressionValidator
                ID="RegularExpressionValidator7"
                runat="server"
                Text="*"
                Display="dynamic"
                ErrorMessage="Initial Payments invalid."
                ControlToValidate="tbiNOP"
                EnableViewState="false"
                ValidationExpression="\d"
              ></asp:RegularExpressionValidator>
            </td>
          </tr>
          <tr>
            <td style="text-align: right">Trial</td>
            <td>
              <asp:TextBox
                runat="server"
                Style="text-align: right;"
                ID="tbtAmount"
                MaxLength="6"
                EnableViewState="false"
                Text="{0:0.00}"
                Width="50px"
              />
              <asp:RequiredFieldValidator
                ID="RequiredFieldValidator2"
                runat="server"
                Text="*"
                Display="dynamic"
                ErrorMessage="You must enter an amount for the Trial charge."
                ControlToValidate="tbtAmount"
                EnableViewState="false"
              ></asp:RequiredFieldValidator>
              <asp:RegularExpressionValidator
                ID="RegularExpressionValidator2"
                runat="server"
                Text="*"
                Display="dynamic"
                ErrorMessage="Trial Amount invalid."
                ControlToValidate="tbtAmount"
                EnableViewState="false"
                ValidationExpression="\d+\.\d{2}"
              ></asp:RegularExpressionValidator>
            </td>
            <td>
              <asp:DropDownList
                runat="server"
                ID="ddltShipMethods"
                DataTextField="Name"
                DataValueField="ShipMethodId"
              />
            </td>

            <td align="center">
              <asp:CheckBox Checked="false" runat="server" ID="cbtSplit" />
            </td>
            <td>
              <asp:TextBox
                Style="text-align: right;"
                runat="server"
                ID="tbtPaymentDays"
                EnableViewState="false"
                Text="0"
                Width="50px"
              />
              <asp:RequiredFieldValidator
                ID="RequiredFieldValidator5"
                runat="server"
                Text="*"
                Display="dynamic"
                ErrorMessage="You must enter an amount for the Trial days."
                ControlToValidate="tbtPaymentDays"
                EnableViewState="false"
              ></asp:RequiredFieldValidator>
              <asp:RegularExpressionValidator
                ID="RegularExpressionValidator5"
                runat="server"
                Text="*"
                Display="dynamic"
                ErrorMessage="Trial Payment Days invalid."
                ControlToValidate="tbtPaymentDays"
                EnableViewState="false"
                ValidationExpression="\d+"
              ></asp:RegularExpressionValidator>
            </td>
            <td>
              <asp:TextBox
                Style="text-align: right;"
                runat="server"
                ID="tbtDaysToCapture"
                EnableViewState="false"
                Text="8"
                Width="50px"
              />
              <asp:RequiredFieldValidator
                ID="RequiredFieldValidator11"
                runat="server"
                Text="*"
                Display="dynamic"
                ErrorMessage="You must enter an amount for the Trial Days to Capture."
                ControlToValidate="tbtDaysToCapture"
                EnableViewState="false"
              ></asp:RequiredFieldValidator>
              <asp:RegularExpressionValidator
                ID="RegularExpressionValidator11"
                runat="server"
                Text="*"
                Display="dynamic"
                ErrorMessage="Trial Days to Capture invalid."
                ControlToValidate="tbtDaysToCapture"
                EnableViewState="false"
                ValidationExpression="\d"
              ></asp:RegularExpressionValidator>
            </td>
            <td>
              <asp:TextBox
                Style="text-align: right;"
                runat="server"
                ID="tbtNOP"
                ReadOnly="true"
                EnableViewState="false"
                Text="1"
                Width="50px"
              />
              <asp:RequiredFieldValidator
                ID="RequiredFieldValidator8"
                runat="server"
                Text="*"
                Display="dynamic"
                ErrorMessage="You must enter an amount for the Trial payments."
                ControlToValidate="tbtNOP"
                EnableViewState="false"
              ></asp:RequiredFieldValidator>
              <asp:RegularExpressionValidator
                ID="RegularExpressionValidator8"
                runat="server"
                Text="*"
                Display="dynamic"
                ErrorMessage="Trial Payments invalid."
                ControlToValidate="tbtNOP"
                EnableViewState="false"
                ValidationExpression="\d+"
              ></asp:RegularExpressionValidator>
            </td>
          </tr>
          <tr>
            <td style="text-align: right">Recurring</td>
            <td>
              <asp:TextBox
                runat="server"
                Style="text-align: right;"
                ID="tbrAmount"
                MaxLength="6"
                EnableViewState="false"
                Text="{0:0.00}"
                Width="50px"
              />
              <asp:RequiredFieldValidator
                ID="RequiredFieldValidator3"
                runat="server"
                Text="*"
                Display="dynamic"
                ErrorMessage="You must enter an amount for the Recurring charge."
                ControlToValidate="tbrAmount"
                EnableViewState="false"
              ></asp:RequiredFieldValidator>
              <asp:RegularExpressionValidator
                ID="RegularExpressionValidator3"
                runat="server"
                Text="*"
                Display="dynamic"
                ErrorMessage="Recurring Amount invalid."
                ControlToValidate="tbrAmount"
                EnableViewState="false"
                ValidationExpression="\d+\.\d{2}"
              ></asp:RegularExpressionValidator>
            </td>
            <td>
              <asp:DropDownList
                runat="server"
                ID="ddlrShipMethods"
                DataTextField="Name"
                DataValueField="ShipMethodId"
              />
            </td>

            <td align="center">
              <asp:CheckBox Checked="false" runat="server" ID="cbrSplit" />
            </td>
            <td>
              <asp:TextBox
                Style="text-align: right;"
                runat="server"
                ID="tbrPaymentDays"
                EnableViewState="false"
                Width="50px"
              />
              <asp:RequiredFieldValidator
                ID="RequiredFieldValidator6"
                runat="server"
                Text="*"
                Display="dynamic"
                ErrorMessage="You must enter an amount for the Recurring days."
                ControlToValidate="tbrPaymentDays"
                EnableViewState="false"
              ></asp:RequiredFieldValidator>
              <asp:RegularExpressionValidator
                ID="RegularExpressionValidator6"
                runat="server"
                Text="*"
                Display="dynamic"
                ErrorMessage="Recurring Payment Days invalid."
                ControlToValidate="tbrPaymentDays"
                EnableViewState="false"
                ValidationExpression="\d+"
              ></asp:RegularExpressionValidator>
            </td>
            <td>
              <asp:TextBox
                Style="text-align: right;"
                runat="server"
                ID="tbrDaysToCapture"
                EnableViewState="false"
                Text=""
                Width="50px"
              />
              <asp:RequiredFieldValidator
                ID="RequiredFieldValidator12"
                runat="server"
                Text="*"
                Display="dynamic"
                ErrorMessage="You must enter an amount for the Recurring Days to Capture."
                ControlToValidate="tbrDaysToCapture"
                EnableViewState="false"
              ></asp:RequiredFieldValidator>
              <asp:RegularExpressionValidator
                ID="RegularExpressionValidator12"
                runat="server"
                Text="*"
                Display="dynamic"
                ErrorMessage="Recurring Days to Capture invalid."
                ControlToValidate="tbrDaysToCapture"
                EnableViewState="false"
                ValidationExpression="\d"
              ></asp:RegularExpressionValidator>
            </td>
            <td>
              <asp:TextBox
                Style="text-align: right;"
                runat="server"
                ID="tbrNOP"
                EnableViewState="false"
                Width="50px"
              />
              <asp:RequiredFieldValidator
                ID="RequiredFieldValidator9"
                runat="server"
                Text="*"
                Display="dynamic"
                ErrorMessage="You must enter an amount for the Recurring payments."
                ControlToValidate="tbrNOP"
                EnableViewState="false"
              ></asp:RequiredFieldValidator>
              <asp:RegularExpressionValidator
                ID="RegularExpressionValidator9"
                runat="server"
                Text="*"
                Display="dynamic"
                ErrorMessage="Recurring Payments invalid."
                ControlToValidate="tbrNOP"
                EnableViewState="false"
                ValidationExpression="\d+"
              ></asp:RegularExpressionValidator>
            </td>
          </tr>
          <tr>
            <td>&nbsp;</td>
            <td colspan="5">
              <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
              <asp:Panel
                ID="AddSubscriptionPlanButtons"
                runat="server"
                Visible="false"
              >
                <asp:Button
                  ID="AddButton"
                  CssClass="button"
                  runat="server"
                  Text="Save"
                  OnClick="AddButton_Click"
                  EnableViewState="false"
                  TabIndex="11"
                />
                <asp:Button
                  ID="CancelButton"
                  CssClass="button"
                  runat="server"
                  Text="Cancel"
                  OnClick="CancelButton_Click"
                  CausesValidation="false"
                  EnableViewState="false"
                  TabIndex="10"
                />
              </asp:Panel>
              <asp:Panel
                ID="EditSubscriptionPlanButtons"
                runat="server"
                Visible="false"
              >
                <asp:Button
                  ID="DeleteButton"
                  CssClass="button"
                  runat="server"
                  Text="Delete"
                  OnClick="DeleteButton_Click"
                  CausesValidation="false"
                  EnableViewState="false"
                  TabIndex="10"
                  OnClientClick="return confirm('Are you sure you want to delete this subscription plan?')"
                />
                <asp:Button
                  ID="UpdateButton"
                  CssClass="button"
                  runat="server"
                  Text="Save"
                  OnClick="UpdateButton_Click"
                  EnableViewState="false"
                  TabIndex="11"
                  Style="height: 26px"
                />
              </asp:Panel>
            </td>
          </tr>
        </table>
      </asp:Panel>
    </ContentTemplate>
  </ajax:UpdatePanel>
</asp:Content>
