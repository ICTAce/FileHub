// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Services;

public record GetMyModuleDto
{
    public int Id { get; set; }
    public int ModuleId { get; set; }
    public required string Name { get; set; }

    public required string CreatedBy { get; set; }
    public required DateTime CreatedOn { get; set; }
    public required string ModifiedBy { get; set; }
    public required DateTime ModifiedOn { get; set; }
}

public record ListMyModuleDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public record CreateUpdateMyModuleDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
    public string Name { get; set; } = string.Empty;
}


public interface IMyModuleService
{
    Task<GetMyModuleDto> GetAsync(int id, int moduleId);

    Task<PagedResult<ListMyModuleDto>> ListAsync(int moduleId, int pageNumber = 1, int pageSize = 10);

    Task<int> CreateAsync(int moduleId, CreateUpdateMyModuleDto dto);

    Task<int> UpdateAsync(int id, int moduleId, CreateUpdateMyModuleDto dto);

    Task DeleteAsync(int id, int moduleId);
}


public class MyModuleService(HttpClient http, SiteState siteState) : ServiceBase(http, siteState), IMyModuleService
{
    private string Apiurl => CreateApiUrl("MyModule");

    public Task<GetMyModuleDto> GetAsync(int id, int moduleId)
    {
        var url = CreateAuthorizationPolicyUrl($"{Apiurl}/{id}?moduleId={moduleId}", EntityNames.Module, moduleId);
        return GetJsonAsync<GetMyModuleDto>(url);
    }

    public Task<PagedResult<ListMyModuleDto>> ListAsync(int moduleId, int pageNumber = 1, int pageSize = 10)
    {
        var url = CreateAuthorizationPolicyUrl($"{Apiurl}?moduleId={moduleId}&pageNumber={pageNumber}&pageSize={pageSize}", EntityNames.Module, moduleId);
        return GetJsonAsync<PagedResult<ListMyModuleDto>>(url, new PagedResult<ListMyModuleDto>());
    }

    public Task<int> CreateAsync(int moduleId, CreateUpdateMyModuleDto dto)
    {
        var url = CreateAuthorizationPolicyUrl($"{Apiurl}?moduleId={moduleId}", EntityNames.Module, moduleId);
        return PostJsonAsync<CreateUpdateMyModuleDto, int>(url, dto);
    }

    public Task<int> UpdateAsync(int id, int moduleId, CreateUpdateMyModuleDto dto)
    {
        var url = CreateAuthorizationPolicyUrl($"{Apiurl}/{id}?moduleId={moduleId}", EntityNames.Module, moduleId);
        return PutJsonAsync<CreateUpdateMyModuleDto, int>(url, dto);
    }

    public Task DeleteAsync(int id, int moduleId)
    {
        var url = CreateAuthorizationPolicyUrl($"{Apiurl}/{id}?moduleId={moduleId}", EntityNames.Module, moduleId);
        return DeleteAsync(url);
    }
}
