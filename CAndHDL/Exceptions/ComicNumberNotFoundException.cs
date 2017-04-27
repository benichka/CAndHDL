using System;

namespace CAndHDL.Exceptions
{
    /// <summary>
    /// Custom exception class used when a number is not found in a comic relative URL.
    /// </summary>
    class ComicNumberNotFoundException : Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ComicNumberNotFoundException()
        {
        }

        /// <summary>
        /// Create a ComicNumberNotFoundException with a custom message
        /// </summary>
        /// <param name="message">ComicNumberNotFoundException message</param>
        public ComicNumberNotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// Create a ComicNumberNotFoundException with a custom message and a reference to the inner exception
        /// </summary>
        /// <param name="message">ComicNumberNotFoundException message</param>
        /// <param name="innerException">Reference to the inner exception</param>
        public ComicNumberNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
