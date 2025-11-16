# LightRate Client C#

A C# client for the Lightrate token management API, providing easy-to-use methods for consuming tokens with local bucket management.

## Installation

Install the package via NuGet:

```bash
Install-Package LightRateClient
```

Or via .NET CLI:

```bash
dotnet add package LightRateClient
```

Or add to your `.csproj`:

```xml
<PackageReference Include="LightRateClient" Version="1.0.0" />
```

## Usage

### Basic Usage

```csharp
using LightRateClient.Client;
using LightRateClient.Models;

// Simple usage - pass your API key and application ID
var client = new LightRateClient("your_api_key", "your_application_id");

// With additional options
var options = new ClientOptions
{
    Timeout = 60,
    DefaultLocalBucketSize = 10
};
var client = new LightRateClient("your_api_key", "your_application_id", options);
```

### Consuming Tokens

```csharp
// Consume tokens by operation
var response = client.ConsumeTokens(
    userIdentifier: "user123",
    tokensRequested: 1,
    operation: "send_email"
);

// Or consume tokens by path
var response = client.ConsumeTokens(
    userIdentifier: "user123",
    tokensRequested: 1,
    path: "/api/v1/emails/send",
    httpMethod: "POST"
);

if (response.TokensConsumed > 0)
{
    Console.WriteLine($"Tokens consumed successfully. Remaining: {response.TokensRemaining}");
}
else
{
    Console.WriteLine("Failed to consume tokens");
}
```

#### Using Local Token Buckets

The client supports local token buckets for improved performance. Buckets are automatically created based on the rules returned by the API, and are matched against incoming requests using the `matcher` field from the rule.

```csharp
// Configure client with default bucket size
var options = new ClientOptions
{
    DefaultLocalBucketSize = 20  // All operations use this bucket size
};
var client = new LightRateClient("your_api_key", "your_application_id", options);

// Consume tokens using local bucket (more efficient)
var result = client.ConsumeLocalBucketToken(
    userIdentifier: "user123",
    operation: "send_email"
);

Console.WriteLine($"Success: {result.Success}");
Console.WriteLine($"Used local token: {result.UsedLocalToken}");
Console.WriteLine($"Bucket status: {result.BucketStatus}");
```

**Bucket Matching:**
- Buckets are matched using the `matcher` field from the rule, which supports regex patterns
- Each user has separate buckets per rule, ensuring proper isolation
- Buckets expire after 60 seconds of inactivity
- Default rules (isDefault: true) do not create local buckets

### Complete Example

```csharp
using LightRateClient.Client;
using LightRateClient.Errors;
using LightRateClient.Models;

// Create a client with your API key and application ID
var client = new LightRateClient("your_api_key", "your_application_id");

try
{
    // Consume tokens
    var consumeResponse = client.ConsumeTokens(
        userIdentifier: "user123",
        tokensRequested: 1,
        operation: "send_email"
    );

    if (consumeResponse.TokensConsumed > 0)
    {
        Console.WriteLine($"Successfully consumed tokens. Remaining: {consumeResponse.TokensRemaining}");
        // Proceed with your operation
    }
    else
    {
        Console.WriteLine("Failed to consume tokens");
        // Handle rate limiting
    }
}
catch (UnauthorizedError e)
{
    Console.WriteLine($"Authentication failed: {e.Message}");
}
catch (TooManyRequestsError e)
{
    Console.WriteLine($"Rate limited: {e.Message}");
}
catch (APIError e)
{
    Console.WriteLine($"API Error ({e.StatusCode}): {e.Message}");
}
catch (NetworkError e)
{
    Console.WriteLine($"Network error: {e.Message}");
}
```

## Error Handling

The client provides comprehensive error handling with specific exception types:

```csharp
try
{
    var response = client.ConsumeTokens(...);
}
catch (UnauthorizedError e)
{
    Console.WriteLine($"Authentication failed: {e.Message}");
}
catch (NotFoundError e)
{
    Console.WriteLine($"Resource not found: {e.Message}");
}
catch (APIError e)
{
    Console.WriteLine($"API Error ({e.StatusCode}): {e.Message}");
}
catch (NetworkError e)
{
    Console.WriteLine($"Network error: {e.Message}");
}
catch (TimeoutError e)
{
    Console.WriteLine($"Request timed out: {e.Message}");
}
```

Available error types:
- `LightRateError` - Base error class
- `ConfigurationError` - Configuration-related errors
- `APIError` - Base API error class
- `BadRequestError` - 400 errors
- `UnauthorizedError` - 401 errors
- `ForbiddenError` - 403 errors
- `NotFoundError` - 404 errors
- `UnprocessableEntityError` - 422 errors
- `TooManyRequestsError` - 429 errors
- `InternalServerError` - 500 errors
- `ServiceUnavailableError` - 503 errors
- `NetworkError` - Network-related errors
- `TimeoutError` - Request timeout errors

## API Reference

### Classes

#### `LightRateClient`

Main client class for interacting with the LightRate API.

**Constructor:**
```csharp
LightRateClient(string apiKey, string applicationId)
LightRateClient(string apiKey, string applicationId, ClientOptions options)
```

**Methods:**

- `ConsumeTokens(userIdentifier, tokensRequested, operation, path, httpMethod) -> ConsumeTokensResponse`
- `ConsumeLocalBucketToken(userIdentifier, operation, path, httpMethod) -> ConsumeLocalBucketTokenResponse`
- `ConsumeTokensWithRequest(request) -> ConsumeTokensResponse`
- `GetAllBucketStatuses() -> Dictionary<string, TokenBucketStatus>`
- `ResetAllBuckets() -> void`
- `GetConfiguration() -> Configuration`

## Development

After checking out the repo, run `dotnet build` to build the project.

## Contributing

Bug reports and pull requests are welcome on GitHub at https://github.com/lightbourne-technologies/lightrate-client-csharp. This project is intended to be a safe, welcoming space for collaboration, and contributors are expected to adhere to the code of conduct.

## License

The package is available as open source under the terms of the [MIT License](https://opensource.org/licenses/MIT).
