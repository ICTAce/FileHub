// Licensed to ICTAce under the MIT license.

using CategoryHandlers = ICTAce.FileHub.Features.Categories;
using static ICTAce.FileHub.Server.Tests.Helpers.CategoryTestHelpers;

namespace ICTAce.FileHub.Server.Tests.Features.Categories;

public class DeleteHandlerTests : HandlerTestBase
{
    [Test]
    public async Task Handle_WithValidId_DeletesCategory()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync();
        await SeedCommandDataAsync(options, CreateTestEntity(id: 1));

        var handler = new CategoryHandlers.DeleteHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new CategoryHandlers.DeleteCategoryRequest { Id = 1, ModuleId = 1 };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsEqualTo(1);

        var entity = await GetFromCommandDbAsync(options, 1);
        await Assert.That(entity).IsNull();

        var count = await GetCountFromCommandDbAsync(options);
        await Assert.That(count).IsEqualTo(0);

        connection.Close();
    }

    [Test]
    public async Task Handle_WithInvalidId_ReturnsMinusOne()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync();
        await SeedCommandDataAsync(options, CreateTestEntity(id: 1));

        var handler = new CategoryHandlers.DeleteHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new CategoryHandlers.DeleteCategoryRequest { Id = 999, ModuleId = 1 };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsEqualTo(-1);

        var count = await GetCountFromCommandDbAsync(options);
        await Assert.That(count).IsEqualTo(1);

        connection.Close();
    }

    [Test]
    public async Task Handle_WithUnauthorizedUser_ReturnsMinusOne()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync();
        await SeedCommandDataAsync(options, CreateTestEntity(id: 1));

        var handler = new CategoryHandlers.DeleteHandler(
            CreateCommandHandlerServices(options, isAuthorized: false));

        var request = new CategoryHandlers.DeleteCategoryRequest { Id = 1, ModuleId = 1 };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsEqualTo(-1);

        var entity = await GetFromCommandDbAsync(options, 1);
        await Assert.That(entity).IsNotNull();

        connection.Close();
    }

    [Test]
    public async Task Handle_WithWrongModuleId_ReturnsMinusOne()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync();
        await SeedCommandDataAsync(options, CreateTestEntity(id: 1, moduleId: 1));

        var handler = new CategoryHandlers.DeleteHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new CategoryHandlers.DeleteCategoryRequest { Id = 1, ModuleId = 2 };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsEqualTo(-1);

        var entity = await GetFromCommandDbAsync(options, 1);
        await Assert.That(entity).IsNotNull();

        connection.Close();
    }

    [Test]
    public async Task Handle_DeletesOnlySpecifiedCategory_LeavesOthersIntact()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync();
        await SeedCommandDataAsync(options,
            CreateTestEntity(id: 1, name: "Category 1"),
            CreateTestEntity(id: 2, name: "Category 2"),
            CreateTestEntity(id: 3, name: "Category 3"));

        var handler = new CategoryHandlers.DeleteHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new CategoryHandlers.DeleteCategoryRequest { Id = 2, ModuleId = 1 };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsEqualTo(2);

        var entity1 = await GetFromCommandDbAsync(options, 1);
        var entity2 = await GetFromCommandDbAsync(options, 2);
        var entity3 = await GetFromCommandDbAsync(options, 3);

        await Assert.That(entity1).IsNotNull();
        await Assert.That(entity2).IsNull();
        await Assert.That(entity3).IsNotNull();

        var count = await GetCountFromCommandDbAsync(options);
        await Assert.That(count).IsEqualTo(2);

        connection.Close();
    }

    [Test]
    public async Task Handle_DeletesParentCategory_Successfully()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync();
        await SeedCommandDataAsync(options,
            CreateTestEntity(id: 1, name: "Parent Category", parentId: 0),
            CreateTestEntity(id: 2, name: "Child Category", parentId: 1));

        var handler = new CategoryHandlers.DeleteHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new CategoryHandlers.DeleteCategoryRequest { Id = 1, ModuleId = 1 };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsEqualTo(1);

        var parent = await GetFromCommandDbAsync(options, 1);
        await Assert.That(parent).IsNull();

        var child = await GetFromCommandDbAsync(options, 2);
        await Assert.That(child).IsNotNull();

        connection.Close();
    }

    [Test]
    public async Task Handle_DeleteMultipleCategories_AllSucceed()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync();
        await SeedCommandDataAsync(options,
            CreateTestEntity(id: 1, name: "Category 1"),
            CreateTestEntity(id: 2, name: "Category 2"),
            CreateTestEntity(id: 3, name: "Category 3"));

        var handler = new CategoryHandlers.DeleteHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        // Act
        var result1 = await handler.Handle(new CategoryHandlers.DeleteCategoryRequest { Id = 1, ModuleId = 1 }, CancellationToken.None);
        var result2 = await handler.Handle(new CategoryHandlers.DeleteCategoryRequest { Id = 3, ModuleId = 1 }, CancellationToken.None);

        // Assert
        await Assert.That(result1).IsEqualTo(1);
        await Assert.That(result2).IsEqualTo(3);

        var count = await GetCountFromCommandDbAsync(options);
        await Assert.That(count).IsEqualTo(1);

        var remaining = await GetFromCommandDbAsync(options, 2);
        await Assert.That(remaining).IsNotNull();
        await Assert.That(remaining!.Name).IsEqualTo("Category 2");

        connection.Close();
    }

    [Test]
    public async Task Handle_DeleteFromDifferentModules_OnlyDeletesMatchingModule()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync();
        await SeedCommandDataAsync(options,
            CreateTestEntity(id: 1, moduleId: 1, name: "Module 1 Category"),
            CreateTestEntity(id: 2, moduleId: 2, name: "Module 2 Category"));

        var handler = new CategoryHandlers.DeleteHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new CategoryHandlers.DeleteCategoryRequest { Id = 1, ModuleId = 1 };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsEqualTo(1);

        var entity1 = await GetFromCommandDbAsync(options, 1);
        var entity2 = await GetFromCommandDbAsync(options, 2);

        await Assert.That(entity1).IsNull();
        await Assert.That(entity2).IsNotNull();

        connection.Close();
    }
}
