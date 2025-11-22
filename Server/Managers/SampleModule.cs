// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Managers;

public class SampleModule(
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
        var sampleModule = db.SampleModule
            .Where(item => item.ModuleId == module.ModuleId)
            .ToList();

        if (sampleModule != null)
        {
            content = JsonSerializer.Serialize(sampleModule);
        }
        return content;
    }

    public void ImportModule(Module module, string content, string version)
    {
        List<Persistence.Entities.SampleModule> SampleModules = null;
        if (!string.IsNullOrEmpty(content))
        {
            SampleModules = JsonSerializer.Deserialize<List<Persistence.Entities.SampleModule>>(content);
        }

        if (SampleModules is not null)
        {
            // Direct data access - no repository layer
            using var db = _contextFactory.CreateDbContext();
            foreach (var task in SampleModules)
            {
                db.SampleModule.Add(new Persistence.Entities.SampleModule { ModuleId = module.ModuleId, Name = task.Name });
            }
            db.SaveChanges();
        }
    }

    public Task<List<SearchContent>> GetSearchContentsAsync(PageModule pageModule, DateTime lastIndexedOn)
    {
        var searchContentList = new List<SearchContent>();

        // Direct data access - no repository layer
        using var db = _contextFactory.CreateDbContext();
        foreach (var sampleModule in db.SampleModule.Where(item => item.ModuleId == pageModule.ModuleId))
        {
            if (sampleModule.ModifiedOn >= lastIndexedOn)
            {
                searchContentList.Add(new SearchContent
                {
                    EntityName = "MyModule",
                    EntityId = sampleModule.Id.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    Title = sampleModule.Name,
                    Body = sampleModule.Name,
                    ContentModifiedBy = sampleModule.ModifiedBy,
                    ContentModifiedOn = sampleModule.ModifiedOn
                });
            }
        }

        return Task.FromResult(searchContentList);
    }
}
