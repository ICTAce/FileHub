// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Persistence.Migrations.EntityBuilders;

public class SampleModuleEntityBuilder : AuditableBaseEntityBuilder<SampleModuleEntityBuilder>
{
    private const string _entityTableName = "Company_SampleModule";
    private readonly PrimaryKey<SampleModuleEntityBuilder> _primaryKey = new("PK_SampleModule", x => x.Id);
    private readonly ForeignKey<SampleModuleEntityBuilder> _moduleForeignKey = new("FK_SampleModule_Module", x => x.ModuleId, "Module", "ModuleId", ReferentialAction.Cascade);

    public SampleModuleEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
    {
        EntityTableName = _entityTableName;
        PrimaryKey = _primaryKey;
        ForeignKeys.Add(_moduleForeignKey);
    }

    protected override SampleModuleEntityBuilder BuildTable(ColumnsBuilder table)
    {
        Id = AddAutoIncrementColumn(table, "Id");
        ModuleId = AddIntegerColumn(table, "ModuleId");
        Name = AddMaxStringColumn(table, "Name");
        AddAuditableColumns(table);
        return this;
    }

    public OperationBuilder<AddColumnOperation> Id { get; set; }
    public OperationBuilder<AddColumnOperation> ModuleId { get; set; }
    public OperationBuilder<AddColumnOperation> Name { get; set; }
}
