// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Persistence.Entities;

public class FileCategory
{
    [Key]
    public int Id { get; set; }

    public int FileHubId { get; set; }
    public File FileHub { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}
