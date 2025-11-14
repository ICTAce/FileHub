namespace ICTAce.FileHub.Repository;

public class Context : DBContextBase, ITransientService, IMultiDatabase
{
    public virtual DbSet<Shared.Models.MyModule> MyModule { get; set; }

    public Context(IDBContextDependencies DBContextDependencies) : base(DBContextDependencies)
    {
        // ContextBase handles multi-tenant database connections
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Shared.Models.MyModule>().ToTable(ActiveDatabase.RewriteName("MyModule"));
    }
}
