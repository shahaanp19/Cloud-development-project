using Azure.Data.Tables;
using CoffeeNChill.Services;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

string? connectionString =
    builder.Configuration["AzureWebJobsStorage"];

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "AzureWebJobsStorage connection string is not configured.");
}

if (connectionString == "UseDevelopmentStorage=true")
{
    connectionString =
        "DefaultEndpointsProtocol=http;" +
        "AccountName=devstoreaccount1;" +
       "AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;" +
        "BlobEndpoint=http://host.docker.internal:10000/devstoreaccount1;" +
        "QueueEndpoint=http://host.docker.internal:10001/devstoreaccount1;" +
        "TableEndpoint=http://host.docker.internal:10002/devstoreaccount1;";
}

builder.Services.AddSingleton(
    new TableServiceClient(connectionString));

builder.Services.AddSingleton<MenuItemService>();
builder.Services.AddSingleton(
    new FileShareService(connectionString));

var host = builder.Build();

await host.Services
    .GetRequiredService<MenuItemService>()
    .InitializeAsync();

await host.Services
    .GetRequiredService<FileShareService>()
    .InitializeAsync();

host.Run();

//ggailey777 (2024). Use dependency injection in .NET Azure Functions. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection [Accessed 10 Sept. 2026].
//MicrosoftLearn. (2025). Azure Function isolated worker - how to run code on start up - Microsoft Q&A. [online] Available at: https://learn.microsoft.com/en-us/answers/questions/5534310/azure-function-isolated-worker-how-to-run-code-on [Accessed 10 Sept. 2026].
//gewarren (2026). Dependency injection - .NET. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection/overview [Accessed 10 Sept. 2026].
//tdykstra (2024). Dependency injection in ASP.NET Core. [online] Microsoft.com. Available at: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-10.0 [Accessed 10 Sept. 2026].