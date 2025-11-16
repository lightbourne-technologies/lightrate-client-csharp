namespace LightRateClient.Errors
{
    /// <summary>
    /// 503 Service Unavailable errors.
    /// </summary>
    public class ServiceUnavailableError : APIError
    {
        public ServiceUnavailableError(string message, int? statusCode = null, string responseBody = null)
            : base(message, statusCode, responseBody)
        {
        }
    }
}
