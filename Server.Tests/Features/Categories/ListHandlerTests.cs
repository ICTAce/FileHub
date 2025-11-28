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
        var (connection, options) = await CreateQueryDatabaseAsync();
        await SeedQueryDataAsync(options,
            CreateTestEntity(id: 1, name: "Category 1", viewOrder: 1),
            CreateTestEntity(id: 2, name: "Category 2", viewOrder: 2),
            CreateTestEntity(id: 3, name: "Category 3", viewOrder: 3));

        var handler = new CategoryHandlers.ListHandler(
            CreateMockQueryContextFactory(options),
            CreateMockUserPermissions(isAuthorized: true),
            CreateMockTenantManager(),
            CreateMockHttpContextAccessor(),
            CreateMockLogger());

        var request = new CategoryHandlers.ListCategoryRequest { ModuleId = 1, PageNumber = 1, PageSize = 10 };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Items).HasCount().EqualTo(3);
        await Assert.That(result.TotalCount).IsEqualTo(3);
        await Assert.That(result.PageNumber).IsEqualTo(1);
        await Assert.That(result.PageSize).IsEqualTo(10);

        connection.Close();
    }

    [Test]
    public async Task Handle_WithUnauthorizedUser_ReturnsNull()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync();
        await SeedQueryDataAsync(options, CreateTestEntity());

        var handler = new CategoryHandlers.ListHandler(
            CreateMockQueryContextFactory(options),
            CreateMockUserPermissions(isAuthorized: false),
            CreateMockTenantManager(),
            CreateMockHttpContextAccessor(),
            CreateMockLogger());

        var request = new CategoryHandlers.ListCategoryRequest { ModuleId = 1, PageNumber = 1, PageSize = 10 };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsNull();
        connection.Close();
    }

    [Test]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync();
        
        for (int i = 1; i <= 25; i++)
        {
            await SeedQueryDataAsync(options, CreateTestEntity(id: i, name: $"Category {i}", viewOrder: i));
        }

        var handler = new CategoryHandlers.ListHandler(
            CreateMockQueryContextFactory(options),
            CreateMockUserPermissions(isAuthorized: true),
            CreateMockTenantManager(),
            CreateMockHttpContextAccessor(),
            CreateMockLogger());

        var request = new CategoryHandlers.ListCategoryRequest { ModuleId = 1, PageNumber = 2, PageSize = 10 };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Items).HasCount().EqualTo(10);
        await Assert.That(result.TotalCount).IsEqualTo(25);
        await Assert.That(result.PageNumber).IsEqualTo(2);
        await Assert.That(result.Items.First().Name).IsEqualTo("Category 11");
        await Assert.That(result.Items.Last().Name).IsEqualTo("Category 20");

        connection.Close();
    }

    [Test]
    public async Task Handle_WithEmptyResult_ReturnsEmptyList()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync();

        var handler = new CategoryHandlers.ListHandler(
            CreateMockQueryContextFactory(options),
            CreateMockUserPermissions(isAuthorized: true),
            CreateMockTenantManager(),
            CreateMockHttpContextAccessor(),
            CreateMockLogger());

        var request = new CategoryHandlers.ListCategoryRequest { ModuleId = 1, PageNumber = 1, PageSize = 10 };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Items).IsEmpty();
        await Assert.That(result.TotalCount).IsEqualTo(0);

        connection.Close();
    }

    [Test]
    public async Task Handle_OrdersByViewOrderThenName()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync();
        await SeedQueryDataAsync(options,
            CreateTestEntity(id: 1, name: "Zebra", viewOrder: 2),
            CreateTestEntity(id: 2, name: "Apple", viewOrder: 1),
            CreateTestEntity(id: 3, name: "Banana", viewOrder: 1));

        var handler = new CategoryHandlers.ListHandler(
            CreateMockQueryContextFactory(options),
            CreateMockUserPermissions(isAuthorized: true),
            CreateMockTenantManager(),
            CreateMockHttpContextAccessor(),
            CreateMockLogger());

        var request = new CategoryHandlers.ListCategoryRequest { ModuleId = 1, PageNumber = 1, PageSize = 10 };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Items).HasCount().EqualTo(3);

        // ViewOrder 1 first (Apple, Banana alphabetically), then ViewOrder 2 (Zebra)
        var items = result.Items.ToList();
        await Assert.That(items[0].Name).IsEqualTo("Apple");
        await Assert.That(items[1].Name).IsEqualTo("Banana");
        await Assert.That(items[2].Name).IsEqualTo("Zebra");

        connection.Close();
    }

    [Test]
    public async Task Handle_FiltersOnlyByModuleId()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync();
        await SeedQueryDataAsync(options,
            CreateTestEntity(id: 1, moduleId: 1, name: "Module 1 Cat 1"),
            CreateTestEntity(id: 2, moduleId: 1, name: "Module 1 Cat 2"),
            CreateTestEntity(id: 3, moduleId: 2, name: "Module 2 Cat 1"));

        var handler = new CategoryHandlers.ListHandler(
            CreateMockQueryContextFactory(options),
            CreateMockUserPermissions(isAuthorized: true),
            CreateMockTenantManager(),
            CreateMockHttpContextAccessor(),
            CreateMockLogger());

        var request = new CategoryHandlers.ListCategoryRequest { ModuleId = 1, PageNumber = 1, PageSize = 10 };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Items).HasCount().EqualTo(2);
        await Assert.That(result.TotalCount).IsEqualTo(2);
        await Assert.That(result.Items.All(c => c.Name.StartsWith("Module 1"))).IsTrue();

        connection.Close();
    }

    [Test]
    public async Task Handle_WithLastPagePartiallyFilled_ReturnsRemainingItems()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync();
        
        for (int i = 1; i <= 15; i++)
        {
            await SeedQueryDataAsync(options, CreateTestEntity(id: i, name: $"Category {i}", viewOrder: i));
        }

        var handler = new CategoryHandlers.ListHandler(
            CreateMockQueryContextFactory(options),
            CreateMockUserPermissions(isAuthorized: true),
            CreateMockTenantManager(),
            CreateMockHttpContextAccessor(),
            CreateMockLogger());

        var request = new CategoryHandlers.ListCategoryRequest { ModuleId = 1, PageNumber = 2, PageSize = 10 };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Items).HasCount().EqualTo(5);
        await Assert.That(result.TotalCount).IsEqualTo(15);

        connection.Close();
    }

    [Test]
    public async Task Handle_IncludesParentIdInResponse()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync();
        await SeedQueryDataAsync(options,
            CreateTestEntity(id: 1, name: "Parent", parentId: 0),
            CreateTestEntity(id: 2, name: "Child", parentId: 1));

        var handler = new CategoryHandlers.ListHandler(
            CreateMockQueryContextFactory(options),
            CreateMockUserPermissions(isAuthorized: true),
            CreateMockTenantManager(),
            CreateMockHttpContextAccessor(),
            CreateMockLogger());

        var request = new CategoryHandlers.ListCategoryRequest { ModuleId = 1, PageNumber = 1, PageSize = 10 };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Items).HasCount().EqualTo(2);
        
        var parent = result.Items.First(c => c.Name == "Parent");
        var child = result.Items.First(c => c.Name == "Child");

        await Assert.That(parent.ParentId).IsEqualTo(0);
        await Assert.That(child.ParentId).IsEqualTo(1);

        connection.Close();
    }

    [Test]
    public async Task Handle_WithPageSizeOne_ReturnsSingleItem()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync();
        await SeedQueryDataAsync(options,
            CreateTestEntity(id: 1, name: "Category 1", viewOrder: 1),
            CreateTestEntity(id: 2, name: "Category 2", viewOrder: 2));

        var handler = new CategoryHandlers.ListHandler(
            CreateMockQueryContextFactory(options),
            CreateMockUserPermissions(isAuthorized: true),
            CreateMockTenantManager(),
            CreateMockHttpContextAccessor(),
            CreateMockLogger());

        var request = new CategoryHandlers.ListCategoryRequest { ModuleId = 1, PageNumber = 1, PageSize = 1 };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Items).HasCount().EqualTo(1);
        await Assert.That(result.TotalCount).IsEqualTo(2);
        await Assert.That(result.Items.First().Name).IsEqualTo("Category 1");

        connection.Close();
    }
}
