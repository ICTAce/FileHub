// Licensed to ICTAce under the MIT license.

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Oqtane.Repository;

namespace ICTAce.FileHub.Server.Tests.Helpers;

/// <summary>
/// Mock implementation of IDBContextDependencies for testing purposes
/// </summary>
public class TestDBContextDependencies : IDBContextDependencies
{
    private readonly DbContextOptions _options;

    public TestDBContextDependencies(DbContextOptions options)
    {
        _options = options;

        // Create mock tenant resolver that returns a test tenant
        var mockTenantResolver = Substitute.For<ITenantResolver>();
        var testTenant = new Oqtane.Models.Tenant
        {
            TenantId = 1,
            Name = "Test"
        };
        mockTenantResolver.GetTenant().Returns(testTenant);
        TenantResolver = mockTenantResolver;

        // Create mock tenant manager
        var mockTenantManager = Substitute.For<ITenantManager>();
        var testAlias = new Oqtane.Models.Alias
        {
            TenantId = 1,
            SiteId = 1,
            Name = "Test"
        };
        mockTenantManager.GetAlias().Returns(testAlias);
        TenantManager = mockTenantManager;

        // Create a simple database manager for testing
        DatabaseManager = Substitute.For<IDatabaseManager>();

        // Create mock HTTP context accessor
        Accessor = Substitute.For<IHttpContextAccessor>();

        // Create mock configuration root
        Config = Substitute.For<IConfigurationRoot>();
    }

    public DbContextOptions Options => _options;
    public ITenantResolver TenantResolver { get; }
    public ITenantManager TenantManager { get; }
    public IDatabaseManager DatabaseManager { get; }
    public IHttpContextAccessor Accessor { get; }
    public IConfigurationRoot Config { get; }
}

/// <summary>
/// Test-friendly version of ApplicationCommandContext that inherits from the real ApplicationCommandContext.
/// This allows proper type compatibility with handlers while using SQLite in-memory database for testing.
/// </summary>
public class TestApplicationCommandContext : ApplicationCommandContext
{
    private readonly SqliteConnection? _connection;
    private static readonly IServiceProvider _serviceProvider;

    static TestApplicationCommandContext()
    {
        // Create a service provider with all necessary EF Core services
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));
        services.AddEntityFrameworkSqlite();
        _serviceProvider = services.BuildServiceProvider();
    }

    public TestApplicationCommandContext(DbContextOptions options)
        : base(new TestDBContextDependencies(options))
    {
        // Extract the connection from options if it's a SQLite connection
        if (options is DbContextOptions<TestApplicationCommandContext> typedOptions)
        {
            var extension = typedOptions.FindExtension<Microsoft.EntityFrameworkCore.Sqlite.Infrastructure.Internal.SqliteOptionsExtension>();
            _connection = extension?.Connection as SqliteConnection;
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Configure SQLite in-memory database
        if (!optionsBuilder.IsConfigured && _connection != null)
        {
            optionsBuilder
                .UseSqlite(_connection)
                .UseInternalServiceProvider(_serviceProvider);
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // DON'T call base.OnModelCreating() because it tries to use ActiveDatabase which is null for SQLite tests
        // Instead, manually configure Identity entities

        // Configure our entities with simple table names (no tenant rewriting for tests)
        builder.Entity<Persistence.Entities.SampleModule>().ToTable("Company_SampleModule");
        builder.Entity<Persistence.Entities.Category>().ToTable("FileHub_Category");

        // Ignore Identity entities that we don't need for these tests
        builder.Ignore<IdentityUser>();
        builder.Ignore<IdentityRole>();
        builder.Ignore<IdentityUserClaim<string>>();
        builder.Ignore<IdentityUserRole<string>>();
        builder.Ignore<IdentityUserLogin<string>>();
        builder.Ignore<IdentityRoleClaim<string>>();
        builder.Ignore<IdentityUserToken<string>>();
    }

    // Override SaveChanges to skip audit field modifications for tests
    public override int SaveChanges()
    {
        // Skip DBContextBase.SaveChanges() which tries to set audit fields from HTTP context
        // Just call the base EF Core SaveChanges directly
        return base.SaveChanges(acceptAllChangesOnSuccess: true);
    }

    // Override SaveChangesAsync to skip audit field modifications for tests
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Skip DBContextBase.SaveChangesAsync() which tries to set audit fields from HTTP context
        // Just call the base EF Core SaveChangesAsync directly
        return base.SaveChangesAsync(acceptAllChangesOnSuccess: true, cancellationToken);
    }
}

/// <summary>
/// Test-friendly version of ApplicationQueryContext that inherits from the real ApplicationQueryContext.
/// This allows proper type compatibility with handlers while using SQLite in-memory database for testing.
/// </summary>
public class TestApplicationQueryContext : ApplicationQueryContext
{
    private readonly SqliteConnection? _connection;
    private static readonly IServiceProvider _serviceProvider;

    static TestApplicationQueryContext()
    {
        // Create a service provider with all necessary EF Core services
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));
        services.AddEntityFrameworkSqlite();
        _serviceProvider = services.BuildServiceProvider();
    }

    public TestApplicationQueryContext(DbContextOptions options)
        : base(new TestDBContextDependencies(options))
    {
        // Extract the connection from options if it's a SQLite connection
        if (options is DbContextOptions<TestApplicationQueryContext> typedOptions)
        {
            var extension = typedOptions.FindExtension<Microsoft.EntityFrameworkCore.Sqlite.Infrastructure.Internal.SqliteOptionsExtension>();
            _connection = extension?.Connection as SqliteConnection;
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Configure SQLite in-memory database
        if (!optionsBuilder.IsConfigured && _connection != null)
        {
            optionsBuilder
                .UseSqlite(_connection)
                .UseInternalServiceProvider(_serviceProvider);
        }
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // DON'T call base.OnModelCreating() because it tries to use ActiveDatabase which is null for SQLite tests
        // Instead, manually configure Identity entities

        // Configure our entities with simple table names (no tenant rewriting for tests)
        builder.Entity<Persistence.Entities.SampleModule>().ToTable("Company_SampleModule");
        builder.Entity<Persistence.Entities.Category>().ToTable("FileHub_Category");

        // Ignore Identity entities that we don't need for these tests
        builder.Ignore<IdentityUser>();
        builder.Ignore<IdentityRole>();
        builder.Ignore<IdentityUserClaim<string>>();
        builder.Ignore<IdentityUserRole<string>>();
        builder.Ignore<IdentityUserLogin<string>>();
        builder.Ignore<IdentityRoleClaim<string>>();
        builder.Ignore<IdentityUserToken<string>>();
    }

    // Override SaveChanges to skip audit field modifications for tests
    public override int SaveChanges()
    {
        // Skip DBContextBase.SaveChanges() which tries to set audit fields from HTTP context
        // Just call the base EF Core SaveChanges directly
        return base.SaveChanges(acceptAllChangesOnSuccess: true);
    }

    // Override SaveChangesAsync to skip audit field modifications for tests
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Skip DBContextBase.SaveChangesAsync() which tries to set audit fields from HTTP context
        // Just call the base EF Core SaveChangesAsync directly
        return base.SaveChangesAsync(acceptAllChangesOnSuccess: true, cancellationToken);
    }
}
