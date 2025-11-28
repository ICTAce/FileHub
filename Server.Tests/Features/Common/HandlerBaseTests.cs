// Licensed to ICTAce under the MIT license.

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
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        var handler = new CreateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new CreateSampleModuleRequest
        {
            ModuleId = 1,
            Name = "Test Module"
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsGreaterThan(0);

        var entity = await GetFromCommandDbAsync(options, result).ConfigureAwait(false);
        await Assert.That(entity).IsNotNull();
        await Assert.That(entity!.Name).IsEqualTo("Test Module");
        await Assert.That(entity.ModuleId).IsEqualTo(1);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task HandleCreateAsync_WithUnauthorizedUser_ReturnsMinusOne()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        var handler = new CreateHandler(
            CreateCommandHandlerServices(options, isAuthorized: false));

        var request = new CreateSampleModuleRequest
        {
            ModuleId = 1,
            Name = "Test Module"
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
    public async Task HandleCreateAsync_AutoAssignsId()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        var handler = new CreateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        // Act - Create multiple entities
        var id1 = await handler.Handle(new CreateSampleModuleRequest 
        { 
            ModuleId = 1, 
            Name = "First" 
        }, CancellationToken.None).ConfigureAwait(false);

        var id2 = await handler.Handle(new CreateSampleModuleRequest 
        { 
            ModuleId = 1, 
            Name = "Second" 
        }, CancellationToken.None).ConfigureAwait(false);

        // Assert - IDs are auto-incremented
        await Assert.That(id1).IsGreaterThan(0);
        await Assert.That(id2).IsGreaterThan(id1);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    #endregion

    #region HandleGetAsync Tests

    [Test]
    public async Task HandleGetAsync_WithExistingEntity_ReturnsDto()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync().ConfigureAwait(false);
        await Helpers.SampleModuleTestHelpers.SeedQueryDataAsync(options,
            CreateTestEntity(id: 1, moduleId: 1, name: "Test")).ConfigureAwait(false);

        var handler = new GetHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        var request = new GetSampleModuleRequest
        {
            Id = 1,
            ModuleId = 1
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Id).IsEqualTo(1);
        await Assert.That(result.Name).IsEqualTo("Test");

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task HandleGetAsync_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync().ConfigureAwait(false);
        var handler = new GetHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        var request = new GetSampleModuleRequest
        {
            Id = 999,
            ModuleId = 1
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsNull();

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task HandleGetAsync_WithUnauthorizedUser_ReturnsNull()
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

    #endregion

    #region HandleDeleteAsync Tests

    [Test]
    public async Task HandleDeleteAsync_WithExistingEntity_DeletesAndReturnsId()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        await SeedCommandDataAsync(options, CreateTestEntity(id: 1, moduleId: 1)).ConfigureAwait(false);

        var handler = new DeleteHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new DeleteSampleModuleRequest
        {
            Id = 1,
            ModuleId = 1
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsEqualTo(1);

        var entity = await GetFromCommandDbAsync(options, 1).ConfigureAwait(false);
        await Assert.That(entity).IsNull();

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task HandleDeleteAsync_WithNonExistentId_ReturnsMinusOne()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        var handler = new DeleteHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new DeleteSampleModuleRequest
        {
            Id = 999,
            ModuleId = 1
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsEqualTo(-1);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task HandleDeleteAsync_WithUnauthorizedUser_ReturnsMinusOne()
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

        // Verify entity still exists
        var entity = await GetFromCommandDbAsync(options, 1).ConfigureAwait(false);
        await Assert.That(entity).IsNotNull();

        await connection.CloseAsync().ConfigureAwait(false);
    }

    #endregion

    #region HandleListAsync Tests

    [Test]
    public async Task HandleListAsync_ReturnsPagedResults()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync().ConfigureAwait(false);
        await Helpers.SampleModuleTestHelpers.SeedQueryDataAsync(options,
            CreateTestEntity(id: 1, moduleId: 1, name: "First"),
            CreateTestEntity(id: 2, moduleId: 1, name: "Second"),
            CreateTestEntity(id: 3, moduleId: 1, name: "Third")).ConfigureAwait(false);

        var handler = new ListHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        var request = new ListSampleModuleRequest
        {
            ModuleId = 1,
            PageNumber = 1,
            PageSize = 2
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Items.Count()).IsEqualTo(2);
        await Assert.That(result.TotalCount).IsEqualTo(3);
        await Assert.That(result.PageNumber).IsEqualTo(1);
        await Assert.That(result.PageSize).IsEqualTo(2);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task HandleListAsync_WithCustomOrdering_SortsCorrectly()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync().ConfigureAwait(false);
        await Helpers.SampleModuleTestHelpers.SeedQueryDataAsync(options,
            CreateTestEntity(id: 1, moduleId: 1, name: "Zebra"),
            CreateTestEntity(id: 2, moduleId: 1, name: "Apple"),
            CreateTestEntity(id: 3, moduleId: 1, name: "Mango")).ConfigureAwait(false);

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

        // Assert
        var items = result!.Items.ToList();
        await Assert.That(result).IsNotNull();
        await Assert.That(items[0].Name).IsEqualTo("Apple");
        await Assert.That(items[1].Name).IsEqualTo("Mango");
        await Assert.That(items[2].Name).IsEqualTo("Zebra");

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task HandleListAsync_WithUnauthorizedUser_ReturnsNull()
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

    #region HandleUpdateAsync Tests

    [Test]
    public async Task HandleUpdateAsync_WithValidRequest_UpdatesEntity()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        await SeedCommandDataAsync(options, CreateTestEntity(id: 1, moduleId: 1, name: "Original")).ConfigureAwait(false);

        var handler = new UpdateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new UpdateSampleModuleRequest
        {
            Id = 1,
            ModuleId = 1,
            Name = "Updated"
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsEqualTo(1);

        var entity = await GetFromCommandDbAsync(options, 1).ConfigureAwait(false);
        await Assert.That(entity!.Name).IsEqualTo("Updated");

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task HandleUpdateAsync_WithNonExistentId_ReturnsMinusOne()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        var handler = new UpdateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new UpdateSampleModuleRequest
        {
            Id = 999,
            ModuleId = 1,
            Name = "Updated"
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsEqualTo(-1);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task HandleUpdateAsync_WithUnauthorizedUser_ReturnsMinusOne()
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
            Name = "Updated"
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsEqualTo(-1);

        var entity = await GetFromCommandDbAsync(options, 1).ConfigureAwait(false);
        await Assert.That(entity!.Name).IsEqualTo("Original");

        await connection.CloseAsync().ConfigureAwait(false);
    }

    #endregion
}
