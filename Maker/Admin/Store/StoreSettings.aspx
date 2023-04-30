<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"
CodeFile="StoreSettings.aspx.cs" Inherits="Admin_Store_StoreSettings"
Title="Global Settings" %> <%@ Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">
  <div class="pageHeader">
    <div class="caption">
      <h1>
        <asp:Localize
          ID="Caption"
          runat="server"
          Text="Configure Store"
        ></asp:Localize>
      </h1>
    </div>
  </div>
  <table cellpadding="2" cellspacing="2" class="innerLayout">
    <tr>
      <td colspan="2">
        <asp:ValidationSummary ID="ValidationSummary2" runat="server" />
        <asp:Label
          ID="SavedMessage"
          runat="server"
          Text="The store settings have been saved."
          Visible="false"
          SkinID="GoodCondition"
        ></asp:Label>
      </td>
    </tr>
    <tr>
      <td align="left" valign="top" width="50%">
        <div class="section" style="padding: 0 0 2px 0">
          <div class="header">
            <h2 class="commonicon">
              <asp:Localize
                ID="GeneralCaption"
                runat="server"
                Text="General"
              ></asp:Localize>
            </h2>
          </div>
          <div class="content">
            <table class="inputForm">
              <tr>
                <th class="rowHeader">
                  <asp:Label
                    ID="StoreNameLabel"
                    runat="Server"
                    Text="Store Name:"
                    AssociatedControlID="StoreName"
                    ToolTip="Name of the store"
                  ></asp:Label>
                </th>
                <td>
                  <asp:TextBox
                    ID="StoreName"
                    runat="server"
                    MaxLength="100"
                  ></asp:TextBox>
                </td>
              </tr>
              <tr>
                <th class="rowHeader">
                  <asp:Label
                    ID="StoreUrlLabel"
                    runat="Server"
                    Text="Store URL:"
                    AssociatedControlID="StoreUrl"
                    ToolTip="Base URL of the store"
                  ></asp:Label>
                </th>
                <td>
                  <asp:TextBox
                    ID="StoreUrl"
                    runat="server"
                    Width="200px"
                    MaxLength="200"
                  ></asp:TextBox>
                  <asp:RegularExpressionValidator
                    ID="StoreUrlValidator"
                    runat="server"
                    ControlToValidate="StoreUrl"
                    Display="Static"
                    ErrorMessage="Store URL should be in the format of http://domain/directory/.  It should not include a page name and should end with the / slash."
                    Text="*"
                    ValidationExpression="^https?://([^/]+?/)+$"
                  ></asp:RegularExpressionValidator>
                </td>
              </tr>
              <tr>
                <th class="rowHeader">
                  <asp:Label
                    ID="SSLEnabledLabel"
                    runat="Server"
                    Text="SSL Enabled"
                    AssociatedControlID="SSLEnabled"
                    ToolTip="Indicates whether SSL is enabled for the store"
                  ></asp:Label>
                </th>
                <td>
                  <asp:LinkButton
                    ID="SSLEnabled"
                    runat="server"
                    OnClick="SSLEnabled_Click"
                  ></asp:LinkButton>
                </td>
              </tr>
              <tr>
                <th class="rowHeader">
                  <asp:Label
                    ID="SiteDisclaimerLabel"
                    runat="server"
                    Text="Site Disclaimer Message"
                  ></asp:Label
                  ><br />
                  <asp:Label
                    ID="SiteDisclaimerHelpLabel"
                    CssClass="helpText"
                    runat="server"
                    Text="When any customer visits the website for the first time during each session he has to agree these terms and conditions and disclaimer message, leave empty if you dont want to enforce any terms and conditions."
                  ></asp:Label
                  ><br />
                </th>
                <td>
                  <asp:TextBox
                    ID="SiteDisclaimerMessage"
                    runat="server"
                    Rows="7"
                    TextMode="MultiLine"
                    Width="260px"
                    Columns="50"
                  ></asp:TextBox>
                </td>
              </tr>
            </table>
          </div>
        </div>
        <div class="section" style="padding: 0 0 2px 0">
          <div class="header">
            <h2 class="commonicon">
              <asp:Localize
                ID="OrderSettingCaption"
                runat="server"
                Text="Order Settings"
              ></asp:Localize>
            </h2>
          </div>
          <div class="content">
            <asp:Label
              ID="NextOrderNumberWarning"
              runat="server"
              SkinID="ErrorCondition"
              Text="The next order number of {0} is less than the highest assigned order number of {1}.  It is recommended that you increase your next order number to at least {2} to prevent errors."
              EnableViewState="false"
            ></asp:Label>
            <table class="inputForm">
              <tr>
                <th class="rowHeader">
                  <cb:ToolTipLabel
                    ID="NextOrderNumberLabel"
                    runat="server"
                    Text="Next Order Number:"
                    ToolTip="The number that will be assigned to the next order placed in the store. It must be greater than the highest assigned order number."
                  >
                  </cb:ToolTipLabel>
                  <br />
                </th>
                <td>
                  <asp:HiddenField ID="OrigNextOrderNumber" runat="server" />
                  <asp:TextBox
                    ID="NextOrderId"
                    runat="server"
                    Columns="8"
                    MaxLength="8"
                  ></asp:TextBox>
                  <asp:Label
                    ID="NextOrderIdLabel"
                    runat="server"
                    Visible="false"
                    EnableViewState="false"
                  ></asp:Label>
                  <asp:RangeValidator
                    ID="NextOrderNumberRangeValidator1"
                    runat="server"
                    Type="Integer"
                    MinimumValue="0"
                    MaximumValue="99999999"
                    ControlToValidate="NextOrderId"
                    ErrorMessage="Next order number must be a numeric value between '{0}' and '99,999,999'."
                    Text="*"
                    EnableViewState="false"
                  ></asp:RangeValidator>
                </td>
              </tr>
              <tr>
                <th class="rowHeader">
                  <cb:ToolTipLabel
                    ID="OrderNumberIncrementLabel"
                    runat="server"
                    Text="Increment:"
                    ToolTip="The number to add to the next order number for each order that is placed.  Use 1 for sequential order numbers."
                  >
                  </cb:ToolTipLabel>
                </th>
                <td>
                  <asp:TextBox
                    ID="OrderIdIncrement"
                    runat="server"
                    Columns="4"
                    MaxLength="4"
                  ></asp:TextBox>
                  <asp:RangeValidator
                    ID="OrderNumberIncrementValidator1"
                    runat="server"
                    Type="Integer"
                    MinimumValue="0"
                    MaximumValue="9999"
                    ControlToValidate="OrderIdIncrement"
                    ErrorMessage="Order number increment must be a numeric value."
                    Text="*"
                  ></asp:RangeValidator>
                </td>
              </tr>
              <tr>
                <th class="rowHeader">
                  <cb:ToolTipLabel
                    ID="OrderMinAmountLabel"
                    runat="server"
                    Text="Order Minimum Amount"
                    ToolTip="If the order amount will be less then the minimum then it will not allowed to checkout"
                  >
                  </cb:ToolTipLabel>
                  <br />
                </th>
                <td>
                  <asp:TextBox
                    ID="OrderMinAmount"
                    runat="server"
                    Columns="8"
                  ></asp:TextBox>
                  <asp:RangeValidator
                    ID="OrderMinAmountValidator1"
                    runat="server"
                    Type="Double"
                    MinimumValue="0"
                    MaximumValue="999999999"
                    ControlToValidate="OrderMinAmount"
                    ErrorMessage="Order minimum amount must be a numeric value."
                    Text="*"
                  ></asp:RangeValidator>
                </td>
              </tr>
              <tr>
                <th class="rowHeader">
                  <cb:ToolTipLabel
                    ID="OrderMaxAmountLabel"
                    runat="server"
                    Text="Order Maximum Amount"
                    ToolTip="If the order amount will be more then the maximum then it will not allowed to checkout"
                  >
                  </cb:ToolTipLabel>
                  <br />
                </th>
                <td>
                  <asp:TextBox
                    ID="OrderMaxAmount"
                    runat="server"
                    Columns="8"
                  ></asp:TextBox>
                  <asp:RangeValidator
                    ID="OrderMaxAmountValidator1"
                    runat="server"
                    Type="Double"
                    MinimumValue="0"
                    MaximumValue="999999999"
                    ControlToValidate="OrderMaxAmount"
                    ErrorMessage="Order maximum amount must be a numeric value.<br/>"
                    Text="*"
                  ></asp:RangeValidator>
                  <asp:CompareValidator
                    ID="OrderMinMaxAmountValidator1"
                    runat="server"
                    Type="Double"
                    Operator="GreaterThanEqual"
                    ControlToValidate="OrderMaxAmount"
                    ControlToCompare="OrderMinAmount"
                    ErrorMessage="Order maximum amount should be greater then minimum amount."
                    Text="*"
                  ></asp:CompareValidator>
                </td>
              </tr>
              <tr>
                <th class="rowHeader">
                  <asp:Label
                    ID="CheckoutTermsLabel"
                    runat="server"
                    Text="Checkout Terms & Conditions"
                  ></asp:Label
                  ><br />
                  <asp:Label
                    ID="CheckoutTermsHelpLabel"
                    CssClass="helpText"
                    runat="server"
                    Text="When placing an order the customer has to agree these terms and conditions, leave empty if you dont want to enforce any terms and conditions."
                  ></asp:Label
                  ><br />
                </th>
                <td>
                  <asp:TextBox
                    ID="CheckoutTerms"
                    runat="server"
                    Rows="5"
                    TextMode="MultiLine"
                    Width="220px"
                  ></asp:TextBox>
                </td>
              </tr>
            </table>
          </div>
        </div>
        <div class="section" style="padding: 0 0 2px 0">
          <div class="header">
            <h2 class="catalogmode">
              <asp:Localize
                ID="ProductPurchasingCaption"
                runat="server"
                Text="Catalog Mode"
              ></asp:Localize>
            </h2>
          </div>
          <div class="content">
            <asp:Label
              ID="ProductPurchasingDisabledLabel"
              runat="server"
              Text="Enabling this feature will hide the add to basket button for all products, so customers can browse your catalog but are unable to purchase directly online."
            ></asp:Label
            ><br />
            <asp:CheckBox
              ID="ProductPurchasingDisabled"
              runat="server"
              Text="Enable Catalog Mode"
            />
          </div>
        </div>
        <div class="section" style="padding: 0 0 2px 0">
          <div class="header">
            <h2 class="commonicon">
              <asp:Localize
                ID="FullTextSearchCaption"
                runat="server"
                Text="Full Text Search"
              ></asp:Localize>
            </h2>
          </div>
          <div class="content">
            <asp:Localize
              ID="FullTextSearchLabel"
              runat="server"
              Text="Enabling full text search can improve the response times for searches performed on your product catalog."
            ></asp:Localize>
            <asp:Localize
              ID="FullTextSearchNotInstalledLabel"
              runat="server"
              Text="  Unfortunately, your database does not appear to have full text search installed or enabled.  If you have a large product catalog, you should ask your server administrator to enable the full text search for your database."
              Visible="false"
            ></asp:Localize>
            <asp:Localize
              ID="FullTextSearchVersionErrorLabel"
              runat="server"
              Text="  Unfortunately, full text searching is only supported in SQL Server 2005 and higher.  You will have to upgrade your database to enable the full text search option."
              Visible="false"
            ></asp:Localize>
            <br />
            <br />
            <asp:CheckBox
              ID="EnableFullTextSearch"
              runat="server"
              Text="Enable Full Text Searching"
              Visible="false"
            />
          </div>
        </div>
      </td>
      <td valign="top" width="50%">
        <div class="section" style="padding: 0 0 2px 0">
          <div class="header">
            <h2 class="localesettings">
              <asp:Localize
                ID="UnitsCaption"
                runat="server"
                Text="Locale Settings"
              ></asp:Localize>
            </h2>
          </div>
          <div class="content">
            <asp:Label
              ID="UnitsHelpText"
              runat="server"
              Text="You must specify the units of weight and measure for your store.  The unit chosen here applies to all product weights and measurements."
            ></asp:Label
            ><br />
            <br />
            <table class="inputForm">
              <tr>
                <th class="rowHeader" valign="top">
                  <cb:ToolTipLabel
                    ID="WeightUnitLabel"
                    runat="server"
                    Text="Unit of Weight:"
                    ToolTip="If you enter weights for your products, select the units that will be used."
                    AssociatedControlID="WeightUnit"
                  ></cb:ToolTipLabel>
                </th>
                <td>
                  <asp:DropDownList ID="WeightUnit" runat="server">
                  </asp:DropDownList>
                </td>
              </tr>
              <tr>
                <th class="rowHeader" valign="top">
                  <cb:ToolTipLabel
                    ID="MeasurementUnitLabel"
                    runat="server"
                    Text="Unit of Measurement:"
                    ToolTip="If you enter dimensions for your products, select the units that will be used."
                    AssociatedControlID="MeasurementUnit"
                  ></cb:ToolTipLabel>
                </th>
                <td>
                  <asp:DropDownList ID="MeasurementUnit" runat="server">
                  </asp:DropDownList>
                </td>
              </tr>
              <tr>
                <th class="rowHeader" valign="top">
                  <cb:ToolTipLabel
                    ID="TimeZoneOffsetLabel"
                    runat="server"
                    Text="TimeZone Offset:"
                    ToolTip="Select the timezone that is currently in effect for this store.  All times displayed will be converted into the time zone you select."
                    AssociatedControlID="TimeZoneOffset"
                  ></cb:ToolTipLabel>
                </th>
                <td>
                  <asp:DropDownList ID="TimeZoneOffset" runat="server">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem
                      Text="Atlantic Standard Time"
                      Value="AST"
                    ></asp:ListItem>
                    <asp:ListItem
                      Text="Atlantic Daylight Time"
                      Value="ADT"
                    ></asp:ListItem>
                    <asp:ListItem
                      Text="Alaska Standard Time"
                      Value="AKST"
                    ></asp:ListItem>
                    <asp:ListItem
                      Text="Alaska Daylight Time"
                      Value="AKDT"
                    ></asp:ListItem>
                    <asp:ListItem
                      Text="Central Standard Time"
                      Value="CST"
                    ></asp:ListItem>
                    <asp:ListItem
                      Text="Central Daylight Time"
                      Value="CDT"
                    ></asp:ListItem>
                    <asp:ListItem
                      Text="Eastern Standard Time"
                      Value="EST"
                    ></asp:ListItem>
                    <asp:ListItem
                      Text="Eastern Daylight Time"
                      Value="EDT"
                    ></asp:ListItem>
                    <asp:ListItem
                      Text="Hawaii Standard Time"
                      Value="HAST"
                    ></asp:ListItem>
                    <asp:ListItem
                      Text="Hawaii Daylight Time"
                      Value="HADT"
                    ></asp:ListItem>
                    <asp:ListItem
                      Text="Mountain Standard Time"
                      Value="MST"
                    ></asp:ListItem>
                    <asp:ListItem
                      Text="Mountain Daylight Time"
                      Value="MDT"
                    ></asp:ListItem>
                    <asp:ListItem
                      Text="Newfoundland Standard Time"
                      Value="NST"
                    ></asp:ListItem>
                    <asp:ListItem
                      Text="Newfoundland Daylight Time"
                      Value="NDT"
                    ></asp:ListItem>
                    <asp:ListItem
                      Text="Pacific Standard Time"
                      Value="PST"
                    ></asp:ListItem>
                    <asp:ListItem
                      Text="Pacific Daylight Time"
                      Value="PDT"
                    ></asp:ListItem>
                    <asp:ListItem
                      Text="-----------------"
                      Value=""
                    ></asp:ListItem>
                  </asp:DropDownList>
                </td>
              </tr>
              <tr>
                <th class="rowHeader" valign="top">
                  <cb:ToolTipLabel
                    ID="PostalCodeCountriesLabel"
                    runat="server"
                    Text="Postal Code Countries:"
                    ToolTip="Comma delimited list of two letter country codes that should require a postal code on address entry forms."
                  >
                  </cb:ToolTipLabel>
                </th>
                <td>
                  <asp:TextBox
                    ID="PostalCodeCountries"
                    runat="server"
                  ></asp:TextBox>
                </td>
              </tr>
            </table>
          </div>
        </div>
        <div class="section" style="padding: 0 0 2px 0">
          <div class="header">
            <h2 class="inventory">
              <asp:Localize
                ID="InventoryCaption"
                runat="server"
                Text="Inventory"
              ></asp:Localize>
            </h2>
          </div>
          <div class="content">
            <asp:Label
              ID="InventoryHelpText"
              runat="server"
              Text="Use the inventory management features to track stock quantities in your store."
            ></asp:Label
            ><br />
            <asp:CheckBox
              ID="EnableInventory"
              runat="server"
              Text="Enable Inventory Management"
            >
            </asp:CheckBox>
            <ajax:UpdatePanel ID="InventoryAjax" runat="server">
              <ContentTemplate>
                <table class="inputForm">
                  <tr>
                    <th class="rowHeader" valign="top">
                      <cb:ToolTipLabel
                        ID="CurrentInventoryDisplayModeLabel"
                        runat="server"
                        Text="Display Inventory:"
                        ToolTip="Display the inventory details of the products on store."
                      ></cb:ToolTipLabel>
                    </th>
                    <td>
                      <asp:DropDownList
                        ID="CurrentInventoryDisplayMode"
                        runat="server"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="CurrentInventoryDisplayMode_SelectedIndexChanged"
                      >
                        <asp:ListItem Value="0" Text="No"></asp:ListItem>
                        <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                      </asp:DropDownList>
                    </td>
                  </tr>
                </table>
                <div id="ProductInventoryPanel" runat="server" visible="false">
                  <table class="inputForm">
                    <tr>
                      <th class="rowHeader" align="right">
                        <cb:ToolTipLabel
                          ID="InStockMessageLabel"
                          runat="server"
                          Text="In Stock Message:"
                          ToolTip="The message that will be displayed on store when the product will be in stock."
                        >
                        </cb:ToolTipLabel>
                      </th>
                      <td>
                        <asp:TextBox
                          ID="InStockMessage"
                          runat="server"
                          Columns="40"
                          MaxLength="200"
                          Text="{0} units available"
                        ></asp:TextBox>
                      </td>
                    </tr>
                    <tr>
                      <th class="rowHeader" align="right">
                        <cb:ToolTipLabel
                          ID="OutOfStockMessageLabel"
                          runat="server"
                          Text="Out of stock message:"
                          ToolTip="The message that will be displayed on store when the product will be out of stock."
                        >
                        </cb:ToolTipLabel>
                      </th>
                      <td>
                        <asp:TextBox
                          ID="OutOfStockMessage"
                          runat="server"
                          Columns="40"
                          MaxLength="200"
                          Text="The product is currently out of stock, current quantity is {0}"
                        ></asp:TextBox>
                      </td>
                    </tr>
                  </table>
                </div>
              </ContentTemplate>
            </ajax:UpdatePanel>
          </div>
        </div>
        <div class="section" style="padding: 0 0 2px 0">
          <div class="header">
            <h2 class="volumediscounts">
              <asp:Localize
                ID="VolumeDiscountsCaption"
                runat="server"
                Text="Volume Discounts"
              ></asp:Localize>
            </h2>
          </div>
          <div class="content">
            <asp:Label
              ID="DiscountModeHelpText"
              runat="server"
              Text='When you assign a discount to a category, how should the products in the basket be counted?  In "Line Item" mode, each product in the basket is checked individually to see whether it meets the total quantity (or value) required to receive the discount.  In "Group by Category" mode, all products that belong to the discounted category are added together to determine eligibility for the discount.<br /><br />This setting also impacts Global discounts.  In Line Item mode, the behavior is as described above.  In Group by Category mode, all products in the basket are added together when calculating the discount.<br /><br />This setting does not impact discounts set to a specific product.'
            ></asp:Label>
            <table>
              <tr>
                <td>
                  <asp:Label
                    ID="DiscountModeLabel"
                    runat="Server"
                    SkinID="FieldHeader"
                    Text="Discount Mode: "
                  ></asp:Label>
                </td>
                <td>
                  <asp:RadioButtonList
                    ID="DiscountMode"
                    runat="server"
                    RepeatDirection="horizontal"
                  >
                    <asp:ListItem Value="0" Text="Line Item"></asp:ListItem>
                    <asp:ListItem
                      Value="1"
                      Text="Group By Category"
                    ></asp:ListItem>
                  </asp:RadioButtonList>
                </td>
              </tr>
            </table>
            <asp:Label
              ID="DiscountModeHelpText2"
              runat="server"
              Text="Note that regardless of discount mode set above, products that have options are always combined.  For example, if you purchase 1 red shirt and 1 blue shirt, this will count as 2 shirts when calculating discounts."
            ></asp:Label>
          </div>
        </div>
        <div class="section" style="padding: 0 0 2px 0">
          <div class="header">
            <h2 class="commonicon">
              <asp:Localize
                ID="SearchSettingsCaption"
                runat="server"
                Text="Search Settings"
              ></asp:Localize>
            </h2>
          </div>
          <div class="content">
            <table class="inputForm">
              <tr>
                <th class="rowHeader">
                  <cb:ToolTipLabel
                    ID="MinimumSearchLengthLabel"
                    runat="server"
                    Text="Minimum Search Length:"
                    ToolTip="The minimum search phrase length for searches on the retail side."
                  >
                  </cb:ToolTipLabel>
                  <br />
                </th>
                <td>
                  <asp:TextBox
                    ID="MinimumSearchLength"
                    runat="server"
                    Columns="3"
                    MaxLength="2"
                  ></asp:TextBox>
                  <asp:RangeValidator
                    ID="MinimumSearchLengthValidator"
                    runat="server"
                    Type="Integer"
                    MinimumValue="1"
                    MaximumValue="99"
                    ControlToValidate="MinimumSearchLength"
                    ErrorMessage="Minimum search length must be a numeric value between '1' and '99'."
                    Text="*"
                    EnableViewState="false"
                  ></asp:RangeValidator>
                </td>
              </tr>
            </table>
          </div>
        </div>
        <div class="section" style="padding: 0 0 2px 0">
          <div class="header">
            <h2 class="commonicon">
              <asp:Localize
                ID="Localize1"
                runat="server"
                Text="USPS Settings"
              ></asp:Localize>
            </h2>
          </div>
          <div class="content">
            <table class="inputForm">
              <tr>
                <th class="rowHeader">
                  <cb:ToolTipLabel
                    ID="ToolTipLabel1"
                    runat="server"
                    Text="WebTool UserId:"
                    ToolTip="The UserId given by USPS."
                  ></cb:ToolTipLabel>
                  <br />
                </th>
                <td>
                  <asp:TextBox ID="USPSUserId" runat="server"></asp:TextBox>
                </td>
              </tr>
            </table>
          </div>
        </div>
      </td>
    </tr>
    <tr>
      <td align="center" colspan="2">
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
        <br />
        <asp:Button
          ID="SaveButon"
          runat="server"
          Text="Save"
          OnClick="SaveButton_Click"
          CssClass="button"
        />
        <asp:Button
          ID="SaveAndCloseButton"
          runat="server"
          Text="Save and Close"
          OnClick="SaveAndCloseButton_Click"
          CssClass="button"
        />
        <asp:Button
          ID="CancelButton"
          runat="server"
          Text="Cancel"
          OnClick="CancelButton_Click"
          CssClass="button"
        />
      </td>
    </tr>
  </table>
</asp:Content>
