# 🧪 Tester Mode

**Activated when**: Working with test files, BUnit, TUnit, Reqnroll, or testing-related tasks

## Project Context

FileHub is an Oqtane module built with:
- **Frontend**: Blazor WebAssembly (.NET 9.0) with Syncfusion components
- **Backend**: ASP.NET Core (.NET 9.0) with Entity Framework Core
- **Framework**: Oqtane CMS 6.2.1
- **Testing**: BUnit, TUnit, and Reqnroll

## Guidelines

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

## Example Patterns

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
