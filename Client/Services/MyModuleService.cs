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
    private string Apiurl => CreateApiUrl("MyModule");

    public Task<GetMyModuleResponse> GetAsync(GetMyModuleRequest request)
    {
        var url = CreateAuthorizationPolicyUrl($"{Apiurl}/{request.ModuleId}/{request.Id}", EntityNames.Module, request.ModuleId);
        return GetJsonAsync<GetMyModuleResponse>(url);
    }

    public Task<PagedResult<ListMyModuleResponse>> ListAsync(ListMyModuleRequest request)
    {
        var url = CreateAuthorizationPolicyUrl($"{Apiurl}/{request.ModuleId}/?pageNumber={request.PageNumber}&pageSize={request.PageSize}", EntityNames.Module, request.ModuleId);
        return GetJsonAsync<PagedResult<ListMyModuleResponse>>(url, new PagedResult<ListMyModuleResponse>());
    }

    public Task<int> CreateAsync(CreateMyModuleRequest request)
    {
        var url = CreateAuthorizationPolicyUrl($"{Apiurl}/{request.ModuleId}", EntityNames.Module, request.ModuleId);
        return PostJsonAsync<CreateMyModuleRequest, int>(url, request);
    }

    public Task<int> UpdateAsync(UpdateMyModuleRequest request)
    {
        var url = CreateAuthorizationPolicyUrl($"{Apiurl}/{request.ModuleId}/{request.Id}", EntityNames.Module, request.ModuleId);
        return PutJsonAsync<UpdateMyModuleRequest, int>(url, request);
    }

    public Task DeleteAsync(DeleteMyModuleRequest request)
    {
        var url = CreateAuthorizationPolicyUrl($"{Apiurl}/{request.ModuleId}/{request.Id}", EntityNames.Module, request.ModuleId);
        return DeleteAsync(url);
    }
}
