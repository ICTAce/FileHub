// Licensed to ICTAce under the MIT license.

using System.Net.Http.Json;

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
    public IList<ListCategoryDto> Children { get; set; } = [];
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
    Task<int> MoveUpAsync(int id, int moduleId);
    Task<int> MoveDownAsync(int id, int moduleId);
}

public class CategoryService : ModuleService<GetCategoryDto, ListCategoryDto, CreateAndUpdateCategoryDto>, ICategoryService
{
    private readonly HttpClient _httpClient;

    public CategoryService(HttpClient http, SiteState siteState)
        : base(http, siteState, "ictace/fileHub/categories")
    {
        _httpClient = http;
    }

    /// <summary>
    /// Moves a category up in the sort order with automatic retry on transient failures.
    /// </summary>
    /// <param name="id">The category ID to move.</param>
    /// <param name="moduleId">The module ID.</param>
    /// <returns>The updated category ID.</returns>
    public async Task<int> MoveUpAsync(int id, int moduleId)
    {
        return await HttpRetryHelper.ExecuteWithRetryAsync(async () =>
        {
            var url = $"api/ictace/fileHub/categories/{id}/move-up?moduleId={moduleId}";
            var response = await _httpClient.PatchAsync(url, null).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<int>().ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    /// <summary>
    /// Moves a category down in the sort order with automatic retry on transient failures.
    /// </summary>
    /// <param name="id">The category ID to move.</param>
    /// <param name="moduleId">The module ID.</param>
    /// <returns>The updated category ID.</returns>
    public async Task<int> MoveDownAsync(int id, int moduleId)
    {
        return await HttpRetryHelper.ExecuteWithRetryAsync(async () =>
        {
            var url = $"api/ictace/fileHub/categories/{id}/move-down?moduleId={moduleId}";
            var response = await _httpClient.PatchAsync(url, null).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<int>().ConfigureAwait(false);
        }).ConfigureAwait(false);
    }
}
