using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace CoffeeNChill.Models;

public sealed class PositivePriceAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(
        object? value,
        ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        var price = value.ToString();

        if (string.IsNullOrWhiteSpace(price))
        {
            return new ValidationResult("Price is required.");
        }

        if (!decimal.TryParse(
                price,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var parsedPrice))
        {
            return new ValidationResult(
                "Price must be a valid number.");
        }

        if (parsedPrice <= 0)
        {
            return new ValidationResult(
                "Price must be greater than zero.");
        }

        return ValidationResult.Success;
    }
}

// azure-sdk (2026). ITableEntity Interface (Azure.Data.Tables) - Azure for .NET Developers. [online] MicrosoftLearn. Available at: https://learn.microsoft.com/en-us/dotnet/api/azure.data.tables.itableentity?view=azure-dotnet [Accessed 10 Sept. 2026].
// azure-sdk (2026). TableEntity Class (Azure.Data.Tables) - Azure for .NET Developers. [online] MicrosoftLearn. Available at: https://learn.microsoft.com/en-us/dotnet/api/azure.data.tables.tableentity?view=azure-dotnet [Accessed 10 Sept. 2026].
// azure-sdk (2026). TableEntity Class (Azure.Data.Tables) - Azure for .NET Developers. [online] MicrosoftLearn. Available at: https://learn.microsoft.com/en-us/dotnet/api/azure.data.tables.tableentity?view=azure-dotnet [Accessed 10 Sept. 2026].
