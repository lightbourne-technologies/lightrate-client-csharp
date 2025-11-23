using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using LightRateClient.Config;
using LightRateClient.Errors;
using LightRateClient.Models;

namespace LightRateClient.Client
{
    /// <summary>
    /// Main client class for interacting with the LightRate API.
    /// </summary>
    public class LightRateClient
    {
        private const string Version = "1.0.0";
        private const string BaseUrl = "https://api.lightrate.lightbournetechnologies.ca";

        private readonly Configuration _configuration;
        private readonly HttpClient _httpClient;
        private readonly Dictionary<string, TokenBucket> _tokenBuckets;
        private readonly object _bucketsLock = new object();

        /// <summary>
        /// Initialize the client.
        /// </summary>
        /// <param name="apiKey">API key for authentication</param>
        /// <param name="applicationId">Application ID</param>
        /// <param name="options">Optional client options</param>
        public LightRateClient(string apiKey, string applicationId, ClientOptions options = null)
        {
            if (!string.IsNullOrEmpty(apiKey) && !string.IsNullOrEmpty(applicationId))
            {
                _configuration = new Configuration
                {
                    ApiKey = apiKey,
                    ApplicationId = applicationId,
                    Timeout = options?.Timeout ?? 30,
                    RetryAttempts = options?.RetryAttempts ?? 3,
                    DefaultLocalBucketSize = options?.DefaultLocalBucketSize ?? 5
                };
            }
            else
            {
                _configuration = new Configuration();
            }

            _tokenBuckets = new Dictionary<string, TokenBucket>();
            ValidateConfiguration();
            _httpClient = SetupHttpClient();
        }

        /// <summary>
        /// Consume tokens by operation or path using local bucket.
        /// </summary>
        public ConsumeLocalBucketTokenResponse ConsumeLocalBucketToken(
            string userIdentifier,
            string operation = null,
            string path = null,
            string httpMethod = null)
        {
            // First, try to find an existing bucket that matches this request
            TokenBucket bucket = FindBucketByMatcher(userIdentifier, operation, path, httpMethod);

            if (bucket != null)
            {
                bool consumed = bucket.CheckAndConsumeToken();
                if (consumed)
                {
                    return new ConsumeLocalBucketTokenResponse
                    {
                        Success = true,
                        UsedLocalToken = true,
                        BucketStatus = bucket.GetStatus()
                    };
                }
            }

            // Still empty, make API call
            int tokensToFetch = _configuration.DefaultLocalBucketSize;
            var request = new ConsumeTokensRequest
            {
                Operation = operation,
                Path = path,
                HttpMethod = httpMethod,
                UserIdentifier = userIdentifier,
                TokensRequested = tokensToFetch,
                TokensRequestedForDefaultBucketMatch = 1,
                ApplicationId = _configuration.ApplicationId
            };

            ConsumeTokensResponse response = ConsumeTokensWithRequest(request);

            if (response.Rule != null && response.Rule.IsDefault)
            {
                return new ConsumeLocalBucketTokenResponse
                {
                    Success = response.TokensConsumed > 0,
                    UsedLocalToken = false,
                    BucketStatus = null
                };
            }

            if (response.Rule != null)
            {
                TokenBucket newBucket = FillBucketAndCreateIfNotExists(
                    userIdentifier,
                    response.Rule,
                    response.TokensConsumed
                );

                bool newBucketTokensAvailable = newBucket.CheckAndConsumeToken();

                return new ConsumeLocalBucketTokenResponse
                {
                    Success = newBucketTokensAvailable,
                    UsedLocalToken = false,
                    BucketStatus = newBucket.GetStatus()
                };
            }

            return new ConsumeLocalBucketTokenResponse
            {
                Success = false,
                UsedLocalToken = false,
                BucketStatus = null
            };
        }

        /// <summary>
        /// Consume tokens directly from API.
        /// </summary>
        public ConsumeTokensResponse ConsumeTokens(
            string userIdentifier,
            int tokensRequested,
            string operation = null,
            string path = null,
            string httpMethod = null)
        {
            var request = new ConsumeTokensRequest
            {
                Operation = operation,
                Path = path,
                HttpMethod = httpMethod,
                UserIdentifier = userIdentifier,
                TokensRequested = tokensRequested,
                ApplicationId = _configuration.ApplicationId
            };

            return ConsumeTokensWithRequest(request);
        }

        /// <summary>
        /// Consume tokens using a request object.
        /// </summary>
        public ConsumeTokensResponse ConsumeTokensWithRequest(ConsumeTokensRequest request)
        {
            if (!IsValidConsumeTokensRequest(request))
            {
                throw new ArgumentException("Invalid request: validation failed");
            }

            string responseBody = Post("/api/v1/tokens/consume", JsonConvert.SerializeObject(request));
            return ParseConsumeTokensResponse(responseBody);
        }

