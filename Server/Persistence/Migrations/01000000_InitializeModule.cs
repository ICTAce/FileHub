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

        var fileHubEntityBuilder = new FileHubEntityBuilder(migrationBuilder, ActiveDatabase);
        fileHubEntityBuilder.Create();

        var categoryEntityBuilder = new CategoryEntityBuilder(migrationBuilder, ActiveDatabase);
        categoryEntityBuilder.Create();
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        var sampleModuleEntityBuilder = new SampleModuleEntityBuilder(migrationBuilder, ActiveDatabase);
        sampleModuleEntityBuilder.Drop();

        var fileHubEntityBuilder = new FileHubEntityBuilder(migrationBuilder, ActiveDatabase);
        fileHubEntityBuilder.Drop();

        var categoryEntityBuilder = new CategoryEntityBuilder(migrationBuilder, ActiveDatabase);
        categoryEntityBuilder.Drop();
    }
}
