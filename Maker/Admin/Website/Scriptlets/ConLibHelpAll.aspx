<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ConLibHelpAll.aspx.cs" Inherits="Admin_Website_Scriptlets_ConLibHelpAll" EnableViewState="false" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>ConLib Reference</title>
</head>
<body>
    <form id="form1" runat="server">
        <br />
        <div class="pageHeader">
    	    <div class="caption">
    		    <h1><asp:Localize ID="Caption" runat="server" Text="ConLib Reference"></asp:Localize></h1>
		        <div class="links noPrint">
                    <asp:HyperLink ID="PrintLink" runat="server" Text="Print" NavigateUrl="javascript:window.print()"></asp:HyperLink>
                </div>
    	    </div>
        </div>
        <div style="padding:8px;background-color:White;">
            <asp:Repeater ID="ConLibList" runat="server" EnableViewState="false">
                <ItemTemplate>
                    <table cellpadding="2" cellspacing="0" class="innerLayout">
                        <tr>
                            <th style="text-align:left">
                                <h2 class="conlib"><%# Eval("Name") %></h2>
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <i>Summary:</i><%# Eval("Summary") %><br /><br />
                                <i>Usage:</i> <%# Eval("Usage") %><br />
                                <asp:Repeater ID="ParamList" runat="server" DataSource='<%#Eval("Params")%>' Visible='<%#((ICollection)Eval("Params")).Count > 0%>'>
                                    <HeaderTemplate><br /><i>Properties:</i><div style="padding-left:20px"><dl></HeaderTemplate>
                                    <ItemTemplate><dt><%#Eval("Name")%></dt><dd><%#Eval("Summary")%></dd></ItemTemplate>
                                    <FooterTemplate></dl></div></FooterTemplate>
                                </asp:Repeater>
                            </td>
                        </tr>
                    </table>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </form>
</body>
</html>