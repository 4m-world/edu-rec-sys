namespace CodeMatrix.Mepd.Application.Wrapper;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public class Result
{
    public List<string> Messages { get; set; } = new();

    public bool Succeeded { get; set; }
}

public class Result<T> : Result
{
    public T Data { get; set; }

    public static Result<T> Success(string message)
    {
        return new() { Succeeded = true, Messages = new List<string> { message } };
    }

    public static Result<T> Success(T data)
    {
        return new() { Succeeded = true, Data = data };
    }   
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
