namespace RESTful.Api.Exceptions;

public class DatabaseException : AppException
{
    public DatabaseException(string message) : base(message) { }

    public DatabaseException(string message, Exception innerException)
        : base(message, innerException) { }
}
