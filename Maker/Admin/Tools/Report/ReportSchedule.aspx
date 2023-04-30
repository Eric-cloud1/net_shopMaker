<%@ Page Language="C#" MasterPageFile="~/Admin/Admin.master" AutoEventWireup="true" CodeFile="ReportSchedule.aspx.cs" Inherits="Admin_Tools_ReportSchedule" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <script type="text/javascript" language="javascript">
        function UsersSelected()
        {
            var count = 0;
            for(i = 0; i< document.forms[0].elements.length; i++){
                var e = document.forms[0].elements[i];
                var name = e.name;
                if ((e.type == 'checkbox') && (name.endsWith('SelectUserCheckBox')) && (e.checked))
                {
                    count ++;
                }
            }
            return (count > 0);
        }
    </script>
    <div class="pageHeader">
        <div class="caption"><h1><asp:Localize ID="Caption" runat="server" Text="Manage Report Schedule"></asp:Localize></h1></div>
    </div>
    <ajax:UpdatePanel ID="SearchAjax" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
        <center>
             <table  class="form" cellpadding="0" cellspacing="0" border="0" width="900px" >
              <tr><td>
            <asp:Label ID="UserAddedMessage" runat="server" Text="Schedule {0} added." SkinID="GoodCondition" Visible="False" EnableViewState="false"></asp:Label>
            </td>
            <tr><td>
              <asp:GridView ID="ScheduleReportGrid" runat="server" AutoGenerateColumns="False" DataKeyNames="ReportId" 
                DefaultSortDirection="Ascending" DefaultSortExpression="Procedure" OnSorting="ScheduleReportGrid_Sorting" 
                AllowPaging="true" PagerStyle-CssClass="paging" PageSize="20" PagerSettings-Position="Bottom" 
                AllowSorting="true" SkinID="PagedList" width="100%"   OnRowDataBound="ScheduleReportGrid_RowDataBound">
                <Columns>
                    <asp:TemplateField HeaderText="Select" >
                        <HeaderStyle horizontalalign="Center" />
                        <ItemStyle horizontalalign="Center" Width="80px" />
                        <ItemTemplate>
                           <asp:Label ID="reportIdlabel" runat="server" Text='<%#Eval("ReportId")%>' style="visibility:hidden" ></asp:Label>
                           <asp:CheckBox ID="selectReport"  runat="server" Checked='<%#IsChecked(Container.DataItem) %>'  OnCheckedChanged="Check_Clicked" AutoPostBack="true"></asp:CheckBox>
                       </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Description" SortExpression="LoweredUserName">
                        <HeaderStyle horizontalalign="Left" />
                        <ItemStyle horizontalalign="Left" Width="250px" />
                        <ItemTemplate>
                            <asp:Label ID="UserNameLabel" runat="server" Text='<%#Eval("Description")%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Name" SortExpression="A.LastName">
                        <HeaderStyle horizontalalign="Left" />
                        <ItemStyle horizontalalign="Left" />
                        <ItemTemplate>
                            <asp:Label ID="FullNameLabel" runat="server" Text='<%#Eval("Procedure")%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
              
                <asp:TemplateField HeaderText="Schedule" SortExpression="A.LastName">
                        <HeaderStyle horizontalalign="Left" />
                        <ItemStyle horizontalalign="Left" />
                        <ItemTemplate>
                            <asp:DropDownList ID="Schedules" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Schedules_SelectedIndexChanged"/>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <div align="center">
                        <asp:Label runat="server" ID="noUsersFound" enableViewState="false" Text="No users match the search criteria."/>
                    </div>
                </EmptyDataTemplate>
            </asp:GridView>   
     </td></tr>
     </table>
     </center>
        </ContentTemplate>
    </ajax:UpdatePanel>
</asp:Content>