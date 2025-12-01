// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.EndToEnd.Tests;

/// <summary>
/// Basic application health and availability tests.
/// Verifies that the Oqtane application is running and accessible.
/// </summary>
public class ApplicationHealthTests : PageTest
{
    [Test]
    public async Task Navigate_ToBaseUrl_ReturnsSuccessfulResponse()
    {
        // Execution: Navigate to the application base URL
        var response = await Page.GotoAsync(TestConfiguration.BaseUrl, new()
        {
            WaitUntil = WaitUntilState.NetworkIdle,
            Timeout = TestConfiguration.NavigationTimeout
        }).ConfigureAwait(false);

        // Verification: Response should be successful
        await Assert.That(response).IsNotNull();
        await Assert.That(response!.Ok).IsTrue();
    }

    [Test]
    public async Task Navigate_ToBaseUrl_LoadsWithoutConsoleErrors()
    {
        var consoleErrors = new List<string>();

        // Setup: Capture console error messages
        Page.Console += (_, msg) =>
        {
            if (string.Equals(msg.Type, "error", StringComparison.Ordinal))
            {
                consoleErrors.Add(msg.Text);
            }
        };

        // Execution: Navigate to the application
        await Page.GotoAsync(TestConfiguration.BaseUrl, new()
        {
            WaitUntil = WaitUntilState.NetworkIdle,
            Timeout = TestConfiguration.NavigationTimeout
        }).ConfigureAwait(false);

        // Verification: No critical console errors should be present
        await Assert.That(consoleErrors.Count).IsEqualTo(0);
    }

    [Test]
    public async Task Navigate_ToBaseUrl_HasValidTitle()
    {
        // Execution: Navigate to the application
        await Page.GotoAsync(TestConfiguration.BaseUrl, new()
        {
            WaitUntil = WaitUntilState.NetworkIdle,
            Timeout = TestConfiguration.NavigationTimeout
        }).ConfigureAwait(false);

        // Verification: Page should have a non-empty title
        var title = await Page.TitleAsync().ConfigureAwait(false);
        await Assert.That(title).IsNotNull();
        await Assert.That(title).IsNotEmpty();
    }

    [Test]
    public async Task Navigate_ToBaseUrl_RendersBodyContent()
    {
        // Execution: Navigate to the application
        await Page.GotoAsync(TestConfiguration.BaseUrl, new()
        {
            WaitUntil = WaitUntilState.NetworkIdle,
            Timeout = TestConfiguration.NavigationTimeout
        }).ConfigureAwait(false);

        // Verification: Body element should exist and have content
        var body = Page.Locator("body");
        await Expect(body).ToBeVisibleAsync().ConfigureAwait(false);
        
        var bodyContent = await body.TextContentAsync().ConfigureAwait(false);
        await Assert.That(bodyContent).IsNotNull();
        await Assert.That(bodyContent).IsNotEmpty();
    }

    [Test]
    public async Task Navigate_ToBaseUrl_CompletesBlazorInitialization()
    {
        // Execution: Navigate to the application
        await Page.GotoAsync(TestConfiguration.BaseUrl, new()
        {
            WaitUntil = WaitUntilState.NetworkIdle,
            Timeout = TestConfiguration.NavigationTimeout
        }).ConfigureAwait(false);

        // Wait for Blazor to initialize
        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded).ConfigureAwait(false);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle).ConfigureAwait(false);

        // Verification: Page state should be ready
        var readyState = await Page.EvaluateAsync<string>("document.readyState").ConfigureAwait(false);
        await Assert.That(readyState).IsEqualTo("complete");
    }
}
