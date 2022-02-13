namespace CodeMatrix.Mepd.Application.Common.Interfaces.Csv;

public interface ICsvReader<T>
{
    IAsyncEnumerable<T> ReadAsync(Stream stream);
}
