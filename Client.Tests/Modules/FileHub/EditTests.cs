// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Client.Tests.Modules.FileHub;

public class EditTests : BaseTest
{
    private readonly MockNavigationManager? _mockNavigationManager;
    private readonly MockFileService? _mockFileService;
    private readonly MockCategoryService? _mockCategoryService;

    public EditTests()
    {
        _mockNavigationManager = TestContext.Services.GetRequiredService<NavigationManager>() as MockNavigationManager;
        _mockFileService = TestContext.Services.GetRequiredService<Services.IFileService>() as MockFileService;
        _mockCategoryService = TestContext.Services.GetRequiredService<ICategoryService>() as MockCategoryService;
        TestContext.JSInterop.Setup<bool>("Oqtane.Interop.formValid", _ => true).SetResult(true);
    }

    #region Service Dependency Tests

    [Test]
    public async Task EditComponent_ServiceDependencies_AreConfigured()
    {
        await Assert.That(_mockFileService).IsNotNull();
        await Assert.That(_mockCategoryService).IsNotNull();
        await Assert.That(_mockNavigationManager).IsNotNull();

        var logService = TestContext.Services.GetService<ILogService>();
        await Assert.That(logService).IsNotNull();
    }

    #endregion

    #region Service Layer Tests - CRUD Operations

    [Test]
    public async Task ServiceLayer_CreateAsync_AddsNewFile()
    {
        var initialCount = _mockFileService!.GetFileCount();

        var dto = new CreateAndUpdateFileDto
        {
            Name = "New Test File",
            FileName = "new-test-file.pdf",
            ImageName = "new-test-image.png",
            Description = "New test file description",
            FileSize = "3.5 MB",
            Downloads = 0,
            CategoryIds = [1]
        };

        var newId = await _mockFileService.CreateAsync(1, dto);

        await Assert.That(newId).IsGreaterThan(0);
        await Assert.That(_mockFileService.GetFileCount()).IsEqualTo(initialCount + 1);

        var created = await _mockFileService.GetAsync(newId, 1);
        await Assert.That(created.Name).IsEqualTo("New Test File");
        await Assert.That(created.FileName).IsEqualTo("new-test-file.pdf");
        await Assert.That(created.ImageName).IsEqualTo("new-test-image.png");
        await Assert.That(created.Description).IsEqualTo("New test file description");
        await Assert.That(created.FileSize).IsEqualTo("3.5 MB");
        await Assert.That(created.CategoryIds.Count).IsEqualTo(1);
        await Assert.That(created.CategoryIds[0]).IsEqualTo(1);
    }

    [Test]
    public async Task ServiceLayer_CreateAsync_WithMultipleCategories()
    {
        var dto = new CreateAndUpdateFileDto
        {
            Name = "Multi-Category File",
            FileName = "multi-cat-file.pdf",
            ImageName = "multi-cat-image.png",
            Description = "File with multiple categories",
            FileSize = "1.0 MB",
            Downloads = 0,
            CategoryIds = [1, 2]
        };

        var newId = await _mockFileService!.CreateAsync(1, dto);
        var created = await _mockFileService.GetAsync(newId, 1);

        await Assert.That(created.CategoryIds.Count).IsEqualTo(2);
        await Assert.That(created.CategoryIds.Contains(1)).IsTrue();
        await Assert.That(created.CategoryIds.Contains(2)).IsTrue();
    }

    [Test]
    public async Task ServiceLayer_CreateAsync_WithNoCategoriesWorks()
    {
        var dto = new CreateAndUpdateFileDto
        {
            Name = "No Category File",
            FileName = "no-cat-file.pdf",
            ImageName = "no-cat-image.png",
            Description = "File without categories",
            FileSize = "2.0 MB",
            Downloads = 0,
            CategoryIds = []
        };

        var newId = await _mockFileService!.CreateAsync(1, dto);
        var created = await _mockFileService.GetAsync(newId, 1);

        await Assert.That(created.CategoryIds.Count).IsEqualTo(0);
    }

