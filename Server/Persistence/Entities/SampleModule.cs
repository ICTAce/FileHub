// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Persistence.Entities;

/// <summary>
/// Represents a module with auditable properties and a required name.
/// </summary>
public class SampleModule : AuditableModuleBase
{
    public required string Name { get; set; }
}
