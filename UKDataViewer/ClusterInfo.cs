using System.Device.Location;

namespace UKDataViewer
{
    /// <summary>
    /// Contains all information for a person to be displayed in a
    /// cluster ListView. Also used to interface with the DBSCAN algorithm.
    /// </summary>
    public class ClusterInfo : DBSCAN.IPointData
    {

        private readonly DBSCAN.Point internalPoint;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="_name">First name of person.</param>
        /// <param name="_county">County of person's address.</param>
        /// <param name="_city">City in which the person lives in.</param>
        /// <param name="_longitude">Longitude coordinate of address.</param>
        /// <param name="_latitude">Latitude coordinate of address.</param>
        /// 
        public ClusterInfo(string _name,
                           string _county,
                           string _city,
                           double _longitude=0,
                           double _latitude=0)
        {
            name = _name;
            county = _county;
            city = _city;
            longitude = _longitude;
            latitude = _latitude;

            // Just used for interfacing with DBSCAN.
            internalPoint = new DBSCAN.Point(longitude, latitude);
        }
        public string size { set; get; }
        public string name { set; get; }
        public string county { set; get; }
        public string city { set; get; }
        public double longitude { set; get; }
        public double latitude { set; get; }

        public ref readonly DBSCAN.Point Point => ref internalPoint;

        /// <summary>
        /// Function to be used when calculating the distance
        /// between two points in the DBSCAN algorithm.
        /// </summary>
        /// <param name="a">First point.</param>
        /// <param name="b">Second point.</param>
        /// <returns>Distance between two points in meters.</returns>
        public static double DistanceFunction(in DBSCAN.Point a, in DBSCAN.Point b)
        {
            // Y is the latitude and X is the longitude.
            GeoCoordinate coord1 = new GeoCoordinate(a.Y, a.X);
            return coord1.GetDistanceTo(new GeoCoordinate(b.Y, b.X));
        }
    }
}
