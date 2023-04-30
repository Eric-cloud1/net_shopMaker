<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"
CodeFile="CommonXMLSiteMap.aspx.cs" Inherits="Admin_Website_CommonXMLSiteMap"
Title="XML SiteMap" %> <%@ Import Namespace="MakerShop.Web.SiteMap" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
  <div class="pageHeader">
    <div class="caption">
      <h1>XML SiteMap</h1>
    </div>
  </div>
  <table cellpadding="2" cellspacing="0" class="innerLayout">
    <tr>
      <td
        colspan="2"
        style="text-align: center; padding-left: 30px; padding-right: 30px"
      >
        <p class="InstructionText">
          XML SiteMap is an XML file that lists URLs for a site along with
          additional metadata about each URL (when it was last updated, how
          often it usually changes, and how important it is, relative to other
          URLs in the site) so that search engines can more intelligently crawl
          the site. XML SiteMap has wide adoption, including support from
          Google, Yahoo!, and Microsoft.
          <br />
          <br />
          Once you have created your Sitemap,
          <a
            href="http://www.sitemaps.org/protocol.php#informing"
            target="_blank"
            >let search engines know</a
          >
          about it by submitting directly to them, pinging them, or adding the
          Sitemap location to your robots.txt file.<br /><br /> When you create
          a SiteMap,
          <strong
            >the SiteMap file is placed in your store's home directory</strong
          >. <br /><br />
          If the sitemap grows beyond 50,000 URLs, then multiple sitemap files
          are created along with a sitemap index file. Sitemap index file is
          always named as SiteMapIndex.xml.
          <br /><br /> For more information please visit
          <a href="http://www.sitemaps.org" target="_blank">www.sitemaps.org</a>
        </p>
      </td>
    </tr>
    <tr>
      <td colspan="2" style="padding-left: 40px">
        <asp:Panel ID="MessagePanel" runat="server" CssClass="contentPanel">
          <div class="contentPanelBody">
            <asp:Label
              ID="SuccessMessageHeader"
              runat="server"
              Text="SUCCESS"
              SkinID="GoodCondition"
            ></asp:Label>
            <asp:Label
              ID="FailureMessageHeader"
              runat="server"
              Text="FAILED"
              SkinID="ErrorCondition"
            ></asp:Label>
            <asp:BulletedList ID="Messages" runat="server"></asp:BulletedList
            ><br />
          </div>
        </asp:Panel>
      </td>
    </tr>
    <tr>
      <td>
        <center>
          <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
        </center>
        <br />
        <table class="inputForm">
          <tr>
            <th class="rowHeader" width="50%">
              <asp:Label
                ID="SiteMapFileNameLabel"
                runat="server"
                Text="SiteMap File Name:"
              ></asp:Label
              ><br />
              <span class="helpText"
                >Name of the SiteMap file. This setting will be ignored if
                sitemap grows beyond 50,000 URLs and multiple sitemap files are
                created. The file names will be SiteMap1.xml, SiteMap2.xml and
                so on.</span
              >
            </th>
            <td valign="top" width="50%">
              <asp:TextBox
                ID="SiteMapFileName"
                runat="server"
                Text=""
                Width="200px"
              ></asp:TextBox>
              <asp:RequiredFieldValidator
                ID="FileNameRequired"
                runat="server"
                ControlToValidate="SiteMapFileName"
                ErrorMessage="File name is required."
                >*</asp:RequiredFieldValidator
              >
            </td>
          </tr>
          <tr>
            <th class="rowHeader" width="50%">
              <asp:Label
                ID="AllowOverwriteLabel"
                runat="server"
                Text="Overwrite Existing SiteMap File"
              ></asp:Label>
              <br />
              <span class="helpText"
                >Should the existing SiteMap file be overwritten when creating
                new SiteMap? This setting will be ignored if sitemap grows
                beyond 50,000 URLs and multiple sitemap files are created. Files
                will always be overwritten in that case.</span
              >
            </th>
            <td valign="top" width="50%">
              <asp:RadioButtonList
                ID="AllowOverwrite"
                runat="server"
                RepeatDirection="Vertical"
              >
                <asp:ListItem Text="No" Value="0"></asp:ListItem>
                <asp:ListItem Text="Yes" Value="1"></asp:ListItem>
              </asp:RadioButtonList>
            </td>
          </tr>
          <tr>
            <th class="rowHeader" width="50%">
              <asp:Label
                ID="Label9"
                runat="server"
                Text="Compressed SiteMap File Name"
              ></asp:Label>
              <br />
              <span class="helpText"
                >Name of the compressed SiteMap file. This setting will be
                ignored if sitemap grows beyond 50,000 URLs and multiple sitemap
                files are created. The file names will be SiteMap1.xml.gz,
                SiteMap2.xml.gz and so on.</span
              >
            </th>
            <td valign="top" width="50%">
              <asp:TextBox
                ID="CompressedSiteMapFileName"
                runat="server"
                Text=""
                Width="200px"
              ></asp:TextBox>
              <asp:RequiredFieldValidator
                ID="CompressedFileNameRequired"
                runat="server"
                ControlToValidate="CompressedSiteMapFileName"
                ErrorMessage="Compressed file name is required."
                >*</asp:RequiredFieldValidator
              >
            </td>
          </tr>
          <tr>
            <th class="rowHeader" width="50%">
              <asp:Label
                ID="AllowOverwriteCompressedLabel"
                runat="server"
                Text="Overwrite Existing Compressed SiteMap"
              ></asp:Label>
              <br />
              <span class="helpText"
                >Should the existing compressed SiteMap file be overwritten when
                creating new compressed SiteMap? This setting will be ignored if
                sitemap grows beyond 50,000 URLs and multiple sitemap files are
                created. Files will always be overwritten in that case.</span
              >
            </th>
            <td valign="top" width="50%">
              <asp:RadioButtonList
                ID="AllowOverwriteCompressed"
                runat="server"
                RepeatDirection="Vertical"
              >
                <asp:ListItem Text="No" Value="0"></asp:ListItem>
                <asp:ListItem Text="Yes" Value="1"></asp:ListItem>
              </asp:RadioButtonList>
            </td>
          </tr>

          <tr>
            <th class="rowHeader" width="50%">
              <asp:Label
                ID="Label2"
                runat="server"
                Text="Include Categories"
              ></asp:Label>
              <br />
              <span class="helpText"
                >Should Category URLs be included in the SiteMap?</span
              >
            </th>
            <td valign="top" width="50%">
              <asp:RadioButtonList
                ID="IncludeCategories"
                runat="server"
                RepeatDirection="Vertical"
              >
                <asp:ListItem Text="No" Value="0"></asp:ListItem>
                <asp:ListItem Text="Yes" Value="1"></asp:ListItem>
              </asp:RadioButtonList>
            </td>
          </tr>

          <tr>
            <th class="rowHeader" width="50%">
              <asp:Label
                ID="Label1"
                runat="server"
                Text="Include Products"
              ></asp:Label>
              <br />
              <span class="helpText"
                >Should Product URLs be included in the SiteMap?</span
              >
            </th>
            <td valign="top" width="50%">
              <asp:RadioButtonList
                ID="IncludeProducts"
                runat="server"
                RepeatDirection="Vertical"
              >
                <asp:ListItem Text="No" Value="0"></asp:ListItem>
                <asp:ListItem Text="Yes" Value="1"></asp:ListItem>
              </asp:RadioButtonList>
            </td>
          </tr>

          <tr>
            <th class="rowHeader" width="50%">
              <asp:Label
                ID="Label3"
                runat="server"
                Text="Include Webpages"
              ></asp:Label>
              <br />
              <span class="helpText"
                >Should Webpage URLs be included in the SiteMap?</span
              >
            </th>
            <td valign="top" width="50%">
              <asp:RadioButtonList
                ID="IncludeWebpages"
                runat="server"
                RepeatDirection="Vertical"
              >
                <asp:ListItem Text="No" Value="0"></asp:ListItem>
                <asp:ListItem Text="Yes" Value="1"></asp:ListItem>
              </asp:RadioButtonList>
            </td>
          </tr>

          <tr>
            <th class="rowHeader" width="50%">
              <asp:Label
                ID="Label4"
                runat="server"
                Text="Default URL Priority"
              ></asp:Label>
              <br />
              <span class="helpText"
                >Default Priority of the URLs. Valid values range from 0.0 to
                1.0</span
              >
            </th>
            <td valign="top" width="50%">
              <asp:TextBox
                ID="DefaultUrlPriority"
                runat="server"
                Text="0.5"
                Width="200px"
              ></asp:TextBox>
              <asp:RequiredFieldValidator
                ID="RequiredFieldValidator1"
                runat="server"
                ControlToValidate="DefaultUrlPriority"
                ErrorMessage="Default URL priority is required."
                >*</asp:RequiredFieldValidator
              >
              <asp:RangeValidator
                ID="ValidUrlPriorityRequired"
                runat="server"
                ControlToValidate="DefaultUrlPriority"
                ErrorMessage="Valid value is required"
                MaximumValue="1.0"
                MinimumValue="0.0"
                Type="Double"
                >*</asp:RangeValidator
              >
            </td>
          </tr>

          <tr>
            <th class="rowHeader" width="50%">
              <asp:Label
                ID="Label5"
                runat="server"
                Text="Default Change Frequency"
              ></asp:Label>
              <br />
              <span class="helpText"
                >Default value for how frequently the pages are likely to
                change.</span
              >
            </th>
            <td>
              <asp:DropDownList ID="DefaultChangeFrequency" runat="server">
              </asp:DropDownList>
            </td>
          </tr>
        </table>

        <div align="center" style="margin-top: 20px">
          <asp:Label
            ID="SiteMapActionLabel"
            runat="server"
            Text="Tasks:"
            SkinID="FieldHeader"
          ></asp:Label>
          <asp:DropDownList ID="SiteMapAction" runat="server">
          </asp:DropDownList>
          <asp:ImageButton
            ID="SiteMapActionButton"
            runat="server"
            SkinID="GoIcon"
            OnClick="SiteMapActionButton_Click"
          />
          <br />
          <br />
          <asp:Button
            ID="BtnSaveSettings"
            runat="server"
            Text="Save Settings"
            OnClick="BtnSaveSettings_Click"
          />
          <asp:Button
            ID="CancelButton"
            runat="server"
            OnClick="CancelButton_Click"
            Text="Cancel"
          />
        </div>
      </td>
    </tr>
  </table>
</asp:Content>
