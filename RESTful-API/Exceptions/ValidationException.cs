namespace RESTful.Exceptions;

public class ValidationException : AppException
{

    public IEnumerable<string> Errors { get; set; }

    public ValidationException(string message) : base(message)
    {
        Errors = [message];
    }

    public ValidationException(IEnumerable<string> errors) : base("Validation failed")
    {
        Errors = errors ?? throw new ArgumentNullException(nameof(errors));
    }

    public ValidationException(string field, string message) : base($"{field}: {message}")
    {
        Errors = [$"{field}: {message}"];
    }

    public ValidationException(Dictionary<string, string[]> validationErrors) : base("Validation failed")
    {
        Errors = validationErrors.SelectMany(kvp =>
            kvp.Value.Select(error => $"{kvp.Key}: {error}"));
    }
}