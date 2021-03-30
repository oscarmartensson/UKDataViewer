using System.Device.Location;
using DBSCAN;

namespace UKDataViewer
{
    public class Location : DBSCAN.IPointData
    {
        private readonly Point internalPoint;

        public Location(double longitude, double latitude)
        {
            internalPoint = new DBSCAN.Point(longitude, latitude);
        }
        public ref readonly DBSCAN.Point Point => ref internalPoint;

        public static double DistanceFunction(in Point a, in Point b)
        {
            GeoCoordinate coord1 = new GeoCoordinate(a.Y, a.X);
            return coord1.GetDistanceTo( new GeoCoordinate(b.Y, b.X) );
        }
    }
}
