// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Managers;

public class FileHub(
    IDbContextFactory<ApplicationCommandContext> contextFactory,
    IDBContextDependencies DBContextDependencies)
    : MigratableModuleBase, IInstallable, IPortable, ISearchable
{
    private readonly IDbContextFactory<ApplicationCommandContext> _contextFactory = contextFactory;
    private readonly IDBContextDependencies _DBContextDependencies = DBContextDependencies;

    public bool Install(Tenant tenant, string version)
    {
        return Migrate(new ApplicationCommandContext(_DBContextDependencies), tenant, MigrationType.Up);
    }

    public bool Uninstall(Tenant tenant)
    {
        return Migrate(new ApplicationCommandContext(_DBContextDependencies), tenant, MigrationType.Down);
    }

    public string ExportModule(Module module)
    {
        string content = "";

        // Direct data access - no repository layer
        using var db = _contextFactory.CreateDbContext();
        var files = db.File
            .Where(item => item.ModuleId == module.ModuleId)
            .ToList();

        if (files.Count > 0)
        {
            content = JsonSerializer.Serialize(files);
        }
        return content;
    }

    public void ImportModule(Module module, string content, string version)
    {
        List<Persistence.Entities.File>? files = null;
        if (!string.IsNullOrEmpty(content))
        {
            files = JsonSerializer.Deserialize<List<Persistence.Entities.File>>(content);
        }

        if (files is not null)
        {
            // Direct data access - no repository layer
            using var db = _contextFactory.CreateDbContext();
            foreach (var file in files)
            {
                db.File.Add(new Persistence.Entities.File 
                { 
                    ModuleId = module.ModuleId, 
                    Name = file.Name,
                    FileName = file.FileName,
                    ImageName = file.ImageName,
                    Description = file.Description,
                    FileSize = file.FileSize,
                    Downloads = file.Downloads
                });
            }
            db.SaveChanges();
        }
    }

    public Task<List<SearchContent>> GetSearchContentsAsync(PageModule pageModule, DateTime lastIndexedOn)
    {
        var searchContentList = new List<SearchContent>();

        // Direct data access - no repository layer
        using var db = _contextFactory.CreateDbContext();
        foreach (var file in db.File.Where(item => item.ModuleId == pageModule.ModuleId))
        {
            if (file.ModifiedOn >= lastIndexedOn)
            {
                searchContentList.Add(new SearchContent
                {
                    EntityName = "ICTAce_FileHub_File",
                    EntityId = file.Id.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    Title = file.Name,
                    Body = file.Description ?? string.Empty,
                    ContentModifiedBy = file.ModifiedBy,
                    ContentModifiedOn = file.ModifiedOn
                });
            }
        }

        return Task.FromResult(searchContentList);
    }
}
