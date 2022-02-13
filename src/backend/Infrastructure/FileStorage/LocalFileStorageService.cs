using CodeMatrix.Mepd.Application.Common.FileStorage;
using CodeMatrix.Mepd.Infrastructure.Common.Extensions;
using CodeMatrix.Mepd.Shared.DTOs.Common;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace CodeMatrix.Mepd.Infrastructure.FileStorage;

/// <inheritdoc />
public class LocalFileStorageService : IFileStorageService
{
    /// <inheritdoc />
    public Task<string> UploadAsync<T>(FileUploadRequest request, FileType supportedFileType, CancellationToken cancellationToken = default)
        where T : class
    {
        if (request == null || request.Data == null)
        {
            return Task.FromResult(string.Empty);
        }

        if (request.Extension is null || !supportedFileType.GetDescriptionList().Contains(request.Extension))
            throw new InvalidOperationException("File Format Not Supported.");
        if (request.Name is null)
            throw new InvalidOperationException("Name is required.");

        string base64Data = Regex.Match(request.Data, "data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;

        var streamData = new MemoryStream(Convert.FromBase64String(base64Data));
        if (streamData.Length > 0)
        {
            string folder = typeof(T).Name;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                folder = folder.Replace(@"\", "/", StringComparison.OrdinalIgnoreCase);
            }

            string folderName = supportedFileType switch
            {
                FileType.Image => Path.Combine("Files", "Images", folder),
                _ => Path.Combine("Files", "Others", folder),
            };
            string pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            bool exists = Directory.Exists(pathToSave);
            if (!exists)
            {
                Directory.CreateDirectory(pathToSave);
            }

            string fileName = request.Name.Trim('"');
            fileName = RemoveSpecialCharacters(fileName);
            fileName = fileName.ReplaceWhitespace("-");
            fileName += request.Extension.Trim();
            string fullPath = Path.Combine(pathToSave, fileName);
            string dbPath = Path.Combine(folderName, fileName);
            if (File.Exists(dbPath))
            {
                dbPath = NextAvailableFilename(dbPath);
                fullPath = NextAvailableFilename(fullPath);
            }

            using var stream = new FileStream(fullPath, FileMode.Create);
            streamData.CopyTo(stream);
            dbPath = dbPath.Replace("\\", "/", StringComparison.OrdinalIgnoreCase);
            return Task.FromResult("{server_url}/" + dbPath);
        }
        else
        {
            return Task.FromResult(string.Empty);
        }
    }

    /// <inheritdoc />
    public void Remove(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    private static string RemoveSpecialCharacters(string str)
    {
        return Regex.Replace(str, "[^a-zA-Z0-9_.]+", string.Empty, RegexOptions.Compiled);
    }

    private const string NumberPattern = "-{0}";

    private static string NextAvailableFilename(string path)
    {
        if (!File.Exists(path))
        {
            return path;
        }

        if (Path.HasExtension(path))
        {
            return GetNextFilename(path.Insert(path.LastIndexOf(Path.GetExtension(path), StringComparison.Ordinal), NumberPattern));
        }

        return GetNextFilename(path + NumberPattern);
    }

    private static string GetNextFilename(string pattern)
    {
        string tmp = string.Format(pattern, 1);

        if (!File.Exists(tmp))
        {
            return tmp;
        }

        int min = 1, max = 2;

        while (File.Exists(string.Format(pattern, max)))
        {
            min = max;
            max *= 2;
        }

        while (max != min + 1)
        {
            int pivot = (max + min) / 2;
            if (File.Exists(string.Format(pattern, pivot)))
            {
                min = pivot;
            }
            else
            {
                max = pivot;
            }
        }

        return string.Format(pattern, max);
    }
}