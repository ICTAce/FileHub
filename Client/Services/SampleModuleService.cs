// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Services;

public record GetSampleModuleDto
{
    public int Id { get; set; }
    public int ModuleId { get; set; }
    public required string Name { get; set; }

    public required string CreatedBy { get; set; }
    public required DateTime CreatedOn { get; set; }
    public required string ModifiedBy { get; set; }
    public required DateTime ModifiedOn { get; set; }
}

public record ListSampleModuleDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public record CreateAndUpdateSampleModuleDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
    public string Name { get; set; } = string.Empty;
}

public interface ISampleModuleService
{
    Task<GetSampleModuleDto> GetAsync(int id, int moduleId);

    Task<PagedResult<ListSampleModuleDto>> ListAsync(int moduleId, int pageNumber = 1, int pageSize = 10);

    Task<int> CreateAsync(int moduleId, CreateAndUpdateSampleModuleDto dto);

    Task<int> UpdateAsync(int id, int moduleId, CreateAndUpdateSampleModuleDto dto);

    Task DeleteAsync(int id, int moduleId);
}

public class SampleModuleService(HttpClient http, SiteState siteState) : ServiceBase(http, siteState), ISampleModuleService
{
    private string Apiurl => CreateApiUrl("company/sampleModule");

    public Task<GetSampleModuleDto> GetAsync(int id, int moduleId)
    {
        var url = CreateAuthorizationPolicyUrl($"{Apiurl}/{id}?moduleId={moduleId}", EntityNames.Module, moduleId);
        return GetJsonAsync<GetSampleModuleDto>(url);
    }

    public Task<PagedResult<ListSampleModuleDto>> ListAsync(int moduleId, int pageNumber = 1, int pageSize = 10)
    {
        var url = CreateAuthorizationPolicyUrl($"{Apiurl}?moduleId={moduleId}&pageNumber={pageNumber}&pageSize={pageSize}", EntityNames.Module, moduleId);
        return GetJsonAsync<PagedResult<ListSampleModuleDto>>(url, new PagedResult<ListSampleModuleDto>());
    }

    public Task<int> CreateAsync(int moduleId, CreateAndUpdateSampleModuleDto dto)
    {
        var url = CreateAuthorizationPolicyUrl($"{Apiurl}?moduleId={moduleId}", EntityNames.Module, moduleId);
        return PostJsonAsync<CreateAndUpdateSampleModuleDto, int>(url, dto);
    }

    public Task<int> UpdateAsync(int id, int moduleId, CreateAndUpdateSampleModuleDto dto)
    {
        var url = CreateAuthorizationPolicyUrl($"{Apiurl}/{id}?moduleId={moduleId}", EntityNames.Module, moduleId);
        return PutJsonAsync<CreateAndUpdateSampleModuleDto, int>(url, dto);
    }

    public Task DeleteAsync(int id, int moduleId)
    {
        var url = CreateAuthorizationPolicyUrl($"{Apiurl}/{id}?moduleId={moduleId}", EntityNames.Module, moduleId);
        return DeleteAsync(url);
    }
}
