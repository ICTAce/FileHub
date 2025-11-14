namespace ICTAce.FileHub.Migrations.EntityBuilders;

public class CategoryEntityBuilder : AuditableBaseEntityBuilder<CategoryEntityBuilder>
{
    private const string _entityTableName = "Category";
    private readonly PrimaryKey<CategoryEntityBuilder> _primaryKey = new("PK_Category", x => x.CategoryId);
    private readonly ForeignKey<CategoryEntityBuilder> _moduleForeignKey = new("FK_Category_Module", x => x.ModuleId, "Module", "ModuleId", ReferentialAction.Cascade);

    public CategoryEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
    {
        EntityTableName = _entityTableName;
        PrimaryKey = _primaryKey;
        ForeignKeys.Add(_moduleForeignKey);
    }

    protected override CategoryEntityBuilder BuildTable(ColumnsBuilder table)
    {
        CategoryId = AddAutoIncrementColumn(table, "CategoryId");
        ModuleId = AddIntegerColumn(table, "ModuleId");
        Name = AddStringColumn(table, "Name", 100);
        Description = AddStringColumn(table, "Description", 500, true);
        ParentCategoryId = AddIntegerColumn(table, "ParentCategoryId", true);
        AddAuditableColumns(table);
        return this;
    }

    public OperationBuilder<AddColumnOperation> CategoryId { get; set; }
    public OperationBuilder<AddColumnOperation> ModuleId { get; set; }
    public OperationBuilder<AddColumnOperation> Name { get; set; }
    public OperationBuilder<AddColumnOperation> Description { get; set; }
    public OperationBuilder<AddColumnOperation> ParentCategoryId { get; set; }
}
