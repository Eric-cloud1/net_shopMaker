<%@ Page Language="C#" MasterPageFile="../Admin.master" CodeFile="Reschedule.aspx.cs"
    Inherits="Admin_Payment_Reschedule" Title="Payment Reschedule" %>

<%@ Register Src="~/Admin/UserControls/PickerAndCalendar.ascx" TagName="PickerAndCalendar"
    TagPrefix="uc1" %>
<asp:Content ContentPlaceHolderID="Head" runat="server">
    <style type="text/css">
        .PopupBody
        {
            background-color: #C1DBFE;
            filter: alpha(opacity=90);
            opacity: 0.9;
            border: solid 2px black;
            padding: }}</style>

    <script type="text/javascript">
        function select_deselectAll(chkVal, idVal) {
            var frm = document.forms[0];
            // Loop through all elements
            for (i = 0; i < frm.length; i++) {
                // Look for our Header Template's Checkbox
                if (idVal.indexOf('CheckAll') != -1) {
                    // Check if main checkbox is checked, then select or deselect datagrid checkboxes
                    if (chkVal == true) {
                        frm.elements[i].checked = true;
                    }
                    else {
                        frm.elements[i].checked = false;
                    }
                    // Work here with the Item Template's multiple checkboxes
                }
                else if (idVal.indexOf('selectThis') != -1) {
                    // Check if any of the checkboxes are not checked, and then uncheck top select all checkbox
                    if (frm.elements[i].checked == false) {
                        frm.elements[1].checked = false; //Uncheck main select all checkbox
                    }
                }
            }

        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <ajax:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <ajax:ModalPopupExtender ID="ModalPopupExtender1" runat="server" CancelControlID="btnCancel"
                TargetControlID="ProcessButton" PopupControlID="PanelPopUp" PopupDragHandleControlID="PopupHeader"
                Drag="true" BackgroundCssClass="ModalPopupBG">
            </ajax:ModalPopupExtender>
            <asp:Panel ID="PanelPopUp" Style="display: none; width: 150px; height: 75px;" runat="server">
                <div class="PopupBody">
                    <center>
                        <br />
                        <br />
                        <b>Do you want to clear queue first?</b>
                        <br />
                        <br />
                        <asp:Button ID="btnYes" OnClick="btnYes_Click" OnClientClick="javascript:alert('Are you sure?');"
                            Text="Yes" runat="server" />
                        <asp:Button ID="btnNo" OnClick="btnNo_Click" Text="No" runat="server" />
                        <asp:Button ID="btnCancel" Text="Cancel" runat="server" />
                        <br />
                        <br />
                    </center>
                </div>
            </asp:Panel>
            <div class="pageHeader">
                <div class="caption">
                    <h1>
                        <asp:Localize ID="TitleCaption" runat="server"></asp:Localize></h1>
                </div>
            </div>
            <asp:PlaceHolder ID="ErrorPanel" runat="server"></asp:PlaceHolder>
            <table cellpadding="2" cellspacing="0">
                <tr>
                    <td>
                        <asp:Label ID="queueLabel" Text="Queue:" runat="server" Style="display: inline; font-weight: bold;
                            width: 110px;" />
                        &nbsp;&nbsp;<asp:DropDownList ID="queueDropdown" runat="server" AutoPostBack="true"
                            OnSelectedIndexChanged="queueDropdown_SelectedIndexChanged" />
                        &nbsp;&nbsp;
                        <asp:Button ID="queueReset" runat="server" Text="Reset Queue" Style="vertical-align: top;
                            height: 20px" OnClick="queueReset_Click" />
                        <br />
                        <asp:Label ID="lbStartDate" Text="Reschedule Date:" runat="server" Style="display: inline;
                            font-weight: bold; width: 110px;" />
                        &nbsp;&nbsp;<uc1:PickerAndCalendar ID="RescheduleDate" runat="server" />
                        &nbsp;&nbsp;
                        <asp:Label ID="Label1" Text="To Queue:" runat="server" Style="display: inline; font-weight: bold;
                            width: 110px;" />
                        &nbsp;&nbsp;<asp:DropDownList ID="ddlToQueue" runat="server" AutoPostBack="false" />
                        &nbsp;&nbsp;
                        <asp:CheckBox runat="server" ID="cbCapture" Checked="false" Text="Capture Move" AutoPostBack="true"
                            OnCheckedChanged="cbCapture_CheckedChanged" />
                        &nbsp;&nbsp;
                        <asp:CheckBox runat="server" ID="cbDetails" Text="Show Details" Checked="false" AutoPostBack="true"
                            OnCheckedChanged="cbDetails_CheckedChanged" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:HiddenField ID="startDate" runat="server" />
                        <asp:HiddenField ID="endDate" runat="server" />
                    </td>
                </tr>
                <asp:Panel runat="server" ID="pBulk">
                    <tr>
                        <td>
                            <asp:Label ID="Label2" runat="server">Bulk Move:</asp:Label>
                            <asp:TextBox runat="server" ID="tbCount" MaxLength="5" />
                        </td>
                    </tr>
                </asp:Panel>
                <tr>
                    <td>
                        <asp:Button ID="bRefresh" runat="server" Text="GO&hellip;" Style="vertical-align: top;
                            height: 20px" OnClick="bRefresh_Click" />
                        &nbsp;
                        <asp:Button ID="ProcessButton" runat="server" Text="MOVE&hellip;" Style="vertical-align: top;
                            height: 20px" />
                    </td>
                </tr>
            </table>
            <ajax:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <table>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td style="font-weight: bold;">
                                            Rows:
                                        </td>
                                        <td>
                                            <asp:Label ID="lRows" runat="server" />
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td style="font-weight: bold;">
                                            Unique Orders:
                                        </td>
                                        <td>
                                            <asp:Label ID="lOrders" runat="server" />
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                        </td>
                                        <td style="font-weight: bold;">
                                            Available #
                                        </td>
                                        <td style="font-weight: bold;">
                                            Available $
                                        </td>
                                        <td style="font-weight: bold;">
                                            Attempted $
                                        </td>
                                        <td style="font-weight: bold;">
                                            Successful $
                                        </td>
                                        <td style="font-weight: bold;">
                                            Attempted #
                                        </td>
                                        <td style="font-weight: bold;">
                                            Successful #
                                        </td>
                                    </tr>
                                    <tr align="right">
                                        <td align="left">
                                            Authorize
                                        </td>
                                        <td>
                                            <asp:Label ID="lAvailableAuth" runat="server" />
                                        </td>
                                        <td>
                                            <asp:Label ID="lAvailableAmountAuth" runat="server" />
                                        </td>
                                        <td>
                                            <asp:Label ID="lAttemptAmountAuth" runat="server" />
                                        </td>
                                        <td>
                                            <asp:Label ID="lSuccessfulAmountAuth" runat="server" />
                                        </td>
                                        <td>
                                            <asp:Label ID="lAttemptedCountAuth" runat="server" />
                                        </td>
                                        <td>
                                            <asp:Label ID="lSuccessfulCountAuth" runat="server" />
                                        </td>
                                    </tr>
                                    <tr align="right">
                                        <td align="left">
                                            Capture
                                        </td>
                                        <td>
                                            <asp:Label ID="lAvailableCap" runat="server" />
                                        </td>
                                        <td>
                                            <asp:Label ID="lAvailableAmountCap" runat="server" />
                                        </td>
                                        <td>
                                            <asp:Label ID="lAttemptAmountCap" runat="server" />
                                        </td>
                                        <td>
                                            <asp:Label ID="lSuccessfulAmountCap" runat="server" />
                                        </td>
                                        <td>
                                            <asp:Label ID="lAttemptedCountCap" runat="server" />
                                        </td>
                                        <td>
                                            <asp:Label ID="lSuccessfulCountCap" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <asp:Panel runat="server" ID="pGrid">
                            <tr>
                                <td width="50%" valign="top" class="itemList">
                                    <div style="overflow: scroll; width: 980px;">
                                        <asp:GridView ID="gridReschedule" runat="server" AutoGenerateColumns="False" DefaultSortExpression="OrderNumber"
                                            DefaultSortDirection="Ascending" AllowPaging="False" AllowSorting="true" PageSize="100"
                                            SkinID="Summary" OnSorting="gridReschedule_Sorting" OnPageIndexChanging="gridReschedule_PageIndexChanging">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <HeaderTemplate>
                                                        <asp:CheckBox ID="CheckAll" OnClick="javascript: return select_deselectAll (this.checked, this.id);"
                                                            runat="server" />
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="selectThis" OnClick="javascript: return select_deselectAll (this.checked, this.id);"
                                                            runat="server" />
                                                        <asp:Label ID="paymentId" Font-Size="0" Style="visibility: hidden;" runat="server"
                                                            Text='<%# Eval("PaymentId","{0}")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Processed" SortExpression="Processed">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbProcessed" runat="server" Text='<%# Eval("Processed", "{0}") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="PaymentId" SortExpression="PaymentId">
                                                    <ItemStyle HorizontalAlign="right" Width="60" />
                                                    <ItemStyle HorizontalAlign="right" Width="60" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbPaymentId" runat="server" Text='<%# Eval("PaymentId", "{0}") %>'
                                                            Width="75px"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="OrderNumber" SortExpression="OrderNumber">
                                                    <ItemStyle HorizontalAlign="right" Width="60" />
                                                    <ItemTemplate>
                                                        <asp:HyperLink ID="lkOrderNumber" runat="server" Text='<%# Eval("OrderNumber", "{0}") %>'
                                                            NavigateUrl='<%# "~/Admin/Orders/ViewOrder.aspx?OrderNumber=" + DataBinder.Eval(Container.DataItem,"OrderNumber").ToString() + "&OrderId="+ DataBinder.Eval(Container.DataItem,"OrderId").ToString() %>'
                                                            Target="_blank" Width="75px"></asp:HyperLink>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Order Date" SortExpression="OrderDate">
                                                    <ItemStyle HorizontalAlign="left" Width="60" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbOrderDate" runat="server" Text='<%# formatOrderDate(Container.DataItem) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Last Transaction Date" SortExpression="LastTransactionDate">
                                                    <ItemStyle HorizontalAlign="left" Width="40" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbLastTransactionDate" runat="server" Text='<%# formatLastTransactionDate(Container.DataItem) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Last Success Transaction Date" SortExpression="LastSuccessTransactionDate">
                                                    <ItemStyle HorizontalAlign="left" Width="40" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbLastSuccessTransactionDate" runat="server" Text='<%# formatLastSuccessTransactionDate(Container.DataItem) %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Attempts" SortExpression="Attempts">
                                                    <ItemStyle HorizontalAlign="right" Width="40" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbAttempts" runat="server" Text='<%# Eval("Attempts", "{0:#,##0;}") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Bill To First Name" SortExpression="BillToFirstName">
                                                    <ItemStyle HorizontalAlign="left" Width="75" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbBillToFirstName" runat="server" Text='<%# Eval("BillToFirstName") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Bill To Last Name" SortExpression="BillToLastName">
                                                    <ItemStyle HorizontalAlign="left" Width="75" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbBillToLastName" runat="server" Text='<%# Eval("BillToLastName") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Bill To Address" SortExpression="BillToAddress1">
                                                    <ItemStyle HorizontalAlign="left" Width="75px" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbBillToAddress1" runat="server" Text='<%# Eval("BillToAddress1", "{0:#,##0;}") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Bill To City" SortExpression="BillToCity">
                                                    <ItemStyle HorizontalAlign="left" Width="75" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbBillToCity" runat="server" Text='<%# Eval("BillToCity", "{0:#,##0;}") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Bill To Province" SortExpression="BillToProvince">
                                                    <ItemStyle HorizontalAlign="left" Width="75" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbBillToProvince" runat="server" Text='<%# Eval("BillToProvince", "{0:#,##0;}") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Bill To Postal Code" SortExpression="BillToPostalCode">
                                                    <ItemStyle HorizontalAlign="left" Width="50" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbBillToPostalCode" runat="server" Text='<%# Eval("BillToPostalCode", "{0:#,##0;}") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Bill To Country Code" SortExpression="BillToCountryCode">
                                                    <ItemStyle HorizontalAlign="left" Width="25" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbBillToCountryCode" runat="server" Text='<%# Eval("BillToCountryCode", "{0:#,##0;}") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Bill To Phone" SortExpression="BillToPhone">
                                                    <ItemStyle HorizontalAlign="left" Width="50" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbBillToPhone" runat="server" Text='<%# Eval("BillToPhone", "{0:#,##0;}") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Transaction Amount" SortExpression="TransactionAmount">
                                                    <ItemStyle HorizontalAlign="right" Width="25" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbTransactionAmount" runat="server" Text='<%# Eval("TransactionAmount", "{0:#,##0;}") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="CC" SortExpression="CC">
                                                    <ItemStyle HorizontalAlign="right" Width="25" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbCC" runat="server" Text='<%# Eval("CC") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Affiliate ID" SortExpression="AffiliateId">
                                                    <ItemStyle HorizontalAlign="right" Width="150" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbAffiliateId" runat="server" Text='<%# Eval("AffiliateId") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Sub Affiliate" SortExpression="SubAffiliate">
                                                    <ItemStyle HorizontalAlign="left" Width="50" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbSubAffiliate" runat="server" Text='<%# Eval("SubAffiliate") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Response Message" SortExpression="ResponseMessage">
                                                    <ItemStyle HorizontalAlign="left" Width="75" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbResponseMessage" runat="server" Text='<%# Eval("ResponseMessage") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Remote IP" SortExpression="RemoteIP">
                                                    <ItemStyle HorizontalAlign="right" Width="50" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbRemoteIP" runat="server" Text='<%# Eval("RemoteIP") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Processor" SortExpression="Processor">
                                                    <ItemStyle HorizontalAlign="left" Width="100" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbProcessor" runat="server" Text='<%# Eval("Processor") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Card Type" SortExpression="CardType">
                                                    <ItemStyle HorizontalAlign="left" Width="50" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbCardType" runat="server" Text='<%# Eval("CardType") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="SKU" SortExpression="SKU">
                                                    <ItemStyle HorizontalAlign="left" Width="50" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbSKU" runat="server" Text='<%# Eval("SKU") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <EmptyDataTemplate>
                                                <div>
                                                    <asp:Label ID="EmptyResultsMessage" runat="server" Text="There are no results for the selected time period."></asp:Label>
                                                </div>
                                            </EmptyDataTemplate>
                                        </asp:GridView>
                                    </div>
                                </td>
                            </tr>
                        </asp:Panel>
                    </table>
                </ContentTemplate>
            </ajax:UpdatePanel>
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>
