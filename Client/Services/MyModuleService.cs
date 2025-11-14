using System.Net.Http.Json;
using ICTAce.FileHub.Client.Features.MyModules;

namespace ICTAce.FileHub.Services;

public interface IMyModuleService
{
    Task<List<Models.MyModule>> ListMyModulesAsync(int ModuleId);

    Task<Models.MyModule> GetMyModuleAsync(int MyModuleId, int ModuleId);

    Task<Models.MyModule> CreateMyModuleAsync(CreateMyModuleRequest command);

    Task<Models.MyModule> UpdateMyModuleAsync(UpdateMyModuleCommand command);

    Task DeleteMyModuleAsync(int MyModuleId, int ModuleId);
}

public class MyModuleService : ServiceBase, IMyModuleService
{
    private readonly HttpClient _http;

    public MyModuleService(HttpClient http, SiteState siteState) : base(http, siteState) 
    {
        _http = http;
    }

    private string Apiurl => CreateApiUrl("MyModule");

    public async Task<List<Models.MyModule>> ListMyModulesAsync(int ModuleId)
    {
        List<Models.MyModule> Tasks = await GetJsonAsync<List<Models.MyModule>>(CreateAuthorizationPolicyUrl($"{Apiurl}?moduleid={ModuleId}", EntityNames.Module, ModuleId), Enumerable.Empty<Models.MyModule>().ToList());
        return Tasks.OrderBy(item => item.Name).ToList();
    }

    public async Task<Models.MyModule> GetMyModuleAsync(int MyModuleId, int ModuleId)
    {
        return await GetJsonAsync<Models.MyModule>(CreateAuthorizationPolicyUrl($"{Apiurl}/{MyModuleId}/{ModuleId}", EntityNames.Module, ModuleId));
    }

    public async Task<Models.MyModule> CreateMyModuleAsync(CreateMyModuleRequest command)
    {
        var response = await _http.PostAsJsonAsync(CreateAuthorizationPolicyUrl($"{Apiurl}", EntityNames.Module, command.ModuleId), command);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Models.MyModule>();
    }

    public async Task<Models.MyModule> UpdateMyModuleAsync(UpdateMyModuleCommand command)
    {
        var response = await _http.PutAsJsonAsync(CreateAuthorizationPolicyUrl($"{Apiurl}/{command.MyModuleId}", EntityNames.Module, command.ModuleId), command);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Models.MyModule>();
    }

    public async Task DeleteMyModuleAsync(int MyModuleId, int ModuleId)
    {
        await DeleteAsync(CreateAuthorizationPolicyUrl($"{Apiurl}/{MyModuleId}/{ModuleId}", EntityNames.Module, ModuleId));
    }
}
