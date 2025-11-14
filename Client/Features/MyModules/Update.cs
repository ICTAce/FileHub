// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ICTAce.FileHub.Client.Features.MyModules;

public class UpdateMyModuleCommand
{
    public int MyModuleId { get; set; }
    public int ModuleId { get; set; }
    public string Name { get; set; }
}
