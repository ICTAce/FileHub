// Licensed to ICTAce under the MIT license.

using Oqtane.Models;
using Oqtane.Services;

namespace ICTAce.FileHub.Client.Tests.Mocks;

/// <summary>
/// Mock implementation of ISettingService for testing.
/// Provides basic setting functionality without making actual API calls.
/// </summary>
public class MockSettingService : ISettingService
{
    private readonly Dictionary<string, Dictionary<int, Dictionary<string, string>>> _settings = new();
    private readonly List<Setting> _settingsList = new();
    private int _nextId = 1;

    public Task<Dictionary<string, string>> GetModuleSettingsAsync(int moduleId)
    {
        return GetSettingsAsync("Module", moduleId);
    }

    public Task<Dictionary<string, string>> GetPageSettingsAsync(int pageId)
    {
        return GetSettingsAsync("Page", pageId);
    }

    public Task<Dictionary<string, string>> GetSiteSettingsAsync(int siteId)
    {
        return GetSettingsAsync("Site", siteId);
    }

    public Task<Dictionary<string, string>> GetUserSettingsAsync(int userId)
    {
        return GetSettingsAsync("User", userId);
    }

    public Task<Dictionary<string, string>> GetVisitorSettingsAsync(int visitorId)
    {
        return GetSettingsAsync("Visitor", visitorId);
    }

    public Task<Dictionary<string, string>> GetSettingsAsync(string entityName, int entityId)
    {
        if (_settings.TryGetValue(entityName, out var entitySettings)
            && entitySettings.TryGetValue(entityId, out var settings))
        {
            return Task.FromResult(new Dictionary<string, string>(settings));
        }
        return Task.FromResult(new Dictionary<string, string>());
    }

    public Task<List<Setting>> GetSettingsAsync(string entityName, int entityId, string settingName)
    {
        var matchingSettings = _settingsList
            .Where(s => s.EntityName == entityName && s.EntityId == entityId && s.SettingName == settingName)
            .ToList();
        return Task.FromResult(matchingSettings);
    }

    public Task<Setting> GetSettingAsync(int entityId, string settingName)
    {
        var setting = _settingsList.FirstOrDefault(s => s.EntityId == entityId && s.SettingName == settingName);
        return Task.FromResult(setting ?? null!);
    }

    public Task<Setting> GetSettingAsync(string entityName, int entityId)
    {
        var setting = _settingsList.FirstOrDefault(s => s.EntityId == entityId);
        return Task.FromResult(setting ?? null!);
    }

    public Task<Setting> GetSettingAsync(int settingId)
    {
        var setting = _settingsList.FirstOrDefault(s => s.SettingId == settingId);
        return Task.FromResult(setting ?? null!);
    }

    public Task<Dictionary<string, string>> GetTenantSettingsAsync()
    {
        return GetSettingsAsync("Tenant", 0);
    }

    public Task<Dictionary<string, string>> GetHostSettingsAsync()
    {
        return GetSettingsAsync("Host", 0);
    }

    public Task<Dictionary<string, string>> GetPageModuleSettingsAsync(int pageModuleId)
    {
        return GetSettingsAsync("PageModule", pageModuleId);
    }

    public Task<Dictionary<string, string>> GetModuleDefinitionSettingsAsync(int moduleDefinitionId)
    {
        return GetSettingsAsync("ModuleDefinition", moduleDefinitionId);
    }

    public Task<Dictionary<string, string>> GetFolderSettingsAsync(int folderId)
    {
        return GetSettingsAsync("Folder", folderId);
    }

    public Task<Setting> AddSettingAsync(Setting setting)
    {
        setting.SettingId = _nextId++;
        _settingsList.Add(setting);
        EnsureEntity(setting.EntityName, setting.EntityId);
        _settings[setting.EntityName][setting.EntityId][setting.SettingName] = setting.SettingValue;
        return Task.FromResult(setting);
    }

    public Task<Setting> UpdateSettingAsync(Setting setting)
    {
        var existing = _settingsList.FirstOrDefault(s => s.SettingId == setting.SettingId);
        if (existing != null)
        {
            _settingsList.Remove(existing);
            _settingsList.Add(setting);
            EnsureEntity(setting.EntityName, setting.EntityId);
            _settings[setting.EntityName][setting.EntityId][setting.SettingName] = setting.SettingValue;
        }
        return Task.FromResult(setting);
    }

    public Task AddOrUpdateSettingAsync(string entityName, int entityId, string settingName, string settingValue, bool isPrivate)
    {
        EnsureEntity(entityName, entityId);
        _settings[entityName][entityId][settingName] = settingValue;

        var existing = _settingsList.FirstOrDefault(s => s.EntityName == entityName
            && s.EntityId == entityId && s.SettingName == settingName);
        if (existing != null)
        {
            existing.SettingValue = settingValue;
            existing.IsPrivate = isPrivate;
        }
        else
        {
            _settingsList.Add(new Setting
            {
                SettingId = _nextId++,
                EntityName = entityName,
                EntityId = entityId,
                SettingName = settingName,
                SettingValue = settingValue,
                IsPrivate = isPrivate
            });
        }
        return Task.CompletedTask;
    }

    public Task DeleteSettingAsync(int entityId, string settingName)
    {
        var toRemove = _settingsList.Where(s => s.EntityId == entityId && s.SettingName == settingName).ToList();
        foreach (var setting in toRemove)
        {
            _settingsList.Remove(setting);
            if (_settings.TryGetValue(setting.EntityName, out var entitySettings)
                && entitySettings.TryGetValue(entityId, out var settings))
            {
                settings.Remove(settingName);
            }
        }
        return Task.CompletedTask;
    }

