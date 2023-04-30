<%@ Page Language="C#" MasterPageFile="../../Admin.master"
CodeFile="GatewayFailover.aspx.cs" Inherits="Admin_Payment_GatewayFailover"
Title="Gateway Failover" %> <%@ Register Src="AddGatewayFailoverDialog.ascx"
TagName="AddDialog" TagPrefix="uc" %> <%@ Register
Src="EditGatewayFailoverDialog.ascx" TagName="EditDialog" TagPrefix="uc" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
  <ajax:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
      <div class="pageHeader">
        <div class="caption">
          <h1>
            <asp:Localize
              ID="Caption"
              runat="server"
              Text="Payment Gateway Failover"
            ></asp:Localize>
          </h1>
        </div>
      </div>
      <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
          <td width="50%" valign="top" class="itemList">
            <div>
              <asp:GridView
                ID="Grid"
                runat="server"
                DataKeyNames="PaymentGatewayFailoverId"
                DataSourceID="Ds"
                AutoGenerateColumns="false"
                width="100%"
                SkinID="PagedList"
                OnRowEditing="Grid_RowEditing"
                OnRowCancelingEdit="Grid_RowCancelingEdit"
                OnRowCommand="Grid_RowCommand"
              >
                <Columns>
                  <asp:BoundField
                    DataField="Name"
                    HeaderText="Name"
                    HeaderStyle-HorizontalAlign="center"
                    ReadOnly="true"
                  />
                  <asp:TemplateField>
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                      <asp:ImageButton
                        ID="EditButton"
                        runat="server"
                        CausesValidation="False"
                        CommandName="Edit"
                        SkinID="EditIcon"
                        AlternateText="Edit"
                      />
                      <asp:ImageButton
                        ID="DeleteButton"
                        runat="server"
                        CausesValidation="False"
                        CommandName="Delete"
                        SkinID="DeleteIcon"
                        OnClientClick='<%#Eval("Name", "return confirm(\"Are you sure you want to delete {0}?\")") %>'
                        AlternateText="Delete"
                      />
                    </ItemTemplate>
                    <EditItemTemplate>
                      <asp:ImageButton
                        ID="CancelButton"
                        runat="server"
                        CausesValidation="False"
                        CommandName="Cancel"
                        SkinID="CancelIcon"
                        AlternateText="Cancel"
                      />
                    </EditItemTemplate>
                  </asp:TemplateField>
                </Columns>
              </asp:GridView>
            </div>
          </td>
          <td width="50%" class="detailPanel" style="padding-top: -10px">
            <asp:Panel ID="AddPanel" runat="server" CssClass="section">
              <div class="header">
                <h2 class="addpaymentmethod">
                  <asp:Localize
                    ID="AddCaption"
                    runat="server"
                    Text="Add Gateway Template"
                  />
                </h2>
              </div>
              <div class="content">
                <uc:AddDialog ID="AddDialog1" runat="server" />
              </div>
            </asp:Panel>
            <asp:Panel
              ID="EditPanel"
              runat="server"
              CssClass="section"
              Visible="false"
            >
              <div class="header">
                <h2>
                  <asp:Localize
                    ID="EditCaption"
                    runat="server"
                    Text="Edit '{0}'"
                    EnableViewState="false"
                  />
                </h2>
              </div>
              <div class="content">
                <uc:EditDialog
                  ID="EditDialog1"
                  runat="server"
                  OnItemUpdated="EditDialog1_ItemUpdated"
                  OnCancelled="EditDialog1_Cancelled"
                />
              </div>
            </asp:Panel>
          </td>
        </tr>
      </table>
    </ContentTemplate>
  </ajax:UpdatePanel>
  <asp:ObjectDataSource
    ID="Ds"
    runat="server"
    OldValuesParameterFormatString="original_{0}"
    SelectMethod="LoadForCriteria"
    TypeName="MakerShop.Payments.PaymentGatewayFailoverDataSource"
    SelectCountMethod="CountForCriteria"
    SortParameterName="sortExpression"
    DataObjectTypeName="MakerShop.Payments.PaymentGatewayFailover"
    DeleteMethod="Delete"
  >
    <SelectParameters>
      <asp:Parameter Name="sqlCriteria" DefaultValue="" />
    </SelectParameters>
  </asp:ObjectDataSource>
</asp:Content>
