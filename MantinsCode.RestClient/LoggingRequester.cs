using RestEase;
using RestEase.Implementation;

namespace MantinsCode.RestClient
{
    public class LoggingRequester(HttpClient httpClient) : Requester(httpClient)
    {
        //private readonly ILogger logger = Log.ForContext<LoggingRequester>();

        public override async Task RequestVoidAsync(IRequestInfo requestInfo)
        {
            try
            {
                HttpResponseMessage responseMessage = await SendRequestAsync(requestInfo, false);
            }
            catch (Exception ex)
            {
                Exception exception = ex;
                throw;
            }
            finally
            {
                //
            }
        }
    }
}
