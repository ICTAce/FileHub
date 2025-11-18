// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Client.Tests.Mocks;

public class MockStringLocalizerFactory : IStringLocalizerFactory
{
    public IStringLocalizer Create(Type resourceSource) => new MockStringLocalizer();
    public IStringLocalizer Create(string baseName, string location) => new MockStringLocalizer();
}

public class MockStringLocalizer : IStringLocalizer
{
    public LocalizedString this[string name] => new LocalizedString(name, name);
    
    public LocalizedString this[string name, params object[] arguments] 
        => new LocalizedString(name, string.Format(name, arguments));
    
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) 
        => Enumerable.Empty<LocalizedString>();
}

public class MockStringLocalizer<T> : MockStringLocalizer, IStringLocalizer<T>
{
}
