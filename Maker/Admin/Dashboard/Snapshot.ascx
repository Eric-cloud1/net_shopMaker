<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Snapshot.ascx.cs" Inherits="Admin_Dashboard_Snapshot" %>
<%@ Register Assembly="MakerShop.Web" Namespace="MakerShop.Web.UI.WebControls" TagPrefix="cb" %>

<div style="text-align: right; width: 100%">
    <asp:Label ID="ReportTime" runat="server" />
</div>
<asp:DataList ID="SnapShotReport" runat="server" Visible="False" ShowWhenEmpty="False" Width="100%">
    <ItemTemplate>
   <table border="1" style="border-color: Black; border-collapse: collapse; border-width: thin" cellspacing="0" width="100%">    
            <tr><tr><td colspan="7" style="text-align:center; width:100%;"><Strong>INITIAL</Strong></td></tr>
            <tr style="background-color: #EEEEEE;">  
            <td style="text-align: center; width: 22px;" class="gridHeader">
            
             <cb:ToolTipLabel ID="AuthorizeLabel" runat="server" Text="Au" ToolTip="Authorized" />
             
            </td>
            <td style="text-align: center; width: 22px;" class="gridHeader">
              <cb:ToolTipLabel ID="CaptureLabel" runat="server"  Text="Cp" ToolTip="Captured" />
            </td>
            <td style="text-align: center; width: 22px;" class="gridHeader">
            <cb:ToolTipLabel ID="RefundLabel" runat="server"  Text="Rf" ToolTip="Refund" />
            </td>
            <td style="text-align: center; width: 22px;" class="gridHeader">
            <cb:ToolTipLabel ID="ShippedLabel" runat="server"  Text="Sh" ToolTip="Shipped" />
            </td>
            <td style="text-align: center; width: 22px;" class="gridHeader">
            <cb:ToolTipLabel ID="VoidLabel" runat="server"  Text="Vd" ToolTip="Void" />
            </td>
            <td style="text-align: center; width: 22px;" class="gridHeader">
             <cb:ToolTipLabel ID="FailedLabel" runat="server"  Text="Fa" ToolTip="Failed" />
            </td>
            <td style="text-align: center; width: 22px;" class="gridHeader">
             <cb:ToolTipLabel ID="CancelLabel" runat="server"  Text="Cl" ToolTip="Cancel" />
            </td>
            </tr>
            <tr style="background-color: #FFFFFF;">
                <td style="text-align: center; width: 22px;">
                    <%# DataBinder.Eval(Container.DataItem, "InitialAuthorized")%>
                </td>
          
                <td style="text-align: center; width: 22px;">
                    <%# DataBinder.Eval(Container.DataItem, "InitialCaptured")%>
                </td>
 
                <td style="text-align: center; width: 22px;">
                    <%# DataBinder.Eval(Container.DataItem, "InitialRefund")%>
                </td>
                
                <td style="text-align: center; width: 22px;">
                    <%# DataBinder.Eval(Container.DataItem, "InitialShipped")%>
                </td>
                
                <td style="text-align: center; width: 22px;">
                    <%# DataBinder.Eval(Container.DataItem, "InitialVoid")%>
                </td>
                 <td style="text-align: center; width: 22px;">
                    <%# DataBinder.Eval(Container.DataItem, "InitialFail")%>
                </td>
                      <td style="text-align: center; width: 22px;">
                    <%# DataBinder.Eval(Container.DataItem, "InitialCancel")%>
                </td>
            </tr>
              <tr><tr><td colspan="7" style="text-align:center;width:100%;"><Strong>TRIAL</Strong></td></tr>
            <tr style="background-color: #EEEEEE;">  
            <td style="text-align: center; width: 22px;" class="gridHeader">
             <cb:ToolTipLabel ID="ToolTipLabel1" runat="server"  Text="Au" ToolTip="Authorized" />
            </td>
            <td style="text-align: center; width: 22px;" class="gridHeader">
              <cb:ToolTipLabel ID="ToolTipLabel2" runat="server"  Text="Cp" ToolTip="Captured" />
            </td>
            <td style="text-align: center; width: 22px;" class="gridHeader">
            <cb:ToolTipLabel ID="ToolTipLabel3" runat="server"  Text="Rf" ToolTip="Refund" />
            </td>
            <td style="text-align: center; width: 22px;" class="gridHeader">
            <cb:ToolTipLabel ID="ToolTipLabel4" runat="server"  Text="Sh" ToolTip="Shipped" />
            </td>
            <td style="text-align: center; width: 22px;" class="gridHeader">
            <cb:ToolTipLabel ID="ToolTipLabel5" runat="server"  Text="Vd" ToolTip="Void" />
            </td>
            <td style="text-align: center; width: 22px;" class="gridHeader">
             <cb:ToolTipLabel ID="ToolTipLabel6" runat="server"  Text="Fa" ToolTip="Failed" />
            </td>
            <td style="text-align: center; width: 22px;" class="gridHeader">
             <cb:ToolTipLabel ID="ToolTipLabel7" runat="server"  Text="Cl" ToolTip="Cancel" />
            </td>
            </tr>
             <tr style="background-color: #FFFFFF;">
                <td style="text-align: center; width: 22px;">
                    <%# DataBinder.Eval(Container.DataItem, "TrialAuthorized")%>
                </td>
                <td style="text-align: center; width: 22px;">
                    <%# DataBinder.Eval(Container.DataItem, "TrialCancel")%>
                </td>
                 
                <td style="text-align: center; width: 22px;">
                    <%# DataBinder.Eval(Container.DataItem, "TrialRefund")%>
                </td>
                
                    <td style="text-align: center; width: 22px;">
                    <%# DataBinder.Eval(Container.DataItem, "TrialShipped")%>
                </td>
                
                   <td style="text-align: center; width: 22px;">
                    <%# DataBinder.Eval(Container.DataItem, "TrialVoid")%>
                </td>
                
                  <td style="text-align: center; width: 22px;">
                    <%# DataBinder.Eval(Container.DataItem, "TrialFail")%>
                </td>
                
                <td style="text-align: center; width: 22px;">
                    <%# DataBinder.Eval(Container.DataItem, "TrialCaptured")%>
                </td>           
            </tr>
            <tr><tr><td colspan="7" style="text-align:center;width:100%;"><strong>RECURRING</strong></td></tr>
             <tr style="background-color: #EEEEEE;">  
            <td style="text-align: center; width: 22px;" class="gridHeader">
             <cb:ToolTipLabel ID="ToolTipLabel8" runat="server"  Text="Au" ToolTip="Authorized" />
            </td>
            <td style="text-align: center; width: 22px;" class="gridHeader">
              <cb:ToolTipLabel ID="ToolTipLabel9" runat="server"  Text="Cp" ToolTip="Captured" />
            </td>
            <td style="text-align: center; width: 22px;" class="gridHeader">
            <cb:ToolTipLabel ID="ToolTipLabel10" runat="server"  Text="Rf" ToolTip="Refund" />
            </td>
            <td style="text-align: center; width: 22px;" class="gridHeader">
            <cb:ToolTipLabel ID="ToolTipLabel11" runat="server"  Text="Sh" ToolTip="Shipped" />
            </td>
            <td style="text-align: center; width: 22px;" class="gridHeader">
            <cb:ToolTipLabel ID="ToolTipLabel12" runat="server"  Text="Vd" ToolTip="Void" />
            </td>
            <td style="text-align: center; width: 22px;" class="gridHeader">
             <cb:ToolTipLabel ID="ToolTipLabel13" runat="server"  Text="Fa" ToolTip="Failed" />
            </td>
            <td style="text-align: center; width: 22px;" class="gridHeader">
             <cb:ToolTipLabel ID="ToolTipLabel14" runat="server"  Text="Cl" ToolTip="Cancel" />
            </td>
            </tr>
             <tr  style="background-color: #FFFFFF;">
                <td style="text-align: center; width: 22px;">
                    <%# DataBinder.Eval(Container.DataItem, "RebillAuthorized")%>
                </td>
            
                <td style="text-align: center; width: 22px;">
                    <%# DataBinder.Eval(Container.DataItem, "RebillCaptured")%>
                </td>
              
                
                <td style="text-align: center; width: 22px;">
                    <%# DataBinder.Eval(Container.DataItem, "RebillRefund")%>
                </td>
                
                <td style="text-align: center; width: 22px;">
                    <%# DataBinder.Eval(Container.DataItem, "RebillShipped")%>
                </td>
                
                <td style="text-align: center; width: 22px;">
                    <%# DataBinder.Eval(Container.DataItem, "RebillVoid")%>
                </td>
                  <td style="text-align: center; width: 22px;">
                    <%# DataBinder.Eval(Container.DataItem, "RebillFail")%>
                </td>
                
                    <td style="text-align: center; width: 22px;">
                    <%# DataBinder.Eval(Container.DataItem, "RebillCancel")%>
                </td>
            </tr>
            </table>
             </ItemTemplate>
  

</asp:DataList>

