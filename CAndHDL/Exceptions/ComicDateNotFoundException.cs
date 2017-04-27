using System;

namespace CAndHDL.Exceptions
{
    /// <summary>
    /// Custom exception class used when a date is not found on a comic page.
    /// </summary>
    class ComicDateNotFoundException : Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ComicDateNotFoundException()
        {
        }

        /// <summary>
        /// Create a ComicDateNotFoundException with a custom message
        /// </summary>
        /// <param name="message">ComicDateNotFoundException message</param>
        public ComicDateNotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// Create a ComicDateNotFoundException with a custom message and a reference to the inner exception
        /// </summary>
        /// <param name="message">ComicDateNotFoundException message</param>
        /// <param name="innerException">Reference to the inner exception</param>
        public ComicDateNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
