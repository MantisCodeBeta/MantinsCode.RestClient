using Microsoft.Extensions.Configuration;
using RestEase;

namespace MantinsCode.RestClient
{
    public static class LoggingRequesterFactoryProvider
    {
        public static Func<HttpClient, IRequester> GetLoggingRequester(IConfiguration configuration) => httpClient => new LoggingRequester(httpClient);
    }
}
