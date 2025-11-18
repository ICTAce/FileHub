// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Server.Persistence.Entities;

public class MyModule : IAuditable
{
    [Key]
    public int Id { get; set; }
    public int ModuleId { get; set; }

    [MaxLength(100)]
    public required string Name { get; set; }

    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string ModifiedBy { get; set; }
    public DateTime ModifiedOn { get; set; }
}
