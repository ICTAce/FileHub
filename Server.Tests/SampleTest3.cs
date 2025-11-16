// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ICTAce.FileHub.Server.Tests.Data;

namespace ICTAce.FileHub.Server.Tests;

[ClassDataSource<DataClass>]
[ClassConstructor<DependencyInjectionClassConstructor>]
public class AndEvenMoreTests(DataClass dataClass)
{
    [Test]
    public void HaveFun()
    {
        Console.WriteLine(dataClass);
        Console.WriteLine("For more information, check out the documentation");
        Console.WriteLine("https://tunit.dev/");
    }
}
