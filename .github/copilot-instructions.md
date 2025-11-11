# GitHub Copilot Instructions for FileHub

This file provides context-aware instructions for GitHub Copilot to assist with different development tasks in the FileHub project.

## Project Context

FileHub is an Oqtane module built with:
- **Frontend**: Blazor WebAssembly (.NET 9.0) with Syncfusion components
- **Backend**: ASP.NET Core (.NET 9.0) with Entity Framework Core
- **Framework**: Oqtane CMS 6.2.1
- **Testing**: BUnit, TUnit, and Reqnroll

## Chat Modes

Detect the user's current task based on context and provide specialized assistance:

---

### 🎨 Blazor Developer Mode
**Activated when**: Working with `.razor` files, Blazor components, or Syncfusion components

**Guidelines**:
- Use Blazor best practices for component lifecycle, data binding, and state management
- Leverage Syncfusion components for rich UI experiences (e.g., SfGrid, SfChart, SfDialog)
- Follow Oqtane module patterns for dependency injection and service registration
- Use `@inject` for service dependencies in components
- Implement proper error boundaries and loading states
- Follow Blazor naming conventions (PascalCase for components, camelCase for parameters)
- Use `EventCallback<T>` for component events
- Implement `IDisposable` when subscribing to events or using unmanaged resources
- Use `StateHasChanged()` appropriately for manual UI updates
- Leverage Oqtane's `ISettingService` for module settings
- Use Oqtane's `IStringLocalizer` for internationalization

**Example Patterns**:
```razor
@inject IFileService FileService
@inject NavigationManager NavigationManager
@implements IDisposable

<SfGrid TValue="FileInfo" DataSource="@files">
    <GridColumns>
        <GridColumn Field="@nameof(FileInfo.Name)" HeaderText="File Name"></GridColumn>
    </GridColumns>
</SfGrid>

@code {
    private List<FileInfo> files;
    
    protected override async Task OnInitializedAsync()
    {
        files = await FileService.GetFilesAsync();
    }
}
```

---

### 🎨 HTML Designer Mode
**Activated when**: Working with `.html`, `.css`, `.scss` files, or styling tasks

**Guidelines**:
- Write semantic HTML5 markup
- Use modern CSS features (Flexbox, Grid, Custom Properties)
- Follow BEM methodology for CSS class naming when appropriate
- Ensure responsive design using mobile-first approach
- Use CSS variables for theming and consistency
- Implement accessibility features (ARIA labels, semantic elements, keyboard navigation)
- Optimize for performance (minimize reflows, use CSS animations over JavaScript)
- Follow Oqtane's theming conventions
- Use utility-first CSS classes when available
- Ensure cross-browser compatibility

**Example Patterns**:
```css
:root {
    --primary-color: #0066cc;
    --spacing-unit: 8px;
}

.file-hub__container {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
    gap: calc(var(--spacing-unit) * 2);
    padding: var(--spacing-unit);
}

.file-hub__item {
    border: 1px solid #e0e0e0;
    border-radius: 4px;
    transition: transform 0.2s ease;
}

.file-hub__item:hover {
    transform: translateY(-2px);
}
```

---

### 💻 .NET Backend Developer Mode
**Activated when**: Working with controllers, services, repositories, Entity Framework, or database migrations

**Guidelines**:
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

**Example Patterns**:
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

---

### 🚀 DevOps Engineer Mode
**Activated when**: Working with CI/CD files (`.yml`, `.yaml`), Docker, deployment scripts, or infrastructure

**Guidelines**:
- Create GitHub Actions workflows for CI/CD
- Implement multi-stage builds for Docker containers
- Use caching to optimize build times
- Implement automated testing in pipelines
- Use secrets management for sensitive data
- Implement proper versioning and tagging strategies
- Create health checks and monitoring
- Use infrastructure as code principles
- Implement blue-green or canary deployments when possible
- Document deployment procedures
- Use matrix builds for testing across multiple configurations

**Example Patterns**:
```yaml
name: CI/CD Pipeline

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '9.0.x'
        
    - name: Restore dependencies
      run: dotnet restore
      
    - name: Build
      run: dotnet build --no-restore --configuration Release
      
    - name: Test
      run: dotnet test --no-build --configuration Release --verbosity normal
      
    - name: Publish
      run: dotnet publish --no-build --configuration Release --output ./publish
      
    - name: Upload artifact
      uses: actions/upload-artifact@v4
      with:
        name: filehub-module
        path: ./publish
```

---

### 🧪 Tester Mode
**Activated when**: Working with test files, BUnit, TUnit, Reqnroll, or testing-related tasks

**Guidelines**:
- Write unit tests using TUnit framework
- Write component tests using BUnit for Blazor components
- Write BDD tests using Reqnroll (Gherkin syntax)
- Follow AAA pattern (Arrange, Act, Assert)
- Use meaningful test names that describe the scenario
- Mock dependencies using Moq or NSubstitute
- Aim for high code coverage while focusing on critical paths
- Write tests that are independent and can run in any order
- Use test fixtures for shared setup
- Implement integration tests for API endpoints
- Test edge cases and error scenarios
- Use parameterized tests for multiple scenarios

**Example Patterns**:
```csharp
// TUnit Test
public class FileServiceTests
{
    private readonly Mock<IFileRepository> _mockRepository;
    private readonly FileService _service;
    
    public FileServiceTests()
    {
        _mockRepository = new Mock<IFileRepository>();
        _service = new FileService(_mockRepository.Object);
    }
    
    [Test]
    public async Task GetFilesByModuleId_ReturnsFiles_WhenFilesExist()
    {
        // Arrange
        var moduleId = 1;
        var expectedFiles = new List<File>
        {
            new File { Id = 1, Name = "test.txt", ModuleId = moduleId }
        };
        _mockRepository
            .Setup(r => r.GetFilesByModuleIdAsync(moduleId))
            .ReturnsAsync(expectedFiles);
        
        // Act
        var result = await _service.GetFilesByModuleIdAsync(moduleId);
        
        // Assert
        Assert.AreEqual(1, result.Count());
        Assert.AreEqual("test.txt", result.First().Name);
    }
}

// BUnit Test
public class FileListComponentTests : TestContext
{
    [Fact]
    public void FileListComponent_RendersFiles_WhenDataProvided()
    {
        // Arrange
        var mockService = new Mock<IFileService>();
        var files = new List<FileDto>
        {
            new FileDto { Id = 1, Name = "test.txt" }
        };
        mockService.Setup(s => s.GetFilesByModuleIdAsync(It.IsAny<int>()))
            .ReturnsAsync(files);
        Services.AddSingleton(mockService.Object);
        
        // Act
        var cut = RenderComponent<FileList>(parameters => parameters
            .Add(p => p.ModuleId, 1));
        
        // Assert
        cut.Find(".file-item").TextContent.Should().Contain("test.txt");
    }
}

// Reqnroll Test
Feature: File Management
    As a user
    I want to manage files
    So that I can organize my content

Scenario: Upload a new file
    Given I am on the FileHub module
    When I upload a file named "document.pdf"
    Then the file "document.pdf" should appear in the file list
    And I should see a success message
```

---

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
