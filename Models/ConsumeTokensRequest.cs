using Newtonsoft.Json;

namespace LightRateClient.Models
{
    /// <summary>
    /// Request object for consuming tokens.
    /// </summary>
    public class ConsumeTokensRequest
    {
        [JsonProperty("operation")]
        public string Operation { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("httpMethod")]
        public string HttpMethod { get; set; }

        [JsonProperty("userIdentifier")]
        public string UserIdentifier { get; set; }

        [JsonProperty("tokensRequested")]
        public int? TokensRequested { get; set; }

        [JsonProperty("tokensRequestedForDefaultBucketMatch")]
        public int? TokensRequestedForDefaultBucketMatch { get; set; }

        [JsonProperty("applicationId")]
        public string ApplicationId { get; set; }
    }
}
