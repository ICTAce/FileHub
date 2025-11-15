// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.MyModules;

public class DeleteMyModuleRequest : RequestBase, IRequest<int>
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Id must be greater than 0")]
    public int Id { get; set; }
}
