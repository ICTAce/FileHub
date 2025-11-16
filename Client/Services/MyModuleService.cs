// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Services;

public interface IMyModuleService
{
    Task<GetMyModuleResponse> GetAsync(GetMyModuleRequest request);

    Task<PagedResult<ListMyModuleResponse>> ListAsync(ListMyModuleRequest request);

    Task<int> CreateAsync(CreateMyModuleRequest request);

    Task<int> UpdateAsync(UpdateMyModuleRequest request);

    Task DeleteAsync(DeleteMyModuleRequest request);
}

public class MyModuleService(HttpClient http, SiteState siteState) : ServiceBase(http, siteState), IMyModuleService
{
    private readonly HttpClient _http = http;

    private string Apiurl => CreateApiUrl("MyModule");

    public Task<GetMyModuleResponse> GetAsync(GetMyModuleRequest request)
    {
        return GetJsonAsync<GetMyModuleResponse>(CreateAuthorizationPolicyUrl($"{Apiurl}/{request.Id}/{request.ModuleId}", EntityNames.Module, request.ModuleId));
    }

    public async Task<PagedResult<ListMyModuleResponse>> ListAsync(ListMyModuleRequest request)
    {
        var url = CreateAuthorizationPolicyUrl(
            $"{Apiurl}?moduleid={request.ModuleId}&pageNumber={request.PageNumber}&pageSize={request.PageSize}", 
            EntityNames.Module, 
            request.ModuleId);

        var result = await GetJsonAsync<PagedResult<ListMyModuleResponse>>(url, new PagedResult<ListMyModuleResponse>()).ConfigureAwait(false);
        return result;
    }

    public async Task<int> CreateAsync(CreateMyModuleRequest request)
    {
        var response = await _http.PostAsJsonAsync(CreateAuthorizationPolicyUrl($"{Apiurl}", EntityNames.Module, request.ModuleId), request).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<int>().ConfigureAwait(false);
    }

    public async Task<int> UpdateAsync(UpdateMyModuleRequest request)
    {
        var response = await _http.PutAsJsonAsync(CreateAuthorizationPolicyUrl($"{Apiurl}/{request.Id}", EntityNames.Module, request.ModuleId), request).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<int>().ConfigureAwait(false);
    }

    public Task DeleteAsync(DeleteMyModuleRequest request)
    {
        return DeleteAsync(CreateAuthorizationPolicyUrl($"{Apiurl}/{request.Id}/{request.ModuleId}", EntityNames.Module, request.ModuleId));
    }
}
