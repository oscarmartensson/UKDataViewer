using System.Device.Location;
using DBSCAN;

namespace UKDataViewer
{
    /// <summary>
    /// Utility class representing a longitude and latitude location.
    /// Used to interface with the DBSCAN algorithm.
    /// </summary>
    public class Location : DBSCAN.IPointData
    {
        private readonly Point internalPoint;

        /// <summary>
        /// Wraps a longitude and latitude coordinate to interface with
        /// the DBSCAN algorithm.
        /// </summary>
        /// <param name="longitude">Angular positional coordinate.</param>
        /// <param name="latitude">Angular positional coordinate.</param>
        public Location(double longitude, double latitude)
        {
            internalPoint = new DBSCAN.Point(longitude, latitude);
        }
        public ref readonly DBSCAN.Point Point => ref internalPoint;

        /// <summary>
        /// Function to be used when calculating the distance
        /// between two points in the DBSCAN algorithm.
        /// </summary>
        /// <param name="a">First point.</param>
        /// <param name="b">Second point.</param>
        /// <returns>Distance between two points in meters.</returns>
        public static double DistanceFunction(in Point a, in Point b)
        {
            // Y is the latitude and X is the longitude.
            GeoCoordinate coord1 = new GeoCoordinate(a.Y, a.X);
            return coord1.GetDistanceTo( new GeoCoordinate(b.Y, b.X) );
        }
    }
}
