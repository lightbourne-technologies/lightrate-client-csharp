using Newtonsoft.Json;

namespace LightRateClient.Models
{
    /// <summary>
    /// Response from token consumption API call.
    /// </summary>
    public class ConsumeTokensResponse
    {
        [JsonProperty("tokensRemaining")]
        public int TokensRemaining { get; set; }

        [JsonProperty("tokensConsumed")]
        public int TokensConsumed { get; set; }

        [JsonProperty("throttles")]
        public int Throttles { get; set; }

        [JsonProperty("rule")]
        public Rule Rule { get; set; }
    }
}
