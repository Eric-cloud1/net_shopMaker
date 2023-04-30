<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Admin_Orders_CustomerService_Default" Title="Untitled Page" %>

<%@ Register src="../../UserControls/CallDisposition.ascx" tagname="CallDisposition" tagprefix="uc1" %>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
   <div class="pageHeader">
        <div class="caption">
         <h1>
            <asp:Localize ID="Caption" runat="server" Text="Campaign Order #{0}" EnableViewState="false"></asp:Localize></h1>
        </div>
    </div>
    
      <table>
      <tr>
      <td valign="top"><iframe  src="<%=this.Src %>" width="800" height="900" scrolling="auto" ></iframe></td>
      <td valign="top"><h1><asp:Localize ID="CallDispositionCaption" runat="server" Text="Call Disposition" EnableViewState="false"></asp:Localize></h1>
      <uc1:CallDisposition ID="CallDisposition1" panelWidth="250" runat="server" Visible="true" /></td>
      </tr>
      </table>
   
    
</asp:Content>

