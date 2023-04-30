<%@ Page Language="C#" MasterPageFile="~/Admin/Orders/Order.master" AutoEventWireup="true" CodeFile="CallDisposition.aspx.cs" Inherits="Admin_Orders_Call_CallDisposition" Title="Call Disposition" %>


<%@ Register src="../../UserControls/CallDisposition.ascx" tagname="CallDisposition" tagprefix="uc1" %>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageHeader">
        <div class="caption">
         <h1>
            <asp:Localize ID="Caption" runat="server" Text="Call Disposition Order #{0} - {1}" EnableViewState="false"></asp:Localize></h1>
        </div>
    </div>
      <div  style="margin:10px 0px;width:500px">

      <uc1:CallDisposition ID="CallDisposition1" panelWidth="500" runat="server" />

      </div>
    
</asp:Content>

