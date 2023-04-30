<%@ Page Language="C#" MasterPageFile="~/Layouts/Scriptlet.master"
AutoEventWireup="true" CodeFile="Subscription.aspx.cs" Inherits="Subscription"
Title="Subscription Confirmation" %> <%@ Register Assembly="MakerShop.Web"
Namespace="MakerShop.Web.UI.WebControls.WebParts" TagPrefix="cb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageContent" Runat="Server">
  <cb:ScriptletPart
    ID="SubscriptionPage"
    runat="server"
    Layout="Left Sidebar"
    Header="Standard Header"
    LeftSidebar="Standard Sidebar 1"
    Content="Subscription Page"
    Footer="Standard Footer"
    Title="Subscription Page"
    AllowClose="False"
    AllowMinimize="false"
  />
</asp:Content>
