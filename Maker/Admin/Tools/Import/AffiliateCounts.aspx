<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="AffiliateCounts.aspx.cs" Inherits="Admin_Tools_Import_AffiliateCounts" Title="Import Affiliate Counts" %>
<%@ Register Assembly="ComponentArt.Web.UI" Namespace="ComponentArt.Web.UI" TagPrefix="ComponentArt" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">


  <ajax:UpdatePanel ID="ReportAjax" runat="server" UpdateMode="Conditional">
  <ContentTemplate>
  
       </ContentTemplate>
  </ajax:UpdatePanel>
  
  
<table align="center" class="form" cellpadding="0" cellspacing="0" border="0" width="400px" >
        <tr>
            <td style="text-align: center;" colspan="3" >
             <div class="pageHeader">
                <div class="caption"><h1>
                <asp:Localize ID="PageCaption"  runat="server" Text="File Upload"></asp:Localize></h1>
                </div>
                </div>
                
            </td>
        </tr>
        <tr>
            <td class="caption">
                <asp:Localize ID="Caption" runat="server"  Text=" Upload CX CSV File"></asp:Localize>
            </td>
            <td>
                <asp:FileUpload ID="FileUpload1" runat="server" />
            </td>
            <td>
                <asp:Button runat="server" ID="upload" Text="Upload" onclick="Submit_Click" />
            </td>
        </tr>
        <tr>
            <td class="style1" colspan="3">
            <asp:Panel runat="server" ID="statusPanel" Visible="false" Width="303px" Height="20px" 
                    style="margin-bottom: 27px">
            <asp:Label ID="lUploadStatus" runat="server" />
    </asp:Panel>
    <asp:Panel runat="server" ID="responsePanel" Visible="false" Width="100%" Height="285px" 
                    style="margin-bottom: 27px">
        <asp:GridView ID="gridBindCodes" runat="server" 
            AutoGenerateColumns="false" onrowdatabound="gridBindCodes_RowDataBound" 
            width="100%" >
            <Columns>
                <asp:TemplateField HeaderText="Affiliate Codes">
                    <ItemTemplate>
                        <asp:Label ID="affiliateCodeLabel" runat="server" ></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Affiliate Id">
                    <ItemTemplate>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                            ControlToValidate="affiliateIdText" ErrorMessage="Required" SetFocusOnError="true" ></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                            ErrorMessage="Numeric Entry Only" SetFocusOnError="True" ValidationExpression="^\d+$" ControlToValidate="affiliateIdText"></asp:RegularExpressionValidator>
                        <asp:TextBox ID="affiliateIdText" runat="server" ></asp:TextBox>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <br /><br />
        <asp:Button ID="submit" runat="server" Text="Submit" onclick="Submit_Click_Two"/>
        <asp:Label ID="bindSubmitUpdateLabel" SkinID="ErrorCondition" runat="server" />
    </asp:Panel>
    
            </td>
        </tr>
    </table>

</asp:Content>

