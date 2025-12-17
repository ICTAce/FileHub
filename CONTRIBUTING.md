# Contributing to FileHub

Thank you for considering contributing to FileHub! This document outlines the process and guidelines for contributing to this project.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Workflow](#development-workflow)
- [Continuous Integration](#continuous-integration)
- [Code Quality Standards](#code-quality-standards)
- [Testing Requirements](#testing-requirements)
- [Pull Request Process](#pull-request-process)

## Code of Conduct

Please be respectful and constructive in all interactions. We aim to maintain a welcoming and inclusive environment for all contributors.

## Getting Started

### Prerequisites

- .NET 9.0 SDK or later
- Visual Studio 2022, JetBrains Rider, or VS Code
- SQL Server LocalDB (or another supported database)
- Git

### Setting Up Your Development Environment

1. Fork the repository on GitHub
2. Clone your fork locally:
   ```bash
   git clone https://github.com/YOUR-USERNAME/FileHub.git
   cd FileHub
   ```

3. Add the upstream repository as a remote:
   ```bash
   git remote add upstream https://github.com/ICTAce/FileHub.git
   ```

4. Restore dependencies:
   ```bash
   dotnet restore
   ```

5. Build the solution:
   ```bash
   dotnet build
   ```

6. Run tests to ensure everything works:
   ```bash
   dotnet test
   ```

## Development Workflow

1. **Create a feature branch** from `develop`:
   ```bash
   git checkout develop
   git pull upstream develop
   git checkout -b feature/your-feature-name
   ```

2. **Make your changes** following the code quality standards below

3. **Test your changes** thoroughly:
   - Run unit tests: `dotnet test Server.Tests`
   - Run component tests: `dotnet test Client.Tests`
   - Run E2E tests: `dotnet test EndToEnd.Tests`

4. **Commit your changes** with clear, descriptive commit messages:
   ```bash
   git add .
   git commit -m "feat: add new feature description"
   ```

5. **Push to your fork**:
   ```bash
   git push origin feature/your-feature-name
   ```

6. **Create a Pull Request** on GitHub

## Continuous Integration

FileHub uses GitHub Actions for automated continuous integration. When you submit a pull request, the CI workflow will automatically:

### Build Process
- ✅ Restore NuGet packages (with caching for performance)
- ✅ Build all projects in Release configuration
- ✅ Enforce code analyzer rules (SonarAnalyzer, Meziantou.Analyzer, AsyncFixer, Roslynator)

### Test Execution
The CI pipeline runs three separate test suites in parallel:

1. **Server Unit Tests** (`Server.Tests`)
   - Framework: TUnit
   - Coverage: Backend logic, handlers, repositories
   - Execution time: ~1-2 minutes

2. **Client Component Tests** (`Client.Tests`)
   - Framework: bUnit
   - Coverage: Blazor components, UI logic
   - Execution time: ~1-2 minutes

3. **End-to-End Tests** (`EndToEnd.Tests`)
   - Framework: Playwright with TUnit
   - Coverage: Full application workflows
   - Execution time: ~3-5 minutes

### Test Results
- Test results are uploaded as artifacts (30-day retention)
- A summary is generated showing pass/fail status
- All tests must pass for the PR to be mergeable

### Viewing CI Results
1. Navigate to your pull request on GitHub
2. Click on the "Checks" tab
3. Review the CI workflow results
4. Download test result artifacts if needed

## Code Quality Standards

FileHub enforces high code quality standards through multiple analyzers:

### Analyzers Enforced
- **SonarAnalyzer.CSharp**: Detects bugs, code smells, and security vulnerabilities
- **Meziantou.Analyzer**: Enforces .NET best practices and performance patterns
- **AsyncFixer**: Ensures proper async/await usage and ConfigureAwait patterns
- **Roslynator.Analyzers**: Provides extensive code analysis and refactoring suggestions
- **BannedApiAnalyzers**: Prevents use of banned APIs

### Coding Standards
- Use **Vertical Slice Architecture (VSA)** for feature organization
- Follow **CQRS pattern** for commands and queries
- Use **MediatR** for request handling
- Implement **Mapperly** for DTO mapping
- Always use `ConfigureAwait(false)` in library code
- Enable and respect nullable reference types
- Write XML documentation for public APIs
- Follow existing naming conventions and code style

### .editorconfig
The project includes an `.editorconfig` file that defines:
- Code formatting rules
- Naming conventions
- Style preferences

Ensure your IDE respects these settings.

## Testing Requirements

### Test Organization
- **Server.Tests**: Unit and integration tests for backend code
- **Client.Tests**: Component tests for Blazor UI
- **EndToEnd.Tests**: E2E tests for complete user workflows

### Writing Tests

#### Test Naming Convention
Use the pattern: `MethodName_Condition_ExpectedResult()`

Example:
```csharp
[Fact(DisplayName = "Create sample module with valid data returns success")]
public async Task CreateSampleModule_ValidData_ReturnsSuccess()
{
    // Arrange
    var handler = CreateHandler();
    var request = CreateValidRequest();

    // Act
    var result = await handler.Handle(request, CancellationToken.None);

    // Assert
    Assert.NotNull(result);
    Assert.True(result.Success);
}
```

#### Test Coverage Requirements
- Minimum 85% code coverage for domain and application layers
- All public APIs must have tests
- Edge cases and error scenarios must be covered
- Integration tests for database operations

#### Running Specific Tests
```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test Server.Tests

# Run tests with detailed output
dotnet test --verbosity detailed

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Playwright E2E Tests
For E2E tests, ensure Playwright browsers are installed:
```bash
pwsh EndToEnd.Tests/bin/Debug/net9.0/playwright.ps1 install
```

## Pull Request Process

### Before Submitting
1. ✅ Ensure all tests pass locally
2. ✅ Fix any analyzer warnings
3. ✅ Update documentation if needed
4. ✅ Add tests for new functionality
5. ✅ Ensure your branch is up to date with `develop`

### PR Description Template
```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
- [ ] Unit tests added/updated
- [ ] Component tests added/updated
- [ ] E2E tests added/updated
- [ ] All tests pass locally

## Checklist
- [ ] Code follows project style guidelines
- [ ] Self-review completed
- [ ] Comments added for complex code
- [ ] Documentation updated
- [ ] No new warnings introduced
```

### Review Process
1. Automated CI checks must pass
2. Code review by at least one maintainer
3. All review comments addressed
4. Final approval from maintainer
5. Merge to `develop` branch

### Merge Strategy
- Feature branches merge to `develop`
- `develop` merges to `main` for releases
- Use squash and merge for feature branches
- Use merge commits for release branches

## Common Issues and Solutions

### Build Failures
**Issue**: NuGet restore fails  
**Solution**: Clear NuGet cache: `dotnet nuget locals all --clear`

**Issue**: Analyzer errors  
**Solution**: Review error messages and fix according to analyzer rules

### Test Failures
**Issue**: Playwright tests fail  
**Solution**: Ensure browsers are installed: `pwsh playwright.ps1 install`

**Issue**: Database tests fail  
**Solution**: Check connection string and database availability

### CI Failures
**Issue**: Tests pass locally but fail in CI  
**Solution**: Check for environment-specific code or timing issues

**Issue**: Timeout in E2E tests  
**Solution**: Increase timeout values or optimize test execution

## Getting Help

- **GitHub Issues**: For bug reports and feature requests
- **GitHub Discussions**: For questions and discussions
- **Pull Request Comments**: For code review discussions

## License

By contributing to FileHub, you agree that your contributions will be licensed under the MIT License.

## Acknowledgments

Thank you for contributing to FileHub! Your efforts help make this project better for everyone.
