// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Client.Tests.Modules.FileHub;

public class CategoryTests : BaseTest
{
    private readonly MockNavigationManager? _mockNavigationManager;
    private readonly MockCategoryService? _mockCategoryService;

    public CategoryTests()
    {
        _mockNavigationManager = TestContext.Services.GetRequiredService<NavigationManager>() as MockNavigationManager;
        _mockCategoryService = TestContext.Services.GetRequiredService<ICategoryService>() as MockCategoryService;
        TestContext.JSInterop.Setup<bool>("Oqtane.Interop.formValid", _ => true).SetResult(true);
    }

    #region Service Dependency Tests

    [Test]
    public async Task CategoryComponent_ServiceDependencies_AreConfigured()
    {
        await Assert.That(_mockCategoryService).IsNotNull();
        await Assert.That(_mockNavigationManager).IsNotNull();

        var logService = TestContext.Services.GetService<ILogService>();
        await Assert.That(logService).IsNotNull();
    }

    #endregion

    #region Service Layer Tests - CRUD Operations

    [Test]
    public async Task ServiceLayer_CreateAsync_AddsNewCategory()
    {
        var initialCount = _mockCategoryService!.GetCategoryCount();

        var dto = new CreateAndUpdateCategoryDto
        {
            Name = "New Test Category",
            ViewOrder = 2,
            ParentId = 0
        };

        var newId = await _mockCategoryService.CreateAsync(1, dto);

        await Assert.That(newId).IsGreaterThan(0);
        await Assert.That(_mockCategoryService.GetCategoryCount()).IsEqualTo(initialCount + 1);

        var created = await _mockCategoryService.GetAsync(newId, 1);
        await Assert.That(created.Name).IsEqualTo("New Test Category");
        await Assert.That(created.ViewOrder).IsEqualTo(2);
        await Assert.That(created.ParentId).IsEqualTo(0);
    }

    [Test]
    public async Task ServiceLayer_CreateAsync_AddsChildCategory()
    {
        var initialCount = _mockCategoryService!.GetCategoryCount();

        var dto = new CreateAndUpdateCategoryDto
        {
            Name = "New Child Category",
            ViewOrder = 1,
            ParentId = 1
        };

        var newId = await _mockCategoryService.CreateAsync(1, dto);

        await Assert.That(newId).IsGreaterThan(0);
        await Assert.That(_mockCategoryService.GetCategoryCount()).IsEqualTo(initialCount + 1);

        var created = await _mockCategoryService.GetAsync(newId, 1);
        await Assert.That(created.Name).IsEqualTo("New Child Category");
        await Assert.That(created.ParentId).IsEqualTo(1);
    }

    [Test]
    public async Task ServiceLayer_UpdateAsync_ModifiesExistingCategory()
    {
        var dto = new CreateAndUpdateCategoryDto
        {
            Name = "Updated Category Name",
            ViewOrder = 5,
            ParentId = 0
        };

        await _mockCategoryService!.UpdateAsync(1, 1, dto);

        var updated = await _mockCategoryService.GetAsync(1, 1);
        await Assert.That(updated.Name).IsEqualTo("Updated Category Name");
        await Assert.That(updated.ViewOrder).IsEqualTo(5);
        await Assert.That(updated.Id).IsEqualTo(1);
    }

    [Test]
    public async Task ServiceLayer_GetAsync_ReturnsCorrectCategory()
    {
        var category1 = await _mockCategoryService!.GetAsync(1, 1);
        var category2 = await _mockCategoryService.GetAsync(2, 1);

        await Assert.That(category1.Id).IsEqualTo(1);
        await Assert.That(category1.Name).IsEqualTo("Test Category 1");
        await Assert.That(category1.ParentId).IsEqualTo(0);
        
        await Assert.That(category2.Id).IsEqualTo(2);
        await Assert.That(category2.Name).IsEqualTo("Test Category 2");
        await Assert.That(category2.ParentId).IsEqualTo(0);
    }

