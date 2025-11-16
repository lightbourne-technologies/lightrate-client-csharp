namespace LightRateClient.Errors
{
    /// <summary>
    /// 401 Unauthorized errors.
    /// </summary>
    public class UnauthorizedError : APIError
    {
        public UnauthorizedError(string message, int? statusCode = null, string responseBody = null)
            : base(message, statusCode, responseBody)
        {
        }
    }
}
