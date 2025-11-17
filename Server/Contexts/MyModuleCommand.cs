// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Server.Contexts;

public class MyModuleCommand : DBContextBase, ITransientService, IMultiDatabase
{
    public virtual DbSet<Entities.MyModule> MyModule { get; set; }

    public MyModuleCommand(IDBContextDependencies DBContextDependencies) : base(DBContextDependencies)
    {
        // ContextBase handles multi-tenant database connections
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Entities.MyModule>().ToTable(ActiveDatabase.RewriteName("MyModule"));
    }
}
