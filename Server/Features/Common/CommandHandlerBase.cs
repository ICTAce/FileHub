namespace ICTAce.FileHub.Features.Common;

/// <summary>
/// Base handler class that provides common dependencies for all command and query handlers.
/// Encapsulates infrastructure concerns (database, authorization, logging) used across vertical slices.
/// </summary>
public abstract class CommandHandlerBase
{
    protected readonly IDbContextFactory<Context> ContextFactory;
    protected readonly IUserPermissions UserPermissions;
    protected readonly ITenantManager TenantManager;
    protected readonly IHttpContextAccessor HttpContextAccessor;
    protected readonly ILogManager Logger;

    protected CommandHandlerBase(
        IDbContextFactory<Context> contextFactory,
        IUserPermissions userPermissions,
        ITenantManager tenantManager,
        IHttpContextAccessor httpContextAccessor,
        ILogManager logger)
    {
        ContextFactory = contextFactory;
        UserPermissions = userPermissions;
        TenantManager = tenantManager;
        HttpContextAccessor = httpContextAccessor;
        Logger = logger;
    }

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
    protected Context CreateDbContext() => ContextFactory.CreateDbContext();
}
