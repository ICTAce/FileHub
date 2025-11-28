// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Server.Tests.Helpers;

/// <summary>
/// Base class for handler tests providing common test infrastructure and helper methods.
/// Handles SQLite in-memory database setup, mock creation, and test data seeding.
/// </summary>
public abstract class HandlerTestBase : IDisposable
{
    private SqliteConnection? _connection;
    private DbContextOptions<TestApplicationCommandContext>? _commandOptions;
    private DbContextOptions<TestApplicationQueryContext>? _queryOptions;
    private bool _disposed;

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

    /// <summary>
    /// Creates a mock IDbContextFactory that returns TestApplicationCommandContext instances.
    /// </summary>
    protected IDbContextFactory<ApplicationCommandContext> CreateMockCommandContextFactory(DbContextOptions<TestApplicationCommandContext> options)
    {
        var mockFactory = Substitute.For<IDbContextFactory<ApplicationCommandContext>>();
        mockFactory.CreateDbContext().Returns(_ => new TestApplicationCommandContext(options));
        return mockFactory;
    }

    /// <summary>
    /// Creates a mock IDbContextFactory that returns TestApplicationQueryContext instances.
    /// </summary>
    protected IDbContextFactory<ApplicationQueryContext> CreateMockQueryContextFactory(DbContextOptions<TestApplicationQueryContext> options)
    {
        var mockFactory = Substitute.For<IDbContextFactory<ApplicationQueryContext>>();
        mockFactory.CreateDbContext().Returns(_ => new TestApplicationQueryContext(options));
        return mockFactory;
    }

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

    /// <summary>
    /// Seeds test data into the command context.
    /// </summary>
    protected async Task SeedCommandDataAsync(
        DbContextOptions<TestApplicationCommandContext> options,
        params Persistence.Entities.SampleModule[] entities)
    {
        using var context = new TestApplicationCommandContext(options);
        context.SampleModule.AddRange(entities);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Seeds test data into the query context.
    /// </summary>
    protected async Task SeedQueryDataAsync(
        DbContextOptions<TestApplicationQueryContext> options,
        params Persistence.Entities.SampleModule[] entities)
    {
        using var context = new TestApplicationQueryContext(options);
        context.SampleModule.AddRange(entities);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Creates a test SampleModule entity with default values.
    /// </summary>
    protected Persistence.Entities.SampleModule CreateTestEntity(
        int id = 1,
        int moduleId = 1,
        string name = "Test Module",
        string createdBy = "admin",
        DateTime? createdOn = null,
        string modifiedBy = "admin",
        DateTime? modifiedOn = null)
    {
        return new Persistence.Entities.SampleModule
        {
            Id = id,
            ModuleId = moduleId,
            Name = name,
            CreatedBy = createdBy,
            CreatedOn = createdOn ?? DateTime.UtcNow,
            ModifiedBy = modifiedBy,
            ModifiedOn = modifiedOn ?? DateTime.UtcNow
        };
    }

    /// <summary>
    /// Verifies that an entity exists in the command database.
    /// </summary>
    protected async Task<Persistence.Entities.SampleModule?> GetEntityFromCommandDbAsync(
        DbContextOptions<TestApplicationCommandContext> options,
        int id)
    {
        using var context = new TestApplicationCommandContext(options);
        return await context.SampleModule.FindAsync(id);
    }

    /// <summary>
    /// Verifies that an entity exists in the query database.
    /// </summary>
    protected async Task<Persistence.Entities.SampleModule?> GetEntityFromQueryDbAsync(
        DbContextOptions<TestApplicationQueryContext> options,
        int id)
    {
        using var context = new TestApplicationQueryContext(options);
        return await context.SampleModule.FindAsync(id);
    }

    /// <summary>
    /// Gets the count of entities in the command database.
    /// </summary>
    protected async Task<int> GetCommandEntityCountAsync(DbContextOptions<TestApplicationCommandContext> options)
    {
        using var context = new TestApplicationCommandContext(options);
        return await context.SampleModule.CountAsync();
    }

    /// <summary>
    /// Gets the count of entities in the query database.
    /// </summary>
    protected async Task<int> GetQueryEntityCountAsync(DbContextOptions<TestApplicationQueryContext> options)
    {
        using var context = new TestApplicationQueryContext(options);
        return await context.SampleModule.CountAsync();
    }

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
}
