<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ProductDescription.ascx.cs" Inherits="ConLib_ProductDescription" %>
<%--
<conlib>
<summary>A control to display product descriptions.</summary>
<param name="DescriptionCaption" default="Description">Possible value can be any string.  Caption for description.</param>
<param name="MoreDetailCaption" default="More Details">Possible value can be any string.  Caption for more details.</param>
<param name="ShowCustomFields" default="false">Possible values are true or false. Indicates whether the CustomFields will be shown or not.</param>
</conlib>
--%>
<%@ Register Src="~/ConLib/ProductCustomFieldsDialog.ascx" TagName="CustomFields" TagPrefix="uc" %>
<ajax:UpdatePanel ID="DescriptionAjax" runat="server">
    <ContentTemplate>
        <div class="section">
            <div class="header">
                <h2>
                    <asp:Literal ID="phCaption" runat="server" Text="Description" EnableViewState="false"></asp:Literal>&nbsp;
                    <asp:LinkButton ID="More" runat="server" Text="more details" OnClick="More_Click" EnableViewState="false"></asp:LinkButton>
                    <asp:LinkButton ID="Less" runat="server" Text="description" OnClick="Less_Click" EnableViewState="false"></asp:LinkButton>
                </h2>
            </div>
            <div class="content">
                <asp:Literal ID="phDescription" runat="server" EnableViewState="false"></asp:Literal>
                <uc:CustomFields ID="CustomFields" runat="server" />
            </div>
        </div>
    </ContentTemplate>
</ajax:UpdatePanel>
