namespace MantinsCode.RestClient
{
    public class HttpException(string title, Exception innerException, string? endpoint)
        : Exception($"HTTP_ERROR_{title}", innerException)
    {
        public string? Endpoint { get; set; } = endpoint;
    }
}