    public Task DeleteSettingAsync(string entityName, int entityId)
    {
        var toRemove = _settingsList.Where(s => s.EntityName == entityName && s.EntityId == entityId).ToList();
        foreach (var setting in toRemove)
        {
            _settingsList.Remove(setting);
        }
        if (_settings.TryGetValue(entityName, out var entitySettings))
        {
            entitySettings.Remove(entityId);
        }
        return Task.CompletedTask;
    }

    public Task DeleteSettingAsync(string entityName, int entityId, string settingName)
    {
        var toRemove = _settingsList.Where(s => s.EntityName == entityName
            && s.EntityId == entityId && s.SettingName == settingName).ToList();
        foreach (var setting in toRemove)
        {
            _settingsList.Remove(setting);
        }
        if (_settings.TryGetValue(entityName, out var entitySettings)
            && entitySettings.TryGetValue(entityId, out var settings))
        {
            settings.Remove(settingName);
        }
        return Task.CompletedTask;
    }

    public Task DeleteSettingAsync(int settingId)
    {
        var setting = _settingsList.FirstOrDefault(s => s.SettingId == settingId);
        if (setting != null)
        {
            _settingsList.Remove(setting);
            if (_settings.TryGetValue(setting.EntityName, out var entitySettings)
                && entitySettings.TryGetValue(setting.EntityId, out var settings))
            {
                settings.Remove(setting.SettingName);
            }
        }
        return Task.CompletedTask;
    }

    public Task UpdateModuleSettingsAsync(Dictionary<string, string> moduleSettings, int moduleId)
    {
        return UpdateSettingsAsync(moduleSettings, "Module", moduleId);
    }

    public Task UpdatePageSettingsAsync(Dictionary<string, string> pageSettings, int pageId)
    {
        return UpdateSettingsAsync(pageSettings, "Page", pageId);
    }

    public Task UpdateSiteSettingsAsync(Dictionary<string, string> siteSettings, int siteId)
    {
        return UpdateSettingsAsync(siteSettings, "Site", siteId);
    }

    public Task UpdateUserSettingsAsync(Dictionary<string, string> userSettings, int userId)
    {
        return UpdateSettingsAsync(userSettings, "User", userId);
    }

    public Task UpdateVisitorSettingsAsync(Dictionary<string, string> visitorSettings, int visitorId)
    {
        return UpdateSettingsAsync(visitorSettings, "Visitor", visitorId);
    }

    public Task UpdateTenantSettingsAsync(Dictionary<string, string> tenantSettings)
    {
        return UpdateSettingsAsync(tenantSettings, "Tenant", 0);
    }

    public Task UpdateHostSettingsAsync(Dictionary<string, string> hostSettings)
    {
        return UpdateSettingsAsync(hostSettings, "Host", 0);
    }

    public Task UpdatePageModuleSettingsAsync(Dictionary<string, string> pageModuleSettings, int pageModuleId)
    {
        return UpdateSettingsAsync(pageModuleSettings, "PageModule", pageModuleId);
    }

    public Task UpdateModuleDefinitionSettingsAsync(Dictionary<string, string> moduleDefinitionSettings, int moduleDefinitionId)
    {
        return UpdateSettingsAsync(moduleDefinitionSettings, "ModuleDefinition", moduleDefinitionId);
    }

    public Task UpdateFolderSettingsAsync(Dictionary<string, string> folderSettings, int folderId)
    {
        return UpdateSettingsAsync(folderSettings, "Folder", folderId);
    }

    public Task UpdateSettingsAsync(Dictionary<string, string> settings, string entityName, int entityId)
    {
        EnsureEntity(entityName, entityId);
        foreach (var setting in settings)
        {
            _settings[entityName][entityId][setting.Key] = setting.Value;
        }
        return Task.CompletedTask;
    }

    public Task<List<string>> GetEntityNamesAsync()
    {
        return Task.FromResult(_settings.Keys.ToList());
    }

    public Task<List<int>> GetEntityIdsAsync(string entityName)
    {
        if (_settings.TryGetValue(entityName, out var entitySettings))
        {
            return Task.FromResult(entitySettings.Keys.ToList());
        }
        return Task.FromResult(new List<int>());
    }

    public Task<Result> ImportSettingsAsync(Result result)
    {
        return Task.FromResult(result);
    }

    public Task ClearSiteSettingsCacheAsync()
    {
        return Task.CompletedTask;
    }

    public string GetSetting(Dictionary<string, string> settings, string settingName, string defaultValue)
    {
        return settings.TryGetValue(settingName, out var value) ? value : defaultValue;
    }

    public Dictionary<string, string> SetSetting(Dictionary<string, string> settings, string settingName, string settingValue)
    {
        settings[settingName] = settingValue;
        return settings;
    }

    public Dictionary<string, string> SetSetting(Dictionary<string, string> settings, string settingName, string settingValue, bool replace)
    {
        if (replace || !settings.ContainsKey(settingName))
        {
            settings[settingName] = settingValue;
        }
        return settings;
    }

    public Dictionary<string, string> MergeSettings(Dictionary<string, string> settings1, Dictionary<string, string> settings2)
    {
        var merged = new Dictionary<string, string>(settings1);
        foreach (var setting in settings2)
        {
            merged[setting.Key] = setting.Value;
        }
        return merged;
    }

    private void EnsureEntity(string entityName, int entityId)
    {
        if (!_settings.ContainsKey(entityName))
        {
            _settings[entityName] = new Dictionary<int, Dictionary<string, string>>();
        }
        if (!_settings[entityName].ContainsKey(entityId))
        {
            _settings[entityName][entityId] = new Dictionary<string, string>();
        }
    }
}
