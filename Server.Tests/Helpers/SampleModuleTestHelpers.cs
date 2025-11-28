// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Server.Tests.Helpers;

/// <summary>
/// Helper methods for SampleModule entity testing.
/// Provides seeding, creation, and retrieval operations specific to SampleModule.
/// </summary>
public static class SampleModuleTestHelpers
{
    #region Seeding Methods

    /// <summary>
    /// Seeds SampleModule test data into the command context.
    /// </summary>
    public static async Task SeedCommandDataAsync(
        DbContextOptions<TestApplicationCommandContext> options,
        params Persistence.Entities.SampleModule[] entities)
    {
        using var context = new TestApplicationCommandContext(options);
        context.SampleModule.AddRange(entities);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Seeds SampleModule test data into the query context.
    /// </summary>
    public static async Task SeedQueryDataAsync(
        DbContextOptions<TestApplicationQueryContext> options,
        params Persistence.Entities.SampleModule[] entities)
    {
        using var context = new TestApplicationQueryContext(options);
        context.SampleModule.AddRange(entities);
        await context.SaveChangesAsync();
    }

    #endregion

    #region Entity Creation

    /// <summary>
    /// Creates a test SampleModule entity with default values.
    /// </summary>
    public static Persistence.Entities.SampleModule CreateTestEntity(
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

    #endregion

    #region Retrieval Methods

    /// <summary>
    /// Gets a SampleModule entity from the command database by ID.
    /// </summary>
    public static async Task<Persistence.Entities.SampleModule?> GetFromCommandDbAsync(
        DbContextOptions<TestApplicationCommandContext> options,
        int id)
    {
        using var context = new TestApplicationCommandContext(options);
        return await context.SampleModule.FindAsync(id);
    }

    /// <summary>
    /// Gets a SampleModule entity from the query database by ID.
    /// </summary>
    public static async Task<Persistence.Entities.SampleModule?> GetFromQueryDbAsync(
        DbContextOptions<TestApplicationQueryContext> options,
        int id)
    {
        using var context = new TestApplicationQueryContext(options);
        return await context.SampleModule.FindAsync(id);
    }

    /// <summary>
    /// Gets the count of SampleModule entities in the command database.
    /// </summary>
    public static async Task<int> GetCountFromCommandDbAsync(
        DbContextOptions<TestApplicationCommandContext> options)
    {
        using var context = new TestApplicationCommandContext(options);
        return await context.SampleModule.CountAsync();
    }

    /// <summary>
    /// Gets the count of SampleModule entities in the query database.
    /// </summary>
    public static async Task<int> GetCountFromQueryDbAsync(
        DbContextOptions<TestApplicationQueryContext> options)
    {
        using var context = new TestApplicationQueryContext(options);
        return await context.SampleModule.CountAsync();
    }

    #endregion
}
