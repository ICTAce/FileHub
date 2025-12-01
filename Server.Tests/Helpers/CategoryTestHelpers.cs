// Licensed to ICTAce under the MIT license.

using System.Diagnostics.CodeAnalysis;

namespace ICTAce.FileHub.Server.Tests.Helpers;

/// <summary>
/// Helper methods for Category entity testing.
/// Provides seeding, creation, and retrieval operations specific to Category.
/// </summary>
public static class CategoryTestHelpers
{
    #region Seeding Methods

    /// <summary>
    /// Seeds Category test data into the command context.
    /// </summary>
    public static async Task SeedCommandDataAsync(
        DbContextOptions<TestApplicationCommandContext> options,
        params Persistence.Entities.Category[] entities)
    {
        using var context = new TestApplicationCommandContext(options);
        context.Category.AddRange(entities);
        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Seeds Category test data into the query context.
    /// </summary>
    public static async Task SeedQueryDataAsync(
        DbContextOptions<TestApplicationQueryContext> options,
        params Persistence.Entities.Category[] entities)
    {
        using var context = new TestApplicationQueryContext(options);
        context.Category.AddRange(entities);
        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    #endregion

    #region Entity Creation

    /// <summary>
    /// Creates a test Category entity with default values.
    /// </summary>
    [SuppressMessage("Sonar", "S107:Methods should not have too many parameters", Justification = "Test helper method with many optional parameters")]
    public static Persistence.Entities.Category CreateTestEntity(
        int id = 1,
        int moduleId = 1,
        string name = "Test Category",
        int viewOrder = 1,
        int parentId = 0,
        string createdBy = "admin",
        DateTime? createdOn = null,
        string modifiedBy = "admin",
        DateTime? modifiedOn = null)
    {
        return new Persistence.Entities.Category
        {
            Id = id,
            ModuleId = moduleId,
            Name = name,
            ViewOrder = viewOrder,
            ParentId = parentId,
            CreatedBy = createdBy,
            CreatedOn = createdOn ?? DateTime.UtcNow,
            ModifiedBy = modifiedBy,
            ModifiedOn = modifiedOn ?? DateTime.UtcNow,
        };
    }

    #endregion

    #region Retrieval Methods

    /// <summary>
    /// Gets a Category entity from the command database by ID.
    /// </summary>
    public static async Task<Persistence.Entities.Category?> GetFromCommandDbAsync(
        DbContextOptions<TestApplicationCommandContext> options,
        int id)
    {
        using var context = new TestApplicationCommandContext(options);
        return await context.Category.FindAsync(id).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets a Category entity from the query database by ID.
    /// </summary>
    public static async Task<Persistence.Entities.Category?> GetFromQueryDbAsync(
        DbContextOptions<TestApplicationQueryContext> options,
        int id)
    {
        using var context = new TestApplicationQueryContext(options);
        return await context.Category.FindAsync(id).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the count of Category entities in the command database.
    /// </summary>
    public static async Task<int> GetCountFromCommandDbAsync(
        DbContextOptions<TestApplicationCommandContext> options)
    {
        using var context = new TestApplicationCommandContext(options);
        return await context.Category.CountAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the count of Category entities in the query database.
    /// </summary>
    public static async Task<int> GetCountFromQueryDbAsync(
        DbContextOptions<TestApplicationQueryContext> options)
    {
        using var context = new TestApplicationQueryContext(options);
        return await context.Category.CountAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Gets all categories from the command database ordered by ViewOrder.
    /// </summary>
    public static async Task<IReadOnlyList<Persistence.Entities.Category>> GetAllFromCommandDbAsync(
        DbContextOptions<TestApplicationCommandContext> options,
        int? moduleId = null)
    {
        using var context = new TestApplicationCommandContext(options);
        var query = context.Category.AsQueryable();
        
        if (moduleId.HasValue)
        {
            query = query.Where(c => c.ModuleId == moduleId.Value);
        }
        
        return await query.OrderBy(c => c.ViewOrder).ThenBy(c => c.Name).ToListAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Gets all categories from the query database ordered by ViewOrder.
    /// </summary>
    public static async Task<IReadOnlyList<Persistence.Entities.Category>> GetAllFromQueryDbAsync(
        DbContextOptions<TestApplicationQueryContext> options,
        int? moduleId = null)
    {
        using var context = new TestApplicationQueryContext(options);
        var query = context.Category.AsQueryable();
        
        if (moduleId.HasValue)
        {
            query = query.Where(c => c.ModuleId == moduleId.Value);
        }
        
        return await query.OrderBy(c => c.ViewOrder).ThenBy(c => c.Name).ToListAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Gets child categories for a specific parent from the query database.
    /// </summary>
    public static async Task<IReadOnlyList<Persistence.Entities.Category>> GetChildrenFromQueryDbAsync(
        DbContextOptions<TestApplicationQueryContext> options,
        int parentId)
    {
        using var context = new TestApplicationQueryContext(options);
        return await context.Category
            .Where(c => c.ParentId == parentId)
            .OrderBy(c => c.ViewOrder)
            .ThenBy(c => c.Name)
            .ToListAsync()
            .ConfigureAwait(false);
    }

    #endregion
}
