using Azure;
using Azure.Data.Tables;
using CoffeeNChill.Models;

namespace CoffeeNChill.Services;

public class MenuItemService
{
    private const string TableName = "MenuItems";

    private readonly TableClient _tableClient;

    public MenuItemService(TableServiceClient tableServiceClient)
    {
        ArgumentNullException.ThrowIfNull(tableServiceClient);

        _tableClient = tableServiceClient.GetTableClient(TableName);
    }

    public Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        return _tableClient.CreateIfNotExistsAsync(cancellationToken);
    }

    public async Task<MenuItem> CreateAsync(
        MenuItem menuItem,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(menuItem);

        await _tableClient.AddEntityAsync(
            menuItem,
            cancellationToken);

        return menuItem;
    }

    public async Task<IReadOnlyList<MenuItem>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var menuItems = new List<MenuItem>();

        await foreach (var menuItem in _tableClient
            .QueryAsync<MenuItem>(cancellationToken: cancellationToken)
            .WithCancellation(cancellationToken))
        {
            menuItems.Add(menuItem);
        }

        return menuItems;
    }

    public async Task<IReadOnlyList<MenuItem>> GetByCategoryAsync(
        string category,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(category);

        var menuItems = new List<MenuItem>();

        var filter = TableClient.CreateQueryFilter(
            $"PartitionKey eq {category}");

        await foreach (var menuItem in _tableClient
            .QueryAsync<MenuItem>(
                filter: filter,
                cancellationToken: cancellationToken)
            .WithCancellation(cancellationToken))
        {
            menuItems.Add(menuItem);
        }

        return menuItems;
    }

    public async Task<MenuItem?> GetByIdAsync(
        string category,
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(category);
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        try
        {
            var response = await _tableClient.GetEntityAsync<MenuItem>(
                category,
                id,
                cancellationToken: cancellationToken);

            return response.Value;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public async Task<bool> UpdateAsync(
        MenuItem menuItem,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(menuItem);

        try
        {
            await _tableClient.UpdateEntityAsync(
                menuItem,
                ETag.All,
                TableUpdateMode.Replace,
                cancellationToken);

            return true;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(
        string category,
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(category);
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        try
        {
            await _tableClient.DeleteEntityAsync(
                category,
                id,
                ETag.All,
                cancellationToken);

            return true;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return false;
        }
    }
}