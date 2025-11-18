// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Client.Tests.Modules;

public class MyModuleTests : BaseTest
{
    [Test]
    public async Task MyModuleService_List_ReturnsModules()
    {
        var request = new ListMyModulesRequest { ModuleId = 1 };
        var mockService = TestContext.Services.GetService<IMyModuleService>() as MockMyModuleService;
        
        var result = await mockService!.ListAsync(request);
        
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Items.Count()).IsEqualTo(mockService.GetModuleCount());
        await Assert.That(result.Items.First().Name).IsEqualTo("Test Module 1");
    }

    [Test]
    public async Task MyModuleService_Get_ReturnsModule()
    {
        var request = new GetMyModuleRequest { Id = 1, ModuleId = 1 };
        var mockService = TestContext.Services.GetService<IMyModuleService>() as MockMyModuleService;
        
        var result = await mockService!.GetAsync(request);
        
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Id).IsEqualTo(1);
        await Assert.That(result.Name).IsEqualTo("Test Module 1");
    }

    [Test]
    public async Task MyModuleService_Create_AddsNewModule()
    {
        var mockService = TestContext.Services.GetService<IMyModuleService>() as MockMyModuleService;
        var initialCount = mockService!.GetModuleCount();
        
        var createRequest = new CreateMyModuleRequest 
        { 
            ModuleId = 1, 
            Name = "New Module" 
        };
        var newId = await mockService.CreateAsync(createRequest);
        
        await Assert.That(newId).IsGreaterThan(0);
        await Assert.That(mockService.GetModuleCount()).IsEqualTo(initialCount + 1);
        
        var listRequest = new ListMyModulesRequest { ModuleId = 1 };
        var allModules = await mockService.ListAsync(listRequest);
        
        await Assert.That(allModules.Items.Any(m => m.Name == "New Module")).IsTrue();
    }

    [Test]
    public async Task MyModuleService_Update_ModifiesExistingModule()
    {
        var mockService = TestContext.Services.GetService<IMyModuleService>() as MockMyModuleService;
        
        var updateRequest = new UpdateMyModuleRequest 
        { 
            Id = 1, 
            ModuleId = 1, 
            Name = "Updated Module Name" 
        };
        await mockService!.UpdateAsync(updateRequest);
        
        var getRequest = new GetMyModuleRequest { Id = 1, ModuleId = 1 };
        var updatedModule = await mockService.GetAsync(getRequest);
        
        await Assert.That(updatedModule.Name).IsEqualTo("Updated Module Name");
    }

    [Test]
    public async Task MyModuleService_Delete_RemovesModule()
    {
        var mockService = TestContext.Services.GetService<IMyModuleService>() as MockMyModuleService;
        var initialCount = mockService!.GetModuleCount();
        
        var deleteRequest = new DeleteMyModuleRequest { Id = 1, ModuleId = 1 };
        await mockService.DeleteAsync(deleteRequest);
        
        await Assert.That(mockService.GetModuleCount()).IsEqualTo(initialCount - 1);
        
        var listRequest = new ListMyModulesRequest { ModuleId = 1 };
        var remainingModules = await mockService.ListAsync(listRequest);
        
        await Assert.That(remainingModules.Items.Any(m => m.Id == 1)).IsFalse();
    }

    [Test]
    public async Task MyModuleService_DeleteMultiple_RemovesAllModules()
    {
        var mockService = TestContext.Services.GetService<IMyModuleService>() as MockMyModuleService;
        
        await mockService!.DeleteAsync(new DeleteMyModuleRequest { Id = 1, ModuleId = 1 });
        await mockService.DeleteAsync(new DeleteMyModuleRequest { Id = 2, ModuleId = 1 });
        
        await Assert.That(mockService.GetModuleCount()).IsEqualTo(0);
    }

    [Test]
    public async Task MyModuleService_Get_NonExistentModule_ThrowsException()
    {
        var mockService = TestContext.Services.GetService<IMyModuleService>() as MockMyModuleService;
        var request = new GetMyModuleRequest { Id = 999, ModuleId = 1 };
        
        var exceptionThrown = false;
        try
        {
            await mockService!.GetAsync(request);
        }
        catch (System.InvalidOperationException ex)
        {
            exceptionThrown = true;
            await Assert.That(ex.Message).Contains("not found");
        }
        
        await Assert.That(exceptionThrown).IsTrue();
    }

    [Test]
    public async Task MyModuleService_CreateMultiple_AllAreStored()
    {
        var mockService = TestContext.Services.GetService<IMyModuleService>() as MockMyModuleService;
        var initialCount = mockService!.GetModuleCount();
        
        await mockService.CreateAsync(new CreateMyModuleRequest { ModuleId = 1, Name = "Module A" });
        await mockService.CreateAsync(new CreateMyModuleRequest { ModuleId = 1, Name = "Module B" });
        await mockService.CreateAsync(new CreateMyModuleRequest { ModuleId = 1, Name = "Module C" });
        
        await Assert.That(mockService.GetModuleCount()).IsEqualTo(initialCount + 3);
    }
}
