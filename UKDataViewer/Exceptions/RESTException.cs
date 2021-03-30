using System;

namespace UKDataViewer.Exceptions
{
    /// <summary>
    /// Exception thrown when something went wrong with making
    /// REST query.
    /// </summary>
    class RESTException : Exception
    {
        /// <summary>
        /// Adds an additional message to the inner exception.
        /// </summary>
        /// <param name="innerException">The inner exception that threw the exception.</param>
        public RESTException(Exception innerException)
            : base("Exception thrown during REST query. See nestled exception for more info.", innerException)
        {
        }
    }
}
