// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Persistence.Common;

/// <summary>
/// Provides a base class for modules that require audit tracking and identification by a unique module ID.
/// </summary>
public abstract class AuditableModuleBase : AuditableBase
{
    public int ModuleId { get; set; }
}
