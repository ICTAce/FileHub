// Licensed to ICTAce under the MIT license.

using Oqtane.Models;
using Oqtane.Services;

namespace ICTAce.FileHub.Client.Tests.Mocks;

public class MockThemeService : IThemeService
{
    public Task<List<Theme>> GetThemesAsync()
    {
        return Task.FromResult(new List<Theme>());
    }

    public Task<List<Theme>> GetThemesAsync(int siteId)
    {
        return Task.FromResult(new List<Theme>());
    }

    public Task<List<ThemeControl>> GetThemeControlsAsync()
    {
        return Task.FromResult(new List<ThemeControl>());
    }

    public Task<List<ThemeControl>> GetThemeControlsAsync(string themeName)
    {
        return Task.FromResult(new List<ThemeControl>());
    }

    public Task<Theme> GetThemeAsync(string themeName)
    {
        return Task.FromResult(new Theme { ThemeName = themeName, Name = themeName });
    }

    public Task<Theme> GetThemeAsync(int themeId, int siteId)
    {
        return Task.FromResult(new Theme { ThemeId = themeId, ThemeName = "Test", Name = "Test" });
    }

    public Theme GetTheme(List<Theme> themes, string themeName)
    {
        return themes.FirstOrDefault(t => t.ThemeName == themeName) ?? new Theme();
    }

    public List<ThemeControl> GetThemeControls(List<Theme> themes)
    {
        return new List<ThemeControl>();
    }

    public List<ThemeControl> GetThemeControls(List<Theme> themes, string themeName)
    {
        return new List<ThemeControl>();
    }

    public List<ThemeControl> GetLayoutControls(List<Theme> themes, string themeName)
    {
        return new List<ThemeControl>();
    }

    public List<ThemeControl> GetContainerControls(List<Theme> themes, string themeName)
    {
        return new List<ThemeControl>();
    }

    public Task<List<string>> GetThemesAsync(string siteId)
    {
        return Task.FromResult(new List<string>());
    }

    public Task DeleteThemeAsync(string themeName)
    {
        return Task.CompletedTask;
    }

    public Task DeleteThemeAsync(int themeId, int siteId)
    {
        return Task.CompletedTask;
    }

    public Task<Theme> CreateThemeAsync(Theme theme)
    {
        return Task.FromResult(theme);
    }

    public Task UpdateThemeAsync(Theme theme)
    {
        return Task.CompletedTask;
    }

    public Task<List<Template>> GetThemeTemplatesAsync()
    {
        return Task.FromResult(new List<Template>());
    }
}

public class MockModuleDefinitionService : IModuleDefinitionService
{
    public Task<List<ModuleDefinition>> GetModuleDefinitionsAsync(int siteId)
    {
        return Task.FromResult(new List<ModuleDefinition>
        {
            new ModuleDefinition
            {
                ModuleDefinitionName = "ICTAce.FileHub.SampleModule",
                Name = "Sample Module",
                Version = "1.0.0"
            }
        });
    }

    public Task<ModuleDefinition> GetModuleDefinitionAsync(int moduleDefinitionId, int siteId)
    {
        return Task.FromResult(new ModuleDefinition
        {
            ModuleDefinitionId = moduleDefinitionId,
            ModuleDefinitionName = "ICTAce.FileHub.SampleModule",
            Name = "Sample Module",
            Version = "1.0.0"
        });
    }

    public Task<ModuleDefinition> GetModuleDefinitionAsync(string moduleDefinitionName, int siteId)
    {
        return Task.FromResult(new ModuleDefinition
        {
            ModuleDefinitionName = moduleDefinitionName,
            Name = "Sample Module",
            Version = "1.0.0"
        });
    }

    public Task UpdateModuleDefinitionAsync(ModuleDefinition moduleDefinition)
    {
        return Task.CompletedTask;
    }

    public Task InstallModuleDefinitionsAsync()
    {
        return Task.CompletedTask;
    }

    public Task DeleteModuleDefinitionAsync(int moduleDefinitionId, int siteId)
    {
        return Task.CompletedTask;
    }

    public Task<ModuleDefinition> CreateModuleDefinitionAsync(ModuleDefinition moduleDefinition)
    {
        return Task.FromResult(moduleDefinition);
    }

    public Task<List<Template>> GetModuleDefinitionTemplatesAsync()
    {
        return Task.FromResult(new List<Template>());
    }
}
