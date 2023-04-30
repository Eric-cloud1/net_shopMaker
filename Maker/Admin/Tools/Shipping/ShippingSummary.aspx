<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="ShippingSummary.aspx.cs"
    Inherits="Admin_Reports_ShippingSummary" Title="Shipping Summary" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="Server">

    <script type="text/javascript" language="javascript">

        function uploadError(sender, args) {
            document.getElementById('<%= lblStatus.ClientID %>').innerHTML = args.get_fileName() + "<span style='font-size:12px;font-weight: bold; padding-bottom: 3px;color:red;'>" + args.get_errorMessage() + "</span>";
        }

        function StartUpload(sender, args) {
            document.getElementById('<%= lblStatus.ClientID %>').innerHTML = "<span style='font-size:12px;font-weight: bold; padding-bottom: 3px;color:#00BFFF;'>Uploading Started.</span>";
        }

        function UploadComplete(sender, args) {
            var filename = args.get_fileName();
            var contentType = args.get_contentType();
            var counts = args.get_errorMessage();
            var text = "File " + filename + " imported";



            document.getElementById('<%= lblStatus.ClientID %>').innerHTML = "<span style='font-size:12px;font-weight: bold; padding-bottom: 3px;color:#008000;'>" + text + "</span>";
        }
    </script>

    <div class="pageHeader">
        <div class="caption">
            <h1>
                <asp:Localize ID="Caption" runat="server" Text="Shipping Summary"></asp:Localize>
            </h1>
        </div>
    </div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td>
                <asp:Label ID="SavedMessage" runat="server" Text="Template saved at {0}." EnableViewState="false"
                    Visible="false" SkinID="GoodCondition"></asp:Label>
                <asp:Label ID="ErrorMessageLabel" runat="server" Text="" EnableViewState="false"
                    Visible="false" SkinID="ErrorCondition"></asp:Label>
            </td>
        </tr>
        <tr>
    <td>
              <cb:ToolTipLabel
                    ID="DateFilterLabel" runat="server" Text="Email:" ToolTip="Check to have the report emailed to you. Keep unchecked to download it." />  <asp:CheckBox ID="cbEmail" runat="server" Text="" Checked="true" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Button ID="ExportUS" runat="server" Text="CSV Shipment Report US" OnClick="Export_Click" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Button ID="ExportInternational" runat="server" Text="CSV Shipment Report International"
                    OnClick="Export_Click" />
            </td>
        </tr>
        <tr>
            <td>
                <hr />
            </td>
        </tr>
        <tr>
            <td>
                <ajax:UpdatePanel runat="server" ID="up" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div>
                             <cc1:AsyncFileUpload 
                             ID="AsyncFileUpload1" 
                             
                             Width="400px" 
                             
                              
                            CompleteBackColor="Lime"
                            
                            UploadingBackColor="#66CCFF" 
                            
                            ErrorBackColor="Red"
                            
                            runat="server" 
                            ThrobberID="Throbber" 
                            OnClientUploadError="uploadError"
                            OnClientUploadStarted="StartUpload" 
                            OnClientUploadComplete="UploadComplete" 
                            OnUploadedComplete="AsyncFileUpload1_UploadedComplete"
                            />

                            
                                
                
                            <asp:Label ID="Throbber" runat="server" Style="display: none">
            <img src="../../../images/indicator.gif" align="absmiddle" alt="loading" />
                            </asp:Label>
                            <br />
                            <asp:Label ID="lblStatus" runat="server" Text="Import Tracking XLS File" Style="font-size: 12px;
                                font-weight: bold; padding-bottom: 3px;"></asp:Label><br />
                            <asp:Label ID="lblCount" Style="font-size: 12px; font-weight: bold; padding-bottom: 3px;
                                color: #008000;" Text=" " runat="server" />
                        </div>
                    </ContentTemplate>
                </ajax:UpdatePanel>
            </td>
        </tr>
    </table>
</asp:Content>
