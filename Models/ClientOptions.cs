namespace LightRateClient.Models
{
    /// <summary>
    /// Options for creating a client.
    /// </summary>
    public class ClientOptions
    {
        public int? Timeout { get; set; }
        public int? RetryAttempts { get; set; }
        public int? DefaultLocalBucketSize { get; set; }
    }
}
