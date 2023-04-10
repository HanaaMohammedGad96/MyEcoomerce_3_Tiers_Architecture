namespace Core.Exceptions;

public class UnprocessableException : Exception
{
    public UnprocessableException(string message)
        : base(message) { }
}
