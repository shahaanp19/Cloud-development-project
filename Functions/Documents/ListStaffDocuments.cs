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

//ggailey777 (2026). Guide for running C# Azure Functions in an isolated worker process. [online] MicrosoftLearn. Available at: https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide?tabs=ihostapplicationbuilder%2Cconfig%2Cwindows#dependency-injection [Accessed 10 Sept. 2026].
//ggailey777 (2025). Azure Functions HTTP trigger. [online] MicrosoftLearn. Available at: https://learn.microsoft.com/en-us/azure/azure-functions/functions-bindings-http-webhook-trigger [Accessed 10 Sept. 2026].
//jviau (2026). Overview of Durable Functions in .NET Isolated Worker. [online] MicrosoftLearn. Available at: https://learn.microsoft.com/en-us/azure/durable-task/durable-functions/durable-functions-dotnet-isolated-overview [Accessed 10 Sept. 2026].