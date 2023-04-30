<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SubscriptionPage.ascx.cs" Inherits="ConLib_SubscriptionPage" EnableViewState="false" %>
<%--
<conlib>
<summary>Simple subscription pages where customer subscribe to emailing lists are confirmed</summary>
<param name="SubscribedMessage" default="Your subscription to the list {0} has been activated.">Successful subscription message.</param>
<param name="DeletedMessage" default="Your address has been removed from the list {0}.">Successfu subscription removed message.</param>
<param name="InvalidMessage" default="The subscription request was not understood or the provided parameters are incorrect.">Invalid access message.</param> 
</conlib>
--%>
<asp:PlaceHolder ID="phMessage" runat="server"></asp:PlaceHolder>