namespace RESTful.Exceptions;

public class NotFoundException : AppException
{
    public NotFoundException(string message) : base(message) { }
}