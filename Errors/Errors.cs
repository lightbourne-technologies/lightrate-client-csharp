namespace LightRateClient.Errors
{
    /// <summary>
    /// Factory class for creating errors from HTTP responses.
    /// </summary>
    public static class Errors
    {
        /// <summary>
        /// Factory function to create appropriate error based on HTTP status code.
        /// </summary>
        public static APIError CreateErrorFromResponse(string message, int statusCode, string responseBody)
        {
            return statusCode switch
            {
                400 => new BadRequestError(message, statusCode, responseBody),
                401 => new UnauthorizedError(message, statusCode, responseBody),
                403 => new ForbiddenError(message, statusCode, responseBody),
                404 => new NotFoundError(message, statusCode, responseBody),
                422 => new UnprocessableEntityError(message, statusCode, responseBody),
                429 => new TooManyRequestsError(message, statusCode, responseBody),
                500 => new InternalServerError(message, statusCode, responseBody),
                503 => new ServiceUnavailableError(message, statusCode, responseBody),
                _ => new APIError(message, statusCode, responseBody)
            };
        }
    }
}
