using System.ComponentModel.DataAnnotations;

namespace CoffeeNChill.Models;

public class CreateMenuItemRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Id { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(500, MinimumLength = 5)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [PositivePrice]
    public string Price { get; set; } = string.Empty;

    public bool IsAvailable { get; set; }
}

// azure-sdk (2026). ITableEntity Interface (Azure.Data.Tables) - Azure for .NET Developers. [online] MicrosoftLearn. Available at: https://learn.microsoft.com/en-us/dotnet/api/azure.data.tables.itableentity?view=azure-dotnet [Accessed 10 Sept. 2026].
// azure-sdk (2026). TableEntity Class (Azure.Data.Tables) - Azure for .NET Developers. [online] MicrosoftLearn. Available at: https://learn.microsoft.com/en-us/dotnet/api/azure.data.tables.tableentity?view=azure-dotnet [Accessed 10 Sept. 2026].
// azure-sdk (2026). TableEntity Class (Azure.Data.Tables) - Azure for .NET Developers. [online] MicrosoftLearn. Available at: https://learn.microsoft.com/en-us/dotnet/api/azure.data.tables.tableentity?view=azure-dotnet [Accessed 10 Sept. 2026].
