// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Services.Common;

/// <summary>
/// Generic service implementation for module-scoped CRUD operations.
/// All operations include automatic retry with exponential backoff for transient failures.
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
    /// Gets a single entity by ID with automatic retry on transient failures.
    /// </summary>
    public virtual Task<TGetDto> GetAsync(int id, int moduleId)
    {
        return HttpRetryHelper.ExecuteWithRetryAsync(() =>
        {
            var url = CreateAuthorizationPolicyUrl($"{Apiurl}/{id}?moduleId={moduleId}", EntityNames.Module, moduleId);
            return GetJsonAsync<TGetDto>(url);
        });
    }

    /// <summary>
    /// Lists entities with pagination and automatic retry on transient failures.
    /// </summary>
    public virtual Task<PagedResult<TListDto>> ListAsync(int moduleId, int pageNumber = 1, int pageSize = 10)
    {
        return HttpRetryHelper.ExecuteWithRetryAsync(() =>
        {
            var url = CreateAuthorizationPolicyUrl($"{Apiurl}?moduleId={moduleId}&pageNumber={pageNumber}&pageSize={pageSize}", EntityNames.Module, moduleId);
            return GetJsonAsync<PagedResult<TListDto>>(url, new PagedResult<TListDto>());
        });
    }

    /// <summary>
    /// Creates a new entity with automatic retry on transient failures.
    /// </summary>
    public virtual Task<int> CreateAsync(int moduleId, TCreateUpdateDto dto)
    {
        return HttpRetryHelper.ExecuteWithRetryAsync(() =>
        {
            var url = CreateAuthorizationPolicyUrl($"{Apiurl}?moduleId={moduleId}", EntityNames.Module, moduleId);
            return PostJsonAsync<TCreateUpdateDto, int>(url, dto);
        });
    }

    /// <summary>
    /// Updates an existing entity with automatic retry on transient failures.
    /// </summary>
    public virtual Task<int> UpdateAsync(int id, int moduleId, TCreateUpdateDto dto)
    {
        return HttpRetryHelper.ExecuteWithRetryAsync(() =>
        {
            var url = CreateAuthorizationPolicyUrl($"{Apiurl}/{id}?moduleId={moduleId}", EntityNames.Module, moduleId);
            return PutJsonAsync<TCreateUpdateDto, int>(url, dto);
        });
    }

    /// <summary>
    /// Deletes an entity with automatic retry on transient failures.
    /// </summary>
    public virtual Task DeleteAsync(int id, int moduleId)
    {
        return HttpRetryHelper.ExecuteWithRetryAsync(() =>
        {
            var url = CreateAuthorizationPolicyUrl($"{Apiurl}/{id}?moduleId={moduleId}", EntityNames.Module, moduleId);
            return DeleteAsync(url);
        });
    }
}
