using Azure.Data.Tables;
using CoffeeNChill.Services;

namespace CoffeeNChill.Tests;

public class GetByCategoryTests
{
    [Fact]
    public async Task GetByCategoryAsync_NullCategory_ThrowsArgumentNullException()
    {
        // Arrange
        var tableServiceClient = new TableServiceClient(
            "UseDevelopmentStorage=true");

        var service = new MenuItemService(tableServiceClient);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => service.GetByCategoryAsync(null!));
    }

    [Fact]
    public async Task GetByCategoryAsync_EmptyCategory_ThrowsArgumentException()
    {
        // Arrange
        var tableServiceClient = new TableServiceClient(
            "UseDevelopmentStorage=true");

        var service = new MenuItemService(tableServiceClient);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => service.GetByCategoryAsync(""));
    }

    [Fact]
    public async Task GetByCategoryAsync_WhitespaceCategory_ThrowsArgumentException()
    {
        // Arrange
        var tableServiceClient = new TableServiceClient(
            "UseDevelopmentStorage=true");

        var service = new MenuItemService(tableServiceClient);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => service.GetByCategoryAsync("   "));
    }
}