    [Test]
    public async Task ServiceLayer_GetAsync_ReturnsChildCategory()
    {
        var category = await _mockCategoryService!.GetAsync(3, 1);

        await Assert.That(category.Id).IsEqualTo(3);
        await Assert.That(category.Name).IsEqualTo("Test Category 1.1");
        await Assert.That(category.ParentId).IsEqualTo(1);
    }

    [Test]
    public async Task ServiceLayer_ListAsync_ReturnsCategories()
    {
        var result = await _mockCategoryService!.ListAsync(1, pageNumber: 1, pageSize: 10);

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Items).IsNotNull();
        await Assert.That(result.Items.Count).IsGreaterThan(0);
        await Assert.That(result.TotalCount).IsEqualTo(3);
    }

    [Test]
    public async Task ServiceLayer_ListAsync_SupportsPagination()
    {
        var page1 = await _mockCategoryService!.ListAsync(1, pageNumber: 1, pageSize: 2);
        var page2 = await _mockCategoryService.ListAsync(1, pageNumber: 2, pageSize: 2);

        await Assert.That(page1.Items.Count).IsEqualTo(2);
        await Assert.That(page1.PageNumber).IsEqualTo(1);
        await Assert.That(page2.PageNumber).IsEqualTo(2);
        await Assert.That(page1.TotalCount).IsEqualTo(3);
    }

    [Test]
    public async Task ServiceLayer_DeleteAsync_RemovesCategory()
    {
        var initialCount = _mockCategoryService!.GetCategoryCount();

        await _mockCategoryService.DeleteAsync(2, 1);

        await Assert.That(_mockCategoryService.GetCategoryCount()).IsEqualTo(initialCount - 1);
    }

    #endregion

    #region Service Layer Tests - Move Operations

    [Test]
    public async Task ServiceLayer_MoveUpAsync_MovesCategory()
    {
        var category2 = await _mockCategoryService!.GetAsync(2, 1);
        var originalViewOrder = category2.ViewOrder;

        var result = await _mockCategoryService.MoveUpAsync(2, 1);

        await Assert.That(result).IsEqualTo(2);
        
        var updated = await _mockCategoryService.GetAsync(2, 1);
        await Assert.That(updated.ViewOrder).IsLessThan(originalViewOrder);
    }

    [Test]
    public async Task ServiceLayer_MoveUpAsync_ReturnsSameIdWhenCannotMoveUp()
    {
        var result = await _mockCategoryService!.MoveUpAsync(1, 1);
        await Assert.That(result).IsEqualTo(1);
    }

    [Test]
    public async Task ServiceLayer_MoveDownAsync_MovesCategory()
    {
        var category1 = await _mockCategoryService!.GetAsync(1, 1);
        var originalViewOrder = category1.ViewOrder;

        var result = await _mockCategoryService.MoveDownAsync(1, 1);

        await Assert.That(result).IsEqualTo(1);
        
        var updated = await _mockCategoryService.GetAsync(1, 1);
        await Assert.That(updated.ViewOrder).IsGreaterThan(originalViewOrder);
    }

    [Test]
    public async Task ServiceLayer_MoveDownAsync_ReturnsSameIdWhenCannotMoveDown()
    {
        var result = await _mockCategoryService!.MoveDownAsync(2, 1);
        await Assert.That(result).IsEqualTo(2);
    }

    [Test]
    public async Task ServiceLayer_MoveAsync_ReturnsMinusOneForNonExistentCategory()
    {
        var result = await _mockCategoryService!.MoveUpAsync(999, 1);
        await Assert.That(result).IsEqualTo(-1);

        result = await _mockCategoryService.MoveDownAsync(999, 1);
        await Assert.That(result).IsEqualTo(-1);
    }

    #endregion

    #region State Management Tests

    [Test]
    public async Task ModuleState_ForCategoryComponent_HasRequiredProperties()
    {
        var moduleState = CreateModuleState(1, 1, "Test Module");

        await Assert.That(moduleState.ModuleId).IsEqualTo(1);
        await Assert.That(moduleState.PageId).IsEqualTo(1);
        await Assert.That(moduleState.ModuleDefinition).IsNotNull();
        await Assert.That(moduleState.PermissionList).IsNotNull();
        await Assert.That(moduleState.PermissionList.Any(p => p.PermissionName == "Edit")).IsTrue();
    }

    [Test]
    public async Task PageState_ForCategoryComponent_IsConfigured()
    {
        var pageState = CreatePageState("Index");

        await Assert.That(pageState.Action).IsEqualTo("Index");
        await Assert.That(pageState.QueryString).IsNotNull();
        await Assert.That(pageState.Page).IsNotNull();
        await Assert.That(pageState.Alias).IsNotNull();
        await Assert.That(pageState.Site).IsNotNull();
    }

    #endregion

    #region Form Validation Tests

    [Test]
    public async Task FormValidation_ValidData_Passes()
    {
        var dto = new CreateAndUpdateCategoryDto
        {
            Name = "Valid Category Name",
            ViewOrder = 0,
            ParentId = 0
        };

        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(dto);
        var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
            dto, context, validationResults, validateAllProperties: true);

        await Assert.That(isValid).IsTrue();
        await Assert.That(validationResults.Count).IsEqualTo(0);
    }

    [Test]
    public async Task FormValidation_EmptyName_Fails()
    {
        var dto = new CreateAndUpdateCategoryDto
        {
            Name = string.Empty,
            ViewOrder = 0,
            ParentId = 0
        };

        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(dto);
        var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
            dto, context, validationResults, validateAllProperties: true);

        await Assert.That(isValid).IsFalse();
        await Assert.That(validationResults.Count).IsGreaterThan(0);
        await Assert.That(validationResults.Any(v => v.MemberNames.Contains("Name"))).IsTrue();
    }

    [Test]
    public async Task FormValidation_NameTooLong_Fails()
    {
        var dto = new CreateAndUpdateCategoryDto
        {
            Name = new string('A', 101),
            ViewOrder = 0,
            ParentId = 0
        };

        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(dto);
        var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
            dto, context, validationResults, validateAllProperties: true);

        await Assert.That(isValid).IsFalse();
        await Assert.That(validationResults.Any(v => v.ErrorMessage?.Contains("100") == true)).IsTrue();
    }

    [Test]
    public async Task FormValidation_NegativeViewOrder_Fails()
    {
        var dto = new CreateAndUpdateCategoryDto
        {
            Name = "Valid Name",
            ViewOrder = -1,
            ParentId = 0
        };

        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(dto);
        var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
            dto, context, validationResults, validateAllProperties: true);

        await Assert.That(isValid).IsFalse();
        await Assert.That(validationResults.Any(v => v.MemberNames.Contains("ViewOrder"))).IsTrue();
    }

    [Test]
    public async Task FormValidation_NegativeParentId_Fails()
    {
        var dto = new CreateAndUpdateCategoryDto
        {
            Name = "Valid Name",
            ViewOrder = 0,
            ParentId = -1
        };

        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(dto);
        var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
            dto, context, validationResults, validateAllProperties: true);

        await Assert.That(isValid).IsFalse();
        await Assert.That(validationResults.Any(v => v.MemberNames.Contains("ParentId"))).IsTrue();
    }

    [Test]
    public async Task FormValidation_ValidChildCategory_Passes()
    {
        var dto = new CreateAndUpdateCategoryDto
        {
            Name = "Valid Child Category",
            ViewOrder = 1,
            ParentId = 1
        };

        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(dto);
        var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
            dto, context, validationResults, validateAllProperties: true);

        await Assert.That(isValid).IsTrue();
        await Assert.That(validationResults.Count).IsEqualTo(0);
    }

    #endregion

    #region Navigation Tests

    [Test]
    public async Task NavigationManager_Reset_ClearsHistory()
    {
        _mockNavigationManager!.Reset();

        await Assert.That(_mockNavigationManager.Uri).IsEqualTo("https://localhost:5001/");
        await Assert.That(_mockNavigationManager.BaseUri).IsEqualTo("https://localhost:5001/");
    }

    #endregion

    #region Mock Service Helper Tests

    [Test]
    public async Task MockService_HasTestData()
    {
        var count = _mockCategoryService!.GetCategoryCount();
        await Assert.That(count).IsGreaterThan(0);
        await Assert.That(count).IsEqualTo(3);
    }

    [Test]
    public async Task MockService_GetAllCategories_ReturnsAllData()
    {
        var categories = _mockCategoryService!.GetAllCategories();
        
        await Assert.That(categories).IsNotNull();
        await Assert.That(categories.Count).IsEqualTo(3);
        await Assert.That(categories.Any(c => c.ParentId == 0)).IsTrue();
        await Assert.That(categories.Any(c => c.ParentId == 1)).IsTrue();
    }

    [Test]
    public async Task MockService_ClearData_RemovesAllCategories()
    {
        _mockCategoryService!.ClearData();
        
        await Assert.That(_mockCategoryService.GetCategoryCount()).IsEqualTo(0);
    }

    [Test]
    public async Task MockService_AddTestData_IncreasesCount()
    {
        var initialCount = _mockCategoryService!.GetCategoryCount();
        
        _mockCategoryService.AddTestData(new GetCategoryDto
        {
            Id = 99,
            ModuleId = 1,
            Name = "Manually Added Category",
            ViewOrder = 99,
            ParentId = 0,
            CreatedBy = "Test",
            CreatedOn = DateTime.Now,
            ModifiedBy = "Test",
            ModifiedOn = DateTime.Now
        });

        await Assert.That(_mockCategoryService.GetCategoryCount()).IsEqualTo(initialCount + 1);
    }

    #endregion

    #region Error Handling Tests

    [Test]
    public async Task ServiceLayer_GetAsync_ThrowsForNonExistentCategory()
    {
        await Assert.That(async () => await _mockCategoryService!.GetAsync(999, 1))
            .ThrowsException();
    }

    [Test]
    public async Task ServiceLayer_UpdateAsync_ThrowsForNonExistentCategory()
    {
        var dto = new CreateAndUpdateCategoryDto
        {
            Name = "Updated Name",
            ViewOrder = 0,
            ParentId = 0
        };

        await Assert.That(async () => await _mockCategoryService!.UpdateAsync(999, 1, dto))
            .ThrowsException();
    }

    #endregion

    #region Tree Structure Tests

    [Test]
    public async Task ServiceLayer_ListAsync_ReturnsHierarchicalStructure()
    {
        var result = await _mockCategoryService!.ListAsync(1, pageNumber: 1, pageSize: 10);

        await Assert.That(result.Items).IsNotNull();
        await Assert.That(result.Items.Any(c => c.ParentId == 0)).IsTrue();
        await Assert.That(result.Items.Any(c => c.ParentId == 1)).IsTrue();
    }

    [Test]
    public async Task ServiceLayer_CreateAsync_SupportsMultipleLevels()
    {
        // Create level 2 category
        var dto = new CreateAndUpdateCategoryDto
        {
            Name = "Level 2 Category",
            ViewOrder = 0,
            ParentId = 3
        };

        var newId = await _mockCategoryService!.CreateAsync(1, dto);
        var created = await _mockCategoryService.GetAsync(newId, 1);

        await Assert.That(created.ParentId).IsEqualTo(3);
        await Assert.That(created.Name).IsEqualTo("Level 2 Category");
    }

    [Test]
    public async Task ServiceLayer_MoveOperations_RespectParentId()
    {
        // Get the initial ViewOrder values for categories 1 and 2 (both have ParentId 0)
        var cat1Before = await _mockCategoryService!.GetAsync(1, 1);
        var cat2Before = await _mockCategoryService.GetAsync(2, 1);
        
        var cat1InitialViewOrder = cat1Before.ViewOrder;
        var cat2InitialViewOrder = cat2Before.ViewOrder;

        // Move category 1 down
        await _mockCategoryService.MoveDownAsync(1, 1);

        var cat1After = await _mockCategoryService.GetAsync(1, 1);
        var cat2After = await _mockCategoryService.GetAsync(2, 1);

        // Verify that the ViewOrder values have been swapped
        await Assert.That(cat1After.ViewOrder).IsEqualTo(cat2InitialViewOrder);
        await Assert.That(cat2After.ViewOrder).IsEqualTo(cat1InitialViewOrder);
    }

    #endregion
}
