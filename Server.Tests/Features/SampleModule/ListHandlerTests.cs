// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Server.Tests.Features.SampleModule;

public class ListHandlerTests : HandlerTestBase
{
    [Test]
    public async Task Handle_WithData_ReturnsPagedResult()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync();
        await SeedQueryDataAsync(options,
            CreateTestEntity(id: 1, name: "Module 1"),
            CreateTestEntity(id: 2, name: "Module 2"),
            CreateTestEntity(id: 3, name: "Module 3"),
            CreateTestEntity(id: 4, name: "Module 4"),
            CreateTestEntity(id: 5, name: "Module 5"));

        var handler = new ListHandler(
            CreateMockQueryContextFactory(options),
            CreateMockUserPermissions(isAuthorized: true),
            CreateMockTenantManager(),
            CreateMockHttpContextAccessor(),
            CreateMockLogger());

        var request = new ListSampleModuleRequest
        {
            ModuleId = 1,
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.TotalCount).IsEqualTo(5);
        await Assert.That(result.Items.Count()).IsEqualTo(5);
        await Assert.That(result.PageNumber).IsEqualTo(1);
        await Assert.That(result.PageSize).IsEqualTo(10);

        connection.Close();
    }

    [Test]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync();
        await SeedQueryDataAsync(options,
            CreateTestEntity(id: 1, name: "Alpha"),
            CreateTestEntity(id: 2, name: "Bravo"),
            CreateTestEntity(id: 3, name: "Charlie"),
            CreateTestEntity(id: 4, name: "Delta"),
            CreateTestEntity(id: 5, name: "Echo"));

        var handler = new ListHandler(
            CreateMockQueryContextFactory(options),
            CreateMockUserPermissions(isAuthorized: true),
            CreateMockTenantManager(),
            CreateMockHttpContextAccessor(),
            CreateMockLogger());

        var request = new ListSampleModuleRequest
        {
            ModuleId = 1,
            PageNumber = 2,
            PageSize = 2
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.TotalCount).IsEqualTo(5);
        await Assert.That(result.Items.Count()).IsEqualTo(2);
        await Assert.That(result.PageNumber).IsEqualTo(2);
        await Assert.That(result.PageSize).IsEqualTo(2);
        
        // Items should be "Charlie" and "Delta" (sorted alphabetically, page 2)
        var items = result.Items.ToList();
        await Assert.That(items[0].Name).IsEqualTo("Charlie");
        await Assert.That(items[1].Name).IsEqualTo("Delta");

        connection.Close();
    }

    [Test]
    public async Task Handle_WithUnauthorizedUser_ReturnsNull()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync();

        var handler = new ListHandler(
            CreateMockQueryContextFactory(options),
            CreateMockUserPermissions(isAuthorized: false),
            CreateMockTenantManager(),
            CreateMockHttpContextAccessor(),
            CreateMockLogger());

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

    [Test]
    public async Task Handle_WithNoData_ReturnsEmptyList()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync();

        var handler = new ListHandler(
            CreateMockQueryContextFactory(options),
            CreateMockUserPermissions(isAuthorized: true),
            CreateMockTenantManager(),
            CreateMockHttpContextAccessor(),
            CreateMockLogger());

        var request = new ListSampleModuleRequest
        {
            ModuleId = 1,
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.TotalCount).IsEqualTo(0);
        await Assert.That(result.Items.Count()).IsEqualTo(0);

        connection.Close();
    }

    [Test]
    public async Task Handle_WithMultipleModules_FiltersCorrectly()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync();
        await SeedQueryDataAsync(options,
            CreateTestEntity(id: 1, moduleId: 1, name: "Module 1-1"),
            CreateTestEntity(id: 2, moduleId: 1, name: "Module 1-2"),
            CreateTestEntity(id: 3, moduleId: 2, name: "Module 2-1"),
            CreateTestEntity(id: 4, moduleId: 2, name: "Module 2-2"));

        var handler = new ListHandler(
            CreateMockQueryContextFactory(options),
            CreateMockUserPermissions(isAuthorized: true),
            CreateMockTenantManager(),
            CreateMockHttpContextAccessor(),
            CreateMockLogger());

        var request = new ListSampleModuleRequest
        {
            ModuleId = 1,
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.TotalCount).IsEqualTo(2);
        await Assert.That(result.Items.Count()).IsEqualTo(2);
        
        var items = result.Items.ToList();
        await Assert.That(items.All(x => x.Name.StartsWith("Module 1"))).IsTrue();

        connection.Close();
    }

    [Test]
    public async Task Handle_VerifiesAlphabeticalOrdering()
    {
        // Arrange
        var (connection, options) = await CreateQueryDatabaseAsync();
        await SeedQueryDataAsync(options,
            CreateTestEntity(id: 1, name: "Zebra"),
            CreateTestEntity(id: 2, name: "Apple"),
            CreateTestEntity(id: 3, name: "Mango"));

        var handler = new ListHandler(
            CreateMockQueryContextFactory(options),
            CreateMockUserPermissions(isAuthorized: true),
            CreateMockTenantManager(),
            CreateMockHttpContextAccessor(),
            CreateMockLogger());

        var request = new ListSampleModuleRequest
        {
            ModuleId = 1,
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        await Assert.That(result).IsNotNull();
        var items = result!.Items.ToList();
        await Assert.That(items[0].Name).IsEqualTo("Apple");
        await Assert.That(items[1].Name).IsEqualTo("Mango");
        await Assert.That(items[2].Name).IsEqualTo("Zebra");

        connection.Close();
    }
}



