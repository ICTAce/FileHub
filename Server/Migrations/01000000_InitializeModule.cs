namespace ICTAce.FileHub.Migrations;

[DbContext(typeof(Context))]
[Migration("ICTAce.FileHub.01.00.00.00")]
public class InitializeModule : MultiDatabaseMigration
{
    public InitializeModule(IDatabase database) : base(database)
    {
    }

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        var myModuleEntityBuilder = new MyModuleEntityBuilder(migrationBuilder, ActiveDatabase);
        myModuleEntityBuilder.Create();
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        var myModuleEntityBuilder = new MyModuleEntityBuilder(migrationBuilder, ActiveDatabase);
        myModuleEntityBuilder.Drop();
    }
}
