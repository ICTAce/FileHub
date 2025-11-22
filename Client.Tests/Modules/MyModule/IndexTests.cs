// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Client.Tests.Modules.MyModule;

public class IndexTests : BaseTest
{
    private readonly IRenderedComponent<ICTAce.FileHub.MyModule.Index>? renderedPage;

    public IndexTests()
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

        var moduleState = new Module { ModuleId = 1 };
        var pageState = new Mocks.PageState
        {
            Action = "Index",
            QueryString = [],
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

        renderedPage = TestContext.Render<ICTAce.FileHub.MyModule.Index>(parameters => parameters
            .AddCascadingValue("ModuleState", moduleState)
            .AddCascadingValue("PageState", pageState)
            .AddCascadingValue("Alias", alias)
            .AddCascadingValue("Site", site));
    }

    [Test]
    public async Task ComponentRendersSuccessfullyOnInitialization()
    {
        await Task.Delay(200);
        renderedPage!.WaitForState(() => renderedPage.Markup.Contains("Test Module 1"), TimeSpan.FromSeconds(3));

        var markup = renderedPage.Markup;
        await Assert.That(markup.Contains("Test Module 1")).IsTrue();
        await Assert.That(markup.Contains("Test Module 2")).IsTrue();
        await Assert.That(markup.Contains("Loading...")).IsFalse();
    }

    [Test]
    public async Task ListAsyncWithMultipleModulesDisplaysAllNames()
    {
        renderedPage!.WaitForState(() => renderedPage.Markup.Contains("Test Module 1"), TimeSpan.FromSeconds(3));

        var markup = renderedPage.Markup;
        await Assert.That(markup.Contains("Test Module 1")).IsTrue();
        await Assert.That(markup.Contains("Test Module 2")).IsTrue();
    }

    [Test]
    public async Task DeleteValidModuleRemovesModuleAndRefreshesUI()
    {
        var mockService = TestContext.Services.GetRequiredService<ISampleModuleService>() as MockMyModuleService;
        
        await Task.Delay(200);
        renderedPage!.WaitForState(() => renderedPage.Markup.Contains("Test Module 1"), TimeSpan.FromSeconds(3));

        var initialCount = mockService!.GetModuleCount();
        await Assert.That(initialCount).IsEqualTo(2);

        var deleteButtons = renderedPage.FindAll("button");
        var deleteButton = deleteButtons.FirstOrDefault(b => b.ClassName?.Contains("btn-danger") == true);
        await Assert.That(deleteButton).IsNotNull();

        await deleteButton!.ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs());

        await Task.Delay(300);

        var finalCount = mockService.GetModuleCount();
        await Assert.That(finalCount).IsEqualTo(1);
    }
}
