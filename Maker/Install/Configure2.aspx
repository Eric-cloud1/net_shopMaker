<%@ Page Language="C#" Title="Install MakerShop {0} (Step 2 of 2)"%> <%@ Import
Namespace="System.IO" %> <%@ Import Namespace="System.Xml" %> <%@ Import
Namespace="MakerShop.Data" %> <%@ Import Namespace="MakerShop.Utility" %> <%@
Import Namespace="MakerShop.Configuration" %> <%@ Import
Namespace="System.Data.SqlClient" %> <%@ Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<script runat="server">
  protected void Page_Load(object sender, EventArgs e)
  {
      string version = MakerShopVersion.Instance.Version;
      Page.Title = string.Format(Page.Title, version);
      Heading.InnerText = string.Format(Heading.InnerText, version);
  }

  protected void BasicData_CheckedChanged(object sender, EventArgs e)
  {
      trAdditionalData.Visible = BasicData.Checked;
  }

  private static double GetDefaultTzOffset()
  {
      DateTime localNow = DateTime.Now;
      DateTime utcNow = localNow.ToUniversalTime();
      if (localNow == utcNow)
      {
          return 0;
      }
      else if (localNow < utcNow)
      {
          TimeSpan ts = utcNow - localNow;
          return -1 * ts.TotalHours;
      }
      else
      {
          TimeSpan ts = localNow - utcNow;
          return ts.TotalHours;
      }
  }


  protected void InstallButton_Click(object sender, EventArgs e)
  {
      //REQUIRED DATA
      Store store = StoreDataSource.CreateStore(StoreName.Text, Email.Text, Password.Text, true);
      //CREATE DEFAULT COUNTRIES
      CreateCountries(CountryDefaults.SelectedIndex == 0);
      //POPULATE DEFAULT DATA
      List<string> errorList = new List<string>();
      RunScript("AC7_Required_Data.sql", errorList);
      store.StoreUrl = GetStoreUrl();
      store.Save();
      //RELOAD STORE FROM DATABASE TO ENSURE CLEAN IN-MEMORY CACHE FROM DB SCRIPT
      store = StoreDataSource.Load(store.StoreId, false);
      store.Settings.TimeZoneOffset = GetDefaultTzOffset();
      store.Settings.TimeZoneCode = store.Settings.TimeZoneOffset.ToString();
      store.Settings.Save();
      Warehouse warehouse = CreateWarehouse(store);
      //ADDITIONAL DATA
      if (BasicData.Checked)
      {
          //RUN THE BASIC DATA SCRIPT
          RunScript("AC7_Basic_Data.sql", errorList);
          UpdateMerchantEmail();
          if (AdditionalData.Checked)
          {
              RunScript("AC7_Sample_Data.sql", errorList);
              //COPY SAMPLE IMAGES
              CopyDirectory(Server.MapPath("~/install/ProductImages"), Server.MapPath("~/Assets/ProductImages"));
  //COPY EMAIL TEMPLATES
  CopyDirectory(Server.MapPath("~/Install/EmailTemplates"), Server.MapPath("~/App_Data/EmailTemplates/" + store.StoreId));
              //MAKE SAMPLE ESD FILE
              File.WriteAllText(Server.MapPath("~/App_Data/DigitalGoods/sample.txt"), "This is a sample text file for use with digital delivery.");
          }
      }
      if (errorList.Count > 0)
      {
          Response.Write("<b>Errors occurred during sample data installation:</b><br /><br />");
          Response.Write(string.Join("<br /><br />", errorList.ToArray()));
          Response.Write("<br /><br />");
          Response.Write("Your installation should still be functional.  If you wish to proceed <a href=\"InstallComplete.aspx\">click here</a>.");
          Response.End();
      }
      Response.Redirect("InstallComplete.aspx");
  }

  private string GetStoreUrl()
  {
      string tempUrl = Request.Url.ToString();
      int qIndex = tempUrl.IndexOf("?");
      if (qIndex > -1)
      {
          tempUrl = tempUrl.Substring(0, qIndex);
      }
      tempUrl = tempUrl.ToLowerInvariant().Replace("install/configure2.aspx", string.Empty);
      return tempUrl;
  }

  protected Warehouse CreateWarehouse(Store store)
  {
      Warehouse warehouse = new Warehouse();
      warehouse.Name = store.Name;
      warehouse.Address1 = Address1.Text;
      warehouse.Address2 = Address2.Text;
      warehouse.City = City.Text;
      warehouse.Province = Province.Text;
      warehouse.PostalCode = PostalCode.Text;
      warehouse.CountryCode = Country.Text;
      warehouse.Phone = Phone.Text;
      warehouse.Fax = Fax.Text;
      warehouse.Save();
      store.Warehouses.Add(warehouse);
      store.DefaultWarehouseId = warehouse.WarehouseId;
      store.Save();
      return warehouse;
  }

  protected void CreateCountries(bool northAmericaOnly)
  {
      XmlDocument countryData = new XmlDocument();
      countryData.Load(Server.MapPath("~/Install/countries.xml"));
      if (northAmericaOnly)
      {
          XmlNode countryNode = countryData.DocumentElement.SelectSingleNode("Country[@Code=\"CA\"]");
          if (countryNode != null) AddCountry(countryNode);
          countryNode = countryData.DocumentElement.SelectSingleNode("Country[@Code=\"MX\"]");
          if (countryNode != null) AddCountry(countryNode);
          countryNode = countryData.DocumentElement.SelectSingleNode("Country[@Code=\"US\"]");
          if (countryNode != null) AddCountry(countryNode);
      }
      else
      {
          XmlNodeList countries = countryData.DocumentElement.SelectNodes("Country");
          foreach (XmlNode countryNode in countries)
          {
              AddCountry(countryNode);
          }
      }
  }

  protected void AddCountry(XmlNode countryNode)
  {
      Country country = new Country();
      country.CountryCode = Server.UrlDecode(XmlUtility.GetAttributeValue(countryNode, "Code", string.Empty));
      country.Name = Server.UrlDecode(XmlUtility.GetAttributeValue(countryNode, "Name", string.Empty));
      if (!string.IsNullOrEmpty(country.CountryCode) && !string.IsNullOrEmpty(country.Name))
      {
          country.AddressFormat = Server.UrlDecode(XmlUtility.GetElementValue(countryNode, "AddressFormat", string.Empty));
          XmlElement provinceNodes = XmlUtility.GetElement(countryNode, "Provinces", false);
          if (provinceNodes != null)
          {
              foreach (XmlNode provinceNode in provinceNodes.ChildNodes)
              {
                  Province province = new Province();
                  province.Name = Server.UrlDecode(XmlUtility.GetAttributeValue(provinceNode, "Name", string.Empty));
                  province.ProvinceCode = Server.UrlDecode(XmlUtility.GetAttributeValue(provinceNode, "Abbreviation", string.Empty));
                  if (province.Name != string.Empty)
                  {
                      country.Provinces.Add(province);
                  }
              }
          }
          country.Save();
      }
  }

  private void RunScript(string dbScriptName, List<string> errorList)
  {
      Trace.Write("Running script " + dbScriptName);
      string dbScript = FileHelper.ReadText(Server.MapPath("~/Install/" + dbScriptName));
      //REMOVE SCRIPT COMMENTS
      dbScript = Regex.Replace(dbScript, @"/\*.*?\*/", string.Empty);
      //SPLIT INTO COMMANDS
      string[] commands = StringHelper.Split(dbScript, "\r\nGO\r\n");
      //LOOP THE COMANDS AND RUN ON DB
      Database database = Token.Instance.Database;
      foreach (string sql in commands)
      {
          if (sql.Trim().Length > 0)
          {
              try
              {
                  System.Data.Common.DbCommand command = database.GetSqlStringCommand(sql);
                  database.ExecuteNonQuery(command);
              }
              catch (Exception ex)
              {
                  errorList.Add("<b>SQL:</b> " + Server.HtmlEncode(sql.Substring(0, 100)) + " [...]");
                  errorList.Add("<b>Error:</b> " + ex.Message);
              }
          }
      }
  }

  private void UpdateMerchantEmail()
  {
      string sql = "UPDATE ac_EmailTemplates SET ToAddress = @merchant WHERE ToAddress = 'merchant'";
      Database database = Token.Instance.Database;
      System.Data.Common.DbCommand command = database.GetSqlStringCommand(sql);
      database.AddInParameter(command, "@merchant", System.Data.DbType.String, Email.Text);
      database.ExecuteNonQuery(command);
  }

  private void CopyDirectory(string source, string target)
  {
      DirectoryInfo sourceDir = new DirectoryInfo(source);
      DirectoryInfo targetDir = new DirectoryInfo(target);
      //verify target exists
      if (!targetDir.Exists) targetDir.Create();
      //copy sub directories
      DirectoryInfo[] sourceSubs = sourceDir.GetDirectories();
      foreach (DirectoryInfo subDir in sourceSubs)
      {
          if (!subDir.Name.StartsWith("."))
          {
              CopyDirectory(subDir.FullName, Path.Combine(targetDir.FullName, subDir.Name));
          }
      }
      //copy files
      FileInfo[] sourceFiles = sourceDir.GetFiles();
      foreach (FileInfo fi in sourceFiles)
      {
          try
          {
              File.Copy(fi.FullName, Path.Combine(targetDir.FullName, fi.Name), true);
          }
          catch { }
      }
  }

  protected void Country_SelectedIndexChanged(Object sender, EventArgs e)
  {
      if (Country.SelectedValue == "US")
      {
          ProvinceRequired.ValidationExpression = "AL|AK|AZ|AR|CA|CO|CT|DE|DC|FL|GA|HI|ID|IL|IN|IA|KS|KY|LA|ME|MD|MA|MI|MN|MS|MO|MT|NE|NV|NH|NJ|NM|NY|NC|ND|OH|OK|OR|PA|RI|SC|SD|TN|TX|UT|VT|VA|WA|WV|WI|WY";
          ProvinceRequired.ErrorMessage = "Please provide a valid two letter US state code (in capital letters).";
          ProvinceRequired.Enabled = true;

          PostalCodeRequired.ValidationExpression = "\\d{5}";
          PostalCodeRequired.ErrorMessage = "You must enter a valid US ZIP (#####).";
          PostalCodeRequired.Enabled = true;
      }
      if (Country.SelectedValue == "CA")
      {
          ProvinceRequired.ValidationExpression = "AB|BC|MB|NB|NL|NT|NS|NU|ON|PE|QC|SK|YT";
          ProvinceRequired.ErrorMessage = "Please provide a valid two letter CA state code (in capital letters).";
          ProvinceRequired.Enabled = true;

          PostalCodeRequired.ValidationExpression = "^[A-Z][0-9][A-Z][0-9][A-Z][0-9]$";
          PostalCodeRequired.ErrorMessage = "You must enter a valid CA ZIP (A#A #A#)";
          PostalCodeRequired.Enabled = true;
      }
      else
      {
          ProvinceRequired.Enabled = false;
          PostalCodeRequired.Enabled = false;
      }
  }
