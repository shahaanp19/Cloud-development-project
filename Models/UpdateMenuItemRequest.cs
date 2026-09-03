using System.ComponentModel.DataAnnotations;

namespace CoffeeNChill.Models;

public class UpdateMenuItemRequest
{
    public string? Price { get; set; }

    public bool? IsAvailable { get; set; }
}