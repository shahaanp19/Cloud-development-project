using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Azure;
using CoffeeNChill.Models;
using CoffeeNChill.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CoffeeNChill.Functions;

public class MenuFunctions
{
    private readonly MenuItemService _menuItemService;
    private readonly ILogger<MenuFunctions> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public MenuFunctions(
        MenuItemService menuItemService,
        ILogger<MenuFunctions> logger)
    {
        _menuItemService = menuItemService;
        _logger = logger;
    }

    // POST /api/menu
    [Function("CreateMenuItem")]
    public async Task<IActionResult> CreateMenuItem(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "menu")]
        HttpRequest req)
    {
        var cancellationToken = req.HttpContext.RequestAborted;

        try
        {
            var request = await JsonSerializer.DeserializeAsync<CreateMenuItemRequest>(
                req.Body,
                JsonOptions,
                cancellationToken);

            if (request is null)
            {
                return new BadRequestObjectResult(new
                {
                    error = "Invalid request body."
                });
            }

            var validationResults = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(
                request,
                new ValidationContext(request),
                validationResults,
                validateAllProperties: true);

            if (!isValid)
            {
                return new BadRequestObjectResult(new
                {
                    error = "Validation failed.",
                    details = validationResults
                        .Select(result => result.ErrorMessage)
                        .Where(message => message is not null)
                });
            }

            var menuItem = new MenuItem
            {
                PartitionKey = request.Category.Trim(),
                RowKey = request.Id.Trim(),
                Name = request.Name.Trim(),
                Description = request.Description.Trim(),
                Price = request.Price,
                IsAvailable = request.IsAvailable
            };

            await _menuItemService.InitializeAsync(cancellationToken);
            await _menuItemService.CreateAsync(menuItem, cancellationToken);

            _logger.LogInformation(
                "Created menu item {MenuItemId} in category {Category}.",
                menuItem.RowKey,
                menuItem.PartitionKey);

            return new CreatedResult(
                $"/api/menu/{menuItem.PartitionKey}/{menuItem.RowKey}",
                menuItem);
        }
        catch (RequestFailedException ex) when (ex.Status == StatusCodes.Status409Conflict)
        {
            return new ConflictObjectResult(new
            {
                error = "A menu item with this ID already exists in this category."
            });
        }
        catch (JsonException)
        {
            return new BadRequestObjectResult(new
            {
                error = "The request body contains invalid JSON."
            });
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Create menu item request was cancelled.");

            return new StatusCodeResult(StatusCodes.Status499ClientClosedRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error while creating a menu item.");

            return new StatusCodeResult(
                StatusCodes.Status500InternalServerError);
        }
    }

    // GET /api/menu
    [Function("GetAllMenuItems")]
    public async Task<IActionResult> GetAllMenuItems(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "menu")]
        HttpRequest req)
    {
        var cancellationToken = req.HttpContext.RequestAborted;

        try
        {
            await _menuItemService.InitializeAsync(cancellationToken);

            var menuItems = await _menuItemService.GetAllAsync(
                cancellationToken);

            return new OkObjectResult(menuItems);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Get all menu items request was cancelled.");

            return new StatusCodeResult(StatusCodes.Status499ClientClosedRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error while retrieving menu items.");

            return new StatusCodeResult(
                StatusCodes.Status500InternalServerError);
        }
    }

    // GET /api/menu/category/{category}
    [Function("GetMenuItemsByCategory")]
    public async Task<IActionResult> GetMenuItemsByCategory(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "get",
            Route = "menu/category/{category}")]
        HttpRequest req,
        string category)
    {
        var cancellationToken = req.HttpContext.RequestAborted;

        try
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return new BadRequestObjectResult(new
                {
                    error = "Category is required."
                });
            }

            await _menuItemService.InitializeAsync(cancellationToken);

            var menuItems = await _menuItemService.GetByCategoryAsync(
                category.Trim(),
                cancellationToken);

            return new OkObjectResult(menuItems);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(
                "Get menu items by category request was cancelled.");

            return new StatusCodeResult(StatusCodes.Status499ClientClosedRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error while retrieving menu items for category {Category}.",
                category);

            return new StatusCodeResult(
                StatusCodes.Status500InternalServerError);
        }
    }

    // PUT /api/menu/{category}/{id}
    [Function("UpdateMenuItem")]
    public async Task<IActionResult> UpdateMenuItem(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "put",
            Route = "menu/{category}/{id}")]
        HttpRequest req,
        string category,
        string id)
    {
        var cancellationToken = req.HttpContext.RequestAborted;

        try
        {
            if (string.IsNullOrWhiteSpace(category) ||
                string.IsNullOrWhiteSpace(id))
            {
                return new BadRequestObjectResult(new
                {
                    error = "Category and ID are required."
                });
            }

            var request = await JsonSerializer.DeserializeAsync<UpdateMenuItemRequest>(
                req.Body,
                JsonOptions,
                cancellationToken);

            if (request is null)
            {
                return new BadRequestObjectResult(new
                {
                    error = "Invalid request body."
                });
            }

            var hasPrice = !string.IsNullOrWhiteSpace(request.Price);

            if (!hasPrice && !request.IsAvailable.HasValue)
            {
                return new BadRequestObjectResult(new
                {
                    error = "At least one of Price or IsAvailable must be provided."
                });
            }

            decimal parsedPrice = 0;
            if (hasPrice)
            {
                if (!decimal.TryParse(request.Price, System.Globalization.CultureInfo.InvariantCulture, out parsedPrice) || parsedPrice <= 0)
                {
                    return new BadRequestObjectResult(new
                    {
                        error = "Price must be a valid number greater than zero."
                    });
                }
            }

            await _menuItemService.InitializeAsync(cancellationToken);

            var menuItem = await _menuItemService.GetByIdAsync(
                category.Trim(),
                id.Trim(),
                cancellationToken);

            if (menuItem is null)
            {
                return new NotFoundObjectResult(new
                {
                    error = "Menu item not found."
                });
            }

            if (hasPrice)
            {
                menuItem.Price = request.Price!;
            }

            if (request.IsAvailable.HasValue)
            {
                menuItem.IsAvailable = request.IsAvailable.Value;
            }

            var updated = await _menuItemService.UpdateAsync(
                menuItem,
                cancellationToken);

            if (!updated)
            {
                return new NotFoundObjectResult(new
                {
                    error = "Menu item not found."
                });
            }

            _logger.LogInformation(
                "Updated menu item {MenuItemId} in category {Category}.",
                id,
                category);

            return new OkObjectResult(menuItem);
        }
        catch (JsonException)
        {
            return new BadRequestObjectResult(new
            {
                error = "The request body contains invalid JSON."
            });
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(
                "Update menu item request was cancelled.");

            return new StatusCodeResult(StatusCodes.Status499ClientClosedRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error while updating menu item {MenuItemId}.",
                id);

            return new StatusCodeResult(
                StatusCodes.Status500InternalServerError);
        }
    }

    // DELETE /api/menu/{category}/{id}
    [Function("DeleteMenuItem")]
    public async Task<IActionResult> DeleteMenuItem(
        [HttpTrigger(
            AuthorizationLevel.Anonymous,
            "delete",
            Route = "menu/{category}/{id}")]
        HttpRequest req,
        string category,
        string id)
    {
        var cancellationToken = req.HttpContext.RequestAborted;

        try
        {
            if (string.IsNullOrWhiteSpace(category) ||
                string.IsNullOrWhiteSpace(id))
            {
                return new BadRequestObjectResult(new
                {
                    error = "Category and ID are required."
                });
            }

            await _menuItemService.InitializeAsync(cancellationToken);

            var deleted = await _menuItemService.DeleteAsync(
                category.Trim(),
                id.Trim(),
                cancellationToken);

            if (!deleted)
            {
                return new NotFoundObjectResult(new
                {
                    error = "Menu item not found."
                });
            }

            _logger.LogInformation(
                "Deleted menu item {MenuItemId} from category {Category}.",
                id,
                category);

            return new OkObjectResult(new
            {
                message = "Menu item deleted successfully."
            });
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(
                "Delete menu item request was cancelled.");

            return new StatusCodeResult(StatusCodes.Status499ClientClosedRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error while deleting menu item {MenuItemId}.",
                id);

            return new StatusCodeResult(
                StatusCodes.Status500InternalServerError);
        }
    }
}