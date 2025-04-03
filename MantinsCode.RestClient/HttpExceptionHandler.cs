namespace MantinsCode.RestClient
{
    public class HttpExceptionHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                return await base.SendAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new HttpException(ex.Message, ex, request?.RequestUri?.PathAndQuery);
            }
        }
    }
}
