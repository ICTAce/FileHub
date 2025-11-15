// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.MyModules;

public class GetMyModuleRequest : RequestBase, IRequest<GetMyModuleResponse>
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0")]
    public int Id { get; set; }
}

public class GetMyModuleResponse
{
    public int Id { get; set; }
    public int ModuleId { get; set; }
    public string Name { get; set; }

    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string ModifiedBy { get; set; }
    public DateTime ModifiedOn { get; set; }
}
