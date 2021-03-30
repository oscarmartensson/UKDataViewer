using System;

namespace UKDataViewer.Exceptions
{
    class RESTException : Exception
    {
        public RESTException(Exception innerException)
            : base("Exception thrown during REST query. See nestled exception for more info.", innerException)
        {
        }
    }
}
