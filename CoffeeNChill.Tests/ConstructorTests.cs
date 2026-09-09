using CoffeeNChill.Services;

namespace CoffeeNChill.Tests;

public class ConstructorTests
{
    [Fact]
    public void NullTableServiceClient_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => new MenuItemService(null!));
    }
}