// Licensed to ICTAce under the MIT license.

using CategoryHandlers = ICTAce.FileHub.Features.Categories;
using static ICTAce.FileHub.Server.Tests.Helpers.CategoryTestHelpers;

namespace ICTAce.FileHub.Server.Tests.Features.Categories;

public class UpdateHandlerTests : HandlerTestBase
{
    [Test]
    public async Task Handle_WithValidRequest_UpdatesCategory()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        await SeedCommandDataAsync(options, CreateTestEntity(id: 1, name: "Original Name"));

        var handler = new CategoryHandlers.UpdateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new CategoryHandlers.UpdateCategoryRequest
        {
            Id = 1,
            ModuleId = 1,
            Name = "Updated Name",
            ViewOrder = 5,
            ParentId = 0
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsEqualTo(1);

        var entity = await GetFromCommandDbAsync(options, 1);
        await Assert.That(entity).IsNotNull();
        await Assert.That(entity!.Name).IsEqualTo("Updated Name");
        await Assert.That(entity.ViewOrder).IsEqualTo(5);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Handle_WithInvalidId_ReturnsMinusOne()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        await SeedCommandDataAsync(options, CreateTestEntity(id: 1));

        var handler = new CategoryHandlers.UpdateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new CategoryHandlers.UpdateCategoryRequest
        {
            Id = 999,
            ModuleId = 1,
            Name = "Updated Name",
            ViewOrder = 1,
            ParentId = 0
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
        await SeedCommandDataAsync(options, CreateTestEntity(id: 1, name: "Original Name"));

        var handler = new CategoryHandlers.UpdateHandler(
            CreateCommandHandlerServices(options, isAuthorized: false));

        var request = new CategoryHandlers.UpdateCategoryRequest
        {
            Id = 1,
            ModuleId = 1,
            Name = "Updated Name",
            ViewOrder = 1,
            ParentId = 0
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsEqualTo(-1);

        var entity = await GetFromCommandDbAsync(options, 1);
        await Assert.That(entity!.Name).IsEqualTo("Original Name");

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Handle_UpdatesParentId_Successfully()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        await SeedCommandDataAsync(options, 
            CreateTestEntity(id: 1, name: "Parent Category", parentId: 0),
            CreateTestEntity(id: 2, name: "Child Category", parentId: 0)).ConfigureAwait(false);

        var handler = new CategoryHandlers.UpdateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new CategoryHandlers.UpdateCategoryRequest
        {
            Id = 2,
            ModuleId = 1,
            Name = "Child Category",
            ViewOrder = 1,
            ParentId = 1
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsEqualTo(2);

        var entity = await GetFromCommandDbAsync(options, 2).ConfigureAwait(false);
        await Assert.That(entity).IsNotNull();
        await Assert.That(entity!.ParentId).IsEqualTo(1);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Handle_UpdatesViewOrder_Successfully()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        await SeedCommandDataAsync(options, CreateTestEntity(id: 1, viewOrder: 1)).ConfigureAwait(false);

        var handler = new CategoryHandlers.UpdateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new CategoryHandlers.UpdateCategoryRequest
        {
            Id = 1,
            ModuleId = 1,
            Name = "Test Category",
            ViewOrder = 99,
            ParentId = 0
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsEqualTo(1);

        var entity = await GetFromCommandDbAsync(options, 1).ConfigureAwait(false);
        await Assert.That(entity!.ViewOrder).IsEqualTo(99);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Handle_UpdatesOnlyName_KeepsOtherFieldsIntact()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        await SeedCommandDataAsync(options, 
            CreateTestEntity(id: 1, name: "Original", viewOrder: 5, parentId: 0)).ConfigureAwait(false);

        var handler = new CategoryHandlers.UpdateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new CategoryHandlers.UpdateCategoryRequest
        {
            Id = 1,
            ModuleId = 1,
            Name = "New Name Only",
            ViewOrder = 5,
            ParentId = 0
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsEqualTo(1);

        var entity = await GetFromCommandDbAsync(options, 1).ConfigureAwait(false);
        await Assert.That(entity!.Name).IsEqualTo("New Name Only");
        await Assert.That(entity.ViewOrder).IsEqualTo(5);
        await Assert.That(entity.ParentId).IsEqualTo(0);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Handle_WithMultipleUpdates_AllSucceed()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        await SeedCommandDataAsync(options,
            CreateTestEntity(id: 1, name: "Category 1"),
            CreateTestEntity(id: 2, name: "Category 2")).ConfigureAwait(false);

        var handler = new CategoryHandlers.UpdateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        // Act
        var result1 = await handler.Handle(new CategoryHandlers.UpdateCategoryRequest
        {
            Id = 1,
            ModuleId = 1,
            Name = "Updated 1",
            ViewOrder = 1,
            ParentId = 0
        }, CancellationToken.None).ConfigureAwait(false);

        var result2 = await handler.Handle(new CategoryHandlers.UpdateCategoryRequest
        {
            Id = 2,
            ModuleId = 1,
            Name = "Updated 2",
            ViewOrder = 2,
            ParentId = 0
        }, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result1).IsEqualTo(1);
        await Assert.That(result2).IsEqualTo(2);

        var entity1 = await GetFromCommandDbAsync(options, 1).ConfigureAwait(false);
        var entity2 = await GetFromCommandDbAsync(options, 2).ConfigureAwait(false);

        await Assert.That(entity1!.Name).IsEqualTo("Updated 1");
        await Assert.That(entity2!.Name).IsEqualTo("Updated 2");

        await connection.CloseAsync().ConfigureAwait(false);
    }
}
