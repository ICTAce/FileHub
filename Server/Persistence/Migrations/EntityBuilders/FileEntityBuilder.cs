// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Persistence.Migrations.EntityBuilders;

public class FileEntityBuilder : AuditableBaseEntityBuilder<FileEntityBuilder>
{
    private const string _entityTableName = "ICTAce_FileHub_File";
    private readonly PrimaryKey<FileEntityBuilder> _primaryKey = new("PK_ICTAce_FileHub_File", x => x.Id);
    private readonly ForeignKey<FileEntityBuilder> _moduleForeignKey = new("FK_ICTAce_FileHub_File_Module", x => x.ModuleId, "Module", "ModuleId", ReferentialAction.Cascade);

    public FileEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
    {
        EntityTableName = _entityTableName;
        PrimaryKey = _primaryKey;
        ForeignKeys.Add(_moduleForeignKey);
    }

    protected override FileEntityBuilder BuildTable(ColumnsBuilder table)
    {
        Id = AddAutoIncrementColumn(table, "Id");
        ModuleId = AddIntegerColumn(table, "ModuleId");
        Name = AddStringColumn(table, "Name", 100);
        FileName = AddStringColumn(table, "FileName", 255);
        ImageName = AddStringColumn(table, "ImageName", 255);
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
    public OperationBuilder<AddColumnOperation> ImageName { get; set; }
    public OperationBuilder<AddColumnOperation> Description { get; set; }
    public OperationBuilder<AddColumnOperation> FileSize { get; set; }
    public OperationBuilder<AddColumnOperation> Downloads { get; set; }
}
