// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Server.Persistence.Common;

public abstract class AuditableModuleBase : AuditableBase
{
    public int ModuleId { get; set; }
}
