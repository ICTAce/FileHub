// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MediatR;

namespace ICTAce.FileHub.Client.Features.MyModules;

public class UpdateMyModuleRequest : IRequest<int>
{
    public int MyModuleId { get; set; }
    public int ModuleId { get; set; }
    public string Name { get; set; }
}
