// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Persistence.Entities;

/// <summary>
/// Represents a file entry with associated metadata, including name, file name, description, file size, and download
/// count.
/// </summary>
/// <remarks>This class is typically used to store and manage information about files within the application, such
/// as for file repositories or download modules. It includes auditing information inherited from the
/// AuditableModuleBase class.</remarks>
public class FileHub : AuditableModuleBase
{
    [Required]
    public required string Name { get; set; }

    [MaxLength(255)]
    public required string FileName { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(12)]
    public required string FileSize { get; set; }

    public int Downloads { get; set; }
}
