namespace LightRateClient.Errors
{
    /// <summary>
    /// 429 Too Many Requests errors.
    /// </summary>
    public class TooManyRequestsError : APIError
    {
        public TooManyRequestsError(string message, int? statusCode = null, string responseBody = null)
            : base(message, statusCode, responseBody)
        {
        }
    }
}
