namespace LightRateClient.Errors
{
    /// <summary>
    /// Base API error class.
    /// </summary>
    public class APIError : LightRateError
    {
        public int? StatusCode { get; }
        public string ResponseBody { get; }

        public APIError(string message, int? statusCode = null, string responseBody = null)
            : base(message)
        {
            StatusCode = statusCode;
            ResponseBody = responseBody;
        }
    }
}
