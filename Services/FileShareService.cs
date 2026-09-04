using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CoffeeNChill.Models;

namespace CoffeeNChill.Services;

public sealed class FileShareService
{
    private const string ContainerName = "staff-docs";

    private readonly BlobContainerClient _containerClient;

    public FileShareService(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException(
                "Azure Storage connection string is required.",
                nameof(connectionString));
        }

        _containerClient = new BlobContainerClient(
            connectionString,
            ContainerName);
    }

    public async Task InitializeAsync(
        CancellationToken cancellationToken = default)
    {
        await _containerClient.CreateIfNotExistsAsync(
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

        BlobClient blobClient =
            _containerClient.GetBlobClient(fileName);

        await blobClient.UploadAsync(
            content,
            overwrite: true,
            cancellationToken: cancellationToken);
    }

    public async Task<List<StaffDocument>> ListAsync(
        CancellationToken cancellationToken = default)
    {
        var documents = new List<StaffDocument>();

        await foreach (
            BlobItem item in
            _containerClient.GetBlobsAsync(
                cancellationToken: cancellationToken))
        {
            documents.Add(
                new StaffDocument
                {
                    FileName = item.Name,
                    Size = item.Properties.ContentLength ?? 0,
                    LastModified =
                        item.Properties.LastModified ?? default
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

        BlobClient blobClient =
            _containerClient.GetBlobClient(fileName);

        try
        {
            BlobDownloadStreamingResult download =
                await blobClient.DownloadStreamingAsync(
                    cancellationToken: cancellationToken);

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