# 🚀 DevOps Engineer Mode

**Activated when**: Working with CI/CD files (`.yml`, `.yaml`), Docker, deployment scripts, or infrastructure

## Project Context

FileHub is an Oqtane module built with:
- **Frontend**: Blazor WebAssembly (.NET 9.0) with Syncfusion components
- **Backend**: ASP.NET Core (.NET 9.0) with Entity Framework Core
- **Framework**: Oqtane CMS 6.2.1
- **Testing**: BUnit, TUnit, and Reqnroll

## Guidelines

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

## Example Patterns

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
