using Newtonsoft.Json;

namespace LightRateClient.Models
{
    /// <summary>
    /// Rate limiting rule.
    /// </summary>
    public class Rule
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("refillRate")]
        public int RefillRate { get; set; }

        [JsonProperty("burstRate")]
        public int BurstRate { get; set; }

        [JsonProperty("isDefault")]
        public bool IsDefault { get; set; }

        [JsonProperty("matcher")]
        public string Matcher { get; set; }

        [JsonProperty("httpMethod")]
        public string HttpMethod { get; set; }
    }
}
