// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ICTAce.FileHub.Client.Features.MyModules;

public class GetMyModuleRequest : IRequest<GetMyModuleResponse>
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "ModuleId must be greater than 0")]
    public int ModuleId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "MyModuleId must be greater than 0")]
    public int MyModuleId { get; set; }
}

public class GetMyModuleResponse
{
    public int MyModuleId { get; set; }
    public int ModuleId { get; set; }
    public string Name { get; set; }

    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string ModifiedBy { get; set; }
    public DateTime ModifiedOn { get; set; }
}
