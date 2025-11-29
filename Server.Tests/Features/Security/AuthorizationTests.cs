// Licensed to ICTAce under the MIT license.

using static ICTAce.FileHub.Server.Tests.Helpers.SampleModuleTestHelpers;
using CategoryHandlers = ICTAce.FileHub.Features.Categories;
using CategoryHelpers = ICTAce.FileHub.Server.Tests.Helpers.CategoryTestHelpers;

namespace ICTAce.FileHub.Server.Tests.Features.Security;

/// <summary>
/// Tests for authorization and security features to ensure proper access control.
/// CRITICAL: These tests verify ModuleId isolation and permission enforcement.
/// </summary>
public class AuthorizationTests : HandlerTestBase
{
    #region Module Isolation Tests - CRITICAL SECURITY

    [Test]
    public async Task Update_WithDifferentModuleId_ReturnsMinusOne()
    {
        // Arrange - Entity in Module 1, try to update from Module 2
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        await SeedCommandDataAsync(options, CreateTestEntity(id: 1, moduleId: 1, name: "Original")).ConfigureAwait(false);

        var handler = new UpdateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new UpdateSampleModuleRequest
        {
            Id = 1,
            ModuleId = 2,  // Different module!
            Name = "Hacked"
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert - Security check prevented update
        await Assert.That(result).IsEqualTo(-1);

        var entity = await GetFromCommandDbAsync(options, 1).ConfigureAwait(false);
        await Assert.That(entity!.Name).IsEqualTo("Original");
        await Assert.That(entity.ModuleId).IsEqualTo(1);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Delete_WithDifferentModuleId_ReturnsMinusOne()
    {
        // Arrange - Entity in Module 1, try to delete from Module 2
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        await SeedCommandDataAsync(options, CreateTestEntity(id: 1, moduleId: 1)).ConfigureAwait(false);

        var handler = new DeleteHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new DeleteSampleModuleRequest
        {
            Id = 1,
            ModuleId = 2  // Different module!
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert - Security check prevented delete
        await Assert.That(result).IsEqualTo(-1);

        var entity = await GetFromCommandDbAsync(options, 1).ConfigureAwait(false);
        await Assert.That(entity).IsNotNull();

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Get_WithDifferentModuleId_ReturnsNull()
    {
        // Arrange - Entity in Module 1, try to get from Module 2
        var (connection, options) = await CreateQueryDatabaseAsync().ConfigureAwait(false);
        await Helpers.SampleModuleTestHelpers.SeedQueryDataAsync(options,
            CreateTestEntity(id: 1, moduleId: 1)).ConfigureAwait(false);

        var handler = new GetHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        var request = new GetSampleModuleRequest
        {
            Id = 1,
            ModuleId = 2  // Different module!
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert - Security check prevented access
        await Assert.That(result).IsNull();

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task List_OnlyReturnsEntitiesFromRequestedModule()
    {
        // Arrange - Entities in both Module 1 and Module 2
        var (connection, options) = await CreateQueryDatabaseAsync().ConfigureAwait(false);
        await Helpers.SampleModuleTestHelpers.SeedQueryDataAsync(options,
            CreateTestEntity(id: 1, moduleId: 1, name: "Module 1 - Item 1"),
            CreateTestEntity(id: 2, moduleId: 1, name: "Module 1 - Item 2"),
            CreateTestEntity(id: 3, moduleId: 2, name: "Module 2 - Item 1"),
            CreateTestEntity(id: 4, moduleId: 2, name: "Module 2 - Item 2")).ConfigureAwait(false);

        var handler = new ListHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        var request = new ListSampleModuleRequest
        {
            ModuleId = 1,
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert - Only Module 1 items returned
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.TotalCount).IsEqualTo(2);
        await Assert.That(result.Items.All(x => x.Name.StartsWith("Module 1", StringComparison.Ordinal))).IsTrue();

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task CategoryUpdate_WithDifferentModuleId_ReturnsMinusOne()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        await CategoryHelpers.SeedCommandDataAsync(options,
            CategoryHelpers.CreateTestEntity(id: 1, moduleId: 1, name: "Original")).ConfigureAwait(false);

        var handler = new CategoryHandlers.UpdateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new CategoryHandlers.UpdateCategoryRequest
        {
            Id = 1,
            ModuleId = 2,  // Different module!
            Name = "Hacked",
            ViewOrder = 1,
            ParentId = 0
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsEqualTo(-1);

        var entity = await CategoryHelpers.GetFromCommandDbAsync(options, 1).ConfigureAwait(false);
        await Assert.That(entity!.Name).IsEqualTo("Original");

        await connection.CloseAsync().ConfigureAwait(false);
    }

    #endregion

    #region Permission Tests

    [Test]
    public async Task Create_WithViewPermissionOnly_ReturnsMinusOne()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        var handler = new CreateHandler(
            CreateCommandHandlerServices(options, isAuthorized: false));

        var request = new CreateSampleModuleRequest
        {
            ModuleId = 1,
            Name = "Test"
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
    public async Task Update_WithoutEditPermission_ReturnsMinusOne()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        await SeedCommandDataAsync(options, CreateTestEntity(id: 1, moduleId: 1, name: "Original")).ConfigureAwait(false);

        var handler = new UpdateHandler(
            CreateCommandHandlerServices(options, isAuthorized: false));

        var request = new UpdateSampleModuleRequest
        {
            Id = 1,
            ModuleId = 1,
            Name = "Unauthorized Update"
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsEqualTo(-1);

        var entity = await GetFromCommandDbAsync(options, 1).ConfigureAwait(false);
        await Assert.That(entity!.Name).IsEqualTo("Original");

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Delete_WithoutEditPermission_ReturnsMinusOne()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        await SeedCommandDataAsync(options, CreateTestEntity(id: 1, moduleId: 1)).ConfigureAwait(false);

        var handler = new DeleteHandler(
            CreateCommandHandlerServices(options, isAuthorized: false));

        var request = new DeleteSampleModuleRequest
        {
            Id = 1,
            ModuleId = 1
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsEqualTo(-1);

        var entity = await GetFromCommandDbAsync(options, 1).ConfigureAwait(false);
        await Assert.That(entity).IsNotNull();

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Get_WithoutViewPermission_ReturnsNull()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync().ConfigureAwait(false);
        await Helpers.SampleModuleTestHelpers.SeedQueryDataAsync(options,
            CreateTestEntity(id: 1, moduleId: 1)).ConfigureAwait(false);

        var handler = new GetHandler(
            CreateQueryHandlerServices(options, isAuthorized: false));

        var request = new GetSampleModuleRequest
        {
            Id = 1,
            ModuleId = 1
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsNull();

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task List_WithoutViewPermission_ReturnsNull()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync().ConfigureAwait(false);
        await Helpers.SampleModuleTestHelpers.SeedQueryDataAsync(options,
            CreateTestEntity(id: 1, moduleId: 1)).ConfigureAwait(false);

        var handler = new ListHandler(
            CreateQueryHandlerServices(options, isAuthorized: false));

        var request = new ListSampleModuleRequest
        {
            ModuleId = 1,
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsNull();

        await connection.CloseAsync().ConfigureAwait(false);
    }

    #endregion

    #region Multi-Module Tests

    [Test]
    public async Task MultipleModules_IsolationMaintained()
    {
        // Arrange - Create entities in different modules
        var (connection, options) = await CreateQueryDatabaseAsync().ConfigureAwait(false);
        await Helpers.SampleModuleTestHelpers.SeedQueryDataAsync(options,
            CreateTestEntity(id: 1, moduleId: 1, name: "Module 1"),
            CreateTestEntity(id: 2, moduleId: 2, name: "Module 2"),
            CreateTestEntity(id: 3, moduleId: 3, name: "Module 3")).ConfigureAwait(false);

        var handler = new ListHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        // Act - Request from Module 2
        var result = await handler.Handle(new ListSampleModuleRequest
        {
            ModuleId = 2,
            PageNumber = 1,
            PageSize = 10
        }, CancellationToken.None).ConfigureAwait(false);

        // Assert - Only Module 2 entity returned
        var items = result!.Items.ToList();
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.TotalCount).IsEqualTo(1);
        await Assert.That(items[0].Name).IsEqualTo("Module 2");

        await connection.CloseAsync().ConfigureAwait(false);
    }

    #endregion
}
