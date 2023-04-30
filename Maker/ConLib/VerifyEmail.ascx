<%@ Control Language="C#" AutoEventWireup="true" CodeFile="VerifyEmail.ascx.cs" Inherits="ConLib_VerifyEmail" EnableViewState="false" %>
<%--
<conlib>
<summary>Displays an email verification page.</summary>
<param name="SuccessMessage" default="Your email address has been verified!">Success message.</param>
<param name="FailureMessage" default="Your email address could not be verified.  Check that you have opened the link exactly as it appeared in the verification notice.">Failure message.</param>
</conlib>
--%>
<asp:Label ID="SuccessText" Text="Your email address has been verified!" runat="server" SkinID="GoodCondition"></asp:Label>
<asp:Label ID="FailureText" Text="Your email address could not be verified.  Check that you have opened the link exactly as it appeared in the verification notice." runat="server" SkinID="ErrorCondition"></asp:Label>