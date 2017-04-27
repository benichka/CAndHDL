using System;

namespace CAndHDL.Exceptions
{
    /// <summary>
    /// Custom exception class used when the comic URL (page URL, relative URL or download URL) is not found on a comic page.
    /// </summary>
    class ComicURLNotFoundException : Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ComicURLNotFoundException()
        {
        }

        /// <summary>
        /// Create a ComicURLNotFoundException with a custom message
        /// </summary>
        /// <param name="message">ComicURLNotFoundException message</param>
        public ComicURLNotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// Create a ComicURLNotFoundException with a custom message and a reference to the inner exception
        /// </summary>
        /// <param name="message">ComicURLNotFoundException message</param>
        /// <param name="innerException">Reference to the inner exception</param>
        public ComicURLNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
