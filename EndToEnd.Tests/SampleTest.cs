// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.EndToEnd.Tests;

public class SampleTest : PageTest
{
    [Test]
    public async Task Test()
    {
        await Page.GotoAsync("https://playwright.dev").ConfigureAwait(false);

        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("Playwright", RegexOptions.None, TimeSpan.FromSeconds(5))).ConfigureAwait(false);

        // create a locator
        var getStarted = Page.Locator("text=Get Started");

        // Expect an attribute "to be strictly equal" to the value.
        await Expect(getStarted).ToHaveAttributeAsync("href", "/docs/intro").ConfigureAwait(false);

        // Click the get started link.
        await getStarted.ClickAsync().ConfigureAwait(false);

        // Expects the URL to contain intro.
        await Expect(Page).ToHaveURLAsync(new Regex(".*intro", RegexOptions.None, TimeSpan.FromSeconds(5))).ConfigureAwait(false);
    }
}
