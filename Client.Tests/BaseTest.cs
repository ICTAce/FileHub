// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Client.Tests;

public abstract class BaseTest : IDisposable
{
    private bool _disposed;

    protected BunitContext TestContext { get; private set; }
    protected Alias TestAlias { get; private set; }
    protected Site TestSite { get; private set; }
    protected Page TestPage { get; private set; }

    protected BaseTest()
    {
        TestContext = new();
        TestContext.JSInterop.Mode = JSRuntimeMode.Loose;
        TestContext.JSInterop.Setup<bool>("Oqtane.Interop.formValid", _ => true).SetResult(true);
        TestContext.JSInterop.Setup<bool>("formValid", _ => true).SetResult(true);
        TestContext.JSInterop.SetupVoid("Oqtane.Interop.setElementAttribute", _ => true);
        TestContext.JSInterop.SetupVoid("Oqtane.Interop.includeCSS", _ => true);
        TestContext.JSInterop.SetupVoid("Oqtane.Interop.includeScript", _ => true);
        TestContext.JSInterop.SetupVoid("Oqtane.Interop.includeLink", _ => true);
        TestContext.JSInterop.SetupVoid("Oqtane.Interop.removeElementsById", _ => true);
        TestContext.JSInterop.Setup<string>("Oqtane.Interop.getElementByName", _ => true).SetResult(string.Empty);
        TestContext.JSInterop.Setup<Dictionary<string, string>>("Oqtane.Interop.getModuleSettings", _ => true).SetResult([]);
        TestContext.JSInterop.Setup<object[]>("Oqtane.Interop.getModuleState", _ => true).SetResult([]);
        TestContext.JSInterop.SetupVoid("Oqtane.Interop.setModuleState", _ => true);
        TestContext.JSInterop.Setup<string>("Oqtane.Interop.getElementValue", _ => true).SetResult(string.Empty);
        TestContext.JSInterop.SetupVoid("Oqtane.Interop.updateTitle", _ => true);
        TestContext.JSInterop.SetupVoid("Oqtane.Interop.includeMeta", _ => true);
        TestContext.JSInterop.Setup<int>("Oqtane.Interop.getScrollPosition", _ => true).SetResult(0);
        TestContext.JSInterop.SetupVoid("Oqtane.Interop.scrollTo", _ => true);

        TestContext.Services.AddLocalization();
        TestContext.Services.AddSingleton<Microsoft.Extensions.Localization.IStringLocalizerFactory, MockStringLocalizerFactory>();

        var mockNavigationManager = new MockNavigationManager();
        TestContext.Services.AddSingleton<NavigationManager>(mockNavigationManager);
        TestContext.Services.AddSingleton(mockNavigationManager);

        // Initialize SiteState with proper data
        var siteState = new SiteState();
        TestContext.Services.AddSingleton(siteState);

        TestContext.Services.AddLogging();

        TestContext.Services.AddScoped<ILogService, MockLogService>();
        TestContext.Services.AddScoped<ISampleModuleService, MockSampleModuleService>();
        TestContext.Services.AddScoped<IUserService, MockUserService>();
        TestContext.Services.AddScoped<ISettingService, MockSettingService>();
        TestContext.Services.AddScoped<IModuleService, MockModuleService>();
        TestContext.Services.AddScoped<IPageService, MockPageService>();
        TestContext.Services.AddScoped<ISiteService, MockSiteService>();
        TestContext.Services.AddScoped<IAliasService, MockAliasService>();
        TestContext.Services.AddScoped<IThemeService, MockThemeService>();
        TestContext.Services.AddScoped<IModuleDefinitionService, MockModuleDefinitionService>();
        TestContext.Services.AddScoped<Microsoft.AspNetCore.Authorization.IAuthorizationService, MockAuthorizationService>();

        // Add authentication state provider
        TestContext.Services.AddScoped<Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider, MockAuthenticationStateProvider>();

        // Add IServiceScopeFactory for dependency resolution
        TestContext.Services.AddScoped<IServiceScopeFactory>(sp => sp.GetRequiredService<IServiceScopeFactory>());

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

    protected Mocks.PageState CreatePageState(string action, Dictionary<string, string>? queryString = null)
    => new Mocks.PageState
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

    protected Module CreateModuleState(int moduleId = 1, int pageId = 1, string title = "Test Module")
    => new Module
    {
        ModuleId = moduleId,
        PageId = pageId,
        Title = title,
        SiteId = 1,
        ModuleDefinitionName = "ICTAce.FileHub.SampleModule",
        AllPages = false,
        IsDeleted = false,
        Pane = "Content",
        Order = 1,
        ContainerType = string.Empty,
        ModuleDefinition = new ModuleDefinition
        {
            ModuleDefinitionName = "ICTAce.FileHub.SampleModule",
            Name = "Sample Module",
            Version = "1.0.0",
            ServerManagerType = string.Empty,
            ControlTypeTemplate = string.Empty,
            ReleaseVersions = string.Empty,
            Dependencies = string.Empty,
            PackageName = "ICTAce.FileHub",
            SiteId = 1
        },
        PermissionList = new List<Permission>
        {
            new Permission
            {
                PermissionName = "View",
                EntityName = "Module",
                EntityId = moduleId,
                PermissionId = 1,
                RoleId = null,
                UserId = null,
                IsAuthorized = true
            },
            new Permission
            {
                PermissionName = "Edit",
                EntityName = "Module",
                EntityId = moduleId,
                PermissionId = 2,
                RoleId = null,
                UserId = null,
                IsAuthorized = true
            }
        },
        Settings = new()
    };

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
