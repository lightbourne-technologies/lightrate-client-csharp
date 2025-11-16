namespace LightRateClient.Models
{
    /// <summary>
    /// Status of a token bucket.
    /// </summary>
    public class TokenBucketStatus
    {
        public int TokensRemaining { get; set; }
        public int MaxTokens { get; set; }
    }
}
