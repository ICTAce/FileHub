// Licensed to ICTAce under the MIT license.

using Oqtane.Models;
using Oqtane.Services;

namespace ICTAce.FileHub.Client.Tests.Mocks;

public class MockPageService : IPageService
{
    private readonly List<Page> _pages = new();
    private int _nextId = 1;

    public MockPageService()
    {
        // Add a default test page
        _pages.Add(new Page
        {
            PageId = 1,
            SiteId = 1,
            Path = "/test",
            Name = "Test Page",
            Title = "Test Page",
            IsNavigation = true,
            Url = "/test",
            IsPersonalizable = false,
            UserId = null,
            IsClickable = true,
            ParentId = null,
            Order = 1,
            Level = 0,
        });
        _nextId = 2;
    }

    public Task<List<Page>> GetPagesAsync(int siteId)
    {
        return Task.FromResult(_pages.Where(p => p.SiteId == siteId && !p.IsDeleted).ToList());
    }

    public Task<Page> GetPageAsync(int pageId)
    {
        var page = _pages.FirstOrDefault(p => p.PageId == pageId);
        return Task.FromResult(page ?? new Page());
    }

    public Task<Page> GetPageAsync(int pageId, int userId)
    {
        var page = _pages.FirstOrDefault(p => p.PageId == pageId);
        return Task.FromResult(page ?? new Page());
    }

    public Task<Page> GetPageAsync(string path, int siteId)
    {
        var page = _pages.FirstOrDefault(p => p.Path == path && p.SiteId == siteId);
        return Task.FromResult(page ?? new Page());
    }

    public Task<Page> AddPageAsync(Page page)
    {
        page.PageId = _nextId++;
        _pages.Add(page);
        return Task.FromResult(page);
    }

    public Task<Page> AddPageAsync(int pageId, int parentId)
    {
        var newPage = new Page
        {
            PageId = _nextId++,
            SiteId = 1,
            ParentId = parentId,
            Name = "New Page",
            Path = "/new-page",
            Order = 1,
            Level = 1
        };
        _pages.Add(newPage);
        return Task.FromResult(newPage);
    }

    public Task<Page> UpdatePageAsync(Page page)
    {
        var existing = _pages.FirstOrDefault(p => p.PageId == page.PageId);
        if (existing != null)
        {
            _pages.Remove(existing);
            _pages.Add(page);
        }
        return Task.FromResult(page);
    }

    public Task UpdatePageOrderAsync(int siteId, int pageId, int? parentId)
    {
        return Task.CompletedTask;
    }

    public Task DeletePageAsync(int pageId)
    {
        var page = _pages.FirstOrDefault(p => p.PageId == pageId);
        if (page != null)
        {
            page.IsDeleted = true;
        }
        return Task.CompletedTask;
    }
}
