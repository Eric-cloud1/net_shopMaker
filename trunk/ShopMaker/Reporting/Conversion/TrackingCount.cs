using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MakerShop.Reporting
{
    public class TrackingCount
    {
        private int _affiliateId;
        private int _clicks;
        private int _leads;
        private int _sales;
        private int _orders;
        private DateTime? _lastOrderDate;
        private int _cx;


        public TrackingCount() { }

        public string Affiliate
        {
            get
            {
                try
                {
                    return   MakerShop.Marketing.AffiliateDataSource.Load(_affiliateId, true).Name;
                }
                catch
                {
                    return "";
                }
            }
        }
        public string Display
        {
            get
            {
                string x = "(" + _affiliateId.ToString() + ") ";

                try
                {
                    x += MakerShop.Marketing.AffiliateDataSource.Load(_affiliateId, true).Name;
                }
                catch
                {
                    
                }
                return x;
            }
        }


       public TrackingCount(int affiliateId, int clicks, int leads, int sales, int orders, DateTime lastOrderDate, int cx)
      {
            this.AffiliateId = affiliateId;
            this.Clicks = clicks;
            this.Leads = leads;
            this.Sales = sales;
            this.Orders = orders;
            this.LastOrderDate = lastOrderDate;
            this.CX = cx;
        }



     public DateTime? LastOrderDate
       {
            get { return _lastOrderDate; }
            set { _lastOrderDate = value; }
        }

     public int CX
     {
         get { return _cx; }
         set { _cx = value; }
     }

        public int AffiliateId
        {
            get { return _affiliateId; }
            set { _affiliateId = value; }
        }

        public int Clicks
        {
            get { return _clicks; }
            set { _clicks = value; }
        }

        public int Leads
        {
            get { return _leads; }
            set { _leads = value; }
        }

        public int Sales
        {
            get { return _sales; }
            set { _sales = value; }
        }

        public int Orders
        {
            get { return _orders; }
            set { _orders = value; }
        }


    }
}
