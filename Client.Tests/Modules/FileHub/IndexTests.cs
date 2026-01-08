// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Client.Tests.Modules.FileHub;

public class IndexTests : BaseTest
{
    private readonly MockNavigationManager? _mockNavigationManager;
    private readonly MockFileService? _mockFileService;

    public IndexTests()
    {
        _mockNavigationManager = TestContext.Services.GetRequiredService<NavigationManager>() as MockNavigationManager;
        _mockFileService = TestContext.Services.GetRequiredService<Services.IFileService>() as MockFileService;
    }

    #region Service Dependency Tests

    [Test]
    public async Task IndexComponent_ServiceDependencies_CanBeResolved()
    {
        await Assert.That(_mockFileService).IsNotNull();
        await Assert.That(_mockNavigationManager).IsNotNull();

        var logService = TestContext.Services.GetService<ILogService>();
        await Assert.That(logService).IsNotNull();
    }

    #endregion

    #region Service Layer Tests

    [Test]
    public async Task ServiceLayer_ListAsync_ReturnsFiles()
    {
        var result = await _mockFileService!.ListAsync(1, pageNumber: 1, pageSize: 10);

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Items).IsNotNull();
        await Assert.That(result.Items.Count).IsEqualTo(2);
        await Assert.That(result.TotalCount).IsEqualTo(2);
    }

    [Test]
    public async Task ServiceLayer_ListAsync_ReturnsCorrectFileData()
    {
        var result = await _mockFileService!.ListAsync(1, pageNumber: 1, pageSize: 10);

        var firstFile = result.Items.First();
        await Assert.That(firstFile.Id).IsEqualTo(1);
        await Assert.That(firstFile.Name).IsEqualTo("Test File 1");
        await Assert.That(firstFile.FileName).IsEqualTo("test-file-1.pdf");
        await Assert.That(firstFile.ImageName).IsEqualTo("test-image-1.png");
        await Assert.That(firstFile.FileSize).IsEqualTo("1.5 MB");
        await Assert.That(firstFile.Downloads).IsEqualTo(10);
    }

    [Test]
    public async Task ServiceLayer_ListAsync_SupportsPagination()
    {
        var page1 = await _mockFileService!.ListAsync(1, pageNumber: 1, pageSize: 1);
        var page2 = await _mockFileService.ListAsync(1, pageNumber: 2, pageSize: 1);

        await Assert.That(page1.Items.Count).IsEqualTo(1);
        await Assert.That(page1.PageNumber).IsEqualTo(1);
        await Assert.That(page2.Items.Count).IsEqualTo(1);
        await Assert.That(page2.PageNumber).IsEqualTo(2);
        await Assert.That(page1.TotalCount).IsEqualTo(2);
        await Assert.That(page2.TotalCount).IsEqualTo(2);
    }

    [Test]
    public async Task ServiceLayer_ListAsync_ReturnsEmptyWhenNoFiles()
    {
        _mockFileService!.ClearData();

        var result = await _mockFileService.ListAsync(1, pageNumber: 1, pageSize: 10);

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Items).IsNotNull();
        await Assert.That(result.Items.Count).IsEqualTo(0);
        await Assert.That(result.TotalCount).IsEqualTo(0);
    }

    [Test]
    public async Task ServiceLayer_DeleteAsync_RemovesFile()
    {
        var initialCount = _mockFileService!.GetFileCount();

        await _mockFileService.DeleteAsync(1, 1);

        await Assert.That(_mockFileService.GetFileCount()).IsEqualTo(initialCount - 1);

        var result = await _mockFileService.ListAsync(1, pageNumber: 1, pageSize: 10);
        await Assert.That(result.Items.Any(f => f.Id == 1)).IsFalse();
    }

    #endregion

    #region State Management Tests

    [Test]
    public async Task PageState_ForIndexComponent_IsConfigured()
    {
        var pageState = CreatePageState("Index");

        await Assert.That(pageState.Action).IsEqualTo("Index");
        await Assert.That(pageState.QueryString).IsNotNull();
        await Assert.That(pageState.Page).IsNotNull();
        await Assert.That(pageState.Alias).IsNotNull();
        await Assert.That(pageState.Site).IsNotNull();
    }

    [Test]
    public async Task ModuleState_ForIndexComponent_HasRequiredProperties()
    {
        var moduleState = CreateModuleState(1, 1, "Test Module");

        await Assert.That(moduleState.ModuleId).IsEqualTo(1);
        await Assert.That(moduleState.PageId).IsEqualTo(1);
        await Assert.That(moduleState.ModuleDefinition).IsNotNull();
        await Assert.That(moduleState.PermissionList).IsNotNull();
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

    #region Mock Service Tests

    [Test]
    public async Task MockService_HasTestData()
    {
        var count = _mockFileService!.GetFileCount();
        await Assert.That(count).IsGreaterThan(0);
        await Assert.That(count).IsEqualTo(2);
    }

    [Test]
    public async Task MockService_GetAllFiles_ReturnsCorrectCount()
    {
        var files = _mockFileService!.GetAllFiles();

        await Assert.That(files).IsNotNull();
        await Assert.That(files.Count).IsEqualTo(2);
    }

    [Test]
    public async Task MockService_SupportsMultipleModules()
    {
        // Add a file for a different module
        _mockFileService!.AddTestData(new GetFileDto
        {
            Id = 100,
            ModuleId = 2,
            Name = "Module 2 File",
            FileName = "module2-file.pdf",
            ImageName = "module2-image.png",
            Description = "File for module 2",
            FileSize = "1.0 MB",
            Downloads = 0,
            CategoryIds = [],
            CreatedBy = "Test",
            CreatedOn = DateTime.Now,
            ModifiedBy = "Test",
            ModifiedOn = DateTime.Now
        });

        var module1Files = await _mockFileService.ListAsync(1, pageNumber: 1, pageSize: 10);
        var module2Files = await _mockFileService.ListAsync(2, pageNumber: 1, pageSize: 10);

        await Assert.That(module1Files.TotalCount).IsEqualTo(2);
        await Assert.That(module2Files.TotalCount).IsEqualTo(1);
    }

    #endregion

    #region File Properties Tests

    [Test]
    public async Task ServiceLayer_ListAsync_ReturnsFileWithAllProperties()
    {
        var result = await _mockFileService!.ListAsync(1, pageNumber: 1, pageSize: 10);
        var file = result.Items.First();

        await Assert.That(file.Id).IsGreaterThan(0);
        await Assert.That(file.Name).IsNotNull();
        await Assert.That(file.FileName).IsNotNull();
        await Assert.That(file.ImageName).IsNotNull();
        await Assert.That(file.FileSize).IsNotNull();
        await Assert.That(file.Downloads).IsGreaterThanOrEqualTo(0);
        await Assert.That(file.CreatedOn).IsGreaterThan(DateTime.MinValue);
    }

    [Test]
    public async Task ServiceLayer_ListAsync_ReturnsFilesInCorrectOrder()
    {
        var result = await _mockFileService!.ListAsync(1, pageNumber: 1, pageSize: 10);
        var files = result.Items.ToList();

        await Assert.That(files.Count).IsEqualTo(2);
        await Assert.That(files[0].Id).IsEqualTo(1);
        await Assert.That(files[1].Id).IsEqualTo(2);
    }

    #endregion

    #region Download Counter Tests

    [Test]
    public async Task FileDto_HasDownloadsProperty()
    {
        var result = await _mockFileService!.ListAsync(1, pageNumber: 1, pageSize: 10);
        var file = result.Items.First();

        await Assert.That(file.Downloads).IsGreaterThanOrEqualTo(0);
    }

    [Test]
    public async Task FileDto_DownloadsCanBeTracked()
    {
        var result = await _mockFileService!.ListAsync(1, pageNumber: 1, pageSize: 10);
        var file1 = result.Items.First(f => f.Id == 1);
        var file2 = result.Items.First(f => f.Id == 2);

        await Assert.That(file1.Downloads).IsEqualTo(10);
        await Assert.That(file2.Downloads).IsEqualTo(25);
    }

    #endregion

    #region Description Tests

    [Test]
    public async Task FileDto_CanHaveNullDescription()
    {
        _mockFileService!.AddTestData(new GetFileDto
        {
            Id = 50,
            ModuleId = 1,
            Name = "No Description File",
            FileName = "no-desc.pdf",
            ImageName = "no-desc-image.png",
            Description = null,
            FileSize = "1.0 MB",
            Downloads = 0,
            CategoryIds = [],
            CreatedBy = "Test",
            CreatedOn = DateTime.Now,
            ModifiedBy = "Test",
            ModifiedOn = DateTime.Now
        });

        var result = await _mockFileService.ListAsync(1, pageNumber: 1, pageSize: 10);
        var fileWithoutDesc = result.Items.FirstOrDefault(f => f.Id == 50);

        await Assert.That(fileWithoutDesc).IsNotNull();
        await Assert.That(fileWithoutDesc!.Description).IsNull();
    }

    [Test]
    public async Task FileDto_CanHaveDescription()
    {
        var result = await _mockFileService!.ListAsync(1, pageNumber: 1, pageSize: 10);
        var fileWithDesc = result.Items.First(f => f.Id == 1);

        await Assert.That(fileWithDesc.Description).IsNotNull();
        await Assert.That(fileWithDesc.Description).IsEqualTo("Test file description 1");
    }

    #endregion

    #region Pagination Edge Cases

    [Test]
    public async Task ServiceLayer_ListAsync_HandlesLargePageSize()
    {
        var result = await _mockFileService!.ListAsync(1, pageNumber: 1, pageSize: 1000);

        await Assert.That(result.Items.Count).IsEqualTo(2);
        await Assert.That(result.TotalCount).IsEqualTo(2);
    }

    [Test]
    public async Task ServiceLayer_ListAsync_HandlesPageBeyondTotalPages()
    {
        var result = await _mockFileService!.ListAsync(1, pageNumber: 10, pageSize: 10);

        await Assert.That(result.Items.Count).IsEqualTo(0);
        await Assert.That(result.TotalCount).IsEqualTo(2);
        await Assert.That(result.PageNumber).IsEqualTo(10);
    }

    #endregion

    #region File Name Tests

    [Test]
    public async Task FileDto_HasUniqueFileNames()
    {
        var result = await _mockFileService!.ListAsync(1, pageNumber: 1, pageSize: 10);
        var fileNames = result.Items.Select(f => f.FileName).ToList();

        await Assert.That(fileNames.Distinct().Count()).IsEqualTo(fileNames.Count);
    }

    [Test]
    public async Task FileDto_HasValidFileExtensions()
    {
        var result = await _mockFileService!.ListAsync(1, pageNumber: 1, pageSize: 10);

        foreach (var file in result.Items)
        {
            await Assert.That(file.FileName).Contains(".");
        }
    }

    #endregion

    #region Image Name Tests

    [Test]
    public async Task FileDto_HasImageNames()
    {
        var result = await _mockFileService!.ListAsync(1, pageNumber: 1, pageSize: 10);

        foreach (var file in result.Items)
        {
            await Assert.That(file.ImageName).IsNotNull();
            await Assert.That(file.ImageName.Length).IsGreaterThan(0);
        }
    }

    [Test]
    public async Task FileDto_ImageNamesAreValid()
    {
        var result = await _mockFileService!.ListAsync(1, pageNumber: 1, pageSize: 10);

        foreach (var file in result.Items)
        {
            await Assert.That(file.ImageName).Contains(".");
        }
    }

    #endregion

    #region File Size Tests

    [Test]
    public async Task FileDto_HasFileSizes()
    {
        var result = await _mockFileService!.ListAsync(1, pageNumber: 1, pageSize: 10);

        foreach (var file in result.Items)
        {
            await Assert.That(file.FileSize).IsNotNull();
            await Assert.That(file.FileSize.Length).IsGreaterThan(0);
        }
    }

    [Test]
    public async Task FileDto_FileSizesAreFormatted()
    {
        var result = await _mockFileService!.ListAsync(1, pageNumber: 1, pageSize: 10);
        var file1 = result.Items.First(f => f.Id == 1);

        await Assert.That(file1.FileSize).Contains("MB");
    }

    #endregion
}
