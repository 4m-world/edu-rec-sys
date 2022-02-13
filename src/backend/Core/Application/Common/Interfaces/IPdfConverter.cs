namespace CodeMatrix.Mepd.Application.Common.Interfaces;

public interface IPdfConverter
{
    Task<Stream> ConvertAasync(string html, PdfOptions options = default);
}

public class PdfOptions { }
