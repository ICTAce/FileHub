// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Persistence.Common;

/// <summary>
/// Provides a base class for entities that require audit information, including creation and modification metadata.
/// </summary>
/// <remarks>This abstract class defines common audit properties such as the user and timestamp for creation and
/// last modification. Inherit from this class to enable consistent auditing across entity types.</remarks>
public abstract class AuditableBase : IAuditable
{
    [Key]
    public int Id { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime ModifiedOn { get; set; }
}
