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