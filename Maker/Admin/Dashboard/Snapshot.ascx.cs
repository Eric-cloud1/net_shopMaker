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
using System.Collections.Generic;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Reporting.StatusSnapshot;



public partial class Admin_Dashboard_Snapshot : System.Web.UI.UserControl
{


    protected void Page_Load(object sender, EventArgs e)
    {
        this.ReportTime.Text = "As of " + System.DateTime.Now.ToShortTimeString();
            BindList();
    }

    private void BindList()
    {
        string paramStartDate = string.Format(@"{0}/{1}/{2}", DateTime.Today.Month, 1, DateTime.Today.Year);

        SnapShotReport.DataSource = ReportDataSource.GetPaymentTypeSnapshotByDate(DateTime.Parse(paramStartDate), 1);
        SnapShotReport.Visible = true;
        SnapShotReport.DataBind();
    }


  



}
