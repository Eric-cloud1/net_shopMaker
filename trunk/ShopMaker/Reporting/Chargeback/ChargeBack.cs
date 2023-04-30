using System;
using System.Collections.Generic;
using System.Text;
using MakerShop.Common;


namespace MakerShop.Reporting
{
    [Serializable]
    public class ChargeBack
    {
        private string _name;
        /// <summary>
        /// The product Name
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private decimal _successful;
        /// <summary>
        /// Successful charges aggregate
        /// </summary>
        public decimal Successful
        {
            get { return  _successful; }
            set { _successful = value; }
        }

        private decimal _authorized;
        /// <summary>
        /// Successful charges aggregate
        /// </summary>
        public decimal Authorized
        {
            get { return _authorized; }
            set { _authorized = value; }
        }


        private decimal _chargeback;
        /// <summary>
        /// Charges back aggregate
        /// </summary>
        public decimal Chargeback
        {
            get { return  _chargeback;  }
            set { _chargeback = value; }
        }


        private decimal _CBRatiotTotal;
        /// <summary>
        /// Percent
        /// </summary>
        public decimal CBRatioTotal
        {
            get { 

                if (Successful ==0) 
                    return 100;

                decimal ratioTotal = this.Authorized + this.Successful;
                ratioTotal = System.Math.Round(Chargeback / ratioTotal, 5);



                return ratioTotal;
            
            }
            set { _CBRatiotTotal = value; }
        }

        private decimal _CBRatio;
        /// <summary>
        /// Percent
        /// </summary>
        public decimal CBRatio
        {
            get
            {

                if (Successful == 0)
                    return 100;

                decimal ratio = this.Successful;
                ratio = System.Math.Round(Chargeback / ratio, 5);



                return ratio;

            }
            set { _CBRatio = value; }
        } 


    }
}
