// Licensed to ICTAce under the MIT license.

using CategoryHandlers = ICTAce.FileHub.Features.Categories;
using static ICTAce.FileHub.Server.Tests.Helpers.CategoryTestHelpers;

namespace ICTAce.FileHub.Server.Tests.Features.Categories;

public class GetHandlerTests : HandlerTestBase
{
    [Test]
    public async Task Handle_WithValidId_ReturnsCategory()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync();
        await SeedQueryDataAsync(options, CreateTestEntity());

        var handler = new CategoryHandlers.GetHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        var request = new CategoryHandlers.GetCategoryRequest { ModuleId = 1, Id = 1 };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Id).IsEqualTo(1);
        await Assert.That(result.Name).IsEqualTo("Test Category");
        await Assert.That(result.ModuleId).IsEqualTo(1);
        await Assert.That(result.ViewOrder).IsEqualTo(1);
        await Assert.That(result.ParentId).IsEqualTo(0);

        connection.Close();
    }

    [Test]
    public async Task Handle_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync();
        await SeedQueryDataAsync(options, CreateTestEntity());

        var handler = new CategoryHandlers.GetHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        var request = new CategoryHandlers.GetCategoryRequest { ModuleId = 1, Id = 999 };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsNull();
        connection.Close();
    }

    [Test]
    public async Task Handle_WithUnauthorizedUser_ReturnsNull()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync();
        await SeedQueryDataAsync(options, CreateTestEntity());

        var handler = new CategoryHandlers.GetHandler(
            CreateQueryHandlerServices(options, isAuthorized: false));

        var request = new CategoryHandlers.GetCategoryRequest { ModuleId = 1, Id = 1 };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsNull();
        connection.Close();
    }

    [Test]
    public async Task Handle_WithDifferentModuleId_ReturnsNull()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync();
        await SeedQueryDataAsync(options, CreateTestEntity(moduleId: 1));

        var handler = new CategoryHandlers.GetHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        var request = new CategoryHandlers.GetCategoryRequest { ModuleId = 2, Id = 1 };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsNull();

        connection.Close();
    }

    [Test]
    public async Task Handle_VerifiesAuditFields_ArePopulated()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync();
        var createdOn = DateTime.UtcNow.AddDays(-5);
        var modifiedOn = DateTime.UtcNow.AddDays(-1);

        await SeedQueryDataAsync(options, 
            CreateTestEntity(id: 1, moduleId: 1, name: "Test Category", viewOrder: 1, parentId: 0,
                createdBy: "creator", createdOn: createdOn, modifiedBy: "modifier", modifiedOn: modifiedOn));

        var handler = new CategoryHandlers.GetHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        var request = new CategoryHandlers.GetCategoryRequest { ModuleId = 1, Id = 1 };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.CreatedBy).IsEqualTo("creator");
        await Assert.That(result.ModifiedBy).IsEqualTo("modifier");
        await Assert.That(result.CreatedOn).IsEqualTo(createdOn);
        await Assert.That(result.ModifiedOn).IsEqualTo(modifiedOn);

        connection.Close();
    }

    [Test]
    public async Task Handle_WithParentCategory_ReturnsCorrectParentId()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync();
        await SeedQueryDataAsync(options, 
            CreateTestEntity(id: 1, name: "Parent Category", parentId: 0),
            CreateTestEntity(id: 2, name: "Child Category", parentId: 1));

        var handler = new CategoryHandlers.GetHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        var request = new CategoryHandlers.GetCategoryRequest { ModuleId = 1, Id = 2 };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.ParentId).IsEqualTo(1);
        await Assert.That(result.Name).IsEqualTo("Child Category");

        connection.Close();
    }
}
