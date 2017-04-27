using System;

namespace CAndHDL.Exceptions
{
    /// <summary>
    /// Custom exception class used when the comic name is not found in a comic relative URL.
    /// </summary>
    class ComicNameNotFoundException : Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ComicNameNotFoundException()
        {
        }

        /// <summary>
        /// Create a ComicNameNotFoundException with a custom message
        /// </summary>
        /// <param name="message">ComicNameNotFoundException message</param>
        public ComicNameNotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// Create a ComicNameNotFoundException with a custom message and a reference to the inner exception
        /// </summary>
        /// <param name="message">ComicNameNotFoundException message</param>
        /// <param name="innerException">Reference to the inner exception</param>
        public ComicNameNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
