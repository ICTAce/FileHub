// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Client.Tests.Modules.SampleModule;

public class EditTests : BaseTest
{
    private readonly MockNavigationManager? _mockNavigationManager;
    private readonly MockSampleModuleService? _mockSampleModuleService;

    public EditTests()
    {
        _mockNavigationManager = TestContext.Services.GetRequiredService<NavigationManager>() as MockNavigationManager;
        _mockSampleModuleService = TestContext.Services.GetRequiredService<ISampleModuleService>() as MockSampleModuleService;
        TestContext.JSInterop.Setup<bool>("Oqtane.Interop.formValid", _ => true).SetResult(true);
    }

    /// <summary>
    /// Verifies that service dependencies are properly configured
    /// </summary>
    [Test]
    public async Task EditComponent_ServiceDependencies_AreConfigured()
    {
        await Assert.That(_mockSampleModuleService).IsNotNull();
        await Assert.That(_mockNavigationManager).IsNotNull();
        
        var logService = TestContext.Services.GetService<ILogService>();
        await Assert.That(logService).IsNotNull();
    }

    /// <summary>
    /// Tests Add mode - service layer creates new module
    /// </summary>
    [Test]
    public async Task ServiceLayer_CreateAsync_AddsNewModule()
    {
        var initialCount = _mockSampleModuleService!.GetModuleCount();
        
        var dto = new CreateAndUpdateSampleModuleDto
        {
            Name = "New Test Module"
        };
        
        var newId = await _mockSampleModuleService.CreateAsync(1, dto);
        
        await Assert.That(newId).IsGreaterThan(0);
        await Assert.That(_mockSampleModuleService.GetModuleCount()).IsEqualTo(initialCount + 1);
        
        var created = await _mockSampleModuleService.GetAsync(newId, 1);
        await Assert.That(created.Name).IsEqualTo("New Test Module");
    }

    /// <summary>
    /// Tests Edit mode - service layer updates existing module
    /// </summary>
    [Test]
    public async Task ServiceLayer_UpdateAsync_ModifiesExistingModule()
    {
        var dto = new CreateAndUpdateSampleModuleDto
        {
            Name = "Updated Module Name"
        };
        
        await _mockSampleModuleService!.UpdateAsync(1, 1, dto);
        
        var updated = await _mockSampleModuleService.GetAsync(1, 1);
        await Assert.That(updated.Name).IsEqualTo("Updated Module Name");
        await Assert.That(updated.Id).IsEqualTo(1);
    }

    /// <summary>
    /// Verifies that GetAsync retrieves correct module data
    /// </summary>
    [Test]
    public async Task ServiceLayer_GetAsync_ReturnsCorrectModule()
    {
        var module1 = await _mockSampleModuleService!.GetAsync(1, 1);
        var module2 = await _mockSampleModuleService.GetAsync(2, 1);
        
        await Assert.That(module1.Id).IsEqualTo(1);
        await Assert.That(module1.Name).IsEqualTo("Test Module 1");
        await Assert.That(module2.Id).IsEqualTo(2);
        await Assert.That(module2.Name).IsEqualTo("Test Module 2");
    }

    /// <summary>
    /// Tests that PageState is properly configured for Add mode
    /// </summary>
    [Test]
    public async Task PageState_AddMode_IsConfigured()
    {
        var pageState = CreatePageState("Add");
        
        await Assert.That(pageState.Action).IsEqualTo("Add");
        await Assert.That(pageState.QueryString).IsNotNull();
        await Assert.That(pageState.QueryString.Count).IsEqualTo(0);
    }

    /// <summary>
    /// Tests that PageState is properly configured for Edit mode
    /// </summary>
    [Test]
    public async Task PageState_EditMode_IsConfigured()
    {
        var queryString = new Dictionary<string, string>
        {
            { "id", "1" }
        };
        
        var pageState = CreatePageState("Edit", queryString);
        
        await Assert.That(pageState.Action).IsEqualTo("Edit");
        await Assert.That(pageState.QueryString).IsNotNull();
        await Assert.That(pageState.QueryString.ContainsKey("id")).IsTrue();
        await Assert.That(pageState.QueryString["id"]).IsEqualTo("1");
    }

    /// <summary>
    /// Tests navigation manager reset functionality
    /// </summary>
    [Test]
    public async Task NavigationManager_Reset_ClearsHistory()
    {
        _mockNavigationManager!.Reset();
        
        await Assert.That(_mockNavigationManager.Uri).IsEqualTo("https://localhost:5001/");
        await Assert.That(_mockNavigationManager.BaseUri).IsEqualTo("https://localhost:5001/");
    }

    /// <summary>
    /// Tests that module state includes all required properties for Edit component
    /// </summary>
    [Test]
    public async Task ModuleState_ForEditComponent_HasRequiredProperties()
    {
        var moduleState = CreateModuleState(1, 1, "Test Module");
        
        await Assert.That(moduleState.ModuleId).IsEqualTo(1);
        await Assert.That(moduleState.PageId).IsEqualTo(1);
        await Assert.That(moduleState.ModuleDefinition).IsNotNull();
        await Assert.That(moduleState.PermissionList).IsNotNull();
        await Assert.That(moduleState.PermissionList.Any(p => p.PermissionName == "Edit")).IsTrue();
    }

    /// <summary>
    /// Tests form validation - valid data
    /// </summary>
    [Test]
    public async Task FormValidation_ValidData_Passes()
    {
        var dto = new CreateAndUpdateSampleModuleDto
        {
            Name = "Valid Name"
        };
        
        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(dto);
        var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(dto, context, validationResults, true);
        
        await Assert.That(isValid).IsTrue();
        await Assert.That(validationResults.Count).IsEqualTo(0);
    }

    /// <summary>
    /// Tests form validation - empty name
    /// </summary>
    [Test]
    public async Task FormValidation_EmptyName_Fails()
    {
        var dto = new CreateAndUpdateSampleModuleDto
        {
            Name = string.Empty
        };
        
        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(dto);
        var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(dto, context, validationResults, true);
        
        await Assert.That(isValid).IsFalse();
        await Assert.That(validationResults.Count).IsGreaterThan(0);
    }

    /// <summary>
    /// Tests form validation - name too long
    /// </summary>
    [Test]
    public async Task FormValidation_NameTooLong_Fails()
    {
        var dto = new CreateAndUpdateSampleModuleDto
        {
            Name = new string('A', 101) // 101 characters, exceeds max of 100
        };
        
        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(dto);
        var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(dto, context, validationResults, true);
        
        await Assert.That(isValid).IsFalse();
        await Assert.That(validationResults.Any(v => v.ErrorMessage?.Contains("100") == true)).IsTrue();
    }
}
