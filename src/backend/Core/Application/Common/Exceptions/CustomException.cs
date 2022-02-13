using System.Net;

namespace CodeMatrix.Mepd.Application.Common.Exceptions;

public class CustomException : Exception
{
    public List<string> ErrorMessages { get; }

    public HttpStatusCode StatusCode { get; }

    public CustomException(string message, List<string> errors = null, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        : base(message)
    {
        ErrorMessages = errors;
        StatusCode = statusCode;
    }
}