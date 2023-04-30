using System;
using MakerShop.Common;
using MakerShop.Users;
using MakerShop.Stores;

namespace MakerShop.Orders
{
    /// <summary>
    /// Class that represents an OrderNote object in the database
    /// </summary>
    public partial class OrderNote
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

        /// <summary>
        /// Is this a private note?
        /// </summary>
        public bool IsPrivate
        {
            get
            {
                return ((this.NoteType == NoteType.Private) || (this.NoteType == NoteType.SystemPrivate));
            }
        }

        /// <summary>
        /// Is this a system note?
        /// </summary>
        public bool IsSystem
        {
            get
            {
                return ((this.NoteType == NoteType.SystemPublic) || (this.NoteType == NoteType.SystemPrivate));
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="orderId">Id of the order this note is associated with</param>
        /// <param name="userId">Id of the user this note is associated with</param>
        /// <param name="createdDate">Creation date of this note</param>
        /// <param name="comment">The note comment</param>
        /// <param name="noteType">The type of note</param>
        public OrderNote(int orderId, int userId, DateTime createdDate, string comment, NoteType noteType)
        {
            this.OrderId = orderId;
            this.UserId = userId;
            this.CreatedDate = createdDate;
            this.Comment = comment;
            this.NoteType = noteType;
        }

        /// <summary>
        /// Save this OrderNote object to the database
        /// </summary>
        /// <returns><b>SaveResult</b> enumeration that represents the result of save operation</returns>
        public SaveResult Save()
        {
            SaveResult result = this.BaseSave();
            if (result == SaveResult.RecordInserted)
            {
                StoreEventEngine.OrderNoteAdded(this);
            }
            return result;
        }
    }
}