    [Test]
    public async Task ServiceLayer_UpdateAsync_ModifiesExistingFile()
    {
        var dto = new CreateAndUpdateFileDto
        {
            Name = "Updated File Name",
            FileName = "updated-file.pdf",
            ImageName = "updated-image.png",
            Description = "Updated description",
            FileSize = "5.0 MB",
            Downloads = 0,
            CategoryIds = [2]
        };

        await _mockFileService!.UpdateAsync(1, 1, dto);

        var updated = await _mockFileService.GetAsync(1, 1);
        await Assert.That(updated.Name).IsEqualTo("Updated File Name");
        await Assert.That(updated.FileName).IsEqualTo("updated-file.pdf");
        await Assert.That(updated.ImageName).IsEqualTo("updated-image.png");
        await Assert.That(updated.Description).IsEqualTo("Updated description");
        await Assert.That(updated.FileSize).IsEqualTo("5.0 MB");
        await Assert.That(updated.Id).IsEqualTo(1);
    }

    [Test]
    public async Task ServiceLayer_GetAsync_ReturnsCorrectFile()
    {
        var file1 = await _mockFileService!.GetAsync(1, 1);
        var file2 = await _mockFileService.GetAsync(2, 1);

        await Assert.That(file1.Id).IsEqualTo(1);
        await Assert.That(file1.Name).IsEqualTo("Test File 1");
        await Assert.That(file1.FileName).IsEqualTo("test-file-1.pdf");
        await Assert.That(file1.Downloads).IsEqualTo(10);

        await Assert.That(file2.Id).IsEqualTo(2);
        await Assert.That(file2.Name).IsEqualTo("Test File 2");
        await Assert.That(file2.FileName).IsEqualTo("test-file-2.docx");
        await Assert.That(file2.Downloads).IsEqualTo(25);
    }

    [Test]
    public async Task ServiceLayer_DeleteAsync_RemovesFile()
    {
        var initialCount = _mockFileService!.GetFileCount();

        await _mockFileService.DeleteAsync(2, 1);

        await Assert.That(_mockFileService.GetFileCount()).IsEqualTo(initialCount - 1);
    }

    #endregion

    #region Service Layer Tests - File Upload

    [Test]
    public async Task ServiceLayer_UploadFileAsync_ReturnsUniqueFileName()
    {
        using var stream = new MemoryStream();
        var fileName = "test-upload.pdf";

        var result = await _mockFileService!.UploadFileAsync(1, stream, fileName);

        await Assert.That(result).IsNotNull();
        await Assert.That(result).Contains(fileName);
        await Assert.That(result.Length).IsGreaterThan(fileName.Length);
    }

    [Test]
    public async Task ServiceLayer_UploadFileAsync_GeneratesDifferentNames()
    {
        using var stream1 = new MemoryStream();
        using var stream2 = new MemoryStream();
        var fileName = "test-upload.pdf";

        var result1 = await _mockFileService!.UploadFileAsync(1, stream1, fileName);
        var result2 = await _mockFileService.UploadFileAsync(1, stream2, fileName);

        await Assert.That(result1).IsNotEqualTo(result2);
    }

    #endregion

    #region State Management Tests

    [Test]
    public async Task PageState_AddMode_IsConfigured()
    {
        var pageState = CreatePageState("Add");

        await Assert.That(pageState.Action).IsEqualTo("Add");
        await Assert.That(pageState.QueryString).IsNotNull();
        await Assert.That(pageState.QueryString.Count).IsEqualTo(0);
    }

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

    #endregion

    #region Form Validation Tests

