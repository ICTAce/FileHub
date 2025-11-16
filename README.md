# FileHub

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
- **Vertical Slice Architecture (VSA)**: Features are organized by business capability rather than technical layers. Each feature slice (Create, Update, Delete, Get, List) contains its own handlers, requests, responses, and mapping logic in a cohesive unit under `Server/Features/MyModules/`.
- **CQRS Pattern**: Clear separation between commands (Create, Update, Delete) and queries (Get, List) using MediatR handlers with dedicated base classes (`CommandHandlerBase`, `QueryHandlerBase`).
- **MediatR**: Implements the mediator pattern for in-process messaging, decoupling request handling across feature slices and enabling clean separation of concerns.

### Key Libraries
- **Mapperly**: Compile-time object mapper providing type-safe, performant DTO mapping between entities and response models without runtime reflection overhead.
- **Entity Framework Core 9.0**: Data access with DbContext factory pattern for efficient database operations.
- **Oqtane Framework**: Built as a native Oqtane CMS module with full framework integration.

### Code Quality
The project enforces high code quality standards with multiple analyzers:
- **SonarAnalyzer.CSharp**: Detects bugs, code smells, and security vulnerabilities
- **Meziantou.Analyzer**: Enforces best practices and performance patterns
- **AsyncFixer**: Ensures proper async/await usage and ConfigureAwait patterns
- **Roslynator.Analyzers**: Provides extensive code analysis and refactoring suggestions

## Project Structure

The solution consists of 5 projects organized in a modular architecture:

### Production Projects
- **ICTAce.FileHub.Server** - ASP.NET Core backend with:
  - Feature-based organization (`Features/MyModules/`)
  - MediatR handlers for CQRS implementation
  - Entity Framework Core entities and migrations
  - RESTful API controllers
  - Oqtane module manager integration

- **ICTAce.FileHub.Client** - Blazor WebAssembly frontend with:
  - Blazor components for UI (`Modules/MyModule/`)
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

