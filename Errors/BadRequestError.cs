namespace LightRateClient.Errors
{
    /// <summary>
    /// 400 Bad Request errors.
    /// </summary>
    public class BadRequestError : APIError
    {
        public BadRequestError(string message, int? statusCode = null, string responseBody = null)
            : base(message, statusCode, responseBody)
        {
        }
    }
}
