namespace LightRateClient.Errors
{
    /// <summary>
    /// Request timeout errors.
    /// </summary>
    public class TimeoutError : LightRateError
    {
        public TimeoutError(string message) : base(message)
        {
        }
    }
}
