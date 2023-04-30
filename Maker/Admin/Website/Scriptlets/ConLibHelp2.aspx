<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="ConLibHelp2.aspx.cs" Inherits="Admin_Website_Scriptlets_ConLibHelp2" Title="ConLIb Reference" EnableViewState="false" %>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<div class="pageHeader">
    <div class="caption">
	    <h1><asp:Localize ID="Caption" runat="server" Text="ConLib Reference"></asp:Localize></h1>
    </div>
</div>
<div style="padding:8px;background-color:White;">
    <ajax:UpdatePanel ID="ConLibAjax" runat="server">
        <ContentTemplate>
            <asp:Label ID="SelectControlLabel" runat="server" AssociatedControlID="SelectControl" Text="Select Control: " SkinID="FieldHeader"></asp:Label>&nbsp;
            <asp:DropDownList ID="SelectControl" runat="server" DataTextField="Name" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="SelectControl_SelectedIndexChanged"></asp:DropDownList>
            &nbsp;&nbsp;<asp:HyperLink ID="ShowAllLink" runat="server" Text="Show All" NavigateUrl="ConLibHelpAll2.aspx"></asp:HyperLink>
            <asp:PlaceHolder ID="phControlDetails" runat="server" Visible="false" EnableViewState="false">
                <br /><br />
                <i><asp:Localize ID="SummaryLabel" runat="server" Text="Summary: "></asp:Localize></i>
                <asp:Literal ID="Summary" runat="server" Text=""></asp:Literal><br /><br />
                <i><asp:Localize ID="UsageLabel" runat="server" Text="Usage: "></asp:Localize></i>
                <asp:Literal ID="Usage" runat="server" Text=""></asp:Literal><br />
                <asp:Repeater ID="ParamList" runat="server" EnableViewState="false" Visible="false">
                    <HeaderTemplate><br /><i>Properties:</i><div style="padding-left:20px"><dl></HeaderTemplate>
                    <ItemTemplate><dt><%#Eval("Name")%></dt><dd><%#Eval("Summary")%></dd></ItemTemplate>
                    <FooterTemplate></dl></div></FooterTemplate>
                </asp:Repeater>
            </asp:PlaceHolder>
        </ContentTemplate>
    </ajax:UpdatePanel>
</div>
</asp:Content>