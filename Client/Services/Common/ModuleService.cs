// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Services.Common;

/// <summary>
/// Generic service implementation for module-scoped CRUD operations.
/// </summary>
/// <typeparam name="TGetDto">DTO type for get operations</typeparam>
/// <typeparam name="TListDto">DTO type for list operations</typeparam>
/// <typeparam name="TCreateUpdateDto">DTO type for create and update operations</typeparam>
public abstract class ModuleService<TGetDto, TListDto, TCreateUpdateDto>(
    HttpClient http,
    SiteState siteState,
    string apiPath)
    : ServiceBase(http, siteState)
{
    private string Apiurl => CreateApiUrl(apiPath);

    /// <summary>
    /// Gets a single entity by ID.
    /// </summary>
    public virtual Task<TGetDto> GetAsync(int id, int moduleId)
    {
        var url = CreateAuthorizationPolicyUrl($"{Apiurl}/{id}?moduleId={moduleId}", EntityNames.Module, moduleId);
        return GetJsonAsync<TGetDto>(url);
    }

    /// <summary>
    /// Lists entities with pagination.
    /// </summary>
    public virtual Task<PagedResult<TListDto>> ListAsync(int moduleId, int pageNumber = 1, int pageSize = 10)
    {
        var url = CreateAuthorizationPolicyUrl($"{Apiurl}?moduleId={moduleId}&pageNumber={pageNumber}&pageSize={pageSize}", EntityNames.Module, moduleId);
        return GetJsonAsync<PagedResult<TListDto>>(url, new PagedResult<TListDto>());
    }

    /// <summary>
    /// Creates a new entity.
    /// </summary>
    public virtual Task<int> CreateAsync(int moduleId, TCreateUpdateDto dto)
    {
        var url = CreateAuthorizationPolicyUrl($"{Apiurl}?moduleId={moduleId}", EntityNames.Module, moduleId);
        return PostJsonAsync<TCreateUpdateDto, int>(url, dto);
    }

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    public virtual Task<int> UpdateAsync(int id, int moduleId, TCreateUpdateDto dto)
    {
        var url = CreateAuthorizationPolicyUrl($"{Apiurl}/{id}?moduleId={moduleId}", EntityNames.Module, moduleId);
        return PutJsonAsync<TCreateUpdateDto, int>(url, dto);
    }

    /// <summary>
    /// Deletes an entity.
    /// </summary>
    public virtual Task DeleteAsync(int id, int moduleId)
    {
        var url = CreateAuthorizationPolicyUrl($"{Apiurl}/{id}?moduleId={moduleId}", EntityNames.Module, moduleId);
        return DeleteAsync(url);
    }
}
