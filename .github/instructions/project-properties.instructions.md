---
applyTo: '*.csproj, Directory.Build.props, Directory.Build.targets'
description: "Instructions for managing MSBuild project properties and ensuring consistency across the solution."
---

# Project Properties and Directory.Build.props Management

## Instructions

You are responsible for maintaining consistency in MSBuild project properties across the solution. Follow these guidelines to ensure proper configuration management.

### 1. Use Directory.Build.props for Common Properties

**Directory.Build.props is the single source of truth for common project settings.** Always add properties that are shared across multiple projects to `Directory.Build.props` rather than duplicating them in individual `.csproj` files.

#### Properties That MUST Be in Directory.Build.props

The following properties should be defined in `Directory.Build.props` and NOT duplicated in individual project files:

- **Target Framework**: `<TargetFramework>` - Define the .NET version for all projects
- **Language Version**: `<LangVersion>` - C# language version
- **Nullable Reference Types**: `<Nullable>` - Nullable context setting
- **Analysis Level**: `<AnalysisLevel>` - Code analysis level
- **Code Style Enforcement**: `<EnforceCodeStyleInBuild>` - Whether to enforce code style on build
- **Implicit Usings**: `<ImplicitUsings>` - Enable/disable implicit global usings
- **Documentation Generation**: `<GenerateDocumentationFile>` - Generate XML documentation files
- **Warning Treatment**: `<TreatWarningsAsErrors>`, `<WarningsAsErrors>` - How to handle warnings
- **Company/Product Metadata**: `<Company>`, `<Product>`, `<Copyright>` - Organization metadata
- **Repository Information**: `<RepositoryUrl>`, `<RepositoryType>` - Source control metadata
- **Package Settings**: `<GeneratePackageOnBuild>`, `<IncludeSymbols>`, `<SymbolPackageFormat>` - NuGet package settings
- **Version Numbers**: `<Version>`, `<AspNetCoreVersion>`, `<OqtaneVersion>` - Version management

#### Properties That Should Be in Individual .csproj Files

Only project-specific properties should remain in individual `.csproj` files:

- **Assembly/Namespace Names**: `<AssemblyName>`, `<RootNamespace>` - When they differ from project name
- **Output Type**: `<OutputType>` - Only if different from Directory.Build.props conditional defaults
- **Project-Specific Configurations**: Properties like `<BlazorWebAssemblyLoadAllGlobalizationData>`, `<StaticWebAssetProjectMode>` that only apply to specific project types
- **User Secrets**: `<UserSecretsId>` - Project-specific secret storage identifier

### 2. Conditional Properties for Project Types

Use MSBuild conditions in `Directory.Build.props` to apply properties to specific project types:

```xml
<!-- Example: Settings for all test projects -->
<PropertyGroup Condition="$(MSBuildProjectName.EndsWith('.Tests'))">
  <IsPackable>false</IsPackable>
  <OutputType>Exe</OutputType>
</PropertyGroup>

<!-- Example: Settings for server projects -->
<PropertyGroup Condition="$(MSBuildProjectName.Contains('Server'))">
  <PreserveCompilationContext>true</PreserveCompilationContext>
</PropertyGroup>
```

### 3. Global Package References

Define analyzer packages and other globally-used NuGet packages in `Directory.Build.props`:

```xml
<ItemGroup>
  <PackageReference Include="SonarAnalyzer.CSharp">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
  </PackageReference>
</ItemGroup>
```

Use conditional `ItemGroup` elements for packages that only apply to certain project types:

```xml
<ItemGroup Condition="$(MSBuildProjectName.EndsWith('.Tests'))">
  <PackageReference Include="TUnit.Analyzers">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
  </PackageReference>
</ItemGroup>
```

### 4. When Reviewing or Creating Project Files

**Before adding a property to a .csproj file, ask yourself:**

1. **Is this property the same across all projects?** → Add it to `Directory.Build.props`
2. **Is this property the same for a subset of projects (e.g., all tests)?** → Add it to `Directory.Build.props` with a condition
3. **Is this property unique to this specific project?** → Keep it in the `.csproj` file

**When you see duplicated properties across multiple .csproj files:**

1. Extract the common property to `Directory.Build.props`
2. Remove it from all individual `.csproj` files
3. If the property has different values in different projects, consider if it should be conditionally set or remain project-specific

### 5. Cleaning Up Individual Project Files

Individual `.csproj` files should be minimal and contain only:

- **SDK declaration**: `<Project Sdk="...">`
- **Project-specific PropertyGroup elements** (if any)
- **PackageReference items** (project-specific dependencies)
- **ProjectReference items** (references to other projects in the solution)
- **ItemGroup elements** for project-specific files or resources

**Example of a clean .csproj file:**

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <!-- Only project-specific properties here -->
    <AssemblyName>ICTAce.FileHub.CustomName</AssemblyName>
  </PropertyGroup>

  <ItemGroup>
    <!-- Project-specific package references -->
    <PackageReference Include="SpecificLibrary" />
  </ItemGroup>

  <ItemGroup>
    <!-- Project references -->
    <ProjectReference Include="..\OtherProject\OtherProject.csproj" />
  </ItemGroup>

</Project>
```

### 6. Version Management Strategy

All version numbers should be centralized in `Directory.Build.props`:

- Use `<Version>` for the solution version
- Use custom properties like `<AspNetCoreVersion>` and `<OqtaneVersion>` for framework versions
- Reference these properties in PackageReference version attributes: `Version="$(OqtaneVersion)"`

### 7. Enforcement

When generating or modifying project files:

1. **Always check `Directory.Build.props` first** to see what properties are already defined globally
2. **Never duplicate** a property that exists in `Directory.Build.props`
3. **Suggest moving common properties** to `Directory.Build.props` when you find duplication
4. **Keep .csproj files minimal** and focused on project-specific configurations

## Examples

### ❌ INCORRECT - Property duplication

```xml
<!-- In multiple .csproj files -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
  </PropertyGroup>
</Project>
```

### ✅ CORRECT - Properties in Directory.Build.props

```xml
<!-- In Directory.Build.props -->
<Project>
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
  </PropertyGroup>
</Project>

<!-- In individual .csproj files -->
<Project Sdk="Microsoft.NET.Sdk">
  <!-- Only project-specific properties and references -->
</Project>
```

## Summary

- **Centralize common properties** in `Directory.Build.props`
- **Use conditional properties** for project-type-specific settings
- **Keep .csproj files minimal** with only project-specific configurations
- **Avoid property duplication** across project files
- **Review and refactor** when you find duplicated properties
