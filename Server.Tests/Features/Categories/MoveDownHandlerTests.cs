// Licensed to ICTAce under the MIT license.

using CategoryHandlers = ICTAce.FileHub.Features.Categories;
using static ICTAce.FileHub.Server.Tests.Helpers.CategoryTestHelpers;

namespace ICTAce.FileHub.Server.Tests.Features.Categories;

public class MoveDownHandlerTests : HandlerTestBase
{
    [Test]
    public async Task Handle_WithValidRequest_SwapsViewOrderWithNextSibling()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        await SeedCommandDataAsync(options,
            CreateTestEntity(id: 1, name: "Category 1", viewOrder: 1, parentId: 0),
            CreateTestEntity(id: 2, name: "Category 2", viewOrder: 2, parentId: 0)).ConfigureAwait(false);

        var handler = new CategoryHandlers.MoveDownHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new CategoryHandlers.MoveDownCategoryRequest
        {
            Id = 1,
            ModuleId = 1,
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsEqualTo(1);

        var category1 = await GetFromCommandDbAsync(options, 1).ConfigureAwait(false);
        var category2 = await GetFromCommandDbAsync(options, 2).ConfigureAwait(false);

        await Assert.That(category1!.ViewOrder).IsEqualTo(2);
        await Assert.That(category2!.ViewOrder).IsEqualTo(1);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Handle_WithLastItem_ReturnsSuccessWithoutChanges()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        await SeedCommandDataAsync(options,
            CreateTestEntity(id: 1, name: "Category 1", viewOrder: 1, parentId: 0),
            CreateTestEntity(id: 2, name: "Category 2", viewOrder: 2, parentId: 0)).ConfigureAwait(false);

        var handler = new CategoryHandlers.MoveDownHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new CategoryHandlers.MoveDownCategoryRequest
        {
            Id = 2,
            ModuleId = 1,
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsEqualTo(2);

        var category1 = await GetFromCommandDbAsync(options, 1).ConfigureAwait(false);
        var category2 = await GetFromCommandDbAsync(options, 2).ConfigureAwait(false);

        await Assert.That(category1!.ViewOrder).IsEqualTo(1);
        await Assert.That(category2!.ViewOrder).IsEqualTo(2);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Handle_WithInvalidId_ReturnsMinusOne()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        await SeedCommandDataAsync(options,
            CreateTestEntity(id: 1, name: "Category 1", viewOrder: 1)).ConfigureAwait(false);

        var handler = new CategoryHandlers.MoveDownHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new CategoryHandlers.MoveDownCategoryRequest
        {
            Id = 999,
            ModuleId = 1,
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsEqualTo(-1);
        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Handle_WithUnauthorizedUser_ReturnsMinusOne()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        await SeedCommandDataAsync(options,
            CreateTestEntity(id: 1, name: "Category 1", viewOrder: 1),
            CreateTestEntity(id: 2, name: "Category 2", viewOrder: 2)).ConfigureAwait(false);

        var handler = new CategoryHandlers.MoveDownHandler(
            CreateCommandHandlerServices(options, isAuthorized: false));

        var request = new CategoryHandlers.MoveDownCategoryRequest
        {
            Id = 1,
            ModuleId = 1,
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsEqualTo(-1);

        var category1 = await GetFromCommandDbAsync(options, 1).ConfigureAwait(false);
        var category2 = await GetFromCommandDbAsync(options, 2).ConfigureAwait(false);

        await Assert.That(category1!.ViewOrder).IsEqualTo(1);
        await Assert.That(category2!.ViewOrder).IsEqualTo(2);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Handle_WithHierarchy_OnlySwapsWithinSameParent()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        await SeedCommandDataAsync(options,
            CreateTestEntity(id: 1, name: "Parent 1", viewOrder: 1, parentId: 0),
            CreateTestEntity(id: 2, name: "Child 1-1", viewOrder: 2, parentId: 1),
            CreateTestEntity(id: 3, name: "Child 1-2", viewOrder: 3, parentId: 1),
            CreateTestEntity(id: 4, name: "Parent 2", viewOrder: 4, parentId: 0)).ConfigureAwait(false);

        var handler = new CategoryHandlers.MoveDownHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new CategoryHandlers.MoveDownCategoryRequest
        {
            Id = 2,
            ModuleId = 1,
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsEqualTo(2);

        var child1 = await GetFromCommandDbAsync(options, 2).ConfigureAwait(false);
        var child2 = await GetFromCommandDbAsync(options, 3).ConfigureAwait(false);

        await Assert.That(child1!.ViewOrder).IsEqualTo(3);
        await Assert.That(child2!.ViewOrder).IsEqualTo(2);

        var parent1 = await GetFromCommandDbAsync(options, 1).ConfigureAwait(false);
        var parent2 = await GetFromCommandDbAsync(options, 4).ConfigureAwait(false);

        await Assert.That(parent1!.ViewOrder).IsEqualTo(1);
        await Assert.That(parent2!.ViewOrder).IsEqualTo(4);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Handle_WithMultipleCategories_SwapsOnlyWithImmediateNext()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        await SeedCommandDataAsync(options,
            CreateTestEntity(id: 1, name: "Category 1", viewOrder: 1, parentId: 0),
            CreateTestEntity(id: 2, name: "Category 2", viewOrder: 2, parentId: 0),
            CreateTestEntity(id: 3, name: "Category 3", viewOrder: 3, parentId: 0),
            CreateTestEntity(id: 4, name: "Category 4", viewOrder: 4, parentId: 0)).ConfigureAwait(false);

        var handler = new CategoryHandlers.MoveDownHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new CategoryHandlers.MoveDownCategoryRequest
        {
            Id = 2,
            ModuleId = 1,
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsEqualTo(2);

        var category1 = await GetFromCommandDbAsync(options, 1).ConfigureAwait(false);
        var category2 = await GetFromCommandDbAsync(options, 2).ConfigureAwait(false);
        var category3 = await GetFromCommandDbAsync(options, 3).ConfigureAwait(false);
        var category4 = await GetFromCommandDbAsync(options, 4).ConfigureAwait(false);

        await Assert.That(category1!.ViewOrder).IsEqualTo(1);
        await Assert.That(category2!.ViewOrder).IsEqualTo(3);
        await Assert.That(category3!.ViewOrder).IsEqualTo(2);
        await Assert.That(category4!.ViewOrder).IsEqualTo(4);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Handle_WithNonSequentialViewOrders_SwapsCorrectly()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        await SeedCommandDataAsync(options,
            CreateTestEntity(id: 1, name: "Category 1", viewOrder: 10, parentId: 0),
            CreateTestEntity(id: 2, name: "Category 2", viewOrder: 20, parentId: 0),
            CreateTestEntity(id: 3, name: "Category 3", viewOrder: 30, parentId: 0)).ConfigureAwait(false);

        var handler = new CategoryHandlers.MoveDownHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new CategoryHandlers.MoveDownCategoryRequest
        {
            Id = 1,
            ModuleId = 1,
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsEqualTo(1);

        var category1 = await GetFromCommandDbAsync(options, 1).ConfigureAwait(false);
        var category2 = await GetFromCommandDbAsync(options, 2).ConfigureAwait(false);

        await Assert.That(category1!.ViewOrder).IsEqualTo(20);
        await Assert.That(category2!.ViewOrder).IsEqualTo(10);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Handle_WithChildAtBottomOfParent_ReturnsSuccessWithoutChanges()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        await SeedCommandDataAsync(options,
            CreateTestEntity(id: 1, name: "Parent", viewOrder: 1, parentId: 0),
            CreateTestEntity(id: 2, name: "Child 1", viewOrder: 2, parentId: 1),
            CreateTestEntity(id: 3, name: "Child 2", viewOrder: 3, parentId: 1)).ConfigureAwait(false);

        var handler = new CategoryHandlers.MoveDownHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new CategoryHandlers.MoveDownCategoryRequest
        {
            Id = 3,
            ModuleId = 1,
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsEqualTo(3);

        var child1 = await GetFromCommandDbAsync(options, 2).ConfigureAwait(false);
        var child2 = await GetFromCommandDbAsync(options, 3).ConfigureAwait(false);

        await Assert.That(child1!.ViewOrder).IsEqualTo(2);
        await Assert.That(child2!.ViewOrder).IsEqualTo(3);

        await connection.CloseAsync().ConfigureAwait(false);
    }
}
