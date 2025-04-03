using Polly;
using Polly.Extensions.Http;
using Polly.RateLimit;
using Polly.Timeout;
using Serilog;

namespace MantisCode.RestClient
{
    public static class PolicyProvider
    {
        public static IAsyncPolicy<HttpResponseMessage> GetBasePolicy(RestClientOptions restClientOptions, HttpRequestMessage request)
        {
            string headerValue = string.Empty;

            if (request.Headers.TryGetValues("RestClient-Policy", out IEnumerable<string>? values))
            {
                headerValue = values.First();
            }

            bool enableRetries = headerValue switch
            {
                "Retry" => true,
                "NoOp" => false,
                _ => restClientOptions.EnableRetries
            };

            if (enableRetries)
                return GetRetryPolicy(restClientOptions, request);
            else
                return Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>();
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(RestClientOptions restClientOptions, HttpRequestMessage request)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .Or<TimeoutRejectedException>()
                .Or<RateLimitRejectedException>()
                .WaitAndRetryAsync(
                restClientOptions.RetriesInMilliseconds!.Select(milliseconds => TimeSpan.FromMicroseconds(milliseconds)),
                (response, timeSpan, retryAttempt, Context) =>
                {
                    const string message = "Request was executed unsuccessfully with result {StatusCode} {StatusCodeDescription}. Retry attempt {RetryAttempt} of {RetryCount} in {RetryTimeSpace}";

                    Log.ForContext<LoggingRequester>().Warning(
                        message,
                        (int?)response.Result.StatusCode,
                        retryAttempt,
                        restClientOptions.RetriesInMilliseconds!.Length,
                        timeSpan);

                    return Task.CompletedTask;
                });
        }
    }
}
