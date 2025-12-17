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

public class SampleModuleService(HttpClient http, SiteState siteState)
    : ModuleService<GetSampleModuleDto, ListSampleModuleDto, CreateAndUpdateSampleModuleDto>(http, siteState, "company/sampleModules"),
      ISampleModuleService
{
}
