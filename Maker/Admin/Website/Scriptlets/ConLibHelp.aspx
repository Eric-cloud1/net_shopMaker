<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ConLibHelp.aspx.cs" Inherits="Admin_Website_Scriptlets_ConLibHelp" EnableViewState="false" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>ConLib Reference</title>
    <script language="javascript">
    function initAjaxProgress()
    {
        var pageHeight = (document.documentElement && document.documentElement.scrollHeight) ? document.documentElement.scrollHeight : (document.body.scrollHeight > document.body.offsetHeight) ? document.body.scrollHeight : document.body.offsetHeight;
        //SET HEIGHT OF BACKGROUND
        var bg = document.getElementById('ajaxProgressBg');
        bg.style.height = pageHeight + 'px';
        //POSITION THE PROGRESS INDICATOR ON INITIAL LOAD
        reposAjaxProgress();
        //REPOSITION THE PROGRESS INDICATOR ON SCROLL
        window.onscroll = reposAjaxProgress;
    }

    function reposAjaxProgress()
    {
        var div = document.getElementById('ajaxProgress');
        var st = document.body.scrollTop;
        if (st == 0) {
            if (window.pageYOffset) st = window.pageYOffset;
            else st = (document.body.parentElement) ? document.body.parentElement.scrollTop : 0;
        }
        div.style.top = 150 + st + "px";
    }
    </script>
</head>
<body onLoad="initAjaxProgress();">
    <form id="form1" runat="server">
        <ajax:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" />
        <ajax:UpdateProgress ID="UpdateProgress1" runat="server" DisplayAfter="1000">
            <ProgressTemplate>
                <div id="ajaxProgressBg"></div>
                <div id="ajaxProgress"></div>
            </ProgressTemplate>
        </ajax:UpdateProgress>
        <br />
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
                    &nbsp;&nbsp;<asp:HyperLink ID="ShowAllLink" runat="server" Text="Show All" NavigateUrl="ConLibHelpAll.aspx"></asp:HyperLink>
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
    </form>
</body>
</html>