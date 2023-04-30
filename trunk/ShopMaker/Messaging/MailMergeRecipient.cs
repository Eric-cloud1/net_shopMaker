//-----------------------------------------------------------------------
// <copyright file="MailMergeRecipient.cs" company="Able Solutions Corporation">
//     Copyright (c) 2009 Able Solutions Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace MakerShop.Messaging
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Net.Mail;
    using System.Text;
    using MakerShop.Utility;

    /// <summary>
    /// Contains recipient address and any associated merge data
    /// </summary>
    public class MailMergeRecipient
    {
        /// <summary>
        /// Private storage for Address property
        /// </summary>
        private Collection<MailAddress> _RecipientAddresses = new Collection<MailAddress>();
        
        /// <summary>
        /// Private storage for NVelocity parameters associated with this mail target
        /// </summary>
        private Hashtable _Parameters;

        /// <summary>
        /// Initializes a new instance of the MailRecipient class.
        /// </summary>
        /// <param name="recipientAddress">Email address for the recipient</param>
        public MailMergeRecipient(string recipientAddress)
            : this(recipientAddress, new Hashtable())
        {
        }

        /// <summary>
        /// Initializes a new instance of the MailRecipient class.
        /// </summary>
        /// <param name="recipientAddress">Email address for the recipient</param>
        /// <param name="parameters">Any associated data for the recipient message merge</param>
        public MailMergeRecipient(string recipientAddress, Hashtable parameters)
        {
            this.Address = recipientAddress;
            _Parameters = parameters;
        }
        
        /// <summary>
        /// Gets or sets a list the email address for this recipient.  The value can also
        /// be a comma delimited list of addresses for multiple destinations.
        /// </summary>
        public string Address
        {
            get
            {
                return MailAddressHelper.ConvertToList(_RecipientAddresses);
            }
            set
            {
                _RecipientAddresses = MailAddressHelper.ParseList(value);
                if (_RecipientAddresses.Count == 0)
                {
                    throw new ArgumentException("At least one email address must be specified.", "recipientAddress");
                }
            }
        }

        /// <summary>
        /// Gets the collection of mailaddress for the recipient list
        /// </summary>
        internal Collection<MailAddress> RecipientAddresses
        {
            get
            {
                return _RecipientAddresses;
            }
        }

        /// <summary>
        /// Gets the parameters for NVelocity processing
        /// </summary>
        public Hashtable Parameters
        {
            get { return _Parameters; }
        }
    }
}
