using System;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.ComponentModel;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Stores;
using MakerShop.Utility;


namespace MakerShop.Reporting.StatusSnapshot
{

    public partial class StatusSnapshotDetails
    {
       
        private bool _IsDirty;
        public bool IsDirty
        {
            get { return this._IsDirty; }
            set { this._IsDirty = value; }
        }

        private Int32 _OrderId;
        [DataObjectField(true, false, false)]
        public Int32 OrderId
        {
            get { return this._OrderId; }
            set
            {
                if (this._OrderId != value)
                {
                    this._OrderId = value;
                    this.IsDirty = true;
                }
            }
        }

        private Int32 _PaymentId;
        [DataObjectField(true, false, false)]
        public Int32 PaymentId
        {
            get { return this._PaymentId; }
            set
            {
                if (this._PaymentId != value)
                {
                    this._PaymentId = value;
                    this.IsDirty = true;
                }
            }
        }

        private Int32 _TransactionId;
        [DataObjectField(true, false, false)]
        public Int32 TransactionId
        {
            get { return this._TransactionId; }
            set
            {
                if (this._TransactionId != value)
                {
                    this._TransactionId = value;
                    this.IsDirty = true;
                }
            }
        }


        private System.DateTime _transactionDate;
        [DataObjectField(true, false, false)]
        public System.DateTime TransactionDate
        {
            get { return this._transactionDate; }
            set
            {
                if (this._transactionDate != value)
                {
                    this._transactionDate = value;
                    this.IsDirty = true;
                }
            }
        }


        private string _initialAuthorized;
        [DataObjectField(true, false, false)]
        public string InitialAuthorized
        {
            get { return this._initialAuthorized; }
            set
            {
                if (this._initialAuthorized != value)
                {
                    this._initialAuthorized = value;
                    this.IsDirty = true;
                }
            }
        }


        private string _initialCaptured;
        [DataObjectField(true, false, false)]
        public string InitialCaptured
        {
            get { return this._initialCaptured; }
            set
            {
                if (this._initialCaptured != value)
                {
                    this._initialCaptured = value;
                    this.IsDirty = true;
                }
            }
        }


        private string _initialShipped;
        [DataObjectField(true, false, false)]
        public string InitialShipped
        {
            get { return this._initialShipped; }
            set
            {
                if (this._initialShipped != value)
                {
                    this._initialShipped = value;
                    this.IsDirty = true;
                }
            }
        }


        private string _initialCancel;
        [DataObjectField(true, false, false)]
        public string InitialCancel
        {
            get { return this._initialCancel; }
            set
            {
                if (this._initialCancel != value)
                {
                    this._initialCancel = value;
                    this.IsDirty = true;
                }
            }
        }

        private string _initialFail;
        [DataObjectField(true, false, false)]
        public string InitialFail
        {
            get { return this._initialFail; }
            set
            {
                if (this._initialFail != value)
                {
                    this._initialFail = value;
                    this.IsDirty = true;
                }
            }
        }


        private string _initialVoid;
        [DataObjectField(true, false, false)]
        public string InitialVoid
        {
            get { return this._initialVoid; }
            set
            {
                if (this._initialVoid != value)
                {
                    this._initialVoid = value;
                    this.IsDirty = true;
                }
            }
        }

        private string _initialRefund;
        [DataObjectField(true, false, false)]
        public string InitialRefund
        {
            get { return this._initialRefund; }
            set
            {
                if (this._initialRefund != value)
                {
                    this._initialRefund = value;
                    this.IsDirty = true;
                }
            }
        }

        private string _trialAuthorized;
        [DataObjectField(true, false, false)]
        public string TrialAuthorized
        {
            get { return this._trialAuthorized; }
            set
            {
                if (this._trialAuthorized != value)
                {
                    this._trialAuthorized = value;
                    this.IsDirty = true;
                }
            }
        }


        private string _trialCaptured;
        [DataObjectField(true, false, false)]
        public string TrialCaptured
        {
            get { return this._trialCaptured; }
            set
            {
                if (this._trialCaptured != value)
                {
                    this._trialCaptured = value;
                    this.IsDirty = true;
                }
            }
        }


        private string _trialShipped;
        [DataObjectField(true, false, false)]
        public string TrialShipped
        {
            get { return this._trialShipped; }
            set
            {
                if (this._trialShipped != value)
                {
                    this._trialShipped = value;
                    this.IsDirty = true;
                }
            }
        }

        private string _trialCancel;
        [DataObjectField(true, false, false)]
        public string TrialCancel
        {
            get { return this._trialCancel; }
            set
            {
                if (this._trialCancel != value)
                {
                    this._trialCancel = value;
                    this.IsDirty = true;
                }
            }
        }

        private string _trialFail;
        [DataObjectField(true, false, false)]
        public string TrialFail
        {
            get { return this._trialFail; }
            set
            {
                if (this._trialFail != value)
                {
                    this._trialFail = value;
                    this.IsDirty = true;
                }
            }
        }

        private string _trialVoid;
        [DataObjectField(true, false, false)]
        public string TrialVoid
        {
            get { return this._trialVoid; }
            set
            {
                if (this._trialVoid != value)
                {
                    this._trialVoid = value;
                    this.IsDirty = true;
                }
            }
        }

        private string _trialRefund;
        [DataObjectField(true, false, false)]
        public string TrialRefund
        {
            get { return this._trialRefund; }
            set
            {
                if (this._trialRefund != value)
                {
                    this._trialRefund = value;
                    this.IsDirty = true;
                }
            }
        }

        private string _rebillAuthorized;
        [DataObjectField(true, false, false)]
        public string RebillAuthorized
        {
            get { return this._rebillAuthorized; }
            set
            {
                if (this._rebillAuthorized != value)
                {
                    this._rebillAuthorized = value;
                    this.IsDirty = true;
                }
            }
        }


        private string _rebillCaptured;
        [DataObjectField(true, false, false)]
        public string RebillCaptured
        {
            get { return this._rebillCaptured; }
            set
            {
                if (this._rebillCaptured != value)
                {
                    this._rebillCaptured = value;
                    this.IsDirty = true;
                }
            }
        }


        private string _rebillShipped;
        [DataObjectField(true, false, false)]
        public string RebillShipped
        {
            get { return this._rebillShipped; }
            set
            {
                if (this._rebillShipped != value)
                {
                    this._rebillShipped = value;
                    this.IsDirty = true;
                }
            }
        }


        private string _rebillCancel;
        [DataObjectField(true, false, false)]
        public string RebillCancel
        {
            get { return this._rebillCancel; }
            set
            {
                if (this._rebillCancel != value)
                {
                    this._rebillCancel = value;
                    this.IsDirty = true;
                }
            }
        }


        private string _rebillFail;
        [DataObjectField(true, false, false)]
        public string RebillFail
        {
            get { return this._rebillFail; }
            set
            {
                if (this._rebillFail != value)
                {
                    this._rebillFail = value;
                    this.IsDirty = true;
                }
            }
        }

        private string _rebillVoid;
        [DataObjectField(true, false, false)]
        public string RebillVoid
        {
            get { return this._rebillVoid; }
            set
            {
                if (this._rebillVoid != value)
                {
                    this._rebillVoid = value;
                    this.IsDirty = true;
                }
            }
        }

        private string _rebillRefund;
        [DataObjectField(true, false, false)]
        public string RebillRefund
        {
            get { return this._rebillRefund; }
            set
            {
                if (this._rebillRefund != value)
                {
                    this._rebillRefund = value;
                    this.IsDirty = true;
                }
            }
        }



    }

  

}