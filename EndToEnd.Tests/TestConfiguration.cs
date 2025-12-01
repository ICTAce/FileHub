// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.EndToEnd.Tests;

/// <summary>
/// Configuration settings for end-to-end tests.
/// </summary>
public static class TestConfiguration
{
    /// <summary>
    /// Base URL of the application under test.
    /// </summary>
    public static string BaseUrl => Environment.GetEnvironmentVariable("E2E_BASE_URL")
        ?? "http://localhost:5145";

    /// <summary>
    /// Default timeout for page operations in milliseconds.
    /// </summary>
    public static int DefaultTimeout => 30000;

    /// <summary>
    /// Timeout for navigation operations in milliseconds.
    /// </summary>
    public static int NavigationTimeout => 60000;
}
