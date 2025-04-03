namespace MantinsCode.RestClient
{
    internal static class HttpClientConfiguratorProvider
    {
        internal static Action<HttpClient> GetConfigurator(RestClientOptions restClientOptions)
        {
            return httpClient =>
            {
                httpClient.BaseAddress = new Uri(restClientOptions.Address);

                if (restClientOptions.Headers?.Count > 0)
                {
                    foreach (KeyValuePair<string, string> header in restClientOptions.Headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                if (restClientOptions.GlobalTimeoutInSeconds.HasValue)
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(restClientOptions.GlobalTimeoutInSeconds.Value);
                }
            };
        }
    }
}
