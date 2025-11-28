// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Common;

public abstract class HandlerBase<TContext>
    where TContext : ApplicationContext
{
    protected readonly IDbContextFactory<TContext> ContextFactory;
    protected readonly IUserPermissions UserPermissions;
    protected readonly ITenantManager TenantManager;
    protected readonly IHttpContextAccessor HttpContextAccessor;
    protected readonly ILogManager Logger;

    // New pattern: Generic service parameter (recommended)
    protected HandlerBase(HandlerServices<TContext> services)
    {
        ContextFactory = services.ContextFactory;
        UserPermissions = services.UserPermissions;
        TenantManager = services.TenantManager;
        HttpContextAccessor = services.HttpContextAccessor;
        Logger = services.Logger;
    }

    // Legacy constructor for backward compatibility
    protected HandlerBase(
        IDbContextFactory<TContext> contextFactory,
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

    protected Alias GetAlias() => TenantManager.GetAlias();

    protected ClaimsPrincipal? GetCurrentUser() => HttpContextAccessor.HttpContext?.User;

    protected bool IsAuthorized(int siteId, int moduleId, string permission)
    {
        var user = GetCurrentUser();
        return user != null && UserPermissions.IsAuthorized(user, siteId, EntityNames.Module, moduleId, permission);
    }

    protected TContext CreateDbContext() => ContextFactory.CreateDbContext();
}
