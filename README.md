# FileHub

[![CI](https://github.com/ICTAce/FileHub/actions/workflows/ci.yml/badge.svg)](https://github.com/ICTAce/FileHub/actions/workflows/ci.yml)
[![CodeQL](https://github.com/ICTAce/FileHub/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/ICTAce/FileHub/actions/workflows/codeql-analysis.yml)
[![Known Vulnerabilities](https://snyk.io/test/github/ICTAce/FileHub/badge.svg)](https://snyk.io/test/github/ICTAce/FileHub)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=ICTAce_FileHub&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=ICTAce_FileHub)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=ICTAce_FileHub&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=ICTAce_FileHub)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=ICTAce_FileHub&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=ICTAce_FileHub)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=ICTAce_FileHub&metric=coverage)](https://sonarcloud.io/summary/new_code?id=ICTAce_FileHub)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=ICTAce_FileHub&metric=bugs)](https://sonarcloud.io/summary/new_code?id=ICTAce_FileHub)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=ICTAce_FileHub&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=ICTAce_FileHub)

A modular file management system built as an **Oqtane CMS module** using modern .NET architecture patterns and best practices.

## Project Information

- **Version:** 1.0.0
- **Target Framework:** .NET 9.0
- **C# Version:** 13.0
- **Platform:** Oqtane 6.2.1
- **License:** MIT
- **Repository:** [https://github.com/ICTAce/FileHub](https://github.com/ICTAce/FileHub)

## Architecture and Development Tools

This project implements a modern, maintainable architecture using:

### Core Architecture
- **Vertical Slice Architecture (VSA)**: Features are organized by business capability rather than technical layers. Each feature slice (Create, Update, Delete, Get, List) contains its own handlers, requests, responses, and mapping logic in a cohesive unit under `Server/Features/SampleModule/`.
- **CQRS Pattern**: Clear separation between commands (Create, Update, Delete) and queries (Get, List) using MediatR handlers with dedicated base classes (`CommandHandlerBase`, `QueryHandlerBase`).
- **MediatR**: Implements the mediator pattern for in-process messaging, decoupling request handling across feature slices and enabling clean separation of concerns.

### Key Libraries & Frameworks

**Core Framework:**
- **Oqtane 6.2.1**: Modular application framework providing CMS capabilities, multi-tenancy, and module infrastructure
- **.NET 9.0 SDK**: Latest .NET platform with C# 13 language features

**Backend:**
- **MediatR 14.0.0**: Mediator pattern implementation for CQRS with in-process messaging
- **Mapperly 4.3.0**: Source generator for compile-time object mapping (zero-reflection, high-performance)
- **Entity Framework Core 9.0**: Modern ORM with DbContext factory pattern and advanced querying
- **ASP.NET Core Identity**: Authentication and authorization infrastructure

**Frontend:**
- **Blazor WebAssembly**: Client-side SPA framework with full .NET runtime in the browser
- **Radzen.Blazor 8.4.1**: Professional UI component library with data grids, charts, and forms
- **Microsoft.AspNetCore.Components.WebAssembly.Authentication**: Built-in auth support for SPAs

**Testing:**
- **TUnit 1.5.70**: Modern testing framework with native async/await and performance optimizations
- **bUnit 2.3.4**: Testing library specifically designed for Blazor components
- **Playwright 1.57.0**: Browser automation for cross-browser end-to-end testing
- **NSubstitute 5.3.0**: Friendly mocking library for .NET tests
- **Bogus 35.6.1**: Fake data generator for realistic test scenarios

**Development Tools:**
- **User Secrets**: Secure local development credential storage
- **EditorConfig**: Consistent coding style across IDEs
- **Central Package Management**: Unified version control via Directory.Packages.props

### Code Quality & Security

The project implements defense-in-depth with multiple layers of automated analysis:

**Static Code Analyzers (Build-Time):**
- **SonarAnalyzer.CSharp 10.17.0**: 5000+ rules for bugs, vulnerabilities, and code smells
- **Meziantou.Analyzer 2.0.263**: Best practices and performance optimization rules
- **AsyncFixer 1.6.0**: Async/await pattern enforcement and deadlock prevention
- **Roslynator.Analyzers 4.15.0**: 500+ analyzers and refactorings for C# code
- **TUnit.Analyzers 0.1.984**: Test-specific rules for proper test authoring
- **Microsoft.CodeAnalysis.BannedApiAnalyzers 4.14.0**: Prevents usage of banned APIs (Moq, AutoFixture)

**Continuous Security Monitoring:**
- **SonarCloud**: Cloud-based code quality platform with quality gates, technical debt tracking, and security hotspots
- **GitHub Advanced Security**: 
  - CodeQL for semantic code analysis (C# & JavaScript)
  - Dependabot for automated dependency updates
  - Secret scanning for credential leak prevention
- **Snyk**: Real-time vulnerability detection for NuGet/npm packages and container images

**Code Quality Configuration:**
- **Nullable Reference Types**: Enabled project-wide to prevent null reference exceptions
- **Treat Warnings as Errors**: Configurable per project for strict quality control
- **Analysis Level**: Set to 'latest' for newest compiler warnings and suggestions
- **EnforceCodeStyleInBuild**: Automatic code style validation during compilation
- **EditorConfig**: 200+ style rules enforced across all files
- **BannedSymbols.txt**: Explicit API bans with justifications (security and maintainability)

## Project Structure

The solution consists of 5 projects organized in a modular architecture:

### Production Projects

#### **ICTAce.FileHub.Server** - ASP.NET Core Backend
A feature-rich server application built with modern .NET patterns:

**Architecture & Patterns:**
- Feature-based organization using Vertical Slice Architecture (`Features/`)
- CQRS implementation with MediatR handlers
- DbContext Factory pattern for efficient database operations
- Separated read/write contexts (Command & Query)
- Generic base handlers with dependency injection
- RESTful API controllers with proper HTTP semantics

**Data Management:**
- Entity Framework Core 9.0 with code-first migrations
- Auditable entity base classes (CreatedBy, CreatedOn, ModifiedBy, ModifiedOn)
- Multi-database support (SQL Server, SQLite, MySQL, PostgreSQL)
- Fluent API entity configuration with builders
- Automatic audit trail tracking

**Key Features:**
- User secrets management for development
- Oqtane module manager integration
- Permission-based authorization
- Tenant isolation support
- Comprehensive logging with ILogManager

#### **ICTAce.FileHub.Client** - Blazor WebAssembly Frontend
Modern, interactive web UI built with Blazor WebAssembly:

**UI Components:**
- Blazor components with code-behind pattern (`.razor.cs`)
- Radzen.Blazor UI component library integration
- Responsive design with custom CSS modules
- JavaScript interop for enhanced functionality

**Architecture:**
- Service-oriented architecture with typed HTTP clients
- Generic base service classes for common operations
- Strongly-typed DTOs for API communication
- PagedResult pattern for efficient data loading
- Module-based organization aligning with Oqtane

**Features:**
- Localization support with .resx resource files
- Client-side validation with data annotations
- Oqtane framework integration (navigation, logging, state management)
- Static web asset management

### Test Projects

The project implements comprehensive testing across all layers:

#### **ICTAce.FileHub.Server.Tests** - Server Unit & Integration Tests
- **Framework**: TUnit with modern async support
- **Database**: SQLite in-memory for fast, isolated tests
- **Mocking**: NSubstitute for clean, type-safe mocks
- **Test Data**: Bogus for realistic fake data generation
- **Patterns**: HandlerTestBase for consistent test setup
- **Coverage**: Feature handlers, pagination, entity mappings

#### **ICTAce.FileHub.Client.Tests** - Client Component Tests
- **Framework**: bUnit for Blazor component testing
- **Features**: Component rendering, interaction testing, service mocking
- **Patterns**: Mock services (MockLogService) for isolation
- **Coverage**: Blazor components, user interactions, state management

#### **ICTAce.FileHub.EndToEnd.Tests** - Browser Automation Tests
- **Framework**: TUnit.Playwright for end-to-end testing
- **Capabilities**: Cross-browser testing, user workflow validation
- **Features**: Full application testing in real browser environments

## Architecture Deep Dive

### Vertical Slice Architecture (VSA)

FileHub organizes code by feature rather than technical layers. Each feature slice contains:

```
Server/Features/
├── Categories/
│   ├── Create.cs         # Command: Create new category
│   ├── Update.cs         # Command: Update existing category
│   ├── Delete.cs         # Command: Delete category
│   ├── Get.cs           # Query: Get single category
│   ├── List.cs          # Query: List categories with pagination
│   ├── MoveUp.cs        # Command: Reorder category up
│   ├── MoveDown.cs      # Command: Reorder category down
│   └── Controller.cs    # API endpoints for category operations
└── _Common/
    ├── HandlerBase.cs           # Base class for all handlers
    ├── RequestBase.cs           # Base request classes
    ├── PagedRequestBase.cs      # Pagination support
    └── HandlerServices.cs       # Service injection pattern
```

**Benefits:**
- High cohesion: related code stays together
- Easy to find: feature location is predictable
- Simple to test: each slice is independently testable
- Scalable: add new features without touching existing code

### CQRS with MediatR

Commands and queries are separated for clarity and optimization:

**Commands** (Write operations):
- Modify application state
- Use `ApplicationCommandContext` (write-optimized)
- Return simple results (id, success/failure)
- Examples: Create, Update, Delete

**Queries** (Read operations):
- Never modify state
- Use `ApplicationQueryContext` (read-optimized)
- Return DTOs optimized for display
- Support pagination and filtering
- Examples: Get, List

### Database Context Pattern

Three specialized DbContext implementations:

1. **ApplicationContext**: Base context with common configuration
2. **ApplicationCommandContext**: Optimized for write operations
3. **ApplicationQueryContext**: Optimized for read operations with `.AsNoTracking()`

**DbContext Factory Pattern:**
- Thread-safe context creation
- Proper disposal management
- Supports high-concurrency scenarios
- Enables unit testing with in-memory databases

### Entity Auditing

All entities inherit from `AuditableBase` or `AuditableModuleBase`:

```csharp
public abstract class AuditableBase
{
    public int Id { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime ModifiedOn { get; set; }
}
```

Audit fields are automatically populated in handlers using Oqtane's user context.

### Client-Side Architecture

**Service Pattern:**
- Generic `ModuleService<TGet, TList, TCreate>` base class
- Typed HTTP client injection
- Automatic URL construction
- PagedResult support for lists
- Consistent error handling

**Component Pattern:**
- Code-behind with `.razor.cs` files
- Dependency injection via `[Inject]` attributes
- Proper use of `ConfigureAwait(true)` in UI code
- Localization with `IStringLocalizer<T>`

## Database Support

FileHub supports multiple database providers through Oqtane's abstraction layer:
- **SQL Server** (LocalDB for development, Azure SQL for production)
- **SQLite** (lightweight, file-based, perfect for testing)
- **MySQL** (open-source, cross-platform)
- **PostgreSQL** (advanced features, high performance)

**Migration Strategy:**
- Code-first migrations in `Server/Persistence/Migrations/`
- Entity builders for Fluent API configuration
- Initial data seeding support
- Automatic migration application on startup

## Localization & Internationalization

FileHub is built with internationalization in mind:

**Resource Files (.resx):**
- Located in `Client/Resources/`
- Organized by module and component
- Examples: `Index.resx`, `Edit.resx`, `Settings.resx`
- Support for multiple languages (add `Index.es.resx` for Spanish, etc.)

**Usage in Components:**
```csharp
[Inject] protected IStringLocalizer<Index> Localizer { get; set; } = default!;

// In code:
AddModuleMessage(Localizer["Message.LoadError"], MessageType.Error);
```

**Best Practices:**
- All user-facing strings should be in resource files
- Use descriptive keys: `Message.LoadError`, `Button.Save`, `Label.Name`
- Provide context in resource comments
- Keep error messages consistent across modules

## Getting Started

### Prerequisites
- **.NET 9.0 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **SQL Server LocalDB** (or another supported database) - Included with Visual Studio
- **IDE**: Visual Studio 2022 17.8+ or JetBrains Rider 2024.1+
- **Optional**: Docker (for containerized database testing)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/ICTAce/FileHub.git
   cd FileHub
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Run the application:
   ```bash
   dotnet run --project Server
   ```

4. Navigate to `https://localhost:5001` and complete the Oqtane installation wizard.

## Login Credentials

Default credentials for development:

**Username:** `webmaster`  
**Password:** `iBrWMLZg@#nR0P%DAUwyF`

> ⚠️ **Security Warning:** Change these credentials immediately in production environments.

## Features

**Architecture & Design Patterns:**
- ✅ Vertical Slice Architecture (VSA) - features organized by business capability
- ✅ CQRS pattern with MediatR for clean command/query separation
- ✅ DbContext Factory pattern for efficient multi-threaded database access
- ✅ Repository pattern abstraction through EF Core
- ✅ Service layer pattern for client-side API communication
- ✅ Generic base classes for reducing boilerplate code

**Performance & Optimization:**
- ✅ Compile-time object mapping with Mapperly (zero reflection overhead)
- ✅ Source generators for build-time code generation
- ✅ Pagination support for efficient data loading
- ✅ Separated read/write database contexts for optimization
- ✅ NuGet package caching in CI/CD pipeline
- ✅ Central Package Management for faster restores

**Code Quality & Security:**
- ✅ 6 static analyzers enforcing 6000+ rules at build time
- ✅ SonarCloud integration with quality gate enforcement
- ✅ GitHub Advanced Security (CodeQL, Dependabot, Secret Scanning)
- ✅ Snyk vulnerability monitoring for dependencies
- ✅ BannedSymbols enforcement preventing insecure API usage
- ✅ EditorConfig for consistent coding standards
- ✅ Nullable reference types enabled project-wide

**Testing & Quality Assurance:**
- ✅ Comprehensive test coverage across all layers (unit, integration, E2E)
- ✅ Modern testing with TUnit framework
- ✅ Blazor component testing with bUnit
- ✅ Browser automation with Playwright
- ✅ NSubstitute for clean mocking
- ✅ Bogus for realistic test data generation
- ✅ Automated test execution in CI/CD pipeline

**Data & Persistence:**
- ✅ Multi-database support (SQL Server, SQLite, MySQL, PostgreSQL)
- ✅ Entity Framework Core 9.0 with code-first migrations
- ✅ Automatic audit trail (CreatedBy, CreatedOn, ModifiedBy, ModifiedOn)
- ✅ Fluent API entity configuration
- ✅ Database seeding for initial data

**Frontend & User Experience:**
- ✅ Blazor WebAssembly for rich client-side interactions
- ✅ Radzen UI components for professional data grids and forms
- ✅ Localization support with resource files (.resx)
- ✅ Responsive design with custom CSS modules
- ✅ JavaScript interop for advanced browser features
- ✅ Client-side validation with data annotations

**DevOps & Automation:**
- ✅ Automated CI/CD with GitHub Actions
- ✅ Parallel test execution for faster feedback
- ✅ Test result artifacts with 30-day retention
- ✅ Automated security scanning on every commit
- ✅ Dependency update automation with Dependabot
- ✅ Workflow concurrency control to prevent conflicts

**Oqtane Integration:**
- ✅ Native Oqtane module with full framework support
- ✅ Multi-tenancy with tenant isolation
- ✅ Role-based authorization and permissions
- ✅ Module settings and configuration UI
- ✅ Integration with Oqtane's logging infrastructure
- ✅ Support for Oqtane's localization system

## Development

### Solution Configuration

The solution uses modern .NET configuration files for centralized management:

**Directory.Build.props:**
- Target framework: .NET 9.0
- C# language version: 13
- Nullable reference types: Enabled
- Implicit usings: Enabled
- XML documentation generation: Enabled
- Common project metadata (company, product, copyright)
- Global analyzer references for all projects
- Shared test project configuration

**Directory.Packages.props:**
- Central Package Management (CPM) enabled
- Unified version management across all projects
- 30+ package references with pinned versions
- Version ranges for framework dependencies

**global.json:**
- .NET SDK version: 9.0.307
- Roll-forward policy: latestFeature

### Building the Solution

```bash
# Build all projects
dotnet build ICTAce.FileHub.slnx

# Build with specific configuration
dotnet build ICTAce.FileHub.slnx --configuration Release

# Clean and rebuild
dotnet clean && dotnet build
```

### Running Tests

```bash
# Run all tests
dotnet test ICTAce.FileHub.slnx

# Run specific test project
dotnet test Server.Tests/ICTAce.FileHub.Server.Tests.csproj
dotnet test Client.Tests/ICTAce.FileHub.Client.Tests.csproj
dotnet test EndToEnd.Tests/ICTAce.FileHub.EndToEnd.Tests.csproj

# Run with detailed output
dotnet test --verbosity detailed

# Generate test results file
dotnet test --logger "trx;LogFileName=test-results.trx"
```

### Code Quality Tools

**Local Development:**
```bash
# Restore dependencies
dotnet restore

# Check for code style violations
dotnet build --no-restore

# View analyzer warnings
dotnet build -warnaserror
```

**Banned API Analysis:**
The project uses `BannedSymbols.txt` to prevent usage of:
- **Moq**: Banned due to security concerns and licensing issues (use NSubstitute)
- **AutoFixture**: Banned for test clarity (use explicit test data or Bogus)

**Editor Configuration:**
- `.editorconfig` enforces consistent style across IDEs
- 200+ rules for formatting, naming, and code style
- Automatically applied in Visual Studio, Rider, and VS Code

### Continuous Integration
The project uses GitHub Actions for automated CI/CD:

**Workflow Features:**
- ✅ Automated builds on pull requests and commits to main/develop
- ✅ Comprehensive test execution across all test projects
- ✅ Code quality enforcement with multiple analyzers
- ✅ Parallel test execution for faster feedback
- ✅ Test result artifacts with 30-day retention
- ✅ Automated test summaries in workflow runs

**Test Coverage:**
- **Server Tests**: Unit and integration tests using TUnit
- **Client Tests**: Blazor component tests using bUnit
- **E2E Tests**: End-to-end browser automation using Playwright

**Code Quality Analysis:**
- **SonarCloud**: Automated code quality and security analysis
  - Quality gate enforcement on pull requests
  - Code coverage tracking and reporting
  - Technical debt measurement
  - Security hotspot detection
  - Continuous monitoring of code smells and bugs

**Security & Dependency Scanning:**
- **Snyk**: Continuous vulnerability monitoring
  - Automated dependency vulnerability detection
  - License compliance checks
  - Container security scanning
  - Actionable remediation advice

The CI workflow ensures code quality and prevents regressions before merging to main branches. View detailed quality metrics on [SonarCloud](https://sonarcloud.io/project/overview?id=ICTAce_FileHub) and security insights on [Snyk](https://snyk.io/test/github/ICTAce/FileHub).

## Security

FileHub implements comprehensive security measures using **GitHub Advanced Security** to protect the codebase:

### Automated Security Scanning
- **CodeQL Analysis**: Continuous code scanning for security vulnerabilities and coding errors
  - Scans C# and JavaScript code on every push and pull request
  - Weekly scheduled scans for comprehensive coverage
  - Security-extended query suite for deeper analysis

- **Snyk**: Comprehensive vulnerability and license compliance scanning
  - Real-time monitoring of NuGet and npm dependencies
  - Automated pull requests for security patches
  - Container image scanning for vulnerabilities
  - License policy enforcement
  - Integration with GitHub for seamless security workflows

- **Dependabot**: Automated dependency management and vulnerability detection
  - Monitors NuGet packages for known vulnerabilities
  - Tracks npm dependencies for security issues
  - Automatic pull requests for security updates
  - Weekly dependency version checks

- **Secret Scanning**: Prevents accidental exposure of sensitive information
  - Detects API keys, tokens, and credentials in commits
  - Alerts on potential secret leaks before they reach production

### Security Best Practices
- OWASP Top 10 compliance guidelines enforced
- SonarCloud security analysis for vulnerability detection and security hotspots
- Secure coding standards validated by multiple analyzers
- Role-based access control and authorization
- Regular security updates through automated workflows

### Reporting Vulnerabilities
For security concerns, please review our [Security Policy](SECURITY.md) for responsible disclosure guidelines.

## Contributing

Contributions are welcome! Please follow these guidelines:

### Before You Start
1. Check existing issues or create a new one to discuss your idea
2. Fork the repository and create a feature branch
3. Ensure you have the latest .NET 9.0 SDK installed

### Development Guidelines

**Code Organization:**
- Follow Vertical Slice Architecture - add new features as complete slices
- Place commands and queries in appropriate feature folders
- Use existing base classes (`HandlerBase`, `ModuleService`) for consistency

**Code Quality:**
- All tests must pass (`dotnet test`)
- No analyzer warnings (`dotnet build` should be clean)
- Follow existing code style (enforced by `.editorconfig`)
- Add XML documentation comments for public APIs
- Keep methods focused and small (< 50 lines preferred)

**Async Patterns:**
- Use `ConfigureAwait(false)` in server/library code
- Use `ConfigureAwait(true)` in UI components (Blazor requirement)
- Never use `.Result` or `.Wait()` - always `await`
- Accept `CancellationToken` parameters for long-running operations

**Testing Requirements:**
- Add unit tests for new handlers in `Server.Tests`
- Add component tests for new Blazor components in `Client.Tests`
- Use NSubstitute for mocking (Moq is banned)
- Use Bogus for test data generation (AutoFixture is banned)
- Ensure tests are isolated and can run in parallel

**Security:**
- Never commit secrets or connection strings
- Use User Secrets for local development
- Validate all user inputs
- Follow OWASP Top 10 guidelines
- Check BannedSymbols.txt for forbidden APIs

**Pull Request Process:**
1. Update README if you're adding new features or dependencies
2. Ensure CI/CD pipeline passes (all checks must be green)
3. Add/update tests to maintain code coverage
4. Update documentation and resource files for UI changes
5. Respond to code review feedback promptly

### Commit Message Convention
```
<type>(<scope>): <subject>

Examples:
feat(categories): add move up/down functionality
fix(auth): resolve token expiration issue
docs(readme): update architecture section
test(handlers): add pagination edge cases
chore(deps): update Mapperly to 4.3.0
```

### Local Development Setup
```bash
# Clone and restore
git clone https://github.com/ICTAce/FileHub.git
cd FileHub
dotnet restore

# Setup user secrets (optional)
dotnet user-secrets init --project Server
dotnet user-secrets set "ConnectionString" "your-connection-string" --project Server

# Build and test
dotnet build
dotnet test

# Run the application
dotnet run --project Server
```

## License

This project is licensed under the MIT License - see the repository for details.

## Technology Stack Summary

| Category | Technologies |
|----------|-------------|
| **Framework** | .NET 9.0, C# 13, Blazor WebAssembly |
| **CMS Platform** | Oqtane 6.2.1 |
| **Architecture** | Vertical Slice Architecture, CQRS, MediatR |
| **Data Access** | Entity Framework Core 9.0, DbContext Factory |
| **Mapping** | Mapperly (source generator) |
| **UI Components** | Radzen.Blazor 8.4.1 |
| **Testing** | TUnit 1.5.70, bUnit 2.3.4, Playwright 1.57.0 |
| **Mocking** | NSubstitute 5.3.0 |
| **Test Data** | Bogus 35.6.1 |
| **Analyzers** | SonarAnalyzer, Meziantou, AsyncFixer, Roslynator, BannedApiAnalyzers |
| **Security** | GitHub Advanced Security, SonarCloud, Snyk |
| **CI/CD** | GitHub Actions |
| **Databases** | SQL Server, SQLite, MySQL, PostgreSQL |
| **Configuration** | Central Package Management, User Secrets, EditorConfig |

## Acknowledgments

Built with these excellent open-source projects:

**Core Framework:**
- [Oqtane Framework](https://www.oqtane.org/) - Modular Application Framework for Blazor
- [.NET](https://dotnet.microsoft.com/) - Free, open-source, cross-platform framework

**Architecture & Patterns:**
- [MediatR](https://github.com/jbogard/MediatR) - Simple mediator implementation in .NET
- [Mapperly](https://github.com/riok/mapperly) - Source generator for object-to-object mapping

**Testing:**
- [TUnit](https://github.com/thomhurst/TUnit) - Modern, fast, and flexible testing framework
- [bUnit](https://bunit.dev/) - Testing library for Blazor components
- [Playwright](https://playwright.dev/) - Cross-browser automation framework
- [NSubstitute](https://nsubstitute.github.io/) - Friendly mocking framework
- [Bogus](https://github.com/bchavez/Bogus) - Fake data generator

**UI & Components:**
- [Radzen Blazor](https://blazor.radzen.com/) - Professional Blazor UI components

**Code Quality:**
- [SonarAnalyzer](https://github.com/SonarSource/sonar-dotnet) - Static code analysis
- [Meziantou.Analyzer](https://github.com/meziantou/Meziantou.Analyzer) - Best practices analyzer
- [AsyncFixer](https://github.com/semihokur/AsyncFixer) - Async/await analyzer
- [Roslynator](https://github.com/dotnet/roslynator) - Collection of analyzers and refactorings

