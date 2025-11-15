// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.MyModules;

public class CreateMyModuleRequest : RequestBase, IRequest<int>
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
    public string Name { get; set; } = string.Empty;
}
