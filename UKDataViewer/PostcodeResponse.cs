using System;

namespace UKDataViewer
{
    [Serializable]
    public class BulkQueryResult<TQuery, TResult> where TResult : class
    {
        public TQuery Query { get; set; }
        public TResult Result { get; set; }
    }

    [Serializable]
    public class PostcodeIOResponse<T>
    {
        public int Status { get; set; }
        public T Result { get; set; }
    }

    [Serializable]
    public class LongLat
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
