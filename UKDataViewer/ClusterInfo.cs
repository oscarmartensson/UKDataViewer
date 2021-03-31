

namespace UKDataViewer
{
    /// <summary>
    /// Contains all information for a person to be displayed in a
    /// cluster ListView.
    /// </summary>
    class ClusterInfo
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="_name">First name of person.</param>
        /// <param name="_county">County of person's address.</param>
        /// <param name="_city">City in which the person lives in.</param>
        /// <param name="_longitude">Longitude coordinate of address.</param>
        /// <param name="_latitude">Latitude coordinate of address.</param>
        public ClusterInfo(string _name,
                           string _county,
                           string _city,
                           double _longitude,
                           double _latitude)
        {
            name = _name;
            county = _county;
            city = _city;
            longitude = _longitude;
            latitude = _latitude;
        }
        public string size { set; get; }
        public string name { set; get; }
        public string county { set; get; }
        public string city { set; get; }
        public double longitude { set; get; }
        public double latitude { set; get; }
    }
}
