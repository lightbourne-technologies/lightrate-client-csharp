namespace LightRateClient.Errors
{
    /// <summary>
    /// 422 Unprocessable Entity errors.
    /// </summary>
    public class UnprocessableEntityError : APIError
    {
        public UnprocessableEntityError(string message, int? statusCode = null, string responseBody = null)
            : base(message, statusCode, responseBody)
        {
        }
    }
}
