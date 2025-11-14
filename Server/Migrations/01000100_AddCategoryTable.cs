namespace ICTAce.FileHub.Migrations;

[DbContext(typeof(Context))]
[Migration("ICTAce.FileHub.01.00.01.00")]
public class AddCategoryTable : MultiDatabaseMigration
{
    public AddCategoryTable(IDatabase database) : base(database)
    {
    }

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        var categoryEntityBuilder = new CategoryEntityBuilder(migrationBuilder, ActiveDatabase);
        categoryEntityBuilder.Create();
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        var categoryEntityBuilder = new CategoryEntityBuilder(migrationBuilder, ActiveDatabase);
        categoryEntityBuilder.Drop();
    }
}
