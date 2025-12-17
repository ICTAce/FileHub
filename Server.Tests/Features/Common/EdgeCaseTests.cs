// Licensed to ICTAce under the MIT license.

using static ICTAce.FileHub.Server.Tests.Helpers.SampleModuleTestHelpers;
using CategoryHandlers = ICTAce.FileHub.Features.Categories;

namespace ICTAce.FileHub.Server.Tests.Features.Common;

/// <summary>
/// Tests for edge cases and error handling scenarios.
/// </summary>
public class EdgeCaseTests : HandlerTestBase
{
    #region Null and Empty String Tests

    [Test]
    public async Task Create_WithEmptyName_CreatesSuccessfully()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        var handler = new CreateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        // Act
        var result = await handler.Handle(new CreateSampleModuleRequest
        {
            ModuleId = 1,
            Name = string.Empty
        }, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsGreaterThan(0);

        var entity = await GetFromCommandDbAsync(options, result).ConfigureAwait(false);
        await Assert.That(entity!.Name).IsEqualTo(string.Empty);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Update_WithEmptyName_UpdatesSuccessfully()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        await SeedCommandDataAsync(options, CreateTestEntity(id: 1, moduleId: 1, name: "Original")).ConfigureAwait(false);

        var handler = new UpdateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        // Act
        var result = await handler.Handle(new UpdateSampleModuleRequest
        {
            Id = 1,
            ModuleId = 1,
            Name = string.Empty
        }, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsEqualTo(1);

        var entity = await GetFromCommandDbAsync(options, 1).ConfigureAwait(false);
        await Assert.That(entity!.Name).IsEqualTo(string.Empty);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Create_WithVeryLongName_CreatesSuccessfully()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        var handler = new CreateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var longName = new string('A', 500);

        // Act
        var result = await handler.Handle(new CreateSampleModuleRequest
        {
            ModuleId = 1,
            Name = longName
        }, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsGreaterThan(0);

        var entity = await GetFromCommandDbAsync(options, result).ConfigureAwait(false);
        await Assert.That(entity!.Name).IsEqualTo(longName);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Create_WithSpecialCharacters_CreatesSuccessfully()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        var handler = new CreateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var specialName = "Test!@#$%^&*()_+-={}[]|\\:\";<>?,./~`";

        // Act
        var result = await handler.Handle(new CreateSampleModuleRequest
        {
            ModuleId = 1,
            Name = specialName
        }, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsGreaterThan(0);

        var entity = await GetFromCommandDbAsync(options, result).ConfigureAwait(false);
        await Assert.That(entity!.Name).IsEqualTo(specialName);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Create_WithUnicodeCharacters_CreatesSuccessfully()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        var handler = new CreateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var unicodeName = "Test 你好 мир 🚀 emoji";

        // Act
        var result = await handler.Handle(new CreateSampleModuleRequest
        {
            ModuleId = 1,
            Name = unicodeName
        }, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsGreaterThan(0);

        var entity = await GetFromCommandDbAsync(options, result).ConfigureAwait(false);
        await Assert.That(entity!.Name).IsEqualTo(unicodeName);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    #endregion

    #region Invalid ID Tests

    [Test]
    public async Task Get_WithZeroId_ReturnsNull()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync().ConfigureAwait(false);
        var handler = new GetHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        // Act
        var result = await handler.Handle(new GetSampleModuleRequest
        {
            Id = 0,
            ModuleId = 1
        }, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsNull();

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Get_WithNegativeId_ReturnsNull()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync().ConfigureAwait(false);
        var handler = new GetHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        // Act
        var result = await handler.Handle(new GetSampleModuleRequest
        {
            Id = -1,
            ModuleId = 1
        }, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsNull();

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Delete_WithZeroId_ReturnsMinusOne()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        var handler = new DeleteHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        // Act
        var result = await handler.Handle(new DeleteSampleModuleRequest
        {
            Id = 0,
            ModuleId = 1
        }, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsEqualTo(-1);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Update_WithZeroId_ReturnsMinusOne()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        var handler = new UpdateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        // Act
        var result = await handler.Handle(new UpdateSampleModuleRequest
        {
            Id = 0,
            ModuleId = 1,
            Name = "Test"
        }, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsEqualTo(-1);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    #endregion

    #region Invalid ModuleId Tests

    [Test]
    public async Task Create_WithZeroModuleId_CreatesSuccessfully()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        var handler = new CreateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        // Act - ModuleId 0 might be valid in some systems
        var result = await handler.Handle(new CreateSampleModuleRequest
        {
            ModuleId = 0,
            Name = "Test"
        }, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsGreaterThan(0);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task List_WithZeroModuleId_ReturnsEmptyList()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync().ConfigureAwait(false);
        await Helpers.SampleModuleTestHelpers.SeedQueryDataAsync(options,
            CreateTestEntity(id: 1, moduleId: 1, name: "Module 1")).ConfigureAwait(false);

        var handler = new ListHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        // Act
        var result = await handler.Handle(new ListSampleModuleRequest
        {
            ModuleId = 0,
            PageNumber = 1,
            PageSize = 10
        }, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.TotalCount).IsEqualTo(0);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    #endregion

    #region Concurrent Operations

    [Test]
    public async Task MultipleCreates_Concurrent_AllSucceed()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        var handler = new CreateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        // Act - Create 10 entities concurrently
        var tasks = Enumerable.Range(1, 10).Select(i =>
            handler.Handle(new CreateSampleModuleRequest
            {
                ModuleId = 1,
                Name = $"Concurrent {i}"
            }, CancellationToken.None)
        );

        var results = await Task.WhenAll(tasks).ConfigureAwait(false);

        // Assert - All creates succeeded with unique IDs
        await Assert.That(results.All(r => r > 0)).IsTrue();
        await Assert.That(results.Distinct().Count()).IsEqualTo(10);

        var count = await GetCountFromCommandDbAsync(options).ConfigureAwait(false);
        await Assert.That(count).IsEqualTo(10);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    #endregion

    #region SQL Injection Prevention

    [Test]
    public async Task Create_WithSqlInjectionAttempt_TreatsAsLiteralString()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        var handler = new CreateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var sqlInjection = "'; DROP TABLE SampleModule; --";

        // Act
        var result = await handler.Handle(new CreateSampleModuleRequest
        {
            ModuleId = 1,
            Name = sqlInjection
        }, CancellationToken.None).ConfigureAwait(false);

        // Assert - Entity created with SQL injection as literal text
        await Assert.That(result).IsGreaterThan(0);

        var entity = await GetFromCommandDbAsync(options, result).ConfigureAwait(false);
        await Assert.That(entity!.Name).IsEqualTo(sqlInjection);

        // Verify table still exists
        var count = await GetCountFromCommandDbAsync(options).ConfigureAwait(false);
        await Assert.That(count).IsEqualTo(1);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    #endregion

    #region Category-Specific Edge Cases

    [Test]
    public async Task Category_WithNegativeViewOrder_CreatesSuccessfully()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        var handler = new CategoryHandlers.CreateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        // Act
        var result = await handler.Handle(new CategoryHandlers.CreateCategoryRequest
        {
            ModuleId = 1,
            Name = "Test",
            ViewOrder = -1,
            ParentId = null
        }, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsGreaterThan(0);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Category_WithNonExistentParentId_CreatesSuccessfully()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        var handler = new CategoryHandlers.CreateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        // Act - ParentId 999 doesn't exist
        var result = await handler.Handle(new CategoryHandlers.CreateCategoryRequest
        {
            ModuleId = 1,
            Name = "Orphan Category",
            ViewOrder = 1,
            ParentId = 999
        }, CancellationToken.None).ConfigureAwait(false);

        // Assert - Creates successfully (orphan category)
        await Assert.That(result).IsGreaterThan(0);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    #endregion
}
