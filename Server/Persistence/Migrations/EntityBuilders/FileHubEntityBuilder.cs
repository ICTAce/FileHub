// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Persistence.Migrations.EntityBuilders;

public class FileHubEntityBuilder : AuditableBaseEntityBuilder<FileHubEntityBuilder>
{
    private const string _entityTableName = "FileHub_FileHub";
    private readonly PrimaryKey<FileHubEntityBuilder> _primaryKey = new("PK_FileHub_FileHub", x => x.Id);
    private readonly ForeignKey<FileHubEntityBuilder> _moduleForeignKey = new("FK_FileHub_FileHub_Module", x => x.ModuleId, "Module", "ModuleId", ReferentialAction.Cascade);

    public FileHubEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
    {
        EntityTableName = _entityTableName;
        PrimaryKey = _primaryKey;
        ForeignKeys.Add(_moduleForeignKey);
    }

    protected override FileHubEntityBuilder BuildTable(ColumnsBuilder table)
    {
        Id = AddAutoIncrementColumn(table, "Id");
        ModuleId = AddIntegerColumn(table, "ModuleId");
        Name = AddMaxStringColumn(table, "Name");
        FileName = AddStringColumn(table, "FileName", 255);
        Description = AddStringColumn(table, "Description", 1000, nullable: true);
        FileSize = AddStringColumn(table, "FileSize", 12);
        Downloads = AddIntegerColumn(table, "Downloads");
        AddAuditableColumns(table);
        return this;
    }

    public OperationBuilder<AddColumnOperation> Id { get; set; }
    public OperationBuilder<AddColumnOperation> ModuleId { get; set; }
    public OperationBuilder<AddColumnOperation> Name { get; set; }
    public OperationBuilder<AddColumnOperation> FileName { get; set; }
    public OperationBuilder<AddColumnOperation> Description { get; set; }
    public OperationBuilder<AddColumnOperation> FileSize { get; set; }
    public OperationBuilder<AddColumnOperation> Downloads { get; set; }
}
