using System.Collections.Generic;
using RestSharp;

using UKDataViewer.Exceptions;


namespace UKDataViewer
{
    class PostcodesClient
    {
        private string endpoint = "https://api.postcodes.io";

        private RestClient client;

        public PostcodesClient()
        {
            client = new RestClient(endpoint);
        }

        public List<BulkQueryResult<string, LongLat>> BulkPostcodeLookup(List<string> postcodes)
        {
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
