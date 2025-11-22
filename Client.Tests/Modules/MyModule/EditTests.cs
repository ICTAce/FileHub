// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Client.Tests.Modules.MyModule;
public class EditTests : BaseTest
{
    private readonly MockNavigationManager? _mockNavigationManager;
    private readonly MockMyModuleService? _mockMyModuleService;
    
    public EditTests()
    {
        _mockNavigationManager = TestContext.Services.GetRequiredService<NavigationManager>() as MockNavigationManager;
        _mockMyModuleService = TestContext.Services.GetRequiredService<ISampleModuleService>() as MockMyModuleService;
        TestContext.JSInterop.Setup<bool>("Oqtane.Interop.formValid", _ => true).SetResult(true);
    }
  
    [Test]
    public async Task OnInitializedAsync_EditModeLoadsExistingData()
    {
        var component = CreateEditModeComponent(1);

        await Task.Delay(200);
        component.WaitForState(() => component.Markup.Contains("Test Module 1"), TimeSpan.FromSeconds(3));

        var markup = component.Markup;
        await Assert.That(markup.Contains("name")).IsTrue();
        await Assert.That(markup.Contains("Save")).IsTrue();
        await Assert.That(markup.Contains("Cancel")).IsTrue();
        await Assert.That(markup.Contains("AuditInfo")).IsTrue();
    }

    [Test]
    public async Task Save_AddModeCreatesNewModule()
    {
        var component = CreateAddModeComponent();
        var initialCount = _mockMyModuleService!.GetModuleCount();

        var nameInput = component.Find("#name");
        nameInput.Change("New Test Module");

        var saveButton = component.Find("button[type='button'].btn-success");
        await saveButton.ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs());

        await Task.Delay(200);

        var finalCount = _mockMyModuleService.GetModuleCount();
        await Assert.That(finalCount).IsEqualTo(initialCount + 1);
        await Assert.That(_mockNavigationManager!.NavigateToInvoked).IsTrue();
    }

    [Test]
    public async Task Save_EditModeUpdatesExistingModule()
    {
        var component = CreateEditModeComponent(1);

        await Task.Delay(200);
        component.WaitForState(() => component.Markup.Contains("Test Module 1"), TimeSpan.FromSeconds(3));

        var nameInput = component.Find("#name");
        nameInput.Change("Updated Module Name");

        var saveButton = component.Find("button[type='button'].btn-success");
        await saveButton.ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs());

        await Task.Delay(200);

        var updatedModule = await _mockMyModuleService!.GetAsync(1, 1);

        await Assert.That(updatedModule.Name).IsEqualTo("Updated Module Name");
        await Assert.That(_mockNavigationManager!.NavigateToInvoked).IsTrue();
    }

    [Test]
    public async Task CancelDoesNotSaveChanges()
    {
        TestContext.JSInterop.Setup<bool>("Oqtane.Interop.formValid", _ => true).SetResult(true);
        
        var component = CreateAddModeComponent();
        var initialCount = _mockMyModuleService!.GetModuleCount();

        var nameInput = component.Find("#name");
        nameInput.Change("Should Not Be Saved");

        var cancelLink = component.Find("a.btn-secondary");
        await Assert.That(cancelLink).IsNotNull();

        var finalCount = _mockMyModuleService.GetModuleCount();
        await Assert.That(finalCount).IsEqualTo(initialCount);
    }

    [Test]
    public async Task OnInitializedAsync_EditModeLoadsCorrectModule()
    {
        var component = CreateEditModeComponent(2);

        await Task.Delay(200);
        component.WaitForState(() => component.Markup.Contains("Test Module 2"), TimeSpan.FromSeconds(3));

        var nameInput = component.Find("#name");
        var value = nameInput.GetAttribute("value");
        await Assert.That(value).IsEqualTo("Test Module 2");
    }

    private IRenderedComponent<ICTAce.FileHub.MyModule.Edit> CreateAddModeComponent()
    {
        _mockNavigationManager!.Reset();

        var (pageState, alias, site) = CreateTestContext("Add", []);
        var moduleState = new Module { ModuleId = 1 };

        return TestContext.Render<ICTAce.FileHub.MyModule.Edit>(parameters => parameters
            .AddCascadingValue("ModuleState", moduleState)
            .AddCascadingValue("PageState", pageState)
            .AddCascadingValue("Alias", alias)
            .AddCascadingValue("Site", site));
    }

    private IRenderedComponent<ICTAce.FileHub.MyModule.Edit> CreateEditModeComponent(int id)
    {
        _mockNavigationManager!.Reset();

        var queryString = new Dictionary<string, string>
        {
            { "id", id.ToString(System.Globalization.CultureInfo.InvariantCulture) }
        };
        
        var (pageState, alias, site) = CreateTestContext("Edit", queryString);
        var moduleState = new Module { ModuleId = 1 };

        return TestContext.Render<ICTAce.FileHub.MyModule.Edit>(parameters => parameters
            .AddCascadingValue("ModuleState", moduleState)
            .AddCascadingValue("PageState", pageState)
            .AddCascadingValue("Alias", alias)
            .AddCascadingValue("Site", site));
    }

    private static (Mocks.PageState, Alias, Site) CreateTestContext(string action, Dictionary<string, string> queryString)
    {
        var alias = new Alias
        {
            AliasId = 1,
            TenantId = 1,
            SiteId = 1,
            Name = "localhost"
        };

        var site = new Site
        {
            SiteId = 1,
            TenantId = 1,
            Name = "Test Site"
        };

        var pageState = new Mocks.PageState
        {
            Action = action,
            QueryString = queryString,
            Page = new Page
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
                IsClickable = true
            },
            Alias = alias,
            Site = site
        };

        return (pageState, alias, site);
    }
}
