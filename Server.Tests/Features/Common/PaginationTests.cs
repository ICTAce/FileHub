// Licensed to ICTAce under the MIT license.

using SampleModuleHandlers = ICTAce.FileHub.Features.SampleModule;
using CategoryHandlers = ICTAce.FileHub.Features.Categories;
using static ICTAce.FileHub.Server.Tests.Helpers.SampleModuleTestHelpers;

namespace ICTAce.FileHub.Server.Tests.Features.Common;

/// <summary>
/// Tests for pagination functionality across all list operations.
/// </summary>
public class PaginationTests : HandlerTestBase
{
    [Test]
    public async Task List_FirstPage_ReturnsCorrectItems()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync().ConfigureAwait(false);
        await Helpers.SampleModuleTestHelpers.SeedQueryDataAsync(options,
            CreateTestEntity(id: 1, moduleId: 1, name: "Item 1"),
            CreateTestEntity(id: 2, moduleId: 1, name: "Item 2"),
            CreateTestEntity(id: 3, moduleId: 1, name: "Item 3"),
            CreateTestEntity(id: 4, moduleId: 1, name: "Item 4"),
            CreateTestEntity(id: 5, moduleId: 1, name: "Item 5")).ConfigureAwait(false);

        var handler = new SampleModuleHandlers.ListHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        // Act
        var result = await handler.Handle(new SampleModuleHandlers.ListSampleModuleRequest
        {
            ModuleId = 1,
            PageNumber = 1,
            PageSize = 3
        }, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.PageNumber).IsEqualTo(1);
        await Assert.That(result.PageSize).IsEqualTo(3);
        await Assert.That(result.TotalCount).IsEqualTo(5);
        await Assert.That(result.Items.Count()).IsEqualTo(3);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task List_LastPageWithFewerItems_ReturnsRemainingItems()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync().ConfigureAwait(false);
        await Helpers.SampleModuleTestHelpers.SeedQueryDataAsync(options,
            CreateTestEntity(id: 1, moduleId: 1, name: "Item 1"),
            CreateTestEntity(id: 2, moduleId: 1, name: "Item 2"),
            CreateTestEntity(id: 3, moduleId: 1, name: "Item 3"),
            CreateTestEntity(id: 4, moduleId: 1, name: "Item 4"),
            CreateTestEntity(id: 5, moduleId: 1, name: "Item 5")).ConfigureAwait(false);

        var handler = new SampleModuleHandlers.ListHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        // Act - Request page 2 with page size 3 (should return 2 items)
        var result = await handler.Handle(new SampleModuleHandlers.ListSampleModuleRequest
        {
            ModuleId = 1,
            PageNumber = 2,
            PageSize = 3
        }, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.PageNumber).IsEqualTo(2);
        await Assert.That(result.PageSize).IsEqualTo(3);
        await Assert.That(result.TotalCount).IsEqualTo(5);
        await Assert.That(result.Items.Count()).IsEqualTo(2);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task List_EmptyResultSet_ReturnsEmptyList()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync().ConfigureAwait(false);
        var handler = new SampleModuleHandlers.ListHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        // Act
        var result = await handler.Handle(new SampleModuleHandlers.ListSampleModuleRequest
        {
            ModuleId = 1,
            PageNumber = 1,
            PageSize = 10
        }, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.TotalCount).IsEqualTo(0);
        await Assert.That(result.Items.Count()).IsEqualTo(0);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task List_PageSizeGreaterThanTotalCount_ReturnsAllItems()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync().ConfigureAwait(false);
        await Helpers.SampleModuleTestHelpers.SeedQueryDataAsync(options,
            CreateTestEntity(id: 1, moduleId: 1, name: "Item 1"),
            CreateTestEntity(id: 2, moduleId: 1, name: "Item 2")).ConfigureAwait(false);

