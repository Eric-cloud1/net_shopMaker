<%@ Page Title="" Language="C#" MasterPageFile="Affiliate.master" AutoEventWireup="true" CodeFile="AffiliatePayment.aspx.cs" Inherits="Admin_Marketing_Affiliates_AffiliatePayment" %>
<%@ Register Src="~/Admin/UserControls/AccountDataViewport.ascx" TagName="AccountDataViewport" TagPrefix="uc" %>
<%@ Register assembly="wwhoverpanel" Namespace="Westwind.Web.Controls" TagPrefix="wwh" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<style type="text/css">
.layer1 {
margin: 0;
padding: 0;
width: 100%;
}
 
.heading {
margin: 1px;
padding: 3px 3px;
cursor: pointer;
position: relative;
top:10px;
}
.content {
padding: 0px 3px;
background-color:White;

}

</style>
<script type="text/javascript" src="<%= Page.ResolveUrl("~")%>js/jquery.min.js"></script>
    <script type="text/javascript">
    
        jQuery(document).ready(function() {
            jQuery(".content").show();
            //toggle the componenet with class msg_body
            jQuery(".heading").click(function() {
                jQuery(this).next(".content").slideToggle(500);
            });
        });

        

        function ShowHoverPanel(event,Id, panelName) {
            
            if(panelName == "cvv")
                CVVCodesHoverLookupPanel.startCallback(event,"ADDCC",null,OnError);    
            else if(panelName == "avs")
                AVSCodesHoverLookupPanel.startCallback(event,"ADDCC",null,OnError);   
            else if(panelName == "h1")
                WwHoverPaymentId.startCallback(event, "paymentId=" + Id, null, OnError);
            else if (panelName == "h2")
                WwHoverPaymentId.startCallback(event, "transactionId=" + Id, null, OnError);
  
        }
        function HideHoverPanel(panelName)
        {
            if (panelName == "cvv")
                CVVCodesHoverLookupPanel.hide();
            else if (panelName == "avs")
                AVSCodesHoverLookupPanel.hide();
            else if (panelName == "h1")
                WwHoverPaymentId.hide();
            else if (panelName == "h2")
                WwHoverPaymentId.hide();
            
            
            // *** If you don't use shadows, you can fade out
            //LookupPanel.fadeout();
        }
        function OnCompletion(Result)
        {
            //alert('done it!\r\n' + Result);
        }
        function OnError(Result)
        {
            alert("*** Error:\r\n\r\n" + Result.message);
        }



        function changeIt(img_name) {

            eval("var theImg = document.images." + img_name + ".src;");

            var x = theImg.split("/");
            var t = x.length - 1;
            var y = x[t];
            

            if (y == 'plus.gif') {
                eval("document.images." + img_name +".src = '<%= Page.ResolveUrl("~")%>images/minus.gif';");
            }
            if (y == 'minus.gif') {
                 eval("document.images." + img_name +".src = '<%= Page.ResolveUrl("~")%>images/plus.gif';");
            }
        } 
</script>          
<div class="layer1">

