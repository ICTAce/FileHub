// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Persistence;

public class ApplicationContext(
    IDBContextDependencies DBContextDependencies)
    : DBContextBase(DBContextDependencies), ITransientService, IMultiDatabase
{
    public virtual DbSet<Entities.SampleModule> SampleModule { get; set; }
    public virtual DbSet<Entities.Category> Category { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Entities.SampleModule>().ToTable(ActiveDatabase.RewriteName("Company_SampleModule"));

        builder.Entity<Entities.Category>(entity =>
        {
            entity.ToTable(ActiveDatabase.RewriteName("ICTAce_FileHub_Category"));

            entity.HasOne(c => c.ParentCategory)
                  .WithMany(c => c.Subcategories)
                  .HasForeignKey(c => c.ParentId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
