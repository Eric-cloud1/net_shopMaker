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


namespace MakerShop.Orders.ChargeBack
{

    public partial class ChargeBackDetails
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


        private System.DateTime _CreateDate;

        [DataObjectField(true, false, false)]
        public System.DateTime CreateDate
        {
            get { return this._CreateDate; }
            set
            {
                if (this._CreateDate != value)
                {
                    this._CreateDate = value;
                    this.IsDirty = true;
                }
            }
        }


        private System.DateTime _InitiateDate;

        [DataObjectField(true, false, false)]
        public System.DateTime InitiateDate
        {
            get { return this._InitiateDate; }
            set
            {
                if (this._InitiateDate != value)
                {
                    this._InitiateDate = value;
                    this.IsDirty = true;
                }
            }
        }


        private string _ReasonCode;

        [DataObjectField(true, false, false)]
        public string ReasonCode
        {
            get { return this._ReasonCode; }
            set
            {
                if (this._ReasonCode != value)
                {
                    this._ReasonCode = value;
                    this.IsDirty = true;
                }
            }
        }


        private string _ReasonDescription;

        [DataObjectField(true, false, false)]
        public string ReasonDescription
        {
            get { return this._ReasonDescription; }
            set
            {
                if (this._ReasonDescription != value)
                {
                    this._ReasonDescription = value;
                    this.IsDirty = true;
                }
            }
        }


        private string _CaseNumber;

        [DataObjectField(true, false, false)]
        public string CaseNumber
        {
            get { return this._CaseNumber; }
            set
            {
                if (this._CaseNumber != value)
                {
                    this._CaseNumber = value;
                    this.IsDirty = true;
                }
            }
        }


        private string _ReferenceNumber;

        [DataObjectField(true, false, false)]
        public string ReferenceNumber
        {
            get { return this._ReferenceNumber; }
            set
            {
                if (this._ReferenceNumber != value)
                {
                    this._ReferenceNumber = value;
                    this.IsDirty = true;
                }
            }
        }


        private Int16 _ChargeBackStatus;

        [DataObjectField(true, false, false)]
        public Int16 ChargeBackStatus
        {
            get { return this._ChargeBackStatus; }
            set
            {
                if (this._ChargeBackStatus != value)
                {
                    this._ChargeBackStatus = value;
                    this.IsDirty = true;
                }
            }
        }


        private string _Comment;

        [DataObjectField(true, false, false)]
        public string Comment
        {
            get { return this._Comment; }
            set
            {
                if (this._Comment != value)
                {
                    this._Comment = value;
                    this.IsDirty = true;
                }
            }
        }
    }

    public enum ChargeBackStatus : short
    {
        Unknown = 0,
        Pending = 1,
        Won = 2,
        Lost = 3
    }

}