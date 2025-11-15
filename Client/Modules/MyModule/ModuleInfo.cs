// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Modules.MyModule;

public class ModuleInfo : IModule
{
    public ModuleDefinition ModuleDefinition => new ModuleDefinition
    {
        Name = "MyModule",
        Description = "Example module",
        Version = "1.0.0",
        ServerManagerType = "ICTAce.FileHub.Manager.MyModuleManager, ICTAce.FileHub.Server.Oqtane",
        ReleaseVersions = "1.0.0",
        Dependencies = "ICTAce.FileHub.Shared.Oqtane",
        PackageName = "ICTAce.FileHub" 
    };
}
