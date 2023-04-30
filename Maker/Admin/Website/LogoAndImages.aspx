<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"
CodeFile="LogoAndImages.aspx.cs" Inherits="Admin_Website_LogoAndImages"
Title="Image Settings" %> <%@ Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
  <script language="javascript" type="text/javascript">
    function verifyUploadFile() {
      var uploadRadioOption = document.getElementById(
        "<%=LogoOptionUpload.ClientID%>"
      );
      if (uploadRadioOption.checked) {
        var fileUpload = document.getElementById("<%=UploadedLogo.ClientID %>");
        var fileName = fileUpload.value;
        if (fileName == "") {
          return false;
        }

        //getting the file name
        while (fileName.indexOf("\\") != -1)
          fileName = fileName.slice(fileName.indexOf("\\") + 1);

        //Getting the file extension
        var ext = fileName.slice(fileName.indexOf(".")).toLowerCase();
        if (ext == ".gif" || ext == ".jpg" || ext == ".png") {
          return true;
        } else {
          alert("You can only upload '.gif', '.jpg' or '.png' files for logo.");
          return false;
        }
      }
      return true;
    }
  </script>
  <div class="pageHeader">
    <div class="caption">
      <h1>
        <asp:Localize
          ID="Caption"
          runat="server"
          Text="Logo and Images"
        ></asp:Localize>
      </h1>
    </div>
  </div>
  <table cellpadding="0" cellspacing="2" class="innerLayout">
    <tr>
      <td colspan="2">
        <div class="section">
          <div class="header">
            <h2 class="storelogo">
              <asp:Localize
                ID="LogoCaption"
                runat="server"
                Text="Store Logo"
              ></asp:Localize>
            </h2>
          </div>
          <div class="content">
            <asp:Literal
              ID="LogoHelpText"
              runat="server"
              Text="Use the form below to change your logo image on the store website."
            ></asp:Literal>
            <table class="inputForm">
              <tr>
                <td>
                  <asp:RadioButton
                    ID="LogoOptionUpload"
                    runat="Server"
                    Checked="True"
                    GroupName="LogoUploadOption"
                  />
                </td>
                <th class="rowHeader" valign="top" style="text-align: left">
                  <cb:ToolTipLabel
                    ID="UploadedLogoLabel"
                    runat="server"
                    Text="Upload Logo:"
                    AssociatedControlID="LogoOptionUpload"
                  ></cb:ToolTipLabel>
                </th>
                <td>
                  <asp:FileUpload
                    ID="UploadedLogo"
                    runat="server"
                    Width="300"
                    CssClass="fileUpload"
                    Size="50"
                  /><br />
                </td>
              </tr>
              <tr>
                <td>
                  <asp:RadioButton
                    ID="LogoOptionBrowse"
                    runat="Server"
                    Checked="True"
                    GroupName="LogoUploadOption"
                  />
                </td>
                <th class="rowHeader" valign="top" style="text-align: left">
                  <cb:ToolTipLabel
                    ID="LogoOptionBrowseUrlLabel"
                    runat="server"
                    Text="Browse from server:"
                    AssociatedControlID="LogoOptionBrowse"
                  ></cb:ToolTipLabel>
                </th>
                <td>
                  <asp:TextBox
                    ID="LogoOptionBrowseUrl"
                    runat="server"
                    Width="300"
                    Size="50"
                  />
                  <asp:ImageButton
                    ID="BrowseLogoUrl"
                    runat="server"
                    SkinID="FindIcon"
                    AlternateText="Browse"
                  />
                </td>
              </tr>
              <tr>
                <td colspan="3">
                  <asp:Button
                    ID="UpdateButton"
                    runat="server"
                    Text="Update"
                    OnClick="UpdateButton_Click"
                    OnClientClick="return verifyUploadFile();"
                  />
                </td>
              </tr>

              <tr>
                <th class="rowHeader" valign="top" colspan="2">
                  <cb:ToolTipLabel
                    ID="CurrentLogoLabel"
                    runat="server"
                    Text="Current Logo:"
                  ></cb:ToolTipLabel>
                </th>
                <td>
                  <asp:PlaceHolder
                    ID="CurrentLogo"
                    runat="server"
                  ></asp:PlaceHolder>
                </td>
              </tr>
            </table>
          </div>
        </div>
      </td>
    </tr>
    <ajax:UpdatePanel ID="PageAjax" runat="server">
      <ContentTemplate>
        <tr>
          <td colspan="2">
            <asp:Label
              ID="SavedMessage"
              runat="server"
              Text="Image options saved at {0:t}<br /><br />"
              SkinID="GoodCondition"
              EnableViewState="False"
              Visible="false"
            ></asp:Label>
          </td>
        </tr>
        <tr>
          <td valign="top" width="50%">
            <div class="section">
              <div class="header">
                <h2 class="defaultimagesize">
                  <asp:Localize
                    ID="ImageSizeCaption"
                    runat="server"
                    Text="Default Image Sizes"
                  ></asp:Localize>
                </h2>
              </div>
              <div class="content" style="min-height: 150px">
                <asp:Literal
                  ID="ImageSizeHelpText"
                  runat="server"
                  Text="When you upload an image, you have the option to automatically generate the icon, thumbnail, and standard image sizes.  Use the fields below to specify the sizes to use for these images in your store.  All image sizes are specified in number of pixels."
                ></asp:Literal>
                <table class="inputForm">
                  <tr>
                    <th class="rowHeader" valign="top">
                      <cb:ToolTipLabel
                        ID="IconSizeLabel"
                        runat="server"
                        Text="Icon Image Size:"
                        ToolTip="This image size applies to products only.  The tiny image is used for display of a product icon in the mini basket."
                      ></cb:ToolTipLabel>
                    </th>
                    <td>
                      <asp:Label
                        ID="IconWidthLabel"
                        runat="server"
                        AssociatedControlID="IconWidth"
                        Text="width: "
                        SkinID="xFieldHeader"
                      ></asp:Label>
                      <asp:TextBox
                        ID="IconWidth"
                        runat="server"
                        Width="40px"
                        MaxLength="2"
                      ></asp:TextBox>
                      &nbsp;
                      <asp:Label
                        ID="IconHeightLabel"
                        runat="server"
                        AssociatedControlID="IconHeight"
                        Text="height: "
                        SkinID="xFieldHeader"
                      ></asp:Label>
                      <asp:TextBox
                        ID="IconHeight"
                        runat="server"
                        Width="40px"
                        MaxLength="2"
                      ></asp:TextBox>
                    </td>
                  </tr>
                  <tr>
                    <th class="rowHeader" valign="top">
                      <cb:ToolTipLabel
                        ID="ThumbnailSizeLabel"
                        runat="server"
                        Text="Thumbnail Image Size:"
                        ToolTip="This image size applies to all catalog items.  The thumbnail image is generally shown with an item in directory or catalog listing pages."
                      ></cb:ToolTipLabel>
                    </th>
                    <td>
                      <asp:Label
                        ID="ThumbnailWidthLabel"
                        runat="server"
                        AssociatedControlID="ThumbnailWidth"
                        Text="width: "
                        SkinID="xFieldHeader"
                      ></asp:Label>
                      <asp:TextBox
                        ID="ThumbnailWidth"
                        runat="server"
                        Width="40px"
                        MaxLength="3"
                      ></asp:TextBox>
                      &nbsp;
                      <asp:Label
                        ID="ThumbnailHeightLabel"
                        runat="server"
                        AssociatedControlID="ThumbnailHeight"
                        Text="height: "
                        SkinID="xFieldHeader"
                      ></asp:Label>
                      <asp:TextBox
                        ID="ThumbnailHeight"
                        runat="server"
                        Width="40px"
                        MaxLength="3"
                      ></asp:TextBox>
                    </td>
                  </tr>
                  <tr>
                    <th class="rowHeader" valign="top">
                      <cb:ToolTipLabel
                        ID="StandardSizeLabel"
                        runat="server"
                        Text="Standard Image Size:"
                        ToolTip="This image size applies to product items only.  The standard image is generally shown on the product detail page, although some other display elements may make use of this image."
                      ></cb:ToolTipLabel>
                    </th>
                    <td>
                      <asp:Label
                        ID="StandardWidthLabel"
                        runat="server"
                        AssociatedControlID="StandardWidth"
                        Text="width: "
                        SkinID="xFieldHeader"
                      ></asp:Label>
                      <asp:TextBox
                        ID="StandardWidth"
                        runat="server"
                        Width="40px"
                        MaxLength="4"
                      ></asp:TextBox>
                      &nbsp;
                      <asp:Label
                        ID="StandardHeightLabel"
                        runat="server"
                        AssociatedControlID="StandardHeight"
                        Text="height: "
                        SkinID="xFieldHeader"
                      ></asp:Label>
                      <asp:TextBox
                        ID="StandardHeight"
                        runat="server"
                        Width="40px"
                        MaxLength="4"
                      ></asp:TextBox>
                    </td>
                  </tr>
                </table>
              </div>
            </div>
          </td>
          <td valign="top" width="50%">
            <div class="section">
              <div class="header">
                <h2 class="commonicon">
                  <asp:Localize
                    ID="SkuImageCaption"
                    runat="server"
                    Text="Product Image Lookup by SKU"
                  ></asp:Localize>
                </h2>
              </div>
              <div class="content" style="min-height: 150px">
                <asp:Literal
                  ID="SkuImageHelpText"
                  runat="server"
                  Text="When this option is enabled, product image URLs are automatically calculated using the SKU if no image is otherwise specified.  The expected format is ?_i.jpg for icons, ?_t.jpg for thumbnails, and ?.jpg for standard images."
                ></asp:Literal
                ><br />
                <table class="inputForm">
                  <tr>
                    <th class="rowHeader">
                      <cb:ToolTipLabel
                        ID="ImageSkuLookupEnabledLabel"
                        runat="server"
                        Text="Enable Image Lookup:"
                        AssociatedControlID="ImageSkuLookupEnabled"
                        ToolTip="Check to enable image lookup from product sku."
                      ></cb:ToolTipLabel>
                    </th>
                    <td>
                      <asp:CheckBox ID="ImageSkuLookupEnabled" runat="server" />
                    </td>
                  </tr>
                </table>
              </div>
            </div>
          </td>
        </tr>
        <tr>
          <td colspan="2">
            <div class="section">
              <div class="header">
                <h2 class="optionswatches">
                  <asp:Localize
                    ID="OptionThumbnailCaption"
                    runat="server"
                    Text="Defaults for Option Swatches"
                  ></asp:Localize>
                </h2>
              </div>
              <div class="content">
                <asp:Literal
                  ID="OptionThumbnailHelpText"
                  runat="server"
                  Text="If you define swatches for your product options, you can set default height, width, and columns that will be used when displaying the images.  The default values can be overridden at the attribute level."
                ></asp:Literal>
                <table class="inputForm" cellspacing="4">
                  <tr>
                    <th class="rowHeader">
                      <cb:ToolTipLabel
                        ID="DefaultThumbnailSizeLabel"
                        runat="server"
                        Text="Default Size:"
                        ToolTip="The default image size for option swatches."
                      ></cb:ToolTipLabel>
                    </th>
                    <td>
                      <asp:Label
                        ID="OptionThumbnailWidthLabel"
                        runat="server"
                        AssociatedControlID="OptionThumbnailWidth"
                        Text="width: "
                      ></asp:Label>
                      <asp:TextBox
                        ID="OptionThumbnailWidth"
                        runat="server"
                        Width="40px"
                        MaxLength="3"
                      ></asp:TextBox>
                      &nbsp;
                      <asp:Label
                        ID="OptionThumbnailHeightLabel"
                        runat="server"
                        AssociatedControlID="OptionThumbnailHeight"
                        Text="height: "
                      ></asp:Label>
                      <asp:TextBox
                        ID="OptionThumbnailHeight"
                        runat="server"
                        Width="40px"
                        MaxLength="3"
                      ></asp:TextBox>
                    </td>
                    <td>&nbsp;&nbsp;&nbsp;</td>
                    <th class="rowHeader">
                      <cb:ToolTipLabel
                        ID="OptionThumbnailColumnsLabel"
                        runat="server"
                        Text="Default Columns:"
                        ToolTip="This default number of thumbnail images to display per row."
                        AssociatedControlID="OptionThumbnailColumns"
                      ></cb:ToolTipLabel>
                    </th>
                    <td>
                      <asp:TextBox
                        ID="OptionThumbnailColumns"
                        runat="server"
                        Width="40px"
                        MaxLength="2"
                      ></asp:TextBox>
                    </td>
                  </tr>
                </table>
              </div>
            </div>
          </td>
        </tr>
        <tr>
          <td>
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
            <asp:Button
              Id="SaveButon"
              runat="server"
              Text="Save Changes"
              OnClick="SaveButton_Click"
              CssClass="button"
            />
          </td>
        </tr>
      </ContentTemplate>
    </ajax:UpdatePanel>
  </table>
</asp:Content>
