// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Persistence.Migrations;

[DbContext(typeof(ApplicationCommandContext))]
[Migration("ICTAce.FileHub.01.00.00.00")]
public class InitializeModule : MultiDatabaseMigration
{
    public InitializeModule(IDatabase database) : base(database)
    {
    }

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        var sampleModuleEntityBuilder = new SampleModuleEntityBuilder(migrationBuilder, ActiveDatabase);
        sampleModuleEntityBuilder.Create();

        var fileEntityBuilder = new EntityBuilders.FileEntityBuilder(migrationBuilder, ActiveDatabase);
        fileEntityBuilder.Create();

        var categoryEntityBuilder = new CategoryEntityBuilder(migrationBuilder, ActiveDatabase);
        categoryEntityBuilder.Create();

        var fileCategoryEntityBuilder = new FileCategoryEntityBuilder(migrationBuilder, ActiveDatabase);
        fileCategoryEntityBuilder.Create();
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        var sampleModuleEntityBuilder = new SampleModuleEntityBuilder(migrationBuilder, ActiveDatabase);
        sampleModuleEntityBuilder.Drop();

        var fileEntityBuilder = new EntityBuilders.FileEntityBuilder(migrationBuilder, ActiveDatabase);
        fileEntityBuilder.Drop();

        var categoryEntityBuilder = new CategoryEntityBuilder(migrationBuilder, ActiveDatabase);
        categoryEntityBuilder.Drop();

        var fileCategoryEntityBuilder = new FileCategoryEntityBuilder(migrationBuilder, ActiveDatabase);
        fileCategoryEntityBuilder.Drop();
    }
}
