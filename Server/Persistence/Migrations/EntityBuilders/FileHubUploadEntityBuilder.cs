// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Persistence.Migrations.EntityBuilders;

public class FileHubUploadEntityBuilder : AuditableBaseEntityBuilder<FileHubUploadEntityBuilder>
{
    private const string _entityTableName = "ICTAce_FileHub_Upload";
    private readonly PrimaryKey<FileHubUploadEntityBuilder> _primaryKey = new("PK_ICTAce_FileHub_Upload", x => x.Id);
    private readonly ForeignKey<FileHubUploadEntityBuilder> _moduleForeignKey = new("FK_ICTAce_FileHub_Upload_Module", x => x.ModuleId, "Module", "ModuleId", ReferentialAction.Cascade);
    private readonly ForeignKey<FileHubUploadEntityBuilder> _categoryForeignKey = new("FK_ICTAce_FileHub_Upload_Category", x => x.CategoryId, "ICTAce_FileHub_Category", "Id", ReferentialAction.Restrict);

    public FileHubUploadEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
    {
        EntityTableName = _entityTableName;
        PrimaryKey = _primaryKey;
        ForeignKeys.Add(_moduleForeignKey);
        ForeignKeys.Add(_categoryForeignKey);
    }

    protected override FileHubUploadEntityBuilder BuildTable(ColumnsBuilder table)
    {
        Id = AddAutoIncrementColumn(table, "Id");
        ModuleId = AddIntegerColumn(table, "ModuleId");
        Title = AddStringColumn(table, "Title", 200);
        FileName = AddStringColumn(table, "FileName", 255);
        CategoryId = AddIntegerColumn(table, "CategoryId");
        Name = AddStringColumn(table, "Name", 100);
        Email = AddStringColumn(table, "Email", 100);
        Description = AddStringColumn(table, "Description", 1000, nullable: true);
        AddAuditableColumns(table);
        return this;
    }

    public OperationBuilder<AddColumnOperation> Id { get; set; }
    public OperationBuilder<AddColumnOperation> ModuleId { get; set; }
    public OperationBuilder<AddColumnOperation> Title { get; set; }
    public OperationBuilder<AddColumnOperation> FileName { get; set; }
    public OperationBuilder<AddColumnOperation> CategoryId { get; set; }
    public OperationBuilder<AddColumnOperation> Name { get; set; }
    public OperationBuilder<AddColumnOperation> Email { get; set; }
    public OperationBuilder<AddColumnOperation> Description { get; set; }
}
