<%@Page Language="C#" MasterPageFile="~/Admin/Admin.master" CodeFile="Default.aspx.cs" Inherits="Admin_Store_PageTracking_Default" Title="Configure Page Tracking"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
	<div class="pageHeader">
		<div class="caption">
			<h1><asp:Localize ID="Caption" runat="server" Text="Configure Page Tracking"></asp:Localize></h1>
		</div>
	</div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td align="left" valign="top">
                <asp:Label ID="InstructionText" runat="server" Text="When you enable tracking of page views, you can see statistics about what categories and products are popular.  It will also enable customers to see their recently viewed items."></asp:Label><br /><br />
                <asp:Label ID="ResponseMessage" SkinID="GoodCondition" runat="server" Text="Your changes to the to the activity logging settings have been saved." Visible="false"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <table class="inputForm">
                    <tr>
                        <th class="rowHeader" style="vertical-align:top;">
                            <asp:Label ID="TrackPageViewsLabel" runat="Server" Text="Enable Tracking: "></asp:Label>
                        </th>
                        <td width="400">
                            <asp:CheckBox ID="TrackPageViews" runat="server" />
                        </td>
                        <td rowspan="4" style="vertical-align:top;padding-left:10px;">
                            <asp:Panel ID="CurrentLogPanel" runat="server" Width="400" CssClass="section">
                                <div class="header">
                                   <h2 class="currentlog"><asp:Localize ID="CurrentLogCaption" runat="server" Text="Current Log"></asp:Localize></h2>
                                </div>
                                <div class="content">
                                    <asp:Label ID="CurrentRecordsLabel" runat="Server" Text="Current Records: " SkinID="FieldHeader"></asp:Label>
                                    <asp:Label ID="CurrentRecords" runat="server"></asp:Label><br /><br />
                                    <asp:Button ID="ViewButton" runat="server" Text="View" OnClick="ViewButton_Click" />
                                    <asp:Button ID="ClearButton" runat="server" Text="Clear" OnClientClick="return confirm('WARNING: This will reset all page tracking statistics. Are you sure you want to clear all records from the log?')" OnClick="ClearButton_Click" />
                                </div>
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader" style="vertical-align:top;">
                            <asp:Label ID="HistoryLengthLabel" runat="Server" Text="History Length: "></asp:Label>
                        </th>
                        <td style="vertical-align:top;" width="300">
                            <asp:Label ID="HistoryLengthPrefix" runat="Server" Text="Maintain history for "></asp:Label>
                            <asp:TextBox ID="HistoryLength" runat="server" Columns="4"></asp:TextBox>
                            <asp:Label ID="HistoryLengthSuffix" runat="Server" Text=" days"></asp:Label><br />
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader" style="vertical-align:top;">
                            <asp:Label ID="SaveArchiveLabel" runat="Server" Text="Archive Option: "></asp:Label>
                        </th>
                        <td style="vertical-align:top;" width="300">
                            <asp:DropDownList ID="SaveArchive" runat="server" AutoPostBack="true" OnSelectedIndexChanged="SaveArchive_SelectedIndexChanged">
                                <asp:ListItem Text="Delete" Value="false"></asp:ListItem>
                                <asp:ListItem Text="Save to File" Value="true"></asp:ListItem>
                            </asp:DropDownList><br />

                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="left">
                            <asp:Panel ID="SaveArchiveWarningPanel" runat="server" Width="100%">
                                <br /><asp:Label ID="SaveArchiveHelpText" runat="server" text="<strong>Note:</strong> When you select the 'Save to File' option, history is written to file before deletion from database.  The log is written to the App_Data/Logs folder in extended log file format.  You must have write permissions to this location, and you are responsible for maintaining and/or removing the log files."></asp:Label>
                            </asp:Panel>
                            <br />                            
                            <asp:Button ID="SaveButton" runat="server" Text="Save" OnClick="SaveButton_Click" />
                            <asp:Button ID="SaveAndCloseButton" runat="server" Text="Save and Close" OnClick="SaveAndCloseButton_Click" />
							<asp:Button ID="CancelButton" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>


	<div class="pageHeader">
		<div class="caption">
			<h1><asp:Localize ID="GoogleAnalyticsCaption" runat="server" Text="Configure Google Analytics"></asp:Localize></h1>
		</div>
	</div>
    <table cellpadding="2" cellspacing="0" class="innerLayout">
        <tr>
            <td align="left" valign="top">
                <asp:Label ID="InstructionTextGA" runat="server" Text="Enter your Google Urchin ID below that will be used for tracking." EnableViewState="false"></asp:Label><br /><br />
                <asp:Label ID="ResponseMessageGA" SkinID="GoodCondition" runat="server" Text="Your Google Analytics settings have been updated." Visible="false"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
                <table class="inputForm">
                    <tr>
                        <th class="rowHeader" style="vertical-align:top;">
                            <asp:Label ID="GoogleUrchinIdLabel" runat="Server" Text="Google Urchin ID: " EnableViewState="false"></asp:Label>
                        </th>
                        <td width="320">
                            <asp:TextBox ID="GooleUrchinId" runat="server" EnableViewState="true" ></asp:TextBox>
                        </td> 
                        <td rowspan="4" style="vertical-align:top;padding-left:10px;">
                            <asp:Panel ID="GoogleAnalyticsHelp" runat="server" Width="400" CssClass="section">
                                <div class="header">
                                   <h2 class="commonicon"><asp:Localize ID="GoogleAnalyticsHelpCaption" runat="server" Text="Configuring Google Analytics"></asp:Localize></h2>
                                </div>
                                <div class="content">
                                    <asp:Label ID="InstructionText1" runat="server" Text="To complete Google Analytics configuration the pages you want to track in your store should use a footer with Google Analytics Widget. <br/><br/>You can add Google Analytics Widget to the footer by adding <b>[[ConLib:GoogleAnalyticsWidget]]</b> to the footer scriptlet.<br/><br/>For E-Commerce tracking to work, <b>Checkout/Receipt.aspx</b> page must use the footer with Google Analytics Widget."></asp:Label>
                                </div>
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader" style="vertical-align:top;">
                            <asp:Label ID="GooglePageTrackLabel" runat="Server" Text="Enable Page Tracking: " EnableViewState="false"></asp:Label>
                        </th>
                        <td width="300">
                            <asp:CheckBox ID="EnablePageTracking" runat="server" EnableViewState="true" ></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <th class="rowHeader" style="vertical-align:top;">
                            <asp:Label ID="GoogleEcomTrackLabel" runat="Server" Text="Enable E-Commerce Tracking: " EnableViewState="false"></asp:Label>
                        </th>
                        <td width="300">
                            <asp:CheckBox ID="EnableEcommerceTracking" runat="server" EnableViewState="true" ></asp:CheckBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" align="left">
                            <br />                            
                            <asp:Button ID="SaveButtonGA" runat="server" Text="Save" OnClick="SaveButtonGA_Click" />
                            <asp:Button ID="SaveAndCloseButtonGA" runat="server" Text="Save and Close" OnClick="SaveAndCloseButtonGA_Click" />
							<asp:Button ID="CancelButtonGA" runat="server" Text="Cancel" OnClick="CancelButton_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</asp:Content>

