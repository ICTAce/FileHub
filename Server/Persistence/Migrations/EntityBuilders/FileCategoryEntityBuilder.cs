// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Persistence.Migrations.EntityBuilders;

public class FileCategoryEntityBuilder : AuditableBaseEntityBuilder<FileCategoryEntityBuilder>
{
    private const string _entityTableName = "ICTAce_FileHub_FileCategory";
    private readonly PrimaryKey<FileCategoryEntityBuilder> _primaryKey = new("PK_ICTAce_FileHub_FileCategory", x => x.Id);
    private readonly ForeignKey<FileCategoryEntityBuilder> _fileForeignKey = new("FK_ICTAce_FileHub_FileCategory_File", x => x.FileHubId, "ICTAce_FileHub_File", "Id", ReferentialAction.Cascade);
    private readonly ForeignKey<FileCategoryEntityBuilder> _categoryForeignKey = new("FK_ICTAce_FileHub_FileCategory_Category", x => x.CategoryId, "ICTAce_FileHub_Category", "Id", ReferentialAction.Cascade);

    public FileCategoryEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
    {
        EntityTableName = _entityTableName;
        PrimaryKey = _primaryKey;
        ForeignKeys.Add(_fileForeignKey);
        ForeignKeys.Add(_categoryForeignKey);
    }

    protected override FileCategoryEntityBuilder BuildTable(ColumnsBuilder table)
    {
        Id = AddAutoIncrementColumn(table, "Id");
        FileHubId = AddIntegerColumn(table, "FileHubId");
        CategoryId = AddIntegerColumn(table, "CategoryId");
        AddAuditableColumns(table);
        return this;
    }

    public OperationBuilder<AddColumnOperation> Id { get; set; }
    public OperationBuilder<AddColumnOperation> FileHubId { get; set; }
    public OperationBuilder<AddColumnOperation> CategoryId { get; set; }
}