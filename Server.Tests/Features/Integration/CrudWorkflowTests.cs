// Licensed to ICTAce under the MIT license.

using static ICTAce.FileHub.Server.Tests.Helpers.SampleModuleTestHelpers;
using CategoryHandlers = ICTAce.FileHub.Features.Categories;

namespace ICTAce.FileHub.Server.Tests.Features.Integration;

/// <summary>
/// Integration tests that verify complete CRUD workflows work correctly end-to-end.
/// </summary>
public class CrudWorkflowTests : HandlerTestBase
{
    [Test]
    public async Task CompleteWorkflow_CreateGetUpdateDelete_AllOperationsSucceed()
    {
        // Arrange
        var (connection, commandOptions) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        var (_, queryOptions) = await CreateQueryDatabaseAsync().ConfigureAwait(false);

        var createHandler = new CreateHandler(
            CreateCommandHandlerServices(commandOptions, isAuthorized: true));
        var getHandler = new GetHandler(
            CreateQueryHandlerServices(queryOptions, isAuthorized: true));
        var updateHandler = new UpdateHandler(
            CreateCommandHandlerServices(commandOptions, isAuthorized: true));
        var deleteHandler = new DeleteHandler(
            CreateCommandHandlerServices(commandOptions, isAuthorized: true));

        // Act & Assert - CREATE
        var createRequest = new CreateSampleModuleRequest
        {
            ModuleId = 1,
            Name = "New Entity"
        };
        var createdId = await createHandler.Handle(createRequest, CancellationToken.None).ConfigureAwait(false);
        await Assert.That(createdId).IsGreaterThan(0);

        // Seed query database for GET
        await Helpers.SampleModuleTestHelpers.SeedQueryDataAsync(queryOptions,
            CreateTestEntity(id: createdId, moduleId: 1, name: "New Entity")).ConfigureAwait(false);

        // Act & Assert - GET
        var getRequest = new GetSampleModuleRequest
        {
            Id = createdId,
            ModuleId = 1
        };
        var getResult = await getHandler.Handle(getRequest, CancellationToken.None).ConfigureAwait(false);
        await Assert.That(getResult).IsNotNull();
        await Assert.That(getResult!.Name).IsEqualTo("New Entity");

        // Act & Assert - UPDATE
        var updateRequest = new UpdateSampleModuleRequest
        {
            Id = createdId,
            ModuleId = 1,
            Name = "Updated Entity"
        };
        var updateResult = await updateHandler.Handle(updateRequest, CancellationToken.None).ConfigureAwait(false);
        await Assert.That(updateResult).IsEqualTo(createdId);

        var updatedEntity = await GetFromCommandDbAsync(commandOptions, createdId).ConfigureAwait(false);
        await Assert.That(updatedEntity!.Name).IsEqualTo("Updated Entity");

        // Act & Assert - DELETE
        var deleteRequest = new DeleteSampleModuleRequest
        {
            Id = createdId,
            ModuleId = 1
        };
        var deleteResult = await deleteHandler.Handle(deleteRequest, CancellationToken.None).ConfigureAwait(false);
        await Assert.That(deleteResult).IsEqualTo(createdId);

        var deletedEntity = await GetFromCommandDbAsync(commandOptions, createdId).ConfigureAwait(false);
        await Assert.That(deletedEntity).IsNull();

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task CreateMultiple_List_ReturnsPaginatedResults()
    {
        // Arrange
        var (connection, queryOptions) = await CreateQueryDatabaseAsync().ConfigureAwait(false);
        await Helpers.SampleModuleTestHelpers.SeedQueryDataAsync(queryOptions,
            CreateTestEntity(id: 1, moduleId: 1, name: "Entity 1"),
            CreateTestEntity(id: 2, moduleId: 1, name: "Entity 2"),
            CreateTestEntity(id: 3, moduleId: 1, name: "Entity 3"),
            CreateTestEntity(id: 4, moduleId: 1, name: "Entity 4"),
            CreateTestEntity(id: 5, moduleId: 1, name: "Entity 5")).ConfigureAwait(false);

        var listHandler = new ListHandler(
            CreateQueryHandlerServices(queryOptions, isAuthorized: true));

        // Act - Page 1
        var page1Request = new ListSampleModuleRequest
        {
            ModuleId = 1,
            PageNumber = 1,
            PageSize = 2
        };
        var page1Result = await listHandler.Handle(page1Request, CancellationToken.None).ConfigureAwait(false);

        // Assert - Page 1
        await Assert.That(page1Result).IsNotNull();
        await Assert.That(page1Result!.Items.Count()).IsEqualTo(2);
        await Assert.That(page1Result.TotalCount).IsEqualTo(5);
        await Assert.That(page1Result.PageNumber).IsEqualTo(1);

        // Act - Page 2
        var page2Request = new ListSampleModuleRequest
        {
            ModuleId = 1,
            PageNumber = 2,
            PageSize = 2
        };
        var page2Result = await listHandler.Handle(page2Request, CancellationToken.None).ConfigureAwait(false);

        // Assert - Page 2
        await Assert.That(page2Result).IsNotNull();
        await Assert.That(page2Result!.Items.Count()).IsEqualTo(2);
        await Assert.That(page2Result.TotalCount).IsEqualTo(5);
        await Assert.That(page2Result.PageNumber).IsEqualTo(2);

        // Act - Page 3 (last page with 1 item)
        var page3Request = new ListSampleModuleRequest
        {
            ModuleId = 1,
            PageNumber = 3,
            PageSize = 2
        };
        var page3Result = await listHandler.Handle(page3Request, CancellationToken.None).ConfigureAwait(false);

        // Assert - Page 3
        await Assert.That(page3Result).IsNotNull();
        await Assert.That(page3Result!.Items.Count()).IsEqualTo(1);
        await Assert.That(page3Result.TotalCount).IsEqualTo(5);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task CreateUpdateGet_VerifiesChanges()
    {
        // Arrange
        var (connection, commandOptions) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        var (_, queryOptions) = await CreateQueryDatabaseAsync().ConfigureAwait(false);

        var createHandler = new CreateHandler(
            CreateCommandHandlerServices(commandOptions, isAuthorized: true));
        var updateHandler = new UpdateHandler(
            CreateCommandHandlerServices(commandOptions, isAuthorized: true));
        var getHandler = new GetHandler(
            CreateQueryHandlerServices(queryOptions, isAuthorized: true));

        // Act - Create
        var id = await createHandler.Handle(new CreateSampleModuleRequest
        {
            ModuleId = 1,
            Name = "Original"
        }, CancellationToken.None).ConfigureAwait(false);

        // Seed query database
        await Helpers.SampleModuleTestHelpers.SeedQueryDataAsync(queryOptions,
            CreateTestEntity(id: id, moduleId: 1, name: "Original")).ConfigureAwait(false);

        // Act - Update
        await updateHandler.Handle(new UpdateSampleModuleRequest
        {
            Id = id,
            ModuleId = 1,
            Name = "Modified"
        }, CancellationToken.None).ConfigureAwait(false);

        // Update query database to match command changes
        using (var context = new Helpers.TestApplicationQueryContext(queryOptions))
        {
            var entity = await context.SampleModule.FindAsync(id).ConfigureAwait(false);
            if (entity != null)
            {
                entity.Name = "Modified";
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        // Act - Get
        var result = await getHandler.Handle(new GetSampleModuleRequest
        {
            Id = id,
            ModuleId = 1
        }, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result!.Name).IsEqualTo("Modified");

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task DeleteNonExistent_AfterDelete_ReturnsMinusOne()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        await SeedCommandDataAsync(options, CreateTestEntity(id: 1, moduleId: 1)).ConfigureAwait(false);

        var deleteHandler = new DeleteHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        // Act - First delete succeeds
        var firstDeleteResult = await deleteHandler.Handle(new DeleteSampleModuleRequest
        {
            Id = 1,
            ModuleId = 1
        }, CancellationToken.None).ConfigureAwait(false);

        await Assert.That(firstDeleteResult).IsEqualTo(1);

        // Act - Second delete fails (already deleted)
        var secondDeleteResult = await deleteHandler.Handle(new DeleteSampleModuleRequest
        {
            Id = 1,
            ModuleId = 1
        }, CancellationToken.None).ConfigureAwait(false);

        // Assert
        await Assert.That(secondDeleteResult).IsEqualTo(-1);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task MultipleCreates_AllSucceed_WithIncrementingIds()
    {
        // Arrange
        var (connection, options) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        var createHandler = new CreateHandler(
            CreateCommandHandlerServices(options, isAuthorized: true));

        // Act - Create 5 entities
        var id1 = await createHandler.Handle(new CreateSampleModuleRequest
        { ModuleId = 1, Name = "Entity 1" }, CancellationToken.None).ConfigureAwait(false);

        var id2 = await createHandler.Handle(new CreateSampleModuleRequest
        { ModuleId = 1, Name = "Entity 2" }, CancellationToken.None).ConfigureAwait(false);

        var id3 = await createHandler.Handle(new CreateSampleModuleRequest
        { ModuleId = 1, Name = "Entity 3" }, CancellationToken.None).ConfigureAwait(false);

        var id4 = await createHandler.Handle(new CreateSampleModuleRequest
        { ModuleId = 1, Name = "Entity 4" }, CancellationToken.None).ConfigureAwait(false);

        var id5 = await createHandler.Handle(new CreateSampleModuleRequest
        { ModuleId = 1, Name = "Entity 5" }, CancellationToken.None).ConfigureAwait(false);

        // Assert - All IDs are unique and incrementing
        await Assert.That(id1).IsGreaterThan(0);
        await Assert.That(id2).IsGreaterThan(id1);
        await Assert.That(id3).IsGreaterThan(id2);
        await Assert.That(id4).IsGreaterThan(id3);
        await Assert.That(id5).IsGreaterThan(id4);

        var count = await GetCountFromCommandDbAsync(options).ConfigureAwait(false);
        await Assert.That(count).IsEqualTo(5);

        await connection.CloseAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task CategoryWorkflow_CreateUpdateListDelete_CompleteSuccess()
    {
        // Arrange
        var (connection, commandOptions) = await CreateCommandDatabaseAsync().ConfigureAwait(false);
        var (_, queryOptions) = await CreateQueryDatabaseAsync().ConfigureAwait(false);

        var createHandler = new CategoryHandlers.CreateHandler(
            CreateCommandHandlerServices(commandOptions, isAuthorized: true));
        var updateHandler = new CategoryHandlers.UpdateHandler(
            CreateCommandHandlerServices(commandOptions, isAuthorized: true));
        var listHandler = new CategoryHandlers.ListHandler(
            CreateQueryHandlerServices(queryOptions, isAuthorized: true));
        var deleteHandler = new CategoryHandlers.DeleteHandler(
            CreateCommandHandlerServices(commandOptions, isAuthorized: true));

        // Act - Create
        var id = await createHandler.Handle(new CategoryHandlers.CreateCategoryRequest
        {
            ModuleId = 1,
            Name = "Test Category",
            ViewOrder = 1,
            ParentId = 0
        }, CancellationToken.None).ConfigureAwait(false);

        await Assert.That(id).IsGreaterThan(0);

        // Seed query database
        await Helpers.CategoryTestHelpers.SeedQueryDataAsync(queryOptions,
            Helpers.CategoryTestHelpers.CreateTestEntity(id: id, moduleId: 1, name: "Test Category", viewOrder: 1)).ConfigureAwait(false);

        // Act - Update
        var updateResult = await updateHandler.Handle(new CategoryHandlers.UpdateCategoryRequest
        {
            Id = id,
            ModuleId = 1,
            Name = "Updated Category",
            ViewOrder = 5,
            ParentId = 0
        }, CancellationToken.None).ConfigureAwait(false);

        await Assert.That(updateResult).IsEqualTo(id);

        // Update query database
        using (var context = new Helpers.TestApplicationQueryContext(queryOptions))
        {
            var entity = await context.Category.FindAsync(id).ConfigureAwait(false);
            if (entity != null)
            {
                entity.Name = "Updated Category";
                entity.ViewOrder = 5;
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        // Act - List
        var listResult = await listHandler.Handle(new CategoryHandlers.ListCategoryRequest
        {
            ModuleId = 1,
            PageNumber = 1,
            PageSize = 10
        }, CancellationToken.None).ConfigureAwait(false);

        await Assert.That(listResult).IsNotNull();
        await Assert.That(listResult!.TotalCount).IsEqualTo(1);
        var items = listResult.Items.ToList();
        await Assert.That(items[0].Name).IsEqualTo("Updated Category");

        // Act - Delete
        var deleteResult = await deleteHandler.Handle(new CategoryHandlers.DeleteCategoryRequest
        {
            Id = id,
            ModuleId = 1
        }, CancellationToken.None).ConfigureAwait(false);

        await Assert.That(deleteResult).IsEqualTo(id);

        await connection.CloseAsync().ConfigureAwait(false);
    }
}
