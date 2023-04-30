<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"
CodeFile="BlockedIPs.aspx.cs" Inherits="Admin_Store_Security_BlockedIPs"
Title="Blocked IPs" %> <%@ Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
  <div class="pageHeader">
    <div class="caption">
      <h1>
        <asp:Localize
          ID="Caption"
          runat="server"
          Text="Blocked IPs"
        ></asp:Localize>
      </h1>
    </div>
  </div>
  <table cellpadding="2" cellspacing="0" class="innerLayout">
    <tr>
      <td width="50%" align="left" valign="top">
        <ajax:UpdatePanel ID="GridAjax" runat="server" UpdateMode="Conditional">
          <ContentTemplate>
            <asp:GridView
              ID="BannedIPGrid"
              runat="server"
              AllowPaging="true"
              AllowSorting="false"
              PageSize="20"
              AutoGenerateColumns="False"
              DataKeyNames="BannedIPId"
              DataSourceID="BannedIPDs"
              ShowFooter="False"
              SkinID="PagedList"
              Width="400"
            >
              <Columns>
                <asp:TemplateField HeaderText="Range Start">
                  <ItemTemplate>
                    <asp:Label
                      ID="IPRangeStart"
                      runat="server"
                      Text='<%# Eval("DottedIPRangeStart") %>'
                    ></asp:Label>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Range End">
                  <ItemTemplate>
                    <asp:Label
                      ID="IPRangeEnd"
                      runat="server"
                      Text='<%# Eval("DottedIPRangeEnd") %>'
                    ></asp:Label>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Comment">
                  <ItemTemplate>
                    <asp:Label
                      ID="Comment"
                      runat="server"
                      Text='<%# Eval("Comment") %>'
                    ></asp:Label>
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="False">
                  <ItemStyle Wrap="false" HorizontalAlign="center" />
                  <ItemTemplate>
                    <asp:LinkButton
                      ID="DeleteButton"
                      runat="server"
                      CausesValidation="False"
                      CommandName="Delete"
                      OnClientClick="return confirm('Are you sure you want to delete this range?')"
                      ><asp:Image
                        ID="DeleteIcon"
                        runat="server"
                        SkinID="DeleteIcon"
                    /></asp:LinkButton>
                  </ItemTemplate>
                </asp:TemplateField>
              </Columns>
              <EmptyDataTemplate>
                <asp:Label
                  ID="EmptyDataText"
                  runat="server"
                  Text="You can use IP blocking to help prevent fraud or to stop unwanted search engines from visiting your store."
                ></asp:Label>
              </EmptyDataTemplate>
            </asp:GridView>
          </ContentTemplate>
        </ajax:UpdatePanel>
      </td>
      <td width="50%" valign="top">
        <div class="section">
          <div class="header">
            <h2 class="block">
              <asp:Localize
                ID="AddCaption"
                runat="server"
                Text="Block IP Range"
              />
            </h2>
          </div>
          <div class="content">
            <asp:Label
              ID="AddHelpText"
              runat="server"
              Text="Enter addresses in dotted notation like <b>127.0.0.1</b>.  To block a single address, leave the end field blank."
            ></asp:Label>
            <ajax:UpdatePanel
              ID="AddAjax"
              runat="server"
              UpdateMode="Conditional"
            >
              <ContentTemplate>
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
                <asp:Label
                  ID="AddedMessage"
                  runat="server"
                  Text="New block added.<br />"
                  SkinID="GoodCondition"
                  Visible="false"
                ></asp:Label>
                <table class="inputForm" cellpadding="4" cellspacing="0">
                  <tr>
                    <th class="rowHeader">
                      <cb:ToolTipLabel
                        ID="AddIPRangeStartLabel"
                        runat="server"
                        Text="Start:"
                        ToolTip="The first IP in the range to block."
                      />
                    </th>
                    <td runat="server">
                      <asp:TextBox
                        ID="AddIPRangeStart"
                        runat="server"
                        TabIndex="1"
                      ></asp:TextBox>
                      <asp:RequiredFieldValidator
                        ID="AddBannedIPNameRequired"
                        runat="server"
                        ControlToValidate="AddIPRangeStart"
                        Display="Static"
                        ErrorMessage="You must enter an address in the start field."
                        Text="*"
                      ></asp:RequiredFieldValidator>
                    </td>
                  </tr>
                  <tr>
                    <th class="rowHeader">
                      <cb:ToolTipLabel
                        ID="AddIPRangeEndLabel"
                        runat="server"
                        Text="End:"
                        ToolTip="The last IP in the range to block; leave blank to only block the IP listed in the start field."
                      />
                    </th>
                    <td runat="server">
                      <asp:TextBox
                        ID="AddIPRangeEnd"
                        runat="server"
                        TabIndex="2"
                      ></asp:TextBox>
                    </td>
                  </tr>
                  <tr>
                    <th class="rowHeader">
                      <cb:ToolTipLabel
                        ID="AddCommentLabel"
                        runat="server"
                        Text="Comment:"
                        ToolTip="Optional comment to attach to the banned IP range."
                      />
                    </th>
                    <td>
                      <asp:TextBox
                        ID="AddComment"
                        runat="server"
                        TabIndex="3"
                        MaxLength="100"
                      ></asp:TextBox>
                    </td>
                  </tr>
                  <tr>
                    <td>&nbsp;</td>
                    <td>
                      <asp:Button
                        ID="AddButton"
                        runat="server"
                        Text="Add"
                        TabIndex="4"
                        OnClick="AddButton_Click"
                      />
                    </td>
                  </tr>
                </table>
              </ContentTemplate>
            </ajax:UpdatePanel>
          </div>
        </div>
      </td>
    </tr>
  </table>
  <asp:ObjectDataSource
    ID="BannedIPDs"
    runat="server"
    OldValuesParameterFormatString="original_{0}"
    SelectMethod="LoadForStore"
    TypeName="MakerShop.Stores.BannedIPDataSource"
    SelectCountMethod="CountForStore"
    SortParameterName="sortExpression"
    DataObjectTypeName="MakerShop.Stores.BannedIP"
    DeleteMethod="Delete"
    UpdateMethod="Update"
  >
  </asp:ObjectDataSource>
</asp:Content>
