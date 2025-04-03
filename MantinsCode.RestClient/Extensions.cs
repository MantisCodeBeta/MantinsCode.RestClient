using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RestEase.HttpClientFactory;

namespace MantisCode.RestClient
{
    public static class Extensions
    {
        public static IHttpClientBuilder AddRestClient<T>(this IServiceCollection services, IConfiguration configuration, string section)
            where T : class
        {
            services.TryAddSingleton(TimeProvider.System);

            IConfigurationSection configurationSection = configuration.GetRequiredSection(section);
            RestClientOptions restClientOptions = new RestClientOptions.RestClientOptionsBuilder(configurationSection).Build();

            services.AddOptions<RestClientOptions>().BindConfiguration(configurationSection.Key);
            services.AddTransient<HttpExceptionHandler>();

            IHttpClientBuilder builder = services
                .AddHttpClient(typeof(T).FullName!, HttpClientConfiguratorProvider.GetConfigurator(restClientOptions))
                .UseWithRestEaseClient
                (new UseWithRestEaseClientOptions<T>
                {
                    RequesterFactory = LoggingRequesterFactoryProvider.GetLoggingRequester(configuration)
                })
                .AddHttpMessageHandler<HttpExceptionHandler>()
                .AddPolicyHandler(request => PolicyProvider.GetBasePolicy(restClientOptions, request));

            if (restClientOptions.EnableRateLimiting)
                builder.AddHttpMessageHandler(() => new RateLimitHandler(restClientOptions)).SetHandlerLifetime(Timeout.InfiniteTimeSpan);

            return builder;
        }
    };
}
