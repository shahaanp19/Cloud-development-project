using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using CoffeeNChill.Models;

namespace CoffeeNChill.Services;

public sealed class FileShareService
{
    private const string ShareName = "staff-docs";

    private readonly ShareClient _shareClient;

    public FileShareService(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException(
                "Azure Storage connection string is required.",
                nameof(connectionString));
        }

        _shareClient = new ShareClient(
            connectionString,
            ShareName);
    }

    public async Task InitializeAsync(
        CancellationToken cancellationToken = default)
    {
        await _shareClient.CreateIfNotExistsAsync(
            cancellationToken: cancellationToken);
    }

    public async Task UploadAsync(
        string fileName,
        Stream content,
        CancellationToken cancellationToken = default)
    {
        ValidateFileName(fileName);

        ArgumentNullException.ThrowIfNull(content);

        if (!content.CanRead)
        {
            throw new ArgumentException(
                "The supplied file stream cannot be read.",
                nameof(content));
        }

        ShareDirectoryClient directoryClient =
            _shareClient.GetRootDirectoryClient();

        ShareFileClient fileClient =
            directoryClient.GetFileClient(fileName);

        await fileClient.CreateAsync(
            maxSize: content.Length,
            cancellationToken: cancellationToken);

        await fileClient.UploadRangeAsync(
            new HttpRange(0, content.Length),
            content,
            cancellationToken: cancellationToken);
    }

    public async Task<List<StaffDocument>> ListAsync(
        CancellationToken cancellationToken = default)
    {
        var documents = new List<StaffDocument>();

        ShareDirectoryClient directoryClient =
            _shareClient.GetRootDirectoryClient();

        await foreach (
            ShareFileItem item in
            directoryClient.GetFilesAndDirectoriesAsync(
                cancellationToken: cancellationToken))
        {
            if (item.IsDirectory)
            {
                continue;
            }

            ShareFileClient fileClient =
                directoryClient.GetFileClient(item.Name);

            ShareFileProperties properties =
                await fileClient.GetPropertiesAsync(
                    cancellationToken: cancellationToken);

            documents.Add(
                new StaffDocument
                {
                    FileName = item.Name,
                    Size = properties.ContentLength,
                    LastModified = properties.LastModified
                });
        }

        return documents
            .OrderByDescending(
                document => document.LastModified)
            .ToList();
    }

    public async Task<Stream?> DownloadAsync(
        string fileName,
        CancellationToken cancellationToken = default)
    {
        ValidateFileName(fileName);

        ShareDirectoryClient directoryClient =
            _shareClient.GetRootDirectoryClient();

        ShareFileClient fileClient =
            directoryClient.GetFileClient(fileName);

        try
        {
            ShareFileDownloadInfo download =
      await fileClient.DownloadAsync();

            return download.Content;
        }
        catch (RequestFailedException ex)
            when (ex.Status == 404)
        {
            return null;
        }
    }

    private static void ValidateFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException(
                "File name cannot be empty.",
                nameof(fileName));
        }

        if (fileName.Contains('/') ||
            fileName.Contains('\\') ||
            fileName.Contains(".."))
        {
            throw new ArgumentException(
                "File name contains invalid path characters.",
                nameof(fileName));
        }

        string actualFileName =
            Path.GetFileName(fileName);

        if (!string.Equals(
                actualFileName,
                fileName,
                StringComparison.Ordinal))
        {
            throw new ArgumentException(
                "File name must not contain a directory path.",
                nameof(fileName));
        }
    }
}