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
        var MyModules = db.SampleModule
            .Where(item => item.ModuleId == module.ModuleId)
            .ToList();

        if (MyModules != null)
        {
            content = JsonSerializer.Serialize(MyModules);
        }
        return content;
    }

    public void ImportModule(Module module, string content, string version)
    {
        List<Persistence.Entities.SampleModule> MyModules = null;
        if (!string.IsNullOrEmpty(content))
        {
            MyModules = JsonSerializer.Deserialize<List<Persistence.Entities.SampleModule>>(content);
        }

        if (MyModules is not null)
        {
            // Direct data access - no repository layer
            using var db = _contextFactory.CreateDbContext();
            foreach (var task in MyModules)
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
        foreach (var MyModule in db.SampleModule.Where(item => item.ModuleId == pageModule.ModuleId))
        {
            if (MyModule.ModifiedOn >= lastIndexedOn)
            {
                searchContentList.Add(new SearchContent
                {
                    EntityName = "MyModule",
                    EntityId = MyModule.Id.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    Title = MyModule.Name,
                    Body = MyModule.Name,
                    ContentModifiedBy = MyModule.ModifiedBy,
                    ContentModifiedOn = MyModule.ModifiedOn
                });
            }
        }

        return Task.FromResult(searchContentList);
    }
}
