namespace LightRateClient.Errors
{
    /// <summary>
    /// Network-related errors.
    /// </summary>
    public class NetworkError : LightRateError
    {
        public NetworkError(string message) : base(message)
        {
        }

        public NetworkError(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
