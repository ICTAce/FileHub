using System.Net.Http.Json;
using ICTAce.FileHub.Client.Features.MyModules;

namespace ICTAce.FileHub.Services;

public interface IMyModuleService
{
    Task<GetMyModuleResponse> GetAsync(GetMyModuleRequest request);

    Task<List<ListMyModulesResponse>> ListAsync(ListMyModulesRequest request);

    Task<int> CreateAsync(CreateMyModuleRequest request);

    Task<int> UpdateAsync(UpdateMyModuleRequest request);

    Task DeleteAsync(DeleteMyModuleRequest request);
}

public class MyModuleService : ServiceBase, IMyModuleService
{
    private readonly HttpClient _http;

    public MyModuleService(HttpClient http, SiteState siteState) : base(http, siteState) 
    {
        _http = http;
    }

    private string Apiurl => CreateApiUrl("MyModule");

    public async Task<GetMyModuleResponse> GetAsync(GetMyModuleRequest request)
    {
        return await GetJsonAsync<GetMyModuleResponse>(CreateAuthorizationPolicyUrl($"{Apiurl}/{request.MyModuleId}/{request.ModuleId}", EntityNames.Module, request.ModuleId));
    }

    public async Task<List<ListMyModulesResponse>> ListAsync(ListMyModulesRequest request)
    {
        var modules = await GetJsonAsync<List<ListMyModulesResponse>>(CreateAuthorizationPolicyUrl($"{Apiurl}?moduleid={request.ModuleId}", EntityNames.Module, request.ModuleId), new List<ListMyModulesResponse>());
        return modules.OrderBy(item => item.Name).ToList();
    }

    public async Task<int> CreateAsync(CreateMyModuleRequest request)
    {
        var response = await _http.PostAsJsonAsync(CreateAuthorizationPolicyUrl($"{Apiurl}", EntityNames.Module, request.ModuleId), request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<int>();
    }

    public async Task<int> UpdateAsync(UpdateMyModuleRequest request)
    {
        var response = await _http.PutAsJsonAsync(CreateAuthorizationPolicyUrl($"{Apiurl}/{request.MyModuleId}", EntityNames.Module, request.ModuleId), request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<int>();
    }

    public async Task DeleteAsync(DeleteMyModuleRequest request)
    {
        await DeleteAsync(CreateAuthorizationPolicyUrl($"{Apiurl}/{request.MyModuleId}/{request.ModuleId}", EntityNames.Module, request.ModuleId));
    }
}
