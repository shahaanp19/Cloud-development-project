using Azure;
using CoffeeNChill.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace CoffeeNChill.Functions.Documents;

public sealed class UploadStaffDocument
{
    private static readonly HashSet<string> AllowedContentTypes =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "application/pdf",
            "application/msword",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "application/vnd.ms-excel",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "image/jpeg",
            "image/png"
        };

    private readonly FileShareService _fileShareService;
    private readonly ILogger<UploadStaffDocument> _logger;

    public UploadStaffDocument(
        FileShareService fileShareService,
        ILogger<UploadStaffDocument> logger)
    {
        _fileShareService = fileShareService;
        _logger = logger;
    }

    [Function("UploadStaffDocument")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "post",
            Route = "documents/upload")]
        HttpRequestData req,
        CancellationToken cancellationToken)
    {
        try
        {
            string contentType = GetContentType(req);

            if (!AllowedContentTypes.Contains(contentType))
            {
                _logger.LogWarning(
                    "Rejected document upload with unsupported content type '{ContentType}'.",
                    contentType);

                return await CreateErrorResponseAsync(
                    req,
                    HttpStatusCode.BadRequest,
                    "The supplied file type is not supported.");
            }

            string fileName = GetFileName(req);

            if (string.IsNullOrWhiteSpace(fileName))
            {
                return await CreateErrorResponseAsync(
                    req,
                    HttpStatusCode.BadRequest,
                    "X-File-Name header is required.");
            }

            if (req.Body is null || !req.Body.CanRead)
            {
                return await CreateErrorResponseAsync(
                    req,
                    HttpStatusCode.BadRequest,
                    "A readable file stream is required.");
            }

            await _fileShareService.UploadAsync(
                fileName,
                req.Body,
                cancellationToken);

            _logger.LogInformation(
                "Staff document '{FileName}' uploaded successfully.",
                fileName);

            var response =
                req.CreateResponse(HttpStatusCode.Created);

            await response.WriteAsJsonAsync(
                new
                {
                    message = "Document uploaded successfully.",
                    fileName,
                    contentType
                });

            return response;
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(
                ex,
                "Invalid staff document upload request.");

            return await CreateErrorResponseAsync(
                req,
                HttpStatusCode.BadRequest,
                ex.Message);
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(
                ex,
                "Azure Storage error while uploading staff document.");

            return await CreateErrorResponseAsync(
                req,
                HttpStatusCode.InternalServerError,
                "The document could not be uploaded because of a storage error.");
        }
        catch (OperationCanceledException) when (
            cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation(
                "Staff document upload request was cancelled.");

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error while uploading staff document.");

            return await CreateErrorResponseAsync(
                req,
                HttpStatusCode.InternalServerError,
                "An unexpected error occurred while uploading the document.");
        }
    }

    private static string GetContentType(HttpRequestData req)
    {
        if (!req.Headers.TryGetValues(
                "Content-Type",
                out var contentTypes))
        {
            return string.Empty;
        }

        return contentTypes
            .FirstOrDefault()?
            .Split(';', 2)[0]
            .Trim() ?? string.Empty;
    }

    private static string GetFileName(HttpRequestData req)
    {
        if (!req.Headers.TryGetValues(
                "X-File-Name",
                out var fileNames))
        {
            return string.Empty;
        }

        return fileNames
            .FirstOrDefault()?
            .Trim() ?? string.Empty;
    }

    private static async Task<HttpResponseData> CreateErrorResponseAsync(
        HttpRequestData req,
        HttpStatusCode statusCode,
        string message)
    {
        var response = req.CreateResponse(statusCode);

        await response.WriteAsJsonAsync(
            new
            {
                error = message
            });

        return response;
    }
}