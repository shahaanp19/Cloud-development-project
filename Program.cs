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