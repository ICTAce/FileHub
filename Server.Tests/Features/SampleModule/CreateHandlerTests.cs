// Licensed to ICTAce under the MIT license.

using static ICTAce.FileHub.Server.Tests.Helpers.SampleModuleTestHelpers;
// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Server.Tests.Features.SampleModule;

public class CreateHandlerTests : HandlerTestBase
{
    [Test]
    public async Task Handle_WithValidRequest_CreatesSampleModule()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);

        var handler = new CreateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new CreateSampleModuleRequest
        {
            ModuleId = 1,
            Name = "Test Sample Module"
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsGreaterThan(0);

        var savedEntity = await GetFromCommandDbAsync(options, result).ConfigureAwait(false);
        await Assert.That(savedEntity).IsNotNull();
        await Assert.That(savedEntity!.Name).IsEqualTo("Test Sample Module");
        await Assert.That(savedEntity.ModuleId).IsEqualTo(1);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Handle_WithUnauthorizedUser_ReturnsMinusOne()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);

        var handler = new CreateHandler(
            CreateCommandHandlerServices(options, isAuthorized: false));

        var request = new CreateSampleModuleRequest
        {
            ModuleId = 1,
            Name = "Unauthorized Module"
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
    [Arguments("")]
    [Arguments("Valid Name")]
    [Arguments("123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890")]
    public async Task Handle_WithDifferentNames_CreatesSuccessfully(string name)
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);

        var handler = new CreateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        var request = new CreateSampleModuleRequest
        {
            ModuleId = 1,
            Name = name
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsGreaterThan(0);

        var savedEntity = await GetFromCommandDbAsync(options, result).ConfigureAwait(false);
        await Assert.That(savedEntity).IsNotNull();
        await Assert.That(savedEntity!.Name).IsEqualTo(name);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task Handle_WithMultipleModules_CreatesAllSuccessfully()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);

        var handler = new CreateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        // Act
        var id1 = await handler.Handle(new CreateSampleModuleRequest { ModuleId = 1, Name = "Module 1" }, CancellationToken.None).ConfigureAwait(false);
        var id2 = await handler.Handle(new CreateSampleModuleRequest { ModuleId = 1, Name = "Module 2" }, CancellationToken.None).ConfigureAwait(false);
        var id3 = await handler.Handle(new CreateSampleModuleRequest { ModuleId = 2, Name = "Module 3" }, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(id1).IsGreaterThan(0);
        await Assert.That(id2).IsGreaterThan(0);
        await Assert.That(id3).IsGreaterThan(0);
        await Assert.That(id1).IsNotEqualTo(id2);
        await Assert.That(id2).IsNotEqualTo(id3);

        var count = await GetCountFromCommandDbAsync(options).ConfigureAwait(false);
        await Assert.That(count).IsEqualTo(3);

        await connection.CloseAsync().ConfigureAwait(false);
    }
}
