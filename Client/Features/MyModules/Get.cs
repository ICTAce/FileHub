// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using MediatR;

namespace ICTAce.FileHub.Client.Features.MyModules;

public class GetMyModuleRequest : IRequest<GetMyModuleResponse>
{
    public int MyModuleId { get; set; }
    public int ModuleId { get; set; }
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
