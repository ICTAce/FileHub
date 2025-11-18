// Licensed to ICTAce under the MIT license.

// You can use attributes at the assembly level to apply to all tests in the assembly
[assembly: Retry(3)]
[assembly: System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]

namespace ICTAce.FileHub.Server.Tests;

public static class GlobalHooks
{
    [Before(TestSession)]
    public static void SetUp()
    {
        Console.WriteLine(@"Or you can define methods that do stuff before...");
    }

    [After(TestSession)]
    public static void CleanUp()
    {
        Console.WriteLine(@"...and after!");
    }
}
