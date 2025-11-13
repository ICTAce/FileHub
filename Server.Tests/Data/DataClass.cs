// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using TUnit.Core.Interfaces;

namespace ICTAce.FileHub.Server.Tests;

public class DataClass : IAsyncInitializer, IAsyncDisposable
{
    public Task InitializeAsync()
    {
        return Console.Out.WriteLineAsync("Classes can be injected into tests, and they can perform some initialisation logic such as starting an in-memory server or a test container.");
    }

    public async ValueTask DisposeAsync()
    {
        await Console.Out.WriteLineAsync("And when the class is finished with, we can clean up any resources.");
    }
}
