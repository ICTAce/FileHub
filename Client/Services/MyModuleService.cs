// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Services;

public interface IMyModuleService
{
    Task<GetMyModuleResponse> GetAsync(int moduleId, int id);

    Task<PagedResult<ListMyModuleResponse>> ListAsync(int moduleId,int pageNumber, int pageSize);

    Task<int> CreateAsync(int moduleId, CreateMyModuleRequest request);

    Task<int> UpdateAsync(int moduleId, UpdateMyModuleRequest request);

    Task DeleteAsync(int moduleId, int id);
}

public class MyModuleService(HttpClient http, SiteState siteState) : ServiceBase(http, siteState), IMyModuleService
{
    private string Apiurl => CreateApiUrl("MyModule");

    public Task<GetMyModuleResponse> GetAsync(int moduleId, int id)
    {
        var url = CreateAuthorizationPolicyUrl($"{Apiurl}/{moduleId}/{id}", EntityNames.Module, moduleId);
        return GetJsonAsync<GetMyModuleResponse>(url);
    }

    public Task<PagedResult<ListMyModuleResponse>> ListAsync(int moduleId, int pageNumber, int pageSize)
    {
        var url = CreateAuthorizationPolicyUrl($"{Apiurl}/{moduleId}/?pageNumber={pageNumber}&pageSize={pageSize}", EntityNames.Module, moduleId);
        return GetJsonAsync<PagedResult<ListMyModuleResponse>>(url, new PagedResult<ListMyModuleResponse>());
    }

    public Task<int> CreateAsync(int moduleId, CreateMyModuleRequest request)
    {
        var url = CreateAuthorizationPolicyUrl($"{Apiurl}/{moduleId}", EntityNames.Module, moduleId);
        return PostJsonAsync<CreateMyModuleRequest, int>(url, request);
    }

    public Task<int> UpdateAsync(int moduleId, UpdateMyModuleRequest request)
    {
        var url = CreateAuthorizationPolicyUrl($"{Apiurl}/{moduleId}/{request.Id}", EntityNames.Module, moduleId);
        return PutJsonAsync<UpdateMyModuleRequest, int>(url, request);
    }

    public Task DeleteAsync(int moduleId, int id)
    {
        var url = CreateAuthorizationPolicyUrl($"{Apiurl}/{moduleId}/{id}", EntityNames.Module, moduleId);
        return DeleteAsync(url);
    }
}
