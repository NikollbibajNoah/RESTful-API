namespace RESTful.Api.Middleware;

public class ErrorDetails
{
    public int StatusCode { get; set; }

    public string Message { get; set; } = String.Empty;

    public string? Details { get; set; }
}