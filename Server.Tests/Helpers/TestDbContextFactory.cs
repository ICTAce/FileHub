// Licensed to ICTAce under the MIT license.

using Microsoft.EntityFrameworkCore;

namespace ICTAce.FileHub.Server.Tests.Helpers;

/// <summary>
/// Test factory that creates test DB contexts for unit testing handlers
/// </summary>
public class TestDbContextFactory<TContext> : IDbContextFactory<TContext> where TContext : DbContext
{
    private readonly DbContextOptions _options;
    private readonly Func<DbContextOptions, TContext> _contextFactory;

    public TestDbContextFactory(DbContextOptions options, Func<DbContextOptions, TContext> contextFactory)
    {
        _options = options;
        _contextFactory = contextFactory;
    }

    public TContext CreateDbContext()
    {
        return _contextFactory(_options);
    }
}
