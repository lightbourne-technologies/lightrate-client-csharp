namespace LightRateClient.Errors
{
    /// <summary>
    /// 500 Internal Server Error errors.
    /// </summary>
    public class InternalServerError : APIError
    {
        public InternalServerError(string message, int? statusCode = null, string responseBody = null)
            : base(message, statusCode, responseBody)
        {
        }
    }
}
