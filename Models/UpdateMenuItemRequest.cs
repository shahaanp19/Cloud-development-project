namespace CoffeeNChill.Models;

public class UpdateMenuItemRequest
{
    [PositivePrice]
    public string? Price { get; set; }

    public bool? IsAvailable { get; set; }
}