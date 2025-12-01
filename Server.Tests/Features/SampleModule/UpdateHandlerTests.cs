// Licensed to ICTAce under the MIT license.

using static ICTAce.FileHub.Server.Tests.Helpers.SampleModuleTestHelpers;
namespace ICTAce.FileHub.Server.Tests.Features.SampleModule;

public class UpdateHandlerTests : HandlerTestBase
{
    [Test]
    public async Task Handle_WithValidRequest_UpdatesSampleModule()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        await SeedCommandDataAsync(options, CreateTestEntity(name: "Original Name")).ConfigureAwait(false);

        var handler = new UpdateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new UpdateSampleModuleRequest
        {
            Id = 1,
            ModuleId = 1,
            Name = "Updated Name"
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsEqualTo(1);

        var updatedEntity = await GetFromCommandDbAsync(options, 1).ConfigureAwait(false);
        await Assert.That(updatedEntity).IsNotNull();
        await Assert.That(updatedEntity!.Name).IsEqualTo("Updated Name");

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Handle_WithUnauthorizedUser_ReturnsMinusOne()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        await SeedCommandDataAsync(options, CreateTestEntity(name: "Original Name")).ConfigureAwait(false);

        var handler = new UpdateHandler(
            CreateCommandHandlerServices(options, isAuthorized: false));

        var request = new UpdateSampleModuleRequest
        {
            Id = 1,
            ModuleId = 1,
            Name = "Updated Name"
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsEqualTo(-1);

        var entity = await GetFromCommandDbAsync(options, 1).ConfigureAwait(false);
        await Assert.That(entity!.Name).IsEqualTo("Original Name");

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Handle_WithNonExistentId_ReturnsMinusOne()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);

        var handler = new UpdateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new UpdateSampleModuleRequest
        {
            Id = 999, // Non-existent ID
            ModuleId = 1,
            Name = "Updated Name"
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsEqualTo(-1);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    [Arguments("")]
    [Arguments("A")]
    [Arguments("Very Long Name That Should Still Work Fine Because We Want To Test Edge Cases")]
    public async Task Handle_WithDifferentNames_UpdatesSuccessfully(string newName)
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        await SeedCommandDataAsync(options, CreateTestEntity(name: "Original Name")).ConfigureAwait(false);

        var handler = new UpdateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new UpdateSampleModuleRequest
        {
            Id = 1,
            ModuleId = 1,
            Name = newName
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsEqualTo(1);

        var updatedEntity = await GetFromCommandDbAsync(options, 1).ConfigureAwait(false);
        await Assert.That(updatedEntity!.Name).IsEqualTo(newName);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Handle_UpdateMultipleTimes_ReflectsLatestChanges()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        await SeedCommandDataAsync(options, CreateTestEntity(name: "Original Name")).ConfigureAwait(false);

        var handler = new UpdateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        // Act - Update multiple times
        await handler.Handle(new UpdateSampleModuleRequest { Id = 1, ModuleId = 1, Name = "First Update" }, CancellationToken.None).ConfigureAwait(false);
        await handler.Handle(new UpdateSampleModuleRequest { Id = 1, ModuleId = 1, Name = "Second Update" }, CancellationToken.None).ConfigureAwait(false);
        await handler.Handle(new UpdateSampleModuleRequest { Id = 1, ModuleId = 1, Name = "Final Update" }, CancellationToken.None).ConfigureAwait(false);

        // Assert
        var entity = await GetFromCommandDbAsync(options, 1).ConfigureAwait(false);
        await Assert.That(entity!.Name).IsEqualTo("Final Update");

        await connection.CloseAsync().ConfigureAwait(false);
    }
}
