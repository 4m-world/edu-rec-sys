namespace CodeMatrix.Mepd.Application.Common.Interfaces.Excel;

public interface IExcelReader<T> : IScopedService
{
    Task<T> ReadAsync(Stream stream);
}

