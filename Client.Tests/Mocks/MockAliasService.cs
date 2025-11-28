// Licensed to ICTAce under the MIT license.

using Oqtane.Models;
using Oqtane.Services;

namespace ICTAce.FileHub.Client.Tests.Mocks;

public class MockAliasService : IAliasService
{
    private readonly List<Alias> _aliases = new();

    public MockAliasService()
    {
        _aliases.Add(new Alias
        {
            AliasId = 1,
            TenantId = 1,
            SiteId = 1,
            Name = "localhost",
            IsDefault = true,
        });
    }

    public Task<List<Alias>> GetAliasesAsync()
    {
        return Task.FromResult(_aliases.ToList());
    }

    public Task<Alias> GetAliasAsync(int aliasId)
    {
        var alias = _aliases.FirstOrDefault(a => a.AliasId == aliasId);
        return Task.FromResult(alias ?? new Alias());
    }

    public Task<Alias> GetAliasAsync(string aliasName, int siteId)
    {
        var alias = _aliases.FirstOrDefault(a => a.Name == aliasName && a.SiteId == siteId);
        return Task.FromResult(alias ?? new Alias());
    }

    public Task<Alias> AddAliasAsync(Alias alias)
    {
        _aliases.Add(alias);
        return Task.FromResult(alias);
    }

    public Task<Alias> UpdateAliasAsync(Alias alias)
    {
        var existing = _aliases.FirstOrDefault(a => a.AliasId == alias.AliasId);
        if (existing != null)
        {
            _aliases.Remove(existing);
            _aliases.Add(alias);
        }
        return Task.FromResult(alias);
    }

    public Task DeleteAliasAsync(int aliasId)
    {
        var alias = _aliases.FirstOrDefault(a => a.AliasId == aliasId);
        if (alias != null)
        {
            _aliases.Remove(alias);
        }
        return Task.CompletedTask;
    }
}
