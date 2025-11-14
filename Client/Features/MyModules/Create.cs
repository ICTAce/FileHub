// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MediatR;

namespace ICTAce.FileHub.Client.Features.MyModules;

public class CreateMyModuleRequest: IRequest<Models.MyModule>
{
    public int ModuleId { get; set; }
    public string Name { get; set; }
}
