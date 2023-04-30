<%@ Control Language="C#" AutoEventWireup="true" CodeFile="autoRefreshImage.ascx.cs"
    Inherits="Admin_UserControls_autoRefreshImage" %>
<asp:Timer runat="server" ID="UpdateTimer" Interval="5000" OnTick="UpdateTimer_Tick" />
<asp:UpdatePanel runat="server">
    <ContentTemplate>
        <asp:PlaceHolder ID="phImageLocation" runat="server"></asp:PlaceHolder>
    </ContentTemplate>
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="UpdateTimer" EventName="Tick" />
    </Triggers>
</asp:UpdatePanel>