    [Test]
    public async Task FormValidation_ValidData_Passes()
    {
        var dto = new CreateAndUpdateFileDto
        {
            Name = "Valid File Name",
            FileName = "valid-file.pdf",
            ImageName = "valid-image.png",
            Description = "Valid description",
            FileSize = "1.5 MB"
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
        var dto = new CreateAndUpdateFileDto
        {
            Name = string.Empty,
            FileName = "file.pdf",
            ImageName = "image.png",
            FileSize = "1.0 MB"
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
        var dto = new CreateAndUpdateFileDto
        {
            Name = new string('A', 101),
            FileName = "file.pdf",
            ImageName = "image.png",
            FileSize = "1.0 MB"
        };

        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(dto);
        var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
            dto, context, validationResults, validateAllProperties: true);

        await Assert.That(isValid).IsFalse();
        await Assert.That(validationResults.Any(v => v.ErrorMessage?.Contains("100") == true)).IsTrue();
    }

    [Test]
    public async Task FormValidation_EmptyFileName_Fails()
    {
        var dto = new CreateAndUpdateFileDto
        {
            Name = "Valid Name",
            FileName = string.Empty,
            ImageName = "image.png",
            FileSize = "1.0 MB"
        };

        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(dto);
        var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
            dto, context, validationResults, validateAllProperties: true);

        await Assert.That(isValid).IsFalse();
        await Assert.That(validationResults.Any(v => v.MemberNames.Contains("FileName"))).IsTrue();
    }

    [Test]
    public async Task FormValidation_FileNameTooLong_Fails()
    {
        var dto = new CreateAndUpdateFileDto
        {
            Name = "Valid Name",
            FileName = new string('A', 256),
            ImageName = "image.png",
            FileSize = "1.0 MB"
        };

        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(dto);
        var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
            dto, context, validationResults, validateAllProperties: true);

