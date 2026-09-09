using Azure.Data.Tables;
using CoffeeNChill.Services;

namespace CoffeeNChill.Tests;

public class DeleteMenuItemTests
{
    [Fact]
    public async Task DeleteAsync_NullCategory_ThrowsArgumentNullException()
    {
        // Arrange
        var tableServiceClient = new TableServiceClient(
            "UseDevelopmentStorage=true");

        var service = new MenuItemService(tableServiceClient);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => service.DeleteAsync(null!, "123"));
    }

    [Fact]
    public async Task DeleteAsync_NullId_ThrowsArgumentNullException()
    {
        // Arrange
        var tableServiceClient = new TableServiceClient(
            "UseDevelopmentStorage=true");

        var service = new MenuItemService(tableServiceClient);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => service.DeleteAsync("Coffee", null!));
    }

    [Fact]
    public async Task DeleteAsync_EmptyId_ThrowsArgumentException()
    {
        // Arrange
        var tableServiceClient = new TableServiceClient(
            "UseDevelopmentStorage=true");

        var service = new MenuItemService(tableServiceClient);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => service.DeleteAsync("Coffee", ""));
    }
}