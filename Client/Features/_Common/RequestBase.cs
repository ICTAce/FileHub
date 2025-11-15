// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Common;

public abstract class RequestBase
{
    [Required(ErrorMessage = "ModuleId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "ModuleId must be greater than 0")]
    public int ModuleId { get; set; }
}
