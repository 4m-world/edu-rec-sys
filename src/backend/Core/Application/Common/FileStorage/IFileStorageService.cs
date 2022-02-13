using CodeMatrix.Mepd.Shared.DTOs.Common;

namespace CodeMatrix.Mepd.Application.Common.FileStorage;

/// <summary>
/// File storage service contract
/// </summary>
public interface IFileStorageService : ITransientService
{
    /// <summary>
    /// Upload file
    /// </summary>
    /// <typeparam name="T">Type that contians uploaded resource</typeparam>
    /// <param name="request">File upload request</param>
    /// <param name="supportedFileType">Supported file type</param>
    /// <param name="cancellationToken"></param>
    /// <returns>file server url</returns>
    public Task<string> UploadAsync<T>(FileUploadRequest request, FileType supportedFileType, CancellationToken cancellationToken = default)
        where T : class;

    /// <summary>
    /// Remove file 
    /// </summary>
    /// <param name="path">File path</param>
    public void Remove(string path);
}