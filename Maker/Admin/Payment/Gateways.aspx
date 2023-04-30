<%@ Page Language="C#" MasterPageFile="../Admin.master"
CodeFile="Gateways.aspx.cs" Inherits="Admin_Payment_Gateways" Title="Payment
Gateways" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
  <div class="pageHeader">
    <div class="caption">
      <h1>
        <asp:Localize
          ID="Caption"
          runat="server"
          Text="Payment Gateways"
        ></asp:Localize>
      </h1>
    </div>
  </div>
  <asp:UpdatePanel runat="server" ID="up">
    <ContentTemplate>
      <div
        class="rowHeader"
        style="
          margin-left: auto;
          margin-right: auto;
          width: 100%;
          text-align: center;
        "
      >
        <cb:ToolTipLabel
          ID="ttActive"
          runat="server"
          Text="Show InActive:"
          ToolTip="Select this if you want to reactivate a gateway."
        >
        </cb:ToolTipLabel>
        <asp:CheckBox
          AutoPostBack="true"
          ID="cbShowActive"
          runat="server"
          Checked="False"
        />
      </div>
      <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
          <td valign="top" align="Center">
            Selected Gateways&nbsp;<asp:DropDownList
              ID="GatewayNames"
              AutoPostBack="true"
              runat="server"
            >
            </asp:DropDownList>
          </td>
        </tr>
        <tr>
          <td>&nbsp;</td>
        </tr>
        <tr>
          <td valign="top" align="center">
            <asp:HyperLink
              ID="AddGateway"
              runat="server"
              Text="Add Gateway"
              NavigateUrl="AddGateway.aspx"
              SkinID="Button"
            ></asp:HyperLink>
            <asp:HyperLink
              ID="MethodsLink"
              runat="server"
              Text="Edit Methods"
              NavigateUrl="Methods.aspx"
              SkinID="Button"
            ></asp:HyperLink>
            <br /> <br />
            <asp:GridView
              ID="GatewayGrid"
              runat="server"
              DataKeyNames="PaymentGatewayId"
              DataSourceID="PaymentGatewayDs"
              AutoGenerateColumns="false"
              Width="450px"
              SkinID="PagedList"
              OnRowCommand="GatewayGrid_RowCommand"
            >
              <Columns>
                <asp:BoundField
                  DataField="Name"
                  HeaderText="Name"
                  HeaderStyle-HorizontalAlign="left"
                  ReadOnly="true"
                  ItemStyle-HorizontalAlign="left"
                />
                <asp:TemplateField HeaderText="Payment Methods">
                  <HeaderStyle HorizontalAlign="left" />
                  <ItemStyle HorizontalAlign="Left" />
                  <ItemTemplate>
                    <asp:Label
                      ID="PaymentMethods"
                      runat="server"
                      Text="<%#GetPaymentMethods(Container.DataItem)%>"
                    ></asp:Label
                    ><br />
                  </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                  <ItemStyle
                    HorizontalAlign="Center"
                    Width="54px"
                    Wrap="false"
                  />
                  <ItemTemplate>
                    <asp:HyperLink
                      ID="EditButton"
                      runat="server"
                      NavigateUrl='<%#Eval("PaymentGatewayId", "EditGateway.aspx?PaymentGatewayId={0}") %>'
                    >
                      <asp:Image
                        ID="EditIcon"
                        runat="server"
                        SkinID="EditIcon"
                        AlternateText="Edit"
                    /></asp:HyperLink>
                    <asp:ImageButton
                      ID="DeleteButton"
                      runat="server"
                      CausesValidation="False"
                      CommandArgument='<% #Eval("PaymentGatewayId") %>'
                      OnClientClick='<%#Eval("Name", "return confirm(\"Are you sure you want to delete {0}?\")") %>'
                      CommandName="Delete"
                      SkinID="DeleteIcon"
                      AlternateText="Delete"
                    />
                  </ItemTemplate>
                </asp:TemplateField>
              </Columns>
              <EmptyDataTemplate>
                <asp:Label
                  ID="EmptyMessage"
                  runat="server"
                  Text="No gateways are defined for your store."
                ></asp:Label>
              </EmptyDataTemplate>
            </asp:GridView>
          </td>
        </tr>
      </table>
    </ContentTemplate>
  </asp:UpdatePanel>
  <asp:ObjectDataSource
    ID="PaymentGatewayDs"
    runat="server"
    OldValuesParameterFormatString="original_{0}"
    SelectMethod="LoadForCriteria"
    TypeName="MakerShop.Payments.PaymentGatewayDataSource"
    SelectCountMethod="CountForCriteria"
    SortParameterName="sortExpression"
    DataObjectTypeName="MakerShop.Payments.PaymentGateway"
    DeleteMethod="Delete"
  >
    <SelectParameters>
      <asp:Parameter Name="sqlCriteria" DefaultValue="" />
    </SelectParameters>
  </asp:ObjectDataSource>
</asp:Content>
