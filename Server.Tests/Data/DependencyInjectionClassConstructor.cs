// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ICTAce.FileHub.Server.Tests.Data;

public class DependencyInjectionClassConstructor : IClassConstructor
{
    public Task<object> Create(Type type, ClassConstructorMetadata classConstructorMetadata)
    {
        Console.WriteLine(@"You can also control how your test classes are new'd up, giving you lots of power and the ability to utilise tools such as dependency injection");

        if (type == typeof(AndEvenMoreTests))
        {
            return Task.FromResult<object>(new AndEvenMoreTests(new DataClass()));
        }

        throw new NotImplementedException();
    }
}
