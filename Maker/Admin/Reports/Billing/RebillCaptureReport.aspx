<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master"
AutoEventWireup="true" CodeFile="RebillCaptureReport.aspx.cs"
Inherits="Admin_Reports_RebillCaptureReport" Title="Rebill Capture" %> <%@
Register Src="~/Admin/UserControls/PickerAndCalendar.ascx"
TagName="PickerAndCalendar" TagPrefix="uc1" %> <%@ Register
Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb"
%> <%@ Register src="~/Admin/UserControls/DatesAndTime.ascx"
tagname="DatesAndTime" tagprefix="uc1" %> <%@ Register
src="../../UserControls/downloadbutton.ascx" tagname="downloadbutton"
tagprefix="uc2" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
  <script src="../../../js/jquery.js" type="text/javascript"></script>
  <script
    src="../../../js/jquery.fixedheadertable.1.0.b.js"
    type="text/javascript"
  ></script>
  <script type="text/javascript">
    function logMenuFocusOn(menuInstance) {
      $("#" + menuInstance).attr("mouseonmenu", "1");
    }
    function logMenuFocusOut(menuInstance) {
      $("#" + menuInstance).attr("mouseonmenu", "0");
    }
    function closeMenu(menuInstance) {
      if ($("#" + menuInstance).attr("mouseonmenu") == 0) {
        $("#" + menuInstance).hide();
      }
    }
    jQuery.fn.setMouseEvents = function () {
      return this.each(function () {
        var me = jQuery(this);
        me.click(function () {
          $("div").filter("[type=ContextMenu]").hide();
          InstantiateMenu(me.attr("id"));
        });
        me.dblclick(function () {
          closeMenu("Menu" + me.attr("id"));
        });
        me.blur(function () {
          closeMenu("Menu" + me.attr("id"));
        });
      });
    };

    function InstantiateMenu(ParentID) {
      if ($("#Menu" + ParentID).html() == null) {
        var p = $("#" + ParentID);
        var offset = p.offset();
        var left = offset.left + $("#" + ParentID).width() - 10;
        var top = offset.top + $("#" + ParentID).height() - 5;

        var strMenuHTML =
          '<div type="ContextMenu" id="Menu' +
          ParentID +
          '" style="position:absolute;left:' +
          left +
          "px;top:" +
          top +
          '\px; mouseonmenu="1">loading...</div>';
        $("#grid").append(strMenuHTML);
        ShowHoverPanel(ParentID);
      }
      var offset = $("#" + ParentID).offset();
      $("#Menu" + ParentID)
        .css({ top: offset.top + $("#" + ParentID).height() - 5 + "px" })
        .show();
      $("#Menu" + ParentID).mouseout(function () {
        logMenuFocusOut("Menu" + ParentID);
        $("#" + ParentID)[0].focus();
      });
      $("#Menu" + ParentID).mouseover(function () {
        logMenuFocusOn("Menu" + ParentID);
      });
    }

    function ShowHoverPanel(Id1) {
      $.post("ViewIdsDetails.ashx?ids=" + Id1, {}, function (response) {
        $("#Menu" + Id1).html(response);
      });
    }

    $(document).ready(function () {
      $("td").filter("[parent=GridTable]").setMouseEvents();
    });
  </script>
  <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
      <div class="pageHeader">
        <div class="caption">
          <h1>
            <asp:Localize
              ID="Caption"
              runat="server"
              Text="Rebill Capture Report"
            ></asp:Localize>
            <asp:Localize
              ID="ReportCaption"
              runat="server"
              Visible="False"
              EnableViewState="False"
            ></asp:Localize>
          </h1>
        </div>
      </div>

      <div id="grid">
        <table
          align="center"
          class="form"
          cellpadding="0"
          cellspacing="0"
          border="0"
          width="100%"
        >
          <tr>
            <td style="width: 100%" colspan="4">
              <asp:Label
                ID="SavedMessage"
                runat="server"
                Text="Template saved at {0}."
                EnableViewState="false"
                Visible="false"
                SkinID="GoodCondition"
              ></asp:Label>
              <asp:Label
                ID="ErrorMessageLabel"
                runat="server"
                Text=""
                EnableViewState="false"
                Visible="false"
                SkinID="ErrorCondition"
              ></asp:Label>
            </td>
          </tr>
          <uc1:DatesAndTime ID="dtRebillCapture" runat="server" />
          <tr>
            <td style="width: 10%">
              <div style="font-weight: bold">
                <cb:ToolTipLabel
                  ID="MethodLabel"
                  runat="server"
                  Text="Pivot: "
                  ToolTip="(Gateway) Displays counts/amounts for each Payment Gateway used.(Instrument) Displays counts/amounts for each Payment Gateway and instrument used."
                ></cb:ToolTipLabel>
              </div>
            </td>
            <td colspan="3">
              <div style="text-align: Left; vertical-align: middle">
                <asp:RadioButtonList
                  ID="rblGatewayInstrument"
                  runat="server"
                  RepeatDirection="Horizontal"
                >
                  <asp:ListItem Selected="True" Value="0">Gateway</asp:ListItem>
                  <asp:ListItem Value="1">Instrument</asp:ListItem>
                </asp:RadioButtonList>
              </div>
            </td>
          </tr>

          <tr>
            <td style="width: 10%">
              <asp:Button
                ID="ProcessButton0"
                runat="server"
                OnClick="ProcessButton_Click"
                Text="GO.."
                Style="vertical-align: top; height: 25px"
              />
            </td>
            <td colspan="2">
              <uc2:downloadbutton
                ID="downloadbutton1"
                FileName="ForecastAuthorize"
                runat="server"
              />
            </td>
            <td>&nbsp;</td>
          </tr>

          <tr>
            <td colspan="5">
              <div style="overflow-x: scroll; width: 950px">
                <asp:GridView
                  ID="RebillReportGrid"
                  runat="server"
                  AutoGenerateColumns="true"
                  OnPageIndexChanging="RebillReportGrid_PageIndexChanging"
                  onrowdatabound="RebillReportGrid_RowDataBound"
                  OnPreRender="RebillReportGrid_PreRender"
                  OnSorting="RebillReportGrid_Sorting"
                  DefaultSortExpression="Name"
                  DefaultSortDirection="Ascending"
                  AllowPaging="false"
                  AllowSorting="true"
                  SkinID="Summary"
                  PageSize="80"
                  Width="100%"
                >
                  <Columns> </Columns>
                  <EmptyDataTemplate>
                    <asp:Label
                      ID="EmptyResultsMessage"
                      runat="server"
                      Text="There are no results for the selected time period."
                    ></asp:Label>
                  </EmptyDataTemplate>
                </asp:GridView>
              </div>
            </td>
          </tr>
        </table>
      </div>
    </ContentTemplate>
  </ajax:UpdatePanel>
</asp:Content>
