<%@ Page Language="C#" Title="Install MakerShop {0} (Step 1 of 2)" %> <%@ Import
Namespace="System.IO" %> <%@ Import Namespace="System.Net" %> <%@ Import
Namespace="System.Xml" %> <%@ Import Namespace="MakerShop.Data" %> <%@ Import
Namespace="MakerShop.Utility" %> <%@ Import Namespace="System.Data.SqlClient" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">

  private string _AcVersion = "7";

     private void HandleError(string errorMessage)
     {
         MessagePanel.Visible = true;
         ReponseMessage.Text = errorMessage;
     }

     private string UrlEncode(string key, string value)
     {        return key + "=" + Server.UrlEncode(value);
     }

     protected void Page_Load(object sender, EventArgs e)
     {
         Domain.Text = Request.Url.Authority;
         //CHECK WHETHER LOCAL DATABASE FILE EXISTS
         bool foundDb = File.Exists(Server.MapPath("~/App_Data/MakerShop.mdf"));
         DbLocalFoundPanel.Visible = foundDb;
  	_AcVersion = MakerShopVersion.Instance.Version;
  	Page.Title = string.Format(Page.Title, _AcVersion);
         Heading.InnerText = string.Format(Heading.InnerText, _AcVersion);
         DemoMessage.InnerText = string.Format(DemoMessage.InnerText, _AcVersion);
     }

     private void ChangeDbOption(int option)
     {
         DbSimplePanel.Visible = (option == 0);
         DbAdvancedPanel.Visible = (option == 1);
         DbLocalPanel.Visible = (option == 2);
     }

     protected void DbSimple_CheckedChanged(object sender, EventArgs e)
     {
         ChangeDbOption(0);
     }

     protected void DbAdvanced_CheckedChanged(object sender, EventArgs e)
     {
         ChangeDbOption(1);
     }

     protected void DbLocal_CheckedChanged(object sender, EventArgs e)
     {
         ChangeDbOption(2);
     }

     private void ChangeKeyOption(int option)
     {
         KeyDemoPanel.Visible = (option == 0);
         LicenseKeyPanel.Visible = (option == 1);
         KeyUploadPanel.Visible = (option == 2);
     }

     protected void KeyDemo_CheckedChanged(object sender, EventArgs e)
     {
         ChangeKeyOption(0);
     }

     protected void LicenseKeyOption_CheckedChanged(object sender, EventArgs e)
     {
         ChangeKeyOption(1);
     }

     protected void KeyUpload_CheckedChanged(object sender, EventArgs e)
     {
         ChangeKeyOption(2);
     }

     protected void InstallButton_Click(object sender, EventArgs e)
     {
         if (PCIAcknowledgement.Checked)
         {
             //VALIDATE THE CONNECTION
             string connectionString = GetConnectionString();
             if (ValidateConnection(connectionString))
             {
                 if (ValidateKey())
                 {
                     //SETUP THE DATABASE
                     Response.Write("<html><head><title>Install MakerShop " + _AcVersion + " (Step 1 of 2)</title><link href=\"../App_Themes/MakerShop/ComponentArt.css\" type=\"text/css\" rel=\"stylesheet\" /><link href=\"../App_Themes/MakerShop/print.css\" type=\"text/css\" rel=\"stylesheet\" /><link href=\"../App_Themes/MakerShop/style.css\" type=\"text/css\" rel=\"stylesheet\" /><link href=\"../App_Themes/MakerShop/webparts.css\" type=\"text/css\" rel=\"stylesheet\" /></head><body style=\"width:780px;margin:auto\">");
                     Response.Write("<br /><div class=\"pageHeader\"><h1 style=\"font-size:16px\">Initializing the database</h1></div>");
                     Response.Flush();
                     if (!DbLocal.Checked)
                     {
                         Database database = new SqlDatabase(connectionString);
                         string version = database.SqlServerMajorVersion < 9? "2000" : "2005";
                         SetupDatabase(connectionString, version);
                     }

                     //FLUSH THE OUTPUT TO MAKE SURE USER GETS THE RESPONSE
                     Response.Write("Initialization is complete!<br /><br />");
                     Response.Write("<div class=\"submit\"><a href=\"configure2.aspx\" class=\"button\">Click Here to Continue</a></div>");
                     Response.Write("</body></html>");
                     Response.Flush();

                     //WRITE WEB.CONFIG AND MOVE TO NEXT PAGE
                     DatabaseHelper.UpdateConnectionString(connectionString, true);
                     Response.End();
                 }
             }
             else
             {
                 HandleError("The specified database connection could not be opened.<br/><br/>Connection String: " + connectionString);
             }
         }
         else
         {
             HandleError("You must acknowledge that you have reviewed the secure implementation guide.");
         }
     }

     private string GetConnectionString()
     {
         if (DbSimple.Checked)
         {
             return string.Format("Server={0};Database={1};Uid={2};Pwd={3};", DbServerName.Text, DbDatabaseName.Text, DbUserName.Text, DbPassword.Text);
         }
         else if (DbAdvanced.Checked)
         {
  	 return DbConnectionString.Text;
         }
         else
             return @"Data Source=.\SQLExpress;Integrated Security=True;AttachDbFileName=|DataDirectory|\MakerShop.mdf;User Instance=true;";
     }

     protected bool ValidateConnection(string connectionString)
     {
         bool valid = false;
         using (SqlConnection conn = new SqlConnection(connectionString))
         {
             try
             {
                 conn.Open();
             }
             catch { }
             if (conn.State == System.Data.ConnectionState.Open)
             {
                 valid = true;
                 conn.Close();
             }
         }
         return valid;
     }

     private bool ValidateKey()
     {
         if (KeyDemo.Checked) return GetDemoKey();
         else if (LicenseKeyOption.Checked) return GetLicenseFile();
         else
         {
             if (Request.Files.Count > 0)
             {
                 string filePath = Server.MapPath("~/App_Data/MakerShop.lic");
                 Request.Files[0].SaveAs(filePath);
                 string keyText = FileHelper.ReadText(filePath);
                 MakerShop.Configuration.License lic = new MakerShop.Configuration.License(keyText);
                 if (lic.IsValid) return true;
                 else
                 {
                     File.Delete(filePath);
                     HandleError("The key file you uploaded is not a valid license key.");
                 }
             }
             else HandleError("No key file was uploaded.");
         }
         return false;
     }

     private bool GetDemoKey()
     {
         //CONSTRUCT FORM DATA
         StringBuilder formData = new StringBuilder();
         formData.Append(UrlEncode("Name", Name.Text));
         formData.Append("&" + UrlEncode("Company", Company.Text));
         formData.Append("&" + UrlEncode("Email", Email.Text));
         formData.Append("&" + UrlEncode("Phone", Phone.Text));
         formData.Append("&" + UrlEncode("Domain", Domain.Text));
         HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://www.MakerShop.com/livereg7/GetDemoKey.ashx");
         httpWebRequest.Method = "POST";
         httpWebRequest.ContentType = "application/x-www-form-urlencoded";
         byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(formData.ToString());
         httpWebRequest.ContentLength = requestBytes.Length;
         using (Stream requestStream = httpWebRequest.GetRequestStream())
         {
             requestStream.Write(requestBytes, 0, requestBytes.Length);
             requestStream.Close();
         }
         string response;
         using (StreamReader responseStream = new StreamReader(httpWebRequest.GetResponse().GetResponseStream(), System.Text.Encoding.UTF8))
         {
             response = responseStream.ReadToEnd();
             responseStream.Close();
         }
         //SPLIT INTO NAME/VALUE DICTIONARY
         AccountDataDictionary dictionary = new AccountDataDictionary(response);
         if (dictionary.ContainsKey("RESULT"))
         {
             if (dictionary["RESULT"] == "1")
             {
                 string keyText = dictionary["KEY"];
                 //WRITE THE KEY
                 string keyPath = Server.MapPath("~/App_Data/MakerShop.lic");
                 File.WriteAllText(keyPath, keyText);
                 return true;
             }
             else
             {
                 HandleError(dictionary["MESSAGE"]);
             }
         }
         else
         {
             HandleError("Unknown error occurred contacting the MakerShop registration service!");
         }
         return false;
     }

     private bool GetLicenseFile()
     {
         //CONSTRUCT FORM DATA
         StringBuilder formData = new StringBuilder();
         formData.Append(UrlEncode("Key", LicenseKey.Text));
         formData.Append("&" + UrlEncode("Path", Server.MapPath("~").ToLowerInvariant() + "\\"));
         HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://www.MakerShop.com/livereg7/GetKey.ashx");
         httpWebRequest.Method = "POST";
         httpWebRequest.ContentType = "application/x-www-form-urlencoded";
         byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(formData.ToString());
         httpWebRequest.ContentLength = requestBytes.Length;
         using (Stream requestStream = httpWebRequest.GetRequestStream())
         {
             requestStream.Write(requestBytes, 0, requestBytes.Length);
             requestStream.Close();
         }
         string response;
         using (StreamReader responseStream = new StreamReader(httpWebRequest.GetResponse().GetResponseStream(), System.Text.Encoding.UTF8))
         {
             response = responseStream.ReadToEnd();
             responseStream.Close();
         }
         //SPLIT INTO NAME/VALUE DICTIONARY
         UrlEncodedDictionary dictionary = new UrlEncodedDictionary(response);
         if (dictionary.ContainsKey("RESULT"))
         {
             if (dictionary["RESULT"] == "1")
             {
                 string keyText = dictionary["KEY"];
                 MakerShop.Configuration.License lic = new MakerShop.Configuration.License(keyText);
                 if (lic.IsValid)
                 {
                     //WRITE THE KEY
                     string keyPath = Server.MapPath("~/App_Data/MakerShop.lic");
                     File.WriteAllText(keyPath, keyText);
                     return true;
                 }
                 else
                 {
                     //SOMETHING IS WRONG WITH THIS KEY
                     if (lic.LastException != null) HandleError(lic.LastException.Message);
                     else HandleError("There is an unknown problem with the specified key.");
                     return false;
                 }
             }
             else
             {
                 HandleError(dictionary["MESSAGE"]);
             }
         }
         else
         {
             HandleError("Unknown error occurred contacting the MakerShop registration service!");
         }
         return false;
     }

     private void SetupDatabase(string connectionString, string dbVersion)
     {
         string dbScriptName = ((dbVersion != "2000") ? "MakerShop.sql" : "MakerShop2000.sql");
         List<string> errorList = new List<string>();
         RunScript(connectionString, Server.MapPath("~/install/" + dbScriptName), errorList);
         RunScript(connectionString, Server.MapPath("~/install/DefaultIndexes.sql"), errorList);
         Response.Write("<br /><br />");
         if (errorList.Count > 0)
         {
             Response.Write("<b>There were errors during the database creation:</b><br /><br />");
             Response.Write(string.Join("<br /><br />", errorList.ToArray()));
             Response.Write("<br /><br />");
         }
     }

     private void RunScript(string connectionString, string dbScriptFile, List<string> errorList)
     {
         string dbScript = FileHelper.ReadText(dbScriptFile);
         //REMOVE SCRIPT COMMENTS
         dbScript = Regex.Replace(dbScript, @"/\*.*?\*/", string.Empty);
         //SPLIT INTO COMMANDS
         string[] commands = StringHelper.Split(dbScript, "\r\ngo\r\n");
         //SETUP DATABASE CONNECTION
         int errorCount = 0;
         using (SqlConnection conn = new SqlConnection(connectionString))
         {
             conn.Open();
             //LOOP THE COMANDS AND RUN ON DB
             foreach (string sql in commands)
             {
                 try
                 {
                     SqlCommand command = new SqlCommand(sql, conn);
                     command.ExecuteNonQuery();
                     Response.Write("o ");
                     Response.Flush();
                 }
                 catch (Exception ex)
                 {
                     errorCount++;
                     Response.Write("x ");
                     Response.Flush();
                     errorList.Add("<b>SQL:</b> " + sql);
                     errorList.Add("<b>Error:</b> " + ex.Message);
                 }
             }
             conn.Close();
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
        padding: 3px;
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
        text-align: center;
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
      <br />
      <div class="pageHeader">
        <h1 style="font-size: 16px" runat="server" id="Heading">
          Install MakerShop {0} (Step 1 of 2)
        </h1>
      </div>
      <div style="padding-left: 10px; padding-right: 10px">
        <asp:Panel ID="MessagePanel" runat="server" Visible="false">
          <div class="error">
            <asp:Literal ID="ReponseMessage" runat="server"></asp:Literal>
          </div>
        </asp:Panel>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
        <ajax:ScriptManager
          ID="ScriptManager"
          runat="server"
          EnablePartialRendering="true"
        ></ajax:ScriptManager>
        <ajax:UpdatePanel ID="InstallAjax" runat="server">
          <Triggers>
            <asp:PostBackTrigger ControlID="InstallButton" />
            <asp:PostBackTrigger ControlID="KeyUpload" />
          </Triggers>
          <ContentTemplate>
            <asp:Panel ID="FormPanel" runat="server">
              <p>
                Provide your license key and database connection to complete the
                first step of installation.
              </p>
              <div class="sectionHeader">
                <h2>License Key</h2>
              </div>
              <div class="radio">
                <asp:RadioButton
                  ID="KeyDemo"
                  runat="server"
                  Text="Use a 30-day demo key"
                  OnCheckedChanged="KeyDemo_CheckedChanged"
                  AutoPostBack="true"
                  GroupName="KeyOption"
                />
              </div>
              <asp:PlaceHolder ID="KeyDemoPanel" runat="server" Visible="false">
                <div class="inputBox">
                  <p runat="server" id="DemoMessage">
                    To register for a 30 day demo key for MakerShop {0}, fill in
                    the form below. Be aware that you can only register the
                    listed domain one time using this form. For extended demo
                    keys, you must contact us at 1-866-571-5888.
                  </p>
                  <table cellpadding="5" cellspacing="0">
                    <tr>
                      <th align="right" valign="top">Domain:</th>
                      <td>
                        <asp:Literal ID="Domain" runat="server"></asp:Literal>
                      </td>
                    </tr>
                    <tr>
                      <th align="right" valign="top">Email:</th>
                      <td>
                        <asp:TextBox
                          ID="Email"
                          runat="server"
                          MaxLength="200"
                        ></asp:TextBox>
                        required
                        <asp:RequiredFieldValidator
                          ID="EmailRequired"
                          runat="server"
                          Text="*"
                          ErrorMessage="You must enter your email."
                          ControlToValidate="Email"
                        ></asp:RequiredFieldValidator
                        ><br />
                        (e.g. name@yourdomain.xyz)
                      </td>
                    </tr>
                    <tr>
                      <th align="right">Name:</th>
                      <td>
                        <asp:TextBox
                          ID="Name"
                          runat="server"
                          MaxLength="50"
                        ></asp:TextBox>
                        required
                        <asp:RequiredFieldValidator
                          ID="NameRequired"
                          runat="server"
                          Text="*"
                          ErrorMessage="You must enter your name."
                          ControlToValidate="Name"
                        ></asp:RequiredFieldValidator>
                      </td>
                    </tr>
                    <tr>
                      <th align="right">Company:</th>
                      <td>
                        <asp:TextBox
                          ID="Company"
                          runat="server"
                          MaxLength="50"
                        ></asp:TextBox>
                      </td>
                    </tr>
                    <tr>
                      <th align="right">Phone:</th>
                      <td>
                        <asp:TextBox
                          ID="Phone"
                          runat="server"
                          MaxLength="50"
                        ></asp:TextBox>
                      </td>
                    </tr>
                  </table>
                </div>
                <br />
              </asp:PlaceHolder>
              <div class="radio">
                <asp:RadioButton
                  ID="LicenseKeyOption"
                  runat="server"
                  Checked="true"
                  Text="Provide my license key"
                  OnCheckedChanged="LicenseKeyOption_CheckedChanged"
                  AutoPostBack="true"
                  GroupName="KeyOption"
                />
              </div>
              <asp:PlaceHolder ID="LicenseKeyPanel" runat="server">
                <div class="inputBox">
                  <table cellpadding="5" cellspacing="0">
                    <tr>
                      <th align="right" valign="top">License Key:</th>
                      <td>
                        <asp:TextBox
                          ID="LicenseKey"
                          runat="server"
                          Text=""
                          MaxLength="38"
                          Width="300px"
                        ></asp:TextBox>
                        <asp:RequiredFieldValidator
                          ID="LicenseKeyRequired"
                          runat="server"
                          Text="*"
                          ErrorMessage="You must enter the license key."
                          ControlToValidate="LicenseKey"
                        ></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator
                          ID="LicenseKeyFormat"
                          runat="server"
                          Text="*"
                          ErrorMessage="Your license key does not match the expected format."
                          ControlToValidate="LicenseKey"
                          ValidationExpression="^\{?[A-Fa-f0-9]{8}-?([A-Fa-f0-9]{4}-?){3}[A-Fa-f0-9]{12}\}?$"
                        ></asp:RegularExpressionValidator>
                        <br />(e.g. FD6B09C0-2AC9-4059-AE89-F27AB9285AAF)
                      </td>
                    </tr>
                  </table>
                </div>
                <br />
              </asp:PlaceHolder>
              <div class="radio">
                <asp:RadioButton
                  ID="KeyUpload"
                  runat="server"
                  Checked="false"
                  Text="Upload my license file"
                  OnCheckedChanged="KeyUpload_CheckedChanged"
                  AutoPostBack="true"
                  GroupName="KeyOption"
                />
              </div>
              <asp:PlaceHolder
                ID="KeyUploadPanel"
                runat="server"
                Visible="false"
              >
                <div class="inputBox">
                  <p>
                    If you have an existing MakerShop.lic file, upload it here.
                  </p>
                  <table cellpadding="5" cellspacing="0">
                    <tr>
                      <th align="right" valign="top">Key File:</th>
                      <td>
                        <asp:FileUpload
                          ID="KeyFile"
                          runat="server"
                          Width="200px"
                        />
                      </td>
                    </tr>
                  </table>
                </div>
                <br />
              </asp:PlaceHolder>
              <div class="sectionHeader">
                <h2>Database Connection</h2>
              </div>
              <p>Specify the database that will be used by MakerShop:</p>
              <br />
              <div class="radio">
                <asp:RadioButton
                  ID="DbSimple"
                  runat="server"
                  Checked="true"
                  Text="Specify database"
                  OnCheckedChanged="DbSimple_CheckedChanged"
                  AutoPostBack="true"
                  GroupName="DbOption"
                />
              </div>
              <asp:PlaceHolder ID="DbSimplePanel" runat="server" Visible="true">
                <div class="inputBox">
                  <p>
                    To use this option, the database you specify must already
                    exist. Also, the user name you provide must have permission
                    to create tables and indexes.
                  </p>
                  <table cellpadding="5" cellspacing="0">
                    <tr>
                      <th align="right" valign="top">Server Name:</th>
                      <td>
                        <asp:TextBox
                          ID="DbServerName"
                          runat="server"
                          MaxLength="100"
                        ></asp:TextBox>
                        <asp:RequiredFieldValidator
                          ID="DbServerNameRequired"
                          runat="server"
                          Text="*"
                          ErrorMessage="You must enter the host name of the database server."
                          ControlToValidate="DbServerName"
                        ></asp:RequiredFieldValidator
                        ><br />
                        <p>
                          You can enter <b>.</b> if the database server is the
                          same as the web server.
                        </p>
                      </td>
                    </tr>
                    <tr>
                      <th align="right" valign="top">Database Name:</th>
                      <td>
                        <asp:TextBox
                          ID="DbDatabaseName"
                          runat="server"
                          MaxLength="100"
                        ></asp:TextBox>
                        <asp:RequiredFieldValidator
                          ID="DbDatabaseNameRequired"
                          runat="server"
                          Text="*"
                          ErrorMessage="You must enter the name of the database."
                          ControlToValidate="DbDatabaseName"
                        ></asp:RequiredFieldValidator
                        ><br />
                      </td>
                    </tr>
                    <tr>
                      <th align="right" valign="top">Database User:</th>
                      <td>
                        <asp:TextBox
                          ID="DbUserName"
                          runat="server"
                          MaxLength="50"
                        ></asp:TextBox>
                        <asp:RequiredFieldValidator
                          ID="DbUserNameValidator"
                          runat="server"
                          Text="*"
                          ErrorMessage="You must enter the user name for the database."
                          ControlToValidate="DbUserName"
                        ></asp:RequiredFieldValidator
                        ><br />
                      </td>
                    </tr>
                    <tr>
                      <th align="right" valign="top">Database Password:</th>
                      <td>
                        <asp:TextBox
                          ID="DbPassword"
                          runat="server"
                          MaxLength="40"
                          TextMode="Password"
                        ></asp:TextBox>
                        <asp:RequiredFieldValidator
                          ID="DbPasswordValidator"
                          runat="server"
                          Text="*"
                          ErrorMessage="You must enter the password for the database user."
                          ControlToValidate="DbPassword"
                        ></asp:RequiredFieldValidator
                        ><br />
                      </td>
                    </tr>
                  </table>
                </div>
                <br />
              </asp:PlaceHolder>
              <div class="radio">
                <asp:RadioButton
                  ID="DbAdvanced"
                  runat="server"
                  Checked="false"
                  Text="Specify connection string (Advanced)"
                  OnCheckedChanged="DbAdvanced_CheckedChanged"
                  AutoPostBack="true"
                  GroupName="DbOption"
                />
              </div>
              <asp:PlaceHolder
                ID="DbAdvancedPanel"
                runat="server"
                Visible="false"
              >
                <div class="inputBox">
                  <p>
                    To use this option, specify the connection string for the
                    database you want to use.
                  </p>
                  <table cellpadding="5" cellspacing="0">
                    <tr>
                      <th align="right">Connection String:</th>
                      <td>
                        <asp:TextBox
                          ID="DbConnectionString"
                          runat="server"
                          MaxLength="200"
                          Width="300px"
                        ></asp:TextBox>
                        <asp:RequiredFieldValidator
                          ID="DbConnectionStringRequired"
                          runat="server"
                          Text="*"
                          ErrorMessage="You must enter the database connection string."
                          ControlToValidate="DbConnectionString"
                        ></asp:RequiredFieldValidator>
                      </td>
                    </tr>
                  </table>
                </div>
                <br />
              </asp:PlaceHolder>
              <asp:PlaceHolder ID="DbLocalFoundPanel" runat="server">
                <div class="radio">
                  <asp:RadioButton
                    ID="DbLocal"
                    runat="server"
                    Checked="false"
                    Text="Use supplied SQL Server 2005 database"
                    OnCheckedChanged="DbLocal_CheckedChanged"
                    AutoPostBack="true"
                    GroupName="DbOption"
                  />
                </div>
                <asp:PlaceHolder
                  ID="DbLocalPanel"
                  runat="server"
                  Visible="false"
                >
                  <div class="inputBox">
                    <p>
                      To use this option, you must ensure that the server
                      running MakerShop has SQL Server 2005 Installed. The
                      freely available SQL Server 2005 Express edition is
                      supported. User instances must not be disabled by the
                      database administrator.
                    </p>
                  </div>
                  <br />
                </asp:PlaceHolder>
              </asp:PlaceHolder>
              <div class="sectionHeader">
                <h2>PCI Compliance</h2>
              </div>
              <p>
                MakerShop provides documentation and a moderated forum to assist
                you with configuring MakerShop in a PCI compliant manner. Please
                review the secure implementation guide prior to installation of
                MakerShop.
              </p>
              <br />
              <p>
                The secure implementation guide is at:
                <a
                  href="http://www.MakerShop.com/ac7pciguide.pdf"
                  target="_blank"
                  >http://www.MakerShop.com/ac7pciguide.pdf</a
                ><br /> The moderated forum is at:
                <a
                  href="http://forums.MakerShop.com/viewforum.php?f=46"
                  target="_blank"
                  >http://forums.MakerShop.com/viewforum.php?f=46</a
                >
              </p>
              <br />
              <asp:CheckBox
                ID="PCIAcknowledgement"
                runat="server"
                Checked="false"
              /><b>
                Check here to acknowledge that you have reviewed the secure
                implementation guide.</b
              ><br /><br />
            </asp:Panel>
            <br />
            <div class="submit">
              <asp:Button
                ID="InstallButton"
                runat="server"
                Text="Continue Install >"
                OnClick="InstallButton_Click"
                OnClientClick="if(Page_ClientValidate()){this.value='Processing...';return plswt();}"
              />
            </div>
          </ContentTemplate>
        </ajax:UpdatePanel>
      </div>
    </form>
  </body>
</html>
