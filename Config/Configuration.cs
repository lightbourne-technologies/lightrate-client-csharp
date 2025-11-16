namespace LightRateClient.Config
{
    /// <summary>
    /// Configuration class for the LightRate client.
    /// </summary>
    public class Configuration
    {
        public string ApiKey { get; set; }
        public string ApplicationId { get; set; }
        public int Timeout { get; set; } = 30;
        public int RetryAttempts { get; set; } = 3;
        public int DefaultLocalBucketSize { get; set; } = 5;

        /// <summary>
        /// Check if the configuration is valid.
        /// </summary>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(ApiKey) && !string.IsNullOrEmpty(ApplicationId);
        }
    }
}
