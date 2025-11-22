// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Modules.MyModule;

public class ModuleInfo : IModule
{
    public ModuleDefinition ModuleDefinition => new()
    {
        Name = "MyModule",
        Description = "Example module",
        Version = "1.0.0",
        ServerManagerType = "ICTAce.FileHub.Server.ApplicationManager, ICTAce.FileHub.Server.Oqtane",
        ReleaseVersions = "1.0.0",
        PackageName = "ICTAce.FileHub",
    };
}
