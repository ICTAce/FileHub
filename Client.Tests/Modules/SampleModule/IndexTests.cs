// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Client.Tests.Modules.SampleModule;

public class IndexTests : BaseTest
{
    [Test]
    public async Task MockService_HasTestData()
    {
        var mockService = TestContext.Services.GetRequiredService<ISampleModuleService>() as MockSampleModuleService;

        await Assert.That(mockService).IsNotNull();
        await Assert.That(mockService!.GetModuleCount()).IsEqualTo(2);

        var result = await mockService.ListAsync(1, 1, 10);
        await Assert.That(result.TotalCount).IsEqualTo(2);
        await Assert.That(result.Items.Count()).IsEqualTo(2);
    }

    [Test]
    public async Task IndexComponent_ServiceDependencies_CanBeResolved()
    {
        var sampleModuleService = TestContext.Services.GetService<ISampleModuleService>();
        var navigationManager = TestContext.Services.GetService<NavigationManager>();
        var logService = TestContext.Services.GetService<ILogService>();

        await Assert.That(sampleModuleService).IsNotNull();
        await Assert.That(navigationManager).IsNotNull();
        await Assert.That(logService).IsNotNull();
    }

    [Test]
    public async Task ModuleState_ShouldBeInitialized()
    {
        var moduleState = CreateModuleState();

        await Assert.That(moduleState.ModuleId).IsEqualTo(1);
        await Assert.That(moduleState.PageId).IsEqualTo(1);
        await Assert.That(moduleState.ModuleDefinition).IsNotNull();
        await Assert.That(moduleState.ModuleDefinition?.ModuleDefinitionName).IsEqualTo("ICTAce.FileHub.SampleModule");
        await Assert.That(moduleState.PermissionList).IsNotNull();
        await Assert.That(moduleState.Settings).IsNotNull();
    }

    [Test]
    public async Task PageState_ShouldBeConfigured()
    {
        var pageState = CreatePageState("Index");

        await Assert.That(pageState.Action).IsEqualTo("Index");
        await Assert.That(pageState.ModuleId).IsEqualTo(1);
        await Assert.That(pageState.PageId).IsEqualTo(1);
        await Assert.That(pageState.Page).IsNotNull();
        await Assert.That(pageState.Alias).IsNotNull();
        await Assert.That(pageState.Site).IsNotNull();
    }

    [Test]
    public async Task ServiceLayer_ListAsync_ReturnsModules()
    {
        var mockService = TestContext.Services.GetRequiredService<ISampleModuleService>() as MockSampleModuleService;

        var result = await mockService!.ListAsync(1, 1, 10);

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Items).IsNotNull();
        await Assert.That(result.Items.Any(m => m.Name == "Test Module 1")).IsTrue();
        await Assert.That(result.Items.Any(m => m.Name == "Test Module 2")).IsTrue();
    }

    [Test]
    public async Task ServiceLayer_DeleteAsync_RemovesModule()
    {
        var mockService = TestContext.Services.GetRequiredService<ISampleModuleService>() as MockSampleModuleService;

        var initialCount = mockService!.GetModuleCount();
        await mockService.DeleteAsync(1, 1);
        var finalCount = mockService.GetModuleCount();

        await Assert.That(finalCount).IsEqualTo(initialCount - 1);
    }

    [Test]
    public async Task ServiceLayer_ListAsync_SupportsPagination()
    {
        var mockService = TestContext.Services.GetRequiredService<ISampleModuleService>() as MockSampleModuleService;

        mockService!.AddTestData(new GetSampleModuleDto
        {
            Id = 3,
            ModuleId = 1,
            Name = "Test Module 3",
            CreatedBy = "Test User",
            CreatedOn = DateTime.Now,
            ModifiedBy = "Test User",
            ModifiedOn = DateTime.Now
        });

        var page1 = await mockService.ListAsync(1, 1, 2);
        var page2 = await mockService.ListAsync(1, 2, 2);

        await Assert.That(page1.Items.Count()).IsEqualTo(2);
        await Assert.That(page2.Items.Count()).IsEqualTo(1);
        await Assert.That(page1.TotalCount).IsEqualTo(3);
    }
}
