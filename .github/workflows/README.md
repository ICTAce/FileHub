# GitHub Actions Workflows

This directory contains automated CI/CD workflows for the FileHub project.

## CI Workflow (`ci.yml`)

The main continuous integration workflow that runs on every pull request and push to main/develop branches.

### Workflow Structure

```
┌─────────────────────────────────────────────────────────┐
│                    CI Workflow Trigger                   │
│  (Push/PR to main/develop or manual dispatch)           │
└─────────────────────────────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────┐
│                   Build and Analyze                      │
│  • Checkout code                                         │
│  • Setup .NET 9 SDK                                      │
│  • Cache NuGet packages                                  │
│  • Restore dependencies                                  │
│  • Build all projects (Release config)                   │
│  • Verify analyzer enforcement                           │
│    - SonarAnalyzer.CSharp                               │
│    - Meziantou.Analyzer                                 │
│    - AsyncFixer                                         │
│    - Roslynator.Analyzers                               │
└─────────────────────────────────────────────────────────┘
                           │
        ┌──────────────────┼──────────────────┐
        │                  │                  │
        ▼                  ▼                  ▼
┌──────────────┐  ┌──────────────┐  ┌──────────────┐
│  Server      │  │   Client     │  │     E2E      │
│  Unit Tests  │  │  Component   │  │    Tests     │
│  (TUnit)     │  │  Tests       │  │  (Playwright)│
│              │  │  (bUnit)     │  │              │
│ • Restore    │  │ • Restore    │  │ • Restore    │
│ • Build      │  │ • Build      │  │ • Build      │
│ • Run tests  │  │ • Run tests  │  │ • Install    │
│ • Upload TRX │  │ • Upload TRX │  │   browsers   │
│              │  │              │  │ • Run tests  │
│              │  │              │  │ • Upload TRX │
│              │  │              │  │ • Upload     │
│              │  │              │  │   screenshots│
└──────────────┘  └──────────────┘  └──────────────┘
        │                  │                  │
        └──────────────────┼──────────────────┘
                           ▼
        ┌─────────────────────────────────────┐
        │        Test Summary Job             │
        │  • Aggregate results                │
        │  • Generate summary                 │
        │  • Report pass/fail status          │
        │  • Fail if any test job failed      │
        └─────────────────────────────────────┘
```

### Job Details

#### 1. Build and Analyze
- **Purpose**: Build the entire solution and verify code quality
- **Duration**: ~2-3 minutes
- **Runner**: ubuntu-latest
- **Dependencies**: None
- **Artifacts**: None

**Steps:**
1. Checkout code with full history
2. Setup .NET 9.0.x SDK
3. Cache NuGet packages for faster subsequent runs
4. Restore dependencies from solution file
5. Build all projects in Release configuration
6. Verify analyzers are enforced

#### 2. Server Unit Tests (test-server)
- **Purpose**: Run backend unit and integration tests
- **Duration**: ~1-2 minutes
- **Runner**: ubuntu-latest
- **Dependencies**: build job
- **Framework**: TUnit

**Steps:**
1. Checkout code
2. Setup .NET SDK
3. Restore from cache
4. Restore test project dependencies
5. Build test project
6. Execute tests with TRX logger
7. Upload test results (30-day retention)

**Test Coverage:**
- MediatR handlers
- Repository operations
- Business logic
- Entity mappings

#### 3. Client Component Tests (test-client)
- **Purpose**: Run Blazor component tests
- **Duration**: ~1-2 minutes
- **Runner**: ubuntu-latest
- **Dependencies**: build job
- **Framework**: bUnit

**Steps:**
1. Checkout code
2. Setup .NET SDK
3. Restore from cache
4. Restore test project dependencies
5. Build test project
6. Execute component tests with TRX logger
7. Upload test results (30-day retention)

**Test Coverage:**
- Blazor component rendering
- Component interactions
- UI state management
- Event handling

