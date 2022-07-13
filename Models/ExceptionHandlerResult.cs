namespace TestApi.Models
{
    // utility model used as return value in Handlers
    public class ExceptionHandlerResult
    {
        public int statusCode { get; set; }
        public Object? message { get; set; }
    }
}
