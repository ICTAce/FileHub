// Licensed to ICTAce under the MIT license.

using Oqtane.Models;
using Oqtane.Services;

namespace ICTAce.FileHub.Client.Tests.Mocks;

public class MockSiteService : ISiteService
{
    private readonly List<Site> _sites = new();
    private int _nextId = 1;

    public MockSiteService()
    {
        // Add a default test site
        _sites.Add(new Site
        {
            SiteId = 1,
            TenantId = 1,
            Name = "Test Site",
            LogoFileId = null,
            FaviconFileId = null,
            DefaultThemeType = "Test.Theme",
            DefaultContainerType = "Test.Container",
        });
        _nextId = 2;
    }

    public Task<List<Site>> GetSitesAsync()
    {
        return Task.FromResult(_sites.Where(s => !s.IsDeleted).ToList());
    }

    public Task<Site> GetSiteAsync(int siteId)
    {
        var site = _sites.FirstOrDefault(s => s.SiteId == siteId);
        return Task.FromResult(site ?? new Site());
    }

    public Task<Site> AddSiteAsync(Site site)
    {
        site.SiteId = _nextId++;
        _sites.Add(site);
        return Task.FromResult(site);
    }

    public Task<Site> UpdateSiteAsync(Site site)
    {
        var existing = _sites.FirstOrDefault(s => s.SiteId == site.SiteId);
        if (existing != null)
        {
            _sites.Remove(existing);
            _sites.Add(site);
        }
        return Task.FromResult(site);
    }

    public Task DeleteSiteAsync(int siteId)
    {
        var site = _sites.FirstOrDefault(s => s.SiteId == siteId);
        if (site != null)
        {
            site.IsDeleted = true;
        }
        return Task.CompletedTask;
    }

    public Task<List<Module>> GetModulesAsync(int siteId, int pageId)
    {
        return Task.FromResult(new List<Module>());
    }

    public void SetAlias(Alias alias)
    {
        // No-op for testing
    }
}
