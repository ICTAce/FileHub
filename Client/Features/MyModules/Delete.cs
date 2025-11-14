// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ICTAce.FileHub.Client.Features.MyModules;

public class DeleteMyModuleRequest : IRequest<int>
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "ModuleId must be greater than 0")]
    public int ModuleId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "MyModuleId must be greater than 0")]
    public int MyModuleId { get; set; }
}
