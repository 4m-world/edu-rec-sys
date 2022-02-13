using System.Globalization;

namespace CodeMatrix.Mepd.Infrastructure.Common.Services
{
    public class CsvWriter<T>
    {
        public async Task WriteAsync(IEnumerable<T> collection, Stream stream)
        {
            using var writer = new StreamWriter(stream);
            using var csv = new CsvHelper.CsvWriter(writer, CultureInfo.InvariantCulture);
            await csv.WriteRecordsAsync(collection);
        }
    }
}
