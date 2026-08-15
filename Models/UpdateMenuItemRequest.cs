using System.ComponentModel.DataAnnotations;

namespace CoffeeNChill.Models;

public class UpdateMenuItemRequest
{
    [Range(0.01, 100000)]
    public decimal? Price { get; set; }

    public bool? IsAvailable { get; set; }
}