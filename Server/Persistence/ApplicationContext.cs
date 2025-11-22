// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Persistence;

public class ApplicationContext : DBContextBase, ITransientService, IMultiDatabase
{
    public virtual DbSet<Entities.SampleModule> SampleModule { get; set; }
    public virtual DbSet<Entities.Category> Category { get; set; }

    public ApplicationContext(IDBContextDependencies DBContextDependencies) : base(DBContextDependencies)
    {
        // ContextBase handles multi-tenant database connections
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Entities.SampleModule>().ToTable(ActiveDatabase.RewriteName("Company_SampleModule"));
        builder.Entity<Entities.Category>().ToTable(ActiveDatabase.RewriteName("FileHub_Category"));
    }
}