<asp:Repeater ID="OrderRepeater"  runat="server" OnItemDataBound="OrderRepeater_ItemDataBound"  >
         <ItemTemplate>
            <fieldset style="border-color:silver">
        <div class="heading">
         <legend > <h1><img src="<%= Page.ResolveUrl("~")%>images/minus.gif" onclick="changeIt('<%#getImageNumber(Container.DataItem)%>')" name="<%#getImageNumber(Container.DataItem)%>" alt="Group/Collapse Payments" border="0" /> <%#getOrderNumber(Container.DataItem)%></h1></legend>
         </div>
         <div class="content">
        <asp:Repeater ID="PaymentRepeater" runat="server"   OnItemDataBound="PaymentRepeater_ItemDataBound" 
            OnItemCommand="PaymentRepeater_ItemCommand">
            <ItemTemplate>
                <div class="pageHeader">
                    <div class="caption" >
                        <h1  onmouseover='ShowHoverPanel(event, "<%# Eval("Paymentid") %>", "h1");' onmouseout='HideHoverPanel("h1");' >
                       
                            <asp:Localize ID="PaymentNumber"  runat="server"  Text='<%#string.Format("Payment #{0} ({1}): ", (Container.ItemIndex + 1),FormatPaymentType(Container.DataItem))%>'></asp:Localize>
                            <asp:Localize ID="PaymentReference" runat="server" Text='<%#string.Format("{0} {1}", Eval("PaymentMethodName"), Eval("ReferenceNumber"))%>'></asp:Localize>
                      
                        </h1>
                    </div>
                </div>
                <table class="inputForm" cellpadding="4" cellspacing="0">
                    <tr>
                        <th class="rowHeader">
                            <asp:Label ID="AmountLabel" runat="server" Text="Amount:"></asp:Label>
                        </th>
                        <td>
                            <asp:Label ID="Amount" runat="server" Text='<%#Eval("Amount", "{0:lc}") %>'></asp:Label> <asp:Label ID="Currency" runat="server" Text='<%# Eval("CurrencyCode") %>'></asp:Label>
                        </td>
                        <th class="rowHeader">
                            <asp:Label ID="PaymentDateLabel" runat="server" Text="Date:"></asp:Label>
                        </th>
                        <td>
                            <asp:Label ID="PaymentDate" runat="server" Text='<%# Eval("PaymentDate", "{0:g}") %>'></asp:Label>
                        </td>
                        <th class="rowHeader">
                            <asp:Label ID="PaymentMethodLabel" runat="server" Text="Method: " SkinID="FieldHeader"></asp:Label>
                        </th>
                        <td>
                            <asp:Label ID="PaymentMethod" runat="server" Text='<%# Eval("PaymentMethodName") %>'></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader" valign="top">
                            <asp:Label ID="PaymentStatusLabel" runat="server" Text="Status:"></asp:Label>
                        </th>
                        <td valign="top">
                            <asp:Label ID="CurrentPaymentStatus" runat="server" Text='<%#StringHelper.SpaceName(Eval("PaymentStatus").ToString()).ToUpperInvariant()%>' CssClass='<%#CssHelper.GetPaymentStatusCssClass(Container.DataItem)%>'></asp:Label>
                        </td>
                        <asp:Panel runat="server" ID="pTask">
                        <th class="rowHeader" valign="top">
                            <asp:Label ID="PaymentActionLabel" runat="server" Text="Tasks:" SkinID="FieldHeader"></asp:Label>
                        </th>
                        <td colspan="3" valign="top">
                            <asp:DropDownList ID="PaymentAction" runat="server">
                                <asp:ListItem Text=""></asp:ListItem>
                                <asp:ListItem Value="EDIT" Text="Edit Payment"></asp:ListItem>
                                <asp:ListItem Value="DELETE" Text="Delete Payment"></asp:ListItem>
                                <asp:ListItem Text="---" Value=""></asp:ListItem>
                            </asp:DropDownList>
                            <asp:ImageButton ID="PaymentActionButton" runat="server" SkinID="GoIcon" CommandName="Do_Task" CommandArgument='<%#Eval("PaymentId")%>' OnClientClick="javascript:return confirmDel()" />
                            <asp:Panel ID="RetryPanel" runat="server" visible="false">
                                <asp:Label ID="RetryHelpText" runat="server" Text="Provide the card security code if available:"></asp:Label><br />
                                <asp:Label ID="RetrySecurityCodeLabel" runat="server" Text="Security Code:" SkinID="fieldheader"></asp:Label>
                                <asp:TextBox ID="RetrySecurityCode" runat="server" Columns="4"></asp:TextBox><br /><br />
                                <asp:LinkButton ID="SubmitRetryButton" runat="server" SkinID="Button" CommandName="Do_SubmitRetry" CommandArgument='<%#Eval("PaymentId")%>' Text="Retry"></asp:LinkButton>
                                <asp:LinkButton ID="CancelRetryButton" runat="server" SkinID="Button" CommandName="Do_CancelRetry" CommandArgument='<%#Eval("PaymentId")%>' Text="Cancel"></asp:LinkButton>
                            </asp:Panel>
                        </td>
                        </asp:Panel>
                    </tr>
                    <tr>
                        <th class="rowHeader" valign="top">
                            <asp:Label ID="PaymentStatusReasonLabel" runat="server" Text="Status Note:" EnableViewState="false"></asp:Label>
                        </th>
                        <td colspan="5">
                            <asp:Label ID="PaymentStatusReason" runat="server" Text='<%# Eval("PaymentStatusReason") %>' EnableViewState="false"></asp:Label>
                        </td>                        
                    </tr>
                    <asp:Panel runat="server" ID="pAccountDetails">
                    <tr>
                        <th class="rowHeader" valign="top">
                            <asp:Label ID="AccountDetailsLabel" runat="server" Text="Account Details:" AssociatedControlID="AccountDataViewport" EnableViewState="false"></asp:Label>
                        </th>
                        <td colspan="5">
                            <uc:AccountDataViewport ID="AccountDataViewport" runat="server" PaymentId='<%# Eval("PaymentId") %>' UnavailableText="n/a"></uc:AccountDataViewport>
                        </td>
                    </tr>
                    </asp:Panel>
                    <tr class="sectionHeader">
                        <th colspan="6" style="text-align:left">
                            <asp:Label ID="TransactionHistoryCaption" runat="server" Text="Transaction History"></asp:Label>
                        </th>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <asp:GridView ID="TransactionGrid" runat="server" DataSource='<%#Eval("Transactions")%>' 
                                AutoGenerateColumns="false" Width="100%" SkinID="NestedList">
                                <Columns>
                                    <asp:TemplateField HeaderText="Date">
                                        <ItemStyle VerticalAlign="Top" Wrap="false" />
                                        <ItemTemplate>
                                            <div onmouseover='ShowHoverPanel(event, "<%# Eval("TransactionId") %>", "h2");' onmouseout='HideHoverPanel("h2");'>
                                                <%# Eval("TransactionDate", "{0:g}") %>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Gateway">
                                        <ItemStyle VerticalAlign="Top" />
                                        <ItemTemplate>
                                            <%#Eval("PaymentGateway.Name")%><br />
                                            <%# FormatChargeBackDateDate(Container.DataItem)%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Type">
                                        <ItemStyle VerticalAlign="Top" />
                                        <ItemTemplate>
                                            <%#StringHelper.SpaceName(Eval("TransactionType").ToString())%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Amount">
                                        <ItemStyle VerticalAlign="Top" />
                                        <ItemTemplate>
                                            <%#Eval("Amount", "{0:lc}")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Result">
                                        <ItemStyle VerticalAlign="Top" />
                                        <ItemTemplate>
                                            <asp:Label ID="SuccessLabel" runat="server" Text="SUCCESS" SkinID="GoodCondition" Visible='<%#(isSuccessfulTransaction(Eval("TransactionStatus")))%>' EnableViewState="false"></asp:Label>
                                            <asp:Label ID="FailedLabel" runat="server" Text="FAILED" SkinID="ErrorCondition" Visible='<%#(isFailedTransaction(Eval("TransactionStatus")))%>' EnableViewState="false"></asp:Label>
									        <asp:Label ID="PendingLabel" runat="server" Text="PENDING" SkinID="PendingCondition" Visible='<%#(isPendingTransaction(Eval("TransactionStatus")))%>' EnableViewState="false"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Notes">
                                        <ItemTemplate>
                                            <asp:PlaceHolder ID="ResponseMessagePanel" runat="server" Visible='<%#!String.IsNullOrEmpty((string)Eval("ResponseMessage"))%>' EnableViewState="false">
                                                <%#Eval("ResponseMessage")%>
                                                <asp:Literal ID="ResponseCode" runat="server" Text='<%#Eval("ResponseCode", "&nbsp;({0})")%>' Visible='<%#!String.IsNullOrEmpty((string)Eval("ResponseCode"))%>' EnableViewState="false"></asp:Literal>
                                                <br />
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder ID="ProviderTransactionIdPanel" runat="server" Visible='<%#!String.IsNullOrEmpty((string)Eval("ProviderTransactionId"))%>' EnableViewState="false">
                                                <asp:Label ID="ProviderTransactionIdLabel" runat="server" Text="Transaction ID:" SkinID="FieldHeader" EnableViewState="false"></asp:Label>
                                                <%#Eval("ProviderTransactionId")%><br />
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder ID="AuthorizationCodePanel" runat="server" Visible='<%#!String.IsNullOrEmpty((string)Eval("AuthorizationCode"))%>' EnableViewState="false">
                                                <asp:Label ID="AuthorizationCodeLabel" runat="server" Text="Authorization:" SkinID="FieldHeader" EnableViewState="false"></asp:Label>
                                                <%#Eval("AuthorizationCode")%><br />
                                            </asp:PlaceHolder>
                                            <asp:PlaceHolder ID="AvsCvsResultPanel" runat="server" Visible='<%# ShowTransactionElement("AVSCVV", Container.DataItem) %>' EnableViewState="false">
                                                <asp:Label ID="AvsCodeLabel" runat="server" Text="AVS:" SkinID="FieldHeader" EnableViewState="false"></asp:Label>
                                                <%# GetAvs(Container.DataItem) %>
                                                &nbsp;<a Name="AvsCodes" href="AVSCodes.htm" target="_blank" onmouseover='ShowHoverPanel(event, "", "avs");' onmouseout='HideHoverPanel("avs");'>Common AVS Codes</a>
                                                <br />
                                                <asp:Label ID="CvvCodeLabel" runat="server" Text="CVV:" SkinID="FieldHeader" EnableViewState="false"></asp:Label>
                                                <%# GetCvv(Container.DataItem) %>
                                                &nbsp;<a Name="CvvCodes" href="CVVCodes.htm" target="_blank" onmouseover='ShowHoverPanel(event, "", "cvv");' onmouseout='HideHoverPanel("cvv");'>Common CVV Codes</a>
                                                <br />
                                            </asp:PlaceHolder>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                              
                                <EmptyDataTemplate>
                                    <asp:Label ID="EmptyMessage" runat="server" Text="No transactions are associated with this payment."></asp:Label>
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:Repeater>
        </div>
          </fieldset>
   </ItemTemplate>
