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
    [StringLength(10, MinimumLength = 3)]
    public string Price { get; set; } = string.Empty;

    public bool IsAvailable { get; set; }
}