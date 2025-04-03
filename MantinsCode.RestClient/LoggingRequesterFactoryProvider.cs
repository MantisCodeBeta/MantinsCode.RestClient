using Microsoft.Extensions.Configuration;
using RestEase;

namespace MantisCode.RestClient
{
    public static class LoggingRequesterFactoryProvider
    {
        public static Func<HttpClient, IRequester> GetLoggingRequester(IConfiguration configuration) => httpClient => new LoggingRequester(httpClient);
    }
}
