namespace ICTAce.FileHub.Categories;

public class ModuleInfo : IModule
{
    public ModuleDefinition ModuleDefinition => new ModuleDefinition
    {
        Name = "Categories",
        Description = "Category Management Module",
        Version = "1.0.0",
        ServerManagerType = "ICTAce.FileHub.Manager.CategoryManager, ICTAce.FileHub.Server.Oqtane",
        ReleaseVersions = "1.0.0",
        Dependencies = "ICTAce.FileHub.Shared.Oqtane",
        PackageName = "ICTAce.FileHub" 
    };
}
