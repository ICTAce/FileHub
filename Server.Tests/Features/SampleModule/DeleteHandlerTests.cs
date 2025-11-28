// Licensed to ICTAce under the MIT license.

using static ICTAce.FileHub.Server.Tests.Helpers.SampleModuleTestHelpers;
namespace ICTAce.FileHub.Server.Tests.Features.SampleModule;

public class DeleteHandlerTests : HandlerTestBase
{
    [Test]
    public async Task Handle_WithValidId_DeletesSampleModule()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        await SeedCommandDataAsync(options, CreateTestEntity()).ConfigureAwait(false);

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

        var deletedEntity = await GetFromCommandDbAsync(options, 1).ConfigureAwait(false);
        await Assert.That(deletedEntity).IsNull();

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Handle_WithUnauthorizedUser_ReturnsMinusOne()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        await SeedCommandDataAsync(options, CreateTestEntity()).ConfigureAwait(false);

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
    public async Task Handle_WithNonExistentId_ReturnsMinusOne()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);

        var handler = new DeleteHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new DeleteSampleModuleRequest
        {
            Id = 999, // Non-existent ID
            ModuleId = 1
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsEqualTo(-1);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Handle_WithWrongModuleId_ReturnsMinusOne()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        await SeedCommandDataAsync(options, CreateTestEntity()).ConfigureAwait(false);

        var handler = new DeleteHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new DeleteSampleModuleRequest
        {
            Id = 1,
            ModuleId = 2 // Wrong module ID
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
    public async Task Handle_DeleteMultipleModules_DeletesAllSuccessfully()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        await SeedCommandDataAsync(options,
            CreateTestEntity(id: 1, name: "Module 1"),
            CreateTestEntity(id: 2, name: "Module 2"),
            CreateTestEntity(id: 3, name: "Module 3")).ConfigureAwait(false);

        var handler = new DeleteHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        // Act
        var result1 = await handler.Handle(new DeleteSampleModuleRequest { Id = 1, ModuleId = 1 }, CancellationToken.None).ConfigureAwait(false);
        var result2 = await handler.Handle(new DeleteSampleModuleRequest { Id = 2, ModuleId = 1 }, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result1).IsEqualTo(1);
        await Assert.That(result2).IsEqualTo(2);

        var count = await GetCountFromCommandDbAsync(options).ConfigureAwait(false);
        await Assert.That(count).IsEqualTo(1); // Only ID 3 should remain

        await connection.CloseAsync().ConfigureAwait(false);
    }
}



