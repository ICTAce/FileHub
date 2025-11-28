// Licensed to ICTAce under the MIT license.

using static ICTAce.FileHub.Server.Tests.Helpers.SampleModuleTestHelpers;

namespace ICTAce.FileHub.Server.Tests.Features.SampleModule;

public class GetHandlerTests : HandlerTestBase
{
    [Test]
    public async Task Handle_WithValidId_ReturnsSampleModule()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync();
        await SeedQueryDataAsync(options, CreateTestEntity());

        var handler = new GetHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        var request = new GetSampleModuleRequest { ModuleId = 1, Id = 1 };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Id).IsEqualTo(1);
        await Assert.That(result.Name).IsEqualTo("Test Module");
        await Assert.That(result.ModuleId).IsEqualTo(1);

        connection.Close();
    }

    [Test]
    public async Task Handle_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync();
        await SeedQueryDataAsync(options, CreateTestEntity());

        var handler = new GetHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        var request = new GetSampleModuleRequest { ModuleId = 1, Id = 999 };

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

        var handler = new GetHandler(
            CreateQueryHandlerServices(options, isAuthorized: false));

        var request = new GetSampleModuleRequest { ModuleId = 1, Id = 1 };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsNull();

        connection.Close();
    }

    [Test]
    public async Task Handle_WithWrongModuleId_ReturnsNull()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync();
        await SeedQueryDataAsync(options, CreateTestEntity());

        var handler = new GetHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        var request = new GetSampleModuleRequest { ModuleId = 2, Id = 1 };

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
            CreateTestEntity(
                id: 1, 
                moduleId: 1, 
                name: "Test Module", 
                createdBy: "creator", 
                createdOn: createdOn,
                modifiedBy: "modifier", 
                modifiedOn: modifiedOn));

        var handler = new GetHandler(
            CreateQueryHandlerServices(options, isAuthorized: true));

        var request = new GetSampleModuleRequest { ModuleId = 1, Id = 1 };

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
}