#### 4. End-to-End Tests (test-e2e)
- **Purpose**: Run browser automation tests
- **Duration**: ~3-5 minutes
- **Runner**: ubuntu-latest
- **Dependencies**: build job
- **Framework**: Playwright + TUnit

**Steps:**
1. Checkout code
2. Setup .NET SDK
3. Restore from cache
4. Restore test project dependencies
5. Build test project
6. Install Chromium browser with dependencies
7. Execute E2E tests with TRX logger
8. Upload test results (30-day retention)
9. Upload screenshots on failure (7-day retention)

**Test Coverage:**
- Complete user workflows
- Cross-browser compatibility
- Application health checks
- Integration with Oqtane

#### 5. Test Summary (test-summary)
- **Purpose**: Aggregate and report test results
- **Duration**: < 1 minute
- **Runner**: ubuntu-latest
- **Dependencies**: All test jobs
- **Runs**: Always (even if tests fail)

**Outputs:**
- Summary in GitHub Actions UI
- Pass/fail status for each test suite
- Code quality checks confirmation
- Overall workflow result

### Triggers

- **Push**: Commits to `main` or `develop` branches
- **Pull Request**: PRs targeting `main` or `develop` branches
- **Manual**: Via workflow_dispatch (Actions tab)

### Concurrency

- Group: `${{ github.workflow }}-${{ github.ref }}`
- Cancel in progress: Yes
- **Benefit**: Prevents resource waste and provides faster feedback

### Permissions

Following the principle of least privilege:
- `contents: read` - Read repository content
- `checks: write` - Write check run results
- `pull-requests: write` - Comment on PRs with results

### Environment Variables

- `DOTNET_VERSION`: 9.0.x
- `DOTNET_SKIP_FIRST_TIME_EXPERIENCE`: true
- `DOTNET_CLI_TELEMETRY_OPTOUT`: true

### Artifacts

| Artifact | Retention | When Created |
|----------|-----------|--------------|
| server-test-results | 30 days | Always after server tests |
| client-test-results | 30 days | Always after client tests |
| e2e-test-results | 30 days | Always after E2E tests |
| playwright-screenshots | 7 days | Only on E2E test failure |

### Performance Optimizations

1. **NuGet Caching**: Caches `~/.nuget/packages` based on package files hash
2. **Parallel Execution**: Test jobs run in parallel after build
3. **Concurrency Control**: Cancels outdated runs on new pushes
4. **Artifact Retention**: Shorter retention for screenshots (7 days vs 30 days)

### Best Practices Implemented

✅ **Security**
- Read-only permissions by default
- Explicit permission grants
- No secrets in logs

✅ **Performance**
- Package caching
- Parallel test execution
- Efficient artifact storage

✅ **Reliability**
- Always upload test results (even on failure)
- Continue on error for test jobs
- Fail fast for critical issues

✅ **Maintainability**
- Clear job names and descriptions
- Consistent step naming
- Comprehensive documentation

✅ **Visibility**
- Test summaries in GitHub UI
- Artifact uploads for investigation
- CI badge in README

## Adding New Workflows

When adding new workflows:

1. Create a new `.yml` file in this directory
2. Follow the naming convention: `<purpose>.yml`
3. Include proper documentation in this README
4. Set appropriate permissions
5. Add caching where applicable
6. Test locally before committing

## Troubleshooting

### Workflow Not Triggering
- Check branch names in triggers
- Verify workflow file syntax (use YAML validator)
- Ensure file is in `.github/workflows/` directory

### Build Failures
- Check analyzer errors in build logs
- Review dependency versions
- Verify .NET SDK version

### Test Failures
- Download test result artifacts
- Check for environment-specific issues
- Review screenshots for E2E failures
- Verify Playwright browser installation

### Cache Issues
- Clear cache from Actions settings
- Update cache key if dependencies changed
- Check cache size limits

## Related Documentation

- [CONTRIBUTING.md](../../CONTRIBUTING.md) - Contribution guidelines
- [README.md](../../README.md) - Project documentation
- [GitHub Actions Best Practices](.github/instructions/github-actions-ci-cd-best-practices.instructions.md)
