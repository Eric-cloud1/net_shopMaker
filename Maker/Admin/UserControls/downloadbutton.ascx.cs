using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public partial class Admin_UserControls_downloadbutton : System.Web.UI.UserControl
{
    private string _fileName;
    public string FileName
    {
        get
        {
            return _fileName;
        }

        set
        {
            _fileName = value;
        }
    }


    public DataTable DownloadData
    {
        get
        {
            Object ds = Session[this.ClientID + "_DownloadData"];
            if (ds != null)
                return (DataTable)ds;

            return null;
        }
        set { Session[this.ClientID + "_DownloadData"] = value; }
    }



    private DateTime _startDate;
    public DateTime StartDate
    {
        get
        {
            return _startDate;
        }

        set
        {
            _startDate = value;
        }
    }

    private DateTime _endDate;
    public DateTime EndDate
    {
        get
        {
              return _endDate;
        }

        set
        {
            _endDate = value;
        }
    }

    [System.Security.Permissions.PermissionSet
   (System.Security.Permissions.SecurityAction.Demand,
    Name = "FullTrust")]
    protected void Page_Load(object sender, EventArgs e)
    {
        registerScript();
    }

    protected void registerScript()
    {

        Page.ClientScript.RegisterClientScriptBlock(
            GetType(),
           "script1",
           @"<script language='javascript' type='text/javascript'>
function downloadXml() {

  var cbEmail =  document.getElementById('" + this.cbEmail.ClientID + @"')
 
  if(cbEmail.checked == false)
      window.open('../ReportDownload.aspx?downloadXML=1&fileName=" + FileName + @"&startDate=" + this.StartDate + @"&endDate=" + this.EndDate + @"&data=" + this.ClientID + @"_DownloadData','Download','height=150,width=450','toolbar=no' );;


  if(cbEmail.checked == true)
      window.open('../ReportDownload.aspx?downloadXML=0&fileName=" + FileName + @"&startDate=" + this.StartDate + @"&endDate=" + this.EndDate + @"&data=" + this.ClientID + @"_DownloadData','Download','height=150,width=450','toolbar=no');;

   }

function downloadCSV() {

  var cbEmail =  document.getElementById('" + this.cbEmail.ClientID + @"')
 
 
  if(cbEmail.checked == false)
     window.open('../ReportDownload.aspx?downloadCSV=1&fileName=" + FileName + @"&startDate=" + this.StartDate + @"&endDate=" + this.EndDate + @"&data=" + this.ClientID + @"_DownloadData','Download','height=150,width=450','toolbar=no');;

  if(cbEmail.checked == true)
      window.open('../ReportDownload.aspx?downloadCSV=0&fileName=" + FileName + @"&startDate=" + this.StartDate + @"&endDate=" + this.EndDate + @"&data=" + this.ClientID + @"_DownloadData','Download','height=150,width=450','toolbar=no');;

   }

<" + "/script>");


    }

}