        /// <summary>
        /// Get all bucket statuses.
        /// </summary>
        public Dictionary<string, TokenBucketStatus> GetAllBucketStatuses()
        {
            var statuses = new Dictionary<string, TokenBucketStatus>();
            lock (_bucketsLock)
            {
                foreach (var kvp in _tokenBuckets)
                {
                    statuses[kvp.Key] = kvp.Value.GetStatus();
                }
            }
            return statuses;
        }

        /// <summary>
        /// Reset all token buckets.
        /// </summary>
        public void ResetAllBuckets()
        {
            lock (_bucketsLock)
            {
                _tokenBuckets.Clear();
            }
        }

        /// <summary>
        /// Get configuration.
        /// </summary>
        public Configuration GetConfiguration()
        {
            return _configuration;
        }

        private TokenBucket FindBucketByMatcher(
            string userIdentifier,
            string operation,
            string path,
            string httpMethod)
        {
            lock (_bucketsLock)
            {
                foreach (var bucket in _tokenBuckets.Values)
                {
                    if (bucket.Matches(operation, path, httpMethod) &&
                        bucket.UserIdentifier == userIdentifier)
                    {
                        return bucket;
                    }
                }
            }
            return null;
        }

        private TokenBucket FillBucketAndCreateIfNotExists(
            string userIdentifier,
            Rule rule,
            int tokenCount)
        {
            string bucketKey = $"{userIdentifier}:rule:{rule.Id}";

            lock (_bucketsLock)
            {
                if (!_tokenBuckets.TryGetValue(bucketKey, out TokenBucket bucket) ||
                    bucket.Expired())
                {
                    bucket = new TokenBucket(
                        _configuration.DefaultLocalBucketSize,
                        rule.Id,
                        userIdentifier,
                        rule.Matcher,
                        rule.HttpMethod
                    );
                    _tokenBuckets[bucketKey] = bucket;
                }

                bucket.Refill(tokenCount);
                return bucket;
            }
        }

        private void ValidateConfiguration()
        {
            if (!_configuration.IsValid())
            {
                throw new ConfigurationError("API key and application ID are required");
            }
        }

        private HttpClient SetupHttpClient()
        {
            var client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(_configuration.Timeout)
            };
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_configuration.ApiKey}");
            client.DefaultRequestHeaders.Add("User-Agent", $"lightrate-client-csharp/{Version}");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            if (!string.IsNullOrEmpty(_configuration.ApplicationId))
            {
                client.DefaultRequestHeaders.Add("X-Application-Id", _configuration.ApplicationId);
            }
            return client;
        }

        private string Post(string path, string jsonBody)
        {
            string url = BaseUrl + path;
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = _httpClient.PostAsync(url, content).Result;
                return HandleResponse(response);
            }
            catch (TaskCanceledException)
            {
                throw new TimeoutError($"Request timed out after {_configuration.Timeout} seconds");
            }
            catch (HttpRequestException e)
            {
                throw new NetworkError($"Network error: {e.Message}");
            }
        }

        private string HandleResponse(HttpResponseMessage response)
        {
            int statusCode = (int)response.StatusCode;
            string responseBody = response.Content.ReadAsStringAsync().Result;

            if (statusCode >= 200 && statusCode < 300)
            {
                return responseBody;
            }

            string errorMessage = "Unknown error";
            try
            {
                var errorData = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseBody);
                if (errorData != null)
                {
                    if (errorData.ContainsKey("message"))
                    {
                        errorMessage = errorData["message"].ToString();
                    }
                    else if (errorData.ContainsKey("error"))
                    {
                        errorMessage = errorData["error"].ToString();
                    }
                    else
                    {
                        errorMessage = $"HTTP {statusCode} Error";
                    }
                }
            }
            catch
            {
                errorMessage = $"HTTP {statusCode} Error";
            }

            throw global::LightRateClient.Errors.Errors.CreateErrorFromResponse(errorMessage, statusCode, responseBody);
        }

        private bool IsValidConsumeTokensRequest(ConsumeTokensRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserIdentifier))
            {
                return false;
            }
            if (request.TokensRequested == null || request.TokensRequested <= 0)
            {
                return false;
            }
            if (string.IsNullOrEmpty(request.Operation) && string.IsNullOrEmpty(request.Path))
            {
                return false;
            }
            if (!string.IsNullOrEmpty(request.Operation) && !string.IsNullOrEmpty(request.Path))
            {
                return false;
            }
            if (!string.IsNullOrEmpty(request.Path) && string.IsNullOrEmpty(request.HttpMethod))
            {
                return false;
            }
            return true;
        }

        private ConsumeTokensResponse ParseConsumeTokensResponse(string json)
        {
            return JsonConvert.DeserializeObject<ConsumeTokensResponse>(json);
        }
    }
}
