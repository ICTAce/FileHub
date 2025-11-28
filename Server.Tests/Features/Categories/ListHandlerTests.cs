// Licensed to ICTAce under the MIT license.

using CategoryHandlers = ICTAce.FileHub.Features.Categories;
using static ICTAce.FileHub.Server.Tests.Helpers.CategoryTestHelpers;

namespace ICTAce.FileHub.Server.Tests.Features.Categories;

public class ListHandlerTests : HandlerTestBase
{
    [Test]
    public async Task Handle_WithValidRequest_ReturnsPaginatedCategories()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync().ConfigureAwait(false);
        await SeedQueryDataAsync(options,
            CreateTestEntity(id: 1, name: "Category 1", viewOrder: 1),
            CreateTestEntity(id: 2, name: "Category 2", viewOrder: 2),
            CreateTestEntity(id: 3, name: "Category 3", viewOrder: 3)).ConfigureAwait(false);

        var handler = new CategoryHandlers.ListHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        var request = new CategoryHandlers.ListCategoryRequest { ModuleId = 1, PageNumber = 1, PageSize = 10 };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Items).HasCount().EqualTo(3);
        await Assert.That(result.TotalCount).IsEqualTo(3);
        await Assert.That(result.PageNumber).IsEqualTo(1);
        await Assert.That(result.PageSize).IsEqualTo(10);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Handle_WithUnauthorizedUser_ReturnsNull()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync().ConfigureAwait(false);
        await SeedQueryDataAsync(options, CreateTestEntity()).ConfigureAwait(false);

        var handler = new CategoryHandlers.ListHandler(
            CreateQueryHandlerServices(options, isAuthorized: false));

        var request = new CategoryHandlers.ListCategoryRequest { ModuleId = 1, PageNumber = 1, PageSize = 10 };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsNull();
        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync().ConfigureAwait(false);
        
        for (int i = 1; i <= 25; i++)
        {
            await SeedQueryDataAsync(options, CreateTestEntity(id: i, name: $"Category {i}", viewOrder: i)).ConfigureAwait(false);
        }

        var handler = new CategoryHandlers.ListHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        var request = new CategoryHandlers.ListCategoryRequest { ModuleId = 1, PageNumber = 2, PageSize = 10 };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Items).HasCount().EqualTo(10);
        await Assert.That(result.TotalCount).IsEqualTo(25);
        await Assert.That(result.PageNumber).IsEqualTo(2);
        await Assert.That(result.Items.First().Name).IsEqualTo("Category 11");
        await Assert.That(result.Items.Last().Name).IsEqualTo("Category 20");

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Handle_WithEmptyResult_ReturnsEmptyList()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync().ConfigureAwait(false);

        var handler = new CategoryHandlers.ListHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        var request = new CategoryHandlers.ListCategoryRequest { ModuleId = 1, PageNumber = 1, PageSize = 10 };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Items).IsEmpty();
        await Assert.That(result.TotalCount).IsEqualTo(0);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Handle_OrdersByViewOrderThenName()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync().ConfigureAwait(false);
        await SeedQueryDataAsync(options,
            CreateTestEntity(id: 1, name: "Zebra", viewOrder: 2),
            CreateTestEntity(id: 2, name: "Apple", viewOrder: 1),
            CreateTestEntity(id: 3, name: "Banana", viewOrder: 1)).ConfigureAwait(false);

        var handler = new CategoryHandlers.ListHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        var request = new CategoryHandlers.ListCategoryRequest { ModuleId = 1, PageNumber = 1, PageSize = 10 };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Items).HasCount().EqualTo(3);

        // ViewOrder 1 first (Apple, Banana alphabetically), then ViewOrder 2 (Zebra)
        var items = result.Items.ToList();
        await Assert.That(items[0].Name).IsEqualTo("Apple");
        await Assert.That(items[1].Name).IsEqualTo("Banana");
        await Assert.That(items[2].Name).IsEqualTo("Zebra");

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Handle_FiltersOnlyByModuleId()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync().ConfigureAwait(false);
        await SeedQueryDataAsync(options,
            CreateTestEntity(id: 1, moduleId: 1, name: "Module 1 Cat 1"),
            CreateTestEntity(id: 2, moduleId: 1, name: "Module 1 Cat 2"),
            CreateTestEntity(id: 3, moduleId: 2, name: "Module 2 Cat 1")).ConfigureAwait(false);

        var handler = new CategoryHandlers.ListHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        var request = new CategoryHandlers.ListCategoryRequest { ModuleId = 1, PageNumber = 1, PageSize = 10 };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Items).HasCount().EqualTo(2);
        await Assert.That(result.TotalCount).IsEqualTo(2);
        await Assert.That(result.Items.All(c => c.Name.StartsWith("Module 1", StringComparison.Ordinal))).IsTrue();

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Handle_WithLastPagePartiallyFilled_ReturnsRemainingItems()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync().ConfigureAwait(false);
        
        for (int i = 1; i <= 15; i++)
        {
            await SeedQueryDataAsync(options, CreateTestEntity(id: i, name: $"Category {i}", viewOrder: i)).ConfigureAwait(false);
        }

        var handler = new CategoryHandlers.ListHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        var request = new CategoryHandlers.ListCategoryRequest { ModuleId = 1, PageNumber = 2, PageSize = 10 };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Items).HasCount().EqualTo(5);
        await Assert.That(result.TotalCount).IsEqualTo(15);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Handle_IncludesParentIdInResponse()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync().ConfigureAwait(false);
        await SeedQueryDataAsync(options,
            CreateTestEntity(id: 1, name: "Parent", parentId: 0),
            CreateTestEntity(id: 2, name: "Child", parentId: 1)).ConfigureAwait(false);

        var handler = new CategoryHandlers.ListHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        var request = new CategoryHandlers.ListCategoryRequest { ModuleId = 1, PageNumber = 1, PageSize = 10 };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Items).HasCount().EqualTo(2);
        
        var parent = result.Items.First(c => string.Equals(c.Name, "Parent", StringComparison.Ordinal));
        var child = result.Items.First(c => string.Equals(c.Name, "Child", StringComparison.Ordinal));

        await Assert.That(parent.ParentId).IsEqualTo(0);
        await Assert.That(child.ParentId).IsEqualTo(1);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Handle_WithPageSizeOne_ReturnsSingleItem()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync().ConfigureAwait(false);
        await SeedQueryDataAsync(options,
            CreateTestEntity(id: 1, name: "Category 1", viewOrder: 1),
            CreateTestEntity(id: 2, name: "Category 2", viewOrder: 2)).ConfigureAwait(false);

        var handler = new CategoryHandlers.ListHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        var request = new CategoryHandlers.ListCategoryRequest { ModuleId = 1, PageNumber = 1, PageSize = 1 };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Items).HasCount().EqualTo(1);
        await Assert.That(result.TotalCount).IsEqualTo(2);
        await Assert.That(result.Items.First().Name).IsEqualTo("Category 1");

        await connection.CloseAsync().ConfigureAwait(false);
    }
}
