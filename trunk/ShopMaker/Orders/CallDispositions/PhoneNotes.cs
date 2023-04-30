using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Text;
using MakerShop.Common;
using MakerShop.Data;
using MakerShop.Orders;
using MakerShop.Users;
using MakerShop.Utility;


namespace MakerShop.Orders
{
    public partial class PhoneNotes
    {
        /// <summary>
        /// Type of order note
        /// </summary>
        public NoteType NoteType
        {
            get
            {
                return (NoteType)this.NoteTypeId;
            }
            set
            {
                this.NoteTypeId = (byte)value;
            }
        }
    }
}
