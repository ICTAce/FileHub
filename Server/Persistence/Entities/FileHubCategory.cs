// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Persistence.Entities;

/// <summary>
/// Represents the association between a file hub and a category.
/// </summary>
/// <remarks>This class is typically used to model a many-to-many relationship between file hubs and categories
/// within the data model. Each instance links a specific file hub to a specific category.</remarks>
public class FileHubCategory
{
    [Key]
    public int Id { get; set; }

    public int FileHubId { get; set; }
    public FileHub FileHub { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}
