<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"
CodeFile="ProductReviewSettings.aspx.cs"
Inherits="Admin_Store_ProductReviewSettings" Title="Configure Product Reviews"
EnableViewState="false" %> <%@ Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
  <div class="pageHeader">
    <div class="caption">
      <h1>
        <asp:Localize
          ID="Caption"
          runat="server"
          Text="Configure Product Reviews"
        ></asp:Localize>
      </h1>
    </div>
  </div>
  <table cellpadding="2" cellspacing="0" class="innerLayout">
    <tr>
      <td>
        <ajax:UpdatePanel ID="PageAjax" runat="server" UpdateMode="Conditional">
          <ContentTemplate>
            <table class="inputForm">
              <tr>
                <th class="rowHeader" style="vertical-align: top">
                  <cb:ToolTipLabel
                    ID="EnabledLabel"
                    runat="server"
                    Text="Enable Product Reviews:"
                    AssociatedControlID="Enabled"
                    ToolTip="Indicates whether the product review feature is enabled for the store."
                  ></cb:ToolTipLabel>
                </th>
                <td>
                  <asp:DropDownList ID="Enabled" runat="server">
                    <asp:ListItem Value="0" Text="No"></asp:ListItem>
                    <asp:ListItem
                      Value="2"
                      Text="Registered Users Only"
                    ></asp:ListItem>
                    <asp:ListItem Value="3" Text="All Users"></asp:ListItem>
                  </asp:DropDownList>
                </td>
              </tr>
              <tr>
                <th class="rowHeader">
                  <cb:ToolTipLabel
                    ID="ApprovalLabel"
                    runat="server"
                    Text="Require Approval for Reviews:"
                    AssociatedControlID="Approval"
                    ToolTip="Indicates whether product reviews made by a customer must be approved by the merchant before being published."
                  />
                </th>
                <td>
                  <asp:DropDownList ID="Approval" runat="server">
                    <asp:ListItem Value="0" Text="No"></asp:ListItem>
                    <asp:ListItem
                      Value="1"
                      Text="Anonymous Users Only"
                    ></asp:ListItem>
                    <asp:ListItem Value="3" Text="All Users"></asp:ListItem>
                  </asp:DropDownList>
                </td>
              </tr>
              <tr>
                <th class="rowHeader">
                  <cb:ToolTipLabel
                    ID="ImageVerificationLabel"
                    runat="server"
                    Text="Use Image Verification"
                    AssociatedControlID="ImageVerification"
                    ToolTip="Indicates whether a verification image will be displayed for customers adding a review.  Using the verification image can help reduce review spamming."
                  />
                </th>
                <td>
                  <asp:DropDownList ID="ImageVerification" runat="server">
                    <asp:ListItem Value="0" Text="No"></asp:ListItem>
                    <asp:ListItem
                      Value="1"
                      Text="Anonymous Users Only"
                    ></asp:ListItem>
                    <asp:ListItem Value="3" Text="All Users"></asp:ListItem>
                  </asp:DropDownList>
                </td>
              </tr>
              <tr>
                <th class="rowHeader">
                  <cb:ToolTipLabel
                    ID="EmailVerificationLabel"
                    runat="server"
                    Text="Use Email Verification"
                    AssociatedControlID="ImageVerification"
                    ToolTip="Indicates whether an email will be sent to the customer with a link that must be clicked to publish the review.  Using email verification can help reduce review spamming."
                  />
                </th>
                <td>
                  <asp:DropDownList ID="EmailVerification" runat="server">
                    <asp:ListItem Value="0" Text="No"></asp:ListItem>
                    <asp:ListItem
                      Value="1"
                      Text="Anonymous Users Only"
                    ></asp:ListItem>
                    <asp:ListItem Value="3" Text="All Users"></asp:ListItem>
                  </asp:DropDownList>
                </td>
              </tr>
              <tr>
                <th class="rowHeader" style="vertical-align: top">
                  <cb:ToolTipLabel
                    ID="EmailTemplateLabel"
                    runat="server"
                    Text="Email Verification Message"
                    AssociatedControlID="EmailTemplate"
                    ToolTip="If you are using email verification, select the message that will be sent to the customer when a review is submitted.  If you do not wish to customize the message, choose 'Default Message' to use a simple default message."
                  />
                </th>
                <td>
                  <asp:DropDownList
                    ID="EmailTemplate"
                    runat="server"
                    DataTextField="Name"
                    DataValueField="EmailTemplateId"
                    AppendDataBoundItems="true"
                  >
                    <asp:ListItem
                      Value=""
                      Text="Default Message"
                    ></asp:ListItem>
                  </asp:DropDownList>
                </td>
              </tr>
              <tr>
                <th class="rowHeader" style="vertical-align: top">
                  <cb:ToolTipLabel
                    ID="TermsAndConditionsLabel"
                    runat="server"
                    Text="Review Terms and Conditions"
                    AssociatedControlID="TermsAndConditions"
                    ToolTip="Provide the terms and conditions or review posting policy that will be shown to customers posting a review."
                  />
                </th>
                <td>
                  <asp:TextBox
                    ID="TermsAndConditions"
                    runat="server"
                    TextMode="MultiLine"
                    Rows="6"
                    Width="400"
                  ></asp:TextBox>
                </td>
              </tr>
              <tr>
                <td align="center" colspan="2">
                  <br />
                  <asp:ValidationSummary
                    ID="ValidationSummary1"
                    runat="server"
                  />
                  <asp:Label
                    ID="SavedMessage"
                    runat="server"
                    Text="Saved at {0:t}<br /><br />"
                    SkinID="GoodCondition"
                    EnableViewState="False"
                    Visible="false"
                  ></asp:Label>
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
                </td>
              </tr>
            </table>
          </ContentTemplate>
        </ajax:UpdatePanel>
      </td>
    </tr>
  </table>
</asp:Content>
