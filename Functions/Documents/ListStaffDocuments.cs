using CoffeeNChill.Models;
using CoffeeNChill.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace CoffeeNChill.Functions.Documents;

public sealed class ListStaffDocuments
{
    private readonly FileShareService _fileShareService;
    private readonly ILogger<ListStaffDocuments> _logger;

    public ListStaffDocuments(
        FileShareService fileShareService,
        ILogger<ListStaffDocuments> logger)
    {
        _fileShareService = fileShareService;
        _logger = logger;
    }

    [Function("ListStaffDocuments")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get",
            Route = "documents")]
        HttpRequestData req,
        CancellationToken cancellationToken)
    {
        try
        {
            List<StaffDocument> documents =
                await _fileShareService.ListAsync(
                    cancellationToken);

            _logger.LogInformation(
                "Retrieved {DocumentCount} staff documents.",
                documents.Count);

            var response =
                req.CreateResponse(HttpStatusCode.OK);

            await response.WriteAsJsonAsync(documents);

            return response;
        }
        catch (OperationCanceledException) when (
            cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation(
                "Staff document list request was cancelled.");

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error while retrieving staff documents.");

            return await CreateErrorResponseAsync(
                req,
                HttpStatusCode.InternalServerError,
                "An unexpected error occurred while retrieving documents.");
        }
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