// Licensed to ICTAce under the MIT license.

using CategoryHandlers = ICTAce.FileHub.Features.Categories;
using static ICTAce.FileHub.Server.Tests.Helpers.CategoryTestHelpers;

namespace ICTAce.FileHub.Server.Tests.Features.Categories;

public class CreateHandlerTests : HandlerTestBase
{
    [Test]
    public async Task Handle_WithValidRequest_CreatesCategory()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);

        var handler = new CategoryHandlers.CreateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new CategoryHandlers.CreateCategoryRequest
        {
            ModuleId = 1,
            Name = "New Category",
            ViewOrder = 1,
            ParentId = null,
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsGreaterThan(0);

        var entity = await GetFromCommandDbAsync(options, result).ConfigureAwait(false);
        await Assert.That(entity).IsNotNull();
        await Assert.That(entity!.Name).IsEqualTo("New Category");
        await Assert.That(entity.ModuleId).IsEqualTo(1);
        await Assert.That(entity.ViewOrder).IsEqualTo(1);
        await Assert.That(entity.ParentId).IsNull();

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Handle_WithUnauthorizedUser_ReturnsMinusOne()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);

        var handler = new CategoryHandlers.CreateHandler(
            CreateCommandHandlerServices(options, isAuthorized: false));

        var request = new CategoryHandlers.CreateCategoryRequest
        {
            ModuleId = 1,
            Name = "New Category",
            ViewOrder = 1,
            ParentId = null,
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsEqualTo(-1);

        var count = await GetCountFromCommandDbAsync(options).ConfigureAwait(false);
        await Assert.That(count).IsEqualTo(0);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Handle_WithChildCategory_SetsCorrectParentId()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        await SeedCommandDataAsync(options, CreateTestEntity(id: 1, name: "Parent Category", parentId: 0)).ConfigureAwait(false);

        var handler = new CategoryHandlers.CreateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new CategoryHandlers.CreateCategoryRequest
        {
            ModuleId = 1,
            Name = "Child Category",
            ViewOrder = 2,
            ParentId = 1,
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsGreaterThan(0);

        var entity = await GetFromCommandDbAsync(options, result).ConfigureAwait(false);
        await Assert.That(entity).IsNotNull();
        await Assert.That(entity!.ParentId).IsEqualTo(1);
        await Assert.That(entity.Name).IsEqualTo("Child Category");

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Handle_WithMultipleCategories_CreatesAll()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);

        var handler = new CategoryHandlers.CreateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        // Act
        var id1 = await handler.Handle(new CategoryHandlers.CreateCategoryRequest
        {
            ModuleId = 1,
            Name = "Category 1",
            ViewOrder = 1,
            ParentId = null,
        }, CancellationToken.None).ConfigureAwait(false);

        var id2 = await handler.Handle(new CategoryHandlers.CreateCategoryRequest
        {
            ModuleId = 1,
            Name = "Category 2",
            ViewOrder = 2,
            ParentId = null,
        }, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(id1).IsGreaterThan(0);
        await Assert.That(id2).IsGreaterThan(0);
        await Assert.That(id1).IsNotEqualTo(id2);

        var count = await GetCountFromCommandDbAsync(options).ConfigureAwait(false);
        await Assert.That(count).IsEqualTo(2);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Handle_WithViewOrderZero_CreatesSuccessfully()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);

        var handler = new CategoryHandlers.CreateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new CategoryHandlers.CreateCategoryRequest
        {
            ModuleId = 1,
            Name = "Zero Order Category",
            ViewOrder = 0,
            ParentId = null,
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsGreaterThan(0);

        var entity = await GetFromCommandDbAsync(options, result).ConfigureAwait(false);
        await Assert.That(entity).IsNotNull();
        await Assert.That(entity!.ViewOrder).IsEqualTo(0);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Handle_WithDifferentModuleIds_CreatesInSeparateModules()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);

        var handler = new CategoryHandlers.CreateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        // Act
        var id1 = await handler.Handle(new CategoryHandlers.CreateCategoryRequest
        {
            ModuleId = 1,
            Name = "Module 1 Category",
            ViewOrder = 1,
            ParentId = null,
        }, CancellationToken.None).ConfigureAwait(false);

        var id2 = await handler.Handle(new CategoryHandlers.CreateCategoryRequest
        {
            ModuleId = 2,
            Name = "Module 2 Category",
            ViewOrder = 1,
            ParentId = null,
        }, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(id1).IsGreaterThan(0);
        await Assert.That(id2).IsGreaterThan(0);

        var entity1 = await GetFromCommandDbAsync(options, id1).ConfigureAwait(false);
        var entity2 = await GetFromCommandDbAsync(options, id2).ConfigureAwait(false);

        await Assert.That(entity1!.ModuleId).IsEqualTo(1);
        await Assert.That(entity2!.ModuleId).IsEqualTo(2);

        await connection.CloseAsync().ConfigureAwait(false);
    }
}
