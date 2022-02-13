namespace CodeMatrix.Mepd.Application.Common.Interfaces.Excel;

public interface IExcelWriter<T> : IScopedService
{
    Task WriteAsync(T data, Stream stream);
}

