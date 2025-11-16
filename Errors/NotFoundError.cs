namespace LightRateClient.Errors
{
    /// <summary>
    /// 404 Not Found errors.
    /// </summary>
    public class NotFoundError : APIError
    {
        public NotFoundError(string message, int? statusCode = null, string responseBody = null)
            : base(message, statusCode, responseBody)
        {
        }
    }
}
