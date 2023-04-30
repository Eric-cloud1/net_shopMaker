<%@Page Language="C#" MasterPageFile="~/Admin/Admin.master"
CodeFile="Connection.aspx.cs" Inherits="Admin_Store_Security_Connection"
Title="Edit Database Connection String" %> <%@ Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
  <div class="pageHeader">
    <div class="caption">
      <h1>
        <asp:Localize
          ID="Caption"
          runat="server"
          Text="Edit Database Connection String"
        ></asp:Localize>
      </h1>
    </div>
  </div>
  <div style="padding: 4px">
    <asp:Localize
      ID="DatabaseHelpText"
      runat="server"
      Text="You can modify the connection string in the field below.  You are responsible for making sure that the specified database has the appropriate structure and data.  Providing an incorrect connection string can disable your installation of MakerShop."
    ></asp:Localize
    ><br /><br />
    <asp:Label
      ID="ErrorMessage"
      runat="server"
      Visible="false"
      SkinID="ErrorCondition"
      EnableViewState="false"
    ></asp:Label>
    <asp:Label
      ID="SavedMessage"
      runat="server"
      Visible="false"
      SkinID="GoodCondition"
      EnableViewState="false"
    ></asp:Label>
    <table class="inputForm">
      <tr>
        <th class="rowHeader">
          <asp:Localize
            ID="ExistingConnectionStringLabel"
            runat="Server"
            Text="Current Connection String:"
          ></asp:Localize>
        </th>
        <td>
          <asp:Literal
            ID="ExistingConnectionString"
            runat="server"
          ></asp:Literal>
        </td>
      </tr>
      <tr>
        <th class="rowHeader">
          <asp:Localize
            ID="ConnectionStringLabel"
            runat="Server"
            Text="New Connection String:"
          ></asp:Localize>
        </th>
        <td>
          <asp:TextBox
            ID="ConnectionString"
            runat="server"
            Width="400px"
          ></asp:TextBox>
        </td>
      </tr>
      <tr>
        <td>&nbsp;</td>
        <td>
          <asp:CheckBox ID="EncryptIt" runat="server" Checked="true" />
          <asp:Localize
            ID="EncryptItText"
            runat="server"
            Text="Encrypt the connection string in the database.config file."
          ></asp:Localize>
        </td>
      </tr>
      <tr>
        <td>&nbsp;</td>
        <td>
          <asp:CheckBox ID="AckBox" runat="server" />
          <asp:Localize
            ID="AckHelpText"
            runat="server"
            Text="I understand that providing an incorrect connection string can disable your installation of MakerShop."
          ></asp:Localize>
        </td>
      </tr>
      <tr>
        <td>&nbsp;</td>
        <td>
          <br />
          <asp:Button
            ID="UpdateButton"
            runat="server"
            Text="Change"
            OnClick="UpdateButton_Click"
          />
          <asp:Button
            ID="CancelButton"
            runat="server"
            Text="Cancel"
            OnClick="CancelButton_Click"
          />
        </td>
      </tr>
    </table>
  </div>
</asp:Content>
