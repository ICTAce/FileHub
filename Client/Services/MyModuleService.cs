// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Services;

public interface IMyModuleService
{
    Task<GetMyModuleResponse> GetAsync(GetMyModuleRequest request);

    Task<PagedResult<ListMyModulesResponse>> ListAsync(ListMyModulesRequest request);

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
        return await GetJsonAsync<GetMyModuleResponse>(CreateAuthorizationPolicyUrl($"{Apiurl}/{request.Id}/{request.ModuleId}", EntityNames.Module, request.ModuleId));
    }

    public async Task<PagedResult<ListMyModulesResponse>> ListAsync(ListMyModulesRequest request)
    {
        var url = CreateAuthorizationPolicyUrl(
            $"{Apiurl}?moduleid={request.ModuleId}&pageNumber={request.PageNumber}&pageSize={request.PageSize}", 
            EntityNames.Module, 
            request.ModuleId);

        var result = await GetJsonAsync<PagedResult<ListMyModulesResponse>>(url, new PagedResult<ListMyModulesResponse>());
        return result;
    }

    public async Task<int> CreateAsync(CreateMyModuleRequest request)
    {
        var response = await _http.PostAsJsonAsync(CreateAuthorizationPolicyUrl($"{Apiurl}", EntityNames.Module, request.ModuleId), request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<int>();
    }

    public async Task<int> UpdateAsync(UpdateMyModuleRequest request)
    {
        var response = await _http.PutAsJsonAsync(CreateAuthorizationPolicyUrl($"{Apiurl}/{request.Id}", EntityNames.Module, request.ModuleId), request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<int>();
    }

    public async Task DeleteAsync(DeleteMyModuleRequest request)
    {
        await DeleteAsync(CreateAuthorizationPolicyUrl($"{Apiurl}/{request.Id}/{request.ModuleId}", EntityNames.Module, request.ModuleId));
    }
}
