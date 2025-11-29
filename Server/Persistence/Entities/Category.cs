// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Persistence.Entities;

/// <summary>
/// Represents a category that can be organized hierarchically and ordered for display purposes.
/// </summary>
/// <remarks>A category may have a parent category, allowing for the creation of nested category structures. The
/// display order of categories can be controlled using the ViewOrder property. Inherits auditing properties from
/// AuditableModuleBase.</remarks>
public class Category : AuditableModuleBase
{
    [MaxLength(100)]
    public required string Name { get; set; }

    public int ViewOrder { get; set; }

    public int ParentId { get; set; }
}
