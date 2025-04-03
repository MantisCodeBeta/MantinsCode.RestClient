namespace MantinsCode.RestClient
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class ApiClientAttribute : Attribute
    {
    }

    public class InternalApiClientAttribute : ApiClientAttribute
    {

    }
    public class ExtenalApiClientAttribute : ApiClientAttribute
    {

    }
}
