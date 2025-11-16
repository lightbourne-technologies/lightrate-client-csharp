namespace LightRateClient.Errors
{
    /// <summary>
    /// 403 Forbidden errors.
    /// </summary>
    public class ForbiddenError : APIError
    {
        public ForbiddenError(string message, int? statusCode = null, string responseBody = null)
            : base(message, statusCode, responseBody)
        {
        }
    }
}
