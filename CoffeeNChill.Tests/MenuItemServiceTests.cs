using CoffeeNChill.Services;

namespace CoffeeNChill.Tests;

public class MenuItemServiceTests
{
    [Fact]
    public void Constructor_NullTableServiceClient_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () => new MenuItemService(null!));
    }
}