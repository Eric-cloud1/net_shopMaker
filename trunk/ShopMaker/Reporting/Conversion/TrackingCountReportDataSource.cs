using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Orders;
using MakerShop.Utility;
using MakerShop.Marketing;
using MakerShop.Users;
using MakerShop.Products;
using System.Web;
using System.Linq;

namespace MakerShop.Reporting
{
    
    public partial class ReportDataSource
    {
        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<TrackingCount> GetTrackingTotalCounts(DateTime startDate, DateTime endDate)
        {
            List<TrackingCount> totalConversion = new List<TrackingCount>();

            Database database = Token.Instance.Database;


            using (System.Data.Common.DbCommand cmd = database.GetStoredProcCommand("rpt_TrackingCounts"))
            {
                database.AddInParameter(cmd, "@StartDate", DbType.DateTime, startDate);
                database.AddInParameter(cmd, "@isTotal", DbType.Int16, 1);

                if(endDate != System.DateTime.MaxValue)
                    database.AddInParameter(cmd, "@EndDate", DbType.DateTime, endDate);

                else
                    database.AddInParameter(cmd, "@EndDate", DbType.DateTime, null);


                DataSet ds = database.ExecuteDataSet(cmd);
               // DataSet ds = database.ExecuteDataSet(cmd);
                using (IDataReader dr = database.ExecuteReader(cmd))
                {
                    TrackingCount trackingCount = null;
                    while (dr.Read())
                    {
                        trackingCount = new TrackingCount();
                        trackingCount.AffiliateId = 0;
                        trackingCount.Clicks = int.Parse(dr["Clicks"].ToString());
                        trackingCount.Leads = int.Parse(dr["Leads"].ToString());
                        trackingCount.Sales = int.Parse(dr["Sales"].ToString());
                        trackingCount.Orders = int.Parse(dr["Orders"].ToString());
                        trackingCount.CX = int.Parse(dr["Cx"].ToString());


                        if (dr["LastOrderDate"] != DBNull.Value)
                            trackingCount.LastOrderDate = (DateTime?)dr["LastOrderDate"];
                   }
                    totalConversion.Add(trackingCount);
                    trackingCount = null;
                    dr.Close();
                }

            }
            
            return totalConversion;
        }

        [System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Select)]
        public static List<TrackingCount> GetTrackingCounts(DateTime startDate, DateTime endDate, string sortExpression, bool asc)
        {
     
            Database database = Token.Instance.Database;
            List<TrackingCount> conversion = new List<TrackingCount>();
          
            using (System.Data.Common.DbCommand cmd = database.GetStoredProcCommand("rpt_TrackingCounts"))
            {
                database.AddInParameter(cmd, "@StartDate", DbType.DateTime, startDate);
                database.AddInParameter(cmd, "@isTotal", DbType.Int16, 0);

                if (endDate != System.DateTime.MaxValue)
                    database.AddInParameter(cmd, "@EndDate", DbType.DateTime, endDate);

                else
                    database.AddInParameter(cmd, "@EndDate", DbType.DateTime, null);
              

               // DataSet ds = database.ExecuteDataSet(cmd);
                using (IDataReader dr = database.ExecuteReader(cmd))
                {
                    TrackingCount trackingCount = null;
                    while (dr.Read())
                    {
                        trackingCount = new TrackingCount();
                        trackingCount.AffiliateId = int.Parse(dr["AffiliateId"].ToString());
                        trackingCount.Clicks = int.Parse(dr["Clicks"].ToString());
                        trackingCount.Leads = int.Parse(dr["Leads"].ToString());
                        trackingCount.Sales = int.Parse(dr["Sales"].ToString());
                        trackingCount.Orders = int.Parse(dr["Orders"].ToString());
                        trackingCount.CX = int.Parse(dr["Cx"].ToString());
                        if (dr["LastOrderDate"] != DBNull.Value)
                            trackingCount.LastOrderDate = (DateTime?)dr["LastOrderDate"];
                        conversion.Add(trackingCount);
                        trackingCount = null;
                    }
                    dr.Close();
                }

            }
            var sortedTrackingCount = from tc in conversion orderby tc.AffiliateId select tc;
            switch (sortExpression)
            {
                case "Cx":
                    if (asc == true) { sortedTrackingCount = from tc in conversion orderby tc.CX ascending select tc; break; }
                    sortedTrackingCount = from tc in conversion orderby tc.CX descending select tc;
                    break;
                case "Clicks":
                    if (asc == true){ sortedTrackingCount = from tc in conversion orderby tc.Clicks ascending select tc; break;}
                        sortedTrackingCount = from tc in conversion orderby tc.Clicks descending  select tc;
                    break;

                case "Leads":
                    if (asc == true) { sortedTrackingCount = from tc in conversion orderby tc.Leads ascending select tc; break; }
                        sortedTrackingCount = from tc in conversion orderby tc.Leads descending select tc;
                    break;

                case "Sales":
                    if (asc == true) { sortedTrackingCount = from tc in conversion orderby tc.Sales ascending select tc; break; }
                        sortedTrackingCount = from tc in conversion orderby tc.Sales descending select tc;
                    break;

                case "Orders":
                    if (asc == true) { sortedTrackingCount = from tc in conversion orderby tc.Orders ascending select tc; break; }
                        sortedTrackingCount = from tc in conversion orderby tc.Orders descending select tc;
                    break;

                case "LastOrderDate":
                    if (asc == true) { sortedTrackingCount = from tc in conversion orderby tc.LastOrderDate ascending select tc; break; }
                        sortedTrackingCount = from tc in conversion orderby tc.LastOrderDate descending select tc;
                    break;

                default:
                    if (asc == true) {  sortedTrackingCount = from tc in conversion orderby tc.AffiliateId ascending select tc; break; }
                        sortedTrackingCount = from tc in conversion orderby tc.AffiliateId descending select tc;
                    break;
            }


            List<TrackingCount> sortedConversion = new List<TrackingCount>(sortedTrackingCount);

            return sortedConversion;

        }
    }
}
