using Microsoft.Extensions.Configuration;

namespace MantisCode.RestClient
{
    public class RestClientOptions
    {
        public string Address { get; set; } = null!;
        public Dictionary<string, string>? Headers { get; set; }
        public int? GlobalTimeoutInSeconds { get; set; }
        public int? TryTimeoutInSeconds { get; set; }
        public bool EnableRetries { get; set; }
        public int[]? RetriesInMilliseconds { get; set; }
        public int? RateLimit { get; set; }
        public int? RateLimitWindowInSeconds { get; set; }
        public bool EnableRateLimiting => RateLimit.HasValue && RateLimitWindowInSeconds.HasValue;

        internal class RestClientOptionsBuilder
        {
            private readonly RestClientOptions restClientOptions;

            public RestClientOptionsBuilder(IConfigurationSection configurationSection)
            {
                restClientOptions = new RestClientOptions();
                configurationSection.Bind(restClientOptions);
            }

            internal RestClientOptions Build()
            {
                restClientOptions.SetDefaultValues();
                restClientOptions.Validate();
                return restClientOptions;
            }
        }

        private void Validate()
        {
            if (!Uri.TryCreate(Address, UriKind.Absolute, out _))
            {
                throw new Exception($"invalid {Address}");
            }

            if (EnableRetries && GlobalTimeoutInSeconds < (TryTimeoutInSeconds * (RetriesInMilliseconds!.Length + 1) + RetriesInMilliseconds.Sum() / 1000d))
            {
                throw new Exception($"{nameof(GlobalTimeoutInSeconds)} should exceed total retry durations");
            }

            if (RateLimit.HasValue ^ RateLimitWindowInSeconds.HasValue)
            {
                throw new Exception($"Both {nameof(RateLimitWindowInSeconds)} should be provided");
            }
        }

        private void SetDefaultValues()
        {
            if (RetriesInMilliseconds is null ||
                RetriesInMilliseconds.Length == 0) RetriesInMilliseconds = [500];

            TryTimeoutInSeconds ??= 30;
            GlobalTimeoutInSeconds ??= 100;
        }
    }
}
