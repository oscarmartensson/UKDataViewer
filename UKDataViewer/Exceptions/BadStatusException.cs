using System;
using System.Net;

namespace UKDataViewer.Exceptions
{
    class BadStatusException : Exception
    {
        public BadStatusException(HttpStatusCode status, string url)
            : base(string.Format("Bad status {0} received during connection to {1}.", (int)status, url) )
        {
        }
    }
}
