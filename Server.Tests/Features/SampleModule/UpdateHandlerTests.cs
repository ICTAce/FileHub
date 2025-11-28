// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Server.Tests.Features.SampleModule;

public class UpdateHandlerTests : HandlerTestBase
{
    [Test]
    public async Task Handle_WithValidRequest_UpdatesSampleModule()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync();
        await SeedCommandDataAsync(options, CreateTestEntity(name: "Original Name"));

        var handler = new UpdateHandler(
            CreateMockCommandContextFactory(options),
            CreateMockUserPermissions(isAuthorized: true),
            CreateMockTenantManager(),
            CreateMockHttpContextAccessor(),
            CreateMockLogger());

        var request = new UpdateSampleModuleRequest
        {
            Id = 1,
            ModuleId = 1,
            Name = "Updated Name"
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsEqualTo(1);

        var updatedEntity = await GetEntityFromCommandDbAsync(options, 1);
        await Assert.That(updatedEntity).IsNotNull();
        await Assert.That(updatedEntity!.Name).IsEqualTo("Updated Name");

        connection.Close();
    }

    [Test]
    public async Task Handle_WithUnauthorizedUser_ReturnsMinusOne()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync();
        await SeedCommandDataAsync(options, CreateTestEntity(name: "Original Name"));

        var handler = new UpdateHandler(
            CreateMockCommandContextFactory(options),
            CreateMockUserPermissions(isAuthorized: false),
            CreateMockTenantManager(),
            CreateMockHttpContextAccessor(),
            CreateMockLogger());

        var request = new UpdateSampleModuleRequest
        {
            Id = 1,
            ModuleId = 1,
            Name = "Updated Name"
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsEqualTo(-1);

        var entity = await GetEntityFromCommandDbAsync(options, 1);
        await Assert.That(entity!.Name).IsEqualTo("Original Name");

        connection.Close();
    }

    [Test]
    public async Task Handle_WithNonExistentId_ReturnsMinusOne()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync();

        var handler = new UpdateHandler(
            CreateMockCommandContextFactory(options),
            CreateMockUserPermissions(isAuthorized: true),
            CreateMockTenantManager(),
            CreateMockHttpContextAccessor(),
            CreateMockLogger());

        var request = new UpdateSampleModuleRequest
        {
            Id = 999, // Non-existent ID
            ModuleId = 1,
            Name = "Updated Name"
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsEqualTo(-1);

        connection.Close();
    }

    [Test]
    [Arguments("")]
    [Arguments("A")]
    [Arguments("Very Long Name That Should Still Work Fine Because We Want To Test Edge Cases")]
    public async Task Handle_WithDifferentNames_UpdatesSuccessfully(string newName)
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync();
        await SeedCommandDataAsync(options, CreateTestEntity(name: "Original Name"));

        var handler = new UpdateHandler(
            CreateMockCommandContextFactory(options),
            CreateMockUserPermissions(isAuthorized: true),
            CreateMockTenantManager(),
            CreateMockHttpContextAccessor(),
            CreateMockLogger());

        var request = new UpdateSampleModuleRequest
        {
            Id = 1,
            ModuleId = 1,
            Name = newName
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsEqualTo(1);

        var updatedEntity = await GetEntityFromCommandDbAsync(options, 1);
        await Assert.That(updatedEntity!.Name).IsEqualTo(newName);

        connection.Close();
    }

    [Test]
    public async Task Handle_UpdateMultipleTimes_ReflectsLatestChanges()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync();
        await SeedCommandDataAsync(options, CreateTestEntity(name: "Original Name"));

        var handler = new UpdateHandler(
            CreateMockCommandContextFactory(options),
            CreateMockUserPermissions(isAuthorized: true),
            CreateMockTenantManager(),
            CreateMockHttpContextAccessor(),
            CreateMockLogger());

        // Act - Update multiple times
        await handler.Handle(new UpdateSampleModuleRequest { Id = 1, ModuleId = 1, Name = "First Update" }, CancellationToken.None);
        await handler.Handle(new UpdateSampleModuleRequest { Id = 1, ModuleId = 1, Name = "Second Update" }, CancellationToken.None);
        await handler.Handle(new UpdateSampleModuleRequest { Id = 1, ModuleId = 1, Name = "Final Update" }, CancellationToken.None);

        // Assert
        var entity = await GetEntityFromCommandDbAsync(options, 1);
        await Assert.That(entity!.Name).IsEqualTo("Final Update");

        connection.Close();
    }
}



