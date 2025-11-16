namespace LightRateClient.Errors
{
    /// <summary>
    /// Configuration-related errors.
    /// </summary>
    public class ConfigurationError : LightRateError
    {
        public ConfigurationError(string message) : base(message)
        {
        }
    }
}
