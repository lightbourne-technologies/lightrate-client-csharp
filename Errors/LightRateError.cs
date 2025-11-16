using System;

namespace LightRateClient.Errors
{
    /// <summary>
    /// Base error class for all LightRate errors.
    /// </summary>
    public class LightRateError : Exception
    {
        public LightRateError(string message) : base(message)
        {
        }

        public LightRateError(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
