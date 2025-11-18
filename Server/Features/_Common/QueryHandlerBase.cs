// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Server.Features.Common;

/// <summary>
/// Base handler class that provides common dependencies for all query handlers.
/// Encapsulates infrastructure concerns (database, authorization, logging) for read-only operations.
/// Follows CQRS principles by separating query concerns from command concerns.
/// </summary>
public abstract class QueryHandlerBase(
    IDbContextFactory<MyModuleCommandContext> contextFactory,
    IUserPermissions userPermissions,
    ITenantManager tenantManager,
    IHttpContextAccessor httpContextAccessor,
    ILogManager logger)
{
    protected readonly IDbContextFactory<MyModuleCommandContext> ContextFactory = contextFactory;
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
    /// Creates and returns a new database context instance for read-only queries
    /// </summary>
    protected MyModuleCommandContext CreateDbContext() => ContextFactory.CreateDbContext();
}
