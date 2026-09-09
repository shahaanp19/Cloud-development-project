using Azure.Data.Tables;
using CoffeeNChill.Services;

namespace CoffeeNChill.Tests;

public class CreateMenuItemTests
{
    [Fact]
    public async Task CreateAsync_NullMenuItem_ThrowsArgumentNullException()
    {
        // Arrange
        var tableServiceClient = new TableServiceClient(
            "UseDevelopmentStorage=true");

        var service = new MenuItemService(tableServiceClient);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => service.CreateAsync(null!));
    }
}