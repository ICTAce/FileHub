// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Services;

public record GetFileDto
{
    public int Id { get; set; }
    public int ModuleId { get; set; }
    public required string Name { get; set; }
    public required string FileName { get; set; }
    public required string ImageName { get; set; }
    public string? Description { get; set; }
    public required string FileSize { get; set; }
    public int Downloads { get; set; }
    public List<int> CategoryIds { get; set; } = [];

    public required string CreatedBy { get; set; }
    public required DateTime CreatedOn { get; set; }
    public required string ModifiedBy { get; set; }
    public required DateTime ModifiedOn { get; set; }
}

public record ListFileDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string FileName { get; set; }
    public required string ImageName { get; set; }
    public string? Description { get; set; }
    public required string FileSize { get; set; }
    public int Downloads { get; set; }
    public required DateTime CreatedOn { get; set; }
}

public record CreateAndUpdateFileDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "FileName is required")]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "FileName must be between 1 and 255 characters")]
    public string FileName { get; set; } = string.Empty;

    [Required(ErrorMessage = "ImageName is required")]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "ImageName must be between 1 and 255 characters")]
    public string ImageName { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description must not exceed 1000 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "FileSize is required")]
    [StringLength(12, MinimumLength = 1, ErrorMessage = "FileSize must be between 1 and 12 characters")]
    public string FileSize { get; set; } = string.Empty;

    public int Downloads { get; set; }
    
    public List<int> CategoryIds { get; set; } = [];
}

public interface IFileService
{
    Task<GetFileDto> GetAsync(int id, int moduleId);
    Task<PagedResult<ListFileDto>> ListAsync(int moduleId, int pageNumber = 1, int pageSize = 10);
    Task<int> CreateAsync(int moduleId, CreateAndUpdateFileDto dto);
    Task<int> UpdateAsync(int id, int moduleId, CreateAndUpdateFileDto dto);
    Task DeleteAsync(int id, int moduleId);
    Task<string> UploadFileAsync(int moduleId, Stream fileStream, string fileName);
}

public class FileService(HttpClient http, SiteState siteState)
    : ModuleService<GetFileDto, ListFileDto, CreateAndUpdateFileDto>(http, siteState, "ictace/fileHub/files"),
      IFileService
{
    private readonly HttpClient _http = http;
    
    public async Task<string> UploadFileAsync(int moduleId, Stream fileStream, string fileName)
    {
        var url = CreateAuthorizationPolicyUrl($"{CreateApiUrl("ictace/fileHub/files")}/upload?moduleId={moduleId}", EntityNames.Module, moduleId);
        
        using var content = new MultipartFormDataContent();
        using var streamContent = new StreamContent(fileStream);
        streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
        content.Add(streamContent, "file", fileName);
        
        var response = await _http.PostAsync(url, content).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
    }
}
