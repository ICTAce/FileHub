// Licensed to ICTAce under the MIT license.

using Oqtane.Models;
using Oqtane.Services;

namespace ICTAce.FileHub.Client.Tests.Mocks;

public class MockModuleService : IModuleService
{
    private readonly List<Module> _modules = new();
    private int _nextId = 1;

    public Task<List<Module>> GetModulesAsync(int siteId)
    {
        return Task.FromResult(_modules.Where(m => m.SiteId == siteId).ToList());
    }

    public Task<Module> GetModuleAsync(int moduleId)
    {
        var module = _modules.FirstOrDefault(m => m.ModuleId == moduleId);
        return Task.FromResult(module ?? new Module());
    }

    public Task<Module> AddModuleAsync(Module module)
    {
        module.ModuleId = _nextId++;
        _modules.Add(module);
        return Task.FromResult(module);
    }

    public Task<Module> UpdateModuleAsync(Module module)
    {
        var existing = _modules.FirstOrDefault(m => m.ModuleId == module.ModuleId);
        if (existing != null)
        {
            _modules.Remove(existing);
            _modules.Add(module);
        }
        return Task.FromResult(module);
    }

    public Task DeleteModuleAsync(int moduleId)
    {
        var module = _modules.FirstOrDefault(m => m.ModuleId == moduleId);
        if (module != null)
        {
            _modules.Remove(module);
        }
        return Task.CompletedTask;
    }

    public Task<bool> ImportModuleAsync(int moduleId, int importModuleId, int pageId)
    {
        return Task.FromResult(true);
    }

    public Task<bool> ImportModuleAsync(int moduleId, int importModuleId, string content)
    {
        return Task.FromResult(true);
    }

    public Task<Module> ExportModuleAsync(int moduleId)
    {
        return GetModuleAsync(moduleId);
    }

    public Task<string> ExportModuleAsync(int moduleId, int pageId)
    {
        return Task.FromResult(string.Empty);
    }

    public Task<int> ExportModuleAsync(int moduleId, int pageId, int templateId, string content)
    {
        return Task.FromResult(0);
    }
}
