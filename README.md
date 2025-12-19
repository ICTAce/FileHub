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

### Key Libraries
- **Mapperly**: Compile-time object mapper providing type-safe, performant DTO mapping between entities and response models without runtime reflection overhead.
- **Entity Framework Core 9.0**: Data access with DbContext factory pattern for efficient database operations.
- **Oqtane Framework**: Built as a native Oqtane CMS module with full framework integration.

### Code Quality & Security
The project enforces high code quality and security standards with multiple analyzers:
- **SonarCloud**: Continuous code quality and security analysis with detailed metrics and quality gates
- **Snyk**: Automated vulnerability scanning for dependencies and container images
- **SonarAnalyzer.CSharp**: Detects bugs, code smells, and security vulnerabilities
- **Meziantou.Analyzer**: Enforces best practices and performance patterns
- **AsyncFixer**: Ensures proper async/await usage and ConfigureAwait patterns
- **Roslynator.Analyzers**: Provides extensive code analysis and refactoring suggestions
- **GitHub Advanced Security**: Automated security scanning with CodeQL, Dependabot, and secret scanning

## Project Structure

The solution consists of 5 projects organized in a modular architecture:

### Production Projects
- **ICTAce.FileHub.Server** - ASP.NET Core backend with:
  - Feature-based organization (`Features/SampleModules/`)
  - MediatR handlers for CQRS implementation
  - Entity Framework Core entities and migrations
  - RESTful API controllers
  - Oqtane module manager integration

- **ICTAce.FileHub.Client** - Blazor WebAssembly frontend with:
  - Blazor components for UI (`Modules/SampleModule/`)
  - HTTP service layer for API communication
  - MediatR contracts for request/response DTOs
  - Oqtane client integration

### Test Projects
The project uses modern testing frameworks for comprehensive test coverage:

- **ICTAce.FileHub.Server.Tests** - Server-side unit and integration tests
- **ICTAce.FileHub.Client.Tests** - Client-side component tests using bUnit
- **ICTAce.FileHub.EndToEnd.Tests** - End-to-end tests using TUnit.Playwright

#### Testing Stack
- **TUnit**: Modern, fast, and flexible testing framework for .NET with native async support and enhanced performance
- **bUnit**: Testing library for Blazor components, enabling comprehensive UI component testing with rendering and interaction validation
- **Playwright**: Browser automation for end-to-end testing scenarios

## Database Support

FileHub supports multiple database providers through Oqtane's abstraction layer:
- SQL Server (LocalDB & Azure SQL)
- SQLite
- MySQL
- PostgreSQL

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- SQL Server LocalDB (or another supported database)
- Visual Studio 2022 or JetBrains Rider

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

- ✅ Vertical Slice Architecture for maintainable feature development
- ✅ CQRS pattern with MediatR for clean separation of concerns
- ✅ Compile-time mapping with Mapperly for performance
- ✅ Comprehensive code quality enforcement with multiple analyzers
- ✅ Full test coverage with modern testing frameworks (TUnit, bUnit, Playwright)
- ✅ Automated CI/CD with GitHub Actions
- ✅ GitHub Advanced Security for continuous security monitoring
- ✅ Multi-database support through Oqtane framework
- ✅ Entity auditing (CreatedBy, CreatedOn, ModifiedBy, ModifiedOn)
- ✅ Role-based authorization and permissions

## Development

### Building the Solution
```bash
dotnet build
```

### Running Tests
```bash
dotnet test
```

### Code Quality
The project is configured with:
- Nullable reference types enabled
- Analysis level set to latest
- Code style enforcement during build
- Multiple analyzers for code quality and security
- SonarCloud integration for continuous quality monitoring

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

Contributions are welcome! Please ensure:
1. All tests pass
2. Code follows the established VSA pattern
3. Proper use of `ConfigureAwait(false)` in async methods
4. Code analyzer warnings are addressed

## License

This project is licensed under the MIT License - see the repository for details.

## Acknowledgments

Built with:
- [Oqtane Framework](https://www.oqtane.org/) - Modular Application Framework
- [MediatR](https://github.com/jbogard/MediatR) - Simple mediator implementation
- [Mapperly](https://github.com/riok/mapperly) - .NET source generator for object mapping
- [TUnit](https://github.com/thomhurst/TUnit) - Modern .NET testing framework
- [bUnit](https://bunit.dev/) - Testing library for Blazor components

