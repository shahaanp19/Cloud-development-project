using Azure.Data.Tables;
using CoffeeNChill.Services;

namespace CoffeeNChill.Tests;

public class GetByIdTests
{
    [Fact]
    public async Task GetByIdAsync_NullCategory_ThrowsArgumentNullException()
    {
        // Arrange
        var tableServiceClient = new TableServiceClient(
            "UseDevelopmentStorage=true");

        var service = new MenuItemService(tableServiceClient);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => service.GetByIdAsync(null!, "123"));
    }

    [Fact]
    public async Task GetByIdAsync_NullId_ThrowsArgumentNullException()
    {
        // Arrange
        var tableServiceClient = new TableServiceClient(
            "UseDevelopmentStorage=true");

        var service = new MenuItemService(tableServiceClient);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => service.GetByIdAsync("Coffee", null!));
    }

    [Fact]
    public async Task GetByIdAsync_EmptyId_ThrowsArgumentException()
    {
        // Arrange
        var tableServiceClient = new TableServiceClient(
            "UseDevelopmentStorage=true");

        var service = new MenuItemService(tableServiceClient);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => service.GetByIdAsync("Coffee", ""));
    }

    [Fact]
    public async Task GetByIdAsync_WhitespaceId_ThrowsArgumentException()
    {
        // Arrange
        var tableServiceClient = new TableServiceClient(
            "UseDevelopmentStorage=true");

        var service = new MenuItemService(tableServiceClient);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => service.GetByIdAsync("Coffee", "   "));
    }

    [Fact]
    public async Task GetByIdAsync_EmptyCategory_ThrowsArgumentException()
    {
        // Arrange
        var tableServiceClient = new TableServiceClient(
            "UseDevelopmentStorage=true");

        var service = new MenuItemService(tableServiceClient);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => service.GetByIdAsync("", "123"));
    }
}