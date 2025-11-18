// Licensed to ICTAce under the MIT license.

using ICTAce.FileHub.Server.Persistence.Common;

namespace ICTAce.FileHub.Server.Persistence.Entities;

public class MyModule : AuditableModuleBase
{
    [MaxLength(100)]
    public required string Name { get; set; }
}
