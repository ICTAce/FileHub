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

    /// <summary>
    /// Generic handler for creating entities with authorization and logging.
    /// </summary>
    /// <typeparam name="TRequest">The request type containing the data (must have ModuleId property)</typeparam>
    /// <typeparam name="TEntity">The entity type to create (must inherit from AuditableBase)</typeparam>
    /// <param name="request">The request containing the data and ModuleId</param>
    /// <param name="mapToEntity">Mapper function to convert request to entity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created entity ID on success, -1 on authorization failure</returns>
    protected async Task<int> HandleCreateAsync<TRequest, TEntity>(
        TRequest request,
        Func<TRequest, TEntity> mapToEntity,
        CancellationToken cancellationToken = default)
        where TRequest : RequestBase
        where TEntity : AuditableBase
    {
        var alias = GetAlias();

        if (IsAuthorized(alias.SiteId, request.ModuleId, PermissionNames.Edit))
        {
            var entity = mapToEntity(request);

            using var db = CreateDbContext();
            db.Set<TEntity>().Add(entity);
            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            Logger.Log(LogLevel.Information, this, LogFunction.Create, "{EntityName} Added {Entity}", typeof(TEntity).Name, entity);
            return entity.Id;
        }

        Logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized {EntityName} Add Attempt {ModuleId}", typeof(TEntity).Name, request.ModuleId);
        return -1;
    }
}
