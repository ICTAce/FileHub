// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Services;

public record GetCategoryDto
{
    public int Id { get; set; }
    public int ModuleId { get; set; }
    public required string Name { get; set; }
    public int ViewOrder { get; set; }
    public int ParentId { get; set; }

    public required string CreatedBy { get; set; }
    public required DateTime CreatedOn { get; set; }
    public required string ModifiedBy { get; set; }
    public required DateTime ModifiedOn { get; set; }
}

public record ListCategoryDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int ViewOrder { get; set; }
    public int ParentId { get; set; }
}

public record CreateAndUpdateCategoryDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "ViewOrder must be greater than or equal to 0")]
    public int ViewOrder { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "ParentId must be greater than or equal to 0")]
    public int ParentId { get; set; }
}

/// <summary>
/// Service interface for Category operations
/// </summary>
public interface ICategoryService
{
    Task<GetCategoryDto> GetAsync(int id, int moduleId);

    Task<PagedResult<ListCategoryDto>> ListAsync(int moduleId, int pageNumber = 1, int pageSize = 10);

    Task<int> CreateAsync(int moduleId, CreateAndUpdateCategoryDto dto);

    Task<int> UpdateAsync(int id, int moduleId, CreateAndUpdateCategoryDto dto);

    Task DeleteAsync(int id, int moduleId);
}

/// <summary>
/// Service implementation for Category operations
/// </summary>
public class CategoryService(HttpClient http, SiteState siteState) : ServiceBase(http, siteState), ICategoryService
{
    private string Apiurl => CreateApiUrl("ictace/fileHub/categories");

    public Task<GetCategoryDto> GetAsync(int id, int moduleId)
    {
        var url = CreateAuthorizationPolicyUrl($"{Apiurl}/{id}?moduleId={moduleId}", EntityNames.Module, moduleId);
        return GetJsonAsync<GetCategoryDto>(url);
    }

    public Task<PagedResult<ListCategoryDto>> ListAsync(int moduleId, int pageNumber = 1, int pageSize = 10)
    {
        var url = CreateAuthorizationPolicyUrl($"{Apiurl}?moduleId={moduleId}&pageNumber={pageNumber}&pageSize={pageSize}", EntityNames.Module, moduleId);
        return GetJsonAsync<PagedResult<ListCategoryDto>>(url, new PagedResult<ListCategoryDto>());
    }

    public Task<int> CreateAsync(int moduleId, CreateAndUpdateCategoryDto dto)
    {
        var url = CreateAuthorizationPolicyUrl($"{Apiurl}?moduleId={moduleId}", EntityNames.Module, moduleId);
        return PostJsonAsync<CreateAndUpdateCategoryDto, int>(url, dto);
    }

    public Task<int> UpdateAsync(int id, int moduleId, CreateAndUpdateCategoryDto dto)
    {
        var url = CreateAuthorizationPolicyUrl($"{Apiurl}/{id}?moduleId={moduleId}", EntityNames.Module, moduleId);
        return PutJsonAsync<CreateAndUpdateCategoryDto, int>(url, dto);
    }

    public Task DeleteAsync(int id, int moduleId)
    {
        var url = CreateAuthorizationPolicyUrl($"{Apiurl}/{id}?moduleId={moduleId}", EntityNames.Module, moduleId);
        return DeleteAsync(url);
    }
}
