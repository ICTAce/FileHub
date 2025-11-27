// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Client;

internal static class Program
{
    static Task Main(string[] args)
    {
        // defer client startup to Oqtane - do not modify
        return Oqtane.Client.Program.Main(args);
    }
}