        var handler = new SampleModuleHandlers.ListHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        // Act
        var result = await handler.Handle(new SampleModuleHandlers.ListSampleModuleRequest
        {
            ModuleId = 1,
            PageNumber = 1,
            PageSize = 100
        }, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.TotalCount).IsEqualTo(2);
        await Assert.That(result.Items.Count()).IsEqualTo(2);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task List_OrderingConsistentAcrossPages()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync().ConfigureAwait(false);
        await Helpers.SampleModuleTestHelpers.SeedQueryDataAsync(options,
            CreateTestEntity(id: 1, moduleId: 1, name: "Zebra"),
            CreateTestEntity(id: 2, moduleId: 1, name: "Apple"),
            CreateTestEntity(id: 3, moduleId: 1, name: "Mango"),
            CreateTestEntity(id: 4, moduleId: 1, name: "Banana"),
            CreateTestEntity(id: 5, moduleId: 1, name: "Orange")).ConfigureAwait(false);

        var handler = new SampleModuleHandlers.ListHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        // Act - Get Page 1
        var page1 = await handler.Handle(new SampleModuleHandlers.ListSampleModuleRequest
        {
            ModuleId = 1,
            PageNumber = 1,
            PageSize = 2
        }, CancellationToken.None).ConfigureAwait(false);

        // Act - Get Page 2
        var page2 = await handler.Handle(new SampleModuleHandlers.ListSampleModuleRequest
        {
            ModuleId = 1,
            PageNumber = 2,
            PageSize = 2
        }, CancellationToken.None).ConfigureAwait(false);

        // Act - Get Page 3
        var page3 = await handler.Handle(new SampleModuleHandlers.ListSampleModuleRequest
        {
            ModuleId = 1,
            PageNumber = 3,
            PageSize = 2
        }, CancellationToken.None).ConfigureAwait(false);

        // Assert - Alphabetical ordering maintained across pages
        var page1List = page1!.Items.ToList();
        var page2List = page2!.Items.ToList();
        var page3List = page3!.Items.ToList();
        
        await Assert.That(page1List[0].Name).IsEqualTo("Apple");
        await Assert.That(page1List[1].Name).IsEqualTo("Banana");
        await Assert.That(page2List[0].Name).IsEqualTo("Mango");
        await Assert.That(page2List[1].Name).IsEqualTo("Orange");
        await Assert.That(page3List[0].Name).IsEqualTo("Zebra");

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task List_TotalCountAccurate()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync().ConfigureAwait(false);
        await Helpers.SampleModuleTestHelpers.SeedQueryDataAsync(options,
            Enumerable.Range(1, 50).Select(i =>
                CreateTestEntity(id: i, moduleId: 1, name: $"Item {i}")).ToArray()).ConfigureAwait(false);

        var handler = new SampleModuleHandlers.ListHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        // Act
        var result = await handler.Handle(new SampleModuleHandlers.ListSampleModuleRequest
        {
            ModuleId = 1,
            PageNumber = 1,
            PageSize = 10
        }, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.TotalCount).IsEqualTo(50);
        await Assert.That(result.Items.Count()).IsEqualTo(10);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task CategoryList_OrderedByViewOrderThenName()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync().ConfigureAwait(false);
        await Helpers.CategoryTestHelpers.SeedQueryDataAsync(options,
            Helpers.CategoryTestHelpers.CreateTestEntity(id: 1, name: "Zebra", viewOrder: 2),
            Helpers.CategoryTestHelpers.CreateTestEntity(id: 2, name: "Apple", viewOrder: 1),
            Helpers.CategoryTestHelpers.CreateTestEntity(id: 3, name: "Banana", viewOrder: 1)).ConfigureAwait(false);

        var handler = new CategoryHandlers.ListHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        // Act
        var result = await handler.Handle(new CategoryHandlers.ListCategoryRequest
        {
            ModuleId = 1,
            PageNumber = 1,
            PageSize = 10
        }, CancellationToken.None).ConfigureAwait(false);

        // Assert - ViewOrder 1 items first (Apple, Banana), then ViewOrder 2 (Zebra)
        var items = result!.Items.ToList();
        await Assert.That(result).IsNotNull();
        await Assert.That(items[0].Name).IsEqualTo("Apple");
        await Assert.That(items[1].Name).IsEqualTo("Banana");
        await Assert.That(items[2].Name).IsEqualTo("Zebra");

        await connection.CloseAsync().ConfigureAwait(false);
    }
}
