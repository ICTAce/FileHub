# 💻 .NET Backend Developer Mode

**Activated when**: Working with controllers, services, repositories, Entity Framework, or database migrations

## Project Context

FileHub is an Oqtane module built with:
- **Frontend**: Blazor WebAssembly (.NET 9.0) with Syncfusion components
- **Backend**: ASP.NET Core (.NET 9.0) with Entity Framework Core
- **Framework**: Oqtane CMS 6.2.1
- **Testing**: BUnit, TUnit, and Reqnroll

## Guidelines

- Follow Repository pattern for data access
- Use dependency injection for all services
- Implement async/await for all I/O operations
- Use Entity Framework Core best practices (eager/lazy loading, tracking)
- Follow REST API conventions (proper HTTP verbs, status codes)
- Implement proper error handling and logging
- Use DTOs for API data transfer
- Implement validation using Data Annotations or FluentValidation
- Follow Oqtane's security model for permissions and authorization
- Use Oqtane's `IRepository<T>` interface for data access
- Implement proper transaction management
- Use meaningful naming conventions (e.g., `IFileRepository`, `FileService`)

## Example Patterns

```csharp
// Repository
public interface IFileRepository : IRepository<File>
{
    Task<IEnumerable<File>> GetFilesByModuleIdAsync(int moduleId);
}

public class FileRepository : Repository<File>, IFileRepository
{
    private readonly IDbContextFactory<FileHubContext> _factory;
    
    public FileRepository(IDbContextFactory<FileHubContext> factory) : base(factory)
    {
        _factory = factory;
    }
    
    public async Task<IEnumerable<File>> GetFilesByModuleIdAsync(int moduleId)
    {
        using var db = await _factory.CreateDbContextAsync();
        return await db.Files
            .Where(f => f.ModuleId == moduleId)
            .OrderByDescending(f => f.CreatedOn)
            .ToListAsync();
    }
}

// Controller
[Authorize]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly IFileService _fileService;
    private readonly ILogger<FileController> _logger;
    
    public FileController(IFileService fileService, ILogger<FileController> logger)
    {
        _fileService = fileService;
        _logger = logger;
    }
    
    [HttpGet("{moduleId}")]
    public async Task<ActionResult<IEnumerable<FileDto>>> GetFiles(int moduleId)
    {
        try
        {
            var files = await _fileService.GetFilesByModuleIdAsync(moduleId);
            return Ok(files);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving files for module {ModuleId}", moduleId);
            return StatusCode(500, "An error occurred while retrieving files");
        }
    }
}
```

## General Development Guidelines

**For all modes**:
- Write clean, readable, and maintainable code
- Follow SOLID principles
- Use meaningful variable and method names
- Add XML documentation comments for public APIs
- Handle exceptions appropriately
- Use async/await for I/O operations
- Follow the existing code style and conventions in the project
- Consider security implications (XSS, SQL injection, CSRF)
- Implement proper logging for debugging and monitoring
- Write self-documenting code with comments only where necessary

**Oqtane-Specific Conventions**:
- Follow Oqtane's module structure and conventions
- Use Oqtane's built-in services (ISettingService, ILogService, IUserPermissions)
- Implement proper permission checks using `[Authorize]` attributes
- Follow Oqtane's localization patterns for multi-language support
- Use Oqtane's notification system for user feedback
