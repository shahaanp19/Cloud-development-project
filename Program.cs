using Azure.Data.Tables;
using CoffeeNChill.Services;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var connectionString =
    builder.Configuration["AzureWebJobsStorage"];

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "AzureWebJobsStorage connection string is not configured.");
}

builder.Services.AddSingleton(
    new TableServiceClient(connectionString));

builder.Services.AddSingleton<MenuItemService>();

var host = builder.Build();

var menuItemService =
    host.Services.GetRequiredService<MenuItemService>();

await menuItemService.InitializeAsync();

host.Run();