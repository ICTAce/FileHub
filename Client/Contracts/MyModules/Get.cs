// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Client.Contracts.MyModules;

public record GetMyModuleRequest : RequestBase, IRequest<GetMyModuleResponse>
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0")]
    public int Id { get; set; }
}

public record GetMyModuleResponse
{
    public int Id { get; set; }
    public int ModuleId { get; set; }
    public required string Name { get; set; }

    public required string CreatedBy { get; set; }
    public required DateTime CreatedOn { get; set; }
    public required string ModifiedBy { get; set; }
    public required DateTime ModifiedOn { get; set; }
}
