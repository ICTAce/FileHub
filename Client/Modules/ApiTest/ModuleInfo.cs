// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Modules.ApiTest;

public class ModuleInfo : IModule
{
    public ModuleDefinition ModuleDefinition => new ModuleDefinition
    {
        Name = "ApiTest",
        Description = "Module for testing backend APIs",
        Version = "1.0.0",
        ReleaseVersions = "1.0.0",
        PackageName = "ICTAce.FileHub",
    };
}
