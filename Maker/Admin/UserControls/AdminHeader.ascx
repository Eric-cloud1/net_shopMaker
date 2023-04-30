<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AdminHeader.ascx.cs" Inherits="Admin_UserControls_AdminHeader" EnableViewState="false"%>
<%@ Register Src="HeaderNavigation.ascx" TagName="HeaderNavigation" TagPrefix="uc1" %>
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>
<%-- 
The header is layed out in a table to avoid the use of float, as this could potentially
interfere with printable pages.
--%>

<table id="adminHeader" cellpadding="0" cellspacing="0" width="100%">
    <tr>
        <td class="logo">
            <asp:Image runat="server" ID="StoreLogo" SkinID="StoreLogo" AlternateText="MakerShop" />
        </td>
        <td class="navigation">
            <uc1:HeaderNavigation ID="HeaderNavigation1" runat="server" /><br />
            <div align="right">
            <table id="SearchPanel" runat="server" enableviewstate="false" cellpadding="2" cellspacing="0" border="0" style="vertical-align:baseline;" >
                <tr align="right" >
                    <td><asp:Label ID="SearchInLabel" runat="Server" Text="Search In:" SkinID="FieldHeader" EnableViewState="False" style="color:White"></asp:Label></td>
                    <td><asp:DropDownList ID="SearchFilter" runat="server"></asp:DropDownList></td>                    
                    <td><asp:Label ID="SearchForLabel" runat="Server" Text="&nbsp;&nbsp;Search For:" SkinID="FieldHeader" EnableViewState="False" style="color:White"></asp:Label></td>                    
                    <td><asp:TextBox ID="SearchPhrase" runat="server" MaxLength="255" ></asp:TextBox></td>
                    <td><asp:Button ID="SearchButton" runat="server" CausesValidation="false" Text="Search" OnClick="SearchButton_Click" /></td>
                </tr>
            </table>
            </div>
        </td>
    </tr>
    <tr>
        <td colspan="2" id="menuBar">
            <ComponentArt:Menu ID="AdminMenu" runat="server" DataSourceID="MenuDataSource" OnItemDataBound="Menu_DataBound" ServerCalculateProperties="true">
                <CustomAttributeMappings>
                    <ComponentArt:CustomAttributeMapping From="LookId" To="LookId" /> 
                    <ComponentArt:CustomAttributeMapping From="Enabled" To="Enabled" /> 
                    <ComponentArt:CustomAttributeMapping From="Look-RightIconUrl" To="Look-RightIconUrl" /> 
                </CustomAttributeMappings>
            </ComponentArt:Menu> 
            <asp:SiteMapDataSource ID="MenuDataSource" SiteMapProvider="AdminMenuMap" ShowStartingNode="false" runat="server" />
        </td>
    </tr>
</table>

