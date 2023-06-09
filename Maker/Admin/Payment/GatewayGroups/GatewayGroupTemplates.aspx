﻿<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"
AutoEventWireup="true" CodeFile="GatewayGroupTemplates.aspx.cs"
Inherits="Admin_Payment_GatewayGroups_GatewayGroupTemplates" Title="Untitled
Page" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
  <div class="pageHeader">
    <div class="caption">
      <h1>
        <asp:Localize
          ID="Caption"
          runat="server"
          Text="Payment Group Gateways Templates"
        ></asp:Localize>
      </h1>
    </div>
  </div>
  <table cellpadding="2" cellspacing="0" class="innerLayout">
    <tr>
      <td valign="top" align="center">
        <asp:GridView
          ID="GatewayGrid"
          runat="server"
          DataKeyNames="PaymentGatewayGroupId"
          DataSourceID="PaymentGatewaysGroupDs"
          AutoGenerateColumns="false"
          Width="450px"
          SkinID="PagedList"
        >
          <Columns>
            <asp:BoundField
              DataField="PaymentGatewayGroup"
              HeaderText="Gateway Group"
              HeaderStyle-HorizontalAlign="left"
              ReadOnly="true"
              ItemStyle-HorizontalAlign="left"
            />
            <asp:TemplateField HeaderText="Payment Gateways">
              <HeaderStyle HorizontalAlign="left" />
              <ItemStyle HorizontalAlign="Left" />
              <ItemTemplate>
                <asp:Label
                  ID="Gateways"
                  runat="server"
                  Text="<%#GetGroupGateways(Container.DataItem)%>"
                ></asp:Label
                ><br />
              </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
              <ItemStyle HorizontalAlign="Center" Width="54px" Wrap="false" />
              <ItemTemplate>
                <asp:HyperLink
                  ID="EditButton"
                  runat="server"
                  NavigateUrl='<%#Eval("PaymentGatewayGroupId", "EditGatewayGroupTemplate.aspx?PaymentGatewayGroupId={0}") %>'
                  ><asp:Image
                    ID="EditIcon"
                    runat="server"
                    SkinID="EditIcon"
                    AlternateText="Edit"
                /></asp:HyperLink>
                <asp:ImageButton
                  ID="DeleteButton"
                  runat="server"
                  CausesValidation="False"
                  CommandName="Delete"
                  SkinID="DeleteIcon"
                  OnClientClick='<%#Eval("PaymentGatewayGroup", "return confirm(\"Are you sure you want to delete {0}?\")") %>'
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
        <br />

        <asp:HyperLink
          ID="MethodsLink"
          runat="server"
          Text="Add/Edit Template Name"
          NavigateUrl="Templates.aspx"
          SkinID="Button"
        ></asp:HyperLink>
      </td>
    </tr>
  </table>
  <asp:ObjectDataSource
    ID="PaymentGatewaysGroupDs"
    runat="server"
    OldValuesParameterFormatString="original_{0}"
    SelectMethod="LoadForCriteria"
    TypeName="MakerShop.Payments.PaymentGatewayGroupsDataSource"
    SelectCountMethod="CountForCriteria"
    SortParameterName="sortExpression"
    DataObjectTypeName="MakerShop.Payments.PaymentGatewayGroupsDataSource"
    DeleteMethod="Delete"
  >
    <SelectParameters>
      <asp:Parameter Name="sqlCriteria" DefaultValue="" />
    </SelectParameters>
  </asp:ObjectDataSource>
</asp:Content>
