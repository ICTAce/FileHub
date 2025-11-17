// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Server.Contexts;

public class MyModuleCommandContext : DBContextBase, ITransientService, IMultiDatabase
{
    public virtual DbSet<Persistence.Entities.MyModule> MyModule { get; set; }

    public MyModuleCommandContext(IDBContextDependencies DBContextDependencies) : base(DBContextDependencies)
    {
        // ContextBase handles multi-tenant database connections
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Persistence.Entities.MyModule>().ToTable(ActiveDatabase.RewriteName("MyModule"));
    }
}
