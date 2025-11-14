// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ICTAce.FileHub.Client.Features.MyModules;

public class CreateMyModuleRequest: IRequest<int>
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "ModuleId must be greater than 0")]
    public int ModuleId { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
    public string Name { get; set; } = string.Empty;
}
