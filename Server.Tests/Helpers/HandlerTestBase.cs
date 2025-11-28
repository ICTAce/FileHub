// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Server.Tests.Helpers;

/// <summary>
/// Base class for handler tests providing common test infrastructure.
/// Entity-agnostic - handles only databases, mocks, and disposal.
/// Use entity-specific helper classes (SampleModuleTestHelpers, CategoryTestHelpers) for entity operations.
/// </summary>
public abstract class HandlerTestBase : IDisposable
{
    private SqliteConnection? _connection;
    private DbContextOptions<TestApplicationCommandContext>? _commandOptions;
    private DbContextOptions<TestApplicationQueryContext>? _queryOptions;
    private bool _disposed;

    #region Database Creation

    /// <summary>
    /// Creates and configures a SQLite in-memory database for command operations.
    /// The database schema is automatically created.
    /// </summary>
    protected async Task<(SqliteConnection connection, DbContextOptions<TestApplicationCommandContext> options)> CreateCommandDatabaseAsync()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _commandOptions = new DbContextOptionsBuilder<TestApplicationCommandContext>()
            .UseSqlite(_connection)
            .Options;

        // Ensure database schema is created
        using var context = new TestApplicationCommandContext(_commandOptions);
        await context.Database.EnsureCreatedAsync();

        return (_connection, _commandOptions);
    }

    /// <summary>
    /// Creates and configures a SQLite in-memory database for query operations.
    /// The database schema is automatically created.
    /// </summary>
    protected async Task<(SqliteConnection connection, DbContextOptions<TestApplicationQueryContext> options)> CreateQueryDatabaseAsync()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _queryOptions = new DbContextOptionsBuilder<TestApplicationQueryContext>()
            .UseSqlite(_connection)
            .Options;

        // Ensure database schema is created
        using var context = new TestApplicationQueryContext(_queryOptions);
        await context.Database.EnsureCreatedAsync();

        return (_connection, _queryOptions);
    }

    #endregion

    #region Context Factory Creation

    /// <summary>
    /// Creates a mock IDbContextFactory that returns TestApplicationCommandContext instances.
    /// </summary>
    protected IDbContextFactory<ApplicationCommandContext> CreateMockCommandContextFactory(
        DbContextOptions<TestApplicationCommandContext> options)
    {
        var mockFactory = Substitute.For<IDbContextFactory<ApplicationCommandContext>>();
        mockFactory.CreateDbContext().Returns(_ => new TestApplicationCommandContext(options));
        return mockFactory;
    }

    /// <summary>
    /// Creates a mock IDbContextFactory that returns TestApplicationQueryContext instances.
    /// </summary>
    protected IDbContextFactory<ApplicationQueryContext> CreateMockQueryContextFactory(
        DbContextOptions<TestApplicationQueryContext> options)
    {
        var mockFactory = Substitute.For<IDbContextFactory<ApplicationQueryContext>>();
        mockFactory.CreateDbContext().Returns(_ => new TestApplicationQueryContext(options));
        return mockFactory;
    }

    #endregion

    #region Mock Creation

    /// <summary>
    /// Creates a mock IUserPermissions with configurable authorization.
    /// </summary>
    /// <param name="isAuthorized">Whether the user should be authorized (default: true)</param>
    protected IUserPermissions CreateMockUserPermissions(bool isAuthorized = true)
    {
        var mockUserPermissions = Substitute.For<IUserPermissions>();
        mockUserPermissions.IsAuthorized(
            Arg.Any<ClaimsPrincipal>(),
            Arg.Any<int>(),
            Arg.Any<string>(),
            Arg.Any<int>(),
            Arg.Any<string>()).Returns(isAuthorized);
        return mockUserPermissions;
    }

    /// <summary>
    /// Creates a mock ITenantManager with a test alias.
    /// </summary>
    /// <param name="siteId">The site ID for the test alias (default: 1)</param>
    /// <param name="aliasName">The alias name (default: "Test")</param>
    protected ITenantManager CreateMockTenantManager(int siteId = 1, string aliasName = "Test")
    {
        var mockTenantManager = Substitute.For<ITenantManager>();
        mockTenantManager.GetAlias().Returns(new Alias { SiteId = siteId, Name = aliasName });
        return mockTenantManager;
    }

    /// <summary>
    /// Creates a mock IHttpContextAccessor with a test claims principal.
    /// </summary>
    protected IHttpContextAccessor CreateMockHttpContextAccessor()
    {
        return TestHelpers.CreateMockHttpContextAccessor(new ClaimsPrincipal());
    }

    /// <summary>
    /// Creates a mock ILogManager for test logging.
    /// </summary>
    protected ILogManager CreateMockLogger()
    {
        return Substitute.For<ILogManager>();
    }

    #endregion

    #region Disposal

    /// <summary>
    /// Disposes of test resources including database connections.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _connection?.Close();
                _connection?.Dispose();
            }
            _disposed = true;
        }
    }

    /// <summary>
    /// Disposes of test resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
