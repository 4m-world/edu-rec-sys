using CodeMatrix.Mepd.Application.Common.Interfaces.Csv;
using System.Globalization;

namespace CodeMatrix.Mepd.Infrastructure.Common.Services
{
    public class CsvReader<T> : ICsvReader<T>
    {
        public IAsyncEnumerable<T> ReadAsync(Stream stream)
        {
            using var reader = new StreamReader(stream);
            using var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture);
            return csv.GetRecordsAsync<T>();
        }
    }
}
