using System;
using System.Net;

namespace UKDataViewer.Exceptions
{
    /// <summary>
    /// Exception thrown if a bad HTTP status is received when making
    /// REST API calls to some endpoint.
    /// </summary>
    class BadStatusException : Exception
    {   
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="status">Status code of the error.</param>
        /// <param name="endpoint">Address of the website the call was made to.</param>
        public BadStatusException(HttpStatusCode status, string endpoint)
            : base(string.Format("Bad status {0} received during connection to {1}.", (int)status, endpoint) )
        {
        }
    }
}
