<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DualList.ascx.cs" Inherits="Admin_UserControls_DualList" %>
<script src="http://localhost/able/js/jquery.min.js" type="text/javascript"></script>



    <script type="text/javascript">

        $(document).ready(function() {

            $("#btnAdd").click(function() {

            $("#<%=ListBox1.ClientID %> > option:selected").appendTo("#<%=ListBox2.ClientID %>");

            });

            $("#btnAddAll").click(function() {

            $("#<%=ListBox1.ClientID %> > option").appendTo("#<%=ListBox2.ClientID %>");

            });

            $("#btnRemove").click(function() {

            $("#<%=ListBox2.ClientID %> > option:selected").appendTo("#<%=ListBox1.ClientID %>");

            });

            $("#btnRemoveAll").click(function() {

            $("#<%=ListBox2.ClientID %> > option").appendTo("#<%=ListBox1.ClientID %>");

            });

        });             

    </script>




  <div >
    <table >
            <tr>
                <td align="center"><asp:Label ID="filter1Label"  runat="server" />
                <asp:DropDownList ID="ListBox1Filter" runat="server" AutoPostBack="true" />
                        <br /><br />
        <asp:ListBox ID="ListBox1" runat="server" Width="150px" Height="300px" SelectionMode="Multiple">

        </asp:ListBox>
 </td>
                <td>
                <br /><br />
        <input id="btnAdd"  type="button" value="&nbsp;>&nbsp;&nbsp;&nbsp;" /><br />
        <input id="btnAddAll" type="button" value="&nbsp;>>&nbsp;" /><br />
        <input id="btnRemoveAll"type="button" value="&nbsp;<<&nbsp;" /><br />
        <input id="btnRemove" type="button" value="&nbsp;<&nbsp;&nbsp;&nbsp;" /><br />
        <br /><br />
         </td>
                <td align="center"><asp:Label ID="filter2Label"  runat="server" />
                    <asp:DropDownList ID="ListBox2Filter" runat="server" AutoPostBack="true" />
                    <br /><br />


        <asp:ListBox ID="ListBox2" runat="server"  Width="150px" Height="300px" SelectionMode="Multiple"></asp:ListBox> 
              </td>
            </tr>
        </table>


    </div>

  

 