using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ProfileMgrApp.Helpers
{
    public class FaceRectangle
    {
        public int top { get; set; }
        public int left { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class FaceInfo
    {
        public string faceId { get; set; }
        public FaceRectangle faceRectangle { get; set; }
    }

    /// <summary>
    /// Helper class to simpilify Face Cognitive REST API calls
    /// </summary>
    public class FaceApiHelper
    {
        private HttpClient _client;
        private ILogger<FaceApiHelper> _logger;
        private readonly string _baseUrl;
        private readonly string _apiKey;

        public FaceApiHelper(HttpClient client, ILogger<FaceApiHelper> logger, IConfiguration config)
        {
            _client = client;
            _logger = logger;
            _baseUrl = config["FaceApi:BaseUrl"];
            _apiKey = config["FaceApi:ApiKey"];
        }

        public async Task<List<FaceInfo>> Detect(byte[] image)
        {
            List<FaceInfo> result = new List<FaceInfo>();

            if (image != null)
            {
                using (ByteArrayContent content = new ByteArrayContent(image))
                {
                    // use content type "application/octet-stream" for binary image data
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    // Request headers
                    _client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _apiKey);

                    // Assemble the URI for the REST API Call
                    string url = _baseUrl + "/detect?returnFaceId=true&returnFaceLandmarks=false";

                    // Execute the REST API call
                    HttpResponseMessage response = await _client.PostAsync(url, content);

                    // Get the JSON response
                    string contentString = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<List<FaceInfo>>(contentString);
                }
            }

            return result;
        }
    }
}
