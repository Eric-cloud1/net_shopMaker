<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"
CodeFile="Search.aspx.cs" Inherits="Admin_Search" Title="Admin Search" %> <%@
Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls"
TagPrefix="cb" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
  <script language="javascript" type="text/javascript">
    function SearchRedirect() {
      if (!Page_ClientValidate("AdminSearch")) return false;

      var url =
        "Search.aspx?k=" +
        document.getElementById("<%=SearchPhrase.ClientID%>").value;
      url +=
        "&s=" + document.getElementById("<%=SearchFilter.ClientID%>").value;
      window.location = url;
      return false;
    }
  </script>
  <asp:Panel ID="SearchForm" runat="server" DefaultButton="SearchButton">
    <table
      id="SearchPanel"
      runat="server"
      enableviewstate="false"
      cellpadding="2"
      cellspacing="0"
      border="0"
      style="vertical-align: baseline"
    >
      <tr align="right">
        <td>
          <asp:Label
            ID="SearchLabel"
            runat="Server"
            Text="Search In:"
            SkinID="FieldHeader"
            EnableViewState="False"
          ></asp:Label>
        </td>
        <td>
          <asp:DropDownList ID="SearchFilter" runat="server"></asp:DropDownList>
        </td>
        <td>
          <asp:Label
            ID="SearchForLabel"
            runat="Server"
            Text="&nbsp;&nbsp;Search For:"
            SkinID="FieldHeader"
            EnableViewState="False"
          ></asp:Label>
        </td>
        <td>
          <asp:TextBox
            ID="SearchPhrase"
            runat="server"
            MaxLength="255"
            ValidationGroup="AdminSearch"
          ></asp:TextBox>
          <asp:RequiredFieldValidator
            ID="SearchPhraseRequired"
            runat="server"
            ControlToValidate="SearchPhrase"
            ValidationGroup="AdminSearch"
            Text="*"
            ErrorMessage="Search keyword(s) must be at least 3 characters in length."
          />
          <asp:RegularExpressionValidator
            ID="SearchPhraseValidator"
            runat="server"
            ControlToValidate="SearchPhrase"
            ValidationExpression=".{3,}"
            ValidationGroup="AdminSearch"
            Text="*"
            ErrorMessage="Search keyword(s) must be at least 3 characters in length."
          >
          </asp:RegularExpressionValidator>
        </td>
        <td>
          <asp:Button
            ID="SearchButton"
            runat="server"
            Text="Search"
            OnClientClick="SearchRedirect();return false;"
            ValidationGroup="AdminSearch"
          />
        </td>
      </tr>
      <tr>
        <td colspan="5">
          <asp:ValidationSummary
            ID="ValidationSummary1"
            runat="server"
            ValidationGroup="AdminSearch"
          /><br />
        </td>
      </tr>
    </table>
  </asp:Panel>
  <asp:Panel ID="ResultsPanel" runat="server">
    <div class="pageHeader">
      <div class="caption">
        <h1>
          <asp:Label
            ID="SearchCaption"
            runat="server"
            Text="You searched '{0}':"
            SkinID="FieldHeader"
            EnableViewState="false"
          ></asp:Label>
        </h1>
      </div>
    </div>
    <asp:Repeater
      ID="SearchAreasRepeater"
      runat="server"
      OnItemCommand="Repeater_OnItemCommand"
      OnItemDataBound="Repeater_OnItemDataBound"
    >
      <ItemTemplate>
        <h2>
          <asp:Label
            ID="Caption"
            runat="server"
            Text='<%#String.Format("{0} Matching {1}.",Eval("TotalMatches"), Eval("SearchArea"))%>'
          ></asp:Label>
        </h2>
        <ajax:UpdatePanel
          ID="ResultsAjax"
          runat="server"
          UpdateMode="Conditional"
        >
          <ContentTemplate>
            <asp:PlaceHolder ID="ResultsPH" runat="server"></asp:PlaceHolder>
            <%--
            <asp:Repeater
              ID="ResultsRepeater"
              runat="server"
              DataSource='<%#Eval("SearchResults")%>'
              OnItemCommand="ResultsRepeater_OnItemCommand"
            >
              <ItemTemplate>
                <asp:HyperLink
                  ID="ResultLink"
                  runat="server"
                  Text='<%#Eval("Name")%>'
                  NavigateUrl='<%#GetLinkUrl(DataBinder.Eval(((RepeaterItem)Container.Parent.Parent.Parent.Parent).DataItem,"SearchArea"),(int)Eval("Id"))%>'
                ></asp:HyperLink
                ><br />
              </ItemTemplate>
              <FooterTemplate>
                <asp:LinkButton
                  ID="AllResultsLink"
                  runat="server"
                  Text='<%#String.Format("See all {0} matches>>", DataBinder.Eval(((RepeaterItem)Container.Parent.Parent.Parent.Parent).DataItem,"TotalMatches"))%>'
                  Visible='<%#((int)DataBinder.Eval(((RepeaterItem)Container.Parent.Parent.Parent.Parent).DataItem,"TotalMatches") > 5) %>'
                  CommandName="ShowAll"
                  CommandArgument='<%#DataBinder.Eval(((RepeaterItem)Container.Parent.Parent.Parent.Parent).DataItem,"SearchArea") %>'
                ></asp:LinkButton>
              </FooterTemplate>
            </asp:Repeater>
            --%>
          </ContentTemplate>
        </ajax:UpdatePanel>
      </ItemTemplate>
    </asp:Repeater>
    <asp:Label
      ID="NoResultsLabel"
      runat="Server"
      Text="<br/>No results found.<br/>"
      Visible="false"
      EnableViewState="false"
    ></asp:Label>
  </asp:Panel>
</asp:Content>
