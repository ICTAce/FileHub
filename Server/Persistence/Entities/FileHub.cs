// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Server.Persistence.Entities;

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
