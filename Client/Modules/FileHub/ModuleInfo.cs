// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Modules;

public class ModuleInfo : IModule
{
    public ModuleDefinition ModuleDefinition => new()
    {
        Name = "FileHub",
        Description = "",
        Version = "1.0.0",
        ServerManagerType = "ICTAce.FileHub.Managers.FileHub, ICTAce.FileHub.Server.Oqtane",
        ReleaseVersions = "1.0.0",
        PackageName = "ICTAce.FileHub",
    };
}
