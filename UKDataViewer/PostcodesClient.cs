﻿using System.Collections.Generic;
using RestSharp;

using UKDataViewer.Exceptions;


namespace UKDataViewer
{
    /// <summary>
    /// Client that interacts with the Postcodes.IO website,
    /// making REST API calls to it.
    /// </summary>
    class PostcodesClient
    {
        private string endpoint = "https://api.postcodes.io";

        private RestClient client;

        /// <summary>
        /// Constructor.
        /// </summary>
        public PostcodesClient()
        {
            client = new RestClient(endpoint);
        }

        /// <summary>
        /// Queries the endpoint for Longitude and Latitude for a given postcode.
        /// </summary>
        /// <param name="postcodes">List of postcodes.</param>
        /// <returns>List of result query from the endpoint.</returns>
        public List<BulkQueryResult<string, LongLat>> BulkPostcodeLookup(List<string> postcodes)
        {
            // We're only interested in the longitude and latitude for cluster computation.
            var request = new RestRequest("postcodes?filter=longitude,latitude", Method.POST)
            {
                RequestFormat = DataFormat.Json
            };
            request.AddJsonBody(new { postcodes });

            var response = client.Execute<PostcodeIOResponse<List<BulkQueryResult<string, LongLat>>>>(request);

            if (response.ErrorException != null)
            {
                throw new RESTException(response.ErrorException);
            }
            if (response.Data == null)
            {
                throw new BadStatusException(response.StatusCode, endpoint);
            }
            return response.Data.Result;
        }
    }
}
