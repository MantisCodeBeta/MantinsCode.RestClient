using Polly.RateLimit;
using System.Threading.RateLimiting;

namespace MantisCode.RestClient
{
    public class RateLimitHandler(RestClientOptions restClientOptions) : DelegatingHandler, IAsyncDisposable
    {
        private readonly RateLimiter limiter = new SlidingWindowRateLimiter(new SlidingWindowRateLimiterOptions
        {
            AutoReplenishment = true,
            PermitLimit = restClientOptions.RateLimit!.Value,
            QueueLimit = 0,
            SegmentsPerWindow = restClientOptions.RateLimitWindowInSeconds!.Value,
            Window = TimeSpan.FromSeconds(restClientOptions.RateLimitWindowInSeconds.Value),
        });

        public async ValueTask DisposeAsync()
        {
            await limiter.DisposeAsync().ConfigureAwait(false);

            Dispose(disposing: false);
            GC.SuppressFinalize(this);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            using RateLimitLease lease = await limiter.AcquireAsync(1, cancellationToken);

            if (!lease.IsAcquired)
                throw new RateLimitRejectedException("Rate limit exceeded. Plase try again later");

            return await base.SendAsync(request, cancellationToken);
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
                limiter.Dispose();
        }
    }
}
