
namespace UKDataViewer
{
     /// <summary>
     /// Query and results from bulk REST API request.
     /// </summary>
     /// <typeparam name="TQuery">Type of the query.</typeparam>
     /// <typeparam name="TResult">Result of the result.</typeparam>
    public class BulkQueryResult<TQuery, TResult> where TResult : class
    {
        public TQuery Query { get; set; }
        public TResult Result { get; set; }
    }

    /// <summary>
    /// Response from Postcode.IO.
    /// </summary>
    /// <typeparam name="T">Type of the result.</typeparam>
    public class PostcodeIOResponse<T>
    {
        public int Status { get; set; }
        public T Result { get; set; }
    }

    /// <summary>
    /// Utility class holding a longitude and latitude coordinate.
    /// </summary>
    public class LongLat
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
