using CoffeeNChill.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace CoffeeNChill.Functions.Documents;

public sealed class DownloadStaffDocument
{
    private readonly FileShareService _fileShareService;
    private readonly ILogger<DownloadStaffDocument> _logger;

    public DownloadStaffDocument(
        FileShareService fileShareService,
        ILogger<DownloadStaffDocument> logger)
    {
        _fileShareService = fileShareService;
        _logger = logger;
    }

    [Function("DownloadStaffDocument")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get",
            Route = "documents/download/{fileName}")]
        HttpRequestData req,
        string fileName,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return await CreateErrorResponseAsync(
                req,
                HttpStatusCode.BadRequest,
                "File name is required.");
        }

        try
        {
            Stream? fileStream =
                await _fileShareService.DownloadAsync(
                    fileName,
                    cancellationToken);

            if (fileStream is null)
            {
                _logger.LogInformation(
                    "Staff document '{FileName}' was not found.",
                    fileName);

                return await CreateErrorResponseAsync(
                    req,
                    HttpStatusCode.NotFound,
                    $"Document '{fileName}' was not found.");
            }

            await using (fileStream)
            {
                var response =
                    req.CreateResponse(HttpStatusCode.OK);

                response.Headers.Add(
                    "Content-Type",
                    GetContentType(fileName));

                response.Headers.Add(
                    "Content-Disposition",
                    $"attachment; filename=\"{fileName}\"");

                await fileStream.CopyToAsync(
                    response.Body,
                    cancellationToken);

                return response;
            }
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(
                ex,
                "Invalid file name supplied for document download: '{FileName}'.",
                fileName);

            return await CreateErrorResponseAsync(
                req,
                HttpStatusCode.BadRequest,
                ex.Message);
        }
        catch (OperationCanceledException) when (
            cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation(
                "Download request for staff document '{FileName}' was cancelled.",
                fileName);

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error while downloading staff document '{FileName}'.",
                fileName);

            return await CreateErrorResponseAsync(
                req,
                HttpStatusCode.InternalServerError,
                "An unexpected error occurred while downloading the document.");
        }
    }

    private static string GetContentType(string fileName)
    {
        return Path.GetExtension(fileName).ToLowerInvariant() switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" =>
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" =>
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            _ => "application/octet-stream"
        };
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