        await Assert.That(isValid).IsFalse();
        await Assert.That(validationResults.Any(v => v.ErrorMessage?.Contains("255") == true)).IsTrue();
    }

    [Test]
    public async Task FormValidation_EmptyImageName_Fails()
    {
        var dto = new CreateAndUpdateFileDto
        {
            Name = "Valid Name",
            FileName = "file.pdf",
            ImageName = string.Empty,
            FileSize = "1.0 MB"
        };

        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(dto);
        var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
            dto, context, validationResults, validateAllProperties: true);

        await Assert.That(isValid).IsFalse();
        await Assert.That(validationResults.Any(v => v.MemberNames.Contains("ImageName"))).IsTrue();
    }

    [Test]
    public async Task FormValidation_ImageNameTooLong_Fails()
    {
        var dto = new CreateAndUpdateFileDto
        {
            Name = "Valid Name",
            FileName = "file.pdf",
            ImageName = new string('A', 256),
            FileSize = "1.0 MB"
        };

        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(dto);
        var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
            dto, context, validationResults, validateAllProperties: true);

        await Assert.That(isValid).IsFalse();
        await Assert.That(validationResults.Any(v => v.ErrorMessage?.Contains("255") == true)).IsTrue();
    }

    [Test]
    public async Task FormValidation_DescriptionTooLong_Fails()
    {
        var dto = new CreateAndUpdateFileDto
        {
            Name = "Valid Name",
            FileName = "file.pdf",
            ImageName = "image.png",
            Description = new string('A', 1001),
            FileSize = "1.0 MB"
        };

        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(dto);
        var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
            dto, context, validationResults, validateAllProperties: true);

        await Assert.That(isValid).IsFalse();
        await Assert.That(validationResults.Any(v => v.ErrorMessage?.Contains("1000") == true)).IsTrue();
    }

    [Test]
    public async Task FormValidation_EmptyFileSize_Fails()
    {
        var dto = new CreateAndUpdateFileDto
        {
            Name = "Valid Name",
            FileName = "file.pdf",
            ImageName = "image.png",
            FileSize = string.Empty
        };

        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(dto);
        var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
            dto, context, validationResults, validateAllProperties: true);

        await Assert.That(isValid).IsFalse();
        await Assert.That(validationResults.Any(v => v.MemberNames.Contains("FileSize"))).IsTrue();
    }

    [Test]
    public async Task FormValidation_FileSizeTooLong_Fails()
    {
        var dto = new CreateAndUpdateFileDto
        {
            Name = "Valid Name",
            FileName = "file.pdf",
            ImageName = "image.png",
            FileSize = new string('1', 13)
        };

        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var context = new System.ComponentModel.DataAnnotations.ValidationContext(dto);
        var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
            dto, context, validationResults, validateAllProperties: true);

        await Assert.That(isValid).IsFalse();
        await Assert.That(validationResults.Any(v => v.ErrorMessage?.Contains("12") == true)).IsTrue();
    }

    [Test]
    public async Task FormValidation_NullDescription_Passes()
    {
        var dto = new CreateAndUpdateFileDto
        {
            Name = "Valid Name",
            FileName = "file.pdf",
            ImageName = "image.png",
            Description = null,
            FileSize = "1.0 MB"
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

    #region Permission Tests

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

    #endregion

    #region Mock Service Helper Tests

    [Test]
    public async Task MockService_HasTestData()
    {
        var count = _mockFileService!.GetFileCount();
        await Assert.That(count).IsGreaterThan(0);
        await Assert.That(count).IsEqualTo(2);
    }

    [Test]
    public async Task MockService_GetAllFiles_ReturnsAllData()
    {
        var files = _mockFileService!.GetAllFiles();

        await Assert.That(files).IsNotNull();
        await Assert.That(files.Count).IsEqualTo(2);
        await Assert.That(files.Any(f => f.CategoryIds.Count > 0)).IsTrue();
    }

    [Test]
    public async Task MockService_ClearData_RemovesAllFiles()
    {
        _mockFileService!.ClearData();

        await Assert.That(_mockFileService.GetFileCount()).IsEqualTo(0);
    }

    [Test]
    public async Task MockService_AddTestData_IncreasesCount()
    {
        var initialCount = _mockFileService!.GetFileCount();

        _mockFileService.AddTestData(new GetFileDto
        {
            Id = 99,
            ModuleId = 1,
            Name = "Manually Added File",
            FileName = "manual-file.pdf",
            ImageName = "manual-image.png",
            Description = "Manually added test file",
            FileSize = "5.5 MB",
            Downloads = 0,
            CategoryIds = [],
            CreatedBy = "Test",
            CreatedOn = DateTime.Now,
            ModifiedBy = "Test",
            ModifiedOn = DateTime.Now
        });

        await Assert.That(_mockFileService.GetFileCount()).IsEqualTo(initialCount + 1);
    }

    #endregion

    #region Error Handling Tests

    [Test]
    public async Task ServiceLayer_GetAsync_ThrowsForNonExistentFile()
    {
        await Assert.That(async () => await _mockFileService!.GetAsync(999, 1).ConfigureAwait(false))
            .ThrowsException();
    }

    [Test]
    public async Task ServiceLayer_UpdateAsync_ThrowsForNonExistentFile()
    {
        var dto = new CreateAndUpdateFileDto
        {
            Name = "Updated Name",
            FileName = "updated.pdf",
            ImageName = "updated.png",
            FileSize = "1.0 MB"
        };

        await Assert.That(async () => await _mockFileService!.UpdateAsync(999, 1, dto).ConfigureAwait(false))
            .ThrowsException();
    }

    #endregion

    #region List and Pagination Tests

    [Test]
    public async Task ServiceLayer_ListAsync_ReturnsFiles()
    {
        var result = await _mockFileService!.ListAsync(1, pageNumber: 1, pageSize: 10).ConfigureAwait(false);

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Items).IsNotNull();
        await Assert.That(result.Items.Count).IsGreaterThan(0);
        await Assert.That(result.TotalCount).IsEqualTo(2);
    }

    [Test]
    public async Task ServiceLayer_ListAsync_SupportsPagination()
    {
        var page1 = await _mockFileService!.ListAsync(1, pageNumber: 1, pageSize: 1).ConfigureAwait(false);
        var page2 = await _mockFileService.ListAsync(1, pageNumber: 2, pageSize: 1).ConfigureAwait(false);

        await Assert.That(page1.Items.Count).IsEqualTo(1);
        await Assert.That(page1.PageNumber).IsEqualTo(1);
        await Assert.That(page2.Items.Count).IsEqualTo(1);
        await Assert.That(page2.PageNumber).IsEqualTo(2);
        await Assert.That(page1.TotalCount).IsEqualTo(2);
    }

    #endregion
}
