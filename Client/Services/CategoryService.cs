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
    public List<ListCategoryDto> Children { get; set; } = new();
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

public interface ICategoryService
{
    Task<GetCategoryDto> GetAsync(int id, int moduleId);
    Task<PagedResult<ListCategoryDto>> ListAsync(int moduleId, int pageNumber = 1, int pageSize = 10);
    Task<int> CreateAsync(int moduleId, CreateAndUpdateCategoryDto dto);
    Task<int> UpdateAsync(int id, int moduleId, CreateAndUpdateCategoryDto dto);
    Task DeleteAsync(int id, int moduleId);
}

public class CategoryService(HttpClient http, SiteState siteState)
    : ModuleService<GetCategoryDto, ListCategoryDto, CreateAndUpdateCategoryDto>(http, siteState, "ictace/fileHub/categories"),
      ICategoryService
{
}
