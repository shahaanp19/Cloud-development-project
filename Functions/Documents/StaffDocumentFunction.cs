using CoffeeNChill.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace CoffeeNChill.Functions;

public sealed class StaffDocumentFunction
{
    private readonly FileShareService _fileShareService;

    public StaffDocumentFunction(FileShareService fileShareService)
    {
        _fileShareService = fileShareService;
    }

    [Function("UploadStaffDocument")]
    public async Task<IActionResult> UploadStaffDocument(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "post",
            Route = "staff-documents")]
        HttpRequest request,
        CancellationToken cancellationToken)
    {
        if (!request.HasFormContentType)
        {
            return new BadRequestObjectResult(
                "The request must contain multipart form data.");
        }

        IFormCollection form =
            await request.ReadFormAsync(cancellationToken);

        IFormFile? file = form.Files.FirstOrDefault();

        if (file is null || file.Length == 0)
        {
            return new BadRequestObjectResult(
                "Please provide a valid file.");
        }

        await _fileShareService.InitializeAsync(cancellationToken);

        await using Stream stream = file.OpenReadStream();

        await _fileShareService.UploadAsync(
            file.FileName,
            stream,
            cancellationToken);

        return new OkObjectResult(new
        {
            message = "Document uploaded successfully.",
            fileName = file.FileName,
            size = file.Length
        });
    }

    [Function("GetStaffDocuments")]
    public async Task<IActionResult> GetStaffDocuments(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get",
            Route = "staff-documents")]
        HttpRequest request,
        CancellationToken cancellationToken)
    {
        await _fileShareService.InitializeAsync(cancellationToken);

        var documents =
            await _fileShareService.ListAsync(cancellationToken);

        return new OkObjectResult(documents);
    }

    [Function("DownloadStaffDocument")]
    public async Task<IActionResult> DownloadStaffDocument(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get",
            Route = "staff-documents/{fileName}")]
        HttpRequest request,
        string fileName,
        CancellationToken cancellationToken)
    {
        await _fileShareService.InitializeAsync(cancellationToken);

        Stream? document =
            await _fileShareService.DownloadAsync(
                fileName,
                cancellationToken);

        if (document is null)
        {
            return new NotFoundObjectResult(
                "The requested document was not found.");
        }

        return new FileStreamResult(
            document,
            "application/octet-stream")
        {
            FileDownloadName = fileName
        };
    }
}
