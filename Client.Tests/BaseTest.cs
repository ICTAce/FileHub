// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Client.Tests;

public abstract class BaseTest : IDisposable
{
    private bool _disposed;
    protected BunitContext TestContext { get; private set; }

    // Common test data
    protected Alias TestAlias { get; private set; }
    protected Site TestSite { get; private set; }
    protected Page TestPage { get; private set; }
    
    // Access to mocks for verification
    protected MockLogService MockLogService => TestContext.Services.GetRequiredService<ILogService>() as MockLogService 
        ?? throw new InvalidOperationException("MockLogService not registered");

    protected BaseTest()
    {
        TestContext = new();
        TestContext.JSInterop.Mode = JSRuntimeMode.Loose;
        TestContext.JSInterop.Setup<bool>("Oqtane.Interop.formValid", _ => true).SetResult(true);
        TestContext.JSInterop.Setup<bool>("formValid", _ => true).SetResult(true);
        TestContext.JSInterop.SetupVoid("Oqtane.Interop.setElementAttribute", _ => true);
        TestContext.JSInterop.SetupVoid("Oqtane.Interop.includeCSS", _ => true);
        TestContext.JSInterop.SetupVoid("Oqtane.Interop.includeScript", _ => true);
        TestContext.JSInterop.Setup<string>("Oqtane.Interop.getElementByName", _ => true).SetResult(string.Empty);
        TestContext.JSInterop.Setup<Dictionary<string, string>>("Oqtane.Interop.getModuleSettings", _ => true).SetResult([]);
        TestContext.JSInterop.Setup<object[]>("Oqtane.Interop.getModuleState", _ => true).SetResult([]);

        TestContext.Services.AddLocalization();
        TestContext.Services.AddSingleton<Microsoft.Extensions.Localization.IStringLocalizerFactory, MockStringLocalizerFactory>();

        var mockNavigationManager = new MockNavigationManager();
        TestContext.Services.AddSingleton<NavigationManager>(mockNavigationManager);
        TestContext.Services.AddSingleton(mockNavigationManager);

        TestContext.Services.AddSingleton(new SiteState());
        TestContext.Services.AddLogging();
        TestContext.Services.AddScoped<ILogService, MockLogService>();
        TestContext.Services.AddScoped<ISampleModuleService, MockSampleModuleService>();

        // Add HttpClient for Blazor components
        TestContext.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:5001/") });

        // Add PageState mock for cascading parameter
        TestContext.Services.AddScoped(_ => new Mocks.PageState
        {
            Action = "Index",
            QueryString = []
        });

        // Initialize common test data
        InitializeCommonTestData();
    }

    private void InitializeCommonTestData()
    {
        TestAlias = new Alias
        {
            AliasId = 1,
            TenantId = 1,
            SiteId = 1,
            Name = "localhost",
            IsDefault = true,
        };

        TestSite = new Site
        {
            SiteId = 1,
            TenantId = 1,
            Name = "Test Site",
            LogoFileId = null,
            FaviconFileId = null,
            DefaultThemeType = "Test.Theme",
            DefaultLayoutType = "Test.Layout",
            DefaultContainerType = "Test.Container",
        };

        TestPage = new Page
        {
            PageId = 1,
            SiteId = 1,
            Path = "/test",
            Name = "Test Page",
            Title = "Test Page",
            IsNavigation = true,
            Url = "/test",
            IsPersonalizable = false,
            UserId = null,
            IsClickable = true,
            ParentId = null,
            Order = 1,
            Level = 0,
        };
    }

    /// <summary>
    /// Creates a PageState for testing with the specified action and query string
    /// </summary>
    protected Mocks.PageState CreatePageState(string action, Dictionary<string, string>? queryString = null)
    {
        return new Mocks.PageState
        {
            Action = action,
            QueryString = queryString ?? [],
            Page = TestPage,
            Alias = TestAlias,
            Site = TestSite,
            ModuleId = 1,
            PageId = 1,
            Url = "/test",
            Path = "/test",
            ReturnUrl = string.Empty
        };
    }

    /// <summary>
    /// Creates a standard Module state for testing
    /// </summary>
    protected Module CreateModuleState(int moduleId = 1, int pageId = 1, string title = "Test Module")
    {
        return new Module
        {
            ModuleId = moduleId,
            PageId = pageId,
            Title = title,
            // Add essential properties that ModuleBase might access
            SiteId = 1,
            ModuleDefinitionName = "ICTAce.FileHub.SampleModule",
            AllPages = false,
            IsDeleted = false,
            // Add ModuleDefinition to prevent null reference
            ModuleDefinition = new ModuleDefinition
            {
                ModuleDefinitionName = "ICTAce.FileHub.SampleModule",
                Name = "Sample Module",
                Version = "1.0.0"
            },
            // Add PermissionList to prevent authorization issues
            PermissionList = new List<Permission>()
        };
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                TestContext.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
