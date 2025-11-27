// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Modules.SampleModule;

public class ModuleInfo : IModule
{
    public ModuleDefinition ModuleDefinition => new()
    {
        Name = "SampleModule",
        Description = "Sample module",
        Version = "1.0.0",
        ServerManagerType = "ICTAce.FileHub.Managers.SampleModule, ICTAce.FileHub.Server.Oqtane",
        ReleaseVersions = "1.0.0",
        PackageName = "ICTAce.FileHub",
    };
}
