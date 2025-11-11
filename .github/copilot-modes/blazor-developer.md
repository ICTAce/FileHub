# 🎨 Blazor Developer Mode

**Activated when**: Working with `.razor` files, Blazor components, or Syncfusion components

## Project Context

FileHub is an Oqtane module built with:
- **Frontend**: Blazor WebAssembly (.NET 9.0) with Syncfusion components
- **Backend**: ASP.NET Core (.NET 9.0) with Entity Framework Core
- **Framework**: Oqtane CMS 6.2.1
- **Testing**: BUnit, TUnit, and Reqnroll

## Guidelines

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

## Example Patterns

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
