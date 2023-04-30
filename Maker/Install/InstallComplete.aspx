<%@ Page Language="C#" Title="MakerShop {0} installation completed."%> <%@
Import Namespace="System.Xml" %> <%@ Import Namespace="MakerShop.Utility" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        string version = MakerShopVersion.Instance.Version;
  Page.Title = string.Format(Page.Title, version);
        Heading.InnerText = string.Format(Heading.InnerText, version);
    }
</script>
<html xmlns="http://www.w3.org/1999/xhtml">
  <head runat="server">
    <style>
      p {
        font-size: 12px;
      }
      .sectionHeader {
        background-color: #efefef;
        padding: 4px;
        margin: 12px 0px;
      }
      h2 {
        font-size: 14px;
        font-weight: bold;
        margin: 0px;
      }
      .error {
        font-weight: bold;
        color: red;
      }
      div.radio {
        margin: 2px 0px 6px 0px;
      }
      div.radio label {
        font-weight: bold;
        padding-top: 6px;
        position: relative;
        top: 1px;
      }
      .inputBox {
        padding: 6px;
        margin: 4px 40px;
        border: solid 1px #cccccc;
      }
      div.submit {
        background-color: #efefef;
        padding: 4px;
        margin: 12px 0px;
        text-align: left;
      }
    </style>
  </head>
  <body style="width: 780px; margin: auto">
    <form id="form1" runat="server">
      <br />
      <div class="pageHeader">
        <h1 style="font-size: 16px" runat="server" ID="Heading">
          MakerShop {0} Installed!
        </h1>
      </div>
      <p align="justify">
        The installation process is complete. Remove the "Install" folder from
        your store directory for security purposes.
      </p>
      <br />
      <asp:HyperLink
        ID="AdminLink"
        runat="server"
        Text="Continue to Admin"
        NavigateUrl="../Admin/Default.aspx"
        SkinID="Button"
      ></asp:HyperLink>
    </form>
  </body>
</html>
