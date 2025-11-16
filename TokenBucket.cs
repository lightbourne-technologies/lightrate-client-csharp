using System;
using System.Text.RegularExpressions;
using System.Threading;
using LightRateClient.Models;

namespace LightRateClient
{
    /// <summary>
    /// Token bucket for local token management.
    /// </summary>
    public class TokenBucket
    {
        private int _availableTokens;
        private readonly int _maxTokens;
        private readonly string _ruleId;
        private readonly string _matcher;
        private readonly string _httpMethod;
        private readonly string _userIdentifier;
        private DateTime _lastAccessedAt;
        private readonly object _lock = new object();

        public TokenBucket(
            int maxTokens,
            string ruleId,
            string userIdentifier,
            string matcher,
            string httpMethod)
        {
            _maxTokens = maxTokens;
            _availableTokens = 0;
            _ruleId = ruleId;
            _matcher = matcher;
            _httpMethod = httpMethod;
            _userIdentifier = userIdentifier;
            _lastAccessedAt = DateTime.Now;
        }

        public bool HasTokens()
        {
            return _availableTokens > 0;
        }

        public bool ConsumeToken()
        {
            if (_availableTokens <= 0)
            {
                return false;
            }
            _availableTokens -= 1;
            return true;
        }

        public int ConsumeTokens(int count)
        {
            if (count <= 0 || _availableTokens <= 0)
            {
                return 0;
            }
            int tokensToConsume = Math.Min(count, _availableTokens);
            _availableTokens -= tokensToConsume;
            return tokensToConsume;
        }

        public int Refill(int tokensToFetch)
        {
            Touch();
            int tokensToAdd = Math.Min(tokensToFetch, _maxTokens - _availableTokens);
            _availableTokens += tokensToAdd;
            return tokensToAdd;
        }

        public TokenBucketStatus GetStatus()
        {
            return new TokenBucketStatus
            {
                TokensRemaining = _availableTokens,
                MaxTokens = _maxTokens
            };
        }

        public void Reset()
        {
            _availableTokens = 0;
        }

        public bool Matches(string operation, string path, string httpMethod)
        {
            if (Expired())
            {
                return false;
            }

            if (string.IsNullOrEmpty(_matcher))
            {
                return false;
            }

            try
            {
                Regex matcherRegex = new Regex(_matcher);

                // For operation-based requests, match against operation
                if (!string.IsNullOrEmpty(operation))
                {
                    return matcherRegex.IsMatch(operation) && string.IsNullOrEmpty(_httpMethod);
                }

                // For path-based requests, match against path and HTTP method
                if (!string.IsNullOrEmpty(path))
                {
                    return matcherRegex.IsMatch(path) &&
                           (string.IsNullOrEmpty(_httpMethod) || _httpMethod == httpMethod);
                }

                return false;
            }
            catch (ArgumentException)
            {
                // If matcher is not a valid regex, fall back to exact match
                if (!string.IsNullOrEmpty(operation))
                {
                    return _matcher == operation && string.IsNullOrEmpty(_httpMethod);
                }
                else if (!string.IsNullOrEmpty(path))
                {
                    return _matcher == path &&
                           (string.IsNullOrEmpty(_httpMethod) || _httpMethod == httpMethod);
                }
                return false;
            }
        }

        public bool Expired()
        {
            DateTime now = DateTime.Now;
            TimeSpan diff = now - _lastAccessedAt;
            return diff.TotalSeconds > 60; // 60 seconds
        }

        public void Touch()
        {
            _lastAccessedAt = DateTime.Now;
        }

        public bool CheckAndConsumeToken()
        {
            lock (_lock)
            {
                Touch();
                if (_availableTokens > 0)
                {
                    _availableTokens -= 1;
                    return true;
                }
                return false;
            }
        }

        public string UserIdentifier => _userIdentifier;
    }
}
