// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Persistence.Common;

public abstract class AuditableBase : IAuditable
{
    [Key]
    public int Id { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime ModifiedOn { get; set; }
}
