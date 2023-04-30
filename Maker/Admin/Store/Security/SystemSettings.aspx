<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"
AutoEventWireup="true" CodeFile="SystemSettings.aspx.cs"
Inherits="Admin_Store_Security_SystemSettings" Title="System Settings" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
  <div class="pageHeader">
    <div class="caption">
      <h1>
        <asp:Localize
          ID="Caption"
          runat="server"
          Text="System Settings"
        ></asp:Localize>
      </h1>
    </div>
  </div>
  <asp:Label
    ID="SavedMessage"
    runat="server"
    Text="Settings updated at {0}."
    Visible="false"
    SkinID="GoodCondition"
    EnableViewState="false"
  ></asp:Label>
  <div class="section" style="padding: 0 0 2px 0">
    <div class="header">
      <h2 class="commonicon">
        <asp:Localize
          ID="FileExtCaption"
          runat="server"
          Text="File Upload Filters"
        ></asp:Localize>
      </h2>
    </div>
    <div class="content">
      <asp:Localize ID="FileExtHelpText" runat="server" EnableViewState="false">
        You can specify the file types that will be accepted for upload through
        the MakerShop web interface. List the allowed file extensions for each
        interface. If the value is unspecified, all file types are accepted for
        upload. For best security, it is recommended that you only list the
        minimum extensions required.<br /><br /> To specify file types, enter
        the extensions in a comma delimited list. For example: <b>gif, jpg</b
        ><br /><br />
      </asp:Localize>
      <table class="inputForm">
        <tr>
          <th align="right">
            <asp:Localize
              ID="FileExtAssetsLabel"
              runat="server"
              Text="Assets:"
            ></asp:Localize>
          </th>
          <td>
            <asp:TextBox
              ID="FileExtAssets"
              runat="server"
              Text=""
              MaxLength="200"
              Width="300px"
              EnableViewState="false"
            ></asp:TextBox>
          </td>
        </tr>
        <tr>
          <th align="right">
            <asp:Localize
              ID="FileExtThemesLabel"
              runat="server"
              Text="Themes:"
            ></asp:Localize>
          </th>
          <td>
            <asp:TextBox
              ID="FileExtThemes"
              runat="server"
              Text=""
              MaxLength="200"
              Width="300px"
              EnableViewState="false"
            ></asp:TextBox>
          </td>
        </tr>
        <tr>
          <th align="right">
            <asp:Localize
              ID="FileExtDigitalGoodsLabel"
              runat="server"
              Text="Digital Goods:"
            ></asp:Localize>
          </th>
          <td>
            <asp:TextBox
              ID="FileExtDigitalGoods"
              runat="server"
              Text=""
              MaxLength="200"
              Width="300px"
              EnableViewState="false"
            ></asp:TextBox>
          </td>
        </tr>
        <tr>
          <td>&nbsp;</td>
          <td>
            <asp:Button
              ID="SaveButton"
              runat="server"
              Text="Save"
              OnClick="SaveButton_Click"
            />
          </td>
        </tr>
      </table>
    </div>
  </div>
</asp:Content>
