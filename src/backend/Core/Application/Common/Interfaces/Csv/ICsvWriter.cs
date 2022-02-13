namespace CodeMatrix.Mepd.Application.Common.Interfaces.Csv;

public interface ICsvWriter<T> : IScopedService
{
    Task WriteAsync(IEnumerable<T> collection, Stream stream);
}
