namespace LightRateClient.Models
{
    /// <summary>
    /// Response from local bucket token consumption.
    /// </summary>
    public class ConsumeLocalBucketTokenResponse
    {
        public bool Success { get; set; }
        public bool UsedLocalToken { get; set; }
        public TokenBucketStatus BucketStatus { get; set; }
    }
}
