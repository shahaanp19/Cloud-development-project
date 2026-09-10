using Azure;
using Azure.Data.Tables;

namespace CoffeeNChill.Models;

public class MenuItem : ITableEntity
{
    // Azure Table Storage keys
    public string PartitionKey { get; set; } = string.Empty;

    public string RowKey { get; set; } = string.Empty;

    public DateTimeOffset? Timestamp { get; set; }

    public ETag ETag { get; set; }

    // CoffeeNChill menu item data
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Price { get; set; } = string.Empty;

    public bool IsAvailable { get; set; }
}

// azure-sdk (2026). ITableEntity Interface (Azure.Data.Tables) - Azure for .NET Developers. [online] MicrosoftLearn. Available at: https://learn.microsoft.com/en-us/dotnet/api/azure.data.tables.itableentity?view=azure-dotnet [Accessed 10 Sept. 2026].
// azure-sdk (2026). TableEntity Class (Azure.Data.Tables) - Azure for .NET Developers. [online] MicrosoftLearn. Available at: https://learn.microsoft.com/en-us/dotnet/api/azure.data.tables.tableentity?view=azure-dotnet [Accessed 10 Sept. 2026].
// azure-sdk (2026). TableEntity Class (Azure.Data.Tables) - Azure for .NET Developers. [online] MicrosoftLearn. Available at: https://learn.microsoft.com/en-us/dotnet/api/azure.data.tables.tableentity?view=azure-dotnet [Accessed 10 Sept. 2026].
