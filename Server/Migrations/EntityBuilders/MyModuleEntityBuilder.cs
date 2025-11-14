namespace ICTAce.FileHub.Migrations.EntityBuilders;

public class MyModuleEntityBuilder : AuditableBaseEntityBuilder<MyModuleEntityBuilder>
{
    private const string _entityTableName = "MyModule";
    private readonly PrimaryKey<MyModuleEntityBuilder> _primaryKey = new("PK_MyModule", x => x.Id);
    private readonly ForeignKey<MyModuleEntityBuilder> _moduleForeignKey = new("FK_MyModule_Module", x => x.ModuleId, "Module", "ModuleId", ReferentialAction.Cascade);

    public MyModuleEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
    {
        EntityTableName = _entityTableName;
        PrimaryKey = _primaryKey;
        ForeignKeys.Add(_moduleForeignKey);
    }

    protected override MyModuleEntityBuilder BuildTable(ColumnsBuilder table)
    {
        Id = AddAutoIncrementColumn(table, "Id");
        ModuleId = AddIntegerColumn(table,"ModuleId");
        Name = AddMaxStringColumn(table,"Name");
        AddAuditableColumns(table);
        return this;
    }

    public OperationBuilder<AddColumnOperation> Id { get; set; }
    public OperationBuilder<AddColumnOperation> ModuleId { get; set; }
    public OperationBuilder<AddColumnOperation> Name { get; set; }
}
