// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Persistence.Entities;

/// <summary>
/// Represents a file upload with associated metadata, including title, file name, category, submitter information, and description.
/// </summary>
/// <remarks>This class is typically used to store file uploads from users, including their contact information
/// and categorization. It includes auditing information inherited from the AuditableModuleBase class.</remarks>
public class FileHubUpload : AuditableModuleBase
{
    [Required]
    [MaxLength(200)]
    public required string Title { get; set; }

    [Required]
    [MaxLength(255)]
    public required string FileName { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public required string Email { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }
}
