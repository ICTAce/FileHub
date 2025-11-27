// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Persistence.Common;

public abstract class AuditableModuleBase : AuditableBase
{
    public int ModuleId { get; set; }
}