</script>

<html xmlns="http://www.w3.org/1999/xhtml">
  <head id="Head1" runat="server">
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
    <script type="text/javascript" language="JavaScript">
      var counter = 0;
      function plswt() {
        counter++;
        if (counter > 1) {
          alert(
            "You have already submitted this form.  Please wait while the install processes."
          );
          return false;
        }
        return true;
      }
    </script>
  </head>
  <body style="width: 780px; margin: auto">
    <form id="form1" runat="server">
      <ajax:ScriptManager
        ID="ScriptManager"
        runat="server"
        EnablePartialRendering="true"
      ></ajax:ScriptManager>
      <br />
      <div class="pageHeader">
        <h1 style="font-size: 16px" runat="server" id="Heading">
          Install MakerShop {0} (Step 2 of 2)
        </h1>
      </div>
      <p align="justify">
        To prepare your installation, please fill out the fields below as
        completely as possible. Then click the "Install" button at the bottom of
        the form. Once the install process is completed, you will be provided a
        link to the merchant administration of your new store!
      </p>
      <table cellpadding="4" cellspacing="0">
        <tr>
          <td align="left" colspan="4" style="color: red">
            <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
          </td>
        </tr>
        <tr>
          <th align="right" nowrap valign="top">
            <asp:Label
              ID="StoreNameLabel"
              runat="server"
              Text="Store Name:"
              EnableViewState="false"
            ></asp:Label>
          </th>
          <td valign="top">
            <asp:TextBox
              ID="StoreName"
              runat="Server"
              Width="200px"
              MaxLength="100"
              EnableViewState="false"
            ></asp:TextBox>
            <asp:RequiredFieldValidator
              ID="StoreNameRequired"
              runat="server"
              ControlToValidate="StoreName"
              ErrorMessage="Store name is required."
              Display="Dynamic"
              Text="*"
              EnableViewState="false"
              >*</asp:RequiredFieldValidator
            >
          </td>
        </tr>
        <tr>
          <th align="right" nowrap>
            <asp:Label
              ID="EmailLabel"
              runat="server"
              AssociatedControlID="Email"
              Text="Admin Email:"
              EnableViewState="false"
            ></asp:Label>
          </th>
          <td align="left">
            <asp:TextBox
              ID="Email"
              runat="server"
              Width="200px"
              MaxLength="200"
              EnableViewState="false"
            ></asp:TextBox>
            <asp:RequiredFieldValidator
              ID="EmailRequired"
              runat="server"
              ControlToValidate="Email"
              ErrorMessage="Email address is required."
              Display="Dynamic"
              Text="*"
              EnableViewState="false"
              >*</asp:RequiredFieldValidator
            >
            <asp:RegularExpressionValidator
              ID="EmailValidator"
              runat="server"
              Display="Dynamic"
              ControlToValidate="Email"
              ValidationExpression="\S+@\S+\.\S+"
              ErrorMessage="The email address is not properly formatted."
              Text="*"
              EnableViewState="false"
            ></asp:RegularExpressionValidator>
          </td>
        </tr>
        <tr>
          <th align="right" nowrap>
            <asp:Label
              ID="PasswordLabel"
              runat="server"
              AssociatedControlID="Password"
              Text="Password:"
              EnableViewState="false"
            ></asp:Label>
          </th>
          <td align="left">
            <asp:TextBox
              ID="Password"
              runat="server"
              TextMode="Password"
              Columns="20"
              MaxLength="30"
              EnableViewState="false"
            ></asp:TextBox>
            <asp:RequiredFieldValidator
              ID="PasswordRequired"
              runat="server"
              ControlToValidate="Password"
              ErrorMessage="Password is required."
              Text="*"
              EnableViewState="false"
            ></asp:RequiredFieldValidator>
          </td>
        </tr>
        <tr>
          <th align="right" nowrap>
            <asp:Label
              ID="ConfirmPasswordLabel"
              runat="server"
              AssociatedControlID="ConfirmPassword"
              Text="Retype:"
              EnableViewState="false"
            ></asp:Label>
          </th>
          <td align="left">
            <asp:TextBox
              ID="ConfirmPassword"
              runat="server"
              TextMode="Password"
              Columns="20"
              MaxLength="30"
              EnableViewState="false"
            ></asp:TextBox>
            <asp:RequiredFieldValidator
              ID="ConfirmPasswordRequired"
              runat="server"
              ControlToValidate="ConfirmPassword"
              ErrorMessage="You must retype the password."
              Text="*"
              EnableViewState="false"
              >*</asp:RequiredFieldValidator
            >
            <asp:CompareValidator
              ID="PasswordCompare"
              runat="server"
              ControlToCompare="Password"
              ControlToValidate="ConfirmPassword"
              Display="Dynamic"
              ErrorMessage="You did not retype the password correctly."
              Text="*"
              EnableViewState="false"
            ></asp:CompareValidator>
          </td>
        </tr>
      </table>
      <div class="sectionHeader">
        <h2>
          <asp:Localize
            ID="StoreAddressCaption"
            runat="server"
            Text="Store Address"
            SkinID="PageCaption"
          ></asp:Localize>
        </h2>
      </div>
      <ajax:UpdatePanel ID="AddressUpdatePanel" runat="server">
        <ContentTemplate>
          <table cellpadding="4" cellspacing="0">
            <tr>
              <th align="right" nowrap>
                <asp:Label
                  ID="Address1Label"
                  runat="server"
                  Text="Street Address 1:"
                  EnableViewState="false"
                ></asp:Label
                ><br />
              </th>
              <td valign="top">
                <asp:TextBox
                  ID="Address1"
                  runat="server"
                  EnableViewState="false"
                ></asp:TextBox>
                <asp:RequiredFieldValidator
                  ID="AddressRequired"
                  runat="server"
                  ControlToValidate="Address1"
                  Display="Dynamic"
                  ErrorMessage="Address is required."
                  EnableViewState="false"
                  >*</asp:RequiredFieldValidator
                >
              </td>
              <th align="right" nowrap>
                <asp:Label
                  ID="Address2Label"
                  runat="server"
                  Text="Street Address 2:"
                  EnableViewState="false"
                ></asp:Label
                ><br />
              </th>
              <td valign="top">
                <asp:TextBox
                  ID="Address2"
                  runat="server"
                  EnableViewState="false"
                ></asp:TextBox>
              </td>
            </tr>
            <tr>
              <th align="right" nowrap>
                <asp:Label
                  ID="CityLabel"
                  runat="server"
                  Text="City:"
                  EnableViewState="false"
                ></asp:Label
                ><br />
              </th>
              <td valign="top">
                <asp:TextBox
                  ID="City"
                  runat="server"
                  EnableViewState="false"
                ></asp:TextBox>
                <asp:RequiredFieldValidator
                  ID="CityRequired"
                  runat="server"
                  ControlToValidate="City"
                  Display="Dynamic"
                  ErrorMessage="City is required."
                  EnableViewState="false"
                  >*</asp:RequiredFieldValidator
                >
              </td>
              <th align="right" nowrap>
                <asp:Label
                  ID="ProvinceLabel"
                  runat="server"
                  Text="State:"
                  EnableViewState="false"
                ></asp:Label
                ><br />
              </th>
              <td valign="top">
                <asp:TextBox
                  ID="Province"
                  runat="server"
                  EnableViewState="false"
                ></asp:TextBox>
                <cb:RequiredRegularExpressionValidator
                  ID="ProvinceRequired"
                  runat="server"
                  Display="Dynamic"
                  Required="true"
                  ErrorMessage="Please provide a valid two letter US state code (in capital letters)."
                  Text="*"
                  ControlToValidate="Province"
                  ValidationExpression="AL|AK|AZ|AR|CA|CO|CT|DE|DC|FL|GA|HI|ID|IL|IN|IA|KS|KY|LA|ME|MD|MA|MI|MN|MS|MO|MT|NE|NV|NH|NJ|NM|NY|NC|ND|OH|OK|OR|PA|RI|SC|SD|TN|TX|UT|VT|VA|WA|WV|WI|WY"
                />
              </td>
            </tr>
            <tr>
              <th align="right" nowrap>
                <asp:Label
                  ID="PostalCodeLabel"
                  runat="server"
                  Text="Zip/Postal Code:"
                  EnableViewState="false"
                ></asp:Label
                ><br />
              </th>
              <td valign="top">
                <asp:TextBox
                  ID="PostalCode"
                  runat="server"
                  EnableViewState="false"
                ></asp:TextBox>
                <cb:RequiredRegularExpressionValidator
                  ID="PostalCodeRequired"
                  runat="server"
                  Display="Dynamic"
                  Required="true"
                  ErrorMessage="You must enter a valid US ZIP (#####)."
                  Text="*"
                  ControlToValidate="PostalCode"
                  ValidationExpression="\d{5}"
                />
              </td>
              <th align="right" nowrap>
                <asp:Label
                  ID="CountryCodeLabel"
                  runat="server"
                  Text="Country:"
                  EnableViewState="false"
                ></asp:Label
                ><br />
              </th>
              <td valign="top">
                <asp:DropDownList
                  ID="Country"
                  runat="server"
                  EnableViewState="false"
                  AutoPostBack="true"
                  OnSelectedIndexChanged="Country_SelectedIndexChanged"
                >
                  <asp:ListItem Value="US" Text="United States" />
                  <asp:ListItem Value="CA" Text="Canada" />
                  <asp:ListItem Value="" Text="------" />
                  <asp:ListItem Value="AF" Text="Afghanistan" />
                  <asp:ListItem Value="AX" Text="&#197;land Islands" />
                  <asp:ListItem Value="AL" Text="Albania" />
                  <asp:ListItem Value="DZ" Text="Algeria" />
                  <asp:ListItem Value="AS" Text="American Samoa" />
                  <asp:ListItem Value="AD" Text="Andorra" />
                  <asp:ListItem Value="AO" Text="Angola" />
                  <asp:ListItem Value="AI" Text="Anguilla" />
                  <asp:ListItem Value="AQ" Text="Antarctica" />
                  <asp:ListItem Value="AG" Text="Antigua and Barbuda" />
                  <asp:ListItem Value="AR" Text="Argentina" />
                  <asp:ListItem Value="AM" Text="Armenia" />
                  <asp:ListItem Value="AW" Text="Aruba" />
                  <asp:ListItem Value="AU" Text="Australia" />
                  <asp:ListItem Value="AT" Text="Austria" />
                  <asp:ListItem Value="AZ" Text="Azerbaijan" />
                  <asp:ListItem Value="BS" Text="Bahamas" />
                  <asp:ListItem Value="BH" Text="Bahrain" />
                  <asp:ListItem Value="BD" Text="Bangladesh" />
                  <asp:ListItem Value="BB" Text="Barbados" />
                  <asp:ListItem Value="BY" Text="Belarus" />
                  <asp:ListItem Value="BE" Text="Belgium" />
                  <asp:ListItem Value="BZ" Text="Belize" />
                  <asp:ListItem Value="BJ" Text="Benin" />
                  <asp:ListItem Value="BM" Text="Bermuda" />
                  <asp:ListItem Value="BT" Text="Bhutan" />
                  <asp:ListItem Value="BO" Text="Bolivia" />
                  <asp:ListItem Value="BA" Text="Bosnia and Herzegovina" />
                  <asp:ListItem Value="BW" Text="Botswana" />
                  <asp:ListItem Value="BV" Text="Bouvet Island" />
                  <asp:ListItem Value="BR" Text="Brazil" />
                  <asp:ListItem Value="BN" Text="Brunei Darussalam" />
                  <asp:ListItem Value="BG" Text="Bulgaria" />
                  <asp:ListItem Value="BF" Text="Burkina Faso" />
                  <asp:ListItem Value="BI" Text="Burundi" />
                  <asp:ListItem Value="KH" Text="Cambodia" />
                  <asp:ListItem Value="CM" Text="Cameroon" />
                  <asp:ListItem Value="CA" Text="Canada" />
                  <asp:ListItem Value="CV" Text="Cape Verde" />
                  <asp:ListItem Value="KY" Text="Cayman Islands" />
                  <asp:ListItem Value="CF" Text="Central African Republic" />
                  <asp:ListItem Value="TD" Text="Chad" />
                  <asp:ListItem Value="CL" Text="Chile" />
                  <asp:ListItem Value="CN" Text="China" />
                  <asp:ListItem Value="CX" Text="Christmas Island" />
                  <asp:ListItem Value="CC" Text="Cocos (Keeling) Islands" />
                  <asp:ListItem Value="CO" Text="Colombia" />
                  <asp:ListItem Value="KM" Text="Comoros" />
                  <asp:ListItem Value="CG" Text="Congo - Brazzaville" />
                  <asp:ListItem Value="CD" Text="Congo - Kinshasa" />
                  <asp:ListItem Value="CK" Text="Cook Islands" />
                  <asp:ListItem Value="CR" Text="Costa Rica" />
                  <asp:ListItem Value="CI" Text="Cote d'Ivoire" />
                  <asp:ListItem Value="HR" Text="Croatia" />
                  <asp:ListItem Value="CU" Text="Cuba" />
                  <asp:ListItem Value="CY" Text="Cyprus" />
                  <asp:ListItem Value="CZ" Text="Czech Republic" />
                  <asp:ListItem Value="DK" Text="Denmark" />
                  <asp:ListItem Value="DJ" Text="Djibouti" />
                  <asp:ListItem Value="DM" Text="Dominica" />
                  <asp:ListItem Value="DO" Text="Dominican Republic" />
                  <asp:ListItem Value="EC" Text="Ecuador" />
                  <asp:ListItem Value="EG" Text="Egypt" />
                  <asp:ListItem Value="SV" Text="El Salvador" />
                  <asp:ListItem Value="GQ" Text="Equatorial Guinea" />
                  <asp:ListItem Value="ER" Text="Eritrea" />
                  <asp:ListItem Value="EE" Text="Estonia" />
                  <asp:ListItem Value="ET" Text="Ethiopia" />
                  <asp:ListItem Value="FK" Text="Falkland Islands" />
                  <asp:ListItem Value="FO" Text="Faroe Islands" />
                  <asp:ListItem Value="FJ" Text="Fiji" />
                  <asp:ListItem Value="FI" Text="Finland" />
                  <asp:ListItem Value="FR" Text="France" />
                  <asp:ListItem Value="GF" Text="French Guiana" />
                  <asp:ListItem Value="PF" Text="French Polynesia" />
                  <asp:ListItem Value="GA" Text="Gabon" />
                  <asp:ListItem Value="GM" Text="Gambia" />
                  <asp:ListItem Value="GE" Text="Georgia" />
                  <asp:ListItem Value="DE" Text="Germany" />
                  <asp:ListItem Value="GH" Text="Ghana" />
                  <asp:ListItem Value="GI" Text="Gibraltar" />
                  <asp:ListItem Value="GR" Text="Greece" />
                  <asp:ListItem Value="GL" Text="Greenland" />
                  <asp:ListItem Value="GD" Text="Grenada" />
                  <asp:ListItem Value="GP" Text="Guadeloupe" />
                  <asp:ListItem Value="GU" Text="Guam" />
                  <asp:ListItem Value="GT" Text="Guatemala" />
                  <asp:ListItem Value="GN" Text="Guinea" />
                  <asp:ListItem Value="GW" Text="Guinea-bissau" />
                  <asp:ListItem Value="GY" Text="Guyana" />
                  <asp:ListItem Value="HT" Text="Haiti" />
                  <asp:ListItem Value="HN" Text="Honduras" />
                  <asp:ListItem Value="HK" Text="Hong Kong" />
                  <asp:ListItem Value="HU" Text="Hungary" />
                  <asp:ListItem Value="IS" Text="Iceland" />
                  <asp:ListItem Value="IN" Text="India" />
                  <asp:ListItem Value="ID" Text="Indonesia" />
                  <asp:ListItem Value="IR" Text="Iran" />
                  <asp:ListItem Value="IQ" Text="Iraq" />
                  <asp:ListItem Value="IE" Text="Ireland" />
                  <asp:ListItem Value="IL" Text="Israel" />
                  <asp:ListItem Value="IT" Text="Italy" />
                  <asp:ListItem Value="JM" Text="Jamaica" />
                  <asp:ListItem Value="JP" Text="Japan" />
                  <asp:ListItem Value="JO" Text="Jordan" />
                  <asp:ListItem Value="KZ" Text="Kazakhstan" />
                  <asp:ListItem Value="KE" Text="Kenya" />
                  <asp:ListItem Value="KI" Text="Kiribati" />
                  <asp:ListItem Value="KP" Text="Korea, North" />
                  <asp:ListItem Value="KR" Text="Korea, South" />
                  <asp:ListItem Value="KW" Text="Kuwait" />
                  <asp:ListItem Value="KG" Text="Kyrgyzstan" />
                  <asp:ListItem Value="LA" Text="Laos" />
                  <asp:ListItem Value="LV" Text="Latvia" />
                  <asp:ListItem Value="LB" Text="Lebanon" />
                  <asp:ListItem Value="LS" Text="Lesotho" />
                  <asp:ListItem Value="LR" Text="Liberia" />
                  <asp:ListItem Value="LY" Text="Libya" />
                  <asp:ListItem Value="LI" Text="Liechtenstein" />
                  <asp:ListItem Value="LT" Text="Lithuania" />
                  <asp:ListItem Value="LU" Text="Luxembourg" />
                  <asp:ListItem Value="MO" Text="Macao" />
                  <asp:ListItem Value="MK" Text="Macedonia" />
                  <asp:ListItem Value="MG" Text="Madagascar" />
                  <asp:ListItem Value="MW" Text="Malawi" />
                  <asp:ListItem Value="MY" Text="Malaysia" />
                  <asp:ListItem Value="MV" Text="Maldives" />
                  <asp:ListItem Value="ML" Text="Mali" />
                  <asp:ListItem Value="MT" Text="Malta" />
                  <asp:ListItem Value="MH" Text="Marshall Islands" />
                  <asp:ListItem Value="MQ" Text="Martinique" />
                  <asp:ListItem Value="MR" Text="Mauritania" />
                  <asp:ListItem Value="MU" Text="Mauritius" />
                  <asp:ListItem Value="YT" Text="Mayotte" />
                  <asp:ListItem Value="MX" Text="Mexico" />
                  <asp:ListItem Value="FM" Text="Micronesia" />
                  <asp:ListItem Value="MD" Text="Moldova" />
                  <asp:ListItem Value="MC" Text="Monaco" />
                  <asp:ListItem Value="MN" Text="Mongolia" />
                  <asp:ListItem Value="MS" Text="Montserrat" />
                  <asp:ListItem Value="MA" Text="Morocco" />
                  <asp:ListItem Value="MZ" Text="Mozambique" />
                  <asp:ListItem Value="MM" Text="Myanmar" />
                  <asp:ListItem Value="NA" Text="Namibia" />
                  <asp:ListItem Value="NR" Text="Nauru" />
                  <asp:ListItem Value="NP" Text="Nepal" />
                  <asp:ListItem Value="NL" Text="Netherlands" />
                  <asp:ListItem Value="AN" Text="Netherlands Antilles" />
                  <asp:ListItem Value="NC" Text="New Caledonia" />
                  <asp:ListItem Value="NZ" Text="New Zealand" />
                  <asp:ListItem Value="NI" Text="Nicaragua" />
                  <asp:ListItem Value="NE" Text="Niger" />
                  <asp:ListItem Value="NG" Text="Nigeria" />
                  <asp:ListItem Value="NU" Text="Niue" />
                  <asp:ListItem Value="NF" Text="Norfolk Island" />
                  <asp:ListItem Value="MP" Text="Northern Mariana Islands" />
                  <asp:ListItem Value="NO" Text="Norway" />
                  <asp:ListItem Value="OM" Text="Oman" />
                  <asp:ListItem Value="PK" Text="Pakistan" />
                  <asp:ListItem Value="PW" Text="Palau" />
                  <asp:ListItem Value="PA" Text="Panama" />
                  <asp:ListItem Value="PG" Text="Papua New Guinea" />
                  <asp:ListItem Value="PY" Text="Paraguay" />
                  <asp:ListItem Value="PE" Text="Peru" />
                  <asp:ListItem Value="PH" Text="Philippines" />
                  <asp:ListItem Value="PN" Text="Pitcairn" />
                  <asp:ListItem Value="PL" Text="Poland" />
                  <asp:ListItem Value="PT" Text="Portugal" />
                  <asp:ListItem Value="PR" Text="Puerto Rico" />
                  <asp:ListItem Value="QA" Text="Qatar" />
                  <asp:ListItem Value="RE" Text="Reunion" />
                  <asp:ListItem Value="RO" Text="Romania" />
                  <asp:ListItem Value="RU" Text="Russian Federation" />
                  <asp:ListItem Value="RW" Text="Rwanda" />
                  <asp:ListItem Value="SH" Text="Saint Helena" />
                  <asp:ListItem Value="KN" Text="Saint Kitts and Nevis" />
                  <asp:ListItem Value="LC" Text="Saint Lucia" />
                  <asp:ListItem Value="PM" Text="Saint Pierre and Miquelon" />
                  <asp:ListItem Value="VC" Text="Saint Vincent" />
                  <asp:ListItem Value="WS" Text="Samoa" />
                  <asp:ListItem Value="SM" Text="San Marino" />
                  <asp:ListItem Value="ST" Text="Sao Tome and Principe" />
                  <asp:ListItem Value="SA" Text="Saudi Arabia" />
                  <asp:ListItem Value="SN" Text="Senegal" />
                  <asp:ListItem Value="RS" Text="Serbia" />
                  <asp:ListItem Value="SC" Text="Seychelles" />
                  <asp:ListItem Value="SL" Text="Sierra Leone" />
                  <asp:ListItem Value="SG" Text="Singapore" />
                  <asp:ListItem Value="SK" Text="Slovakia" />
                  <asp:ListItem Value="SI" Text="Slovenia" />
                  <asp:ListItem Value="SB" Text="Solomon Islands" />
                  <asp:ListItem Value="SO" Text="Somalia" />
                  <asp:ListItem Value="ZA" Text="South Africa" />
                  <asp:ListItem Value="ES" Text="Spain" />
                  <asp:ListItem Value="LK" Text="Sri Lanka" />
                  <asp:ListItem Value="SD" Text="Sudan" />
                  <asp:ListItem Value="SR" Text="Suriname" />
                  <asp:ListItem Value="SJ" Text="Svalbard and Jan Mayen" />
                  <asp:ListItem Value="SZ" Text="Swaziland" />
                  <asp:ListItem Value="SE" Text="Sweden" />
                  <asp:ListItem Value="CH" Text="Switzerland" />
                  <asp:ListItem Value="SY" Text="Syria" />
                  <asp:ListItem Value="TW" Text="Taiwan" />
                  <asp:ListItem Value="TJ" Text="Tajikistan" />
                  <asp:ListItem Value="TZ" Text="Tanzania" />
                  <asp:ListItem Value="TH" Text="Thailand" />
                  <asp:ListItem Value="TL" Text="Timor-leste" />
                  <asp:ListItem Value="TG" Text="Togo" />
                  <asp:ListItem Value="TK" Text="Tokelau" />
                  <asp:ListItem Value="TO" Text="Tonga" />
                  <asp:ListItem Value="TT" Text="Trinidad and Tobago" />
                  <asp:ListItem Value="TN" Text="Tunisia" />
                  <asp:ListItem Value="TR" Text="Turkey" />
                  <asp:ListItem Value="TM" Text="Turkmenistan" />
                  <asp:ListItem Value="TC" Text="Turks and Caicos Islands" />
                  <asp:ListItem Value="TV" Text="Tuvalu" />
                  <asp:ListItem Value="UG" Text="Uganda" />
                  <asp:ListItem Value="UA" Text="Ukraine" />
                  <asp:ListItem Value="AE" Text="United Arab Emirates" />
                  <asp:ListItem Value="GB" Text="United Kingdom" />
                  <asp:ListItem Value="UY" Text="Uruguay" />
                  <asp:ListItem Value="UZ" Text="Uzbekistan" />
                  <asp:ListItem Value="VU" Text="Vanuatu" />
                  <asp:ListItem Value="VA" Text="Vatican City (Holy See)" />
                  <asp:ListItem Value="VE" Text="Venezuela" />
                  <asp:ListItem Value="VN" Text="Viet Nam" />
                  <asp:ListItem Value="VG" Text="Virgin Islands, British" />
                  <asp:ListItem Value="VI" Text="Virgin Islands, U.S." />
                  <asp:ListItem Value="WF" Text="Wallis and Futuna" />
                  <asp:ListItem Value="EH" Text="Western Sahara" />
                  <asp:ListItem Value="YE" Text="Yemen" />
                  <asp:ListItem Value="ZM" Text="Zambia" />
                  <asp:ListItem Value="ZW" Text="Zimbabwe" />
                </asp:DropDownList>
                <asp:RequiredFieldValidator
                  ID="CountryCodeValidator1"
                  runat="server"
                  ControlToValidate="Country"
                  Display="Dynamic"
                  ErrorMessage="Country is required."
                  EnableViewState="false"
                  >*</asp:RequiredFieldValidator
                >
              </td>
            </tr>
            <tr>
              <th align="right" nowrap>
                <asp:Label
                  ID="PhoneLabel"
                  runat="server"
                  Text="Phone:"
                  EnableViewState="false"
                ></asp:Label
                ><br />
              </th>
              <td valign="top">
                <asp:TextBox
                  ID="Phone"
                  runat="server"
                  EnableViewState="false"
                ></asp:TextBox>
              </td>
              <th align="right" nowrap>
                <asp:Label
                  ID="FaxLabel"
                  runat="server"
                  Text="Fax:"
                  EnableViewState="false"
                ></asp:Label
                ><br />
              </th>
              <td valign="top">
                <asp:TextBox
                  ID="Fax"
                  runat="server"
                  EnableViewState="false"
                ></asp:TextBox>
              </td>
            </tr>
          </table>
        </ContentTemplate>
      </ajax:UpdatePanel>
      <div class="sectionHeader">
        <h2>
          <asp:Localize
            ID="DefaultDataCaption"
            runat="server"
            Text="Default Data"
          ></asp:Localize>
        </h2>
      </div>
      <ajax:UpdatePanel ID="DefaultDataAjax" runat="server">
        <ContentTemplate>
          <table cellpadding="4" cellspacing="0">
            <tr>
              <td colspan="2">
                <asp:Label
                  ID="DefaultDataHelpText"
                  runat="server"
                  Text="You can choose to install default data to aid in the creation of your store.  Any default data that you add can be modfied through the admin interface afterward."
                  EnableViewState="false"
                ></asp:Label>
              </td>
            </tr>
            <tr>
              <th align="right" nowrap>
                <asp:Label
                  ID="CountryDefaultsLabel"
                  runat="server"
                  Text="Countries:"
                  EnableViewState="false"
                ></asp:Label>
              </th>
              <td align="left">
                <asp:DropDownList ID="CountryDefaults" runat="server">
                  <asp:ListItem
                    Value="NorthAmerica"
                    Text="Canada, Mexico, and United States"
                  ></asp:ListItem>
                  <asp:ListItem
                    Value="All"
                    Text="All Countries"
                    Selected="true"
                  ></asp:ListItem>
                </asp:DropDownList>
              </td>
            </tr>
            <tr>
              <th align="right" nowrap>
                <asp:Label
                  ID="Label1"
                  runat="server"
                  Text="Basic Data:"
                  EnableViewState="false"
                ></asp:Label>
              </th>
              <td align="left">
                <asp:CheckBox
                  ID="BasicData"
                  runat="server"
                  Checked="true"
                  OnCheckedChanged="BasicData_CheckedChanged"
                  AutoPostBack="true"
                />
                Check this box to pre-populate defaults to help you get your
                store up and running quickly. Basic data covers many elements
                such as order statuses, email templates, and payment methods.
                Including this data is recommended.
              </td>
            </tr>
            <tr id="trAdditionalData" runat="server">
              <th align="right" nowrap>
                <asp:Label
                  ID="AdditionalDataLabel"
                  runat="server"
                  Text="Additional Data:"
                  EnableViewState="false"
                ></asp:Label>
              </th>
              <td align="left">
                <asp:CheckBox
                  ID="AdditionalData"
                  runat="server"
                  Checked="true"
                />
                Check this box to include additional data such as a product
                catalog, digital goods, gift wrapping, discounts and coupons.
                This data is recommended for evaluation purposes, but is not
                necessary when setting up an actual store.
              </td>
            </tr>
          </table>
        </ContentTemplate>
      </ajax:UpdatePanel>
      <div class="submit">
        <asp:Button
          ID="InstallButton"
          runat="server"
          Text="Install MakerShop 7!"
          OnClick="InstallButton_Click"
          CssClass="installButton"
          OnClientClick="if(Page_ClientValidate()){this.value='Processing...';return plswt();}"
        />
      </div>
    </form>
  </body>
</html>
