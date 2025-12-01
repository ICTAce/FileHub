// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.EndToEnd.Tests;

/// <summary>
/// Oqtane-specific application tests.
/// Verifies Oqtane framework components and module integration.
/// </summary>
public class OqtaneApplicationTests : PageTest
{
    [Test]
    public async Task Navigate_ToOqtaneApp_RendersWithoutServerErrors()
    {
        var serverErrors = new List<string>();

        // Setup: Monitor for failed requests
        Page.RequestFailed += (_, request) =>
        {
            serverErrors.Add($"{request.Failure} - {request.Url}");
        };

        // Execution: Navigate to the application
        await Page.GotoAsync(TestConfiguration.BaseUrl, new()
        {
            WaitUntil = WaitUntilState.NetworkIdle,
            Timeout = TestConfiguration.NavigationTimeout
        }).ConfigureAwait(false);

        // Give time for initial requests to complete
        await Task.Delay(2000).ConfigureAwait(false);

        // Verification: No critical request failures should occur
        // Filter out common non-critical failures like favicon
        var criticalErrors = serverErrors
            .Where(e => !e.Contains("favicon.ico"))
            .ToList();

        await Assert.That(criticalErrors.Count).IsEqualTo(0);
    }

    [Test]
    public async Task Navigate_ToOqtaneApp_HasValidMetadata()
    {
        // Execution: Navigate to the application
        await Page.GotoAsync(TestConfiguration.BaseUrl, new()
        {
            WaitUntil = WaitUntilState.NetworkIdle,
            Timeout = TestConfiguration.NavigationTimeout
        }).ConfigureAwait(false);

        // Verification: Check for essential meta tags
        var viewport = Page.Locator("meta[name='viewport']");
        await Expect(viewport).ToHaveCountAsync(1).ConfigureAwait(false);

        var charset = Page.Locator("meta[charset]");
        var charsetCount = await charset.CountAsync().ConfigureAwait(false);
        await Assert.That(charsetCount).IsGreaterThan(0);
    }

    [Test]
    public async Task Navigate_ToOqtaneApp_LoadsBlazorResources()
    {
        var blazorScriptLoaded = false;

        // Setup: Monitor resource loading
        Page.Response += (_, response) =>
        {
            if ((response.Url.Contains("blazor.webassembly.js") ||
                response.Url.Contains("blazor.web.js")) &&
                response.Ok)
            {
                blazorScriptLoaded = true;
            }
        };

        // Execution: Navigate to the application
        await Page.GotoAsync(TestConfiguration.BaseUrl, new()
        {
            WaitUntil = WaitUntilState.NetworkIdle,
            Timeout = TestConfiguration.NavigationTimeout
        }).ConfigureAwait(false);

        // Verification: Blazor script should be loaded
        await Assert.That(blazorScriptLoaded).IsTrue();
    }

    [Test]
    public async Task Navigate_ToOqtaneApp_HasApplicationRootElement()
    {
        // Execution: Navigate to the application
        await Page.GotoAsync(TestConfiguration.BaseUrl, new()
        {
            WaitUntil = WaitUntilState.NetworkIdle,
            Timeout = TestConfiguration.NavigationTimeout
        }).ConfigureAwait(false);

        // Verification: Check for common Blazor/Oqtane root elements
        var appElement = Page.Locator("#app, [role='main'], main, .app");
        var count = await appElement.CountAsync().ConfigureAwait(false);

        await Assert.That(count).IsGreaterThan(0);
    }
}