</asp:Repeater>
  </div>
     
   
    <wwh:wwHoverPanel ID="CVVCodesHoverLookupPanel"
        runat="server" 
        serverurl="~/Admin/Orders/Payments/CVVCodes.htm"
        Navigatedelay="1000"              
        scriptlocation="WebResource"
        style="display: none; background: white;" 
        panelopacity="0.89" 
        shadowoffset="8"
        shadowopacity="0.18"
        PostBackMode="None"
        AdjustWindowPosition="true">
    </wwh:wwHoverPanel>
    <wwh:wwHoverPanel ID="AVSCodesHoverLookupPanel"
        runat="server" 
        serverurl="~/Admin/Orders/Payments/AVSCodes.htm"
        Navigatedelay="1000"              
        scriptlocation="WebResource"
        style="display: none; background: white;" 
        panelopacity="0.89" 
        shadowoffset="8"
        shadowopacity="0.18"
        PostBackMode="None"
        AdjustWindowPosition="true">
    </wwh:wwHoverPanel>
     <wwh:wwHoverPanel ID="WwHoverPaymentId"
        runat="server" 
        serverurl="~/Admin/Orders/Payments/viewDetails.ashx"
        Navigatedelay="500"              
        scriptlocation="WebResource"
        style="display: none; background: white;" 
        panelopacity="0.89" 
        shadowoffset="8"
        shadowopacity="0.18"
        PostBackMode="None"
        HoverOffsetBottom ="3"
        HoverOffsetRight ="10"
        AdjustWindowPosition="true">
    </wwh:wwHoverPanel>
</asp:Content>