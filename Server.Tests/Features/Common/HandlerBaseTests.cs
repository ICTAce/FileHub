// Licensed to ICTAce under the MIT license.

using ICTAce.FileHub.Features.SampleModule;
using static ICTAce.FileHub.Server.Tests.Helpers.SampleModuleTestHelpers;

namespace ICTAce.FileHub.Server.Tests.Features.Common;

/// <summary>
/// Tests for the generic HandlerBase methods to ensure they work correctly
/// across different entity types and scenarios.
/// </summary>
public class HandlerBaseTests : HandlerTestBase
{
    #region HandleCreateAsync Tests

    [Test]
    public async Task HandleCreateAsync_WithValidRequest_CreatesEntity()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync();
        var handler = new CreateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new CreateSampleModuleRequest
        {
            ModuleId = 1,
            Name = "Test Module"
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsGreaterThan(0);

        var entity = await GetFromCommandDbAsync(options, result);
        await Assert.That(entity).IsNotNull();
        await Assert.That(entity!.Name).IsEqualTo("Test Module");
        await Assert.That(entity.ModuleId).IsEqualTo(1);

        connection.Close();
    }

    [Test]
    public async Task HandleCreateAsync_WithUnauthorizedUser_ReturnsMinusOne()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync();
        var handler = new CreateHandler(
            CreateCommandHandlerServices(options, isAuthorized: false));

        var request = new CreateSampleModuleRequest
        {
            ModuleId = 1,
            Name = "Test Module"
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsEqualTo(-1);

        var count = await GetCountFromCommandDbAsync(options);
        await Assert.That(count).IsEqualTo(0);

        connection.Close();
    }

    [Test]
    public async Task HandleCreateAsync_AutoAssignsId()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync();
        var handler = new CreateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        // Act - Create multiple entities
        var id1 = await handler.Handle(new CreateSampleModuleRequest 
        { 
            ModuleId = 1, 
            Name = "First" 
        }, CancellationToken.None);

        var id2 = await handler.Handle(new CreateSampleModuleRequest 
        { 
            ModuleId = 1, 
            Name = "Second" 
        }, CancellationToken.None);

        // Assert - IDs are auto-incremented
        await Assert.That(id1).IsGreaterThan(0);
        await Assert.That(id2).IsGreaterThan(id1);

