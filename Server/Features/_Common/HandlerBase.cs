// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Server.Features.Common;

/// <summary>
/// Base handler class that provides common dependencies for all command and query handlers.
/// Encapsulates infrastructure concerns (database, authorization, logging) used across vertical slices.
/// </summary>
/// <typeparam name="TContext">The type of database context (Query or Command)</typeparam>
public abstract class HandlerBase<TContext>(
    IDbContextFactory<TContext> contextFactory,
    IUserPermissions userPermissions,
    ITenantManager tenantManager,
    IHttpContextAccessor httpContextAccessor,
    ILogManager logger)
    where TContext : MyModuleContext
{
    protected readonly IDbContextFactory<TContext> ContextFactory = contextFactory;
    protected readonly IUserPermissions UserPermissions = userPermissions;
    protected readonly ITenantManager TenantManager = tenantManager;
    protected readonly IHttpContextAccessor HttpContextAccessor = httpContextAccessor;
    protected readonly ILogManager Logger = logger;

    /// <summary>
    /// Gets the current tenant alias from TenantManager
    /// </summary>
    protected Alias GetAlias() => TenantManager.GetAlias();

    /// <summary>
    /// Gets the current user from HttpContext
    /// </summary>
    protected ClaimsPrincipal? GetCurrentUser() => HttpContextAccessor.HttpContext?.User;

    /// <summary>
    /// Checks if the current user is authorized for the specified permission
    /// </summary>
    protected bool IsAuthorized(int siteId, int moduleId, string permission)
    {
        var user = GetCurrentUser();
        return user != null && UserPermissions.IsAuthorized(user, siteId, EntityNames.Module, moduleId, permission);
    }

    /// <summary>
    /// Creates and returns a new database context instance
    /// </summary>
    protected TContext CreateDbContext() => ContextFactory.CreateDbContext();
}
