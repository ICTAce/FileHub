// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Persistence.Entities;

public class Category : AuditableModuleBase
{
    [MaxLength(100)]
    public required string Name { get; set; }

    public int ViewOrder { get; set; }

    public int? ParentId { get; set; }

    public Category? ParentCategory { get; set; }

    public ICollection<Category>? Subcategories { get; set; }
}
