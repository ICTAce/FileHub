// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Persistence.Migrations.EntityBuilders;

public class CategoryEntityBuilder : AuditableBaseEntityBuilder<CategoryEntityBuilder>
{
    private const string _entityTableName = "ICTAce_FileHub_Category";
    private readonly PrimaryKey<CategoryEntityBuilder> _primaryKey = new("PK_ICTAce_FileHub_Category", x => x.Id);
    private readonly ForeignKey<CategoryEntityBuilder> _moduleForeignKey = new("FK_ICTAce_FileHub_Category_Module", x => x.ModuleId, "Module", "ModuleId", ReferentialAction.Cascade);
    private readonly ForeignKey<CategoryEntityBuilder> _parentCategoryForeignKey = new("FK_ICTAce_FileHub_Category_ParentCategory", x => x.ParentId, _entityTableName, "Id", ReferentialAction.Restrict);

    public CategoryEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
    {
        EntityTableName = _entityTableName;
        PrimaryKey = _primaryKey;
        ForeignKeys.Add(_moduleForeignKey);
        ForeignKeys.Add(_parentCategoryForeignKey);
    }

    protected override CategoryEntityBuilder BuildTable(ColumnsBuilder table)
    {
        Id = AddAutoIncrementColumn(table, "Id");
        ModuleId = AddIntegerColumn(table, "ModuleId");
        Name = AddStringColumn(table, "Name", 100);
        ViewOrder = AddIntegerColumn(table, "ViewOrder");
        ParentId = AddIntegerColumn(table, "ParentId", nullable: true);
        AddAuditableColumns(table);
        return this;
    }

    public OperationBuilder<AddColumnOperation> Id { get; set; }
    public OperationBuilder<AddColumnOperation> ModuleId { get; set; }
    public OperationBuilder<AddColumnOperation> Name { get; set; }
    public OperationBuilder<AddColumnOperation> ViewOrder { get; set; }
    public OperationBuilder<AddColumnOperation> ParentId { get; set; }
}
