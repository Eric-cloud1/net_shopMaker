using System;
using System.Collections.Generic;
using System.Text;

namespace MakerShop.Utility
{
    /// <summary>
    /// Enumeration that represents the severity of an error/log message
    /// </summary>
    public enum MessageSeverity
    {
        /// <summary>
        /// This is a debugging message
        /// </summary>
        Debug = 0,

        /// <summary>
        /// This is an information message
        /// </summary>
        Info,

        /// <summary>
        /// This is a warning message
        /// </summary>
        Warn,

        /// <summary>
        /// This is an error message
        /// </summary>
        Error,

        /// <summary>
        /// This is a fatal error message
        /// </summary>
        Fatal
    }
}
