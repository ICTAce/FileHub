// Licensed to ICTAce under the MIT license.

namespace ICTAce.FileHub.Features.Common;

public abstract class HandlerBase<TContext>(
    IDbContextFactory<TContext> contextFactory,
    IUserPermissions userPermissions,
    ITenantManager tenantManager,
    IHttpContextAccessor httpContextAccessor,
    ILogManager logger)
    where TContext : ApplicationContext
{
    protected readonly IDbContextFactory<TContext> ContextFactory = contextFactory;
    protected readonly IUserPermissions UserPermissions = userPermissions;
    protected readonly ITenantManager TenantManager = tenantManager;
    protected readonly IHttpContextAccessor HttpContextAccessor = httpContextAccessor;
    protected readonly ILogManager Logger = logger;

    protected Alias GetAlias() => TenantManager.GetAlias();

    protected ClaimsPrincipal? GetCurrentUser() => HttpContextAccessor.HttpContext?.User;

    protected bool IsAuthorized(int siteId, int moduleId, string permission)
    {
        var user = GetCurrentUser();
        return user != null && UserPermissions.IsAuthorized(user, siteId, EntityNames.Module, moduleId, permission);
    }

    protected TContext CreateDbContext() => ContextFactory.CreateDbContext();
}
