// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Server.Manager;

public class MyModuleManager : MigratableModuleBase, IInstallable, IPortable, ISearchable
{
    private readonly IDbContextFactory<Context> _contextFactory;
    private readonly IDBContextDependencies _DBContextDependencies;

    public MyModuleManager(IDbContextFactory<Context> contextFactory, IDBContextDependencies DBContextDependencies)
    {
        _contextFactory = contextFactory;
        _DBContextDependencies = DBContextDependencies;
    }

    public bool Install(Tenant tenant, string version)
    {
        return Migrate(new Context(_DBContextDependencies), tenant, MigrationType.Up);
    }

    public bool Uninstall(Tenant tenant)
    {
        return Migrate(new Context(_DBContextDependencies), tenant, MigrationType.Down);
    }

    public string ExportModule(Module module)
    {
        string content = "";

        // Direct data access - no repository layer
        using var db = _contextFactory.CreateDbContext();
        var MyModules = db.MyModule
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
        List<Entities.MyModule> MyModules = null;
        if (!string.IsNullOrEmpty(content))
        {
            MyModules = JsonSerializer.Deserialize<List<Entities.MyModule>>(content);
        }

        if (MyModules is not null)
        {
            // Direct data access - no repository layer
            using var db = _contextFactory.CreateDbContext();
            foreach (var task in MyModules)
            {
                db.MyModule.Add(new Entities.MyModule { ModuleId = module.ModuleId, Name = task.Name });
            }
            db.SaveChanges();
        }
    }

    public Task<List<SearchContent>> GetSearchContentsAsync(PageModule pageModule, DateTime lastIndexedOn)
    {
        var searchContentList = new List<SearchContent>();

        // Direct data access - no repository layer
        using var db = _contextFactory.CreateDbContext();
        foreach (var MyModule in db.MyModule.Where(item => item.ModuleId == pageModule.ModuleId))
        {
            if (MyModule.ModifiedOn >= lastIndexedOn)
            {
                searchContentList.Add(new SearchContent
                {
                    EntityName = "MyModule",
                    EntityId = MyModule.Id.ToString(),
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
