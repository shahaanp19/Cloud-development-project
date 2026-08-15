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

    public decimal Price { get; set; }

    public bool IsAvailable { get; set; }
}