        connection.Close();
    }

    #endregion

    #region HandleGetAsync Tests

    [Test]
    public async Task HandleGetAsync_WithExistingEntity_ReturnsDto()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync();
        await Helpers.SampleModuleTestHelpers.SeedQueryDataAsync(options,
            CreateTestEntity(id: 1, moduleId: 1, name: "Test"));

        var handler = new GetHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        var request = new GetSampleModuleRequest
        {
            Id = 1,
            ModuleId = 1
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Id).IsEqualTo(1);
        await Assert.That(result.Name).IsEqualTo("Test");

        connection.Close();
    }

    [Test]
    public async Task HandleGetAsync_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync();
        var handler = new GetHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        var request = new GetSampleModuleRequest
        {
            Id = 999,
            ModuleId = 1
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsNull();

        connection.Close();
    }

    [Test]
    public async Task HandleGetAsync_WithUnauthorizedUser_ReturnsNull()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync();
        await Helpers.SampleModuleTestHelpers.SeedQueryDataAsync(options,
            CreateTestEntity(id: 1, moduleId: 1));

        var handler = new GetHandler(
            CreateQueryHandlerServices(options, isAuthorized: false));

        var request = new GetSampleModuleRequest
        {
            Id = 1,
            ModuleId = 1
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsNull();

        connection.Close();
    }

    #endregion

    #region HandleDeleteAsync Tests

    [Test]
    public async Task HandleDeleteAsync_WithExistingEntity_DeletesAndReturnsId()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync();
        await SeedCommandDataAsync(options, CreateTestEntity(id: 1, moduleId: 1));

        var handler = new DeleteHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new DeleteSampleModuleRequest
        {
            Id = 1,
            ModuleId = 1
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsEqualTo(1);

        var entity = await GetFromCommandDbAsync(options, 1);
        await Assert.That(entity).IsNull();

        connection.Close();
    }

    [Test]
    public async Task HandleDeleteAsync_WithNonExistentId_ReturnsMinusOne()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync();
        var handler = new DeleteHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new DeleteSampleModuleRequest
        {
            Id = 999,
            ModuleId = 1
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsEqualTo(-1);

        connection.Close();
    }

    [Test]
    public async Task HandleDeleteAsync_WithUnauthorizedUser_ReturnsMinusOne()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync();
        await SeedCommandDataAsync(options, CreateTestEntity(id: 1, moduleId: 1));

        var handler = new DeleteHandler(
            CreateCommandHandlerServices(options, isAuthorized: false));

        var request = new DeleteSampleModuleRequest
        {
            Id = 1,
            ModuleId = 1
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsEqualTo(-1);

        // Verify entity still exists
        var entity = await GetFromCommandDbAsync(options, 1);
        await Assert.That(entity).IsNotNull();

        connection.Close();
    }

    #endregion

    #region HandleListAsync Tests

    [Test]
    public async Task HandleListAsync_ReturnsPagedResults()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync();
        await Helpers.SampleModuleTestHelpers.SeedQueryDataAsync(options,
            CreateTestEntity(id: 1, moduleId: 1, name: "First"),
            CreateTestEntity(id: 2, moduleId: 1, name: "Second"),
            CreateTestEntity(id: 3, moduleId: 1, name: "Third"));

        var handler = new ListHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        var request = new ListSampleModuleRequest
        {
            ModuleId = 1,
            PageNumber = 1,
            PageSize = 2
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Items.Count).IsEqualTo(2);
        await Assert.That(result.TotalCount).IsEqualTo(3);
        await Assert.That(result.PageNumber).IsEqualTo(1);
        await Assert.That(result.PageSize).IsEqualTo(2);

        connection.Close();
    }

    [Test]
    public async Task HandleListAsync_WithCustomOrdering_SortsCorrectly()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync();
        await Helpers.SampleModuleTestHelpers.SeedQueryDataAsync(options,
            CreateTestEntity(id: 1, moduleId: 1, name: "Zebra"),
            CreateTestEntity(id: 2, moduleId: 1, name: "Apple"),
            CreateTestEntity(id: 3, moduleId: 1, name: "Mango"));

        var handler = new ListHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        var request = new ListSampleModuleRequest
        {
            ModuleId = 1,
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        var items = result!.Items.ToList();
        await Assert.That(result).IsNotNull();
        await Assert.That(items[0].Name).IsEqualTo("Apple");
        await Assert.That(items[1].Name).IsEqualTo("Mango");
        await Assert.That(items[2].Name).IsEqualTo("Zebra");

        connection.Close();
    }

    [Test]
    public async Task HandleListAsync_WithUnauthorizedUser_ReturnsNull()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync();
        await Helpers.SampleModuleTestHelpers.SeedQueryDataAsync(options,
            CreateTestEntity(id: 1, moduleId: 1));

        var handler = new ListHandler(
            CreateQueryHandlerServices(options, isAuthorized: false));

        var request = new ListSampleModuleRequest
        {
            ModuleId = 1,
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsNull();

        connection.Close();
    }

    #endregion

    #region HandleUpdateAsync Tests

    [Test]
    public async Task HandleUpdateAsync_WithValidRequest_UpdatesEntity()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync();
        await SeedCommandDataAsync(options, CreateTestEntity(id: 1, moduleId: 1, name: "Original"));

        var handler = new UpdateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new UpdateSampleModuleRequest
        {
            Id = 1,
            ModuleId = 1,
            Name = "Updated"
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsEqualTo(1);

        var entity = await GetFromCommandDbAsync(options, 1);
        await Assert.That(entity!.Name).IsEqualTo("Updated");

        connection.Close();
    }

    [Test]
    public async Task HandleUpdateAsync_WithNonExistentId_ReturnsMinusOne()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync();
        var handler = new UpdateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new UpdateSampleModuleRequest
        {
            Id = 999,
            ModuleId = 1,
            Name = "Updated"
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsEqualTo(-1);

        connection.Close();
    }

    [Test]
    public async Task HandleUpdateAsync_WithUnauthorizedUser_ReturnsMinusOne()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync();
        await SeedCommandDataAsync(options, CreateTestEntity(id: 1, moduleId: 1, name: "Original"));

        var handler = new UpdateHandler(
            CreateCommandHandlerServices(options, isAuthorized: false));

        var request = new UpdateSampleModuleRequest
        {
            Id = 1,
            ModuleId = 1,
            Name = "Updated"
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsEqualTo(-1);

        var entity = await GetFromCommandDbAsync(options, 1);
        await Assert.That(entity!.Name).IsEqualTo("Original");

        connection.Close();
    }

    #endregion
}
