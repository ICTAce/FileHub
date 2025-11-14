namespace ICTAce.FileHub.Repository;

public interface IMyModuleRepository
{
    IEnumerable<Shared.Models.MyModule> GetMyModules(int ModuleId);
    Shared.Models.MyModule GetMyModule(int MyModuleId);
    Shared.Models.MyModule GetMyModule(int MyModuleId, bool tracking);
    Shared.Models.MyModule AddMyModule(Shared.Models.MyModule MyModule);
    Shared.Models.MyModule UpdateMyModule(Shared.Models.MyModule MyModule);
    void DeleteMyModule(int MyModuleId);
}

public class MyModuleRepository : IMyModuleRepository, ITransientService
{
    private readonly IDbContextFactory<Context> _factory;

    public MyModuleRepository(IDbContextFactory<Context> factory)
    {
        _factory = factory;
    }

    public IEnumerable<Shared.Models.MyModule> GetMyModules(int ModuleId)
    {
        using var db = _factory.CreateDbContext();
        return db.MyModule.Where(item => item.ModuleId == ModuleId).ToList();
    }

    public Shared.Models.MyModule GetMyModule(int MyModuleId)
    {
        return GetMyModule(MyModuleId, true);
    }

    public Shared.Models.MyModule GetMyModule(int MyModuleId, bool tracking)
    {
        using var db = _factory.CreateDbContext();
        if (tracking)
        {
            return db.MyModule.Find(MyModuleId);
        }
        else
        {
            return db.MyModule.AsNoTracking().FirstOrDefault(item => item.MyModuleId == MyModuleId);
        }
    }

    public Shared.Models.MyModule AddMyModule(Shared.Models.MyModule MyModule)
    {
        using var db = _factory.CreateDbContext();
        db.MyModule.Add(MyModule);
        db.SaveChanges();
        return MyModule;
    }

    public Shared.Models.MyModule UpdateMyModule(Shared.Models.MyModule MyModule)
    {
        using var db = _factory.CreateDbContext();
        db.Entry(MyModule).State = EntityState.Modified;
        db.SaveChanges();
        return MyModule;
    }

    public void DeleteMyModule(int MyModuleId)
    {
        using var db = _factory.CreateDbContext();
        var MyModule = db.MyModule.Find(MyModuleId);
        db.MyModule.Remove(MyModule);
        db.SaveChanges();
    }
}
